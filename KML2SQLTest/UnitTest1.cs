using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KML2SQL;

namespace KML2SQLTest
{
    [TestClass]
    public class UnitTest1
    {
        MapUploader myUploader;
        //Well, obviously you don't get my passwords! :D
        PasswordList passwordList = new PasswordList();

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
        public void TestGeometry()
        {
            myUploader = new MapUploader("ylkx1ic1so.database.windows.net", "hackathon", "pabreetzio", passwordList[0], "polygon", @"TestData\Basic.kml", "myTable", 4326, false);
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
