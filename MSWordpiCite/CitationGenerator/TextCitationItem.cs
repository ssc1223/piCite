using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSWordpiCite.Tools;
using MSWordpiCite.Entities;
using MSWordpiCite.Classes;

namespace MSWordpiCite.CitationGenerator
{
    public class TextCitationItem
    {
        #region Properties

        public ReferenceCitationItem RCI;
        public List<string> OldFormatString;
        public List<string> FormatString;
        public List<string> URLList;
        public List<ItemMasterRow> Items;
        public List<string> HashList;
        public List<int> FieldIndexes;
        public List<int> DeletedIndexes;
        public int StartIndex;
        public int DuplicateCount;
        public int NotFoundCount;

        #endregion

        // TextCitationItem includes a group of Intext Citation e.g.: [1-3], with their URL, LinkIndexes, etc.
        public TextCitationItem(ReferenceCitationItem rci)
        {
            this.RCI = rci;
            this.OldFormatString = new List<string>();
            this.FormatString = new List<string>();
            this.URLList = new List<string>();
            this.Items = new List<ItemMasterRow>();
            this.HashList = new List<string>();
            this.FieldIndexes = new List<int>();
            this.DeletedIndexes = new List<int>();
            this.StartIndex = -1;
            this.DuplicateCount = 0;
            this.NotFoundCount = 0;
        }
        public TextCitationItem()
        {
            this.RCI = new ReferenceCitationItem();
            this.OldFormatString = new List<string>();
            this.FormatString = new List<string>();
            this.URLList = new List<string>();
            this.Items = new List<ItemMasterRow>();
            this.HashList = new List<string>();
            this.FieldIndexes = new List<int>();
            this.DeletedIndexes = new List<int>();
            this.StartIndex = -1;
            this.DuplicateCount = 0;
            this.NotFoundCount = 0;
        }

        // Add item in to the group of InText Citation
        public void AddItem(ItemMasterRow item, string url, int index, string formatstring)
        {
            item = this.RCI.AddItem(ref item, url);
            if(item != null)
            {
                string hash = CitationTools.GetItemHash(item);
                this.FieldIndexes.Add(index);
                if (this.HashList.Contains(hash))
                    this.DuplicateCount++;
                else
                {
                    this.Items.Add(item);
                    this.URLList.Add(url);
                    this.OldFormatString.Add(formatstring);
                    this.HashList.Add(hash);                    
                    this.FormatString.Add(formatstring);
                }
            }
        }
        public void SetNotFound(ItemMasterRow item)
        {
            string hash = CitationTools.GetItemHash(item);
            int index = this.HashList.IndexOf(hash);
            if(index > -1)
            {
                this.Items.RemoveAt(index);
                this.URLList.RemoveAt(index);
                this.HashList.RemoveAt(index);
                this.FormatString.RemoveAt(index);
                this.DeletedIndexes.Add(this.FieldIndexes[index]);
                this.FieldIndexes.RemoveAt(index);
                this.OldFormatString.RemoveAt(index);                
                this.NotFoundCount++;
            }
        }
    }
}
