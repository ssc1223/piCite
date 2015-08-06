using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSWordpiCite.Entities;
using MSWordpiCite.Enums;
using MSWordpiCite.Classes;
using MSWordpiCite.Tools;

namespace MSWordpiCite.CitationGenerator
{
    public class InTextCitation
    {
        #region Properties

        private List<string> _Template;
        public List<string> Template
        {
            get
            {
                return _Template;
            }
            set
            {
                _Template = value;
            }
        }

        private EnclosureType _Enclosure;
        public EnclosureType Enclosure
        {
            get
            {
                return _Enclosure;
            }
            set
            {
                _Enclosure = value;
            }
        }

        private string _Separator;
        public string Separator
        {
            get
            {
                return _Separator;
            }
            set
            {
                _Separator = value;
            }
        }

        private bool _Superscript;
        public bool Superscript
        {
            get
            {
                return _Superscript;
            }
            set
            {
                _Superscript = value;
            }
        }

        private bool _UseNumberRange;
        public bool UseNumberRange
        {
            get
            {
                return _UseNumberRange;
            }
            set
            {
                _UseNumberRange = value;
            }
        }

        private InTextFields _Fields;
        public InTextFields Fields
        {
            get
            {
                return _Fields;
            }
            set
            {
                _Fields = value;
            }
        }

        #endregion

        public InTextCitation()
        {}
        public string Format(ItemMasterRow item, bool bInHtmlFormat, bool bIsSuperscript)
        {
            string intext = string.Empty;
            for(int i=0; i<Template.Count; i++)
            {
                string temp = string.Empty;
                switch(Template[i].ToLower())
                {
                    case "author":
                        temp = Fields.Author.Format(item, bInHtmlFormat);
                        break;
                    case "year":
                        temp = Fields.Year.Format(item, bInHtmlFormat);
                        break;
                    case "number":
                        temp = Fields.Number.Format(item, bInHtmlFormat);
                        break;
                    case "pages":
                        temp = Fields.Pages.Format(item, bInHtmlFormat);
                        break;
                }
                if (bIsSuperscript)
                {
                    if(bInHtmlFormat)
                        temp = CitationTools.GetHtmlFormatString(temp, TextFormat.Superscript);
                    else
                        temp = CitationTools.GetWordMLFormatString(temp, TextFormat.Superscript);
                }

                intext += temp;
            }
            return intext;
        }
    }

    public class InTextFields
    {
        #region Properties

        private Author _Author;
        public Author Author
        {
            get
            {
                return _Author;
            }
            set
            {
                _Author = value;
            }
        }

        private Year _Year;
        public Year Year
        {
            get
            {
                return _Year;
            }
            set
            {
                _Year = value;
            }
        }

        private Number _Number;
        public Number Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
            }
        }

        private Pages _Pages;
        public Pages Pages
        {
            get
            {
                return _Pages;
            }
            set
            {
                _Pages = value;
            }
        }

        #endregion

        public InTextFields()
        {
            Author = new Author();
            Year = new Year();
            Number = new Number();
            Pages = new Pages();
        }
    }
}
