using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using MSWordpiCite.Tools;
using System.Windows.Forms;

namespace MSWordpiCite.Classes
{
    // Custom HTTP request class
    public class CustomHttpRequest
    {
        private HttpWebRequest req;
        public CustomHttpRequest(string url)
        {
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = Properties.Settings.Default.USERAGENT;
            }
            catch
            { }
        }
        public CustomHttpRequest(string url, string data)
        {
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes (data);
                Stream os = null;
                try
                { // send the Post
                    req.ContentLength = bytes.Length;   //Count bytes to send
                    os = req.GetRequestStream();
                    os.Write (bytes, 0, bytes.Length);         //Send it
                }
                catch (WebException ex)
                {
                    MessageBox.Show ( ex.Message, "HttpPost: Request error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                finally
                {
                  if (os != null)
                  {
                     os.Close();
                  }
                } 
            }
            catch{}
        }
        public string GetResponse()
        {
            string strResp = string.Empty;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                strResp = sr.ReadToEnd();
                if(Globals.ThisAddIn.DebugMode)
                {
                    Globals.ThisAddIn.log.WriteLine(LogType.Info, "GetResponse()", strResp);
                }
                resp.Close();
            }
            catch(Exception ex)
            {
                Globals.ThisAddIn.log.WriteLine(LogType.Error, "CustomHttpRequest::GetResponse", ex.ToString());
            }
            return strResp;
        }
        public string GetResponseWithNetworkConnHandler()
        {
            string strResp = string.Empty;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                strResp = sr.ReadToEnd();
                if (Globals.ThisAddIn.DebugMode)
                {
                    Globals.ThisAddIn.log.WriteLine(LogType.Info, "GetResponse()", strResp);
                }
                resp.Close();
            }
            catch (Exception ex)                
            {
                Globals.ThisAddIn.log.WriteLine(LogType.Error, "CustomHttpRequest::GetResponseWithNetworkConnHandler", ex.ToString());
                try
                {
                    IPHostEntry ip = Dns.GetHostEntry(Properties.Settings.Default.URL_ROOTSERVER);
                }
                catch (Exception ex1)   
                {
                    MessageBox.Show(Lang.en_US.Login_NoInternet_Msg, Lang.en_US.Login_NoInternet_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Globals.ThisAddIn.log.WriteLine(LogType.Error, "CustomHttpRequest::GetResponseWithNetworkConnHandler - NoInternet", ex1.ToString());
                }
            }
            return strResp;                     
        }
    }
}
