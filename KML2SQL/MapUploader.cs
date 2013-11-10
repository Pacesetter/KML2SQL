using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Diagnostics;

namespace KML2SQL
{
    public class MapUploader : INotifyPropertyChanged
    {
        string connectionString;
        string placemarkColumnName;
        string fileLocation;
        string tableName;
        bool geographyMode;
        int srid;
        Kml kml;
        string sqlGeoType;
        BackgroundWorker worker;
        List<MapFeature> mapFeatures = new List<MapFeature>();
        List<string> columnNames = new List<string>();

        private string progress = "";

        public string Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                this.OnPropertyChanged("Progress");
            }
        }

        public MapUploader(string serverName, string databaseName, string username, string password, string columnName, string fileLocation, string tableName, int srid, bool geographyMode)
        {
            connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;
            this.placemarkColumnName = columnName;
            this.fileLocation = fileLocation;
            this.tableName = tableName;
            this.geographyMode = geographyMode;
            this.srid = srid;
            kml = KMLParser.Parse(fileLocation);
            sqlGeoType = geographyMode == true ? "geography" : "geometry";
            initializeBackgroundWorker();
            foreach (MapFeature mapFeature in enumerablePlacemarks(kml))
            {
                mapFeatures.Add(mapFeature);
                foreach (KeyValuePair<string, string> pair in mapFeature.Data)
                    if (!columnNames.Contains(pair.Key))
                        if (pair.Key.ToLower() != "id")
                            columnNames.Add(pair.Key);

            }
        }

        private void initializeBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(bw_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerComleted);
            worker.WorkerSupportsCancellation = true;
        }

        public void Upload()
        {
#if !DEBUG
            worker.RunWorkerAsync();
#else
            DoWork();
#endif
        }

        private void DoWork()
        {
            try
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    dropTable(connection);
                    createTable(connection);
                    foreach (MapFeature mapFeature in mapFeatures)
                    {
                        SqlCommand command = MsSqlCommandCreator.createCommand(mapFeature, geographyMode, srid, tableName, placemarkColumnName, connection);
                        command.ExecuteNonQuery();
                        worker.ReportProgress(0, "Uploading Placemark # " + mapFeature.Id.ToString());
                    }
                    worker.ReportProgress(0, "Done!");
                }
            }
            catch (Exception ex)
            {
                worker.ReportProgress(0, ex.Message);
                worker.CancelAsync();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWork();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.UserState.ToString();
        }

        private void bw_RunWorkerComleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (!worker.CancellationPending)
            //    Progress = "Done!";
        }

        private IEnumerable<MapFeature> enumerablePlacemarks(Kml kml)
        {
            int Id = 1;
            foreach (var placemark in kml.Flatten().OfType<Placemark>())
            {
                MapFeature mapFeature = new MapFeature(placemark, Id);
                Id++;
                yield return mapFeature;
            }
        }
        

        private void dropTable(System.Data.SqlClient.SqlConnection connection)
        {
            try
            {
                string dropCommandString = "DROP TABLE " + tableName + ";";
                var dropCommand = new System.Data.SqlClient.SqlCommand(dropCommandString, connection);
                dropCommand.CommandType = System.Data.CommandType.Text;
                dropCommand.ExecuteNonQuery();
                worker.ReportProgress(0, "Existing Table Dropped");
            }
            catch
            {
                worker.ReportProgress(0, "No table found to drop");
            }
        }

        private void createTable(System.Data.SqlClient.SqlConnection connection)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE [" + tableName + "] (");
            sb.Append("[Id] INT NOT NULL PRIMARY KEY,");
            if (columnNames.Count > 0)
            {
                foreach (string columnName in columnNames)
                    sb.Append("[" + columnName + "] VARCHAR(max), ");
            }
            sb.Append("[" + placemarkColumnName + "] [sys].[" + sqlGeoType + "] NOT NULL, );");
            var command = new System.Data.SqlClient.SqlCommand(sb.ToString(), connection);
            command.CommandType = System.Data.CommandType.Text;
            command.ExecuteNonQuery();
            worker.ReportProgress(0, "Table Created");       
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
