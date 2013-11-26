using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;

namespace KML2SQL
{
    public static class UsageReporter
    {
        public static void Report(string message, bool error)
        {
            try
            {
                using (var md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Environment.UserName));
                    StringBuilder userId = new StringBuilder();
                    foreach (var d in data)
                    {
                        userId.Append(d.ToString("x2"));
                    }
                    string url = string.Format("http://usagetracker.azurewebsites.net?user_id={0}&message={1}&error={2}", userId.ToString(), message, error.ToString());
                    //string url = "http://usagetracker.azurewebsites.net?user_id=" + userId.ToString() + ;
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "GET";
                    var response = httpWebRequest.GetResponse();
                }
            }
            catch (Exception ex) { string x = ex.ToString(); Console.Write(x); }
        }
    }
}
