using System;

namespace MSWordpiCite.Entities
{
    using MSWordpiCite.Enums;
    using System.Reflection;
    using System.Data;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;    
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Configuration;
    using System.Web;

    [Serializable]
    public enum ItemFolderMapColumns
    {
        FolderID,
        ItemID,
        SequenceNo
    }

    [Serializable]
    public enum ItemMasterColumns
    {
        ItemID,
        ItemTypeID,
        FlagID,
        Title,
        Title2,
        Author,
        Affiliation,
        Edition,
        Volume,
        Volume2,
        Abstract,
        Notes,
        PubPlace,
        PubDate,
        IssueDate,
        Date3,
        Pages,
        ID1,
        ID2,
        DOI,
        Source,
        ItemFile,
        AttachedFiles,
        Links,
        DateCreated,
        DateModified,
        Disable,
        Keywords,
        Tags,
        JournalAbbr,
        PubYear,
        AccessCode
    }

    [Serializable]
    public class ItemMasterRow
    {
        public static readonly Dictionary<ItemTypes, Dictionary<string, bool>> FieldMap;

        static ItemMasterRow()
        {
            FieldMap = new Dictionary<ItemTypes, Dictionary<string, bool>>();

            Dictionary<string, bool> h = new Dictionary<string, bool>();
            h.Add("Title2", true);
            h.Add("Affiliation", true);
            h.Add("Edition", false);
            h.Add("Volume", true);
            h.Add("Volume2", true);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", false);
            h.Add("PubDate", true);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", true);
            h.Add("DOI", true);
            h.Add("ID2", true);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", true);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", false);
            h.Add("Publisher", false);
            FieldMap.Add(ItemTypes.JournalArticle, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", true);
            h.Add("Affiliation", true);
            h.Add("Edition", false);
            h.Add("Volume", true);
            h.Add("Volume2", false);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", true);
            h.Add("PubDate", true);
            h.Add("IssueDate", true);
            h.Add("Pages", true);
            h.Add("ID1", true);
            h.Add("DOI", true);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", true);
            h.Add("Publisher", true);
            FieldMap.Add(ItemTypes.Proceeding, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", true);
            h.Add("Affiliation", true);
            h.Add("Edition", false);
            h.Add("Volume", true);
            h.Add("Volume2", true);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", true);
            h.Add("PubDate", true);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", true);
            h.Add("DOI", false);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", false);
            h.Add("Publisher", false);
            FieldMap.Add(ItemTypes.Thesis, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", true);
            h.Add("Affiliation", false);
            h.Add("Edition", true);
            h.Add("Volume", true);
            h.Add("Volume2", true);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", true);
            h.Add("PubDate", true);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", true);
            h.Add("DOI", false);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", true);
            h.Add("Publisher", true);
            FieldMap.Add(ItemTypes.BookChapter, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", false);
            h.Add("Affiliation", false);
            h.Add("Edition", true);
            h.Add("Volume", true);
            h.Add("Volume2", true);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", true);
            h.Add("PubDate", true);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", true);
            h.Add("DOI", false);
            h.Add("ID2", true);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", true);
            h.Add("Publisher", true);
            FieldMap.Add(ItemTypes.BookWhole, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", false);
            h.Add("Affiliation", true);
            h.Add("Edition", false);
            h.Add("Volume", false);
            h.Add("Volume2", false);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", true);
            h.Add("PubDate", true);
            h.Add("IssueDate", true);
            h.Add("Pages", true);
            h.Add("ID1", true);
            h.Add("DOI", false);
            h.Add("ID2", true);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", true);
            h.Add("Author", true);
            h.Add("Editor", false);
            h.Add("Publisher", true);
            FieldMap.Add(ItemTypes.Patent, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", true);
            h.Add("Affiliation", true);
            h.Add("Edition", false);
            h.Add("Volume", true);
            h.Add("Volume2", false);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", false);
            h.Add("PubDate", true);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", false);
            h.Add("DOI", false);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", true);
            h.Add("Publisher", false);
            FieldMap.Add(ItemTypes.Audio, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", false);
            h.Add("Affiliation", false);
            h.Add("Edition", false);
            h.Add("Volume", false);
            h.Add("Volume2", false);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", false);
            h.Add("PubDate", false);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", false);
            h.Add("DOI", false);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", false);
            h.Add("Publisher", false);
            FieldMap.Add(ItemTypes.LectureNote, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", false);
            h.Add("Affiliation", false);
            h.Add("Edition", false);
            h.Add("Volume", false);
            h.Add("Volume2", false);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", false);
            h.Add("PubDate", false);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", false);
            h.Add("DOI", false);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", false);
            h.Add("Publisher", false);
            FieldMap.Add(ItemTypes.Video, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", false);
            h.Add("Affiliation", false);
            h.Add("Edition", false);
            h.Add("Volume", false);
            h.Add("Volume2", false);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", false);
            h.Add("PubDate", false);
            h.Add("IssueDate", false);
            h.Add("Pages", true);
            h.Add("ID1", false);
            h.Add("DOI", false);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", false);
            h.Add("Publisher", false);
            FieldMap.Add(ItemTypes.Document, h);

            h = new Dictionary<string, bool>();
            h.Add("Title2", false);
            h.Add("Affiliation", true);
            h.Add("Edition", false);
            h.Add("Volume", false);
            h.Add("Volume2", false);
            h.Add("Abstract", true);
            h.Add("Notes", true);
            h.Add("PubPlace", false);
            h.Add("PubDate", true);
            h.Add("IssueDate", true);
            h.Add("Pages", false);
            h.Add("ID1", false);
            h.Add("DOI", false);
            h.Add("ID2", false);
            h.Add("Source", true);
            h.Add("ItemFile", true);
            h.Add("JournalAbbr", false);
            h.Add("Date3", false);
            h.Add("Author", true);
            h.Add("Editor", false);
            h.Add("Publisher", true);
            FieldMap.Add(ItemTypes.WebPage, h);
        }

        private string _UserID = string.Empty;
        private int _ItemID = 0;
        private int _SequenceNo = 0;
        private ItemTypes _ItemTypeID = ItemTypes.Document;
        private Flags _FlagID = Flags.Clear;
        private string _Title = string.Empty;
        private string _Title2 = string.Empty;
        private string _JournalAbbr = string.Empty;
        private string _Author = string.Empty;
        private string _Affiliation = string.Empty;
        private string _Edition = string.Empty;
        private string _Volume = string.Empty;
        private string _Volume2 = string.Empty;
        private string _Abstract = string.Empty;
        private string _Notes = string.Empty;
        private string _PubPlace = string.Empty;
        private string _PubDate = string.Empty;
        private string _PubYear = string.Empty;
        private int _SortYear = -1;
        private string _IssueDate = string.Empty;
        private string _Date3 = string.Empty;
        private string _Pages = string.Empty;
        private string _ID1 = string.Empty;
        private string _ID2 = string.Empty;
        private string _DOI = string.Empty;
        private string _Source = string.Empty;
        private string _ItemFile = string.Empty;
        private string _AttachedFiles = string.Empty;
        private string _Links = string.Empty;
        private DateTime _DateCreated = DateTime.Now;
        private DateTime _DateModified = DateTime.Now;
        private string _AccessCode = string.Empty;

        private string _Keywords = string.Empty;
        private string _Tags = string.Empty;

        private bool _Selected = true;
        private Boolean _Disable = false;
        private bool _Shared = false;

        public List<NameMasterRow> Authors;

        public ItemMasterRow()
        {
            _ItemID = int.MinValue;
            _SequenceNo = int.MinValue;
            _FlagID = Flags.Clear;
            Authors = new List<NameMasterRow>();
        }

        protected ItemMasterRow(int itemId, ItemTypes itemTypeId, Flags flagId, string title, string title2, string author,
                             string affiliation, string edition, string volume, string volume2, string abstracts,
                             string notes, string pubPlace, string pubDate, string issueDate, string date3, string pages,
                             string id1, string id2, string doi, string source, string itemFile, string attachedFiles,
                             string links, DateTime dateCreated, DateTime dateModified, bool disable, int seqNo, string keywords,
                             string tags, string journalabbr, string pubyear, string accesscode)
        {
            this.ItemID = itemId;
            this.ItemTypeID = itemTypeId;
            this._FlagID = flagId;
            this.Title = title;
            this.Title2 = title2;
            this.Author = author;
            this.Affiliation = affiliation;
            this.Edition = edition;
            this.Volume = volume;
            this.Volume2 = volume2;
            this.Abstract = abstracts;
            this.Notes = notes;
            this.PubPlace = pubPlace;
            this.PubDate = pubDate;
            this.IssueDate = issueDate;
            this.Date3 = date3;
            this.Pages = pages;
            this.ID1 = id1;
            this.ID2 = id2;
            this.DOI = doi;
            this.Source = source;
            this.ItemFile = itemFile;
            this.AttachedFiles = attachedFiles;
            this.Links = links;
            _DateCreated = dateCreated;
            _DateModified = dateModified;
            this.Disable = disable;
            this._SequenceNo = seqNo;
            this._Keywords = keywords;
            this._Tags = tags;
            this._JournalAbbr = journalabbr;
            this._PubYear = pubyear;
            this._AccessCode = accesscode;

            this.Authors = new List<NameMasterRow>();
        }

        public static List<ItemMasterRow> Convert(DataView dv, string UserID)
        {
            List<ItemMasterRow> ItemList = new List<ItemMasterRow>();

            string[] strColumns = TableColumns;
            bool HasSeqNo = true;

            if (dv.Count > 0)
            {
                for (int i = 0; i < strColumns.Length; i++)
                {
                    if (!dv.Table.Columns.Contains(strColumns[i]))
                        return ItemList;
                }

                if (!dv.Table.Columns.Contains(ItemMasterColumns.DateCreated.ToString() + "_INT") ||
                    !dv.Table.Columns.Contains(ItemMasterColumns.DateModified.ToString() + "_INT"))
                    return ItemList;

                if (!dv.Table.Columns.Contains(ItemFolderMapColumns.SequenceNo.ToString()))
                {
                    dv.Table.Columns.Add(ItemFolderMapColumns.SequenceNo.ToString());
                    HasSeqNo = false;
                }
            }

            for (int i = 0; i < dv.Count; i++)
            {
                if (!HasSeqNo)
                    dv[i][ItemFolderMapColumns.SequenceNo.ToString()] = i;

                ItemMasterRow item = new ItemMasterRow(
                      int.Parse(dv[i][ItemMasterColumns.ItemID.ToString()] + ""),
                      (ItemTypes)int.Parse(dv[i][ItemMasterColumns.ItemTypeID.ToString()] + ""),
                      (Flags)int.Parse(dv[i][ItemMasterColumns.FlagID.ToString()] + ""),
                      dv[i][ItemMasterColumns.Title.ToString()] + "",
                      dv[i][ItemMasterColumns.Title2.ToString()] + "",
                      dv[i][ItemMasterColumns.Author.ToString()] + "",
                      dv[i][ItemMasterColumns.Affiliation.ToString()] + "",
                      dv[i][ItemMasterColumns.Edition.ToString()] + "",
                      dv[i][ItemMasterColumns.Volume.ToString()] + "",
                      dv[i][ItemMasterColumns.Volume2.ToString()] + "",
                      dv[i][ItemMasterColumns.Abstract.ToString()] + "",
                      dv[i][ItemMasterColumns.Notes.ToString()] + "",
                      dv[i][ItemMasterColumns.PubPlace.ToString()] + "",
                      dv[i][ItemMasterColumns.PubDate.ToString()] + "",
                      dv[i][ItemMasterColumns.IssueDate.ToString()] + "",
                      dv[i][ItemMasterColumns.Date3.ToString()] + "",
                      dv[i][ItemMasterColumns.Pages.ToString()] + "",
                      dv[i][ItemMasterColumns.ID1.ToString()] + "",
                      dv[i][ItemMasterColumns.ID2.ToString()] + "",
                      dv[i][ItemMasterColumns.DOI.ToString()] + "",
                      dv[i][ItemMasterColumns.Source.ToString()] + "",
                      dv[i][ItemMasterColumns.ItemFile.ToString()] + "",
                      dv[i][ItemMasterColumns.AttachedFiles.ToString()] + "",
                      dv[i][ItemMasterColumns.Links.ToString()] + "",
                      DateTime.ParseExact(dv[i][ItemMasterColumns.DateCreated.ToString() + "_INT"] + "", "yyyyMMddHHmmss", Thread.CurrentThread.CurrentCulture),
                      DateTime.ParseExact(dv[i][ItemMasterColumns.DateModified.ToString() + "_INT"] + "", "yyyyMMddHHmmss", Thread.CurrentThread.CurrentCulture),
                      bool.Parse(dv[i][ItemMasterColumns.Disable.ToString()] + ""),
                      int.Parse(dv[i][ItemFolderMapColumns.SequenceNo.ToString()] + ""),
                      dv[i][ItemMasterColumns.Keywords.ToString()] + "",
                      dv[i][ItemMasterColumns.Tags.ToString()] + "",
                      dv[i][ItemMasterColumns.JournalAbbr.ToString()] + "",
                      dv[i][ItemMasterColumns.PubYear.ToString()] + "",
                      dv[i][ItemMasterColumns.AccessCode.ToString()] + ""
                      );

                item.UserID = UserID;

                ItemList.Add(item);
            }

            return ItemList;
        }

        public string UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                _UserID = value;
            }
        }

        public int ItemID
        {
            get { return _ItemID; }
            set { _ItemID = value; }
        }

        public int SequenceNo
        {
            get { return _SequenceNo; }
            set { _SequenceNo = value; }
        }

        public ItemTypes ItemTypeID
        {
            get { return _ItemTypeID; }
            set { _ItemTypeID = value; }
        }

        public int FlagID
        {
            get { return (int)_FlagID; }
            set { _FlagID = (Flags)value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string Title2
        {
            get { return _Title2; }
            set { _Title2 = value; }
        }

        public string Author
        {
            get { return _Author; }
            set { _Author = value; }
        }

        public string Affiliation
        {
            get { return _Affiliation; }
            set { _Affiliation = value; }
        }

        public string Edition
        {
            get { return _Edition; }
            set { _Edition = value; }
        }

        public string Volume
        {
            get { return _Volume; }
            set { _Volume = value; }
        }

        public string Volume2
        {
            get { return _Volume2; }
            set { _Volume2 = value; }
        }

        public string Abstract
        {
            get { return _Abstract; }
            set { _Abstract = value; }
        }

        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }

        public string PubPlace
        {
            get { return _PubPlace; }
            set { _PubPlace = value; }
        }

        public string PubDate
        {
            get { return _PubDate; }
            set { _PubDate = value; }
        }

        public string IssueDate
        {
            get { return _IssueDate; }
            set { _IssueDate = value; }
        }

        public string Date3
        {
            get { return _Date3; }
            set { _Date3 = value; }
        }

        public string Pages
        {
            get { return _Pages; }
            set { _Pages = value; }
        }

        public string ID1
        {
            get { return _ID1; }
            set { _ID1 = value; }
        }

        public string ID2
        {
            get { return _ID2; }
            set { _ID2 = value; }
        }

        public string DOI
        {
            get { return _DOI; }
            set { _DOI = value; }
        }

        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        public string ItemFile
        {
            get { return _ItemFile; }
            set { _ItemFile = value; }
        }

        public string AttachedFiles
        {
            get { return _AttachedFiles; }
            set { _AttachedFiles = value; }
        }

        public string Links
        {
            get { return _Links; }
            set { _Links = value; }
        }

        public int SortYear
        {
            get { return _SortYear; }
            set { _SortYear = value; }
        }

        public string DateCreated
        {
            get { return _DateCreated.ToString("yyyyMMddHHmmss"); }
            set
            {
                try
                {
                    _DateCreated = DateTime.ParseExact(value, "yyyyMMddHHmmss", Thread.CurrentThread.CurrentCulture);
                }
                catch
                {
                    _DateCreated = DateTime.Now;
                }
            }
        }

        public string DateModified
        {
            get { return _DateModified.ToString("yyyyMMddHHmmss"); }
            set
            {
                try
                {
                    _DateModified = DateTime.ParseExact(value, "yyyyMMddHHmmss", Thread.CurrentThread.CurrentCulture);
                }
                catch
                {
                    _DateModified = DateTime.Now;
                }
            }
        }

        public string Keywords
        {
            get { return _Keywords; }
            set { _Keywords = value; }
        }

        public string Tags
        {
            get
            {
                return _Tags;
            }
            set
            {
                _Tags = value;
            }
        }

        public string JournalAbbr
        {
            get
            {
                return _JournalAbbr;
            }
            set
            {
                _JournalAbbr = value;
            }
        }

        public string PubYear
        {
            get
            {
                return _PubYear;
            }
            set
            {
                _PubYear = value;
            }
        }

        public string AccessCode
        {
            get
            {
                return _AccessCode;
            }
            set
            {
                _AccessCode = value;
            }
        }

        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }

        public Boolean Disable
        {
            get { return _Disable; }
            set { _Disable = value; }
        }

        public bool Shared
        {
            get { return _Shared; }
            set { _Shared = value; }
        }

        public void Trim()
        {
            _UserID = _UserID.Trim();
            _Title = _Title.Trim();
            _Title2 = _Title2.Trim();
            _JournalAbbr = _JournalAbbr.Trim();
            _Author = _Author.Trim();
            _Affiliation = _Affiliation.Trim();
            _Edition = _Edition.Trim();
            _Volume = _Volume.Trim();
            _Volume2 = _Volume2.Trim();
            _Abstract = _Abstract.Trim();
            _Notes = _Notes.Trim();
            _PubPlace = _PubPlace.Trim();
            _PubDate = _PubDate.Trim();
            _PubYear = _PubYear.Trim();
            _IssueDate = _IssueDate.Trim();
            _Date3 = _Date3.Trim();
            _Pages = _Pages.Trim();
            _ID1 = _ID1.Trim();
            _ID2 = _ID2.Trim();
            _DOI = _DOI.Trim();
            _Source = _Source.Trim();
            _ItemFile = _ItemFile.Trim();
            _AttachedFiles = _AttachedFiles.Trim();
            _Links = _Links.Trim();

            _Keywords = _Keywords.Trim();
            _Tags = _Tags.Trim();
        }

        public void Clone(ItemMasterRow newitem)
        {
            this.ItemID = newitem.ItemID;
            this.ItemTypeID = newitem.ItemTypeID;
            this._FlagID = newitem._FlagID;
            this.Title = newitem.Title;
            this.Title2 = newitem.Title2;
            this.Author = newitem.Author;
            this.Affiliation = newitem.Affiliation;
            this.Edition = newitem.Edition;
            this.Volume = newitem.Volume;
            this.Volume2 = newitem.Volume2;
            this.Abstract = newitem.Abstract;
            this.Notes = newitem.Notes;
            this.PubPlace = newitem.PubPlace;
            this.PubDate = newitem.PubDate;
            this.IssueDate = newitem.IssueDate;
            this.Date3 = newitem.Date3;
            this.Pages = newitem.Pages;
            this.ID1 = newitem.ID1;
            this.ID2 = newitem.ID2;
            this.DOI = newitem.DOI;
            this.Source = newitem.Source;
            this.ItemFile = newitem.ItemFile;
            this.AttachedFiles = newitem.AttachedFiles;
            this.Links = newitem.Links;
            _DateCreated = newitem._DateCreated;
            _DateModified = newitem._DateModified;
            this.Disable = newitem.Disable;
            this._SequenceNo = newitem._SequenceNo;
            this._Keywords = newitem._Keywords;
            this._Tags = newitem._Tags;
            this._JournalAbbr = newitem._JournalAbbr;
            this._PubYear = newitem._PubYear;
            this._AccessCode = newitem._AccessCode;
            this.Authors = newitem.Authors;

        }

        public static string[] TableColumns
        {
            get { return Enum.GetNames(typeof(ItemMasterColumns)); }
        }

        public static bool Equals(ItemMasterRow objA, ItemMasterRow objB)
        {
            if (objA == null) return false;
            else if (objB == null) return false;
            else return (objA.ItemID == objB.ItemID);
        }

        public static bool DetectConflicts(ref ItemMasterRow item, ref ItemMasterRow Duplicate, ref Dictionary<string, int> ConflictRules)
        {
            Dictionary<string, bool> h = FieldMap[item.ItemTypeID];

            if (!ConflictRules.ContainsKey("Title2") && h["Title2"] && item._Title2 != Duplicate._Title2)
                ConflictRules.Add("Title2", 0);

            if (!ConflictRules.ContainsKey("Affiliation") && h["Affiliation"] && item._Affiliation != Duplicate._Affiliation)
                ConflictRules.Add("Affiliation", 0);

            if (!ConflictRules.ContainsKey("Edition") && h["Edition"] && item._Edition != Duplicate._Edition)
                ConflictRules.Add("Edition", 0);

            if (!ConflictRules.ContainsKey("Volume") && h["Volume"] && item._Volume != Duplicate._Volume)
                ConflictRules.Add("Volume", 0);

            if (!ConflictRules.ContainsKey("Volume2") && h["Volume2"] && item._Volume2 != Duplicate._Volume2)
                ConflictRules.Add("Volume2", 0);

            if (!ConflictRules.ContainsKey("Abstract") && h["Abstract"] && item._Abstract != Duplicate._Abstract)
                ConflictRules.Add("Abstract", 0);

            if (!ConflictRules.ContainsKey("Notes") && h["Notes"] && item._Notes != Duplicate._Notes)
                ConflictRules.Add("Notes", 0);

            if (!ConflictRules.ContainsKey("PubPlace") && h["PubPlace"] && item._PubPlace != Duplicate._PubPlace)
                ConflictRules.Add("PubPlace", 0);

            if (!ConflictRules.ContainsKey("PubDate") && h["PubDate"] && item._PubDate != Duplicate._PubDate)
                ConflictRules.Add("PubDate", 0);

            if (!ConflictRules.ContainsKey("IssueDate") && h["IssueDate"] && item._IssueDate != Duplicate._IssueDate)
                ConflictRules.Add("IssueDate", 0);

            if (!ConflictRules.ContainsKey("Pages") && h["Pages"] && item._Pages != Duplicate._Pages)
                ConflictRules.Add("Pages", 0);

            if (!ConflictRules.ContainsKey("ID1") && h["ID1"] && item._ID1 != Duplicate._ID1)
                ConflictRules.Add("ID1", 0);

            if (!ConflictRules.ContainsKey("DOI") && h["DOI"] && item._DOI != Duplicate._DOI)
                ConflictRules.Add("DOI", 0);

            if (!ConflictRules.ContainsKey("ID2") && h["ID2"] && item._ID2 != Duplicate._ID2)
                ConflictRules.Add("ID2", 0);

            if (!ConflictRules.ContainsKey("Source") && h["Source"] && item._Source != Duplicate._Source)
                ConflictRules.Add("Source", 0);

            if (!ConflictRules.ContainsKey("ItemFile") && h["ItemFile"] && item._ItemFile != Duplicate._ItemFile)
            {
                if (item.ItemFile.Length == 0)
                    ConflictRules.Add("ItemFile", 2);
                else
                    ConflictRules.Add("ItemFile", 0);
            }

            if (!ConflictRules.ContainsKey("JournalAbbr") && h["JournalAbbr"] && item._JournalAbbr != Duplicate._JournalAbbr)
                ConflictRules.Add("JournalAbbr", 0);

            if (!ConflictRules.ContainsKey("Date3") && h["Date3"] && item._Date3 != Duplicate._Date3)
                ConflictRules.Add("Date3", 0);

            #region Author

            item.Authors.Sort();
            Duplicate.Authors.Sort();

            List<List<NameMasterRow>> list = new List<List<NameMasterRow>>();
            for (int i = 0; i < 6; i++)
                list.Add(new List<NameMasterRow>());

            for (int i = 0; i < item.Authors.Count; i++)
            {
                NameTypes type = item.Authors[i].NameTypeID;
                if (type == NameTypes.Author)
                    list[0].Add(item.Authors[i]);
                else if (type == NameTypes.Editor)
                    list[2].Add(item.Authors[i]);
                else if (type == NameTypes.Publisher)
                    list[4].Add(item.Authors[i]);
            }

            for (int i = 0; i < Duplicate.Authors.Count; i++)
            {
                NameTypes type = Duplicate.Authors[i].NameTypeID;
                if (type == NameTypes.Author)
                    list[1].Add(Duplicate.Authors[i]);
                else if (type == NameTypes.Editor)
                    list[3].Add(Duplicate.Authors[i]);
                else if (type == NameTypes.Publisher)
                    list[5].Add(Duplicate.Authors[i]);
            }

            if (h["Author"])
            {
                if (list[0].Count != list[1].Count)
                {
                    if (!ConflictRules.ContainsKey("Author"))
                        ConflictRules.Add("Author", 0);
                }
                else
                {
                    for (int i = 0; i < list[0].Count; i++)
                    {
                        if (list[0][i].LastName != list[1][i].LastName || list[0][i].ForeName != list[1][i].ForeName)
                            if (!ConflictRules.ContainsKey("Author"))
                            {
                                ConflictRules.Add("Author", 0);
                                break;
                            }
                    }
                }
            }

            if (h["Editor"])
            {
                if (list[2].Count != list[3].Count)
                {
                    if (!ConflictRules.ContainsKey("Editor"))
                        ConflictRules.Add("Editor", 0);
                }
                else
                {
                    for (int i = 0; i < list[2].Count; i++)
                    {
                        if (list[2][i].LastName != list[3][i].LastName || list[2][i].ForeName != list[3][i].ForeName)
                            if (!ConflictRules.ContainsKey("Editor"))
                            {
                                ConflictRules.Add("Editor", 0);
                                break;
                            }
                    }
                }
            }

            if (h["Publisher"])
            {
                if (list[4].Count != list[5].Count)
                {
                    if (!ConflictRules.ContainsKey("Publisher"))
                        ConflictRules.Add("Publisher", 0);
                }
                else
                {
                    for (int i = 0; i < list[4].Count; i++)
                    {
                        if (list[4][i].LastName != list[5][i].LastName)
                            if (!ConflictRules.ContainsKey("Publisher"))
                            {
                                ConflictRules.Add("Publisher", 0);
                                break;
                            }
                    }
                }
            }

            #endregion

            Dictionary<string, int>.KeyCollection.Enumerator col = ConflictRules.Keys.GetEnumerator();
            while (col.MoveNext())
            {
                if (ConflictRules[col.Current] == 0)
                    return true;
            }

            return false;
        }

        public string BuildOpenURLString()
        {
            string _sfxUrl = string.Empty;
            string strStartPage = Regex.Match(Pages, @"(.*?)(\-|$)").Groups[1].Value;
            string strLastPage = Regex.Match(Pages, @"\-(.*?)$").Groups[1].Value;
            string curDate = DateTime.Now.ToString();
            curDate = Regex.Match(curDate, @"^(.*?\/\d{4})").Groups[1].Value;
            if (curDate != null)
            {
                curDate = Regex.Replace(curDate, @"\/", "-");
                if (curDate != null)
                {
                    string[] curDate1 = curDate.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (curDate1.Length > 1)
                        curDate = curDate1[2] + "-" + curDate1[0] + "-" + curDate1[1];
                }
            }
            curDate = "&ctx_tim=" + curDate + "T";//23%3A5%3A19CST
            _sfxUrl = ConfigurationManager.AppSettings["OPENURL_STRING"] + curDate;
            string strType = string.Empty;
            string strType1 = "&rft_val_fmt=info%3Aofi%2Ffmt%3Akev%3Amtx%3A";
            switch (ItemTypeID)
            {
                case ItemTypes.JournalArticle:
                    strType = "article";
                    strType1 += "journal";
                    break;
                case ItemTypes.Proceeding:
                    strType1 += "proceeding";
                    strType = "book";
                    break;
                case ItemTypes.BookChapter:
                case ItemTypes.BookWhole:
                    strType1 += "book";
                    strType = "book";
                    break;
                case ItemTypes.Patent:
                    strType1 += "patent";
                    break;
                case ItemTypes.Thesis:
                    strType1 += "dissertation";
                    break;
                default:
                    strType1 += "dc";
                    break;
            }
            if (strType.Length > 0)
                _sfxUrl += "&rft.genre=" + strType;

            _sfxUrl += strType1;

            if (Title2.Length > 0)
                _sfxUrl += "&rft.jtitle=" + HttpUtility.UrlEncode(Title2);

            if (ID1.Length > 0)
                _sfxUrl += "&rft.issn=" + HttpUtility.UrlEncode(ID1);

            if (Authors.Count > 0)
            {
                _sfxUrl += "&rft.aulast=" + HttpUtility.UrlEncode(Authors[0].LastName);
                _sfxUrl += "&rft.aufirst=" + HttpUtility.UrlEncode(Authors[0].ForeName);
                for (int i = 0; i < Authors.Count; i++)
                {
                    _sfxUrl += "&rft.au=" + HttpUtility.UrlEncode(Authors[i].LastName + ", " + Authors[i].ForeName);
                }
            }

            if (PubYear.Length > 0)
                _sfxUrl += "&rft.date=" + HttpUtility.UrlEncode(PubYear);
            else if (PubDate.Length > 0)
                _sfxUrl += "&rft.date=" + HttpUtility.UrlEncode(PubDate);

            if (strStartPage.Length > 0)
                _sfxUrl += "&rft.spage=" + HttpUtility.UrlEncode(strStartPage);

            if (strLastPage.Length > 0)
                _sfxUrl += "&rft.epage=" + HttpUtility.UrlEncode(strLastPage);

            if (ID2.Length > 0)
                _sfxUrl += "&rft_id=info%3Apmid%2F" + HttpUtility.UrlEncode(ID2);

            if (Volume2.Length > 0)
                _sfxUrl += "&rft.issue=" + HttpUtility.UrlEncode(Volume2);

            if (Volume.Length > 0)
                _sfxUrl += "&rft.volume=" + HttpUtility.UrlEncode(Volume);

            if (Title.Length > 0)
            {
                Title = Title.Replace(@"\.$", "");
                if (_ItemTypeID == ItemTypes.JournalArticle)
                    _sfxUrl += "&rft.atitle=" + HttpUtility.UrlEncode(Title);
                else
                    _sfxUrl += "&rft.title=" + HttpUtility.UrlEncode(Title);
            }
            return _sfxUrl;
        }

        /// <summary>
        /// Form the BibTeX Format string for Item
        /// </summary>
        /// <param name="bIsMore"></param>
        /// <returns></returns>
        public string ToBibTex(bool bIsMore)
        {
            StringBuilder sb = new StringBuilder();
            switch (_ItemTypeID)
            {
                case ItemTypes.JournalArticle:
                    sb.Append("@article{");
                    break;
                case ItemTypes.BookWhole:
                    sb.Append("@book{");
                    break;
                case ItemTypes.BookChapter:
                    sb.Append("@inbook{");
                    break;
                case ItemTypes.Proceeding:
                    sb.Append("@conference{");
                    break;
                case ItemTypes.Thesis:
                    sb.Append("@phdthesis{");
                    break;
                case ItemTypes.Patent:
                    sb.Append("@misc{");
                    break;
                case ItemTypes.WebPage:
                    sb.Append("@misc{");
                    break;
                default:
                    sb.Append("@misc{");
                    break;
            }

            sb.Append(_BibTexCitation());
            sb.Append(",");
            sb.AppendLine();

            //Title
            if (_ItemTypeID == ItemTypes.BookChapter)
            {
                sb.Append("\ttitle = {" + (_Title2.Length > 0 ? _Title2 : "") + "}");
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tchapter = {" + (_Title.Length > 0 ? _Title : "") + "}");
            }
            else
                sb.Append("\ttitle = {" + (_Title.Length > 0 ? _Title : "") + "}");

            //Authors
            if (Authors.Count > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tauthor = {");
                int Count = 0;
                int iNumPublishers = 0;
                int iNumEditors = 0;
                foreach (NameMasterRow name in Authors)
                {
                    if (name.NameTypeID == NameTypes.Author)
                    {
                        if (Count > 0)
                            sb.Append(" and ");
                        sb.Append(name.LastName + ", " + name.ForeName);
                        Count++;
                    }
                    else if (name.NameTypeID == NameTypes.Publisher)
                        iNumPublishers++;
                    else if (name.NameTypeID == NameTypes.Editor)
                        iNumEditors++;
                }
                sb.Append("}");

                if (iNumPublishers > 0 && (_ItemTypeID == ItemTypes.Proceeding || _ItemTypeID == ItemTypes.BookChapter || _ItemTypeID == ItemTypes.BookWhole))
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.Append("\tpublisher = {");
                    Count = 0;
                    foreach (NameMasterRow name in Authors)
                    {
                        if (name.NameTypeID == NameTypes.Publisher)
                        {
                            if (Count > 0)
                                sb.Append(" and ");
                            sb.Append(name.LastName);
                            Count++;
                        }
                    }
                    sb.Append("}");
                }

                if (iNumEditors > 0 && (_ItemTypeID == ItemTypes.Proceeding || _ItemTypeID == ItemTypes.BookChapter || _ItemTypeID == ItemTypes.BookWhole))
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.Append("\teditor = {");
                    Count = 0;
                    foreach (NameMasterRow name in Authors)
                    {
                        if (name.NameTypeID == NameTypes.Editor)
                        {
                            if (Count > 0)
                                sb.Append(" and ");
                            sb.Append(name.LastName + ", " + name.ForeName);
                            Count++;
                        }
                    }
                    sb.Append("}");
                }
            }

            //PubDate
            if (_PubDate.Length > 0 && Regex.Match(_PubDate, @"\d{4}").Success)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tyear = {" + Regex.Match(_PubDate, @"\d{4}").Value + "}");
            }

            //Title2
            if (_Title2.Length > 0)
            {
                if (_ItemTypeID == ItemTypes.JournalArticle)
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.Append("\tjournal = {" + _Title2 + "}");
                }
                else if (_ItemTypeID == ItemTypes.Proceeding)
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.Append("\tbooktitle = {" + _Title2 + "}");
                }
                else if (_ItemTypeID == ItemTypes.Thesis)
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.Append("\ttype = {" + _Title2 + "}");
                }
                else if (_ItemTypeID == ItemTypes.BookChapter)
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.Append("\ttype = {" + _Title2 + "}");
                }
            }

            //Volume
            if (_Volume.Length > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                if (_ItemTypeID == ItemTypes.Thesis)
                    sb.Append("\tschool = {" + _Volume + "}");
                else
                {
                    sb.Append("\tvolume = {" + _Volume + "}");
                }
            }

            //Volume2
            if (_Volume2.Length > 0 && _ItemTypeID == ItemTypes.JournalArticle)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tnumber = {" + _Volume2 + "}");
            }

            //Abstract
            if (_Abstract.Length > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tabstract = {" + _Abstract + "}");
            }

            //ISSN or ISBN
            if (_ID1.Length > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                if (_ItemTypeID == ItemTypes.JournalArticle)
                    sb.Append("\tISSN = {" + _ID1 + "}");
                else if (_ItemTypeID == ItemTypes.Proceeding || _ItemTypeID == ItemTypes.BookWhole || _ItemTypeID == ItemTypes.BookChapter)
                    sb.Append("\tISBN = {" + _ID1 + "}");
            }

            //DOI
            if (_DOI.Length > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tDOI = {" + _DOI + "}");
            }

            //Pages
            if (_Pages.Length > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tpages = {" + _Pages + "}");
            }

            //Links
            if (_Links.Length > 0)
            {
                string[] strTemp = Regex.Split(_Links, @"\|");
                if (strTemp.Length > 1)
                {
                    if (strTemp[1].Length > 0)
                    {
                        sb.Append(",");
                        sb.AppendLine();
                        sb.Append("\turl = {" + strTemp[1] + "}");
                    }
                }
            }

            //Notes
            if (_Notes.Length > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tnote = {" + _Notes + "}");
            }

            //Keywords
            if (_Keywords.Length > 0)
            {
                sb.Append(",");
                sb.AppendLine();
                sb.Append("\tkeywords = {");
                string[] strKeywords = Regex.Split(_Keywords, @"\|");
                for (int i = 0; i < strKeywords.Length; i++)
                {
                    if (i != 0)
                        sb.Append(", ");
                    sb.Append(strKeywords[i]);
                }
                sb.Append("}");
            }

            sb.AppendLine();
            sb.Append("}");
            if (bIsMore)
                sb.Append(",");
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Form BibTex Citation key: e.g: Clinton_Scandal_1998
        /// </summary>
        /// <returns></returns>
        private string _BibTexCitation()
        {
            string strCite = "";
            if (Authors.Count > 0)
                strCite += Authors[0].LastName;
            if (_Title.Length > 0)
                strCite += "_" + Regex.Replace(Regex.Replace(_Title, @"^\s+", ""), @"\s+.*", "");
            if (_PubDate.Length > 0)
                strCite += "_" + Regex.Match(_PubDate, @"\d{4}").Value;

            return strCite;
        }

        /// <summary>
        /// Form the RIS Format string for Item
        /// </summary>
        /// <returns></returns>
        public string ToRIS()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("TY  - ");

            switch (_ItemTypeID)
            {
                case ItemTypes.JournalArticle:
                    sb.Append("JOUR");
                    break;
                case ItemTypes.BookWhole:
                    sb.Append("BOOK");
                    break;
                case ItemTypes.BookChapter:
                    sb.Append("CHAP");
                    break;
                case ItemTypes.Proceeding:
                    sb.Append("CONF");
                    break;
                case ItemTypes.Thesis:
                    sb.Append("THES");
                    break;
                case ItemTypes.Patent:
                    sb.Append("PAT");
                    break;
                case ItemTypes.WebPage:
                    sb.Append("ICOMM");
                    break;
                default:
                    sb.Append("DATA");
                    break;
            }

            sb.AppendLine();

            //if(_ItemID >= 0)
            //    sb.AppendLine("ID - " + _ItemID);

            if (_Title.Length > 0)
                sb.AppendLine("TI  - " + _Title);

            for (int i = 0; i < Authors.Count; i++)
            {
                if (Authors[i].NameTypeID == NameTypes.Author)
                    sb.AppendLine("A1  - " + Authors[i].LastName + "," + Authors[i].ForeName);
                else if (Authors[i].NameTypeID == NameTypes.Editor)
                    sb.AppendLine("ED  - " + Authors[i].LastName + "," + Authors[i].ForeName);
                else if (Authors[i].NameTypeID == NameTypes.Publisher)
                {
                    if (_ItemTypeID == ItemTypes.Patent)
                        sb.AppendLine("A2  - " + Authors[i].LastName);
                    else
                        sb.AppendLine("PB  - " + Authors[i].LastName);
                }
            }

            if (_PubDate.Length > 0)
                sb.AppendLine("Y1  - " + _PubDate);

            if (_IssueDate.Length > 0)
                sb.AppendLine("Y2  - " + _IssueDate);

            if (_Title2.Length > 0)
            {
                if (_ItemTypeID == ItemTypes.JournalArticle)
                    sb.AppendLine("JF  - " + _Title2);
                else if (_ItemTypeID == ItemTypes.Proceeding)
                    sb.AppendLine("T3  - " + _Title2);
                else
                    sb.AppendLine("T2  - " + _Title2);
            }

            if (_Abstract.Length > 0)
                sb.AppendLine("N2  - " + _Abstract);

            if (_Notes.Length > 0)
                sb.AppendLine("N1  - " + _Notes);

            if (_Keywords.Length > 0)
            {
                string[] temp = _Keywords.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < temp.Length; i++)
                    sb.AppendLine("KW  - " + temp[i]);
            }

            if (_Tags.Length > 0)
            {
                string[] temp = _Tags.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < temp.Length; i++)
                    sb.AppendLine("TG  - " + temp[i]);
            }

            if (_Pages.Length > 0)
            {
                string[] temp = _Pages.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length > 0)
                    sb.AppendLine("SP  - " + temp[0]);
                if (temp.Length > 1)
                    sb.AppendLine("EP  - " + temp[1]);
            }

            if (_Volume.Length > 0)
            {
                sb.AppendLine("VL  - " + _Volume);
            }

            if (_Volume2.Length > 0)
                sb.AppendLine("IS  - " + _Volume2);

            if (_PubPlace.Length > 0)
                sb.AppendLine("CY  - " + _PubPlace);

            if (_ID1.Length > 0)
                sb.AppendLine("SN  - " + _ID1);

            if (_Links.Length > 0)
            {
                string[] temp = _Links.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < temp.Length / 2; i++)
                    sb.AppendLine("UR  - " + temp[i * 2 + 1]);

                sb.AppendLine("LK  - " + _Links);
            }

            if (_AttachedFiles.Length > 0)
                sb.AppendLine("AF  - " + _AttachedFiles);

            if (_ItemFile.Length > 0)
                sb.AppendLine("IF  - " + _ItemFile);

            if (_Affiliation.Length > 0)
            {
                if (_ItemTypeID == ItemTypes.WebPage)
                    sb.AppendLine("UR  - " + _Affiliation);
                else
                    sb.AppendLine("AD  - " + _Affiliation);
            }

            if (_DOI.Length > 0)
                sb.AppendLine("DOI  - " + _DOI);

            if (_Source.Length > 0)
                sb.AppendLine("SRC  - " + _Source);

            if (_ID2.Length > 0)
            {
                if (_ItemTypeID == ItemTypes.Patent)
                    sb.AppendLine("VL  - " + _ID2);
                else
                    sb.AppendLine("ID2  - " + _ID2);
            }

            if (_Date3.Length > 0)
                sb.AppendLine("Y3  - " + _Date3);

            if (_Edition.Length > 0)
                sb.AppendLine("EDI  - " + _Edition);

            sb.AppendLine("FG  - " + (int)_FlagID);

            sb.AppendLine("ER  - ");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
