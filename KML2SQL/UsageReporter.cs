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
            string userId;
            using (var md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Environment.UserName));
                StringBuilder sb = new StringBuilder();
                foreach (var d in data)
                {
                    sb.Append(d.ToString("x2"));
                }
                userId = sb.ToString();
            }
            try
            {
                SendReport(userId, message, error);
            }
            catch (Exception ex)
            {
                SendReport(userId, "Error sending usage report.", true);
            }
        }

        private static void SendReport(string userId, string message, bool error)
        {
            string url = string.Format("http://usagetracker.azurewebsites.net?user_id={0}&message={1}&error={2}", userId, message, error.ToString());
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            var response = httpWebRequest.GetResponse();
        }
    }
}
