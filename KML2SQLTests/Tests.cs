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
        StringBuilder log = new StringBuilder();
        string logdir = string.Empty;
        string login = "zshuford";
        string database = "TestDB";
        string server = "sdfzufbaq8.database.windows.net";
        private string connectionString;

        [TestInitialize]
        public void InitializeTests()
        {
            connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";Persist Security Info=True;User ID="
                + login + ";Password=" + passwordList[0];
        }

        [TestMethod]
        public void CheckNPA()
        {
            myUploader = new MapUploader(connectionString, "polygon", @"TestData\npa.kml", "myTable", 4326, true, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void BasicKML()
        {
            myUploader = new MapUploader(connectionString, "polygon", @"TestData\Basic.kml", "myTable", 4326, true, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void BasicKMLGeometry()
        {
            myUploader = new MapUploader(connectionString, "polygon", @"TestData\Basic.kml", "myTable", 4326, false, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void CheckNPAGeometry()
        {
            myUploader = new MapUploader(connectionString, "polygon", @"TestData\npa.kml", "myTable", 4326, false, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void SchoolTest()
        {
            myUploader = new MapUploader(connectionString, "polygon", @"TestData\school.kml", "myTable", 4326, true, log, logdir);
            myUploader.Upload();
        }

        [TestMethod]
        public void SchoolTestGeometry()
        {
            myUploader = new MapUploader(connectionString, "polygon", @"TestData\school.kml", "myTable", 4326, false, log, logdir);
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
