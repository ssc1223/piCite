using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSWordpiCite.Enums;
using MSWordpiCite.Entities;
using MSWordpiCite.Classes;
using MSWordpiCite.Tools;

namespace MSWordpiCite.CitationGenerator
{
    public class ListReferences
    {
        #region Properties

        private SortOrders _SortOrder;
        public SortOrders SortOrder
        {
            get
            {
                return _SortOrder;
            }
            set
            {
                _SortOrder = value;
            }
        }

        private string _Header;
        public string Header
        {
            get
            {
                return _Header;
            }
            set
            {
                _Header = value;
            }
        }

        private int _LineSpacing;
        public int LineSpacing
        {
            get
            {
                return _LineSpacing;
            }
            set
            {
                _LineSpacing = value;
            }
        }

        private Dictionary<string, CitationFormatter> _itemtypes;
        public Dictionary<string, CitationFormatter> ItemTypes
        {
            get
            {
                return _itemtypes;
            }
            set
            {
                _itemtypes = value;
            }
        }

        #endregion

        public ListReferences()
        {}
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            string text = string.Empty;            
            string itemtypes = item.ItemTypeID.ToString();
            if (item.ItemTypeID == Enums.ItemTypes.LectureNote)
                itemtypes = Enums.ItemTypes.Document.ToString();
            else if (item.ItemTypeID == Enums.ItemTypes.Video || item.ItemTypeID == Enums.ItemTypes.Audio)
                itemtypes = Enums.ItemTypes.WebPage.ToString();

            text = ItemTypes[itemtypes].Format(item, bInHtmlFormat);
            for (int i = 0; i < LineSpacing; i++)
            {
                if(bInHtmlFormat)
                    text += CitationTools.GetHtmlFormatString("", TextFormat.LineBreak);
                else
                    text += CitationTools.GetWordMLFormatString("", TextFormat.Paragraph);
            }
            if (bInHtmlFormat)
                text += CitationTools.GetHtmlFormatString("", TextFormat.LineBreak);
            return text;
        }
    }

    public class TypeOfItems
    {
        #region Variables

        public CitationFormatter JournalArticle;
        public CitationFormatter Proceeding;
        public CitationFormatter BookWhole;
        public CitationFormatter BookChapter;
        public CitationFormatter Thesis;
        public CitationFormatter Patent;
        public CitationFormatter Document;
        public CitationFormatter WebPage;

        #endregion

        public TypeOfItems()
        {}
    }
}
