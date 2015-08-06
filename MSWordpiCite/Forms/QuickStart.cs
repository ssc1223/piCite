using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSWordpiCite.Forms
{
    public partial class QuickStart : Form
    {
        private string URL;

        public QuickStart(string url)
        {
            InitializeComponent();
            this.URL = url;
        }

        private void initializeUI()
        {
            this.Width = Properties.Settings.Default.DEFAULT_QUICKSTART_WIDTH;
            this.webBrowser.Width = Properties.Settings.Default.DEFAULT_QUICKSTART_WIDTH;
            this.Height = Properties.Settings.Default.DEFAULT_QUICKSTART_HEIGHT;
            this.Location = new Point(Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Width - this.Width) / 2), Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Height - this.Height) / 2));
            this.lblLoading.Location = new Point((this.Width - this.lblLoading.Width) / 2, (this.Height - this.lblLoading.Height) / 2);
            this.webBrowser.Visible = true;
            this.lblLoading.Visible = true;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Navigate(this.URL);
        }

        private void initializeLang()
        {
            this.Text = string.Format(Lang.en_US.QuickStart_Title, Properties.Settings.Default.VERSION);
        }

        private void initializeHandlers()
        {
            this.FormClosing += new FormClosingEventHandler(this.QuickStart_FormClosing);
            this.Resize += new EventHandler(this.QuickStart_Resize);
        }

        private void QuickStart_Resize(object sender, EventArgs e)
        {
            this.webBrowser.Width = this.Width - (this.webBrowser.Margin.Left + this.webBrowser.Margin.Right);
            this.webBrowser.Height = this.Height - (this.webBrowser.Margin.Top + this.webBrowser.Margin.Bottom);
        }

        private void QuickStart_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Properties.Settings.Default.QUICKSTART_ALLOW_PERMANENTCLOSED)
            {
                DialogResult result = MessageBox.Show(Lang.en_US.QuickStart_ShowNextTime_Msg, Lang.en_US.QuickStart_ShowNextTime_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                    Properties.Settings.Default.DEFAULT_SHOWQUICKSTART = false;
            }
            else
                Properties.Settings.Default.QUICKSTART_ALLOW_PERMANENTCLOSED = true;

            Properties.Settings.Default.Save();
        }

        private void QuickStart_Load(object sender, EventArgs e)
        {
            initializeUI();
            initializeLang();
            initializeHandlers();
        }
    }
}
