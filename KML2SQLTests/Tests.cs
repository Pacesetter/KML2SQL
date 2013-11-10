using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KML2SQL;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Diagnostics;
using System.Text;

namespace KML2SQLTests
{
    [TestClass]
    public class Tests
    {
        MapUploader myUploader;
        //Well, obviously you don't get my passwords! :D
        PasswordList passwordList = new PasswordList();
        Kml kml;
        StringBuilder log;
        string logdir = string.Empty;
        string login = "zshuford";
        string database = "TestDB";
        string connection = "sdfzufbaq8.database.windows.net";

        [TestMethod]
        public void CheckNPA()
        {
            myUploader = new MapUploader(connection, database, login, passwordList[0], "polygon", @"TestData\npa.kml", "myTable", 4326, true, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void BasicKML()
        {
            myUploader = new MapUploader(connection, database, login, passwordList[0], "polygon", @"TestData\Basic.kml", "myTable", 4326, true, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void BasicKMLGeometry()
        {
            myUploader = new MapUploader(connection, database, login, passwordList[0], "polygon", @"TestData\Basic.kml", "myTable", 4326, false, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void CheckNPAGeometry()
        {
            myUploader = new MapUploader(connection, database, login, passwordList[0], "polygon", @"TestData\npa.kml", "myTable", 4326, false, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void SchoolTest()
        {
            myUploader = new MapUploader(connection, database, login, passwordList[0], "polygon", @"TestData\school.kml", "myTable", 4326, true, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void SchoolTestGeometry()
        {
            myUploader = new MapUploader(connection, database, login, passwordList[0], "polygon", @"TestData\school.kml", "myTable", 4326, false, log, logdir);
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
