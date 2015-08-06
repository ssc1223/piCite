namespace MSWordpiCite.Forms
{
    partial class CitationStyleWindow
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
            this.splitter = new System.Windows.Forms.SplitContainer();
            this.tableTop = new System.Windows.Forms.TableLayoutPanel();
            this.listCitations = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnIcon = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.inputSearch = new System.Windows.Forms.TextBox();
            this.btnShowAllStyles = new MSWordpiCite.Classes.CustomButton();
            this.tableBottom = new System.Windows.Forms.TableLayoutPanel();
            this.ctrlPreview = new MSWordpiCite.Controls.CitationStylePreviewControl();
            this.flowControls = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.listGroupIcons = new System.Windows.Forms.ImageList(this.components);
            this.flowLoadingStyles = new System.Windows.Forms.FlowLayoutPanel();
            this.lblLoadingStyles = new System.Windows.Forms.Label();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            this.tableTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listCitations)).BeginInit();
            this.tableBottom.SuspendLayout();
            this.flowControls.SuspendLayout();
            this.flowLoadingStyles.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter.IsSplitterFixed = true;
            this.splitter.Location = new System.Drawing.Point(0, 0);
            this.splitter.Name = "splitter";
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.Controls.Add(this.tableTop);
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.tableBottom);
            this.splitter.Size = new System.Drawing.Size(850, 415);
            this.splitter.SplitterDistance = 380;
            this.splitter.TabIndex = 0;
            // 
            // tableTop
            // 
            this.tableTop.AutoSize = true;
            this.tableTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableTop.ColumnCount = 2;
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.Controls.Add(this.listCitations, 0, 0);
            this.tableTop.Controls.Add(this.inputSearch, 0, 1);
            this.tableTop.Controls.Add(this.btnShowAllStyles, 1, 1);
            this.tableTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableTop.Location = new System.Drawing.Point(0, 0);
            this.tableTop.Name = "tableTop";
            this.tableTop.RowCount = 2;
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableTop.Size = new System.Drawing.Size(380, 415);
            this.tableTop.TabIndex = 0;
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
            this.listCitations.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.listCitations.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.listCitations.FullRowSelect = true;
            this.listCitations.GroupWithItemCountFormat = "";
            this.listCitations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listCitations.HideSelection = false;
            this.listCitations.Location = new System.Drawing.Point(3, 3);
            this.listCitations.MultiSelect = false;
            this.listCitations.Name = "listCitations";
            this.listCitations.OwnerDraw = true;
            this.listCitations.RowHeight = 24;
            this.listCitations.ShowItemCountOnGroups = true;
            this.listCitations.Size = new System.Drawing.Size(374, 381);
            this.listCitations.TabIndex = 0;
            this.listCitations.UseCellFormatEvents = true;
            this.listCitations.UseCompatibleStateImageBehavior = false;
            this.listCitations.UseTranslucentSelection = true;
            this.listCitations.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnName
            // 
            this.olvColumnName.AspectName = "StyleName";
            this.olvColumnName.FillsFreeSpace = true;
            this.olvColumnName.Text = "Name";
            // 
            // olvColumnIcon
            // 
            this.olvColumnIcon.Text = "Image";
            this.olvColumnIcon.Width = 24;
            // 
            // olvColumnType
            // 
            this.olvColumnType.AspectName = "Type";
            this.olvColumnType.Text = "Type";
            // 
            // inputSearch
            // 
            this.inputSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.inputSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            this.inputSearch.Location = new System.Drawing.Point(3, 392);
            this.inputSearch.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.inputSearch.Name = "inputSearch";
            this.inputSearch.Size = new System.Drawing.Size(350, 20);
            this.inputSearch.TabIndex = 1;
            // 
            // btnShowAllStyles
            // 
            this.btnShowAllStyles.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnShowAllStyles.FlatAppearance.BorderSize = 0;
            this.btnShowAllStyles.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnShowAllStyles.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnShowAllStyles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnShowAllStyles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowAllStyles.Image = global::MSWordpiCite.Properties.Resources.DisplayIcon;
            this.btnShowAllStyles.Location = new System.Drawing.Point(356, 390);
            this.btnShowAllStyles.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnShowAllStyles.Name = "btnShowAllStyles";
            this.btnShowAllStyles.Size = new System.Drawing.Size(24, 22);
            this.btnShowAllStyles.TabIndex = 2;
            this.btnShowAllStyles.TabStop = false;
            // 
            // tableBottom
            // 
            this.tableBottom.AutoSize = true;
            this.tableBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableBottom.Controls.Add(this.ctrlPreview, 0, 0);
            this.tableBottom.Controls.Add(this.flowControls, 0, 1);
            this.tableBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableBottom.Location = new System.Drawing.Point(0, 0);
            this.tableBottom.Name = "tableBottom";
            this.tableBottom.RowCount = 2;
            this.tableBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableBottom.Size = new System.Drawing.Size(466, 415);
            this.tableBottom.TabIndex = 4;
            // 
            // ctrlPreview
            // 
            this.ctrlPreview.AutoSize = true;
            this.ctrlPreview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ctrlPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctrlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlPreview.Location = new System.Drawing.Point(3, 3);
            this.ctrlPreview.Name = "ctrlPreview";
            this.ctrlPreview.Size = new System.Drawing.Size(507, 381);
            this.ctrlPreview.TabIndex = 0;
            // 
            // flowControls
            // 
            this.flowControls.AutoSize = true;
            this.flowControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowControls.Controls.Add(this.btnCancel);
            this.flowControls.Controls.Add(this.btnOK);
            this.flowControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowControls.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowControls.Location = new System.Drawing.Point(0, 387);
            this.flowControls.Margin = new System.Windows.Forms.Padding(0);
            this.flowControls.Name = "flowControls";
            this.flowControls.Size = new System.Drawing.Size(513, 28);
            this.flowControls.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(435, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(354, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 21);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            // 
            // listGroupIcons
            // 
            this.listGroupIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.listGroupIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.listGroupIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // flowLoadingStyles
            // 
            this.flowLoadingStyles.AutoSize = true;
            this.flowLoadingStyles.BackColor = System.Drawing.Color.White;
            this.flowLoadingStyles.Controls.Add(this.lblLoadingStyles);
            this.flowLoadingStyles.Location = new System.Drawing.Point(0, 0);
            this.flowLoadingStyles.Name = "flowLoadingStyles";
            this.flowLoadingStyles.Size = new System.Drawing.Size(200, 100);
            this.flowLoadingStyles.TabIndex = 0;
            // 
            // lblLoadingStyles
            // 
            this.lblLoadingStyles.AutoSize = true;
            this.lblLoadingStyles.Location = new System.Drawing.Point(3, 0);
            this.lblLoadingStyles.Name = "lblLoadingStyles";
            this.lblLoadingStyles.Size = new System.Drawing.Size(121, 12);
            this.lblLoadingStyles.TabIndex = 1;
            this.lblLoadingStyles.Text = "Loading citation styles ...";
            this.lblLoadingStyles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CitationStyleWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 415);
            this.Controls.Add(this.splitter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CitationStyleWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Choose a citation style";
            this.Load += new System.EventHandler(this.CitationStyleForm_Load);
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel1.PerformLayout();
            this.splitter.Panel2.ResumeLayout(false);
            this.splitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
            this.splitter.ResumeLayout(false);
            this.tableTop.ResumeLayout(false);
            this.tableTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listCitations)).EndInit();
            this.tableBottom.ResumeLayout(false);
            this.tableBottom.PerformLayout();
            this.flowControls.ResumeLayout(false);
            this.flowLoadingStyles.ResumeLayout(false);
            this.flowLoadingStyles.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.SplitContainer splitter;
        private System.Windows.Forms.TableLayoutPanel tableTop;
        private System.Windows.Forms.TableLayoutPanel tableBottom;
        private BrightIdeasSoftware.ObjectListView listCitations;
        private BrightIdeasSoftware.OLVColumn olvColumnIcon;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnType;
        private System.Windows.Forms.TextBox inputSearch;
        private MSWordpiCite.Controls.CitationStylePreviewControl ctrlPreview;
        private System.Windows.Forms.FlowLayoutPanel flowControls;
        private MSWordpiCite.Classes.CustomButton btnShowAllStyles;
        private System.Windows.Forms.ImageList listGroupIcons;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLoadingStyles;
        private System.Windows.Forms.Label lblLoadingStyles;

        #endregion
    }
}
