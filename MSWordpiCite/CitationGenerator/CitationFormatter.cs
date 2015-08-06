using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSWordpiCite.Enums;
using MSWordpiCite.Classes;
using MSWordpiCite.Entities;
using MSWordpiCite.Tools;

namespace MSWordpiCite.CitationGenerator
{
    public class CitationFormatter
    {
        private ItemTypes _ItemTypeID;
        public ItemTypes ItemTypeID
        {
            get
            {
                return _ItemTypeID;
            }
            set
            {
                _ItemTypeID = value;
            }
        }

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

        private ReferenceFields _Fields;
        public ReferenceFields Fields
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

        private Logger log = Globals.ThisAddIn.log;

        public CitationFormatter()
        {}
        public string Format(ItemMasterRow item, bool bInHtmlFormat)
        {
            string text = string.Empty;
            try
            {
                if (this.ItemTypeID == item.ItemTypeID)
                {
                    for (int i = 0; i < this.Template.Count; i++)
                    {
                        object field = Fields.GetType().GetField(this.Template[i]).GetValue(Fields);
                        text += field.GetType().GetMethod("Format").Invoke(field, new object[] { item, bInHtmlFormat });
                    }
                }
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "CitationFormatter::Format", ex.ToString() + " ItemID: " + item.ItemID + " ItemTitle:" + item.Title);
            }            
            return text;
        }
    }

    public class ReferenceFields
    {
        #region Fields

        public Author Author;
        public Editor Editor;
        public Publisher Publisher;
        public Inventor Inventor;
        public Assignee Assignee;
        public ConfDate ConfDate;
        public SubmitDate SubmitDate;
        public DateFiled DateFiled;
        public PubDate PubDate;
        public IssueDate IssueDate;
        public AccessDate AccessDate;
        public Year Year;
        public Number Number;
        public Pages Pages;
        public Title Title;
        public JournalName JournalName;
        public ConfName ConfName;
        public ConfLocation ConfLocation;
        public Volume Volume;
        public Issue Issue;
        public Abstract Abstract;
        public ISSN ISSN;
        public DOI DOI;
        public PubmedID PubmedID;
        public BookTitle BookTitle;
        public Edition Edition;
        public City City;
        public Degree Degree;
        public School School;
        public Department Department;
        public Country Country;
        public PatentNumber PatentNumber;
        public ApplicationNumber ApplicationNumber;
        public Link Link;

        #endregion

        public ReferenceFields()
        {}
    }
}
