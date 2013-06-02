using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KML2SQL;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Diagnostics;

namespace KML2SQLTests
{
    [TestClass]
    public class Tests
    {
        MapUploader myUploader;
        //Well, obviously you don't get my passwords! :D
        PasswordList passwordList = new PasswordList();
        Kml kml;

        [TestMethod]
        public void CheckNPA()
        {
            myUploader = new MapUploader("ylkx1ic1so.database.windows.net", "hackathon", "pabreetzio", passwordList[0], "polygon", @"TestData\npa.kml", "myTable", 4326, true);
            myUploader.Upload();
        }

        [TestMethod]
        public void BasicKML()
        {
            myUploader = new MapUploader("ylkx1ic1so.database.windows.net", "hackathon", "pabreetzio", passwordList[0], "polygon", @"TestData\Basic.kml", "myTable", 4326, true);
            myUploader.Upload();
        }

        [TestMethod]
        public void BasicKMLGeometry()
        {
            myUploader = new MapUploader("ylkx1ic1so.database.windows.net", "hackathon", "pabreetzio", passwordList[0], "polygon", @"TestData\Basic.kml", "myTable", 4326, false);
            myUploader.Upload();
        }

        [TestMethod]
        public void CheckNPAGeometry()
        {
            myUploader = new MapUploader("ylkx1ic1so.database.windows.net", "hackathon", "pabreetzio", passwordList[0], "polygon", @"TestData\npa.kml", "myTable", 4326, false);
            myUploader.Upload();
        }

        //[TestMethod]
        //public void BasicKmlOnMySql()
        //{
        //    myUploader = new MapUploader("192.168.0.202", "test", "root", passwordList[1], "placemark", @"TestData\Basic.kml", "myTable", 4326, true);
        //    myUploader.Upload();
        //}
    }
}
