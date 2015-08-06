namespace MSWordpiCite.Controls
{
    using MSWordpiCite.Classes;
    partial class MasterControl
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
            this.listIcons = new System.Windows.Forms.ImageList(this.components);
            this.splitParent = new System.Windows.Forms.SplitContainer();
            this.tableTop = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurrentUserName = new System.Windows.Forms.Label();
            this.btnReload = new MSWordpiCite.Classes.CustomButton();
            this.btnLogout = new MSWordpiCite.Classes.CustomButton();
            this.treeFolders = new System.Windows.Forms.TreeView();
            this.btnTemplate = new MSWordpiCite.Classes.CustomButton();
            this.chkQuickStartTip = new System.Windows.Forms.CheckBox();
            this.splitChild = new System.Windows.Forms.SplitContainer();
            this.tableMiddle = new System.Windows.Forms.TableLayoutPanel();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCite = new MSWordpiCite.Classes.CustomButton();
            this.btnStyle = new MSWordpiCite.Classes.CustomButton();
            this.btnRefresh = new MSWordpiCite.Classes.CustomButton();
            this.btnRefList = new MSWordpiCite.Classes.CustomButton();
            this.btnHighlight = new MSWordpiCite.Classes.CustomButton();
            this.lblDownloading = new System.Windows.Forms.Label();
            this.btnShowHidePreview = new MSWordpiCite.Classes.CustomButton();
            this.btnSortBy = new MSWordpiCite.Classes.CustomButton();
            this.listItems = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnImage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDisplay = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnItemFile = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitemSortBy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemShowHidePreview = new System.Windows.Forms.ToolStripMenuItem();
            this.inputSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new MSWordpiCite.Classes.CustomButton();
            this.btnMore = new MSWordpiCite.Classes.CustomButton();
            this.tableBottom = new System.Windows.Forms.TableLayoutPanel();
            this.ctrlPreview = new MSWordpiCite.Controls.PreviewControl();
            this.contextmenuSortBy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitemFilterFields = new System.Windows.Forms.ToolStripMenuItem();
            this.searchOptionsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblLoadingFolders = new System.Windows.Forms.Label();
            this.lblLoadingItems = new System.Windows.Forms.Label();
            this.lblSortBy = new System.Windows.Forms.Label();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLoadingFolders = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLoadingItems = new System.Windows.Forms.FlowLayoutPanel();
            this.cmbViews = new MSWordpiCite.Classes.CustomComboBoxControl();
            this.cmbSortBy = new MSWordpiCite.Classes.CustomComboBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitParent)).BeginInit();
            this.splitParent.Panel1.SuspendLayout();
            this.splitParent.Panel2.SuspendLayout();
            this.splitParent.SuspendLayout();
            this.tableTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitChild)).BeginInit();
            this.splitChild.Panel1.SuspendLayout();
            this.splitChild.Panel2.SuspendLayout();
            this.splitChild.SuspendLayout();
            this.tableMiddle.SuspendLayout();
            this.flowPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listItems)).BeginInit();
            this.contextmenu.SuspendLayout();
            this.tableBottom.SuspendLayout();
            this.flowLoadingFolders.SuspendLayout();
            this.flowLoadingItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // listIcons
            // 
            this.listIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.listIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.listIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // splitParent
            // 
            this.splitParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitParent.Location = new System.Drawing.Point(0, 0);
            this.splitParent.MinimumSize = new System.Drawing.Size(370, 0);
            this.splitParent.Name = "splitParent";
            this.splitParent.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitParent.Panel1
            // 
            this.splitParent.Panel1.Controls.Add(this.tableTop);
            // 
            // splitParent.Panel2
            // 
            this.splitParent.Panel2.Controls.Add(this.splitChild);
            this.splitParent.Size = new System.Drawing.Size(370, 738);
            this.splitParent.SplitterDistance = 341;
            this.splitParent.TabIndex = 0;
            this.splitParent.TabStop = false;
            // 
            // tableTop
            // 
            this.tableTop.AutoScroll = true;
            this.tableTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableTop.ColumnCount = 5;
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableTop.Controls.Add(this.lblCurrentUserName, 0, 0);
            this.tableTop.Controls.Add(this.btnReload, 3, 0);
            this.tableTop.Controls.Add(this.btnLogout, 4, 0);
            this.tableTop.Controls.Add(this.treeFolders, 0, 1);
            this.tableTop.Controls.Add(this.btnTemplate, 2, 0);
            this.tableTop.Controls.Add(this.chkQuickStartTip, 1, 0);
            this.tableTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableTop.Location = new System.Drawing.Point(0, 0);
            this.tableTop.Name = "tableTop";
            this.tableTop.RowCount = 2;
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTop.Size = new System.Drawing.Size(370, 341);
            this.tableTop.TabIndex = 0;
            // 
            // lblCurrentUserName
            // 
            this.lblCurrentUserName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurrentUserName.AutoSize = true;
            this.lblCurrentUserName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCurrentUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentUserName.Location = new System.Drawing.Point(3, 7);
            this.lblCurrentUserName.Name = "lblCurrentUserName";
            this.lblCurrentUserName.Size = new System.Drawing.Size(59, 13);
            this.lblCurrentUserName.TabIndex = 0;
            this.lblCurrentUserName.Text = "First Last";
            // 
            // btnReload
            // 
            this.btnReload.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnReload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnReload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Image = global::MSWordpiCite.Properties.Resources.transfer;
            this.btnReload.Location = new System.Drawing.Point(313, 3);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(24, 22);
            this.btnReload.TabIndex = 2;
            this.btnReload.TabStop = false;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Clicked);
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnLogout.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnLogout.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Image = global::MSWordpiCite.Properties.Resources.logout;
            this.btnLogout.Location = new System.Drawing.Point(343, 3);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(24, 22);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.TabStop = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Clicked);
            // 
            // treeFolders
            // 
            this.treeFolders.BackColor = System.Drawing.SystemColors.Window;
            this.treeFolders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableTop.SetColumnSpan(this.treeFolders, 5);
            this.treeFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFolders.HideSelection = false;
            this.treeFolders.Location = new System.Drawing.Point(3, 31);
            this.treeFolders.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.treeFolders.Name = "treeFolders";
            this.treeFolders.ShowLines = false;
            this.treeFolders.Size = new System.Drawing.Size(364, 310);
            this.treeFolders.TabIndex = 4;
            this.treeFolders.TabStop = false;
            this.treeFolders.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeFolders_KeyUp);
            // 
            // btnTemplate
            // 
            this.btnTemplate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnTemplate.FlatAppearance.BorderSize = 0;
            this.btnTemplate.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnTemplate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnTemplate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTemplate.Image = global::MSWordpiCite.Properties.Resources.ThesisType;
            this.btnTemplate.Location = new System.Drawing.Point(283, 3);
            this.btnTemplate.Name = "btnTemplate";
            this.btnTemplate.Size = new System.Drawing.Size(24, 22);
            this.btnTemplate.TabIndex = 5;
            this.btnTemplate.UseVisualStyleBackColor = true;
            this.btnTemplate.Click += new System.EventHandler(this.btnTemplate_Click);
            // 
            // chkQuickStartTip
            // 
            this.chkQuickStartTip.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkQuickStartTip.Location = new System.Drawing.Point(253, 3);
            this.chkQuickStartTip.Name = "chkQuickStartTip";
            this.chkQuickStartTip.Size = new System.Drawing.Size(24, 22);
            this.chkQuickStartTip.TabIndex = 6;
            this.chkQuickStartTip.UseVisualStyleBackColor = true;
            this.chkQuickStartTip.Visible = false;
            // 
            // splitChild
            // 
            this.splitChild.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitChild.Location = new System.Drawing.Point(0, 0);
            this.splitChild.Name = "splitChild";
            this.splitChild.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitChild.Panel1
            // 
            this.splitChild.Panel1.Controls.Add(this.tableMiddle);
            // 
            // splitChild.Panel2
            // 
            this.splitChild.Panel2.Controls.Add(this.tableBottom);
            this.splitChild.Size = new System.Drawing.Size(370, 393);
            this.splitChild.SplitterDistance = 178;
            this.splitChild.TabIndex = 0;
            this.splitChild.TabStop = false;
            // 
            // tableMiddle
            // 
            this.tableMiddle.AutoScroll = true;
            this.tableMiddle.AutoScrollMinSize = new System.Drawing.Size(300, 0);
            this.tableMiddle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableMiddle.ColumnCount = 4;
            this.tableMiddle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMiddle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableMiddle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableMiddle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableMiddle.Controls.Add(this.flowPanel, 0, 0);
            this.tableMiddle.Controls.Add(this.btnShowHidePreview, 2, 0);
            this.tableMiddle.Controls.Add(this.btnSortBy, 3, 0);
            this.tableMiddle.Controls.Add(this.listItems, 0, 1);
            this.tableMiddle.Controls.Add(this.inputSearch, 0, 4);
            this.tableMiddle.Controls.Add(this.btnSearch, 2, 4);
            this.tableMiddle.Controls.Add(this.btnMore, 3, 4);
            this.tableMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMiddle.Location = new System.Drawing.Point(0, 0);
            this.tableMiddle.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tableMiddle.Name = "tableMiddle";
            this.tableMiddle.RowCount = 5;
            this.tableMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableMiddle.Size = new System.Drawing.Size(370, 178);
            this.tableMiddle.TabIndex = 0;
            this.tableMiddle.TabStop = true;
            // 
            // flowPanel
            // 
            this.flowPanel.AutoSize = true;
            this.tableMiddle.SetColumnSpan(this.flowPanel, 2);
            this.flowPanel.Controls.Add(this.btnCite);
            this.flowPanel.Controls.Add(this.btnStyle);
            this.flowPanel.Controls.Add(this.btnRefresh);
            this.flowPanel.Controls.Add(this.btnRefList);
            this.flowPanel.Controls.Add(this.btnHighlight);
            this.flowPanel.Controls.Add(this.lblDownloading);
            this.flowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanel.Location = new System.Drawing.Point(0, 0);
            this.flowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(310, 28);
            this.flowPanel.TabIndex = 5;
            // 
            // btnCite
            // 
            this.btnCite.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnCite.FlatAppearance.BorderSize = 0;
            this.btnCite.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnCite.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnCite.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnCite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCite.Image = global::MSWordpiCite.Properties.Resources.CitationInsert;
            this.btnCite.Location = new System.Drawing.Point(3, 3);
            this.btnCite.Name = "btnCite";
            this.btnCite.Size = new System.Drawing.Size(24, 22);
            this.btnCite.TabIndex = 0;
            this.btnCite.TabStop = false;
            // 
            // btnStyle
            // 
            this.btnStyle.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnStyle.FlatAppearance.BorderSize = 0;
            this.btnStyle.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnStyle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnStyle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStyle.Image = global::MSWordpiCite.Properties.Resources.CitationStyleChange;
            this.btnStyle.Location = new System.Drawing.Point(33, 3);
            this.btnStyle.Name = "btnStyle";
            this.btnStyle.Size = new System.Drawing.Size(24, 22);
            this.btnStyle.TabIndex = 1;
            this.btnStyle.TabStop = false;
            this.btnStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRefresh
            // 
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Image = global::MSWordpiCite.Properties.Resources.FolderRefresh2;
            this.btnRefresh.Location = new System.Drawing.Point(63, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 22);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.TabStop = false;
            // 
            // btnRefList
            // 
            this.btnRefList.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnRefList.FlatAppearance.BorderSize = 0;
            this.btnRefList.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnRefList.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnRefList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnRefList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefList.Image = global::MSWordpiCite.Properties.Resources.ReferenceLocate;
            this.btnRefList.Location = new System.Drawing.Point(93, 3);
            this.btnRefList.Name = "btnRefList";
            this.btnRefList.Size = new System.Drawing.Size(24, 22);
            this.btnRefList.TabIndex = 4;
            this.btnRefList.TabStop = false;
            // 
            // btnHighlight
            // 
            this.btnHighlight.Enabled = false;
            this.btnHighlight.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnHighlight.FlatAppearance.BorderSize = 0;
            this.btnHighlight.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnHighlight.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnHighlight.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnHighlight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHighlight.Image = global::MSWordpiCite.Properties.Resources.Highlight;
            this.btnHighlight.Location = new System.Drawing.Point(123, 3);
            this.btnHighlight.Name = "btnHighlight";
            this.btnHighlight.Size = new System.Drawing.Size(24, 22);
            this.btnHighlight.TabIndex = 3;
            this.btnHighlight.TabStop = false;
            this.btnHighlight.Visible = false;
            // 
            // lblDownloading
            // 
            this.lblDownloading.AutoSize = true;
            this.lblDownloading.Location = new System.Drawing.Point(153, 0);
            this.lblDownloading.Name = "lblDownloading";
            this.lblDownloading.Padding = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this.lblDownloading.Size = new System.Drawing.Size(83, 21);
            this.lblDownloading.TabIndex = 5;
            this.lblDownloading.Text = "Downloading ... ";
            this.lblDownloading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDownloading.Visible = false;
            // 
            // btnShowHidePreview
            // 
            this.btnShowHidePreview.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnShowHidePreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowHidePreview.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnShowHidePreview.FlatAppearance.BorderSize = 0;
            this.btnShowHidePreview.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnShowHidePreview.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnShowHidePreview.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnShowHidePreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowHidePreview.Image = global::MSWordpiCite.Properties.Resources.preview;
            this.btnShowHidePreview.Location = new System.Drawing.Point(313, 3);
            this.btnShowHidePreview.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnShowHidePreview.Name = "btnShowHidePreview";
            this.btnShowHidePreview.Size = new System.Drawing.Size(24, 22);
            this.btnShowHidePreview.TabIndex = 6;
            this.btnShowHidePreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSortBy
            // 
            this.btnSortBy.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSortBy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSortBy.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnSortBy.FlatAppearance.BorderSize = 0;
            this.btnSortBy.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnSortBy.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnSortBy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnSortBy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSortBy.Image = global::MSWordpiCite.Properties.Resources.sort_asc;
            this.btnSortBy.Location = new System.Drawing.Point(343, 3);
            this.btnSortBy.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnSortBy.Name = "btnSortBy";
            this.btnSortBy.Size = new System.Drawing.Size(24, 22);
            this.btnSortBy.TabIndex = 6;
            this.btnSortBy.TabStop = false;
            this.btnSortBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSortBy.Click += new System.EventHandler(this.btnSortBy_Click);
            // 
            // listItems
            // 
            this.listItems.AllColumns.Add(this.olvColumnImage);
            this.listItems.AllColumns.Add(this.olvColumnDisplay);
            this.listItems.AllColumns.Add(this.olvColumnItemFile);
            this.listItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnImage,
            this.olvColumnDisplay,
            this.olvColumnItemFile});
            this.tableMiddle.SetColumnSpan(this.listItems, 4);
            this.listItems.ContextMenuStrip = this.contextmenu;
            this.listItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.listItems.FullRowSelect = true;
            this.listItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listItems.HideSelection = false;
            this.listItems.Location = new System.Drawing.Point(3, 31);
            this.listItems.Name = "listItems";
            this.listItems.OwnerDraw = true;
            this.listItems.RowHeight = 45;
            this.listItems.ShowGroups = false;
            this.listItems.Size = new System.Drawing.Size(364, 122);
            this.listItems.SmallImageList = this.listIcons;
            this.listItems.TabIndex = 0;
            this.listItems.UseCellFormatEvents = true;
            this.listItems.UseCompatibleStateImageBehavior = false;
            this.listItems.UseTranslucentSelection = true;
            this.listItems.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnImage
            // 
            this.olvColumnImage.Text = "Image";
            this.olvColumnImage.Width = 25;
            // 
            // olvColumnDisplay
            // 
            this.olvColumnDisplay.AspectName = "Display";
            this.olvColumnDisplay.FillsFreeSpace = true;
            this.olvColumnDisplay.Text = "Display";
            this.olvColumnDisplay.Width = 32;
            // 
            // olvColumnItemFile
            // 
            this.olvColumnItemFile.Text = "ItemFile";
            this.olvColumnItemFile.Width = 25;
            // 
            // contextmenu
            // 
            this.contextmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemSortBy,
            this.menuitemShowHidePreview});
            this.contextmenu.Name = "contextmenu";
            this.contextmenu.Size = new System.Drawing.Size(215, 48);
            // 
            // menuitemSortBy
            // 
            this.menuitemSortBy.Name = "menuitemSortBy";
            this.menuitemSortBy.Size = new System.Drawing.Size(214, 22);
            this.menuitemSortBy.Text = "Sort";
            // 
            // menuitemShowHidePreview
            // 
            this.menuitemShowHidePreview.Name = "menuitemShowHidePreview";
            this.menuitemShowHidePreview.Size = new System.Drawing.Size(214, 22);
            this.menuitemShowHidePreview.Text = "Show/Hide item preview";
            // 
            // inputSearch
            // 
            this.inputSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableMiddle.SetColumnSpan(this.inputSearch, 2);
            this.inputSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            this.inputSearch.Location = new System.Drawing.Point(3, 156);
            this.inputSearch.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.inputSearch.Name = "inputSearch";
            this.inputSearch.Size = new System.Drawing.Size(304, 22);
            this.inputSearch.TabIndex = 1;
            this.inputSearch.Text = "Find items ...";
            // 
            // btnSearch
            // 
            this.btnSearch.BackgroundImage = global::MSWordpiCite.Properties.Resources.Filter;
            this.btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Image = global::MSWordpiCite.Properties.Resources.ArrowUp;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnSearch.Location = new System.Drawing.Point(310, 156);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 19);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.TabStop = false;
            // 
            // btnMore
            // 
            this.btnMore.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(170)))), ((int)(((byte)(115)))));
            this.btnMore.FlatAppearance.BorderSize = 0;
            this.btnMore.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnMore.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.btnMore.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(225)))), ((int)(((byte)(205)))));
            this.btnMore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMore.Image = global::MSWordpiCite.Properties.Resources.more_icon;
            this.btnMore.Location = new System.Drawing.Point(343, 156);
            this.btnMore.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.btnMore.Name = "btnMore";
            this.btnMore.Size = new System.Drawing.Size(24, 19);
            this.btnMore.TabIndex = 7;
            this.btnMore.TabStop = false;
            // 
            // tableBottom
            // 
            this.tableBottom.AutoScroll = true;
            this.tableBottom.AutoScrollMinSize = new System.Drawing.Size(300, 0);
            this.tableBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableBottom.Controls.Add(this.ctrlPreview, 0, 0);
            this.tableBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableBottom.Location = new System.Drawing.Point(0, 0);
            this.tableBottom.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tableBottom.Name = "tableBottom";
            this.tableBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableBottom.Size = new System.Drawing.Size(370, 211);
            this.tableBottom.TabIndex = 0;
            // 
            // ctrlPreview
            // 
            this.ctrlPreview.AutoScroll = true;
            this.ctrlPreview.AutoSize = true;
            this.ctrlPreview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ctrlPreview.BackColor = System.Drawing.SystemColors.Window;
            this.ctrlPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctrlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlPreview.Location = new System.Drawing.Point(3, 3);
            this.ctrlPreview.Name = "ctrlPreview";
            this.ctrlPreview.Size = new System.Drawing.Size(364, 205);
            this.ctrlPreview.TabIndex = 6;
            this.ctrlPreview.TabStop = false;
            // 
            // contextmenuSortBy
            // 
            this.contextmenuSortBy.Name = "contextmenuSortBy";
            this.contextmenuSortBy.Size = new System.Drawing.Size(61, 4);
            // 
            // menuitemFilterFields
            // 
            this.menuitemFilterFields.Name = "menuitemFilterFields";
            this.menuitemFilterFields.Size = new System.Drawing.Size(189, 22);
            this.menuitemFilterFields.Text = "Filter options";
            // 
            // searchOptionsContextMenu
            // 
            this.searchOptionsContextMenu.Name = "searchOptionsContextMenu";
            this.searchOptionsContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // lblLoadingFolders
            // 
            this.lblLoadingFolders.AutoSize = true;
            this.lblLoadingFolders.Location = new System.Drawing.Point(3, 0);
            this.lblLoadingFolders.Name = "lblLoadingFolders";
            this.lblLoadingFolders.Size = new System.Drawing.Size(94, 12);
            this.lblLoadingFolders.TabIndex = 0;
            this.lblLoadingFolders.Text = "Loading folders ... ";
            this.lblLoadingFolders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLoadingItems
            // 
            this.lblLoadingItems.Location = new System.Drawing.Point(3, 0);
            this.lblLoadingItems.Name = "lblLoadingItems";
            this.lblLoadingItems.Size = new System.Drawing.Size(160, 23);
            this.lblLoadingItems.TabIndex = 0;
            // 
            // lblSortBy
            // 
            this.lblSortBy.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSortBy.AutoSize = true;
            this.lblSortBy.Location = new System.Drawing.Point(-25, 67);
            this.lblSortBy.Margin = new System.Windows.Forms.Padding(0);
            this.lblSortBy.Name = "lblSortBy";
            this.lblSortBy.Size = new System.Drawing.Size(26, 13);
            this.lblSortBy.TabIndex = 0;
            this.lblSortBy.Text = "Sort";
            this.lblSortBy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLoadingFolders
            // 
            this.flowLoadingFolders.BackColor = System.Drawing.Color.Red;
            this.flowLoadingFolders.Controls.Add(this.lblLoadingFolders);
            this.flowLoadingFolders.Location = new System.Drawing.Point(0, 0);
            this.flowLoadingFolders.Name = "flowLoadingFolders";
            this.flowLoadingFolders.Size = new System.Drawing.Size(200, 100);
            this.flowLoadingFolders.TabIndex = 0;
            // 
            // flowLoadingItems
            // 
            this.flowLoadingItems.BackColor = System.Drawing.Color.Green;
            this.flowLoadingItems.Controls.Add(this.lblLoadingItems);
            this.flowLoadingItems.Location = new System.Drawing.Point(0, 0);
            this.flowLoadingItems.Name = "flowLoadingItems";
            this.flowLoadingItems.Size = new System.Drawing.Size(200, 100);
            this.flowLoadingItems.TabIndex = 0;
            // 
            // cmbViews
            // 
            this.cmbViews.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbViews.IntegralHeight = false;
            this.cmbViews.Location = new System.Drawing.Point(-67, 3);
            this.cmbViews.MaximumSize = new System.Drawing.Size(80, 0);
            this.cmbViews.MinimumSize = new System.Drawing.Size(65, 0);
            this.cmbViews.Name = "cmbViews";
            this.cmbViews.Size = new System.Drawing.Size(65, 20);
            this.cmbViews.TabIndex = 3;
            this.cmbViews.TabStop = false;
            // 
            // cmbSortBy
            // 
            this.cmbSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortBy.IntegralHeight = false;
            this.cmbSortBy.Location = new System.Drawing.Point(-59, 43);
            this.cmbSortBy.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cmbSortBy.MaximumSize = new System.Drawing.Size(75, 0);
            this.cmbSortBy.MinimumSize = new System.Drawing.Size(60, 0);
            this.cmbSortBy.Name = "cmbSortBy";
            this.cmbSortBy.Size = new System.Drawing.Size(60, 20);
            this.cmbSortBy.TabIndex = 2;
            this.cmbSortBy.TabStop = false;
            // 
            // MasterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.splitParent);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(350, 738);
            this.Name = "MasterControl";
            this.Size = new System.Drawing.Size(350, 738);
            this.Load += new System.EventHandler(this.MasterControl_Load);
            this.splitParent.Panel1.ResumeLayout(false);
            this.splitParent.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitParent)).EndInit();
            this.splitParent.ResumeLayout(false);
            this.tableTop.ResumeLayout(false);
            this.tableTop.PerformLayout();
            this.splitChild.Panel1.ResumeLayout(false);
            this.splitChild.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitChild)).EndInit();
            this.splitChild.ResumeLayout(false);
            this.tableMiddle.ResumeLayout(false);
            this.tableMiddle.PerformLayout();
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listItems)).EndInit();
            this.contextmenu.ResumeLayout(false);
            this.tableBottom.ResumeLayout(false);
            this.tableBottom.PerformLayout();
            this.flowLoadingFolders.ResumeLayout(false);
            this.flowLoadingFolders.PerformLayout();
            this.flowLoadingItems.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox inputSearch;
        private BrightIdeasSoftware.ObjectListView listItems;
        private BrightIdeasSoftware.OLVColumn olvColumnImage;
        private BrightIdeasSoftware.OLVColumn olvColumnDisplay;
        private BrightIdeasSoftware.OLVColumn olvColumnItemFile;
        private System.Windows.Forms.TableLayoutPanel tableTop;
        private System.Windows.Forms.TableLayoutPanel tableMiddle;
        private System.Windows.Forms.TableLayoutPanel tableBottom;
        private System.Windows.Forms.ImageList listIcons;
        private System.Windows.Forms.Label lblCurrentUserName;
        private MSWordpiCite.Classes.CustomButton btnLogout;
        private MSWordpiCite.Classes.CustomButton btnReload;
        private MSWordpiCite.Classes.CustomButton btnCite;
        private MSWordpiCite.Classes.CustomButton btnStyle;
        private MSWordpiCite.Classes.CustomButton btnRefresh;
        private MSWordpiCite.Classes.CustomButton btnRefList;
        private MSWordpiCite.Classes.CustomButton btnHighlight;
        private MSWordpiCite.Classes.CustomButton btnShowHidePreview;
        private MSWordpiCite.Classes.CustomButton btnSortBy;
        private MSWordpiCite.Classes.CustomButton btnMore;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.Label lblSortBy;
        private System.Windows.Forms.Label lblLoadingFolders;
        private System.Windows.Forms.Label lblLoadingItems;
        private System.Windows.Forms.Label lblDownloading;
        private System.Windows.Forms.FlowLayoutPanel flowLoadingFolders;
        private System.Windows.Forms.FlowLayoutPanel flowLoadingItems;
        private MSWordpiCite.Classes.CustomComboBoxControl cmbSortBy;
        private MSWordpiCite.Classes.CustomComboBoxControl cmbViews;
        private MSWordpiCite.Controls.PreviewControl ctrlPreview;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.SplitContainer splitParent;
        private System.Windows.Forms.SplitContainer splitChild;
        private System.Windows.Forms.ContextMenuStrip contextmenu;
        private System.Windows.Forms.ContextMenuStrip contextmenuSortBy;
        private System.Windows.Forms.ContextMenuStrip searchOptionsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuitemSortBy;
        private System.Windows.Forms.ToolStripMenuItem menuitemFilterFields;
        private System.Windows.Forms.ToolStripMenuItem menuitemShowHidePreview;
        private CustomButton btnTemplate;
        private System.Windows.Forms.TreeView treeFolders;
        private System.Windows.Forms.CheckBox chkQuickStartTip;
        private CustomButton btnSearch;
    }
}
