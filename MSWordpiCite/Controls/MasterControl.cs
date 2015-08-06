using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using MSWordpiCite.Classes;
using MSWordpiCite.Entities;
using MSWordpiCite.Enums;
using MSWordpiCite.Formatter;
using MSWordpiCite.Tools;
using BrightIdeasSoftware;
using MSWordpiCite.Threads;
using MSWordpiCite.Forms;

using System.Deployment.Application;
using System.Net;

namespace MSWordpiCite.Controls
{
    public delegate void DelegateLoadingFolderFinished(IEnumerable<Folder> folders);
    public delegate void DelegateLoadingColleagues(IEnumerable<Colleague> colleagues, TreeNode currNode);
    public delegate void DelegateLoadingItemsFinished(List<ItemMasterRow> items, bool bfiltered);
    public delegate void DelegateFileDownloadProgressChanged(int percentage);
    public delegate void DelegateLoadingItemsProgress(int current, int total);
    public delegate void DelegateSearchingItemsFinished(List<ItemMasterRow> items);
    public delegate void DelegateInsertFinished();
    public delegate void DelegateFileDownloaded();
    public delegate void DelegateRefreshFinished();
    public delegate void DelegateFocusFinished();

    public partial class MasterControl : UserControl
    {
        #region To disable WebBrowser click sound

        const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        const int SET_FEATURE_ON_PROCESS = 0x00000002;
        [System.Runtime.InteropServices.DllImport("urlmon.dll")]
        [System.Runtime.InteropServices.PreserveSig]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(int FeatureEntry, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] int dwFlags, bool fEnable);

        #endregion

        #region Variables

        private IEnumerable<Folder> listFolders;
        private IEnumerable<ItemMasterRow> itemList;

        private bool bFolderSelected = false;
        private bool bFilterOptionClicked = false;
        private bool bSearchFocused = false;
        private bool bMoving = false;
        private bool bUsingCitedList = false;
        private bool bBuildingColleaguesList = false;
        private bool bDownloadingFile = false;
        
        private BrightIdeasSoftware.RowBorderDecoration rbd;
        private BrightIdeasSoftware.RowBorderDecoration alternateRBD;

        private Logger log = Globals.ThisAddIn.log;
        private Folder selectedFolder;
        private SortByType currentSortBy;
        private SearchOptions currentSearchOption;
        private DocumentFormatter formatter;
        private TreeNode nodeCitedRef;
        private TreeNode nodeSearchDBs;
        private ProgressForm progress;
        private MasterControlThread masterControlThread;
        private Thread masterThread;
        private static TemplateForm Templateform;


        public bool bIsAscending = true;
        public DelegateLoadingFolderFinished delegateLoadingFolderFinished;
        public DelegateLoadingColleagues delegateLoadingColleaguesFinished;
        public DelegateLoadingItemsFinished delegateLoadingItemsFinished;
        public DelegateFileDownloaded delegateFileDownloaded;
        public DelegateRefreshFinished delegateRefreshFinished;
        public DelegateInsertFinished delegateInsertFinished;
        public DelegateFocusFinished delegateFocusFinished;
        public DelegateLoadingItemsProgress delegateLoadingItemsProgress;
        public DelegateFileDownloadProgressChanged delegateDownloadingProgressChanged;
        public DelegateSearchingItemsFinished delegateSearchingItemsFinished;

        #endregion

        public MasterControl()
        {
            InitializeComponent();
            this.formatter = new DocumentFormatter();
            this.ctrlPreview.Master = this;
            this.bIsAscending = Properties.Settings.Default.DEFAULT_IS_SORTDIRECTION_ASCENDING;
        }

        #region Initialization

        private void initializeUI()
        {
            try
            {
                this.Width = Globals.ThisAddIn.iTempWidth;
                this.Height = Globals.ThisAddIn.iTempHeight;

                BrightIdeasSoftware.TextOverlay textOverlay = this.listItems.EmptyListMsgOverlay as BrightIdeasSoftware.TextOverlay;
                textOverlay.TextColor = Color.FromArgb(255, 96, 96, 96);
                textOverlay.BackColor = System.Drawing.Color.White;
                textOverlay.BorderWidth = 0;
                textOverlay.InsetY = 0;
                textOverlay.Font = new System.Drawing.Font("Serif", 9);
                                
                rbd = new BrightIdeasSoftware.RowBorderDecoration();
                rbd.BorderPen = new Pen(Color.FromArgb(170, Color.DarkGray), 1);
                rbd.FillBrush = new SolidBrush(Color.FromArgb(40, Color.Black));
                rbd.BoundsPadding = new Size(-2, -2);
                rbd.CornerRounding = 6.0f;
                this.listItems.SelectedRowDecoration = rbd;
                this.listItems.CellToolTip.ReshowDelay = 0;

                alternateRBD = new BrightIdeasSoftware.RowBorderDecoration();
                alternateRBD.BorderPen = new Pen(Color.LightGray, 1);
                alternateRBD.FillBrush = null;
                alternateRBD.BoundsPadding = new Size(2, 0);
                alternateRBD.CornerRounding = 0;

                this.menuitemSortBy.DropDownItems.AddRange(getSortByMenuItems());
                this.menuitemSortBy.DropDownItems.Add(new ToolStripSeparator());
                this.menuitemSortBy.DropDownItems.AddRange(getSortByDirectionMenuItems());
                ((ToolStripMenuItem)this.menuitemSortBy.DropDownItems[Properties.Settings.Default.DEFAULT_SORTBY + ""]).CheckState = CheckState.Indeterminate;
                ((ToolStripMenuItem)this.menuitemSortBy.DropDownItems[Properties.Settings.Default.DEFAULT_SORTBY + ""]).Checked = true;

                this.contextmenuSortBy.Items.AddRange(getSortByMenuItems());
                this.contextmenuSortBy.Items.Add(new ToolStripSeparator());
                this.contextmenuSortBy.Items.AddRange(getSortByDirectionMenuItems());
                ((ToolStripMenuItem)this.contextmenuSortBy.Items[Properties.Settings.Default.DEFAULT_SORTBY + ""]).CheckState = CheckState.Indeterminate;
                ((ToolStripMenuItem)this.contextmenuSortBy.Items[Properties.Settings.Default.DEFAULT_SORTBY + ""]).Checked = true;                

                currentSortBy = (SortByType)Properties.Settings.Default.DEFAULT_SORTBY;
                DragSource dragSource = new DragSource();
                this.listItems.DragSource = dragSource;
                dragSource.endDragEvent +=new EndDragDelegate(dragSource_endDragEvent);
                
                this.treeFolders.ShowNodeToolTips = true;
                this.treeFolders.ShowLines = true;
                this.btnMore.Enabled = false;
                this.btnSearch.BackgroundImageLayout = ImageLayout.Center;
                buildSearchOptionsContextMenu();
                buildFilterFieldsContextMenu();
                buildImageList();
                prepareLayout();
                positionSplitter();
                //將來可能需要加上開關QuickStart的checkbox by Gene
                if (this.chkQuickStartTip.Checked)
                {
                    Properties.Settings.Default.DEFAULT_SHOWQUICKSTART = true;
                }
                try
                {
                    disableClickSounds();
                }
                catch { }

                //先將quickstart拿掉
                //Properties.Settings.Default.DEFAULT_SHOWQUICKSTART = true;
                //if (Properties.Settings.Default.DEFAULT_SHOWQUICKSTART)
                //{
                //    QuickStart quickstart = new QuickStart(Properties.Settings.Default.URL_QUICKSTART);
                //    quickstart.Show();
                //}
                //btnTemplate.PerformClick();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::initializeUI", ex.ToString());
            }            
        }
        private void initializeLang()
        {
            this.lblCurrentUserName.Text = Globals.ThisAddIn.user.ForeName + " " + Globals.ThisAddIn.user.LastName;
            this.listItems.EmptyListMsg = string.Empty;
            this.inputSearch.Text = Lang.en_US.Master_InputSearch_Filter_Label;
            this.lblSortBy.Text = Lang.en_US.Master_SortBy_Label;
            this.menuitemSortBy.Text = Lang.en_US.Master_Sort_MenuItem;
            this.menuitemFilterFields.Text = Lang.en_US.Master_Filter_MenuItem;
            this.menuitemShowHidePreview.Text = this.splitChild.Panel2Collapsed ? Lang.en_US.Master_ShowPreview_MenuItem : Lang.en_US.Master_HidePreview_MenuItem;
            this.lblLoadingFolders.Text = Lang.en_US.Master_LoadingFolders_Label;
            this.lblLoadingItems.Text = Lang.en_US.Master_LoadingItems_Label;
            this.lblDownloading.Text = Lang.en_US.Master_DownloadingFile_Label;
            this.tooltip.SetToolTip(this.lblCurrentUserName, Lang.en_US.Master_UserName_Tooltip);
            this.tooltip.SetToolTip(this.btnLogout, Lang.en_US.Master_Logout_Link);
            this.tooltip.SetToolTip(this.btnReload, Lang.en_US.Master_Reload_Link);
            this.tooltip.SetToolTip(this.btnCite, Lang.en_US.Master_BtnCite_Tooltip);
            this.tooltip.SetToolTip(this.btnStyle, Lang.en_US.Master_BtnStyle_Tooltip);
            this.tooltip.SetToolTip(this.btnRefresh, Lang.en_US.Master_BtnRefresh_Tooltip);
            this.tooltip.SetToolTip(this.btnHighlight, Lang.en_US.Master_BtnHightlight_Tooltip.Replace("\\n", Environment.NewLine));
            this.tooltip.SetToolTip(this.btnRefList, Lang.en_US.Master_BtnRefList_Tooltip);
            this.tooltip.SetToolTip(this.btnSortBy, Lang.en_US.Master_BtnSortBy_Tooltip);
            this.tooltip.SetToolTip(this.btnSearch, Lang.en_US.Master_BtnSearch_Tooltip);
            this.tooltip.SetToolTip(this.btnMore, Lang.en_US.Master_BtnMore_Tooltip);
            this.tooltip.SetToolTip(this.btnTemplate, Lang.en_US.Master_BtnTemplate_Tooltip);
        }
        private void initializeHandlers()
        {
            this.Resize += new System.EventHandler(this.MasterControl_Resized);
            this.KeyPress += new KeyPressEventHandler(this.MasterControl_KeyPress);

            this.listItems.SelectedIndexChanged += new EventHandler(this.listItems_SelectedIndexChanged);
            this.listItems.GotFocus += new System.EventHandler(this.listItems_GotFocus);
            this.listItems.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.listItems_CellToolTipShowing);
            this.listItems.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.listItems_FormatCell);
            this.listItems.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.listItems_FormatRow);
            this.listItems.LostFocus += new System.EventHandler(this.listItems_LostFocus);
            this.listItems.KeyDown += new KeyEventHandler(this.listItems_KeyDown);
            this.listItems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listItems_KeyPress);
            this.listItems.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.listItems_CellClick);
            this.listItems.DoubleClick += new EventHandler(this.listItems_DoubleClick);
            this.listItems.Scroll += new EventHandler<ScrollEventArgs>(this.listItems_Scroll);
            this.splitParent.SplitterMoved += new SplitterEventHandler(splitParent_SplitterMoved);
            this.splitChild.SplitterMoved += new SplitterEventHandler(splitChild_SplitterMoved);

            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
            this.btnStyle.Click += new System.EventHandler(this.btnStyle_Click);
            this.btnCite.Click += new EventHandler(this.btnCite_Click);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.btnRefList.Click += new EventHandler(this.btnRefList_Click);
            this.btnHighlight.Click += new EventHandler(this.btnHighlight_Click);
            this.btnShowHidePreview.Click += new EventHandler(this.btnShowHidePreview_Click);
            
            this.lblCurrentUserName.Click += new EventHandler(this.lblCurrentUserName_Clicked);
            this.inputSearch.TextChanged += new System.EventHandler(this.inputSearch_TextChanged);
            this.inputSearch.GotFocus += new System.EventHandler(this.inputSearch_GotFocus);
            this.inputSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputSearch_KeyDown);
            this.inputSearch.Leave += new System.EventHandler(this.inputSearch_Leave);
            this.inputSearch.LostFocus += new System.EventHandler(this.inputSearch_LostFocus);
            this.inputSearch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.inputSearch_MouseUp);
            this.menuitemShowHidePreview.Click += new EventHandler(this.menuitemShowHidePreview_Click);
            this.treeFolders.KeyPress += new KeyPressEventHandler(this.treeFolders_KeyPress);            
            Globals.ThisAddIn.DockPositionChanged +=new EventHandler(ThisAddIn_DockPositionChanged);
            
            this.olvColumnDisplay.AspectToStringConverter = delegate(object x){return "";};
            this.olvColumnImage.AspectToStringConverter = delegate(object x){return "";};
            this.olvColumnItemFile.AspectToStringConverter = delegate(object x){return "";};

            this.masterControlThread = new MasterControlThread(this);
            this.delegateLoadingFolderFinished = new DelegateLoadingFolderFinished(this.loadingFolderFinished);
            this.delegateLoadingColleaguesFinished = new DelegateLoadingColleagues(this.loadingColleaguesFinished);
            this.delegateLoadingItemsFinished = new DelegateLoadingItemsFinished(this.loadingItemsFinished);
            this.delegateFileDownloaded = new DelegateFileDownloaded(this.downloadingFileFinished);
            this.delegateRefreshFinished = new DelegateRefreshFinished(this.refreshingFinished);
            this.delegateInsertFinished = new DelegateInsertFinished(this.insertingFinished);
            this.delegateFocusFinished = new DelegateFocusFinished(this.focusingFinished);
            this.delegateLoadingItemsProgress = new DelegateLoadingItemsProgress(this.loadingItemsProgress);
            this.delegateDownloadingProgressChanged = new DelegateFileDownloadProgressChanged(this.downloadingFileProgressChanged);
            this.delegateSearchingItemsFinished = new DelegateSearchingItemsFinished(this.searchingItemFinished);
        }

        #endregion

        #region Public Functions

        public void InsertCitations()
        {
            btnCite_Click(null, null);
        }
        public bool GetItem(ref ItemMasterRow item, int itemID)
        {
            foreach (ItemMasterRow curritem in itemList)
            {
                if (curritem.ItemID == itemID)
                {
                    item = curritem;
                    return true;
                }
            }
            return false;
        }        
        public void FocusOnListItems()
        {
            this.Focus();
            this.listItems.Focus();
        }
        public void FocusReferenceList()
        {
            if (btnRefList.Enabled)
            {
                btnRefList_Click(null, null);
                FocusOnListItems();
            }
        }
        public void FocusFindReference()
        {
            inputSearch.Focus();
        }
        public void FocusChangeStyle()
        {
            btnStyle_Click(null, null);
        }
        public void DownloadFile(string strFileName)
        {
            if (bDownloadingFile)
                MessageBox.Show(Lang.en_US.Master_DownloadingBusy_Label, Lang.en_US.Master_DownloadingBusy_Title, MessageBoxButtons.OK);
            else
            {
                bDownloadingFile = true;
                Globals.ThisAddIn.user.DownloadFile(strFileName, this, null);
                this.lblDownloading.Visible = true;
            }
        }

        #endregion

        #region Private Functions

        private void disableClickSounds()
        {
            CoInternetSetFeatureEnabled(FEATURE_DISABLE_NAVIGATION_SOUNDS, SET_FEATURE_ON_PROCESS, true);
        }
        private void documentFocus()
        {
            Globals.ThisAddIn.Application.ActiveDocument.ActiveWindow.SetFocus();
        }
        private void prepareSplitter()
        {            
            if (splitParent.Orientation == Orientation.Horizontal && splitChild.Orientation == Orientation.Horizontal)
            {
                this.splitChild.Panel2Collapsed = false;
                this.splitParent.SplitterDistance = Convert.ToInt32(Properties.Settings.Default.MASTER_VERTICAL_SPLITTERPARENT_RATIO * this.Height);
                this.splitChild.SplitterDistance = Convert.ToInt32(Properties.Settings.Default.MASTER_VERTICAL_SPLITTERCHILD_RATIO * this.Height);
                try
                {
                    this.splitParent.Panel1MinSize = Properties.Settings.Default.MASTER_SPLITPARENT_PANEL1_H_MINHEIGHT;
                    this.splitParent.Panel2MinSize = Properties.Settings.Default.MASTER_SPLITPARENT_PANEL2_H_MINHEIGHT;
                    this.splitChild.Panel1MinSize = Properties.Settings.Default.MASTER_SPLITCHILD_PANEL1_H_MINHEIGHT;
                    this.splitChild.Panel2MinSize = Properties.Settings.Default.MASTER_SPLITCHILD_PANEL2_H_MINHEIGHT;                    
                    this.splitParent.SplitterWidth = 3;
                    this.menuitemShowHidePreview.Enabled = true;
                    this.splitChild.Panel2Collapsed = Properties.Settings.Default.MASTER_PREVIEW_COLLAPSED;
                }
                catch {
                    this.splitParent.Panel1MinSize = 0;
                    this.splitParent.Panel2MinSize = 0;
                    this.splitChild.Panel1MinSize = 0;
                    this.splitChild.Panel2MinSize = 0;
                }
            }
            else if (splitParent.Orientation == Orientation.Vertical && splitChild.Orientation == Orientation.Vertical)
            {
                this.splitParent.SplitterDistance = Convert.ToInt32(Properties.Settings.Default.MASTER_HORIZONTAL_SPLITTERPARENT_RATIO * this.Width);
                this.splitChild.SplitterDistance = Convert.ToInt32(Properties.Settings.Default.MASTER_HORIZONTAL_SPLITTERCHILD_RATIO * this.Width);
                try
                {
                    this.splitParent.Panel1MinSize = Properties.Settings.Default.MASTER_SPLITPARENT_PANEL1_V_MINWIDTH;
                    this.splitParent.Panel2MinSize = Properties.Settings.Default.MASTER_SPLITPARENT_PANEL2_V_MINWIDTH;
                    this.splitChild.Panel1MinSize = Properties.Settings.Default.MASTER_SPLITCHILD_PANEL1_V_MINWIDTH;
                    this.splitChild.Panel2MinSize = Properties.Settings.Default.MASTER_SPLITCHILD_PANEL2_V_MINWIDTH;
                    this.splitParent.SplitterWidth = 12;
                    this.menuitemShowHidePreview.Enabled = false;
                }
                catch {
                    this.splitParent.Panel1MinSize = 0;
                    this.splitParent.Panel2MinSize = 0;
                    this.splitChild.Panel1MinSize = 0;
                    this.splitChild.Panel2MinSize = 0;
                }
            }
        }
        private void dockRepositionFinished()
        {
            try
            {
                bMoving = false;
                prepareSplitter();
                this.splitParent.SplitterMoved += new SplitterEventHandler(splitParent_SplitterMoved);
                this.splitChild.SplitterMoved += new SplitterEventHandler(splitChild_SplitterMoved);
                inputSearch.Width = inputSearch.Parent.Width;
                Globals.ThisAddIn.ResetPaneWidth(this.Width);
                Globals.ThisAddIn.iTempWidth = Globals.ThisAddIn.GetCurrentCustomPaneWidth();
                Globals.ThisAddIn.iTempHeight = Globals.ThisAddIn.GetCurrentCustomPaneHeight();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::dockRepositionFinished", ex.ToString());
            }
        }
        private void positionSplitter()
        {
            try
            {
                prepareSplitter();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::positionSplitter", ex.ToString());
            }
            inputSearch.Width = inputSearch.Parent.Width;
        }
        private bool isInsideDocument()
        {
            bool b = false;
            if (this.listItems.Focused || this.treeFolders.Focused || this.ctrlPreview.Focused || this.Focused)
                return b;
            Point p = Cursor.Position;
            int left = Globals.ThisAddIn.Application.ActiveWindow.Left;
            int top = Globals.ThisAddIn.Application.ActiveWindow.Top + 198;
            int width = Globals.ThisAddIn.Application.ActiveWindow.Width;
            int height = Globals.ThisAddIn.Application.ActiveWindow.Height;
            if (p.X > left && p.X < (left + width))
                if (p.Y > top && p.Y < (top + height))
                    b = true;
            return b;
        }
        private void clearSearchFilter()
        {
            this.inputSearch.TextChanged -= new System.EventHandler(this.inputSearch_TextChanged);
            //inputSearch.Text = Lang.en_US.Master_InputSearch_Text;
            inputSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            this.inputSearch.TextChanged += new System.EventHandler(this.inputSearch_TextChanged);
        }
        private void enableEmptyMsg(bool b)
        {
            listItems.EmptyListMsg = b ? Lang.en_US.Master_NoItems_Label : string.Empty;
        }
        private void adjustLoadingLabels()
        {
            if (this.Controls.Contains(this.flowLoadingFolders))
                this.flowLoadingFolders.Location = new Point(this.treeFolders.Location.X + 3, this.treeFolders.Location.Y + 3);
            if (this.Controls.Contains(this.flowLoadingItems))
            {
                if (splitParent.Orientation == Orientation.Horizontal)
                    this.flowLoadingItems.Location = new Point(this.listItems.Location.X + 3, this.splitParent.Panel2.Top + this.listItems.Location.Y + 3);
                else
                    this.flowLoadingItems.Location = new Point(this.splitParent.Panel2.Left + this.listItems.Location.X + 3, this.listItems.Location.Y + 3);
            }
        }

        private void prepareLayout()
        {
            try
            {
                Microsoft.Office.Core.MsoCTPDockPosition DockPosition = Globals.ThisAddIn.GetCurrentDockPosition();
                this.splitParent.SplitterMoved -= new SplitterEventHandler(splitParent_SplitterMoved);
                this.splitChild.SplitterMoved -= new SplitterEventHandler(splitChild_SplitterMoved);
                switch (DockPosition)
                {
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionBottom:
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionTop:
                        prepareHorizontalLayout();
                        break;
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionLeft:
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight:
                        prepareVerticalLayout();
                        break;
                    default:
                        prepareVerticalLayout();
                        break;
                }
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::prepareLayout", ex.ToString());
            }
        }
        private void prepareHorizontalLayout()
        {
            splitParent.Panel1MinSize = 0;
            splitParent.Panel2MinSize = 0;
            splitChild.Panel1MinSize = 0;
            splitChild.Panel2MinSize = 0;
            splitChild.Orientation = Orientation.Vertical;
            splitParent.Orientation = Orientation.Vertical;
            splitChild.Panel2Collapsed = Properties.Settings.Default.MASTER_PREVIEW_COLLAPSED;
        }
        private void prepareVerticalLayout()
        {
            this.MinimumSize = new System.Drawing.Size(330, 0);
            splitParent.Panel1MinSize = 0;
            splitParent.Panel2MinSize = 0;
            splitChild.Panel1MinSize = 0;
            splitChild.Panel2MinSize = 0;
            splitChild.Orientation = Orientation.Horizontal;
            splitParent.Orientation = Orientation.Horizontal;
            splitChild.Panel2Collapsed = Properties.Settings.Default.MASTER_PREVIEW_COLLAPSED;
        }        
        private void prepareLoadingFoldersLabel()
        {
            // 一開始還沒有撈回資料的時候，會先有一個 loading 的字樣
            this.Controls.Add(this.flowLoadingFolders);
            Graphics g = lblLoadingFolders.CreateGraphics();
            SizeF s = g.MeasureString(lblLoadingFolders.Text, lblLoadingFolders.Font);
            this.flowLoadingFolders.Location = new Point(this.treeFolders.Location.X + 3, this.treeFolders.Location.Y + 3);
            this.flowLoadingFolders.Size = new System.Drawing.Size(Convert.ToInt32(s.Width + 40), 24);
            this.flowLoadingFolders.BringToFront();
        }
        private void prepareLoadingItemsLabel()
        {
            if (!this.Controls.Contains(this.flowLoadingItems))
            {
                this.Controls.Add(this.flowLoadingItems);
                if(splitParent.Orientation == Orientation.Horizontal)
                    this.flowLoadingItems.Location = new Point(this.listItems.Location.X + 3, this.splitParent.Panel2.Top + this.listItems.Location.Y + 3);
                else
                    this.flowLoadingItems.Location = new Point(this.splitParent.Panel2.Left + this.listItems.Location.X + 3, this.listItems.Location.Y + 3);
                Graphics g = lblLoadingItems.CreateGraphics();
                SizeF s = g.MeasureString(lblLoadingItems.Text, lblLoadingFolders.Font);
                this.flowLoadingItems.Size = new System.Drawing.Size(Convert.ToInt32(s.Width + 60), 23);
                this.flowLoadingItems.BringToFront();
            }
        }        
        
        private void loadFolders()
        {
            try
            {                
                treeFolders.Nodes.Clear();
                treeFolders.Enabled = false;
                btnCite.Enabled = false;
                btnStyle.Enabled = false;
                btnRefList.Enabled = false;
                btnRefresh.Enabled = false;                
                listItems.Items.Clear();
                enableEmptyMsg(false);
                prepareLoadingFoldersLabel(); // 尚未從 server 端撈回資料的時候，在 treeFolders 元件會顯示一個 loading 字樣
                prepareLoadingItemsLabel();   // 尚未從 server 端撈回資料的時候，會 listItems   元件會顯示一個 loading 字樣
                inputSearch.Enabled = false;
                ThreadStart pThreadStart = new ThreadStart(masterControlThread.LoadFolders);
                masterThread = new Thread(pThreadStart);
                masterThread.Name = "loadingfolders";
                masterThread.Start();
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::loadFolders", ex.ToString());
            }
        }        
        private void loadItems(bool bFiltering, string filter, bool bFromColleague, Colleague colleague, string strFolderID)
        {
            try
            {
                if (selectedFolder != null || bFromColleague)
                {
                    if (bFiltering)
                    {
                        if (masterThread.IsAlive  && masterThread.Name == "loadingitems")
                            masterThread.Abort();
                       
                        ParameterizedThreadStart threadStart = new ParameterizedThreadStart(masterControlThread.FilterItems);
                        masterThread = new Thread(threadStart);
                        masterThread.Name = "loadingitems";
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data["ItemList"] = itemList;
                        data["Filter"] = filter;
                        masterThread.Start(data);
                    }
                    else
                    {
                        if (masterThread.IsAlive && masterThread.Name == "loadingitems")
                            masterThread.Abort();
                        inputSearch.Enabled = false;
                        prepareLoadingItemsLabel();
                        if (bFromColleague)
                        {
                            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(masterControlThread.LoadColleagueItems);
                            masterThread = new Thread(threadStart);
                            masterThread.Name = "loadingitems";
                            Dictionary<string, object> data = new Dictionary<string, object>();
                            data["Colleague"] = colleague;
                            data["FolderID"] = strFolderID;
                            masterThread.Start(data);
                        }                            
                        else
                        {
                            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(masterControlThread.LoadItems);
                            masterThread = new Thread(threadStart);
                            masterThread.Name = "loadingitems";
                            masterThread.Start(selectedFolder.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::loadItems", ex.ToString());
            }
        }
        private void loadingFolderFinished(IEnumerable<Folder> folders)
        {
            try
            {
                if(masterThread.IsAlive && masterThread.Name=="loadingfolders")
                    masterThread.Abort();
                this.Controls.Remove(this.flowLoadingFolders);
                listFolders = folders;

                // 把該使用者所擁有的資料夾用遞迴的方式來建立 node
                buildFolderTreeRecursive(0, treeFolders.Nodes);

                TreeNode nodeColleagueList = new TreeNode();
                nodeColleagueList.Name = "mycolleagues";
                nodeColleagueList.Text = Lang.en_US.Master_ColleaguesList_Label;
                nodeColleagueList.ImageKey = nodeColleagueList.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_COLLEAGUELIST;
                nodeColleagueList.ToolTipText = Lang.en_US.Master_ColleagueList_Tooltip;
                treeFolders.Nodes.Add(nodeColleagueList);

                nodeCitedRef = new TreeNode();
                nodeCitedRef.Name = "cited";
                nodeCitedRef.Text = Lang.en_US.Master_CitedReferences_Label;
                nodeCitedRef.ToolTipText = Lang.en_US.Master_CitedReferences_Tooltip;
                nodeCitedRef.ImageKey = nodeCitedRef.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_CITEDREFERENCES;
                treeFolders.Nodes.Add(nodeCitedRef);

                // 搜尋的 node 有 google, PubMed...
                //buildSearchOptionsNode();                

                treeFolders.Enabled = true;
                treeFolders.Select();
                treeFolders.AfterSelect += new TreeViewEventHandler(treeFolders_ChangeSelectedFolder);
                treeFolders.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeFolders_NodeMouseDoubleClick);
                loadItems(false, string.Empty, false, null, "");
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::loadingFolderFinished", ex.ToString());
            }
        }
        private void loadingColleaguesFinished(IEnumerable<Colleague> colleagues, TreeNode currNode)
        {
            try
            {
                if(masterThread.IsAlive && masterThread.Name=="loadingcolleagues")
                    masterThread.Abort();
                currNode.Text = Lang.en_US.Master_ColleaguesList_Label + " ( " + (colleagues.Count() + " colleague" + ((colleagues.Count() > 1) ? "s" : "")) + " ) ";
                buildColleagueList(currNode.Nodes, colleagues);
                currNode.Expand();
                bBuildingColleaguesList = false;
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::loadingColleaguesFinished", ex.ToString());
            }
        }
        private void loadingItemsFinished(List<ItemMasterRow> items, bool bfiltered)
        {
            try
            {
                if (masterThread.IsAlive && masterThread.Name == "loadingitems")
                    masterThread.Abort();

                inputSearch.Enabled = true;
                enableEmptyMsg(true);
                IEnumerable<ItemMasterRow> enumItems = null;
                if (this.bIsAscending)
                {
                    switch (currentSortBy)
                    {
                        case SortByType.Title:
                            enumItems = items.OrderBy(ItemMasterRow => ItemMasterRow.Title);
                            break;
                        case SortByType.Author:
                            enumItems = items.OrderBy(ItemMasterRow => Tools.CitationTools.GetAuthorsSortableString(ItemMasterRow.Authors));
                            break;
                        case SortByType.Year:
                            enumItems = items.OrderBy(ItemMasterRow => ItemMasterRow.PubYear);
                            break;
                        case SortByType.JournalName:
                            enumItems = items.OrderBy(ItemMasterRow => ItemMasterRow.Title2);
                            break;
                        case SortByType.ItemType:
                            enumItems = items.OrderBy(ItemMasterRow => ItemMasterRow.ItemTypeID);
                            break;
                        default:
                            enumItems = items.OrderBy(ItemMasterRow => ItemMasterRow.SequenceNo);
                            break;
                    }
                }
                else
                {
                    switch (currentSortBy)
                    {
                        case SortByType.Title:
                            enumItems = items.OrderByDescending(ItemMasterRow => ItemMasterRow.Title);
                            break;
                        case SortByType.Author:
                            enumItems = items.OrderByDescending(ItemMasterRow => Tools.CitationTools.GetAuthorsSortableString(ItemMasterRow.Authors));
                            break;
                        case SortByType.Year:
                            enumItems = items.OrderByDescending(ItemMasterRow => ItemMasterRow.PubYear);
                            break;
                        case SortByType.JournalName:
                            enumItems = items.OrderByDescending(ItemMasterRow => ItemMasterRow.Title2);
                            break;
                        case SortByType.ItemType:
                            enumItems = items.OrderByDescending(ItemMasterRow => ItemMasterRow.ItemTypeID);
                            break;
                        default:
                            enumItems = items.OrderByDescending(ItemMasterRow => ItemMasterRow.SequenceNo);
                            break;
                    }
                }
                
                if (bfiltered)
                {
                    //btnSearch.Image = Properties.Resources.search;
                }
                else
                {
                    this.Controls.Remove(this.flowLoadingItems);
                    itemList = enumItems;
                }
                this.listItems.SetObjects(enumItems);
                if (this.listItems.Items.Count > 0)
                {
                    this.listItems.Items[0].Selected = true;                    
                }
                listItems.Enabled = true;
                btnCite.Enabled = true;
                btnStyle.Enabled = true;
                btnRefList.Enabled = true;
                btnRefresh.Enabled = true;
                this.lblLoadingItems.Text = Lang.en_US.Master_LoadingItems_Label;
                listItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::loadingItemsFinished", ex.ToString());
            }            
        }
        private void downloadingFileFinished()
        {
            bDownloadingFile = false;
            this.lblDownloading.Text = Lang.en_US.Master_DownloadingFile_Label;
            this.lblDownloading.Visible = false;
        }
        private void refreshingFinished()
        {
            try
            {
                progress.Close();
                if (masterThread.IsAlive && masterThread.Name=="refreshingcitations")
                    masterThread.Abort();
                if (bUsingCitedList)
                {
                    enableEmptyMsg(true);
                    inputSearch.Enabled = true;
                    if (listItems.Items.Count > 0)
                    {
                        btnHighlight.Enabled = true;
                        btnHighlight.Visible = true;
                    }
                    if (formatter.listCurrentCitedReferences != null)
                    {
                        List<ItemMasterRow> items = new List<ItemMasterRow>();
                        
                        for (int i = 0; i < formatter.listCurrentCitedReferences.Count; i++)
                            items.Add((formatter.listCurrentCitedReferences.ElementAt(i)).Value);

                        itemList = items.OrderBy(ItemMasterRow => ItemMasterRow.SequenceNo);
                        resetCurrentSortByMenu(SortByType.None);
                        this.listItems.SetObjects(itemList);
                        
                        if (this.listItems.Items.Count > 0)
                            this.listItems.Items[0].Selected = true;
                        
                        listItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
                        
                        if (listItems.Items.Count == 0)
                            ctrlPreview.ClearPreview();
                        
                        listItems.Focus();
                    }
                    else
                    {
                        listItems.Items.Clear();
                        ctrlPreview.ClearPreview();
                    }
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::refreshingFinished", ex.ToString());
            }
        }
        private void insertingFinished()
        {
            try
            {
                progress.Close();
                if (masterThread.IsAlive && masterThread.Name == "insertingcitations")
                    masterThread.Abort();                
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::insertingFinished", ex.ToString());
            }
        }
        private void focusingFinished()
        {
            if (masterThread.IsAlive)
                masterThread.Abort();
        }
        private void loadingItemsProgress(int current, int total)
        {
            int percentage = Convert.ToInt32(((float) current / total) * 100);
            this.lblLoadingItems.Text = string.Format(Lang.en_US.Master_LoadingItemsProgress_Label, percentage + " % ");
        }
        private void downloadingFileProgressChanged(int percentage)
        {
            this.lblDownloading.Text = Lang.en_US.Master_DownloadingFile_Label + " ( " + percentage + "% ) ";
        }
        private void searchingItemFinished(List<ItemMasterRow> items)
        {
            try
            {
                if (masterThread.IsAlive && masterThread.Name == "searchingitems")
                    masterThread.Abort();

                enableEmptyMsg(true);
                if(listItems.Objects != null)
                    items.AddRange((IEnumerable<ItemMasterRow>) listItems.Objects);
                IEnumerable<ItemMasterRow> enumItems = null;
                enumItems = items;
                this.Controls.Remove(this.flowLoadingItems);
                this.listItems.SetObjects(enumItems);
                if (this.listItems.Items.Count > 0)
                {
                    this.listItems.Items[0].Selected = true;
                }
                enableSearchControls(true);
                enableCitationControls(true);
                this.lblLoadingItems.Text = Lang.en_US.Master_LoadingItems_Label;
                listItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::loadingItemsFinished", ex.ToString());
            }   
        }        
        private void resetCurrentSortByMenu(SortByType type)
        {
            this.bIsAscending = true;
            foreach (ToolStripMenuItem item in menuitemSortBy.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if (item.Checked)
                    item.Checked = false;

                if (item.Tag + "" == "sortdescending")
                    item.Checked = false;
                else if (item.Tag + "" == "sortascending")
                {
                    item.CheckState = CheckState.Indeterminate;
                    item.Checked = true;
                }
                else if ((SortByType)item.Tag == type)
                {
                    item.Checked = true;
                    item.CheckState = CheckState.Indeterminate;
                }
            }
            foreach (ToolStripMenuItem item in contextmenuSortBy.Items.OfType<ToolStripMenuItem>())
            {
                if (item.Checked)
                    item.Checked = false;

                if (item.Tag + "" == "sortdescending")
                    item.Checked = false;
                else if (item.Tag + "" == "sortascending")
                {
                    item.CheckState = CheckState.Indeterminate;
                    item.Checked = true;
                }
                else if ((SortByType)item.Tag == type)
                {
                    item.Checked = true;
                    item.CheckState = CheckState.Indeterminate;
                }
            }
        }        
        private void buildImageList()
        {
            treeFolders.ImageList = new ImageList();
            treeFolders.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_NORMALFOLDER, Properties.Resources.DefaultFolder);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_PUBLICATIONFOLDER, Properties.Resources.MyPublicationsFolder);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_ALLITEMS, Properties.Resources.WizFolder);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_RECYCLEBIN, Properties.Resources.FolderTrash);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SHAREDCOLLECTION, Properties.Resources.CollectionsParent);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_WIZADD, Properties.Resources.Import24x24);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_COLLEAGUE, Properties.Resources.usericon);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_COLLEAGUELIST, Properties.Resources.publicicon);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_CITEDREFERENCES, Properties.Resources.CitedList);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_LOADING, Properties.Resources.loading);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_PUBMEDNOTICAT, Properties.Resources.PubMedNoti);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_PUBMEDNOTI, Properties.Resources.PubMedNotiChild);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_NOTITIME, Properties.Resources.PubMedNotiDay);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_PUBMEDSEARCH, Properties.Resources.PubMed);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SCHOLARSPORTALSEARCH, Properties.Resources.ScholarsPortal);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_PUBMED, Properties.Resources.PubMed);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_SCHOLARSPORTAL, Properties.Resources.ScholarsPortal);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_CITEULIKE, Properties.Resources.CiteUlike);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WORLDCAT, Properties.Resources.WorldCat);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_GOOGLESCHOLAR, Properties.Resources.GoogleScholar);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WIZSEARCH, Properties.Resources.WizFolioSearch);
            treeFolders.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_SEARCHDB, Properties.Resources.xmag);

            searchOptionsContextMenu.ImageList = new ImageList();
            searchOptionsContextMenu.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            searchOptionsContextMenu.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_FILTER, Properties.Resources.Filter);
            searchOptionsContextMenu.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_PUBMED, Properties.Resources.PubMed);
            searchOptionsContextMenu.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_SCHOLARSPORTAL, Properties.Resources.ScholarsPortal);
            searchOptionsContextMenu.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_CITEULIKE, Properties.Resources.CiteUlike);
            searchOptionsContextMenu.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WORLDCAT, Properties.Resources.WorldCat);
            searchOptionsContextMenu.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_GOOGLESCHOLAR, Properties.Resources.GoogleScholar);
            searchOptionsContextMenu.ImageList.Images.Add(Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WIZSEARCH, Properties.Resources.WizFolioSearch);

            listIcons.ColorDepth = ColorDepth.Depth32Bit;
            listIcons.Images.Add(ItemTypes.BookChapter.ToString(), Properties.Resources.ChapterType);
            listIcons.Images.Add(ItemTypes.BookWhole.ToString(), Properties.Resources.booktype);
            listIcons.Images.Add(ItemTypes.Proceeding.ToString(), Properties.Resources.ProceedingType);
            listIcons.Images.Add(ItemTypes.JournalArticle.ToString(), Properties.Resources.JournalType);
            listIcons.Images.Add(ItemTypes.Document.ToString(), Properties.Resources.DocumentType);
            listIcons.Images.Add(ItemTypes.WebPage.ToString(), Properties.Resources.WebPageType);
            listIcons.Images.Add(ItemTypes.Thesis.ToString(), Properties.Resources.ThesisType);
            listIcons.Images.Add(ItemTypes.Patent.ToString(), Properties.Resources.PatentType);
        }
        private void buildColleagueList(TreeNodeCollection nodeCollection, IEnumerable<Colleague> colleagues)
        {
            foreach (Colleague colleague in colleagues)
            {
                TreeNode nodeColleague = new TreeNode();
                nodeColleague.Name = "colleague";
                nodeColleague.Text = colleague.ForeName + " " + colleague.LastName;
                nodeColleague.ImageKey = nodeColleague.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_COLLEAGUE;
                nodeColleague.ToolTipText = Lang.en_US.Master_NodeColleague_Tooltip;
                nodeColleague.Tag = colleague;
                nodeCollection.Add(nodeColleague);
            }
        }
        private void buildSearchOptionsNode()
        {
            try
            {
                nodeSearchDBs = new TreeNode();
                nodeSearchDBs.Name = "searchdbs";
                nodeSearchDBs.Text = Lang.en_US.Master_SearchOptions_SearchDatabases;
                nodeSearchDBs.ImageKey = nodeSearchDBs.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_SEARCHDB;
                this.treeFolders.Nodes.Add(nodeSearchDBs);
                foreach (SearchOptions option in Enum.GetValues(typeof(SearchOptions)))
                {
                    TreeNode node = null;
                    switch (option)
                    {
                        case SearchOptions.Filter:                            
                            break;
                        case SearchOptions.PubMed:
                            node = new TreeNode();
                            node.Text = Lang.en_US.Master_SearchOptions_PubMed;                            
                            node.ImageKey = node.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_PUBMED;
                            break;
                        case SearchOptions.ScholarsPortal:
                            node = new TreeNode();
                            node.Text = Lang.en_US.Master_SearchOptions_ScholarsPortal;
                            node.ImageKey = node.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_SCHOLARSPORTAL;
                            break;
                        case SearchOptions.CiteULike:
                            node = new TreeNode();
                            node.Text = Lang.en_US.Master_SearchOptions_CiteULike;
                            node.ImageKey = node.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_CITEULIKE;
                            break;
                        case SearchOptions.WorldCat:
                            node = new TreeNode();
                            node.Text = Lang.en_US.Master_SearchOptions_WorldCat;
                            node.ImageKey = node.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WORLDCAT;
                            break;
                        case SearchOptions.GoogleScholar:
                            node = new TreeNode();
                            node.Text = Lang.en_US.Master_SearchOptions_GoogleScholar;
                            node.ImageKey = node.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_GOOGLESCHOLAR;
                            break;
                        case SearchOptions.WizFolioSearch:
                            node = new TreeNode();
                            node.Text = Lang.en_US.Master_SearchOptions_WizSearch;
                            node.ImageKey = node.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WIZSEARCH;
                            break;
                    }
                    if(node!=null)
                    {
                        node.Tag = (int)option;
                        node.Name = option.ToString();
                        nodeSearchDBs.Nodes.Add(node);
                    }                    
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::buildSearchOptionsNode", ex.ToString());
            }
        }
        private void buildFolderTreeRecursive(int iParentID, TreeNodeCollection nodeCollection)
        {
            // listFolders 在呼叫前會被指定好值

            foreach (Folder folder in listFolders)
            {
                if (folder.ParentID == iParentID)
                {
                    TreeNode nodeFolder = new TreeNode();
                    nodeFolder.Name = folder.ID + "";
                    nodeFolder.Text = folder.Name;
                    nodeFolder.ImageKey = nodeFolder.SelectedImageKey = getMappedFolderImage(folder.Type);
                    if(folder.Type == FolderType.PubmedNotification)
                    {
                        if(folder.ParentID == 0)
                            nodeFolder.ImageKey = nodeFolder.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_PUBMEDNOTICAT;
                        else if (Regex.Match(folder.Name, @"\d{4}\/\d{2}").Success)
                            nodeFolder.ImageKey = nodeFolder.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_NOTITIME;
                        else
                            nodeFolder.ImageKey = nodeFolder.SelectedImageKey = Properties.Settings.Default.IMAGEKEY_PUBMEDNOTI;
                    }
                    nodeCollection.Add(nodeFolder);
                    if (!bFolderSelected && folder.Type == FolderType.ALL)
                    {
                        treeFolders.SelectedNode = nodeFolder;
                        selectedFolder = folder;
                    }
                    if (folder.LastFocused)
                    {
                        treeFolders.SelectedNode = nodeFolder;
                        bFolderSelected = true;
                        selectedFolder = folder;
                    }
                    buildFolderTreeRecursive(folder.ID, nodeFolder.Nodes);
                }
            }
        }
        private void buildColleagueTreeRecursive(int iParentID, Colleague colleague, TreeNodeCollection nodeCollection)
        {
            IEnumerable<Folder> listColleagueFolders = Globals.ThisAddIn.user.LoadColleagueFolders(colleague.ServerAddress, colleague.UserID, iParentID);
            foreach (Folder folder in listColleagueFolders)
            {
                TreeNode nodeFolder = new TreeNode();
                nodeFolder.Name = "colleaguefolder_" + folder.ID;
                nodeFolder.Text = folder.Name;
                nodeFolder.Tag = colleague;
                nodeFolder.ImageKey = nodeFolder.SelectedImageKey = getMappedFolderImage(folder.Type);
                nodeCollection.Add(nodeFolder);
                buildColleagueTreeRecursive(folder.ID, colleague, nodeFolder.Nodes);
            }
        }
        private void buildFilterFieldsContextMenu()
        {
            try
            {
                int iFilterFields = Properties.Settings.Default.DEFAULT_FILTER_FIELDS;
                foreach (FilterFields field in Enum.GetValues(typeof(FilterFields)))
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    ToolStripMenuItem cloneditem = new ToolStripMenuItem();
                    switch (field)
                    {
                        case FilterFields.Title:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_Title_Label;
                            break;
                        case FilterFields.Title2:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_Title2_Label;
                            break;
                        case FilterFields.Authors:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_Authors_Label;
                            break;
                        case FilterFields.PubDate:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_PubDate_Label;
                            break;
                        case FilterFields.Abstract:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_Abstract_Label;
                            break;
                        case FilterFields.Notes:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_Notes_Label;
                            break;
                        case FilterFields.Tags:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_Tags_Label;
                            break;
                        case FilterFields.Keywords:
                            item.Text = cloneditem.Text = Lang.en_US.Master_FilterField_Keywords_Label;
                            break;
                    }
                    item.Tag = cloneditem.Tag = (int)field;
                    item.Name = cloneditem.Name = (int)field + "";
                    item.Checked = cloneditem.Checked = ((iFilterFields & (int)field) == (int)field);
                    item.Font = new System.Drawing.Font(item.Font, FontStyle.Regular);
                    item.Click += new EventHandler(this.itemFilterField_Click);
                    cloneditem.Click += new EventHandler(this.itemFilterField_Click);
                    menuitemFilterFields.DropDown.Items.Add(cloneditem);
                    menuitemFilterFields.DropDown.Font = new System.Drawing.Font(menuitemFilterFields.DropDown.Font, FontStyle.Regular);
                }
                menuitemFilterFields.DropDown.Closing += new ToolStripDropDownClosingEventHandler(this.menuitemFilterFieldsDropDown_Closing);                
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::buildFilterFieldsContextMenu", ex.ToString());
            }
        }
        private void buildSearchOptionsContextMenu()
        {
            try
            {
                foreach (SearchOptions option in Enum.GetValues(typeof(SearchOptions)))
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    switch (option)
                    {
                        case SearchOptions.Filter:
                            item.Text = Lang.en_US.Master_SearchOptions_Filter;
                            item.ImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_FILTER;
                            break;
                        case SearchOptions.PubMed:
                            item.Text = Lang.en_US.Master_SearchOptions_PubMed;
                            item.ImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_PUBMED;
                            break;
                        case SearchOptions.ScholarsPortal:
                            item.Text = Lang.en_US.Master_SearchOptions_ScholarsPortal;
                            item.ImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_SCHOLARSPORTAL;
                            break;
                        case SearchOptions.CiteULike:
                            item.Text = Lang.en_US.Master_SearchOptions_CiteULike;
                            item.ImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_CITEULIKE;
                            break;
                        case SearchOptions.WorldCat:
                            item.Text = Lang.en_US.Master_SearchOptions_WorldCat;
                            item.ImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WORLDCAT;
                            break;
                        case SearchOptions.GoogleScholar:
                            item.Text = Lang.en_US.Master_SearchOptions_GoogleScholar;
                            item.ImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_GOOGLESCHOLAR;
                            break;
                        case SearchOptions.WizFolioSearch:
                            item.Text = Lang.en_US.Master_SearchOptions_WizSearch;
                            item.ImageKey = Properties.Settings.Default.IMAGEKEY_SEARCHOPTIONS_WIZSEARCH;
                            break;
                    }
                    item.Tag = (int)option;
                    item.Name = (int)option + "";                    
                    item.Click += new EventHandler(this.itemSearchOption_Click);
                    searchOptionsContextMenu.Items.Add(item);
                    if (option == SearchOptions.Filter)
                    {
                        item.CheckState = CheckState.Indeterminate;
                        item.Checked = true;
                        item.Font = new System.Drawing.Font(item.Font, FontStyle.Bold);
                        currentSearchOption = SearchOptions.Filter;
                        this.menuitemFilterFields = item;
                    }
                }                
                searchOptionsContextMenu.Height = 180;
                searchOptionsContextMenu.Closing += new ToolStripDropDownClosingEventHandler(this.searchOptionsContextMenu_Closing);
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::buildFilterFieldsContextMenu", ex.ToString());
            }
        }

        private ToolStripMenuItem[] getSortByMenuItems()
        {
            List<ToolStripMenuItem> listMenuItems = new List<ToolStripMenuItem>();
            try
            {
                foreach (SortByType sortby in Enum.GetValues(typeof(SortByType)))
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    switch (sortby)
                    {
                        case SortByType.Title:
                            item.Text = Lang.en_US.Master_SortBy_Title;
                            break;
                        case SortByType.Author:
                            item.Text = Lang.en_US.Master_SortBy_Author;
                            break;
                        case SortByType.Year:
                            item.Text = Lang.en_US.Master_SortBy_Year;
                            break;
                        case SortByType.JournalName:
                            item.Text = Lang.en_US.Master_SortBy_Journal;
                            break;
                        case SortByType.ItemType:
                            item.Text = Lang.en_US.Master_SortBy_ItemType;
                            break;
                        case SortByType.None:
                            item.Text = Lang.en_US.Master_SortBy_None;
                            break;
                    }
                    item.Tag = (int)sortby;
                    item.Name = (int)sortby + "";
                    item.Click += new EventHandler(itemSortby_Click);
                    listMenuItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::getSortByMenuItems", ex.ToString());
            }
            return listMenuItems.ToArray();
        }
        private ToolStripMenuItem[] getSortByDirectionMenuItems()
        {
            List<ToolStripMenuItem> listMenuItems = new List<ToolStripMenuItem>();

            ToolStripMenuItem itemSortAscending = new ToolStripMenuItem();
            itemSortAscending.Text = Lang.en_US.SortAscending_Label;
            itemSortAscending.CheckState = CheckState.Indeterminate;
            itemSortAscending.Checked = this.bIsAscending;
            itemSortAscending.Tag = "sortascending";
            itemSortAscending.Click += new EventHandler(this.itemSortByDirection_Click);
            listMenuItems.Add(itemSortAscending);

            ToolStripMenuItem itemSortDescending = new ToolStripMenuItem();
            itemSortDescending.Text = Lang.en_US.SortDescending_Label;
            itemSortDescending.CheckState = CheckState.Indeterminate;
            itemSortDescending.Checked = !this.bIsAscending;
            itemSortDescending.Tag = "sortdescending";
            itemSortDescending.Click += new EventHandler(this.itemSortByDirection_Click);
            listMenuItems.Add(itemSortDescending);
                        
            return listMenuItems.ToArray();
        }
        private string getMappedFolderImage(FolderType type)
        {
            string strImageKey = Properties.Settings.Default.IMAGEKEY_NORMALFOLDER;
            switch (type)
            {
                case FolderType.Collection:
                case FolderType.PublicCollections:
                    strImageKey = Properties.Settings.Default.IMAGEKEY_SHAREDCOLLECTION;
                    break;
                case FolderType.MyPublication:
                    strImageKey = Properties.Settings.Default.IMAGEKEY_PUBLICATIONFOLDER;
                    break;
                case FolderType.Normal:
                    strImageKey = Properties.Settings.Default.IMAGEKEY_NORMALFOLDER;
                    break;
                case FolderType.Trash:
                    strImageKey = Properties.Settings.Default.IMAGEKEY_RECYCLEBIN;
                    break;
                case FolderType.ALL:
                    strImageKey = Properties.Settings.Default.IMAGEKEY_ALLITEMS;
                    break;
                case FolderType.Search:
                    strImageKey = Properties.Settings.Default.IMAGEKEY_WIZADD;
                    break;
            }
            return strImageKey;
        }
       
        // Process search option state        
        private void processSearchOptionState(SearchOptions option)
        {
            currentSearchOption = option;
            bool bNoChanged = false;
            foreach (ToolStripMenuItem item in searchOptionsContextMenu.Items)
            {
                if (item.Checked)
                {
                    bNoChanged = bNoChanged | (option == (SearchOptions)item.Tag);
                    item.Checked = false;
                    item.Font = new System.Drawing.Font(item.Font, FontStyle.Regular);
                }
                if(option == (SearchOptions) item.Tag)
                {
                    item.CheckState = CheckState.Indeterminate;
                    item.Checked = true;
                    item.Font = new System.Drawing.Font(item.Font, FontStyle.Bold);
                }
            }
            btnMore.Enabled = (option != SearchOptions.Filter);
            menuitemFilterFields.Enabled = (option == SearchOptions.Filter);
            inputSearch.TextChanged -= new EventHandler(inputSearch_TextChanged);
            inputSearch.Text = getDefaultInputSearchText(option);
            btnSearch.Enabled = true;
            inputSearch.Enabled = true;
            if (!bNoChanged)
                ctrlPreview.ClearPreview();
            switch (option)
            {
                case SearchOptions.Filter:
                    btnSearch.BackgroundImage = Properties.Resources.Filter;              
                    inputSearch.TextChanged +=new EventHandler(inputSearch_TextChanged);
                    inputSearch.KeyDown -= new KeyEventHandler(inputSearch_KeyDownEnter);
                    listItems.EmptyListMsg = Lang.en_US.Master_NoItems_Label;
                    break;
                case SearchOptions.PubMed:
                    btnSearch.BackgroundImage = Properties.Resources.PubMed;
                    break;
                case SearchOptions.ScholarsPortal:
                    btnSearch.BackgroundImage = Properties.Resources.ScholarsPortal;
                    break;
                case SearchOptions.CiteULike:
                    btnSearch.BackgroundImage = Properties.Resources.CiteUlike;
                    break;
                case SearchOptions.WorldCat:
                    btnSearch.BackgroundImage = Properties.Resources.WorldCat;
                    break;
                case SearchOptions.GoogleScholar:
                    btnSearch.BackgroundImage = Properties.Resources.GoogleScholar;
                    break;
                case SearchOptions.WizFolioSearch:
                    btnSearch.BackgroundImage = Properties.Resources.WizFolioSearch;                     
                    break;
            }
            if(option != SearchOptions.Filter)
            {
                inputSearch.KeyDown += new KeyEventHandler(inputSearch_KeyDownEnter);
                listItems.EmptyListMsg = Lang.en_US.Master_ClickEnterToSearch_Label;
                btnMore.Enabled = true;
            }
        }
        private void jumpToSearchOptionNode(SearchOptions searchOption)
        {
            foreach(TreeNode node in nodeSearchDBs.Nodes)
            {
                if(node.Name == searchOption + "")
                {
                    this.treeFolders.SelectedNode = node;
                    break;
                }
            }
        }
        private bool isDefaultInputSearchText(SearchOptions option)
        {
            return (inputSearch.Text == getDefaultInputSearchText(option));
        }
        private string getDefaultInputSearchText(SearchOptions option)
        {
            string text = string.Empty;
            switch (option)
            {
                case SearchOptions.Filter:
                    text = Lang.en_US.Master_InputSearch_Filter_Label;
                    break;
                case SearchOptions.PubMed:
                    text = Lang.en_US.Master_InputSearch_PubMed_Label;
                    break;
                case SearchOptions.ScholarsPortal:
                    text = Lang.en_US.Master_InputSearch_ScholarsPortal_Label;
                    break;
                case SearchOptions.CiteULike:
                    text = Lang.en_US.Master_InputSearch_CiteULike_Label;
                    break;
                case SearchOptions.GoogleScholar:
                    text = Lang.en_US.Master_InputSearch_GoogleScholar_Label;
                    break;
                case SearchOptions.WizFolioSearch:
                    text = Lang.en_US.Master_InputSearch_WizSearch_Label;
                    break;
                case SearchOptions.WorldCat:
                    text = Lang.en_US.Master_InputSearch_WorldCat_Label;
                    break;
            }
            return text;
        }
        private void enableSearchControls(bool bEnabled)
        {
            inputSearch.Enabled = btnSearch.Enabled = btnMore.Enabled = listItems.Enabled = bEnabled;
        }
        private void enableCitationControls(bool bEnabled)
        {
            btnCite.Enabled = btnStyle.Enabled = btnRefList.Enabled = btnRefresh.Enabled = btnHighlight.Enabled = bEnabled;
        }
        private void startSearchProgress(SearchOptions option)
        {
            if (masterThread.IsAlive && (masterThread.Name == "loadingitems" || masterThread.Name == "searchingitems"))
                masterThread.Abort();
            enableEmptyMsg(false);
            prepareLoadingItemsLabel();
            enableSearchControls(false);
            enableCitationControls(false);
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(masterControlThread.SearchItems);
            masterThread = new Thread(threadStart);
            masterThread.Name = "searchingitems";
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["source"] = (int) option + "";
            data["start"] = listItems.Items.Count + "";
            data["query"] = this.inputSearch.Text;
            masterThread.Start(data);
        }

        #endregion

        #region Event Handlers

        #region MasterControl Events

        // Prepare / Initialization
        private void MasterControl_Load(object sender, EventArgs e)
        {
            initializeUI();
            initializeLang();
            initializeHandlers(); // 初始一些動作相關的處理，例如在 listItem 點選某筆記錄的時候要做什麼
            loadFolders();
        }
        // Focusing into the document when press ESC
        private void MasterControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                documentFocus();
        }        
        // Handle resize event
        private void MasterControl_Resized(object sender, EventArgs e)
        {
            try
            {
                if (bMoving)
                {
                    if (Globals.ThisAddIn.DockRepositionFinished())
                        dockRepositionFinished();
                    else
                        return;
                }
                Globals.ThisAddIn.iTempWidth = Globals.ThisAddIn.GetCurrentCustomPaneWidth();
                Globals.ThisAddIn.iTempHeight = Globals.ThisAddIn.GetCurrentCustomPaneHeight();
                inputSearch.Width = inputSearch.Parent.Width;
                adjustLoadingLabels();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::MasterControl_Resized", ex.ToString());
            }
        }

        #endregion

        #region ListItems Events

        // Display tooltip on item in ObjectViewList
        private void listItems_CellToolTipShowing(object sender, BrightIdeasSoftware.ToolTipShowingEventArgs e)
        {
            ItemMasterRow item = (ItemMasterRow)e.Model;
            if (e.ColumnIndex == 2)
            {
                if (item.ItemFile != string.Empty && item.ItemFile.Length > 0)
                {
                    e.Text = String.Format("{0}\r\n{1}", Regex.Replace(item.ItemFile, @"^\d+_\d+_|_$", ""), Lang.en_US.Master_ClickOpen_Tooltip);
                }
                else if (item.ItemTypeID == ItemTypes.WebPage)
                {
                    e.Text = String.Format("{0}\r\n{1}", item.Affiliation, Lang.en_US.Master_ClickOpenLink_Tooltip);
                }
            }
            else
            {
                e.Text = String.Format("{0}", item.Title);
            }
        }
        
        // Handling event when listItems got focus, change RowBorderRowdecoration, change TabIndex
        private void listItems_GotFocus(object sender, EventArgs e)
        {
            rbd.BorderPen = new Pen(Color.FromArgb(128, Color.DarkGreen), 1);
            rbd.FillBrush = new SolidBrush(Color.FromArgb(50, Color.DarkGreen));
            rbd.BoundsPadding = new Size(-2, -2);
            rbd.CornerRounding = 6.0f;
            this.listItems.SelectedRowDecoration = rbd;
            this.listItems.Invalidate();
            listItems.TabIndex = 0;
            inputSearch.TabIndex = 1;
        }
        
        // Change Decoration when ListItem lost focus
        private void listItems_LostFocus(object sender, EventArgs e)
        {
            rbd.BorderPen = new Pen(Color.FromArgb(170, Color.DarkGray), 1);
            rbd.FillBrush = new SolidBrush(Color.FromArgb(40, Color.Black));
            rbd.BoundsPadding = new Size(-2, -2);
            rbd.CornerRounding = 6.0f;
            this.listItems.SelectedRowDecoration = rbd;
            //this.listItems.Invalidate();
        }

        // Apply different decoration for Alternative rows
        private void listItems_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            if (e.RowIndex % 2 == 1)
                e.Item.Decoration = alternateRBD;
        }
        
        // Input data and decoration for cell
        private void listItems_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            try
            {
                ItemMasterRow item = (ItemMasterRow)e.Model;
                if (e.ColumnIndex == 0)
                {
                    BrightIdeasSoftware.ImageDecoration imageDeco = new BrightIdeasSoftware.ImageDecoration();
                    imageDeco.Image = listIcons.Images[item.ItemTypeID.ToString()];
                    imageDeco.Transparency = 255;
                    imageDeco.Alignment = ContentAlignment.MiddleCenter;
                    imageDeco.ShrinkToWidth = true;
                    e.SubItem.Decoration = imageDeco;
                }
                else if (e.ColumnIndex == 1)
                {
                    ItemDecoration decoration = new ItemDecoration();
                    decoration.Title = item.Title;
                    if (item.PubYear == "-1" || item.PubYear == null)
                    {
                        if (Regex.Match(item.PubDate, @"\d{4}").Success)
                            decoration.Year = Regex.Match(item.PubDate, @"\d{4}").Value;
                        else
                            decoration.Year = "";
                    }
                    else
                        decoration.Year = item.PubYear;
                    decoration.Authors = item.Author = CitationTools.GetAuthorsString(item.Authors);
                    e.SubItem.Decoration = decoration;
                }
                else if (e.ColumnIndex == 2) //如果有 3 行(通常都是 連結 或者 pdf 檔案的圖示)
                {
                    if (item.ItemFile.Length > 0 && item.ItemFile != string.Empty)
                    {
                        BrightIdeasSoftware.ImageDecoration imageDeco = new BrightIdeasSoftware.ImageDecoration();
                        imageDeco.Image = CitationTools.GetItemFileIcon(item.ItemFile);
                        if (imageDeco.Image != null)
                        {
                            imageDeco.Transparency = 255;
                            imageDeco.Offset = new Size(-5, 0);
                            imageDeco.ShrinkToWidth = true;
                            imageDeco.Alignment = ContentAlignment.MiddleRight;
                            e.SubItem.Decoration = imageDeco;
                        }
                    }
                    else if (item.ItemTypeID == ItemTypes.WebPage)
                    {
                        BrightIdeasSoftware.ImageDecoration imageDeco = new BrightIdeasSoftware.ImageDecoration();
                        imageDeco.Image = CitationTools.GetItemFileIcon("wizfolio.wfweb");
                        if (imageDeco.Image != null)
                        {
                            imageDeco.Transparency = 255;
                            imageDeco.Offset = new Size(-5, 0);
                            imageDeco.ShrinkToWidth = true;
                            imageDeco.Alignment = ContentAlignment.MiddleRight;
                            e.SubItem.Decoration = imageDeco;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::listItems_FormatCell", ex.ToString());
            }
        }
        
        // Check whether the Click event is on the ItemFile icon column. -> trigger download if available
        private void listItems_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                ItemMasterRow item = (ItemMasterRow)e.Model;
                if (item.ItemFile != string.Empty && item.ItemFile.Length > 0)
                    DownloadFile(item.ItemFile);
                else if (item.ItemTypeID == ItemTypes.WebPage)
                    CitationTools.ExecuteApplication(item.Affiliation);
            }
        }
        
        // Show preview panel if this panel is hidden
        private void listItems_DoubleClick(object sender, EventArgs e)
        {
            this.btnShowHidePreview_Click(null, null);
        }
        
        // Handle KeyDown event (only for Delete button)
        private void listItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (listItems.Focused)
            {
                if (e.KeyCode == Keys.Delete)
                    inputSearch.Focus();
            }
        }
        
        // Handle KeyPress event (for SpaceBar, character or Escape)
        private void listItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (listItems.Focused)
                {
                    if (Char.IsLetterOrDigit(e.KeyChar))
                    {
                        if(inputSearch.Enabled)
                        {
                            inputSearch.Focus();
                            inputSearch.DeselectAll();
                            inputSearch.AppendText(e.KeyChar.ToString());
                        }                        
                    }
                    else if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return)
                    {
                        btnCite_Click(null, null);
                    }
                    else if (Convert.ToInt32(e.KeyChar) == 8)
                    {
                        if(inputSearch.Enabled)
                        {
                            inputSearch.Focus();
                            inputSearch.DeselectAll();
                            inputSearch.Text = inputSearch.Text.Remove(inputSearch.Text.Length - 1, 1);
                            inputSearch.SelectionStart = inputSearch.Text.Length;
                            inputSearch.SelectionLength = 0;
                        }                        
                    }
                    else if (e.KeyChar == (char) Keys.Escape)
                    {
                        documentFocus();
                    }
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::listItems_KeyPress", ex.ToString());
            }
        }
        
        // Handle event when Selected item is changed
        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listItems.Items.Count == 0 || listItems.SelectedItems.Count == 0)
            {
                ctrlPreview.ClearPreview();
                btnCite.Enabled = false;
                btnHighlight.Enabled = false;
                btnHighlight.Visible = false;
                return;
            }            
            ItemMasterRow item = (ItemMasterRow)listItems.SelectedObject;
            if (item == null && listItems.SelectedObjects.Count > 0)
                item = (ItemMasterRow)listItems.SelectedObjects[0];

            if (item != null)
            {
                btnCite.Enabled = true;
                if (bUsingCitedList)
                {
                    btnHighlight.Enabled = true;
                    btnHighlight.Visible = true;
                }
                ctrlPreview.SetPreview(item);
            }
            else
            {
                btnCite.Enabled = false;
                btnHighlight.Enabled = false;
                btnHighlight.Visible = false;
                ctrlPreview.ClearPreview();
            }
        }
        
        private void listItems_Scroll(object sender, ScrollEventArgs e)
        {
            if(e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                MessageBox.Show("X");
            }
        }

        #endregion

        #region InputSearch Events

        private void inputSearch_TextChanged(object sender, EventArgs e)
        {
            if (currentSearchOption != SearchOptions.Filter)
            {
                inputSearch.TextChanged -= new EventHandler(inputSearch_TextChanged);
                return;
            }
            try
            {
                string strFilter = inputSearch.Text;
                if (strFilter.Length > 0 && strFilter != Lang.en_US.Master_InputSearch_Text)
                    loadItems(true, strFilter, false, null, "");
                else
                {
                    if (masterThread.IsAlive && masterThread.Name == "loadingitems")
                        masterThread.Abort();
                    if(this.bIsAscending)
                    {
                        switch (currentSortBy)
                        {
                            case SortByType.Title:
                                itemList = itemList.OrderBy(ItemMasterRow => ItemMasterRow.Title);
                                break;
                            case SortByType.Author:
                                itemList = itemList.OrderBy(ItemMasterRow => Tools.CitationTools.GetAuthorsSortableString(ItemMasterRow.Authors));
                                break;
                            case SortByType.Year:
                                itemList = itemList.OrderBy(ItemMasterRow => ItemMasterRow.PubYear);
                                break;
                            case SortByType.JournalName:
                                itemList = itemList.OrderBy(ItemMasterRow => ItemMasterRow.Title2);
                                break;
                            case SortByType.ItemType:
                                itemList = itemList.OrderBy(ItemMasterRow => ItemMasterRow.ItemTypeID);
                                break;
                            default:
                                itemList = itemList.OrderBy(ItemMasterRow => ItemMasterRow.SequenceNo);
                                break;
                        }
                    }
                    else
                    {
                        switch (currentSortBy)
                        {
                            case SortByType.Title:
                                itemList = itemList.OrderByDescending(ItemMasterRow => ItemMasterRow.Title);
                                break;
                            case SortByType.Author:
                                itemList = itemList.OrderByDescending(ItemMasterRow => Tools.CitationTools.GetAuthorsSortableString(ItemMasterRow.Authors));
                                break;
                            case SortByType.Year:
                                itemList = itemList.OrderByDescending(ItemMasterRow => ItemMasterRow.PubYear);
                                break;
                            case SortByType.JournalName:
                                itemList = itemList.OrderByDescending(ItemMasterRow => ItemMasterRow.Title2);
                                break;
                            case SortByType.ItemType:
                                itemList = itemList.OrderByDescending(ItemMasterRow => ItemMasterRow.ItemTypeID);
                                break;
                            default:
                                itemList = itemList.OrderByDescending(ItemMasterRow => ItemMasterRow.SequenceNo);
                                break;
                        }
                    }
                    this.listItems.SetObjects(itemList);
                    if (this.listItems.Items.Count > 0)
                        this.listItems.Items[0].Selected = true;
                    listItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
                }
                if (listItems.Items.Count == 0)
                    ctrlPreview.ClearPreview();
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::inputSearch_TextChanged", ex.ToString());
            }
        }
        private void inputSearch_KeyDownEnter(object sender, KeyEventArgs e)
        {
            if (inputSearch.Focused)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (currentSearchOption == SearchOptions.Filter)
                        return;
                    else
                    {
                        listItems.ClearObjects();
                        startSearchProgress(currentSearchOption);
                    }
                }
            }

        }
        private void inputSearch_GotFocus(object sender, EventArgs e)
        {
            this.inputSearch.TextChanged -= new System.EventHandler(this.inputSearch_TextChanged);
            if (isDefaultInputSearchText(currentSearchOption))
                inputSearch.Text = "";
            else
            {
                if (MouseButtons == MouseButtons.Left || MouseButtons == MouseButtons.None)
                {
                    this.inputSearch.SelectAll();
                    bSearchFocused = true;
                }
            }
            this.inputSearch.TextChanged += new System.EventHandler(this.inputSearch_TextChanged);
            inputSearch.ForeColor = System.Drawing.SystemColors.WindowText;
            listItems.TabIndex = 1;
            inputSearch.TabIndex = 0;
        }
        private void inputSearch_LostFocus(object sender, EventArgs e)
        {
            this.inputSearch.TextChanged -= new System.EventHandler(this.inputSearch_TextChanged);
            if (inputSearch.Text == "" || inputSearch.Text.Length == 0)
            {
                inputSearch.Text = getDefaultInputSearchText(currentSearchOption);
                inputSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            else
                inputSearch.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.inputSearch.TextChanged += new System.EventHandler(this.inputSearch_TextChanged);
        }
        private void inputSearch_Leave(object sender, EventArgs e)
        {
            bSearchFocused = false;
        }
        private void inputSearch_MouseUp(object sender, MouseEventArgs e)
        {
            if (!bSearchFocused && this.inputSearch.SelectionLength == 0)
            {
                bSearchFocused = true;
                this.inputSearch.SelectAll();
            }
        }
        private void inputSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputSearch.Focused)
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                    listItems.Focus();
                else if(e.KeyCode == Keys.PageDown)
                {
                    listItems.Focus();
                    int currIndex = -1;
                    if(listItems.SelectedIndex != -1)
                        currIndex = listItems.SelectedIndex;
                    else if(listItems.SelectedIndices.Count > 0)
                        currIndex = listItems.SelectedIndices[0];
                    if(currIndex != -1)
                    {
                        int step = 10;
                        OLVListItem item = null;
                        if(currIndex + step < listItems.Items.Count)
                            item = listItems.GetItem(currIndex + step);
                        if(item == null)
                            item = listItems.GetLastItemInDisplayOrder();
                        if(item != null)
                        {
                            listItems.SelectedItem = item;
                            listItems.EnsureVisible(item.Index);
                        }                        
                    }
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    listItems.Focus();
                    int currIndex = -1;
                    if (listItems.SelectedIndex != -1)
                        currIndex = listItems.SelectedIndex;
                    else if (listItems.SelectedIndices.Count > 0)
                        currIndex = listItems.SelectedIndices[0];
                    if (currIndex != -1)
                    {
                        int step = 10;
                        OLVListItem item = null;
                        if (currIndex - step > -1)
                            item = listItems.GetItem(currIndex - step);
                        if (item == null)
                            item = listItems.GetItem(0);
                        if (item != null)
                        {
                            listItems.SelectedItem = item;
                            listItems.EnsureVisible(item.Index);
                        }
                    }
                }
            }
                
        }
                
        #endregion

        #region Buttons Events

        private void btnMore_Click(object sender, EventArgs e)
        {
            startSearchProgress(currentSearchOption);
        }
        // Jump to the next matched in-text citation
        private void btnHighlight_Click(object sender, EventArgs e)
        {
            try
            {
                ItemMasterRow item = (ItemMasterRow)listItems.SelectedObject;
                if (item == null && listItems.SelectedObjects.Count > 0)
                    item = (ItemMasterRow)listItems.SelectedObjects[0];
                formatter.FindItem(item);
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::btnHighlight_Click", ex.ToString());
            }            
        }        
        // Refresh to generate latest reference list -> show the list, btnHighLight will be enabled
        private void btnRefList_Click(object sender, EventArgs e)
        {
            try
            {
                if (bUsingCitedList)
                {
                    clearSearchFilter();
                    enableEmptyMsg(false);
                    btnRefresh_Click(null, null);
                }
                else
                    treeFolders.SelectedNode = nodeCitedRef;
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::btnRefList_Click", ex.ToString());
            }     
        }
        // Refresh the references in the current document
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(formatter.RefreshCitationsInThread);
                masterThread = new Thread(threadStart);
                masterThread.Name = "refreshingcitations";
                masterThread.Start(this);
                progress = new ProgressForm(Lang.en_US.Progress_RefreshCitation_Title, Lang.en_US.Progress_RefreshCitation_Msg, true);
                progress.FormClosed += new FormClosedEventHandler(progress_FormClosed);
                progress.ShowDialog();
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::btnRefList_Click", ex.ToString());
            }            
        }        
        // Cite the selected items
        private void btnCite_Click(object sender, EventArgs e)
        {
            try
            {
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(formatter.InsertCitationsInThread);
                masterThread = new Thread(threadStart);
                masterThread.Name = "insertingcitations";
                List<ItemMasterRow> listSelectedItems = new List<ItemMasterRow>();
                foreach (ItemMasterRow item in listItems.SelectedObjects)
                {
                    listSelectedItems.Add(item);
                }
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["Control"] = this;
                data["ListItems"] = listSelectedItems;
                if (currentSearchOption != SearchOptions.Filter)
                    data["FromSearch"] = 1;

                masterThread.Start(data);
                progress = new ProgressForm(Lang.en_US.Progress_InsertCitation_Title, Lang.en_US.Progress_InsertCitation_Msg, true);
                progress.FormClosed += new FormClosedEventHandler(this.progress_FormClosed);
                progress.ShowDialog();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::btnCite_Click", ex.ToString());
            }
        }        
        // Logout of the account
        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            this.splitParent.SplitterMoved -= new SplitterEventHandler(splitParent_SplitterMoved);
            this.splitChild.SplitterMoved -= new SplitterEventHandler(splitChild_SplitterMoved);
            Globals.ThisAddIn.iTempWidth = this.Width;
            Globals.ThisAddIn.iTempHeight = this.Height;
            Globals.ThisAddIn.Logout();
        }
        // Reload folders and items
        private void btnReload_Clicked(object sender, EventArgs e)
        {
            this.splitParent.SplitterMoved -= new SplitterEventHandler(splitParent_SplitterMoved);
            this.splitChild.SplitterMoved -= new SplitterEventHandler(splitChild_SplitterMoved);
            loadFolders();
        }
        // Show the context menu
        private void btnSortBy_Click(object sender, EventArgs e)
        {
            Point p = new Point();
            p.Y = this.btnSortBy.Location.Y + this.btnSortBy.Height;
            p.X = this.btnSortBy.Location.X + this.btnSortBy.Width - this.contextmenu.Width;
            this.contextmenuSortBy.Show(this.tableMiddle, p);
        }
        // Show option to select fields for Filter
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Point p = new Point();
            p.Y = this.btnSearch.Location.Y - this.searchOptionsContextMenu.Height - 3;
            p.X = this.btnSearch.Location.X + this.btnSearch.Width - this.searchOptionsContextMenu.Width;
            this.searchOptionsContextMenu.Show(this.tableMiddle, p);
        }
        // Display Style Control Window
        private void btnStyle_Click(object sender, EventArgs e)
        {
            Globals.ThisAddIn.ShowStyleChooserForm(formatter);
        }        
        
        private void btnShowHidePreview_Click(object sender, EventArgs e)
        {            
            splitChild.Panel2Collapsed = !splitChild.Panel2Collapsed;
            this.menuitemShowHidePreview.Text = this.splitChild.Panel2Collapsed ? Lang.en_US.Master_ShowPreview_MenuItem : Lang.en_US.Master_HidePreview_MenuItem;
            Properties.Settings.Default.MASTER_PREVIEW_COLLAPSED = splitChild.Panel2Collapsed;
            if (listItems.SelectedItem != null)
                listItems.SelectedItem.EnsureVisible();
            listItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
        }

        private void btnTemplate_Click(object sender, EventArgs e)
        {
            if (Templateform == null || Templateform.IsDisposed)
            {
                Templateform = new TemplateForm();
                Templateform.Show();
            }
            else
            {
                Templateform.Activate();
                Templateform.WindowState = FormWindowState.Normal;
            }

            //上傳模組程式碼
            //WebClient myWebClient = new WebClient();

            //Uri uriString = new Uri("http://provider.pifolio.com/pages/uploadmodel.aspx?email=ssc1223@gmail.com&pw=123456");

            //byte[] responseArray = myWebClient.UploadFile(uriString, "POST", @"c:\123.txt");

            //string test = Encoding.ASCII.GetString(responseArray);

            WebClient myWebClient = new WebClient();

            Uri uriString = new Uri("http://provider.pifolio.com/pages/uploadmodel.aspx?email=ssc1223@gmail.com&pw=123456&title=MyBigTestTitle");

            byte[] responseArray = myWebClient.UploadFile(uriString, "POST", @"c:\BigTitle.docx");

            string test = Encoding.ASCII.GetString(responseArray);
        }

        #endregion

        #region Other Events

        private void ThisAddIn_DockPositionChanged(object sender, EventArgs e)
        {
            bMoving = true;
            prepareLayout();
        }
        private void splitParent_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (Globals.ThisAddIn.DockRepositionFinished())
            {
                if (this.splitParent.Orientation == Orientation.Horizontal)
                    Properties.Settings.Default.MASTER_VERTICAL_SPLITTERPARENT_RATIO = (double)this.splitParent.SplitterDistance / this.Height;
                else
                    Properties.Settings.Default.MASTER_HORIZONTAL_SPLITTERPARENT_RATIO = (double)this.splitParent.SplitterDistance / this.Width;
                listItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
                adjustLoadingLabels();                
            }
        }
        private void splitChild_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (Globals.ThisAddIn.DockRepositionFinished())
            {
                if (this.splitChild.Orientation == Orientation.Horizontal)
                    Properties.Settings.Default.MASTER_VERTICAL_SPLITTERCHILD_RATIO = (double)this.splitChild.SplitterDistance / this.Height;
                else
                    Properties.Settings.Default.MASTER_HORIZONTAL_SPLITTERCHILD_RATIO = (double)this.splitChild.SplitterDistance / this.Width;
                listItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
                adjustLoadingLabels();
            }
        }        
        private void progress_FormClosed(object sender, FormClosedEventArgs e)
        {
            ThreadStart thStart = new ThreadStart(masterControlThread.DocumentFocus);
            masterThread = new Thread(thStart);
            masterThread.Name = "focusingdocument";
            masterThread.Start();
        }
        private void dragSource_endDragEvent()
        {
            try
            {
                if (isInsideDocument())
                    InsertCitations();
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::dragSource_endDragEvent", ex.ToString());
            }
        }
        private void contextMenu_Collapse(object sender, EventArgs e)
        {
            if(bFilterOptionClicked)
                bFilterOptionClicked = false;
        }        
        private void menuitemShowHidePreview_Click(object sender, EventArgs e)
        {
            btnShowHidePreview_Click(null, null);
        }
        private void menuitemFilterFieldsDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                e.Cancel = true;
        }
        private void itemSortby_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem currentItem = (ToolStripMenuItem)sender;
                currentSortBy = (SortByType)currentItem.Tag;
                Properties.Settings.Default.DEFAULT_SORTBY = (int)currentSortBy;
                foreach (ToolStripMenuItem item in menuitemSortBy.DropDownItems.OfType<ToolStripMenuItem>())
                {
                    if (item.Checked && item.Tag.ToString() != "sortascending" && item.Tag.ToString() != "sortdescending")
                        item.Checked = false;

                    if (item.Text == currentItem.Text)
                    {
                        item.CheckState = CheckState.Indeterminate;
                        item.Checked = true;
                    }
                }
                foreach (ToolStripMenuItem item in contextmenuSortBy.Items.OfType<ToolStripMenuItem>())
                {
                    if (item.Checked && item.Tag.ToString() != "sortascending" && item.Tag.ToString() != "sortdescending")
                        item.Checked = false;

                    if (item.Text == currentItem.Text)
                    {
                        item.CheckState = CheckState.Indeterminate;
                        item.Checked = true;
                    }
                }
                inputSearch_TextChanged(null, null);
                listItems.Focus();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::itemSortby_Click", ex.ToString());
            }            
        }
        private void itemFilterField_Click(object sender, EventArgs e)
        {
            try
            {
                bFilterOptionClicked = true;
                ToolStripMenuItem currentItem = (ToolStripMenuItem)sender;
                int iFieldValue = int.Parse(currentItem.Tag.ToString());
                int iFilterFields = Properties.Settings.Default.DEFAULT_FILTER_FIELDS;
                iFilterFields = currentItem.Checked ? (iFilterFields - iFieldValue) : (iFilterFields + iFieldValue);
                bool status = !currentItem.Checked;
                ((ToolStripMenuItem)menuitemFilterFields.DropDownItems[currentItem.Name]).Checked = status;
                Properties.Settings.Default.DEFAULT_FILTER_FIELDS = iFilterFields;
                inputSearch_TextChanged(null, null);
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::itemFilterField_Click", ex.ToString());
            }
        }
        private void itemSearchOption_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem currentItem = (ToolStripMenuItem)sender;
                
                if (currentSearchOption == (SearchOptions)currentItem.Tag)
                    return;

                currentSearchOption = (SearchOptions)currentItem.Tag;
                processSearchOptionState(currentSearchOption);                
                searchOptionsContextMenu.Close();
                if(currentSearchOption != SearchOptions.Filter)
                    jumpToSearchOptionNode(currentSearchOption);                
                listItems.Focus();
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::itemSearchOption_Click", ex.ToString());
            }
        }
        private void itemSortByDirection_Click(object sender, EventArgs e)
        {
            this.bIsAscending = !this.bIsAscending;

            ToolStripMenuItem currentItem = (ToolStripMenuItem)sender;
            foreach (ToolStripMenuItem item in menuitemSortBy.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if (item.Tag + "" == "sortdescending")
                {
                    if (!this.bIsAscending)
                    {
                        item.CheckState = CheckState.Indeterminate;
                        item.Checked = true;
                    }
                    else
                        item.Checked = false;
                }
                else if (item.Tag + "" == "sortascending")
                {
                    if (this.bIsAscending)
                    {
                        item.CheckState = CheckState.Indeterminate;
                        item.Checked = true;
                    }
                    else
                        item.Checked = false;
                }
            }

            foreach (ToolStripMenuItem item in contextmenuSortBy.Items.OfType<ToolStripMenuItem>())
            {
                if (item.Tag + "" == "sortdescending")
                {
                    if (!this.bIsAscending)
                    {
                        item.CheckState = CheckState.Indeterminate;
                        item.Checked = true;
                    }
                    else
                        item.Checked = false;
                }
                else if (item.Tag + "" == "sortascending")
                {
                    if (this.bIsAscending)
                    {
                        item.CheckState = CheckState.Indeterminate;
                        item.Checked = true;
                    }
                    else
                        item.Checked = false;
                }
            }

            if (this.bIsAscending)
                this.btnSortBy.Image = Properties.Resources.sort_asc;
            else
                this.btnSortBy.Image = Properties.Resources.sort_des;

            Properties.Settings.Default.DEFAULT_IS_SORTDIRECTION_ASCENDING = this.bIsAscending;
            this.inputSearch_TextChanged(null, null);
        }
        private void filterContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                e.Cancel = true;
        }
        private void searchOptionsContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                e.Cancel = true;
        }
        private void lblCurrentUserName_Clicked(object sender, EventArgs e)
        {
            try
            {
                string url = Properties.Settings.Default.URL_ROOTSERVER + (string.Format(Properties.Settings.Default.URI_LOGINACCESS, Globals.ThisAddIn.user.Authentication, Globals.ThisAddIn.user.ServerAddress.Replace("https://", "http://")));
                CitationTools.ExecuteApplication(url);
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::lblCurrentUserName_Clicked", ex.ToString());
            }
        }

        private void treeFolders_KeyUp(object sender, KeyEventArgs e)
        {
            if (treeFolders.Focused)
            {
                if (Char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)))
                {
                    if (inputSearch.Enabled)
                    {
                        inputSearch.Focus();
                        inputSearch.DeselectAll();
                        inputSearch.AppendText(((inputSearch.Text.Length > 0) ? " " : "") + Convert.ToChar(e.KeyCode).ToString().ToLower());
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                    documentFocus();
            }
        }
        private void treeFolders_ChangeSelectedFolder(object sender, EventArgs e)
        {
            try
            {
                btnHighlight.Enabled = false;
                btnHighlight.Visible = false;
                
                resetCurrentSortByMenu(currentSortBy);
                clearSearchFilter();
                this.listItems.Items.Clear();
                if (treeFolders.SelectedNode.Name.Contains("colleaguefolder"))
                {
                    enableEmptyMsg(false);
                    Colleague colleague = (Colleague)treeFolders.SelectedNode.Tag;
                    string strFolderID = Regex.Match(treeFolders.SelectedNode.Name, @"\d+").Value;
                    loadItems(false, string.Empty, true, colleague, strFolderID);
                    if (listItems.Items.Count == 0)
                        ctrlPreview.ClearPreview();
                    bUsingCitedList = false;
                    selectedFolder = null;
                    processSearchOptionState(SearchOptions.Filter);
                }
                else if (treeFolders.SelectedNode.Name == "mycolleagues" || treeFolders.SelectedNode.Name == "colleague")
                {
                    listItems.Items.Clear();
                    ctrlPreview.ClearPreview();
                    bUsingCitedList = false;
                    processSearchOptionState(SearchOptions.Filter);
                    selectedFolder = null;
                }
                else if (treeFolders.SelectedNode.Name == "cited")
                {
                    bUsingCitedList = true;
                    inputSearch.Enabled = false;
                    enableEmptyMsg(false);
                    processSearchOptionState(SearchOptions.Filter);
                    btnRefresh_Click(null, null);
                    selectedFolder = null;
                }                
                else if (bUsingCitedList || selectedFolder == null || selectedFolder.ID + "" != treeFolders.SelectedNode.Name)
                {                    
                    foreach (Folder folder in listFolders)
                    {
                        if (folder.ID + "" == treeFolders.SelectedNode.Name)
                        {
                            selectedFolder = folder;
                            loadItems(false, string.Empty, false, null, "");
                            if (listItems.Items.Count == 0)
                                ctrlPreview.ClearPreview();
                            break;
                        }
                    }
                    bUsingCitedList = false;
                    processSearchOptionState(SearchOptions.Filter);
                    enableEmptyMsg(false);
                }
                else if (treeFolders.SelectedNode.Parent != null)
                {
                    if (treeFolders.SelectedNode.Parent.Name == "searchdbs")
                    {
                        if (currentSearchOption != (SearchOptions)treeFolders.SelectedNode.Tag)
                        {
                            currentSearchOption = (SearchOptions)treeFolders.SelectedNode.Tag;
                            processSearchOptionState(currentSearchOption);
                        }
                        selectedFolder = null;
                    }
                }

                if (listItems.Items.Count == 0)
                    btnCite.Enabled = false;
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "MasterControl::treeFolders_ChangeSelectedFolder", ex.ToString());
            }
        }
        private void treeFolders_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left)
                    return;
                if ((e.Node.Name + "") == "colleague")
                {
                    if (e.Node.Nodes.Count == 0)
                    {
                        Colleague colleague = (Colleague)e.Node.Tag;
                        buildColleagueTreeRecursive(0, colleague, e.Node.Nodes);
                    }
                    e.Node.Expand();
                }
                else if ((e.Node.Name + "") == "mycolleagues")
                {
                    if (e.Node.Nodes.Count == 0 && !bBuildingColleaguesList)
                    {
                        bBuildingColleaguesList = true;
                        ParameterizedThreadStart threadStart = new ParameterizedThreadStart(masterControlThread.LoadColleagues);
                        masterThread = new Thread(threadStart);
                        masterThread.Name = "loadingcolleagues";
                        masterThread.Start(e.Node);
                        e.Node.Text = e.Node.Text + " ( Loading colleagues ... )";
                    }
                    else
                        e.Node.Expand();
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "MasterControl::treeFolders_NodeMouseDoubleClick", ex.ToString());
            }
        }
        private void treeFolders_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #endregion
    }
}