using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using MSWordpiCite.CitationGenerator;
using MSWordpiCite.Entities;
using MSWordpiCite.Enums;
using MSWordpiCite.Classes;
using MSWordpiCite.Forms;
using System.Web;
using MSWordpiCite.Tools;

namespace MSWordpiCite.Controls
{
    public delegate void ReloadAllStyleHandler(bool _reload);
    public delegate void CanApplyStyleHandler(bool _allowed);
    public delegate void DelegateAllowStyle(bool _allowed);
    public partial class CitationStylePreviewControl : UserControl
    {
        #region Variables

        private CitationStyle style;
        private BackgroundWorker bgLoadStyle;
        private StyleOwner owner;
        private List<ItemMasterRow> listItems;
        public event ReloadAllStyleHandler reloadAllStyleEvent;
        public event CanApplyStyleHandler allowApplyStyleEvent;
        public DelegateAllowStyle allowStyle;
        private ReferenceCitationItem rci;

        #endregion

        public CitationStylePreviewControl()
        {
            InitializeComponent();
        }

        #region Public Functions

        public void LoadStyle(StyleOwner owner, string strStyleName)
        {
            if (bgLoadStyle != null && bgLoadStyle.IsBusy)
            {
                bgLoadStyle.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.bgLoadStyle_RunWorkerCompleted);
                bgLoadStyle.CancelAsync();
            }
            this.lblPremiumServices.Visible = false;
            this.btnUpgradeAccount.Visible = false;
            this.lblTitle.Text = strStyleName;             
            this.owner = owner;
            preparePreview(true);
            bgLoadStyle = new BackgroundWorker();
            bgLoadStyle.WorkerSupportsCancellation = true;
            bgLoadStyle.DoWork += new DoWorkEventHandler(this.bgLoadStyle_DoWork);
            bgLoadStyle.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bgLoadStyle_RunWorkerCompleted);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Owner"] = owner;
            data["StyleName"] = strStyleName;
            bgLoadStyle.RunWorkerAsync(data);            
        }

        #endregion

        #region Initialization

        private void bgLoadStyle_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                rci = new ReferenceCitationItem();
                TextCitationItem tci = rci.AddBlock();
                tci.AddItem(listItems[6], "", 0, "");
                tci.AddItem(listItems[1], "", 1, "");
                tci.AddItem(listItems[2], "", 2, "");

                tci = rci.AddBlock();
                tci.AddItem(listItems[0], "", 3, "");
                tci.AddItem(listItems[3], "", 4, "");
                tci.AddItem(listItems[4], "", 5, "");

                tci = rci.AddBlock();
                tci.AddItem(listItems[0], "", 6, "");
                tci.AddItem(listItems[1], "", 7, "");
                tci.AddItem(listItems[2], "", 8, "");
                tci.AddItem(listItems[5], "", 9, "");
                tci.AddItem(listItems[6], "", 10, "");
                tci.AddItem(listItems[7], "", 11, "");
                preparePreview(false);
            }
            catch{}
        }
        private void bgLoadStyle_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Dictionary<string, object> data = e.Argument as Dictionary<string, object>;
                StyleOwner owner = (StyleOwner) data["Owner"];
                string strStyleName = (string)data["StyleName"];
                style = Globals.ThisAddIn.user.LoadStyle(owner, strStyleName);
                if (style.Name == "APA (6th edition)")
                {
                    style.Reference.ItemTypes["JournalArticle"].Fields.JournalName.NameFormat = JournalNameFormat.AsIs;
                    style.Reference.ItemTypes["JournalArticle"].Fields.JournalName.IsAPA = true;
                }
                bool bStyleAllowed = false;
                if(owner == StyleOwner.User)
                    bStyleAllowed = true;
                else
                {
                    bStyleAllowed = checkStyleAccess(strStyleName);
                }
                this.Invoke(this.allowStyle, new Object[] { bStyleAllowed });
            }
            catch
            {}
        }
        private void CitationStylePreviewControl_Load(object sender, EventArgs e)
        {
            initializeUI();
            initializeLang();
            initializeDatabase();
            initializeHandlers();
        }   
        private void initializeUI()
        {
            this.lblLoading.Location = new Point((this.Width - this.lblLoading.Width) / 2, (this.Height - this.lblLoading.Height) / 2);
            this.lblLoading.Visible = true;
        }
        private void initializeLang()
        {
            tooltip.SetToolTip(this.btnEditStyle, Lang.en_US.StyleChooser_BtnEdit_Tooltip);
            this.lblLoading.Text = Lang.en_US.StyleChooser_LoadingPreview_Label;
        }
        private void initializeDatabase()
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            listItems = sr.Deserialize<List<ItemMasterRow>>(Properties.Resources.SampleItems);            
        }
        private void initializeHandlers()
        {
            this.btnEditStyle.Click += new System.EventHandler(this.btnEditStyle_Click);
            this.btnUpgradeAccount.Click += new EventHandler(this.btnUpgradeAccount_Click);
            this.allowStyle += new DelegateAllowStyle(this.allowStyleHandler);
        }

        #endregion

        #region Private Functions

        private bool checkStyleAccess(string strStyleName)
        {
            return Globals.ThisAddIn.user.CheckStyleAccess(strStyleName);
        }
        private void preparePreview(bool bCleared)
        {
            if(bCleared)
            {
                this.lblLoading.Visible = true;
                webBrowser.DocumentText = "";
                webBrowser.ScrollBarsEnabled = false;
            }
            else
            {
                this.lblLoading.Visible = false;
                style.GenerateCitation(rci, true);
                string intext1 = getInText(rci.TextCitations[0]);
                string intext2 = getInText(rci.TextCitations[1]);
                string intext3 = getInText(rci.TextCitations[2]);
                webBrowser.DocumentText = string.Format(Lang.en_US.StyleChooser_Preview_Html, intext1, intext2, intext3, rci.FormatString);
                webBrowser.ScrollBarsEnabled = false;               
                lblTitle.Text = style.Name;
            }            
        }
        private string getInText(TextCitationItem tci)
        {
            string text = string.Empty;
            foreach (string formatstring in tci.FormatString)
            {
                text += formatstring;
            }
            return text;
        }

        #endregion

        #region Event Handlers

        private void allowStyleHandler(bool bAllowed)
        {
            if (bAllowed)
            {
                btnEditStyle.Enabled = true;
                lblPremiumServices.Visible = false;
                btnUpgradeAccount.Visible = false;
            }
            else
            {
                btnEditStyle.Enabled = false;
                lblPremiumServices.Visible = true;
                btnUpgradeAccount.Visible = true;
            }
            allowApplyStyleEvent(bAllowed);
        }
        private void btnEditStyle_Click(object sender, EventArgs e)
        {
            string url = Properties.Settings.Default.URL_ROOTSERVER + string.Format(Properties.Settings.Default.URI_LOGINSTYLEEDITOR, Globals.ThisAddIn.user.Authentication, (owner == StyleOwner.User ? Globals.ThisAddIn.user.UserID : -1), style.Name);
            CitationStyleEditorForm editor = new CitationStyleEditorForm(url);
            editor.reloadStyleEvent += new ReloadStyleHandler(this.editor_reloadStyleEvent);
            editor.ShowDialog();
        }
        private void btnUpgradeAccount_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.user.AccType == MSWordpiCite.Enums.AccountType.Free)
            {
                try
                {                    
                    string url_subscription = Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_SUBSCRIPTION;
                    CitationTools.ExecuteApplication(Properties.Settings.Default.URL_ROOTSERVER + (string.Format(Properties.Settings.Default.URI_LOGINACCESS, Globals.ThisAddIn.user.Authentication, url_subscription)));                    
                }
                catch
                {}
            }
        }
        private void editor_reloadStyleEvent(bool bReload)
        {
            reloadAllStyleEvent(bReload);
        }

        #endregion
    }
}
