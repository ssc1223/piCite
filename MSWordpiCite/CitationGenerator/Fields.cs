using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MSWordpiCite.Classes;
using MSWordpiCite.Entities;
using MSWordpiCite.Tools;
using MSWordpiCite.Enums;
using Word = Microsoft.Office.Interop.Word;

namespace MSWordpiCite.CitationGenerator
{
    public class BaseClass
    {
        #region Properties

        private string _Prefix;
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
            set
            {
                _Prefix = value;
            }
        }

        private string _Suffix;
        public string Suffix
        {
            get
            {
                return _Suffix;
            }
            set
            {
                _Suffix = value;
            }
        }

        private bool _Bold;
        public bool Bold
        {
            get
            {
                return _Bold;
            }
            set
            {
                _Bold = value;
            }
        }

        private bool _Italic;
        public bool Italic
        {
            get
            {
                return _Italic;
            }
            set
            {
                _Italic = value;
            }
        }

        private bool _Underline;
        public bool Underline
        {
            get
            {
                return _Underline;
            }
            set
            {
                _Underline = value;
            }
        }
        
        #endregion

        public void Initialize()
        {
            Prefix = "";
            Suffix = "";
            Bold = false;
            Italic = false;
            Underline = false;
        }
    }

    #region Contributors

    public class Author : BaseClass
    {
        #region Properties

        private AuthorFormat _FirstAuthor;
        public AuthorFormat FirstAuthor
        {
            get
            {
                return _FirstAuthor;
            }
            set
            {
                _FirstAuthor = value;
            }
        }

        private AuthorFormat _OtherAuthors;
        public AuthorFormat OtherAuthors
        {
            get
            {
                return _OtherAuthors;
            }
            set
            {
                _OtherAuthors = value;
            }
        }

        private AuthorDelimitor _BetweenAuthors;
        public AuthorDelimitor BetweenAuthors
        {
            get
            {
                return _BetweenAuthors;
            }
            set
            {
                _BetweenAuthors = value;
            }
        }

        private string _BeforeLast;
        public string BeforeLast
        {
            get
            {
                return _BeforeLast;
            }
            set
            {
                _BeforeLast = value;
            }
        }

        private Capitalization _Capitalization;
        public Capitalization Capitalization
        {
            get
            {
                return _Capitalization;
            }
            set
            {
                _Capitalization = value;
            }
        }

        private int _MaxAuthors;
        public int MaxAuthors
        {
            get
            {
                return _MaxAuthors;
            }
            set
            {
                _MaxAuthors = value;
            }
        }

        private int _ListAuthors;
        public int ListAuthors
        {
            get
            {
                return _ListAuthors;
            }
            set
            {
                _ListAuthors = value;
            }
        }

        private string _FollowedBy;
        public string FollowedBy
        {
            get
            {
                return _FollowedBy;
            }
            set
            {
                _FollowedBy = value;
            }
        }

        private bool _FollowedBy_Italic;
        public bool FollowedBy_Italic
        {
            get
            {
                return _FollowedBy_Italic;
            }
            set
            {
                _FollowedBy_Italic = value;
            }
        }

        #endregion

        public Author()
        {
            FirstAuthor = AuthorFormat.FirstNameFirstMidNameInitialWithDot;
            OtherAuthors = AuthorFormat.FirstNameFirstMidNameInitialWithDot;
            BetweenAuthors = AuthorDelimitor.Comma;
            BeforeLast = ", & ";
            Capitalization = Capitalization.AsIs;
            MaxAuthors = 1;
            ListAuthors = 1;
            FollowedBy = " et al";
            FollowedBy_Italic = false;

            Prefix = "";
            Suffix = "";
            Bold = false;
            Italic = false;
            Underline = false;
        }
        public virtual string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            List<NameMasterRow> authors = item.Authors;
            string text = string.Empty;

            if (Tools.CitationTools.CountAuthor(authors, NameTypes.Author) > 0)
                text = Tools.CitationTools.FormatAuthor(authors, this, NameTypes.Author, bInHtmlFormat);

            return text;
        }
    }

    public class Editor : Author
    {
        public Editor()
        {           
            Prefix = "";
            Suffix = ", editor.";
            Bold = false;
            Italic = false;
            Underline = false;
        }
        public override string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            List<NameMasterRow> authors = item.Authors;
            string text = string.Empty;
            if (Tools.CitationTools.CountAuthor(authors, NameTypes.Editor) > 0)
                text = Tools.CitationTools.FormatAuthor(authors, this, NameTypes.Editor, bInHtmlFormat);
            
            return text;
        }
    }

    public class Publisher : BaseClass
    {
        public Publisher()
        {            
            Prefix = "";
            Suffix = ": ";
            Bold = false;
            Italic = false;
            Underline = false;
        }
        public virtual string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            List<NameMasterRow> authors = item.Authors;
            string text = string.Empty;
            if (Tools.CitationTools.CountAuthor(authors, NameTypes.Publisher) > 0)
            {
                text = Tools.CitationTools.FormatPublisher(authors, this, NameTypes.Publisher, bInHtmlFormat);
            }
            return text;
        }
    }

    public class Inventor : Author
    {
        public Inventor()
        {
            FirstAuthor = AuthorFormat.FirstNameFirstMidNameInitialWithDot;
            OtherAuthors = AuthorFormat.FirstNameFirstMidNameInitialWithDot;
            BetweenAuthors = AuthorDelimitor.Comma;
            BeforeLast = "and ";
            Capitalization = Capitalization.AsIs;
            MaxAuthors = 6;
            ListAuthors = 6;
            FollowedBy = " et al.";
            FollowedBy_Italic = false;

            Prefix = "";
            Suffix = ": ";
            Bold = false;
            Italic = false;
            Underline = false;
        }
        public override string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            List<NameMasterRow> authors = item.Authors;
            string text = string.Empty;
            if (Tools.CitationTools.CountAuthor(authors, NameTypes.Author) > 0)
            {
                text = Tools.CitationTools.FormatAuthor(authors, this, NameTypes.Author, bInHtmlFormat);
            }
            return text;
        }
    }

    public class Assignee : Author
    {
        public Assignee()
        {
            Prefix = "";
            Suffix = ", assignee";
            Bold = false;
            Italic = false;
            Underline = false;
        }
        public override string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            List<NameMasterRow> authors = item.Authors;
            string text = string.Empty;
            if (Tools.CitationTools.CountAuthor(authors, NameTypes.Publisher) > 0)
            {
                text = Tools.CitationTools.FormatAuthor(authors, this, NameTypes.Publisher, bInHtmlFormat);
            }
            return text;
        }
    }

    #endregion

    #region Date

    public class DateClass : BaseClass
    {
        private YearFormats _YearFormat;
        public YearFormats YearFormat
        {
            get
            {
                return _YearFormat;
            }
            set
            {
                _YearFormat = value;
            }
        }        
    }

    public class ConfDate : IssueDate
    {
        public ConfDate()
        {
            Initialize();
            Prefix = "";
            Suffix = "; ";
            YearFormat = YearFormats.FourDigits;
        }
    }

    public class SubmitDate : PubDate
    {
        public SubmitDate()
        {
            Initialize();
            Prefix = " ";
            Suffix = ",";
            YearFormat = YearFormats.FourDigits;
        }
    }

    public class DateFiled : DateClass
    {
        public DateFiled()
        {
            Initialize();
            Prefix = " ";
            Suffix = ",";
            YearFormat = YearFormats.FourDigits;
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            return CitationTools.FormateDate(item.Date3, this, bInHtmlFormat);
        }
    }

    public class PubDate : DateClass
    {
        public PubDate()
        {
            Initialize();
            Prefix = "";
            Suffix = ".";
            YearFormat = YearFormats.FourDigits;
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            return CitationTools.FormateDate(item.PubDate, this, bInHtmlFormat);
        }
    }

    public class IssueDate : DateClass
    {
        public IssueDate()
        {
            Initialize();
            Prefix = "";
            Suffix = ".";
            YearFormat = YearFormats.FourDigits;
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            return CitationTools.FormateDate(item.IssueDate, this, bInHtmlFormat);
        }
    }

    public class AccessDate : DateClass
    {
        public AccessDate()
        {
            Initialize();
            Prefix = " ";
            Suffix = ".";
            YearFormat = YearFormats.AsIs;
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            return CitationTools.FormateFullDate(item.IssueDate, this, bInHtmlFormat);
        }
    }

    public class Year : DateClass
    {
        public Year()
        {
            Initialize();
            Prefix = "";
            Suffix = ";";
            YearFormat = YearFormats.FourDigits;
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            string text = CitationTools.FormateDate(item.PubDate, this, bInHtmlFormat);
            if(item.ItemTypeID == ItemTypes.Patent)
            {
                if (text.Length == 0)
                    text = CitationTools.FormateDate(item.IssueDate, this, bInHtmlFormat);
                if (text.Length == 0)
                    text = CitationTools.FormateDate(item.Date3, this, bInHtmlFormat);
            }
            return text;
        }
    }

    #endregion

    #region Other fields

    public class Number : BaseClass
    {
        public Number()
        {
            Prefix = "";
            Suffix = ". ";
            Bold = false;
            Italic = false;
            Underline = false;
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            return CitationTools.ApplyStandardFormat(this, item.SequenceNo + "", bInHtmlFormat);
        }
    }

    public class Pages : BaseClass
    {
        private PageNumberFormat _PageFormat;
        public PageNumberFormat PageFormat
        {
            get
            {
                return _PageFormat;
            }
            set
            {
                _PageFormat = value;
            }
        }

        public Pages()
        {
            Prefix = ":";
            Suffix = ".";
            Bold = false;
            Italic = false;
            Underline = false;
            PageFormat = PageNumberFormat.AbbrLastPageOneDigit;
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            string pages = item.Pages;
            bool bIsJournal = item.ItemTypeID == ItemTypes.JournalArticle;
            string text = string.Empty;
            if (Regex.Replace(item.Pages, @"\s+", "").Length == 0)
                return text;            
            string[] pageTokens = Regex.Split(pages, @"\s*[" + Convert.ToChar(8211).ToString() + "|" + Convert.ToChar(45).ToString() + @"]\s*");            
            string startpage = string.Empty;
            string endpage = string.Empty;
            if (pageTokens.Length > 0)
                startpage = Regex.Replace(pageTokens[0], @"^\s+|\s+$", "");
            if (pageTokens.Length > 1)
            {
                endpage = Regex.Replace(pageTokens[1], @"^\s+|\s+$", "");
                if (endpage.Length < startpage.Length)
                    endpage = startpage.Substring(0, startpage.Length - endpage.Length) + endpage;
            }
            switch (this.PageFormat)
            {
                case PageNumberFormat.AsIs:
                default:
                    text = pages;
                    break;
                case PageNumberFormat.FirstPageOnly:
                    text = startpage;
                    break;
                case PageNumberFormat.LastPageOnly:
                    text = endpage;
                    break;
                case PageNumberFormat.Full:
                    text = startpage;
                    if (endpage.Length > 0)
                        text += "-" + endpage;
                    break;
                case PageNumberFormat.AbbrLastPageOneDigit:
                    for (int i = 0; i < startpage.Length && endpage.Length > 0; i++)
                    {
                        if (startpage.Substring(i, 1) == endpage.Substring(0, 1))
                            endpage = endpage.Substring(1);
                        else
                            break;
                    }
                    text = startpage;
                    if (endpage.Length > 0)
                        text += '-' + endpage;
                    break;
                case PageNumberFormat.AbbrLastPageTwoDigits:
                    for (int i = 0; i < startpage.Length && endpage.Length > 0; i++)
                    {
                        if (startpage.Substring(i, 1) == endpage.Substring(0, 1) && endpage.Length > 2)
                            endpage = endpage.Substring(1);
                        else
                            break;
                    }
                    text = startpage;
                    if (endpage.Length > 0)
                        text += '-' + endpage;
                    break;
                case PageNumberFormat.FirstPageOnlyForJournals:
                    text = startpage;
                    if (!bIsJournal)
                    {
                        if (endpage.Length > 0)
                            text += '-' + endpage;
                    }
                    break;
            }
            text = CitationTools.ApplyStandardFormat(this, text, bInHtmlFormat);
            return text;
        }
    }

    public class Title : BaseClass
    {
        private Capitalization _Capitalization;
        public Capitalization Capitalization
        {
            get
            {
                return _Capitalization;
            }
            set
            {
                _Capitalization = value;
            }
        }

        public Title()
        {
            Prefix = "";
            Suffix = ". ";
            Bold = true;
            Italic = false;
            Underline = false;
            Capitalization = Capitalization.AsIs;
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Title, @"\s+", "").Length == 0)
                return string.Empty;
            string text = string.Empty;
            switch(this.Capitalization)
            {
                case Capitalization.AsIs:
                    text = item.Title;
                    break;
                case Capitalization.FirstIsCapital:
                    string[] terms = Regex.Split(item.Title.ToLower(), @"\s+");
                    string[] stopwords = {"a", "an", "and", "as", "at", "but", "by", "for", "from", "in", "into", "nor", "of", "on", "or", "over", "per", "the", "to", "upon", "vs.", "with", "gi"};
                    bool bTextBefore = false;
                    for (int i = 0; i < terms.Length; i++)
                    {
                        if(bTextBefore)
                            text += " ";
                        else
                            bTextBefore = true;

                        if (stopwords.Contains(terms[i]))
                            text += terms[i];
                        else
                            text += terms[i].Substring(0, 1).ToUpper() + terms[i].Substring(1);
                    }
                    if(text.Length > 0)
                        text = text.Substring(0, 1).ToUpper() + text.Substring(1);

                    break;
                case Capitalization.AllCapital:
                    text = item.Title.Substring(0, 1).ToUpper() + item.Title.Substring(1).ToLower();
                    break;
            }
            text = Regex.Replace(text, @"^\s+|\.*\s*$", "");
            text = CitationTools.ApplyStandardFormat(this, text, bInHtmlFormat);
            return text;
        }
    }

    public class JournalName : BaseClass
    {
        private JournalNameFormat _NameFormat;
        public JournalNameFormat NameFormat
        {
            get
            {
                return _NameFormat;
            }
            set
            {
                _NameFormat = value;
            }
        }

        private bool _IsAPA;
        public bool IsAPA
        {
            get
            {                
                return _IsAPA;
            }
            set
            {
                _IsAPA = value;
            }
        }

        private bool _RemovePeriod;
        public bool RemovePeriod
        {
            get
            {
                return _RemovePeriod;
            }
            set
            {
                _RemovePeriod = value;
            }
        }

        public JournalName()
        {
            Prefix = " ";
            Suffix = "";
            Bold = false;
            Italic = true;
            Underline = false;
            NameFormat = JournalNameFormat.Abbreviation;
            RemovePeriod = false;
        }    
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            string text = string.Empty;
            text = item.Title2;
            if(NameFormat == JournalNameFormat.Abbreviation)
            {
                if (Regex.Replace(item.JournalAbbr, @"\s+", "").Length > 0)
                    text = item.JournalAbbr;
            }                
            
            if (this.RemovePeriod)
                text.Replace(".", "");

            if(IsAPA && text.Length > 0)
            {
                string temp = string.Empty;
                // Capitalization each words
                string[] terms = Regex.Split(text.ToLower(), @"\s+");
                string[] stopwords = { "a", "an", "and", "as", "at", "but", "by", "for", "from", "in", "into", "nor", "of", "on", "or", "over", "per", "the", "to", "upon", "vs.", "with", "gi" };
                bool bTextBefore = false;
                for (int i = 0; i < terms.Length; i++)
                {
                    if (bTextBefore)
                        temp += " ";
                    else
                        bTextBefore = true;

                    if (stopwords.Contains(terms[i]))
                        temp += terms[i];
                    else
                        temp += terms[i].Substring(0, 1).ToUpper() + terms[i].Substring(1);
                }
                if (temp.Length > 0)
                    temp = temp.Substring(0, 1).ToUpper() + temp.Substring(1);

                text = temp;
            }

            text = CitationTools.ApplyStandardFormat(this, text, bInHtmlFormat);
            return text;
        }
    }

    public class ConfName : BaseClass
    {
        public ConfName()
        {
            Initialize();
            Prefix = "";
            Suffix = "; ";
        }
    
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Title2, @"\s+", "").Length == 0)
                return string.Empty;

            return CitationTools.ApplyStandardFormat(this, item.Title2, bInHtmlFormat);
        }
    }

    public class ConfLocation : BaseClass
    {
        public ConfLocation()
        {
            Initialize();
            Prefix = "";
            Suffix = ": ";
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.PubPlace, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.PubPlace, bInHtmlFormat);
        }
    }

    public class Volume : BaseClass
    {
        public Volume()
        {
            Prefix = "";
            Suffix = "";
            Bold = false;
            Italic = false;
            Underline = false;
        }

        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Volume, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Volume, bInHtmlFormat);
        }
    }

    public class Issue : BaseClass
    {
        public Issue()
        {
            Prefix = "(";
            Suffix = ")";
            Bold = false;
            Italic = false;
            Underline = false;
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Volume2, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Volume2, bInHtmlFormat);
        }
    }

    public class Abstract : BaseClass
    {
        public Abstract()
        {
            Initialize();
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Abstract, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Abstract, bInHtmlFormat);
        }
    }

    public class ISSN : BaseClass
    {
        public ISSN()
        {
            Initialize();
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.ID1, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.ID1, bInHtmlFormat);
        }
    }

    public class DOI : BaseClass
    {
        public DOI()
        {
            Initialize();
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.DOI, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.DOI, bInHtmlFormat);
        }
    }

    public class PubmedID : BaseClass
    {
        public PubmedID()
        {
            Initialize();
            Prefix = "[PubMed: ";
            Suffix = "]";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.ID2, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.ID2, bInHtmlFormat);
        }
    }

    public class BookTitle : BaseClass
    {
        private Capitalization _Capitalization;
        public Capitalization Capitalization
        {
            get
            {
                return _Capitalization;
            }
            set
            {
                _Capitalization = value;
            }
        }
        public BookTitle()
        {
            Initialize();
            Prefix = "";
            Suffix = ". ";
            Capitalization = Capitalization.AsIs;
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Title2, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Title2, bInHtmlFormat);
        }
    }

    public class Edition : BaseClass
    {
        public Edition()
        {
            Initialize();
            Prefix = "";
            Suffix = "ed. ";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Edition, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Edition, bInHtmlFormat);
        }
    }

    public class City : BaseClass
    {
        public City()
        {
            Initialize();
            Prefix = " ";
            Suffix = ":";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.PubPlace, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.PubPlace, bInHtmlFormat);
        }
    }

    public class Degree : BaseClass
    {
        public Degree()
        {
            Initialize();
            Prefix = "";
            Suffix = " [thesis]. ";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Title2, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Title2, bInHtmlFormat);
        }
    }

    public class School : BaseClass
    {
        public School()
        {
            Initialize();
            Prefix = "";
            Suffix = "; ";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Volume, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Volume, bInHtmlFormat);
        }
    }

    public class Department : BaseClass
    {
        public Department()
        {
            Initialize();
            Prefix = " ";
            Suffix = ";";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.Volume2, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.Volume2, bInHtmlFormat);
        }
    }

    public class Country : BaseClass
    {
        public Country()
        {
            Initialize();
            Prefix = "";
            Suffix = ": ";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.PubPlace, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, item.PubPlace, bInHtmlFormat);
        }
    }

    public class PatentNumber : BaseClass
    {
        public PatentNumber()
        {
            Initialize();
            Suffix = ".";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.ID1, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, "Patent " + item.ID1, bInHtmlFormat);
        }
    }

    public class ApplicationNumber : BaseClass
    {
        public ApplicationNumber()
        {
            Initialize();
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            if (Regex.Replace(item.ID2, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, "Patent " + item.ID2, bInHtmlFormat);
        }
    }

    public class Link : BaseClass
    {
        public Link()
        {
            Initialize();
            Prefix = " ";
        }
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            string link = string.Empty;
            if (item.ItemTypeID == ItemTypes.WebPage)
                link = item.Affiliation;
            else
                link = item.Links;
            if (Regex.Replace(link, @"\s+", "").Length == 0)
                return string.Empty;
            return CitationTools.ApplyStandardFormat(this, link, bInHtmlFormat);
        }
    }

    #endregion
}
