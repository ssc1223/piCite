using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using MSWordpiCite.Classes;
using MSWordpiCite.Enums;
using MSWordpiCite.Controls;
using MSWordpiCite.Tools;

namespace MSWordpiCite.Threads
{
    class LoginThread
    {
        private string Email;
        private string Password;
        public LoginControl loginControl;

        public LoginThread(LoginControl logincontrol)
        {
            this.loginControl = logincontrol;
        }
        public LoginThread(string email, string password, LoginControl logincontrol)
        {
            this.Email = email;
            this.Password = password;
            this.loginControl = logincontrol;
        }
        public void Login()
        {
            bool bLoginned = false;
            Dictionary<string, string> data = new Dictionary<string, string>();
            try
            {
                //string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER.Replace("http://", "https://") + Properties.Settings.Default.URI_LOGINHANDLER + "?op=oauth&email=" + this.Email + "&password=" + System.Web.HttpUtility.UrlEncode(this.Password) + "&from=" + DeviceTypes.MSWordWin.ToString();
                string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_LOGINHANDLER + "?op=oauth&email=" + this.Email + "&password=" + System.Web.HttpUtility.UrlEncode(this.Password) + "&from=" + DeviceTypes.MSWordWin.ToString();
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                data = js.Deserialize<Dictionary<string, string>>(Regex.Replace(req.GetResponseWithNetworkConnHandler(), @"^\(|\)$", ""));                
            }
            catch(Exception ex)
            {
                Globals.ThisAddIn.log.WriteLine(LogType.Error, "LoginThread::Login", ex.ToString());
            }
            if (data != null && data.ContainsKey("Result"))
            {
                bLoginned = data["Result"] == "1";
            }
            if (bLoginned)
                loginControl.Invoke(loginControl.LoginSucceedEvent, new Object[] { data });
            else
                loginControl.Invoke(loginControl.LoginFailedEvent, new Object[] { Lang.en_US.Status_Login_Failed });
        }

        public void LoginByAuthentication()
        {
            bool bLoginned = false;
            Dictionary<string, string> data = new Dictionary<string, string>();
            try
            {
                //string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER.Replace("http://", "https://") + Properties.Settings.Default.URI_LOGINHANDLER + "?op=oauthalt&authstr=" + Properties.Settings.Default.USER_AUTHENTICATION;
                string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_LOGINHANDLER + "?op=oauthalt&authstr=" + Properties.Settings.Default.USER_AUTHENTICATION;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                data = js.Deserialize<Dictionary<string, string>>(System.Text.RegularExpressions.Regex.Replace(req.GetResponseWithNetworkConnHandler(), @"^\(|\)$", ""));                
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.log.WriteLine(LogType.Error, "LoginThread::Login", ex.ToString());
            }
            if (data != null && data.ContainsKey("Result"))
                bLoginned = data["Result"] == "1";
            if (bLoginned)
                loginControl.Invoke(loginControl.LoginSucceedEvent, new Object[] { data });
            else
                loginControl.Invoke(loginControl.LoginFailedEvent, new Object[] { Lang.en_US.Status_Login_Failed });
            
        }
    }
}
