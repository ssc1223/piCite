using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MSWordpiCite.Controls;
using MSWordpiCite.Entities;
using MSWordpiCite.Classes;
using System.Threading;
using MSWordpiCite.Enums;
using MSWordpiCite.Search;

namespace MSWordpiCite.Threads
{
    class MasterControlThread
    {
        private MasterControl mastercontrol;

        public MasterControlThread(MasterControl master)
        {
            this.mastercontrol = master;
        }

        public void LoadFolders()
        {
            try
            {
                IEnumerable<Folder> folders = Globals.ThisAddIn.user.LoadFolders();
                mastercontrol.Invoke(mastercontrol.delegateLoadingFolderFinished, new Object[] { folders });
            }
            catch { }            
        }
        public void LoadColleagues(object obj)
        {
            try
            {
                TreeNode currNode = (TreeNode) obj;
                IEnumerable<Colleague> colleagues = Globals.ThisAddIn.user.LoadColleagues();
                mastercontrol.Invoke(mastercontrol.delegateLoadingColleaguesFinished, new Object[] { colleagues, currNode });
            }
            catch{}
        }
        public void FilterItems(object obj)
        {
            try
            {
                Thread.Sleep(300);
                Dictionary<string, object> data = (Dictionary<string, object>)obj;
                IEnumerable<ItemMasterRow> itemList = (IEnumerable<ItemMasterRow>)data["ItemList"];
                string filter = (string)data["Filter"];
                List<ItemMasterRow> items = new List<ItemMasterRow>();
                foreach (ItemMasterRow item in itemList)
                {
                    string strFullText = prepareFullText(item);
                    filter = Regex.Replace(filter, @"[^\w\d\s]", "");
                    string[] terms = Regex.Split(filter, @"\s+");
                    if (stringValidated(strFullText, terms))
                    {
                        items.Add(item);
                    }
                }
                mastercontrol.Invoke(mastercontrol.delegateLoadingItemsFinished, new Object[]{items, true, });
            }
            catch { }
        }
        public void LoadItems(object obj)
        {
            try
            {
                int folderID = (int)obj;
                List<ItemMasterRow> items = new List<ItemMasterRow>();
                int totalItems = Globals.ThisAddIn.user.GetItemCount(folderID);
                if(totalItems > 0)
                {
                    int iLoadingCount = totalItems / Properties.Settings.Default.DEFAULT_NUMOFITEMS_PER_LOAD + 1;
                    for (int i = 0; i < iLoadingCount; i++)
                    {
                        int start = i * Properties.Settings.Default.DEFAULT_NUMOFITEMS_PER_LOAD;
                        int count = Properties.Settings.Default.DEFAULT_NUMOFITEMS_PER_LOAD;
                        List<ItemMasterRow> _items = Globals.ThisAddIn.user.LoadItemsInRange(folderID, start, count);
                        items.AddRange(_items);
                        mastercontrol.Invoke(mastercontrol.delegateLoadingItemsProgress, new Object[] { items.Count, totalItems });
                    }
                }                                    
                mastercontrol.Invoke(mastercontrol.delegateLoadingItemsFinished, new Object[] { items, false });
            }
            catch { }
        }
        public void LoadColleagueItems(object obj)
        {
            try
            {
                List<ItemMasterRow> items = new List<ItemMasterRow>();
                Dictionary<string, object> data = (Dictionary<string, object>) obj;                
                Colleague colleague = (Colleague)data["Colleague"];
                string folderid = (string)data["FolderID"];
                int totalItems = Globals.ThisAddIn.user.GetColleagueItemCount(colleague.ServerAddress, colleague.UserID, folderid);
                if (totalItems > 0)
                {
                    int iLoadingCount = totalItems / Properties.Settings.Default.DEFAULT_NUMOFITEMS_PER_LOAD + 1;
                    for (int i = 0; i < iLoadingCount; i++)
                    {
                        int start = i * Properties.Settings.Default.DEFAULT_NUMOFITEMS_PER_LOAD;
                        int count = Properties.Settings.Default.DEFAULT_NUMOFITEMS_PER_LOAD;
                        List<ItemMasterRow> _items = Globals.ThisAddIn.user.LoadColleagueItemsInRange(colleague.ServerAddress, colleague.UserID, folderid, start, count);
                        items.AddRange(_items);
                    }
                }
                mastercontrol.Invoke(mastercontrol.delegateLoadingItemsFinished, new Object[] { items, false });
            }
            catch { }
        }
        public void DocumentFocus()
        {
            try
            {
                Globals.ThisAddIn.Application.ActiveDocument.ActiveWindow.SetFocus();
                mastercontrol.Invoke(mastercontrol.delegateFocusFinished, new Object[] { });
            }
            catch
            { }
        }
        public void SearchItems(object obj)
        {
            try
            {
                Dictionary<string, string> data = (Dictionary<string, string>)obj;
                List<ItemMasterRow> items = new List<ItemMasterRow>();
                string query = data["query"];
                SearchOptions option = (SearchOptions) int.Parse(data["source"]);
                int start = int.Parse(data["start"]);
                Dictionary<string, object> results = new Dictionary<string, object>();
                switch(option)
                {
                    case SearchOptions.Filter:
                        break;
                    case SearchOptions.PubMed:
                        PubMed pubmed = new PubMed(query);
                        results = pubmed.GetResults(start);
                        items = (List<ItemMasterRow>)results["ItemList"];
                        break;
                    case SearchOptions.ScholarsPortal:
                        ScholarsPortal sp = new ScholarsPortal(query);
                        results = sp.GetResults(start);
                        items = (List<ItemMasterRow>)results["ItemList"];
                        break;
                    case SearchOptions.CiteULike:
                        CiteULike cul = new CiteULike(query);
                        results = cul.GetResults(start);
                        items = (List<ItemMasterRow>)results["ItemList"];
                        break;
                    case SearchOptions.WorldCat:
                        WorldCat wc = new WorldCat(query);
                        results = wc.GetResults(start);
                        items = (List<ItemMasterRow>)results["ItemList"];
                        break;
                    case SearchOptions.GoogleScholar:
                        GoogleScholar gs = new GoogleScholar(query);
                        results = gs.GetResults(start);
                        items = (List<ItemMasterRow>)results["ItemList"];
                        break;
                    case SearchOptions.WizFolioSearch:
                        WizFolioSearch wfs = new WizFolioSearch(query);
                        items = wfs.GetResults(start);
                        break;

                }                
                mastercontrol.Invoke(mastercontrol.delegateSearchingItemsFinished, new Object[] { items });
            }
            catch { }
        }

        private bool stringValidated(string text, string[] terms)
        {
            foreach (string term in terms)
            {
                if (!Regex.Match(text, term, RegexOptions.IgnoreCase).Success)
                    return false;
            }
            return true;
        }
        private string prepareFullText(ItemMasterRow item)
        {
            StringBuilder sb = new StringBuilder();
            int iFilterFields = Properties.Settings.Default.DEFAULT_FILTER_FIELDS;
            if ((iFilterFields & (int)FilterFields.Title) == (int)FilterFields.Title)
                sb.Append(item.Title + " ");
            if ((iFilterFields & (int)FilterFields.Title2) == (int)FilterFields.Title2)
                sb.Append(item.Title2 + " ");
            if ((iFilterFields & (int)FilterFields.Authors) == (int)FilterFields.Authors)
                sb.Append(item.Author + " ");
            if ((iFilterFields & (int)FilterFields.Abstract) == (int)FilterFields.Abstract)
                sb.Append(item.Abstract + " ");
            if ((iFilterFields & (int)FilterFields.Keywords) == (int)FilterFields.Keywords)
                sb.Append(item.Keywords + " ");
            if ((iFilterFields & (int)FilterFields.Notes) == (int)FilterFields.Notes)
                sb.Append(item.Notes + " ");
            if ((iFilterFields & (int)FilterFields.PubDate) == (int)FilterFields.PubDate)
            {
                sb.Append(item.PubDate + " ");
                if(item.ItemTypeID == ItemTypes.Patent)
                    sb.Append(item.IssueDate + " " + item.Date3 + " ");
            }
            if ((iFilterFields & (int)FilterFields.Tags) == (int)FilterFields.Tags)
                sb.Append(item.Tags + " ");
            return sb.ToString();
        }
    }
}
