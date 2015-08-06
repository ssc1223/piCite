using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WizFolioUtils;

namespace MSWordWizCite.Controls
{
    public partial class CitationStyleControl : UserControl
    {
        private Logger log;
        private BrightIdeasSoftware.RowBorderDecoration alternateRBD;
        private BrightIdeasSoftware.RowBorderDecoration rbd;
        private bool bSearchFocused = false;
        private bool bFiltered = false;
        private bool bShowAll = false;
        List<Dictionary<string, string>> listStyleNames;

        public CitationStyleControl()
        {
            InitializeComponent();
            this.log = Globals.ThisAddIn.log;
        }

        #region Initialization

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        private void initializeUI()
        {
            try
            {
                this.Width = Globals.ThisAddIn.iTempWidth;
                this.Height = Globals.ThisAddIn.iTempHeight;

                BrightIdeasSoftware.TextOverlay textOverlay = this.listCitations.EmptyListMsgOverlay as BrightIdeasSoftware.TextOverlay;
                textOverlay.TextColor = Color.FromArgb(255, 96, 96, 96);
                textOverlay.BackColor = System.Drawing.Color.White;
                textOverlay.BorderWidth = 0;
                textOverlay.InsetY = 0;
                textOverlay.Font = new System.Drawing.Font("Segoe UI", 9);

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

                if (Globals.ThisAddIn.user.AccType == WizFolioUtils.EnumList.AccountType.Free)
                    btnShowAllStyles.Visible = false;

                splitter.SplitterDistance = Convert.ToInt32(0.5 * splitter.Height);
                ThisAddIn_DockPositionChanged(null, null);
                buildGroupListIcons();
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleControl::initializeUI", ex.ToString());
            }
        }

        /// <summary>
        /// Builds the group list icons.
        /// </summary>
        private void buildGroupListIcons()
        {
            listGroupIcons.ColorDepth = ColorDepth.Depth32Bit;
            listGroupIcons.Images.Add("user", Properties.Resources.usericon);
            listGroupIcons.Images.Add("official", Properties.Resources.publicicon);
            listCitations.GroupImageList = listGroupIcons;
        }

        /// <summary>
        /// Initializes the lang.
        /// </summary>
        private void initializeLang()
        {
            lblTitle.Text = Lang.en_US.StyleChooser_Title_Label;
            listCitations.EmptyListMsg = Lang.en_US.StyleChooser_EmptyList_Label;
            tooltip.SetToolTip(this.btnClose, Lang.en_US.StyleChooser_BtnClose_Tooltip);
            tooltip.SetToolTip(this.btnShowAllStyles, Lang.en_US.StyleChooser_BtnShowAll_Tooltip);
            tooltip.SetToolTip(this.btnReload, Lang.en_US.StyleChooser_BtnReload_Tooltip);
            inputSearch.Text = Lang.en_US.StyleChooser_InputSearch_Text;
        }

        /// <summary>
        /// Initializes the handlers.
        /// </summary>
        private void initializeHandlers()
        {
            this.Resize += new EventHandler(this.CitationStyleControl_Resize);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);            
            this.listCitations.FormatCell +=new EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.listCitations_FormatCell);
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

            this.inputSearch.GotFocus += new EventHandler(this.inputSearch_GotFocus);
            this.inputSearch.LostFocus += new EventHandler(this.inputSearch_LostFocus);
            this.inputSearch.TextChanged +=new EventHandler(this.inputSearch_TextChanged);
            this.inputSearch.KeyDown += new KeyEventHandler(this.inputSearch_KeyDown);
            if(this.btnShowAllStyles.Visible)
                this.btnShowAllStyles.Click += new EventHandler(this.btnShowAllStyles_Click);

            Globals.ThisAddIn.DockPositionChanged += new EventHandler(ThisAddIn_DockPositionChanged);
        }

        /// <summary>
        /// Loads the user citation styles.
        /// </summary>
        private void loadStyles(bool bFiltering, string filter)
        {
            if(bFiltering)
            {
                List<Dictionary<string, string>> listStyles = new List<Dictionary<string, string>>();
                foreach(Dictionary<string, string> style in listStyleNames)
                {
                    filter = Regex.Replace(filter, @"[^\w\d\s]", "");
                    string[] terms = Regex.Split(filter, @"\s+");
                    if (stringValidated(style["StyleName"], terms))
                    {
                        listStyles.Add(style);
                    }
                }
                this.listCitations.SetObjects(listStyles);
            }
            else
            {
                List<string> listUserStyleNames = Globals.ThisAddIn.user.LoadUserCitationStyleNames();
                List<string> listOfficialStyleNames = Globals.ThisAddIn.user.LoadOfficialStyleNames();
                listStyleNames = new List<Dictionary<string, string>>();

                foreach (string style in listUserStyleNames)
                {
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    Data["Type"] = "user";
                    Data["StyleName"] = style;
                    listStyleNames.Add(Data);
                }
                foreach (string style in listOfficialStyleNames)
                {
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    Data["Type"] = "official";
                    Data["StyleName"] = style;
                    listStyleNames.Add(Data);
                }
                this.listCitations.SetObjects(listStyleNames);
            }                        
            this.listCitations.BuildGroups(this.olvColumnType, SortOrder.None, this.olvColumnType, SortOrder.None, this.olvColumnName, SortOrder.None);
            if (this.listCitations.Items.Count > 0)
            {
                this.listCitations.Items[0].Selected = true;
            }
            listCitations.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
        }

        /// <summary>
        /// Strings the validated.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="terms">The terms.</param>
        /// <returns></returns>
        private bool stringValidated(string text, string[] terms)
        {
            foreach (string term in terms)
            {
                if (!Regex.Match(text, term, RegexOptions.IgnoreCase).Success)
                    return false;
            }
            return true;
        }

        #endregion

        #region Handlers

        private void listCitations_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listCitations.Items.Count == 0)
                {
                    //clear preview
                }
                else
                {
                    Dictionary<string, string> style = (Dictionary<string, string>)listCitations.GetSelectedObject();
                    ctrlPreview.LoadStyle(style["Type"], style["StyleName"]);
                }
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleControl::listCitations_SelectedIndexChanged", ex.ToString());
            }
        }

        /// <summary>
        /// Button the show all styles_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void btnShowAllStyles_Click(object sender, EventArgs e)
        {
            if (bShowAll)
            {
                tooltip.SetToolTip(this.btnShowAllStyles, Lang.en_US.StyleChooser_BtnShowAll_Tooltip);
                loadStyles(false, string.Empty);
            }
            else
            {
                tooltip.SetToolTip(this.btnShowAllStyles, Lang.en_US.StyleChooser_BtnShowAll_Hide_Tooltip);
                List<string> listUserStyleNames = Globals.ThisAddIn.user.LoadUserCitationStyleNames();
                List<string> listAllOfficialStyleNames = Globals.ThisAddIn.user.LoadAllOfficialStyleNames();
                listStyleNames = new List<Dictionary<string, string>>();
                foreach (string style in listUserStyleNames)
                {
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    Data["Type"] = "user";
                    Data["StyleName"] = style;
                    listStyleNames.Add(Data);
                }
                foreach (string style in listAllOfficialStyleNames)
                {
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    Data["Type"] = "official";
                    Data["StyleName"] = style;
                    listStyleNames.Add(Data);
                }                                
            }
            bFiltered = true;
            inputSearch_TextChanged(null, null);
            listCitations.Focus();
            bShowAll = !bShowAll;
        }

        /// <summary>
        /// Handles the FormatCell event of the listCitations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BrightIdeasSoftware.FormatCellEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the FormatRow event of the listCitations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BrightIdeasSoftware.FormatRowEventArgs"/> instance containing the event data.</param>
        private void listCitations_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            if (e.RowIndex % 2 == 1)
            {
                e.Item.Decoration = alternateRBD;
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the listCitations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the LostFocus event of the listCitations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void listCitations_LostFocus(object sender, EventArgs e)
        {
            rbd.BorderPen = new Pen(Color.FromArgb(170, Color.DarkGray), 1);
            rbd.FillBrush = new SolidBrush(Color.FromArgb(30, Color.Black));
            rbd.BoundsPadding = new Size(-2, -2);
            rbd.CornerRounding = 6.0f;
            this.listCitations.SelectedRowDecoration = rbd;
            this.listCitations.Invalidate();
        }

        /// <summary>
        /// Handles the KeyPress event of the listCitations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
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
                    else if (Char.IsWhiteSpace(e.KeyChar))
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
            catch
            {

            }
        }

        /// <summary>
        /// Handles the KeyDown event of the listCitations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void listCitations_KeyDown(object sender, KeyEventArgs e)
        {
            if (listCitations.Focused)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    inputSearch.Focus();
                }
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the inputSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void inputSearch_TextChanged(object sender, EventArgs e)
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

        /// <summary>
        /// Handles the KeyDown event of the inputSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void inputSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputSearch.Focused)
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                    listCitations.Focus();
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the inputSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void inputSearch_GotFocus(object sender, EventArgs e)
        {
            if (inputSearch.Text == Lang.en_US.StyleChooser_InputSearch_Text)
            {
                inputSearch.Text = "";
            }
            else
            {
                if (MouseButtons == MouseButtons.None)
                {
                    this.inputSearch.SelectAll();
                    bSearchFocused = true;
                }
            }
            inputSearch.ForeColor = System.Drawing.SystemColors.WindowText;
            listCitations.TabIndex = 1;
            inputSearch.TabIndex = 0;
        }

        /// <summary>
        /// Handles the LostFocus event of the inputSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void inputSearch_LostFocus(object sender, EventArgs e)
        {
            if (inputSearch.Text == "" || inputSearch.Text.Length == 0)
            {
                inputSearch.Text = Lang.en_US.StyleChooser_InputSearch_Text;
                inputSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            else
            {
                inputSearch.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            }
        }
        
        /// <summary>
        /// Olvs the column type_ group formatter.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="p">The p.</param>
        private void olvColumnType_GroupFormatter(BrightIdeasSoftware.OLVGroup group, BrightIdeasSoftware.GroupingParameters p)
        {
            p.TitleFormat = "{0} ({1})";
            p.TitleSingularFormat = "{0}";
            if (((string)group.Key) == "user")
                group.Header = Lang.en_US.StyleChooser_UserStyleTitle_Label + " (" + group.Items.Count + ")";
            else if (((string)group.Key) == "official")
                group.Header = Lang.en_US.StyleChooser_OfficialStyleTitle_Label + " (" + group.Items.Count + ")";
        }
                
        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Globals.ThisAddIn.iTempHeight = this.Height;
            Globals.ThisAddIn.iTempWidth = this.Width;
            Globals.ThisAddIn.ShowMasterControl();
            Globals.ThisAddIn.ShowCustomPanel();
        }

        /// <summary>
        /// Handles the DockPositionChanged event of the ThisAddIn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ThisAddIn_DockPositionChanged(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Office.Core.MsoCTPDockPosition DockPosition = Globals.ThisAddIn.GetCurrentDockPosition();
                switch (DockPosition)
                {
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionBottom:
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionTop:
                        splitter.Orientation = Orientation.Vertical;
                        splitter.SplitterDistance = Convert.ToInt32(0.3 * splitter.Width);
                        break;
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionLeft:
                    case Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight:
                        splitter.Orientation = Orientation.Horizontal;
                        splitter.SplitterDistance = Convert.ToInt32(0.5 * splitter.Height);
                        this.MinimumSize = new System.Drawing.Size(265, 0);
                        break;
                    default:
                        splitter.Orientation = Orientation.Horizontal;
                        splitter.SplitterDistance = Convert.ToInt32(0.5 * splitter.Height);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleControl::ThisAddIn_DockPositionChanged", ex.ToString());
            }
        }

        /// <summary>
        /// Handles the Resize event of the CitationStyleControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CitationStyleControl_Resize(object sender, EventArgs e)
        {
            try
            {
                Globals.ThisAddIn.iTempWidth = Globals.ThisAddIn.GetCurrentCustomPaneWidth();
                Globals.ThisAddIn.iTempHeight = Globals.ThisAddIn.GetCurrentCustomPaneHeight();
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationStyleControl::CitationStyleControl_Resize", ex.ToString());
            }
        }

        /// <summary>
        /// Handles the Load event of the CitationStyleControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CitationStyleControl_Load(object sender, EventArgs e)
        {
            initializeUI();
            initializeLang();
            initializeHandlers();
            loadStyles(false, string.Empty);
        }

        #endregion
    }
}
