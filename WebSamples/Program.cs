using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Mail;

namespace WebSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            //string siteToLogon = @"";

            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://myapp/home.aspx");

            //request.Method = "GET";
            //request.UseDefaultCredentials = false;
            //request.PreAuthenticate = true;
            //// request.Credentials = new NetworkCredential("username", "password", "domain");

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Raises Unauthorized Exception
        }


        public static bool Login()
        {
            string siteToLogin = @"";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://myapp/home.aspx");

            request.Method = "POST";
            request.UseDefaultCredentials = true;
            request.PreAuthenticate = true;

            request.Credentials = CredentialCache.DefaultCredentials;

            // Otherwise try
            // request.Credentials = new NetworkCredential("username", "password", "domain");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Raises Unauthorized Exception


            return false;
        }

        public static void LoginEmulatingTheMozilla()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://sso.bhmobile.ba/sso/login");
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)";
            req.Method = "POST";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.Headers.Add("Accept-Language: en-us,en;q=0.5");
            req.Headers.Add("Accept-Encoding: gzip,deflate");
            req.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            req.KeepAlive = true;
            req.Headers.Add("Keep-Alive: 300");
            req.Referer = "http://sso.bhmobile.ba/sso/login";

            req.ContentType = "application/x-www-form-urlencoded";

            String Username = "username";
            String PassWord = "Password";

            StreamWriter sw = new StreamWriter(req.GetRequestStream());
            sw.Write("");
            sw.Close();

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();


            StreamReader reader = new StreamReader(response.GetResponseStream());
            string tmp = reader.ReadToEnd();

            foreach (Cookie cook in response.Cookies)
            {
                tmp += "\n" + cook.Name + ": " + cook.Value;
            }

        }

    }
}
