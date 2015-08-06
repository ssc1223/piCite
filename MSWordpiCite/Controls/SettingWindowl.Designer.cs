namespace MSWordWizCite.Controls
{
    partial class SettingControl
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
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblSettingTitle = new System.Windows.Forms.Label();
            this.chkRememberEmail = new System.Windows.Forms.CheckBox();
            this.lblSettingLanguague = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.AutoSize = true;
            this.tableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayout.ColumnCount = 3;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayout.Controls.Add(this.lblSettingTitle, 0, 0);
            this.tableLayout.Controls.Add(this.chkRememberEmail, 0, 1);
            this.tableLayout.Controls.Add(this.lblSettingLanguague, 0, 2);
            this.tableLayout.Controls.Add(this.btnOK, 1, 4);
            this.tableLayout.Controls.Add(this.btnCancel, 2, 4);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.MaximumSize = new System.Drawing.Size(300, 0);
            this.tableLayout.MinimumSize = new System.Drawing.Size(200, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 6;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Size = new System.Drawing.Size(300, 170);
            this.tableLayout.TabIndex = 0;
            // 
            // lblSettingTitle
            // 
            this.lblSettingTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSettingTitle.AutoSize = true;
            this.tableLayout.SetColumnSpan(this.lblSettingTitle, 3);
            this.lblSettingTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSettingTitle.Location = new System.Drawing.Point(3, 5);
            this.lblSettingTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 30);
            this.lblSettingTitle.Name = "lblSettingTitle";
            this.lblSettingTitle.Size = new System.Drawing.Size(92, 16);
            this.lblSettingTitle.TabIndex = 0;
            this.lblSettingTitle.Text = "Preferences";
            // 
            // chkRememberEmail
            // 
            this.chkRememberEmail.AutoSize = true;
            this.tableLayout.SetColumnSpan(this.chkRememberEmail, 3);
            this.chkRememberEmail.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkRememberEmail.Location = new System.Drawing.Point(10, 54);
            this.chkRememberEmail.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.chkRememberEmail.Name = "chkRememberEmail";
            this.chkRememberEmail.Size = new System.Drawing.Size(139, 17);
            this.chkRememberEmail.TabIndex = 0;
            this.chkRememberEmail.Text = "Remember logined email";
            this.chkRememberEmail.UseVisualStyleBackColor = true;
            // 
            // lblSettingLanguague
            // 
            this.lblSettingLanguague.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSettingLanguague.AutoSize = true;
            this.tableLayout.SetColumnSpan(this.lblSettingLanguague, 3);
            this.lblSettingLanguague.Location = new System.Drawing.Point(25, 82);
            this.lblSettingLanguague.Margin = new System.Windows.Forms.Padding(25, 8, 3, 2);
            this.lblSettingLanguague.Name = "lblSettingLanguague";
            this.lblSettingLanguague.Size = new System.Drawing.Size(55, 13);
            this.lblSettingLanguague.TabIndex = 0;
            this.lblSettingLanguague.Text = "Language";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Location = new System.Drawing.Point(191, 144);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(50, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Location = new System.Drawing.Point(247, 144);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(50, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayout);
            this.Name = "SettingControl";
            this.Size = new System.Drawing.Size(300, 126);
            this.Load += new System.EventHandler(this.SettingControl_Load);
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Label lblSettingTitle;
        private System.Windows.Forms.CheckBox chkRememberEmail;
        private System.Windows.Forms.Label lblSettingLanguague;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
