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
using System.IO;

namespace KML2SQL
{
    public class MapUploader : INotifyPropertyChanged
    {
        readonly string _connectionString, _placemarkColumnName, _tableName, _sqlGeoType, _logFile;
        readonly StringBuilder _log;
        readonly bool _geographyMode;
        readonly int _srid;
        BackgroundWorker _worker;
        readonly List<MapFeature> _mapFeatures = new List<MapFeature>();
        readonly List<string> _columnNames = new List<string>();
        private string _progress = "";

        public string Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                this.OnPropertyChanged("Progress");
                _log.Append(value + Environment.NewLine);
            }
        }

        public MapUploader(string connectionString, string columnName, string fileLocation, string tableName, int srid, 
            bool geographyMode, StringBuilder log, string logFile)
        {
            _connectionString = connectionString;
            _placemarkColumnName = columnName;
            _tableName = tableName;
            _geographyMode = geographyMode;
            _srid = srid;
            _log = log;
            _logFile = logFile;
            _sqlGeoType = geographyMode == true ? "geography" : "geometry";
            Kml kml = KMLParser.Parse(fileLocation);
            InitializeMapFeatures(kml);
            InitializeBackgroundWorker();
            UsageReporter.Report("MapUploader Process Started", false);
        }

        private void InitializeMapFeatures(Kml kml)
        {
            foreach (MapFeature mapFeature in EnumerablePlacemarks(kml))
            {
                _mapFeatures.Add(mapFeature);
                foreach (KeyValuePair<string, string> pair in mapFeature.Data)
                    if (!_columnNames.Contains(pair.Key) && pair.Key.ToLower() != "id")
                        _columnNames.Add(pair.Key);
            }
        }

        private void InitializeBackgroundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(bw_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerComleted);
            _worker.WorkerSupportsCancellation = true;
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
                using (var connection = new System.Data.SqlClient.SqlConnection(_connectionString))
                {
                    connection.Open();
                    DropTable(connection);
                    CreateTable(connection);
                    foreach (MapFeature mapFeature in _mapFeatures)
                    {
                        SqlCommand command = MsSqlCommandCreator.CreateCommand(mapFeature, _geographyMode, _srid, _tableName, _placemarkColumnName, connection);
                        command.ExecuteNonQuery();
                        _worker.ReportProgress(0, "Uploading Placemark # " + mapFeature.Id.ToString());
                    }
                    _worker.ReportProgress(0, "Done!");
                }
            }
            catch (Exception ex)
            {
                _worker.ReportProgress(0, ex.Message);
                _worker.CancelAsync();
                UsageReporter.Report(ex.Message, true);
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
            using (var writer = new StreamWriter(_logFile, true))
            {
                if (_log != null)
                    writer.Write(_log);
            }
        }

        private static IEnumerable<MapFeature> EnumerablePlacemarks(Kml kml)
        {
            int id = 1;
            foreach (var placemark in kml.Flatten().OfType<Placemark>())
            {
                MapFeature mapFeature = new MapFeature(placemark, id);
                id++;
                yield return mapFeature;
            }
        }
        

        private void DropTable(SqlConnection connection)
        {
            try
            {
                string dropCommandString = "DROP TABLE " + _tableName + ";";
                var dropCommand = new System.Data.SqlClient.SqlCommand(dropCommandString, connection);
                dropCommand.CommandType = System.Data.CommandType.Text;
                dropCommand.ExecuteNonQuery();
                _worker.ReportProgress(0, "Existing Table Dropped");
            }
            catch
            {
                _worker.ReportProgress(0, "No table found to drop");
            }
        }

        private void CreateTable(SqlConnection connection)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE [" + _tableName + "] (");
            sb.Append("[Id] INT NOT NULL PRIMARY KEY,");
            if (_columnNames.Count > 0)
            {
                foreach (string columnName in _columnNames)
                    sb.Append("[" + columnName + "] VARCHAR(max), ");
            }
            sb.Append("[" + _placemarkColumnName + "] [sys].[" + _sqlGeoType + "] NOT NULL, );");
            var command = new System.Data.SqlClient.SqlCommand(sb.ToString(), connection);
            command.CommandType = System.Data.CommandType.Text;
            command.ExecuteNonQuery();
            _worker.ReportProgress(0, "Table Created");       
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
