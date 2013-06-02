using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using Microsoft.SqlServer.Types;

namespace KML2SQL
{
    class MapFeature
    {
        public int Id;
        public string Name;
        public OpenGisGeometryType GeometryType;
        public Vector[] Coordinates;
        Placemark placemark;
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        public MapFeature(Placemark placemark, int Id)
        {
            this.placemark = placemark;
            if (placemark.Name != null)
                Name = placemark.Name;
            this.Id = Id;
            GeometryType = getPlacemarkType(placemark);
            switch (this.GeometryType)
            {
                case OpenGisGeometryType.LineString:
                    Coordinates = getLineCoordinates(placemark);
                    break;
                case OpenGisGeometryType.Point:
                    Coordinates = getPointCoordinates(placemark);
                    break;
                case OpenGisGeometryType.Polygon:
                    Coordinates = getPolygonCoordinates(placemark);
                    break;
            }
            initializeData(placemark);
            santizeData();
        }

        private void santizeData()
        {
            foreach (KeyValuePair<string, string> kvPair in Data)
            {
                kvPair.Key.Replace(";", " -");
                kvPair.Value.Replace(";", " -");
                kvPair.Key.Replace("'", "");
                kvPair.Value.Replace("'", "");
            }
        }

        private void initializeData(Placemark placemark)
        {
            foreach (SimpleData sd in placemark.Flatten().OfType<SimpleData>())
            {
                if (sd.Name.ToString().ToLower() == "id")
                    sd.Name = "sd_id";
                Data.Add(sd.Name, sd.Text);
            }
            foreach (Data data in placemark.Flatten().OfType<Data>())
            {
                if (data.Name.ToString().ToLower() == "id")
                    data.Name = "data_id";
                Data.Add(data.Name, data.Value);
            }
        }

        private OpenGisGeometryType getPlacemarkType(Placemark placemark)
        {
            foreach (var element in placemark.Flatten())
            {
                if (element is Polygon)
                    return OpenGisGeometryType.Polygon;
                else if (element is Point)
                    return OpenGisGeometryType.Point;
                else if (element is LineString)
                    return OpenGisGeometryType.LineString;
            }
            throw new Exception("Placemark "+Id.ToString()+"Not a line, point, or polygon");
        }

        private Vector[] getPointCoordinates(Placemark placemark)
        {
            List<Vector> coordinates = new List<Vector>();
            foreach (var element in placemark.Flatten())
                if (element is Point)
                {
                    Point myPoint = (Point)element;
                    Vector myVector = new Vector();
                    myVector.Latitude = myPoint.Coordinate.Latitude;
                    myVector.Longitude = myPoint.Coordinate.Longitude;
                    coordinates.Add(myVector);
                }
            return coordinates.ToArray();
        }

        private Vector[] getLineCoordinates(Placemark placemark)
        {
            List<Vector> coordinates = new List<Vector>();
            LineString lineString;
            foreach (var element in placemark.Flatten())
            {
                if (element is LineString)
                {
                    lineString = (LineString)element;
                    foreach (var vector in lineString.Coordinates)
                        coordinates.Add(vector);
                }
            }
            return coordinates.ToArray();
        }

        private Vector[] getPolygonCoordinates(Placemark placemark)
        {
            List<Vector> coordinates = new List<Vector>();
            Polygon myGeometry;
            foreach (var element in placemark.Flatten())
            {
                if (element is Polygon)
                {
                    myGeometry = (Polygon)element;
                    foreach (var vector in myGeometry.OuterBoundary.LinearRing.Coordinates)
                        coordinates.Add(vector);
                }
            }
            return coordinates.ToArray();
        }

        public override string ToString()
        {
            return Name + " " + Id.ToString() + " - " + GeometryType.ToString();
        }
    }
}
