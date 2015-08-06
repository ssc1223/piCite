namespace MSWordpiCite.RibbonButtons
{
    partial class ribpiCiteTab
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Office.Tools.Ribbon.RibbonDialogLauncher ribbonDialogLauncherImpl1 = this.Factory.CreateRibbonDialogLauncher();
            this.TabHome = this.Factory.CreateRibbonTab();
            this.grppiCiteBtns = this.Factory.CreateRibbonGroup();
            this.btnpiCite = this.Factory.CreateRibbonButton();
            this.TabHome.SuspendLayout();
            this.grppiCiteBtns.SuspendLayout();
            // 
            // TabHome
            // 
            this.TabHome.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.TabHome.ControlId.OfficeId = "TabHome";
            this.TabHome.Groups.Add(this.grppiCiteBtns);
            this.TabHome.Label = "Home";
            this.TabHome.Name = "TabHome";
            // 
            // grppiCiteBtns
            // 
            ribbonDialogLauncherImpl1.Visible = false;
            this.grppiCiteBtns.DialogLauncher = ribbonDialogLauncherImpl1;
            this.grppiCiteBtns.Items.Add(this.btnpiCite);
            this.grppiCiteBtns.Label = "πFolio  ";
            this.grppiCiteBtns.Name = "grppiCiteBtns";
            this.grppiCiteBtns.Position = this.Factory.RibbonPosition.BeforeOfficeId("GroupClipboard");
            // 
            // btnpiCite
            // 
            this.btnpiCite.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnpiCite.Image = global::MSWordpiCite.Properties.Resources.pifolioicon;
            this.btnpiCite.Label = "";
            this.btnpiCite.Name = "btnpiCite";
            this.btnpiCite.ShowImage = true;
            this.btnpiCite.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnpiCite_Click);
            // 
            // ribpiCiteTab
            // 
            this.Name = "ribpiCiteTab";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.TabHome);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ribpiCiteTab_Load);
            this.TabHome.ResumeLayout(false);
            this.TabHome.PerformLayout();
            this.grppiCiteBtns.ResumeLayout(false);
            this.grppiCiteBtns.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnpiCite;
        private Microsoft.Office.Tools.Ribbon.RibbonTab TabHome;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grppiCiteBtns;

    }

}
