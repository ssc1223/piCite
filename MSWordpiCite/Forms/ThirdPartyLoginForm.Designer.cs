﻿namespace MSWordpiCite.Forms
{
    partial class ThirdPartyLoginForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wBLogin = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wBLogin
            // 
            this.wBLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wBLogin.Location = new System.Drawing.Point(0, 0);
            this.wBLogin.MinimumSize = new System.Drawing.Size(20, 20);
            this.wBLogin.Name = "wBLogin";
            this.wBLogin.Size = new System.Drawing.Size(629, 465);
            this.wBLogin.TabIndex = 0;
            // 
            // ThirdPartyLoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 465);
            this.Controls.Add(this.wBLogin);
            this.Name = "ThirdPartyLoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login with ...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.wbLoginForm_FormClosed);
            this.Load += new System.EventHandler(this.wbLoginForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wBLogin;
    }
}