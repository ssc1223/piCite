namespace MSWordpiCite.Controls
{
    partial class CitationStylePreviewControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CitationStylePreviewControl));
            this.panel = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPremiumServices = new System.Windows.Forms.Label();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.lblLoading = new System.Windows.Forms.Label();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnEditStyle = new MSWordpiCite.Classes.CustomButton();
            this.btnUpgradeAccount = new MSWordpiCite.Classes.CustomButton();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.AutoSize = true;
            this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel.BackColor = System.Drawing.Color.White;
            this.panel.ColumnCount = 2;
            this.panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.panel.Controls.Add(this.lblTitle, 0, 0);
            this.panel.Controls.Add(this.btnEditStyle, 1, 0);
            this.panel.Controls.Add(this.lblPremiumServices, 0, 1);
            this.panel.Controls.Add(this.btnUpgradeAccount, 1, 1);
            this.panel.Controls.Add(this.webBrowser, 0, 2);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.RowCount = 3;
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel.Size = new System.Drawing.Size(505, 78);
            this.panel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.SystemColors.Window;
            this.lblTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(349, 32);
            this.lblTitle.TabIndex = 0;
            // 
            // lblPremiumServices
            // 
            this.lblPremiumServices.AutoSize = true;
            this.lblPremiumServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPremiumServices.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPremiumServices.ForeColor = System.Drawing.Color.Orange;
            this.lblPremiumServices.Location = new System.Drawing.Point(3, 35);
            this.lblPremiumServices.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblPremiumServices.Name = "lblPremiumServices";
            this.lblPremiumServices.Size = new System.Drawing.Size(349, 33);
            this.lblPremiumServices.TabIndex = 1;
            this.lblPremiumServices.Text = "* Only available for Premium and Institutional users";
            this.lblPremiumServices.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPremiumServices.Visible = false;
            // 
            // webBrowser
            // 
            this.panel.SetColumnSpan(this.webBrowser, 2);
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 68);
            this.webBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(505, 10);
            this.webBrowser.TabIndex = 1;
            this.webBrowser.TabStop = false;
            // 
            // lblLoading
            // 
            this.lblLoading.BackColor = System.Drawing.Color.White;
            this.lblLoading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblLoading.Image = global::MSWordpiCite.Properties.Resources.loading;
            this.lblLoading.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLoading.Location = new System.Drawing.Point(0, 41);
            this.lblLoading.Margin = new System.Windows.Forms.Padding(250, 250, 3, 0);
            this.lblLoading.MinimumSize = new System.Drawing.Size(185, 20);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(185, 20);
            this.lblLoading.TabIndex = 0;
            this.lblLoading.Text = "  Loading citation style preview ...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLoading.Visible = false;
            // 
            // btnEditStyle
            // 
            this.btnEditStyle.AutoSize = true;
            this.btnEditStyle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEditStyle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEditStyle.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnEditStyle.FlatAppearance.BorderSize = 0;
            this.btnEditStyle.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnEditStyle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnEditStyle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnEditStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditStyle.Image = ((System.Drawing.Image)(resources.GetObject("btnEditStyle.Image")));
            this.btnEditStyle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditStyle.Location = new System.Drawing.Point(358, 3);
            this.btnEditStyle.Name = "btnEditStyle";
            this.btnEditStyle.Size = new System.Drawing.Size(144, 26);
            this.btnEditStyle.TabIndex = 0;
            // 
            // btnUpgradeAccount
            // 
            this.btnUpgradeAccount.AutoSize = true;
            this.btnUpgradeAccount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUpgradeAccount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUpgradeAccount.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnUpgradeAccount.FlatAppearance.BorderSize = 0;
            this.btnUpgradeAccount.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnUpgradeAccount.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnUpgradeAccount.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnUpgradeAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpgradeAccount.Image = global::MSWordpiCite.Properties.Resources.premiumbtn;
            this.btnUpgradeAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpgradeAccount.Location = new System.Drawing.Point(358, 35);
            this.btnUpgradeAccount.MinimumSize = new System.Drawing.Size(0, 30);
            this.btnUpgradeAccount.Name = "btnUpgradeAccount";
            this.btnUpgradeAccount.Size = new System.Drawing.Size(144, 30);
            this.btnUpgradeAccount.TabIndex = 2;
            this.btnUpgradeAccount.Visible = false;
            // 
            // CitationStylePreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.panel);
            this.Name = "CitationStylePreviewControl";
            this.Size = new System.Drawing.Size(505, 78);
            this.Load += new System.EventHandler(this.CitationStylePreviewControl_Load);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TableLayoutPanel panel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblPremiumServices;
        private MSWordpiCite.Classes.CustomButton btnEditStyle;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.WebBrowser webBrowser;

        #endregion
        private MSWordpiCite.Classes.CustomButton btnUpgradeAccount;
        private System.Windows.Forms.Label lblLoading;
    }
}
