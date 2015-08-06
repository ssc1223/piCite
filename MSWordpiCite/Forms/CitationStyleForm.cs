using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using MSWordpiCite.Threads;
using MSWordpiCite.Classes;
using MSWordpiCite.Formatter;
using MSWordpiCite.Tools;
using MSWordpiCite.Controls;

namespace MSWordpiCite.Forms
{
    public delegate void DelegateReapplyStyleFinished();
    public delegate void DelegateLoadStyleFinished(List<Dictionary<string, string>> data, bool bFiltered);
    public partial class CitationStyleWindow : Form
    {
        #region Variables

        private Logger log = Globals.ThisAddIn.log;
        private BrightIdeasSoftware.RowBorderDecoration alternateRBD;
        private BrightIdeasSoftware.RowBorderDecoration rbd;
        private bool bFiltered = false;
        private bool bShowAll = false;
        private List<Dictionary<string, string>> listStyleNames;
        private DocumentFormatter formatter;
        private Thread curThread;
        private StyleControlThread stylecontrolThread;
        private ProgressForm progress;

        public DelegateReapplyStyleFinished delegateReapplyStyleFinished;
        public DelegateLoadStyleFinished delegateLoadStyleFinished;

        #endregion

        public CitationStyleWindow(DocumentFormatter _formatter)
        {
            InitializeComponent();
            formatter = _formatter;
        }

        #region Initialization

        private void CitationStyleForm_Load(object sender, EventArgs e)
        {
            initializeUI();
            initializeLang();
            initializeHandlers();
            loadStyles(false, string.Empty);
        }
        private void initializeUI()
        {
            try
            {
                this.Width = Properties.Settings.Default.DEFAULT_STYLECHOOSER_WIDTH;
                this.Height = Properties.Settings.Default.DEFAULT_STYLECHOOSER_HEIGHT;
                this.Location = new Point(Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Width - this.Width) / 2), Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Height - this.Height) / 2));
                splitter.SplitterDistance = Convert.ToInt32(0.4 * splitter.Width);
                
                BrightIdeasSoftware.TextOverlay textOverlay = this.listCitations.EmptyListMsgOverlay as BrightIdeasSoftware.TextOverlay;
                textOverlay.TextColor = Color.FromArgb(255, 96, 96, 96);
                textOverlay.BackColor = System.Drawing.Color.White;
                textOverlay.BorderWidth = 0;
                textOverlay.InsetY = 0;
                textOverlay.Font = new System.Drawing.Font("Serif", 9);

                rbd = new BrightIdeasSoftware.RowBorderDecoration();
                rbd.BorderPen = new Pen(Color.FromArgb(170, Color.DarkGray), 1);
                rbd.FillBrush = new SolidBrush(Color.FromArgb(30, Color.Black));
                rbd.BoundsPadding = new Size(-2, -2);
                rbd.CornerRounding = 6.0f;
                this.listCitations.SelectedRowDecoration = rbd;

                alternateRBD = new BrightIdeasSoftware.RowBorderDecoration();
                alternateRBD.BorderPen = new Pen(Color.LightGray, 1);
                alternateRBD.FillBrush = null;
                alternateRBD.BoundsPadding = new Size(2, 0);
                alternateRBD.CornerRounding = 0;

                buildGroupListIcons();
                checkDPISetting();
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::initializeUI", ex.ToString());
            }
        }
        private void initializeLang()
        {
            listCitations.EmptyListMsg = Lang.en_US.StyleChooser_EmptyList_Label;
            tooltip.SetToolTip(this.btnShowAllStyles, Lang.en_US.StyleChooser_BtnShowAll_Tooltip);
            inputSearch.Text = Lang.en_US.StyleChooser_InputSearch_Text;
            this.Text = Lang.en_US.StyleChooser_Title_Label;
        }
        private void initializeHandlers()
        {
            this.CancelButton = btnCancel;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.listCitations.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.listCitations_CellToolTipShowing);
            this.listCitations.FormatCell += new EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.listCitations_FormatCell);
            this.listCitations.FormatRow += new EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.listCitations_FormatRow);
            this.listCitations.GotFocus += new EventHandler(this.listCitations_GotFocus);
            this.listCitations.LostFocus += new EventHandler(this.listCitations_LostFocus);
            this.listCitations.KeyDown += new KeyEventHandler(this.listCitations_KeyDown);
            this.listCitations.KeyPress += new KeyPressEventHandler(this.listCitations_KeyPress);
            this.listCitations.SelectedIndexChanged += new EventHandler(this.listCitations_SelectedIndexChanged);
            this.olvColumnType.GroupFormatter += new BrightIdeasSoftware.GroupFormatterDelegate(this.olvColumnType_GroupFormatter);
            this.olvColumnIcon.AspectToStringConverter = delegate(object x)
            {
                return "";
            };
            this.ctrlPreview.reloadAllStyleEvent += new MSWordpiCite.Controls.ReloadAllStyleHandler(this.ctrlPreview_reloadAllStyleEvent);
            this.ctrlPreview.allowApplyStyleEvent +=new CanApplyStyleHandler(this.ctrlPreview_allowApplyStyleEvent);
            this.inputSearch.GotFocus += new EventHandler(this.inputSearch_GotFocus);
            this.inputSearch.LostFocus += new EventHandler(this.inputSearch_LostFocus);
            this.inputSearch.TextChanged += new EventHandler(this.inputSearch_TextChanged);
            this.inputSearch.KeyDown += new KeyEventHandler(this.inputSearch_KeyDown);
            if (this.btnShowAllStyles.Visible)
                this.btnShowAllStyles.Click += new EventHandler(this.btnShowAllStyles_Click);

            this.delegateReapplyStyleFinished = new DelegateReapplyStyleFinished(refreshingFinished);
            this.delegateLoadStyleFinished = new DelegateLoadStyleFinished(loadingStyleFinished);
            this.stylecontrolThread = new StyleControlThread(this);
        }

        #endregion

        #region Private Functions

        private void checkDPISetting()
        {
            Graphics g = listCitations.CreateGraphics();
            int currentDPI = (int)g.DpiX;
            this.listCitations.Font = new Font(this.listCitations.Font.FontFamily, 9 * ((float)96 / currentDPI), this.listCitations.Font.Style);
            this.splitter.SplitterDistance = Convert.ToInt32(((float)96 / currentDPI) * this.splitter.SplitterDistance);            
        }
        private void prepareLoadingStylesLabel()
        {
            this.Controls.Add(this.flowLoadingStyles);
            this.flowLoadingStyles.Location = new Point(this.listCitations.Location.X + 5, this.listCitations.Location.Y + 5);
            this.flowLoadingStyles.BringToFront();
        }
        private void buildGroupListIcons()
        {
            listGroupIcons.ColorDepth = ColorDepth.Depth32Bit;
            listGroupIcons.Images.Add("user", Properties.Resources.usericon);
            listGroupIcons.Images.Add("official", Properties.Resources.publicicon);
            listGroupIcons.Images.Add("fav", Properties.Resources.favorites);
            listCitations.GroupImageList = listGroupIcons;
        }
        private void loadStyles(bool bFiltering, string filter)
        {            
            try
            {
                this.listCitations.EmptyListMsg = "";
                if (curThread != null)
                    if (curThread.IsAlive)
                        curThread.Abort();

                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(stylecontrolThread.LoadStyles);
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["Filtering"] = bFiltering;
                if (bFiltering)
                {
                    data["FilterString"] = filter;
                    data["Styles"] = listStyleNames;
                }
                else
                    prepareLoadingStylesLabel();
                curThread = new Thread(threadStart);
                curThread.Start(data);            
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::loadStyles", ex.ToString());
            }
        }        
        private void loadAllStyles()
        {
            try
            {
                this.listCitations.EmptyListMsg = "";
                prepareLoadingStylesLabel();
                if (curThread != null)
                    if (curThread.IsAlive)
                        curThread.Abort();

                ThreadStart threadStart = new ThreadStart(stylecontrolThread.LoadAllStyles);
                curThread = new Thread(threadStart);
                curThread.Start();
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::loadAllStyles", ex.ToString());
            }
        }
        private void addToFavList(Dictionary<string, string> style)
        {
            try
            {
                JavaScriptSerializer sr = new JavaScriptSerializer();
                ListDictionary ld = null;
                try
                {
                    ld = sr.Deserialize<ListDictionary>(Properties.Settings.Default.LAST_USED_CITATIONSTYLES);
                }
                catch { }
                if (ld == null)
                    ld = new ListDictionary();
                if (style["Type"] == "fav" && ld.Contains(style["StyleName"]))
                {
                    style["Type"] = (int)ld[style["StyleName"]] == -1 ? "official" : "user";
                }
                if (ld.Contains(style["StyleName"]))
                {
                    ld.Remove(style["StyleName"]);
                }
                int iUserID = (style["Type"] == "official") ? -1 : Globals.ThisAddIn.user.UserID;
                ld.Add(style["StyleName"], iUserID);
                while (ld.Count > Properties.Settings.Default.LAST_USED_CITATIONSTYLES_NUMBER)
                {
                    IDictionaryEnumerator myEnumerator = ld.GetEnumerator();
                    if (myEnumerator.MoveNext())
                        ld.Remove(myEnumerator.Key);
                }
                Properties.Settings.Default.LAST_USED_CITATIONSTYLES = sr.Serialize(ld);
                Properties.Settings.Default.Save();
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::addToFavList", ex.ToString());
            }
        }       

        #endregion

        #region Handlers

        private void ctrlPreview_allowApplyStyleEvent(bool bAllowed)
        {
            btnOK.Enabled = bAllowed;
        }
        private void ctrlPreview_reloadAllStyleEvent(bool bReload)
        {
            if (bReload)
                loadStyles(false, string.Empty);
        }
        private void loadingStyleFinished(List<Dictionary<string, string>> data, bool bFiltered)
        {
            try
            {
                curThread.Abort();
                this.listCitations.EmptyListMsg = Lang.en_US.StyleChooser_EmptyList_Label;
                if (!bFiltered)
                {
                    listStyleNames = data;
                    this.Controls.Remove(this.flowLoadingStyles);
                }
                this.listCitations.SetObjects(data);
                this.listCitations.BuildGroups(this.olvColumnType, SortOrder.None, this.olvColumnType, SortOrder.None, this.olvColumnName, SortOrder.None);
                if (this.listCitations.Items.Count > 0)
                    this.listCitations.Items[0].Selected = true;
                listCitations.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
                if (!bFiltered)
                    listCitations.Focus();
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::loadingStyleFinished", ex.ToString());
            }
        }
        private void refreshingFinished()
        {
            progress.Close();
            Globals.ThisAddIn.Application.ActiveDocument.ActiveWindow.SetFocus();            
        }        
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCitations.SelectedItems.Count == 0)
                {
                    MessageBox.Show(Lang.en_US.StyleChooser_NoStyleSelected_Label);
                    return;
                }
                Dictionary<string, string> style = (Dictionary<string, string>)listCitations.SelectedObject;
                int iUserID = (style["Type"] == "official") ? -1 : Globals.ThisAddIn.user.UserID;
                if (style["Type"] == "fav")
                {
                    JavaScriptSerializer sr = new JavaScriptSerializer();
                    ListDictionary ld = null;
                    try
                    {
                        ld = sr.Deserialize<ListDictionary>(Properties.Settings.Default.LAST_USED_CITATIONSTYLES);
                    }
                    catch { }
                    iUserID = (int)ld[style["StyleName"]];
                }
                StyleInformation styleInfo = new StyleInformation(iUserID, style["StyleName"]);
                addToFavList(style);
                this.Close();
                Globals.ThisAddIn.Application.ActiveDocument.Activate();
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(formatter.ReappyStyleInThread);
                curThread = new Thread(threadStart);
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["Control"] = this;
                data["Style"] = styleInfo;
                curThread.Start(data);
                progress = new ProgressForm(Lang.en_US.Progress_ChangeCitationStyle_Title, string.Format(Lang.en_US.Progress_ChangeCitationStyle_Msg, styleInfo.StyleName), true);
                progress.ShowDialog();            
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::btnOK_Click", ex.ToString());
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnShowAllStyles_Click(object sender, EventArgs e)
        {
            try
            {
                this.inputSearch.TextChanged -= new EventHandler(this.inputSearch_TextChanged);
                this.inputSearch.Text = Lang.en_US.StyleChooser_InputSearch_Text;
                this.listCitations.Items.Clear();
                bFiltered = false;
                this.inputSearch.TextChanged += new EventHandler(this.inputSearch_TextChanged);
                if (bShowAll)
                {
                    tooltip.SetToolTip(this.btnShowAllStyles, Lang.en_US.StyleChooser_BtnShowAll_Tooltip);
                    loadStyles(false, string.Empty);
                }
                else
                {
                    tooltip.SetToolTip(this.btnShowAllStyles, Lang.en_US.StyleChooser_BtnShowAll_Hide_Tooltip);
                    loadAllStyles();
                }
                bShowAll = !bShowAll;
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::btnShowAllStyles_Click", ex.ToString());
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Globals.ThisAddIn.iTempHeight = this.Height;
            Globals.ThisAddIn.iTempWidth = this.Width;
            Globals.ThisAddIn.ShowMasterControl();
            Globals.ThisAddIn.ShowCustomPanel();
        }        
        private void listCitations_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //this.Controls.Remove(this.flowLoadingStyleDetails);
                if (listCitations.Items.Count == 0)
                {
                    //clear preview
                }
                else
                {
                    object ostyle = listCitations.SelectedObject;
                    if(ostyle != null)
                    {
                        Dictionary<string, string> style = (Dictionary<string, string>)ostyle;
                        StyleOwner owner = style["Type"] == "official" ? StyleOwner.Public : StyleOwner.User;
                        if (style["Type"] == "fav")
                        {
                            JavaScriptSerializer sr = new JavaScriptSerializer();
                            ListDictionary ld = null;
                            try
                            {
                                ld = sr.Deserialize<ListDictionary>(Properties.Settings.Default.LAST_USED_CITATIONSTYLES);
                            }
                            catch (Exception ex)
                            {
                                this.log.WriteLine(LogType.Error, "CitationStyleForm::listCitations_SelectedIndexChanged", ex.ToString());
                            }
                            if (ld != null)
                            {
                                if ((int)ld[style["StyleName"]] == -1)
                                    owner = StyleOwner.Public;
                                else
                                    owner = StyleOwner.User;
                            }
                        }
                        ctrlPreview.LoadStyle(owner, style["StyleName"]);
                    }                    
                }
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::listCitations_SelectedIndexChanged", ex.ToString());
            }
        }        
        private void listCitations_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Dictionary<string, string> data = (Dictionary<string, string>)e.Model;
                BrightIdeasSoftware.ImageDecoration imageDeco = new BrightIdeasSoftware.ImageDecoration();
                imageDeco.Image = listGroupIcons.Images[data["Type"]];
                imageDeco.Transparency = 255;
                imageDeco.Alignment = ContentAlignment.MiddleCenter;
                imageDeco.ShrinkToWidth = true;
                e.SubItem.Decoration = imageDeco;
            }
        }
        private void listCitations_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            if (e.RowIndex % 2 == 1)
                e.Item.Decoration = alternateRBD;
        }
        private void listCitations_CellToolTipShowing(object sender, BrightIdeasSoftware.ToolTipShowingEventArgs e)
        {
            try
            {
                Dictionary<string, string> data = (Dictionary<string, string>)e.Model;
                e.Text = String.Format("{0}", data["StyleName"]);
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::listCitations_CellToolTipShowing", ex.ToString());
            }            
        }
        private void listCitations_GotFocus(object sender, EventArgs e)
        {
            rbd.BorderPen = new Pen(Color.FromArgb(128, Color.DarkGreen), 1);
            rbd.FillBrush = new SolidBrush(Color.FromArgb(50, Color.DarkGreen));
            rbd.BoundsPadding = new Size(-2, -2);
            rbd.CornerRounding = 6.0f;
            this.listCitations.SelectedRowDecoration = rbd;
            this.listCitations.Invalidate();
            listCitations.TabIndex = 0;
            inputSearch.TabIndex = 1;
        }
        private void listCitations_LostFocus(object sender, EventArgs e)
        {
            rbd.BorderPen = new Pen(Color.FromArgb(170, Color.DarkGray), 1);
            rbd.FillBrush = new SolidBrush(Color.FromArgb(30, Color.Black));
            rbd.BoundsPadding = new Size(-2, -2);
            rbd.CornerRounding = 6.0f;
            this.listCitations.SelectedRowDecoration = rbd;
            this.listCitations.Invalidate();
        }
        private void listCitations_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (listCitations.Focused)
                {
                    if (Char.IsLetterOrDigit(e.KeyChar))
                    {
                        inputSearch.Focus();
                        inputSearch.DeselectAll();
                        inputSearch.AppendText(e.KeyChar.ToString());
                    }
                    else if (e.KeyChar == (char) Keys.Space)
                    {
                        inputSearch.Focus();
                        inputSearch.DeselectAll();
                        inputSearch.AppendText(" ");
                    }
                    else if (Convert.ToInt32(e.KeyChar) == 8)
                    {
                        inputSearch.Focus();
                        inputSearch.DeselectAll();
                        inputSearch.Text = inputSearch.Text.Remove(inputSearch.Text.Length - 1, 1);
                        inputSearch.SelectionStart = inputSearch.Text.Length;
                        inputSearch.SelectionLength = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::listCitations_KeyPress", ex.ToString());
            }
        }
        private void listCitations_KeyDown(object sender, KeyEventArgs e)
        {
            if (listCitations.Focused)
                if (e.KeyCode == Keys.Delete)
                    inputSearch.Focus();
        }
        private void inputSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strFilter = inputSearch.Text;
                if (strFilter.Length > 0 && strFilter != Lang.en_US.StyleChooser_InputSearch_Text)
                {
                    loadStyles(true, strFilter);
                    bFiltered = true;
                }
                else if (bFiltered)
                {
                    loadStyles(true, string.Empty);
                    bFiltered = false;
                }
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleForm::inputSearch_TextChanged", ex.ToString());
            }            
        }
        private void inputSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputSearch.Focused)
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                    listCitations.Focus();
        }
        private void inputSearch_GotFocus(object sender, EventArgs e)
        {
            if (inputSearch.Text == Lang.en_US.StyleChooser_InputSearch_Text)
                inputSearch.Text = "";
            else
                if (MouseButtons == MouseButtons.None)
                    this.inputSearch.SelectAll();

            inputSearch.ForeColor = System.Drawing.SystemColors.WindowText;
            listCitations.TabIndex = 1;
            inputSearch.TabIndex = 0;
        }
        private void inputSearch_LostFocus(object sender, EventArgs e)
        {
            if (inputSearch.Text == "" || inputSearch.Text.Length == 0)
            {
                inputSearch.Text = Lang.en_US.StyleChooser_InputSearch_Text;
                inputSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            else
                inputSearch.ForeColor = System.Drawing.SystemColors.MenuHighlight;
        }
        private void olvColumnType_GroupFormatter(BrightIdeasSoftware.OLVGroup group, BrightIdeasSoftware.GroupingParameters p)
        {
            p.TitleFormat = "{0} ({1})";
            p.TitleSingularFormat = "{0}";
            if (((string)group.Key) == "user")
                group.Header = Lang.en_US.StyleChooser_UserStyleTitle_Label + " (" + group.Items.Count + ")";
            else if (((string)group.Key) == "official")
            {
                if(bShowAll)
                    group.Header = Lang.en_US.StyleChooser_OfficialStyleAllTitle_Label + " (" + group.Items.Count + ")";
                else
                    group.Header = Lang.en_US.StyleChooser_OfficialStyleTitle_Label + " (" + group.Items.Count + ")";
            }
            else if (((string)group.Key) == "fav")
                group.Header = Lang.en_US.StyleChooser_FavoriteStyleTitle_Label + " (" + group.Items.Count + ")";
        }
        
        #endregion
    }
}
