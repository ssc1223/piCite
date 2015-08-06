using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MSWordpiCite.Entities;
using MSWordpiCite.Tools;
using MSWordpiCite.Classes;

namespace MSWordpiCite.CitationGenerator
{
    public class ReferenceCitationItem
    {
        private string _oldFormatString;
        public string OldFormatString
        {
            get
            {
                return _oldFormatString;
            }
            set
            {
                _oldFormatString = value;
            }
        }

        public bool IsAPA = false;

        public int FieldIndex;
        public string FormatString;
        public string DocType;
        public string URL;
        public List<TextCitationItem> TextCitations;
        public List<ItemMasterRow> MasterRefList;
        public List<string> MasterURLList;        
        public List<string> MasterHashList;
        public Dictionary<int, User> UserList;        
        public StyleInformation StyleInfo;

        public ReferenceCitationItem()
        {
            OldFormatString = string.Empty;
            FormatString = string.Empty;
            MasterURLList = new List<string>();
            MasterRefList = new List<ItemMasterRow>();
            TextCitations = new List<TextCitationItem>();
            UserList = new Dictionary<int, User>();
            MasterHashList = new List<string>();
            DocType = string.Empty;
            StyleInfo = null;
            FieldIndex = -1;
            URL = string.Empty;
        }
        public ReferenceCitationItem CreateRCI()
        {
            ReferenceCitationItem currrci = new ReferenceCitationItem();
            currrci.OldFormatString = this.OldFormatString;
            currrci.FormatString = this.FormatString;
            currrci.FieldIndex = this.FieldIndex;
            currrci.DocType = this.DocType;
            currrci.URL = this.URL;
            currrci.StyleInfo = this.StyleInfo;
            if (Regex.Match(this.URL, @"\?style\=(.*?)$", RegexOptions.IgnoreCase).Success)
            {
                Dictionary<string, object> obj = CitationTools.ParseQueryString(this.URL);
                currrci.StyleInfo = new StyleInformation(int.Parse(obj["UserID"] + ""), obj["StyleName"]+"");
                if(currrci.StyleInfo.UserID != -1)
                    currrci.UserList[currrci.StyleInfo.UserID] = new User(currrci.StyleInfo.UserID);
            }
            List<TextCitationItem> newtci = this.TextCitations;
            TextCitationItem currtci;
            for (int i = 0; i < newtci.Count; i++)
            {
                currtci = currrci.AddBlock();
                currtci.FormatString = newtci[i].FormatString;
                currtci.FieldIndexes = newtci[i].FieldIndexes;
                if(newtci[i].StartIndex != -1)
                {
                    currtci.StartIndex = newtci[i].StartIndex;
                }
                for(int j=0; j<newtci[i].URLList.Count; j++)
                {
                    Dictionary<string, object> obj = CitationTools.ParseQueryString(newtci[i].URLList[j]);
                    ItemMasterRow item = new ItemMasterRow();
                    item.UserID = obj["UserID"] + "";
                    item.ItemID = int.Parse(obj["ItemID"] + "");
                    item.AccessCode = obj["AccessCode"] + "";
                    currtci.AddItem(item, newtci[i].URLList[j], newtci[i].FieldIndexes[j], newtci[i].FormatString[j]);
                }
            }
            return currrci;
        }
        public void GetRCI(string urlRootServer)
        {
            this.URL = String.Format(Properties.Settings.Default.FORMAT_URL_CITATIONSTYLE, urlRootServer, this.StyleInfo.UserID, this.StyleInfo.StyleName);            
        }
        public string GetFormatString()
        {
            return this.FormatString;
        }
        public ItemMasterRow AddItem(ref ItemMasterRow item, string url)
        {
            string hash = CitationTools.GetItemHash(item);
            int index = this.MasterHashList.IndexOf(hash);
            if (index > -1)
                return this.MasterRefList[index];
            else
            {
                this.MasterRefList.Add(item);
                this.MasterURLList.Add(url);
                this.MasterHashList.Add(hash);
                this.UserList[int.Parse(item.UserID)] = new User(int.Parse(item.UserID));
                return item;
            }
        }
        public TextCitationItem AddBlock()
        {
            TextCitationItem tci = new TextCitationItem(this);
            this.TextCitations.Add(tci);
            return tci;
        }
        public bool SetNotFound(ItemMasterRow item)
        {
            string hash = CitationTools.GetItemHash(item);
            int index = this.MasterHashList.IndexOf(hash);
            if(index > -1)
            {
                this.MasterRefList.RemoveAt(index);
                this.MasterURLList.RemoveAt(index);
                this.MasterHashList.RemoveAt(index);
                foreach (TextCitationItem tci in this.TextCitations)
                    tci.SetNotFound(item);                    
                return true;
            }
            
            return false;
        }
    }
}
