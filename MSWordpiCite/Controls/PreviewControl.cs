using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MSWordpiCite.Entities;
using MSWordpiCite.Enums;
using MSWordpiCite.Tools;
using System.Runtime.InteropServices;

namespace MSWordpiCite.Controls
{       

    public partial class PreviewControl : UserControl
    {
         private const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        private const int SET_FEATURE_ON_THREAD = 0x00000001;
        private const int SET_FEATURE_ON_PROCESS = 0x00000002;
        private const int SET_FEATURE_IN_REGISTRY = 0x00000004;
        private const int SET_FEATURE_ON_THREAD_LOCALMACHINE = 0x00000008;
        private const int SET_FEATURE_ON_THREAD_INTRANET = 0x00000010;
        private const int SET_FEATURE_ON_THREAD_TRUSTED = 0x00000020;
        private const int SET_FEATURE_ON_THREAD_INTERNET = 0x00000040;
        private const int SET_FEATURE_ON_THREAD_RESTRICTED = 0x00000080;

        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return:MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(int FeatureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags, bool fEnable);


        #region Variables

        private ItemMasterRow currentItem;
        public MasterControl Master;

        #endregion

        public PreviewControl()
        {
            InitializeComponent();
        }

        #region Initialization

        private void PreviewControl_Load(object sender, EventArgs e)
        {
            initializeLang();
            panel.Visible = true;
        }
        private void initializeLang()
        {
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.Resize += new EventHandler(this.PreviewControl_Resize);
            try
            {
                int feature = FEATURE_DISABLE_NAVIGATION_SOUNDS;
                CoInternetSetFeatureEnabled(feature, SET_FEATURE_ON_PROCESS, true);
            }
            catch{}            
        }

        #endregion

        #region Public Functions

        public void SetPreview(ItemMasterRow item)
        {
            currentItem = item;
            webBrowser.DocumentText = prepareHTML();
        }
        public void ClearPreview()
        {
            string textNoItems = "<html><head><title>{0}</title></head><body style='overflow:auto;'><center><p style='font-family:Tahoma;text-align:center;vertical-align:middle;font-size:11px;color:#555555;'>{1}</p></center></body></html>";
            webBrowser.DocumentText = string.Format(textNoItems, Lang.en_US.Preview_NoItem_Label, Lang.en_US.Preview_NoItem_Label);
        }

        #endregion

        #region Private Functions

        private string prepareHTML()
        {
            string html = "<html><head><title>{0}</title></head><body style='font-family:Tahoma;font-size:12px;line-height:1.5em;overflow:auto;scrollbar-3dlight-color:#919191;scrollbar-arrow-color:#919191;scrollbar-track-color:#E8E8E8;scrollbar-darkshadow-color:#919191;scrollbar-face-color:white;scrollbar-highlight-color:#919191;scrollbar-shadow-color:#919191;padding-top:0px;'><p style='color:#555555;margin-bottom:6px;margin-top:0px;'>{1}</p><p style='font-size:13px;margin-top:0px;margin-bottom:6px;'><b>{2}</b></p><p style='color:#555555;margin-bottom:6px;margin-top:0px;font-size:11px;'>{3}</p>{4}{5}</body></html>";
            html = string.Format(html, prepareTitle(), prepareBibliography(), prepareTitle(), prepareAuthorsString(), prepareNotes(), prepareAbstract());
            return html;
        }
        private string prepareNotes()
        {
            string notes = string.Empty;
            if ((Regex.Replace(currentItem.Notes, @"\s+", "")).Length > 0)
                notes = "<p style='font-family:Georgia,Serif;font-style:italic;margin-top:0px;margin-bottom:6px;padding:7px;background-color:#FEFCE7;border:solid 1px #E1DDA4;color:#555555;'>" + Regex.Replace(currentItem.Notes, @"<.*?>|(\\n)*$", "") + "</p>";
            return notes;
        }
        private string prepareTitle()
        {
            string title = currentItem.Title;
            if (currentItem.ItemTypeID == ItemTypes.Patent)
            {
                if (currentItem.ID1.Length > 0)
                    title += " (" + currentItem.ID1 + ")";
                else if (currentItem.ID2.Length > 0)
                    title += " (" + currentItem.ID2 + ")";
            }
            return title;
        }
        private string prepareAuthorsString()
        {
            return CitationTools.GetAuthorsString(currentItem.Authors);
        }
        private string prepareBibliography()
        {
            string strBibliography = string.Empty;
            bool bHasTextBefore = false;
            if (currentItem.Title2.Length > 0 && currentItem.Title2 != string.Empty)
            {
                strBibliography += currentItem.Title2 + " ";
                bHasTextBefore = true;
            }
            if (currentItem.ItemTypeID != ItemTypes.Thesis)
            {
                if (currentItem.Volume.Length > 0 && currentItem.Volume != string.Empty)
                {
                    strBibliography += currentItem.Volume + " ";
                    bHasTextBefore = true;
                }
                if (currentItem.Volume2.Length > 0 && currentItem.Volume2 != string.Empty)
                {
                    strBibliography += "(" + currentItem.Volume2 + ") ";
                    bHasTextBefore = true;
                }
            }
            if (currentItem.Pages.Length > 0 && currentItem.Pages != string.Empty)
            {
                strBibliography += (bHasTextBefore ? ": " : "");
                strBibliography += (currentItem.ItemTypeID == ItemTypes.BookWhole ? "pp. " : "p ") + currentItem.Pages;
                bHasTextBefore = true;
            }
            if (currentItem.PubYear.Length > 0 && currentItem.PubYear != string.Empty)
                strBibliography += (bHasTextBefore ? ", " : "") + currentItem.PubYear;
            else if (currentItem.PubDate.Length > 0 && currentItem.PubDate != string.Empty)
            {
                if (Regex.Match(currentItem.PubDate, @"\d{4}").Success)
                    strBibliography += (bHasTextBefore ? ", " : "") + Regex.Match(currentItem.PubDate, @"\d{4}").Value;
                else
                    strBibliography += (bHasTextBefore ? ", " : "") + currentItem.PubDate;
            }

            return strBibliography;
        }
        private string prepareAbstract()
        {
            string strAbstract = string.Empty;
            strAbstract = Regex.Replace(currentItem.Abstract, @"\s+", " ");
            strAbstract = Regex.Replace(strAbstract, @"^\s+|^\t+|<.*?>", "");
            return ((strAbstract.Length > 0 ? "<b>Abstract</b><br/>" : "") + strAbstract);
        }

        #endregion

        #region Event Handlers

        private void PreviewControl_Resize(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width - this.Margin.Left - this.Margin.Right;
            this.Height = this.Parent.Height - this.Margin.Top - this.Margin.Bottom;
        }

        #endregion
    }
}
