using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace KML2SQL
{
    static class KMLParser
    {
        public static Kml Parse(string filePath)
        {
            KmlFile file = KmlFile.Load(filePath);
            Kml kml = file.Root as Kml;
            if (kml == null)
                throw new Exception("Could not parse file into KML. If this is a KMZ, unzip it first!");
            return kml;    
        }
    }
}
