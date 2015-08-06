using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MSWordpiCite.Forms
{
    public delegate void ReloadStyleHandler(bool _reload);
    public partial class CitationStyleEditorForm : Form
    {
        private Uri url;
        public event ReloadStyleHandler reloadStyleEvent;

        public CitationStyleEditorForm(string _url)
        {
            InitializeComponent();
            this.url = new Uri(_url);
        }
        
        #region Initialization

        private void CitationStyleEditorForm_Load(object sender, EventArgs e)
        {
            initializeUI();
            initializeLang();
        }
        private void initializeUI()
        {
            this.Width = Properties.Settings.Default.DEFAULT_STYLEEDITOR_WIDTH;
            this.Height = Properties.Settings.Default.DEFAULT_STYLEEDITOR_HEIGHT;
            this.Location = new Point(Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Width - this.Width) / 2), Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Height - this.Height) / 2));
            this.lblLoading.Location = new Point((this.Width - this.lblLoading.Width) / 2, (this.Height - this.lblLoading.Height) / 2);
            this.webBrowser.Visible = true;
            this.lblLoading.Visible = true;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.ObjectForScripting = new External(this);
            this.webBrowser.Navigate(url);
        }
        private void initializeLang()
        {
            this.Text = Lang.en_US.StyleEditor_Title;
        }

        #endregion

        #region Public Functions

        public void ShowWebBrowser()
        {
            this.lblLoading.Visible = false;
            this.webBrowser.Dock = DockStyle.Fill;
            this.webBrowser.Visible = true;
        }
        public void Reload(bool bReload)
        {
            reloadStyleEvent(true);
            this.Close();
            this.Dispose();
        }

        #endregion
    }

    // Expose to web browser as JavaScript functions: window.external
    [ComVisible(true)]
    public class External
    {
        CitationStyleEditorForm parent;

        public External(CitationStyleEditorForm _parent)
        {
            this.parent = _parent;
        }

        public void CloseEditorWindow(int c)
        {            
            parent.Reload(c == 1);            
        }

        public void EditorLoaded()
        {
            parent.ShowWebBrowser();
        }
    }
}
