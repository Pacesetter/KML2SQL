using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.SqlServer.Types;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace KML2SQL
{
    static class MsSqlCommandCreator
    {
        public static SqlCommand createCommand(MapFeature mapFeature, bool geographyMode, int srid, string tableName, 
            string placemarkColumnName, SqlConnection connection)
        {
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            foreach (KeyValuePair<string, string> simpleData in mapFeature.Data)
            {
                sbColumns.Append(simpleData.Key + ",");
                sbValues.Append("@" + simpleData.Key + ",");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + tableName + "(Id,");
            sb.Append(sbColumns);
            sb.Append(placemarkColumnName);
            sb.Append(") VALUES(");
            sb.Append("@Id,");
            sb.Append(sbValues);
            sb.Append("@placemark");
            sb.Append(")");
            string sqlCommandText = sb.ToString();
            SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
            sqlCommand.Parameters.AddWithValue("@Id", mapFeature.Id);
            foreach (KeyValuePair<string, string> simpleData in mapFeature.Data)
            {
                sqlCommand.Parameters.AddWithValue("@" + simpleData.Key, simpleData.Value);
            }
            string UdtType;
            if (geographyMode)
                UdtType = "Geography";
            else
                UdtType = "Geometry";

            sqlCommand.Parameters.Add(new SqlParameter("@placemark", parseCoordinates(srid, mapFeature, geographyMode)) { UdtTypeName = UdtType });
            return sqlCommand;
        }

        private static object parseCoordinates(int srid, MapFeature mapFeature, bool geographyMode)
        {
            if (geographyMode)
                return parseCoordinatesGeography(srid, mapFeature);
            else
                return parseCoordinatesGeometry(srid, mapFeature);

        }

        private static SqlGeometry parseCoordinatesGeometry(int srid, MapFeature mapFeature)
        {
            SqlGeometryBuilder builder = new SqlGeometryBuilder();
            builder.SetSrid(srid);
            builder.BeginGeometry(mapFeature.GeometryType);
            builder.BeginFigure(mapFeature.Coordinates[0].Latitude, mapFeature.Coordinates[0].Longitude);
            if (mapFeature.GeometryType != OpenGisGeometryType.Point)
            {
                for (int i = 1; i < mapFeature.Coordinates.Length; i++)
                    builder.AddLine(mapFeature.Coordinates[0].Latitude, mapFeature.Coordinates[0].Longitude); 
            }
            builder.EndFigure();
            builder.EndGeometry();
            return builder.ConstructedGeometry;
        }

        private static SqlGeography parseCoordinatesGeography(int srid, MapFeature mapFeature)
        {
            SqlGeometry geometryHelper = parseCoordinatesGeometry(srid, mapFeature);
            SqlGeography convertedGeography = new SqlGeography();
            if (mapFeature.GeometryType == OpenGisGeometryType.Polygon)
            {
                SqlGeometry validGeom = geometryHelper.MakeValid().STUnion(geometryHelper.MakeValid().STStartPoint());
                convertedGeography = SqlGeography.STGeomFromText(validGeom.STAsText(), srid);
            }
            else
                convertedGeography = SqlGeography.STGeomFromText(geometryHelper.MakeValid().STAsText(), srid);
            return convertedGeography;
        }
    }
}
