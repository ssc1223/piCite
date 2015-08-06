namespace MSWordpiCite.Forms
{
    partial class TemplateForm
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
            this.treeTemplate = new System.Windows.Forms.TreeView();
            this.grpTemplatePreview = new System.Windows.Forms.GroupBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.lblLoading = new System.Windows.Forms.Label();
            this.grpTemplatePreview.SuspendLayout();
            this.pnlPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeTemplate
            // 
            this.treeTemplate.Location = new System.Drawing.Point(12, 12);
            this.treeTemplate.Name = "treeTemplate";
            this.treeTemplate.Size = new System.Drawing.Size(275, 626);
            this.treeTemplate.TabIndex = 1;
            this.treeTemplate.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeTemplate_AfterExpand);
            this.treeTemplate.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeTemplate_NodeMouseClick);
            // 
            // grpTemplatePreview
            // 
            this.grpTemplatePreview.Controls.Add(this.pnlPreview);
            this.grpTemplatePreview.Location = new System.Drawing.Point(293, 12);
            this.grpTemplatePreview.Name = "grpTemplatePreview";
            this.grpTemplatePreview.Size = new System.Drawing.Size(650, 626);
            this.grpTemplatePreview.TabIndex = 2;
            this.grpTemplatePreview.TabStop = false;
            this.grpTemplatePreview.Text = "Preview";
            // 
            // pnlPreview
            // 
            this.pnlPreview.AutoScroll = true;
            this.pnlPreview.Controls.Add(this.lblLoading);
            this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPreview.Location = new System.Drawing.Point(3, 18);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(644, 605);
            this.pnlPreview.TabIndex = 1;
            // 
            // lblLoading
            // 
            this.lblLoading.BackColor = System.Drawing.Color.White;
            this.lblLoading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblLoading.Image = global::MSWordpiCite.Properties.Resources.loading;
            this.lblLoading.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLoading.Location = new System.Drawing.Point(230, 293);
            this.lblLoading.Margin = new System.Windows.Forms.Padding(250, 231, 3, 0);
            this.lblLoading.MinimumSize = new System.Drawing.Size(185, 18);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(185, 18);
            this.lblLoading.TabIndex = 1;
            this.lblLoading.Text = "  Loading Templates, Please wait...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLoading.Visible = false;
            // 
            // TemplateForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 650);
            this.Controls.Add(this.grpTemplatePreview);
            this.Controls.Add(this.treeTemplate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TemplateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Template Selection";
            this.grpTemplatePreview.ResumeLayout(false);
            this.pnlPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeTemplate;
        private System.Windows.Forms.GroupBox grpTemplatePreview;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.Label lblLoading;
    }
}