using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KML2SQL
{
    class MapFeature
    {
        public int ID;
        public string Name;
        public PlacemarkType Type;
        public string Coordinates;
    }

    enum PlacemarkType
    {
        Point,
        Line,
        Polygon,
    }
}
