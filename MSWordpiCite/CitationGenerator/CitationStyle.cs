using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSWordpiCite.Classes;
using MSWordpiCite.Entities;
using MSWordpiCite.Tools;
using System.Text.RegularExpressions;

namespace MSWordpiCite.CitationGenerator
{
    [Serializable]
    public class CitationStyle
    {
        #region Properties

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private string _Version;
        public string Version
        {
            get
            {
                return _Version;
            }
            set
            {
                _Version = value;
            }
        }

        private InTextCitation _InText;
        public InTextCitation InText
        {
            get
            {
                return _InText;
            }
            set
            {
                _InText = value;
            }
        }

        private ListReferences _Reference;
        public ListReferences Reference
        {
            get
            {
                return _Reference;
            }
            set
            {
                _Reference = value;
            }
        }

        private bool _RulesLoaded;
        public bool RulesLoaded
        {
            get
            {
                return _RulesLoaded;
            }
            set
            {
                _RulesLoaded = value;
            }
        }

        private int _OwnerUserID = -1;
        public int OwnerUserID
        {
            get
            {   
                return _OwnerUserID;
            }
            set
            {
                _OwnerUserID = value;
            }
        }

        #endregion

        public CitationStyle()
        { }

        #region Public Functions
        
        public void GenerateCitation(ReferenceCitationItem rci, bool bInHtmlFormat)
        {
            setSortOrder(ref rci);
            generateInText(ref rci, bInHtmlFormat);
            generateBibliography(ref rci, bInHtmlFormat);
        }

        #endregion

        #region Private Functions

        private void setSortOrder(ref ReferenceCitationItem rci)
        {
            List<ItemMasterRow> listRefItems = rci.MasterRefList;
            for(int i=0; i<listRefItems.Count; i++)
            {
                string year = "-1";
                if(Regex.Match(listRefItems[i].PubDate, @"(18|19|20)\d{2}").Success)
                {
                    year = Regex.Match(listRefItems[i].PubDate, @"(18|19|20)\d{2}").Value;
                }
                listRefItems[i].SortYear = int.Parse(year);
                if (listRefItems[i].PubYear.Length == 0)
                    listRefItems[i].PubYear = year;
            }

            if(this.Reference.SortOrder == SortOrders.AuthorYearTitle)
            {
                listRefItems.Sort(delegate(ItemMasterRow item1, ItemMasterRow item2)
                    {
                        string authors1 = CitationTools.GetAuthorsSortableString(item1.Authors);
                        string authors2 = CitationTools.GetAuthorsSortableString(item2.Authors);
                        if (Comparer<string>.Default.Compare(authors1, authors2) < 0)
                            return -1;
                        else if (Comparer<string>.Default.Compare(authors1, authors2) > 0)
                            return 1;
                        else
                        {
                            if (Comparer<int>.Default.Compare(item1.SortYear, item2.SortYear) < 0)
                                return -1;
                            else if (Comparer<int>.Default.Compare(item1.SortYear, item2.SortYear) > 0)
                                return 1;
                            else
                            {
                                if (Comparer<string>.Default.Compare(item1.Title, item2.Title) < 0)
                                    return -1;
                                else if (Comparer<string>.Default.Compare(item1.Title, item2.Title) > 0)
                                    return 1;
                                else
                                    return 0;
                            }
                        }
                    });
            }
            else if(this.Reference.SortOrder == SortOrders.AuthorTitle)
            {
                listRefItems.Sort(delegate(ItemMasterRow item1, ItemMasterRow item2)
                    {
                        string authors1 = CitationTools.GetAuthorsSortableString(item1.Authors);
                        string authors2 = CitationTools.GetAuthorsSortableString(item2.Authors);
                        if (Comparer<string>.Default.Compare(authors1, authors2) < 0)
                            return -1;
                        else if (Comparer<string>.Default.Compare(authors1, authors2) > 0)
                            return 1;
                        else
                        {

                            if (Comparer<string>.Default.Compare(item1.Title, item2.Title) < 0)
                                return -1;
                            else if (Comparer<string>.Default.Compare(item1.Title, item2.Title) > 0)
                                return 1;
                            else
                                return 0;
                        }
                    });
            }
            for (int i = 0; i < listRefItems.Count; i++)
                listRefItems[i].SequenceNo = i + 1;
        }
        private void generateInText(ref ReferenceCitationItem rci, bool bInHtmlFormat)
        {
            for (int i = 0; i < rci.TextCitations.Count; i++)
            {
                List<ItemMasterRow> items = rci.TextCitations[i].Items;
                if (this.InText.UseNumberRange && this.InText.Template.Contains("Number"))
                {
                    items.Sort(delegate(ItemMasterRow item1, ItemMasterRow item2)
                    {
                        return Comparer<int>.Default.Compare(item1.SequenceNo, item2.SequenceNo);
                    });
                }
                bool bSkip = false;
                for (int j = 0; j < items.Count; j++)
                {
                    string text = this.InText.Format(items[j], bInHtmlFormat, this.InText.Superscript);
                    if (this.InText.UseNumberRange && this.InText.Template.Contains("Number"))
                    {
                        if (items.Count > 1)
                        {
                            if (j < items.Count - 1)
                            {
                                if (j > 0)
                                {
                                    if (items[j].SequenceNo == items[j - 1].SequenceNo + 1)
                                    {
                                        if (items[j].SequenceNo + 1 == items[j + 1].SequenceNo)
                                        {
                                            bSkip = true;
                                            text = "";
                                        }
                                        else
                                        {
                                            if (bSkip)
                                                text = CitationTools.GetPlainFormat("-", bInHtmlFormat, this.InText.Superscript) + text;
                                            else
                                                text = CitationTools.GetPlainFormat(this.InText.Separator, bInHtmlFormat, this.InText.Superscript) + text;
                                        }
                                    }
                                }

                                if (text.Length > 0 && items[j].SequenceNo + 1 != items[j + 1].SequenceNo)
                                    text += CitationTools.GetPlainFormat(this.InText.Separator, bInHtmlFormat, this.InText.Superscript);
                            }
                            else
                            {
                                if (items[j].SequenceNo == items[j - 1].SequenceNo + 1)
                                {
                                    if (bSkip)
                                        text = CitationTools.GetPlainFormat("-", bInHtmlFormat, this.InText.Superscript) + text;
                                    else
                                        text = CitationTools.GetPlainFormat(this.InText.Separator, bInHtmlFormat, this.InText.Superscript) + text;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (j < items.Count - 1)
                            text += CitationTools.GetPlainFormat(this.InText.Separator, bInHtmlFormat, this.InText.Superscript);
                    }

                    if (text.Length > 0)
                    {
                        if (this.InText.Enclosure != EnclosureType.None)
                        {
                            string enclosure = CitationTools.GetEnclosure(this.InText.Enclosure);
                            if (j == 0)
                                text = CitationTools.GetPlainFormat(enclosure.Substring(0, 1), bInHtmlFormat, this.InText.Superscript) + text;
                            if (j == items.Count - 1)
                                text += CitationTools.GetPlainFormat(enclosure.Substring(enclosure.Length - 1, 1), bInHtmlFormat, this.InText.Superscript);
                        }
                    }
                    rci.TextCitations[i].FormatString[j] = text;
                }
            }            
        }
        private void generateBibliography(ref ReferenceCitationItem rci, bool bInHtmlFormat)
        {
            try
            {
                Dictionary<string, CitationFormatter> formatter = this.Reference.ItemTypes;
                string text = string.Empty;

                if (this.Reference.Header == null)
                    this.Reference.Header = "References";

                if (bInHtmlFormat)
                    text = CitationTools.GetHtmlFormatString(this.Reference.Header, TextFormat.Bold);

                for (int i = 0; i < this.Reference.LineSpacing+1; i++)
                {
                    if (bInHtmlFormat)
                        text += CitationTools.GetHtmlFormatString("", TextFormat.LineBreak);
                    else
                        text += CitationTools.GetWordMLFormatString(CitationTools.GetWordMLFormatString(" ", TextFormat.None), TextFormat.Paragraph);
                }
                for (int i = 0; i < rci.MasterRefList.Count; i++)
                {
                    if(rci.IsAPA)
                        text += CitationTools.GetWordMLFormatString(this.Reference.Format(rci.MasterRefList[i], bInHtmlFormat) + " ", TextFormat.ParagraphWithIndent);
                    else
                        text += CitationTools.GetWordMLFormatString(this.Reference.Format(rci.MasterRefList[i], bInHtmlFormat) + " ", TextFormat.Paragraph);
                }

                rci.FormatString = text;
            }
            catch
            {}
        }

        #endregion
    }
}
