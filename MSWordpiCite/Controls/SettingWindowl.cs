using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSWordWizCite.Controls
{
    public partial class SettingControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingControl"/> class.
        /// </summary>
        public SettingControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the SettingControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingControl_Load(object sender, EventArgs e)
        {
            initializeUI();
            initializeLang();
            initializeHandlers();
        }

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        private void initializeUI()
        {
            this.Width = Globals.ThisAddIn.iTempWidth;
            this.Height = Globals.ThisAddIn.iTempHeight;
            chkRememberEmail.Checked = Properties.Settings.Default.DEFAULT_REMEMBER_EMAIL;
        }

        /// <summary>
        /// Initializes the lang.
        /// </summary>
        private void initializeLang()
        {
            lblSettingTitle.Text = Lang.en_US.Setting_Title;
            lblSettingLanguague.Text = Lang.en_US.Setting_Language;
            btnOK.Text = Lang.en_US.Setting_OK;
            btnCancel.Text = Lang.en_US.Setting_Cancel;
        }

        /// <summary>
        /// Initializes the handlers.
        /// </summary>
        private void initializeHandlers()
        {
            tableLayout.SizeChanged += new EventHandler(this.tableLayout_Resized);
        }

        /// <summary>
        /// Handles the Resized event of the tableLayout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tableLayout_Resized(object sender, EventArgs e)
        {
            Globals.ThisAddIn.iTempWidth = Globals.ThisAddIn.GetCurrentCustomPaneWidth();
            Globals.ThisAddIn.iTempHeight = Globals.ThisAddIn.GetCurrentCustomPaneHeight();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Globals.ThisAddIn.iTempHeight = this.Height;
            Globals.ThisAddIn.iTempWidth = this.Width;
            Globals.ThisAddIn.ShowMasterControl();
            Globals.ThisAddIn.ShowCustomPanel();
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DEFAULT_REMEMBER_EMAIL = chkRememberEmail.Checked;
            if (!chkRememberEmail.Checked)
                Properties.Settings.Default.LAST_LOGIN_EMAIL = string.Empty;
            Properties.Settings.Default.Save();
            Globals.ThisAddIn.iTempHeight = this.Height;
            Globals.ThisAddIn.iTempWidth = this.Width;
            Globals.ThisAddIn.ShowMasterControl();
            Globals.ThisAddIn.ShowCustomPanel();
        }
    }
}
