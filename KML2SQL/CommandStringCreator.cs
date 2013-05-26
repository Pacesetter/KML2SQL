using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KML2SQL
{
    static class CommandStringCreator
    {
        public static string setCommandString(MapFeature mapFeature, bool geographyMode, int srid, string tableName)
        {
            switch (mapFeature.Type)
            {
                case PlacemarkType.Polygon:
                    return polygonCommand(mapFeature, geographyMode, srid, tableName);
                case PlacemarkType.Line:
                    return lineCommand(mapFeature, geographyMode, srid, tableName);
                default:
                    return pointCommand(mapFeature, geographyMode, srid, tableName);
            }
        }

        private static string pointCommand(MapFeature mapFeature, bool geographyMode, int srid, string tableName)
        {
            string commandString;
            string dataType;

            if (geographyMode)
                dataType = "geography";
            else
                dataType = "geometry";

                commandString = @"DECLARE @point "+dataType+@";
                    set @point = "+dataType+@"::STPointFromText('POINT (-84 30)', 4326);
                    INSERT INTO myTable VALUES ("+mapFeature.ID+@", '"+mapFeature.Name+@"', @point);";
            return commandString;
        }

        private static string lineCommand(MapFeature mapFeature, bool geographyMode, int srid, string tableName)
        {
            string commandString;
            string dataType;

            if (geographyMode)
                dataType = "geography";
            else
                dataType = "geometry";

            commandString = @"DECLARE @line " + dataType + @";
                    set @line = " + dataType + @"::STLineFromText('LINESTRING ("+mapFeature.Coordinates+@")', 4326);
                    INSERT INTO myTable VALUES ("+mapFeature.ID+@", '"+mapFeature.Name+@"', @line);";
            return commandString;
        }

        private static string polygonCommand(MapFeature mapFeature, bool geographyMode, int srid, string tableName)
        {
            string commandString;
            if (geographyMode)
                commandString = @"DECLARE @geom geometry;
                            SET @geom = geometry::STPolyFromText('POLYGON((" + mapFeature.Coordinates + @"))', " + srid + @");
                            DECLARE @validGeom geometry;
                            set @validGeom = @geom.MakeValid().STUnion(@geom.STStartPoint())
                            DECLARE @validGeo geography;
                            SET @validGeo = geography::STGeomFromText(@validGeom.STAsText(), " + srid + @")
                            insert into " + tableName + " VALUES (" + mapFeature.ID + ", '"+mapFeature.Name+"', @validGeo);";
            else
                commandString = @"DECLARE @geom geometry;
                            SET @geom = geometry::STPolyFromText('POLYGON((" + mapFeature.Coordinates + @"))', " + srid + @");
                            DECLARE @validGeom geometry;
                            set @validGeom = @geom.MakeValid().STUnion(@geom.STStartPoint())
                            insert into " + tableName + " VALUES (" + mapFeature.ID + ", '"+mapFeature.Name+"', @validGeom);";
            return commandString;
        }
    }
}
