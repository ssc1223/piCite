using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Text.RegularExpressions;
using MSWordpiCite.Lang;
using MSWordpiCite.Tools;
using MSWordpiCite.Enums;
using MSWordpiCite.Classes;
using System.Threading;
using MSWordpiCite.Threads;
using MSWordpiCite.Forms;

namespace MSWordpiCite.Controls
{
    public delegate void LoginFailedDelegate(string error);
    public delegate void LoginSucceedDelegate(Dictionary<string, string> data);

    public partial class LoginControl : UserControl
    {
        #region Variables

        private Logger log = Globals.ThisAddIn.log;
        private Thread loginThread;
        public LoginFailedDelegate LoginFailedEvent;
        public LoginSucceedDelegate LoginSucceedEvent;

        #endregion

        public LoginControl()
        {
            InitializeComponent();
        }

        #region Initialization

        private void initializeUI()
        {
            this.Width = Globals.ThisAddIn.iTempWidth;
            this.Height = Globals.ThisAddIn.iTempHeight;
            this.chkRemember.Checked = Properties.Settings.Default.DEFAULT_REMEMBERME;
            this.inputEmail.Text = Properties.Settings.Default.LAST_LOGIN_EMAIL;
        }
        private void initializeLang()
        {
            this.lblTitle.Text = Lang.en_US.Login_Title_Label;
            this.lblEmail.Text = Lang.en_US.Login_Email_Label;
            this.lblPassword.Text = Lang.en_US.Login_Password_Label;
            this.lnkForgotPassword.Text = Lang.en_US.Login_ForgotPassword_Link;
            this.lnkRegister.Text = Lang.en_US.Login_Register_Link;
            this.btnLogin.Text = Lang.en_US.Login_Login_Button;
            this.chkRemember.Text = Lang.en_US.Login_RememberMe_CheckBox;
            this.lblLoading.Text = Lang.en_US.Login_LoggingIn_Label;
        }
        private void initializeHandlers()
        {
            this.tableLayout.SizeChanged += new EventHandler(this.tableLayout_Resized);
            this.inputEmail.KeyDown += new KeyEventHandler(this.this_KeyPressed);
            this.inputPassword.KeyDown += new KeyEventHandler(this.this_KeyPressed);
            this.btnLogin.KeyDown += new KeyEventHandler(this.this_KeyPressed);
            this.LoginFailedEvent = new LoginFailedDelegate(this.LoginThread_LoginFailed);
            this.LoginSucceedEvent = new LoginSucceedDelegate(this.LoginThread_LoginSucceed);
        }
        private void loginError(string error)
        {
            lblStatus.Text = error;
            lblStatus.ForeColor = Color.Red;
            lblStatus.Visible = true;
            btnLogin.Enabled = true;
            chkRemember.Enabled = true;
            inputEmail.Enabled = true;
            inputPassword.Enabled = true;
        }

        #endregion

        #region Event Handlers

        private void LoginControl_Load(object sender, EventArgs e)
        {
            try
            {
                initializeUI();
                initializeLang();
                initializeHandlers();
                if (Properties.Settings.Default.USER_REMEMBERME)
                {
                    this.lblStatus.Visible = false;
                    this.inputPassword.Text = "password";
                    this.inputEmail.Enabled = false;
                    this.inputPassword.Enabled = false;
                    this.chkRemember.Enabled = false;
                    this.btnLogin.Enabled = false;
                    if (loginThread != null)
                        if (loginThread.IsAlive)
                            loginThread.Abort();
                    this.lblLoading.Visible = true;
                    this.imgLoading.Visible = true;
                    LoginThread login = new LoginThread(this);
                    ThreadStart threadStart = new ThreadStart(login.LoginByAuthentication);
                    loginThread = new Thread(threadStart);
                    loginThread.Start();
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "LoginControl::LoginControl_Load", ex.ToString());
            }
        }
        private void LoginThread_LoginFailed(string error)
        {
            this.lblLoading.Visible = false;
            this.imgLoading.Visible = false;
            loginThread.Abort();
            loginError(error);
        }
        private void LoginThread_LoginSucceed(Dictionary<string, string> data)
        {
            try
            {
                loginThread.Abort();
                Globals.ThisAddIn.user = new User(data["UserID"], data["LastName"], data["ForeName"], data["Authentication"], data["ServerAddress"], int.Parse(data["AccountType"]));
                Properties.Settings.Default.DEFAULT_REMEMBERME = chkRemember.Checked;
                Properties.Settings.Default.LAST_LOGIN_EMAIL = this.inputEmail.Text;
                if (chkRemember.Checked)
                    Globals.ThisAddIn.RememberMe();
                Properties.Settings.Default.Save();
                Globals.ThisAddIn.iTempHeight = this.Height;
                Globals.ThisAddIn.iTempWidth = this.Width;
                Globals.ThisAddIn.ShowMasterControl();
                Globals.ThisAddIn.ShowCustomPanel();
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "LoginControl::LoginThread_LoginSucceed", ex.ToString());
            }
        }
        private void tableLayout_Resized(object sender, EventArgs e)
        {
            try
            {
                Globals.ThisAddIn.iTempWidth = Globals.ThisAddIn.GetCurrentCustomPaneWidth();
                Globals.ThisAddIn.iTempHeight = Globals.ThisAddIn.GetCurrentCustomPaneHeight();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "LoginControl::table1_Resized", ex.ToString());
            }
        }
        private void this_KeyPressed(object sender, KeyEventArgs e)
        {
            try
            {
                bool bControlFocused = inputEmail.Focused || inputPassword.Focused || chkRemember.Focused || btnLogin.Focused;
                if (bControlFocused && e.KeyCode == Keys.Enter)
                    btnLogin_Click(null, null);
            }            
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "LoginControl::this_KeyPressed", ex.ToString());
            }
        }
        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                CitationTools.ExecuteApplication(Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_FORGOTPASSWORD);
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "LoginControl::lnkForgotPassword_LinkClicked", ex.ToString());
            }            
        }
        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                CitationTools.ExecuteApplication(Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_REGISTER);
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "LoginControl::lnkRegister_LinkClicked", ex.ToString());
            }            
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblStatus.Visible = false;
                this.btnLogin.Enabled = false;
                this.chkRemember.Enabled = false;
                this.inputEmail.Enabled = false;
                this.inputPassword.Enabled = false;
                string strEmail = Regex.Replace(inputEmail.Text.ToLower(), @"^\s+|\s+$", "");                
                if(Regex.Match(strEmail, @"#debug$").Success)
                {
                    DialogResult dr = MessageBox.Show("Are you sure that you want to switch to Debug Mode?", "DEBUG MODE", MessageBoxButtons.YesNo);
                    Globals.ThisAddIn.DebugMode = dr == DialogResult.Yes;
                    strEmail = Regex.Replace(strEmail, @"#debug$", "");                
                }
                inputEmail.Text = strEmail;
                string strPassword = inputPassword.Text;
                // Check for email format
                if (!Regex.Match(strEmail, @"^[a-zA-Z0-9\._\-]+@([a-zA-Z0-9\.\-]+\.)+[a-zA-Z0-9\.\-]{1,4}$").Success)
                {
                    loginError(Lang.en_US.Status_Invalid_Email);
                    inputEmail.Focus();
                    return;
                }

                // Check the password validity
                if (strPassword.Length == 0)
                {
                    loginError(Lang.en_US.Status_Empty_Password);
                    inputPassword.Focus();
                    return;
                }

                LoginThread login = new LoginThread(strEmail, strPassword, this);
                loginThread = new Thread(new ThreadStart(login.Login));
                loginThread.Start();
                this.lblLoading.Visible = true;
                this.imgLoading.Visible = true;
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "LoginControl::btnLogin_Click", ex.ToString());
            }
        }

        private void btThirdPartyLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //this.lblStatus.Visible = false;
                //this.btnLogin.Enabled = false;
                //this.chkRemember.Enabled = false;
                //this.inputEmail.Enabled = false;
                //this.inputPassword.Enabled = false;
                ThirdPartyLoginForm wBLoginForm = new ThirdPartyLoginForm();
                wBLoginForm.Show();
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "LoginControl::btnFBLogin_Click", ex.ToString());
            }
        }

        #endregion
    }
}
