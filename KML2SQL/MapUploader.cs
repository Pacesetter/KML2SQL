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

namespace KML2SQL
{
    public class MapUploader : INotifyPropertyChanged
    {
        string connectionString;
        string columnName;
        string fileLocation;
        string tableName;
        bool geographyMode;
        int srid;
        Kml kml;
        string sqlGeoType;
        BackgroundWorker worker;

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
            this.columnName = columnName;
            this.fileLocation = fileLocation;
            this.tableName = tableName;
            this.geographyMode = geographyMode;
            this.srid = srid;
            kml = KMLParser.Parse(fileLocation);
            sqlGeoType = geographyMode == true ? "geography" : "geometry";
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(bw_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerComleted);
        }

        public void Upload()
        {
#if DEBUG
            DoWork();
#endif
            worker.RunWorkerAsync();
        }

        private void DoWork()
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                dropTable(connection);
                createTable(connection);
                foreach (MapFeature mapFeature in enumerablePlacemarks(kml))
                {
                    string commandString = CommandStringCreator.setCommandString(mapFeature, geographyMode, srid, tableName);
                    var command = new SqlCommand(commandString, connection);
                    command.CommandType = System.Data.CommandType.Text;
                    command.ExecuteNonQuery();
                    worker.ReportProgress(0, "Uploading Placemark # " + mapFeature.ID.ToString());
                }
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
            Progress = "Done!";
        }

        private IEnumerable<MapFeature> enumerablePlacemarks(Kml kml)
        {
            int ID = 1;
            foreach (var placemark in kml.Flatten().OfType<Placemark>())
            {
                MapFeature mapFeature = new MapFeature();
                if (placemark.Name != null)
                    mapFeature.Name = placemark.Name;
                else
                    mapFeature.Name = ID.ToString();
                mapFeature.ID = ID;
                mapFeature.Type = getPlacemarkType(placemark);
                switch (mapFeature.Type)
                {
                    case PlacemarkType.Line:
                        mapFeature.Coordinates = getLineCoordinates(placemark);
                        break;
                    case PlacemarkType.Point:
                        mapFeature.Coordinates = getPointCoordinates(placemark);
                        break;
                    case PlacemarkType.Polygon:
                        mapFeature.Coordinates = getPolygonCoordinates(placemark);
                        break;
                }
                ID++;
                yield return mapFeature;
            }
        }

        private string getPointCoordinates(Placemark placemark)
        {
            StringBuilder sb = new StringBuilder();
            Point myPoint;
            foreach (var element in placemark.Flatten())
            {
                if (element is Point)
                {
                    myPoint = (Point)element;
                    sb.Append(myPoint.Coordinate.Longitude + " " + myPoint.Coordinate.Latitude);
                }
            }
            return sb.ToString();
        }

        private string getLineCoordinates(Placemark placemark)
        {
            StringBuilder sb = new StringBuilder();
            LineString lineString;
            foreach (var element in placemark.Flatten())
            {
                if (element is LineString)
                {
                    lineString = (LineString)element;
                    foreach (var vector in lineString.Coordinates)
                        sb.Append(vector.Longitude.ToString() + " " + vector.Latitude.ToString() + ", ");
                }
            }
            return sb.Remove(sb.Length - 2, 2).ToString();
        }

        private string getPolygonCoordinates(Placemark placemark)
        {
            StringBuilder sb = new StringBuilder();
            Polygon myGeometry;
            foreach (var element in placemark.Flatten())
            {
                if (element is Polygon)
                {
                    myGeometry = (Polygon)element;
                    foreach (var vector in myGeometry.OuterBoundary.LinearRing.Coordinates)
                        sb.Append(vector.Longitude.ToString() + " " + vector.Latitude.ToString() + ", ");
                }
            }
            return sb.Remove(sb.Length - 2, 2).ToString();
        }

        private PlacemarkType getPlacemarkType(Placemark placemark)
        {
            foreach (var element in placemark.Flatten())
            {
                if (element is Polygon)
                    return PlacemarkType.Polygon;
                else if (element is Point)
                    return PlacemarkType.Point;
                else if (element is LineString)
                    return PlacemarkType.Line;                    
            }
                throw new Exception("Not a line, point, or polygon");
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
            string commandString = @"CREATE TABLE [" + tableName + @"] ([Id] INT NOT NULL PRIMARY KEY,
                [Name] VARCHAR(50) NOT NULL, 
                [" + columnName + @"] [sys].[" + sqlGeoType + @"] NOT NULL, );";
            var command = new System.Data.SqlClient.SqlCommand(commandString, connection);
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
