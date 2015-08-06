namespace MSWordWizCite.Controls
{
    partial class CitationStyleControl
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.splitter = new System.Windows.Forms.SplitContainer();
            this.tableTop = new System.Windows.Forms.TableLayoutPanel();
            this.tableControls = new System.Windows.Forms.TableLayoutPanel();
            this.flowControls = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new MSWordWizCite.Classes.CustomButton();
            this.btnReload = new MSWordWizCite.Classes.CustomButton();
            this.btnShowAllStyles = new MSWordWizCite.Classes.CustomButton();
            this.listCitations = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnName = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnIcon = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnType = new BrightIdeasSoftware.OLVColumn();
            this.inputSearch = new System.Windows.Forms.TextBox();
            this.listGroupIcons = new System.Windows.Forms.ImageList();
            this.btnSearch = new MSWordWizCite.Classes.CustomButton();
            this.tableBottom = new System.Windows.Forms.TableLayoutPanel();
            this.ctrlPreview = new MSWordWizCite.Controls.CitationStylePreviewControl();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            this.tableTop.SuspendLayout();
            this.tableControls.SuspendLayout();
            this.flowControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listCitations)).BeginInit();
            this.tableBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Location = new System.Drawing.Point(3, 3);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(151, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Choose a citation style";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter.Location = new System.Drawing.Point(0, 0);
            this.splitter.Name = "splitter";
            this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.Controls.Add(this.tableTop);
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.tableBottom);
            this.splitter.Size = new System.Drawing.Size(0, 0);
            this.splitter.SplitterDistance = 25;
            this.splitter.TabIndex = 0;
            // 
            // tableTop
            // 
            this.tableTop.AutoSize = true;
            this.tableTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableTop.ColumnCount = 2;
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableTop.Controls.Add(this.tableControls, 0, 0);
            this.tableTop.Controls.Add(this.listCitations, 0, 1);
            this.tableTop.Controls.Add(this.inputSearch, 0, 2);
            this.tableTop.Controls.Add(this.btnSearch, 1, 2);
            this.tableTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableTop.Location = new System.Drawing.Point(0, 0);
            this.tableTop.Name = "tableTop";
            this.tableTop.RowCount = 3;
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableTop.Size = new System.Drawing.Size(0, 25);
            this.tableTop.TabIndex = 0;
            // 
            // tableControls
            // 
            this.tableControls.AutoSize = true;
            this.tableControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableControls.ColumnCount = 2;
            this.tableTop.SetColumnSpan(this.tableControls, 2);
            this.tableControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableControls.Controls.Add(this.lblTitle, 0, 0);
            this.tableControls.Controls.Add(this.flowControls, 1, 0);
            this.tableControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableControls.Location = new System.Drawing.Point(3, 3);
            this.tableControls.Name = "tableControls";
            this.tableControls.RowCount = 1;
            this.tableControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableControls.Size = new System.Drawing.Size(1, 29);
            this.tableControls.TabIndex = 1;
            // 
            // flowControls
            // 
            this.flowControls.AutoSize = true;
            this.flowControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowControls.Controls.Add(this.btnClose);
            this.flowControls.Controls.Add(this.btnReload);
            this.flowControls.Controls.Add(this.btnShowAllStyles);
            this.flowControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowControls.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowControls.Location = new System.Drawing.Point(157, 0);
            this.flowControls.Margin = new System.Windows.Forms.Padding(0);
            this.flowControls.Name = "flowControls";
            this.flowControls.Size = new System.Drawing.Size(90, 30);
            this.flowControls.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::MSWordWizCite.Properties.Resources.logout;
            this.btnClose.Location = new System.Drawing.Point(63, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(24, 24);
            this.btnClose.TabIndex = 0;
            this.btnClose.TabStop = false;
            // 
            // btnReload
            // 
            this.btnReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnReload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnReload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Image = global::MSWordWizCite.Properties.Resources.transfer;
            this.btnReload.Location = new System.Drawing.Point(33, 3);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(24, 24);
            this.btnReload.TabIndex = 1;
            this.btnReload.TabStop = false;
            // 
            // btnShowAllStyles
            // 
            this.btnShowAllStyles.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnShowAllStyles.FlatAppearance.BorderSize = 0;
            this.btnShowAllStyles.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnShowAllStyles.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnShowAllStyles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnShowAllStyles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowAllStyles.Image = global::MSWordWizCite.Properties.Resources.DisplayIcon;
            this.btnShowAllStyles.Location = new System.Drawing.Point(3, 3);
            this.btnShowAllStyles.Name = "btnShowAllStyles";
            this.btnShowAllStyles.Size = new System.Drawing.Size(24, 24);
            this.btnShowAllStyles.TabIndex = 2;
            this.btnShowAllStyles.TabStop = false;
            // 
            // listCitations
            // 
            this.listCitations.AllColumns.Add(this.olvColumnName);
            this.listCitations.AllColumns.Add(this.olvColumnIcon);
            this.listCitations.AllColumns.Add(this.olvColumnType);
            this.listCitations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;            
            this.listCitations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnIcon,
            this.olvColumnName});
            this.tableTop.SetColumnSpan(this.listCitations, 2);
            this.listCitations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listCitations.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listCitations.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.listCitations.FullRowSelect = true;
            this.listCitations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listCitations.ShowGroups = true;
            this.listCitations.HideSelection = false;
            this.listCitations.Location = new System.Drawing.Point(3, 38);
            this.listCitations.MultiSelect = false;
            this.listCitations.Name = "listCitations";
            this.listCitations.OwnerDraw = true;
            this.listCitations.RowHeight = 24;
            this.listCitations.Size = new System.Drawing.Size(1, 1);
            this.listCitations.ShowItemCountOnGroups = true;
            this.listCitations.GroupWithItemCountFormat = "";
            this.listCitations.TabIndex = 0;
            this.listCitations.UseCellFormatEvents = true;
            this.listCitations.UseCompatibleStateImageBehavior = false;
            this.listCitations.UseTranslucentSelection = true;
            this.listCitations.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnName
            // 
            this.olvColumnType.AspectName = "Type";
            this.olvColumnType.Text = "Type";

            this.olvColumnName.AspectName = "StyleName";
            this.olvColumnName.FillsFreeSpace = true;
            this.olvColumnName.Text = "Name";
            // 
            // olvColumnIcon
            // 
            this.olvColumnIcon.Text = "Image";
            this.olvColumnIcon.Width = 24;
            // 
            // inputSearch
            // 
            this.inputSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.inputSearch.Location = new System.Drawing.Point(3, -2);
            this.inputSearch.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.inputSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            this.inputSearch.Name = "inputSearch";
            this.inputSearch.Size = new System.Drawing.Size(1, 21);
            this.inputSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Image = global::MSWordWizCite.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(-26, -2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(24, 24);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.TabStop = false;
            // 
            // tableBottom
            // 
            this.tableBottom.AutoSize = true;
            this.tableBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableBottom.Controls.Add(this.ctrlPreview, 0, 0);
            this.tableBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableBottom.Location = new System.Drawing.Point(0, 0);
            this.tableBottom.Name = "tableBottom";
            this.tableBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableBottom.Size = new System.Drawing.Size(0, 25);
            this.tableBottom.TabIndex = 4;
            this.tableBottom.TabStop = false;
            // 
            // ctrlPreview
            // 
            this.ctrlPreview.AutoSize = true;
            this.ctrlPreview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ctrlPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctrlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlPreview.Location = new System.Drawing.Point(3, 3);
            this.ctrlPreview.Name = "ctrlPreview";
            this.ctrlPreview.Size = new System.Drawing.Size(194, 19);
            this.ctrlPreview.TabIndex = 2;
            this.ctrlPreview.TabStop = false;
            // 
            // CitationStyleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.splitter);
            this.Name = "CitationStyleControl";
            this.Size = new System.Drawing.Size(0, 0);
            this.Load += new System.EventHandler(this.CitationStyleControl_Load);
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel1.PerformLayout();
            this.splitter.Panel2.ResumeLayout(false);
            this.splitter.Panel2.PerformLayout();
            this.splitter.ResumeLayout(false);
            this.tableTop.ResumeLayout(false);
            this.tableTop.PerformLayout();
            this.tableControls.ResumeLayout(false);
            this.tableControls.PerformLayout();
            this.flowControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listCitations)).EndInit();
            this.tableBottom.ResumeLayout(false);
            this.tableBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.SplitContainer splitter;
        private System.Windows.Forms.TableLayoutPanel tableTop;
        private System.Windows.Forms.TableLayoutPanel tableBottom;
        private BrightIdeasSoftware.ObjectListView listCitations;
        private BrightIdeasSoftware.OLVColumn olvColumnIcon;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnType;
        private System.Windows.Forms.TextBox inputSearch;
        private MSWordWizCite.Classes.CustomButton btnSearch;
        private MSWordWizCite.Controls.CitationStylePreviewControl ctrlPreview;
        private System.Windows.Forms.TableLayoutPanel tableControls;
        private System.Windows.Forms.FlowLayoutPanel flowControls;
        private MSWordWizCite.Classes.CustomButton btnClose;
        private MSWordWizCite.Classes.CustomButton btnShowAllStyles;
        private MSWordWizCite.Classes.CustomButton btnReload;
        private System.Windows.Forms.ImageList listGroupIcons;
        private System.Windows.Forms.ToolTip tooltip;

        #endregion
    }
}
