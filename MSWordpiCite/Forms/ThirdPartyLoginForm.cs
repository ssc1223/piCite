using MSWordpiCite.Classes;
using MSWordpiCite.Enums;
using MSWordpiCite.Threads;
using MSWordpiCite.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace MSWordpiCite.Forms
{
    public partial class ThirdPartyLoginForm : Form
    {
        public bool bFormComplete = false;
        public bool GetFormStatus
        {
           get {return bFormComplete; }
        }

        public ThirdPartyLoginForm()
        {
            InitializeComponent();
        }

        private void initializeUI()
        {
            bFormComplete = false;
            this.wBLogin.ScriptErrorsSuppressed = true;
            this.wBLogin.IsWebBrowserContextMenuEnabled = false;
            this.wBLogin.Navigate("http://root.pifolio.com/school/login.aspx?from=citetool");
            this.wBLogin.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
        }

        private void wbLoginForm_Load(object sender, EventArgs e)
        {
            initializeUI();
        }

        /// <summary>
        /// FormClose Event
        /// </summary>
        private void wbLoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WebBrowserHelper.ClearCache();
        }

        /// <summary>
        /// WebBrowser load complete event
        /// </summary>
        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string strURL = this.wBLogin.Document.Url.ToString();
            Regex.Replace(strURL, @"^\s+|\s$", "");
            if (strURL.Contains("citeidentity="))
            {
                bool bLoginned = false;
                Dictionary<string, string> data = new Dictionary<string, string>();
                string[] strUserInfo = Regex.Split(strURL, "citeidentity=");
                string strDesDecrypt = desDecryptBase64(strUserInfo[1]);//DesDecrypt user's information
                string strEmail = strDesDecrypt.Split('|')[0];//Get Email
                string strPassword = strDesDecrypt.Split('|')[1];//Get PW

                try
                {
                    string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_LOGINHANDLER + "?op=oauth&email=" + strEmail + "&password=" + strPassword + "&from=" + DeviceTypes.MSWordWin.ToString();
                    CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    data = js.Deserialize<Dictionary<string, string>>(Regex.Replace(req.GetResponseWithNetworkConnHandler(), @"^\(|\)$", ""));
                }
                catch (Exception ex)
                {
                    Globals.ThisAddIn.log.WriteLine(LogType.Error, "ThirdPartyLoginForm::ThirdPartyLogin", ex.ToString());
                }
                if (data != null && data.ContainsKey("Result"))
                {
                    bLoginned = data["Result"] == "1";
                }
                Form lastOpenedForm = Application.OpenForms[Application.OpenForms.Count - 1];
                if (lastOpenedForm.Name == "ThirdPartyLoginForm")
                {
                    lastOpenedForm.Activate();
                    Form.ActiveForm.Close();//Close the login form to complete the login process
                }
                Globals.ThisAddIn.user = new User(data["UserID"], data["LastName"], data["ForeName"], data["Authentication"], data["ServerAddress"], int.Parse(data["AccountType"]));
                Globals.ThisAddIn.iTempHeight = this.Height;
                Globals.ThisAddIn.iTempWidth = this.Width;
                Globals.ThisAddIn.ShowMasterControl();
                Globals.ThisAddIn.ShowCustomPanel();
            }
        }


        #region Encrypt & Desdencrypt

        /// <summary>
        /// Desencrypt string data
        /// </summary>
        private string desEncryptBase64(string source)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] key = Encoding.ASCII.GetBytes("igroupTW");
            byte[] iv = Encoding.ASCII.GetBytes("igroupTW");
            byte[] dataByteArray = Encoding.UTF8.GetBytes(source);

            des.Key = key;
            des.IV = iv;
            string encrypt = "";
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(dataByteArray, 0, dataByteArray.Length);
                cs.FlushFinalBlock();
                encrypt = Convert.ToBase64String(ms.ToArray());
            }
            return encrypt;
        }

        /// <summary>
        /// Desdecrypt a string data
        /// </summary>
        private string desDecryptBase64(string encrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] key = Encoding.ASCII.GetBytes("igroupTW");
            byte[] iv = Encoding.ASCII.GetBytes("igroupTW");
            des.Key = key;
            des.IV = iv;

            byte[] dataByteArray = Convert.FromBase64String(encrypt);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        #endregion
    }
}
