using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MSWordpiCite.Entities;
using MSWordpiCite.CitationGenerator;
using MSWordpiCite.Tools;
using MSWordpiCite.Classes;
using MSWordpiCite.Forms;
using MSWordpiCite.Controls;
using System.Web.Script.Serialization;
using Word = Microsoft.Office.Interop.Word;
using System.Threading;
using System.Collections.Specialized;
using System.Windows.Forms;
using MSWordpiCite.Enums;

namespace MSWordpiCite.Formatter
{
    public class DocumentFormatter
    {
        #region Variables

        public Dictionary<int, ItemMasterRow> listCurrentCitedReferences;
        private CitationStyle citationstyle;
        private DocumentEditor editor;
        private ReferenceCitationItem rci;
        private bool bForceReload;
        private bool bShowedNoRefreshWarning = false;
        private bool bTooManyCitations = false;
        private List<int> listMasterRefItemIDs;        
        private List<int> listSearchingItemFieldIndexes;
        private int iCurrentSearchingItemCursor;
        private string currentSearchingItemHash = string.Empty;
        private Logger log = Globals.ThisAddIn.log;

        #endregion

        public DocumentFormatter()
        {
            citationstyle = null;
            bForceReload = false;
            editor = new DocumentEditor();
            listCurrentCitedReferences = new Dictionary<int, ItemMasterRow>();
        }

        #region Public Functions

        public void FindItem(ItemMasterRow item)
        {
            try
            {
                string hash = CitationTools.GetItemHash(item);
                if (currentSearchingItemHash != hash)
                {
                    listSearchingItemFieldIndexes = new List<int>();
                    foreach (TextCitationItem tci in rci.TextCitations)
                    {
                        if (tci.HashList.Contains(hash))
                        {
                            int index = tci.HashList.IndexOf(hash);
                            listSearchingItemFieldIndexes.Add(tci.FieldIndexes[index]);
                        }
                    }
                    currentSearchingItemHash = hash;
                    iCurrentSearchingItemCursor = 0;
                }
                if (listSearchingItemFieldIndexes.Count > iCurrentSearchingItemCursor)
                {
                    editor.JumpToNextMatchingItem(listSearchingItemFieldIndexes[iCurrentSearchingItemCursor]);
                    iCurrentSearchingItemCursor++;
                }
                else
                {
                    MessageBox.Show(Lang.en_US.Alert_NoMoreMatching_Msg);
                    iCurrentSearchingItemCursor = 0;
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::FindItem", ex.ToString());
            }
        }
        public void InsertCitations(List<ItemMasterRow> items)
        {
            try
            {
                currentSearchingItemHash = string.Empty;
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                string intext = string.Empty;
                bool bCited = false;
                for (int i = 0; i < items.Count; i++)
                {
                    string text = "{";
                    if (items[i].Authors.Count > 0)
                        text += items[i].Authors[0].LastName + ", ";
                    if (items[i].PubYear.Length > 0)
                        text += items[i].PubYear + ' ';
                    if (text.Length < 8 && items[i].Title.Length > 0)
                    {
                        if (items[i].Title.Length < 10)
                            text += items[i].Title;
                        else
                            text += items[i].Title.Substring(0, 10) + "...";
                    }
                    text += "}";
                    string url = String.Format(Properties.Settings.Default.FORMAT_URL_CITEDITEM, Properties.Settings.Default.URL_ROOTSERVER, items[i].ItemID, items[i].UserID, items[i].AccessCode);
                    editor.InsertCitation(text, url);
                    bCited = true;
                }

                if (bCited)
                {
                    if (bTooManyCitations)
                    {
                        if (!bShowedNoRefreshWarning)
                        {
                            MessageBox.Show(string.Format(Lang.en_US.Master_NoAutoRefresh, Properties.Settings.Default.DEFAULT_NUMOFCITATIONS_NOREFRESH).Replace("\\n", Environment.NewLine), Lang.en_US.Master_NoAutoRefresh_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bShowedNoRefreshWarning = true;
                        }
                    }
                    else
                    {
                        RefreshCitations(false);
                    }
                }
                else
                    MessageBox.Show(Lang.en_US.Alert_NoItemSelected_Msg);

                Globals.ThisAddIn.Application.ScreenRefresh();
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::InsertCitations", ex.ToString());
            }
        }
        public void ReappyStyleInThread(object obj)
        {
            try
            {
                Dictionary<string, object> data = (Dictionary<string, object>)obj;
                CitationStyleWindow master = (CitationStyleWindow)data["Control"];
                StyleInformation style = (StyleInformation)data["Style"];
                ReapplySytle(style);
                master.Invoke(master.delegateReapplyStyleFinished, new Object[] { });
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::ReappyStyleInThread", ex.ToString());
            }
        }
        public void InsertCitationsInThread(object obj)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)obj;
            MasterControl master = (MasterControl)data["Control"];
            List<ItemMasterRow> listItems = new List<ItemMasterRow>();
            if(data.ContainsKey("FromSearch"))
                listItems = Globals.ThisAddIn.user.AddItems(data["ListItems"]);
            else
                listItems = (List<ItemMasterRow>)data["ListItems"];
            InsertCitations(listItems);
            master.Invoke(master.delegateInsertFinished, new Object[] { });
        }
        public void RefreshCitationsInThread(object obj)
        {
            MasterControl master = (MasterControl)obj;
            RefreshCitations(true);
            master.Invoke(master.delegateRefreshFinished, new Object[] { });
        }
        public void RefreshCitations(bool bFlushed)
        {
            try
            {
                if (bFlushed)
                {
                    listMasterRefItemIDs = new List<int>();
                    listCurrentCitedReferences = new Dictionary<int, ItemMasterRow>();
                }
                rci = getRCI();
                if (rci.TextCitations.Count > 0)
                    refreshReferences();
                
                bTooManyCitations = rci.MasterRefList.Count > Properties.Settings.Default.DEFAULT_NUMOFCITATIONS_NOREFRESH;
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::RefreshCitations", ex.ToString());
            }            
        }                
        public void ReapplySytle(StyleInformation styleinfo)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                if (styleinfo != null)
                {
                    StyleOwner styleOwner = (styleinfo.UserID == -1) ? StyleOwner.Public : StyleOwner.User;                    
                    citationstyle = Globals.ThisAddIn.user.LoadStyle(styleOwner, styleinfo.StyleName);
                    citationstyle.OwnerUserID = styleinfo.UserID;
                }
                bForceReload = true;
                RefreshCitations(false);
                Globals.ThisAddIn.Application.ScreenRefresh();
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::ReapplySytle", ex.ToString());
            }            
        }

        #endregion

        #region Private Functions

        private void refreshReferences()
        {
            bool bUsingTrackChanges = Globals.ThisAddIn.Application.ActiveDocument.TrackRevisions;
            if (bUsingTrackChanges)
            {
                Globals.ThisAddIn.Application.ActiveDocument.TrackRevisions = false;
            }
            refreshCitedItem();
            try
            {
                if (rci.StyleInfo != null && !bForceReload)
                {
                    StyleOwner styleOwner = (rci.StyleInfo.UserID == -1) ? StyleOwner.Public : StyleOwner.User;
                    if (citationstyle != null)
                    {
                        if(citationstyle.Name != rci.StyleInfo.StyleName)
                        {
                            citationstyle = Globals.ThisAddIn.user.LoadStyle(styleOwner, rci.StyleInfo.StyleName);
                            citationstyle.OwnerUserID =rci.StyleInfo.UserID;
                        }
                    }
                    if(citationstyle == null)
                    {
                        citationstyle = Globals.ThisAddIn.user.LoadStyle(styleOwner, rci.StyleInfo.StyleName);
                        citationstyle.OwnerUserID =rci.StyleInfo.UserID;
                    }
                    citationstyle.GenerateCitation(rci, false);
                    rci.GetRCI(Properties.Settings.Default.URL_ROOTSERVER.Replace("http://", "https://"));
                    updateRCI(rci);

                    if (bUsingTrackChanges)
                        Globals.ThisAddIn.Application.ActiveDocument.TrackRevisions = true;
                    return;
                }
                else
                {
                    bForceReload = false;
                    if(citationstyle != null)
                    {
                        rci.StyleInfo = new StyleInformation(citationstyle.OwnerUserID, citationstyle.Name);
                    }
                    else
                    {
                        JavaScriptSerializer sr = new JavaScriptSerializer();
                        ListDictionary ld = null;
                        try
                        {
                            ld = sr.Deserialize<ListDictionary>(Properties.Settings.Default.LAST_USED_CITATIONSTYLES);
                        }
                        catch { }
                        if(ld != null)
                        {
                            String[] Keys = new String[ld.Keys.Count];
                            ld.Keys.CopyTo(Keys, 0);
                            rci.StyleInfo = new StyleInformation((int)ld[Keys[Keys.Length - 1]], Keys[Keys.Length - 1]);
                        }
                        else
                        {
                            rci.StyleInfo = new StyleInformation(-1, Properties.Settings.Default.DEFAULT_CITATION_STYLE_NAME);
                        }
                        citationstyle = Globals.ThisAddIn.user.LoadStyle(rci.StyleInfo.UserID == -1 ? StyleOwner.Public : StyleOwner.User, rci.StyleInfo.StyleName);
                        citationstyle.OwnerUserID = rci.StyleInfo.UserID;
                    }
                    
                    if(citationstyle != null)
                    {
                        if(citationstyle.Name.ToLower() == "apa (6th edition)")
                        {
                            citationstyle.Reference.ItemTypes["JournalArticle"].Fields.JournalName.NameFormat = JournalNameFormat.AsIs;
                            citationstyle.Reference.ItemTypes["JournalArticle"].Fields.JournalName.IsAPA = true;
                            rci.IsAPA = true;
                        }
                        citationstyle.GenerateCitation(rci, false);
                        rci.GetRCI(Properties.Settings.Default.URL_ROOTSERVER.Replace("http://", "https://"));
                        updateRCI(rci);

                        if (bUsingTrackChanges)
                            Globals.ThisAddIn.Application.ActiveDocument.TrackRevisions = true;
                        return;
                    }
                }
            }
             catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::refreshReferences", ex.ToString());
            }

            if (bUsingTrackChanges)
                Globals.ThisAddIn.Application.ActiveDocument.TrackRevisions = true;
        }
        private void refreshCitedItem()
        {
            try
            {
                List<string> user_servers = new List<string>();
                List<string> user_ids = new List<string>();
                if (rci.UserList.Count > 0)
                {
                    Dictionary<int, User>.KeyCollection keycoll = rci.UserList.Keys;
                    foreach (int key in keycoll)
                    {
                        user_ids.Add(rci.UserList[key].UserID + "");
                    }
                }
                JavaScriptSerializer sr = new JavaScriptSerializer();
                //string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER.Replace("http://", "https://") + Properties.Settings.Default.URI_ROOTDATAHANDLER + "?op=getusersserveraddress&userlist=" + sr.Serialize(user_ids);
                string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_ROOTDATAHANDLER + "?op=getusersserveraddress&userlist=" + sr.Serialize(user_ids);
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                user_servers = sr.Deserialize<List<string>>(Regex.Replace(req.GetResponse(), @"^\(|\)$", ""));
                for (int i = 0; i < user_ids.Count; i++)
                {
                    rci.UserList[int.Parse(user_ids[i])].ServerAddress = user_servers[i];
                }
                listMasterRefItemIDs = new List<int>();
                itemDownloader(0);
                quarantineReferenceList();
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::refreshCitedItem", ex.ToString());
            }
        }
        private void quarantineReferenceList()
        {
            try
            {
                if (listCurrentCitedReferences.Count > rci.MasterRefList.Count)
                {
                    for (int i = 0; i < listCurrentCitedReferences.Keys.Count; i++)
                    {
                        int key = listCurrentCitedReferences.Keys.ElementAt(i);
                        if (!listMasterRefItemIDs.Contains(key))
                            listCurrentCitedReferences.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::quarantineReferenceList", ex.ToString());
            }
        }
        private void itemDownloader(int index)
        {
            try
            {
                List<ItemMasterRow> masterRefList = rci.MasterRefList;
                if (index < masterRefList.Count)
                {
                    ItemMasterRow item = masterRefList[index];
                    listMasterRefItemIDs.Add(item.ItemID);
                    if (listCurrentCitedReferences.ContainsKey(item.ItemID))
                        masterRefList[index].Clone(listCurrentCitedReferences[item.ItemID]);
                    else
                    {
                        string strServerAddress = rci.UserList[int.Parse(item.UserID)].ServerAddress;
                        if (strServerAddress == null || strServerAddress.Length == 0)
                        {
                            rci.SetNotFound(item);
                            index--;
                            listMasterRefItemIDs.Remove(item.ItemID);
                        }
                        else
                        {
                            ItemMasterRow fullitem = Globals.ThisAddIn.user.GetItem(strServerAddress, item.UserID, item.ItemID, item.AccessCode);
                            if (fullitem == null)
                            {
                                if (rci.SetNotFound(item))
                                {
                                    index--;
                                    listMasterRefItemIDs.Remove(item.ItemID);
                                }
                            }
                            else
                            {
                                masterRefList[index].Clone(fullitem);
                                listCurrentCitedReferences.Add(masterRefList[index].ItemID, masterRefList[index]);
                            }
                        }
                    }
                    itemDownloader(index + 1);
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::itemDownloader", ex.ToString());
            }
        }
        private ReferenceCitationItem getRCI()
        {
            ReferenceCitationItem rci = new ReferenceCitationItem();
            try
            {
                rci.DocType = "MSWORD";
                TextCitationItem currtci = null;
                WordHyperLink prevlink = null;
                int fieldDelCount = 0;
                List<int> listDeletedFields = new List<int>();
                List<int> listNullURL = new List<int>();
                int iLinkCount = editor.GetLinkCount();
                for (int i = 1; i <= iLinkCount; i++)
                {
                    try
                    {
                        WordHyperLink link = editor.GetLinkAt(i);
                        if (link == null || link.URL == null)
                        {
                            listNullURL.Add(i);
                            fieldDelCount++;
                            prevlink = null;
                            continue;
                        }
                        else if (Regex.Match(link.URL, @"pifolio\.com", RegexOptions.IgnoreCase).Success)//from wizfolio to pifolio
                        {
                            if (Regex.Match(link.URL, @"\?citation\=1&", RegexOptions.IgnoreCase).Success)
                            {
                                string strRangeText = string.Empty;
                                if (prevlink == null)
                                    strRangeText = " ";
                                else if (prevlink != null && prevlink.End != link.Start)
                                    strRangeText = editor.GetTextInRange(prevlink.End, link.Start);

                                if (rci.TextCitations.Count == 0 || strRangeText.Length > 0)
                                {
                                    currtci = new TextCitationItem(rci);
                                    rci.TextCitations.Add(currtci);
                                }
                                Dictionary<string, object> obj = CitationTools.ParseQueryString(link.URL);
                                ItemMasterRow item = new ItemMasterRow();
                                item.UserID = obj["UserID"] + "";
                                item.ItemID = int.Parse(obj["ItemID"] + "");
                                item.AccessCode = obj["AccessCode"] + "";
                                currtci.AddItem(item, link.URL, i - fieldDelCount, "");
                            }
                            else if (Regex.Match(link.URL, @"\?style\=1&", RegexOptions.IgnoreCase).Success)
                            {
                                if (rci.FieldIndex == -1)
                                {
                                    rci.URL = link.URL;
                                    rci.FieldIndex = i - fieldDelCount;
                                    Dictionary<string, object> obj = CitationTools.ParseQueryString(link.URL);
                                    rci.StyleInfo = new StyleInformation(int.Parse(obj["UserID"] + ""), obj["StyleName"] + "");
                                    if (rci.StyleInfo.UserID != -1)
                                        rci.UserList[rci.StyleInfo.UserID] = new User(rci.StyleInfo.UserID);
                                }
                                else
                                {
                                    listDeletedFields.Add(i);
                                    fieldDelCount++;
                                }
                            }
                            else
                            {
                                prevlink = null;
                                continue;
                            }
                            prevlink = link;
                        }
                        else
                        {
                            prevlink = null;
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.WriteLine(LogType.Error, "DocumentFormatter::getRCI", ex.ToString());
                    }
                }
                fieldDelCount = 0;
                for (int i = 0; i < listNullURL.Count; i++)
                {
                    editor.RemoveHyperLink(listNullURL[i] - fieldDelCount);
                    fieldDelCount++;
                }
                for (int i = 0; i < listDeletedFields.Count; i++)
                {
                    editor.RemoveCitation(listDeletedFields[i] - fieldDelCount);
                    fieldDelCount++;
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::getRCI", ex.ToString());
            }            
            return rci;
        }
        private void exportOldCitations()
        {
            //Reserve
        }
        private void updateRCI(ReferenceCitationItem rci)
        {
            try
            {
                int index_lastfield = 0;
                int fieldDelCount = 0;
                TextCitationItem currtci = null;
                for (int i = 0; i < rci.TextCitations.Count; i++)
                {
                    currtci = rci.TextCitations[i];
                    for (int j = 0; j < currtci.FormatString.Count; j++)
                    {
                        int index = currtci.FieldIndexes[j];
                        string url = currtci.URLList[j];
                        string screentip = CitationTools.FormatCitationScreenTip(currtci.Items[j]);                       
                            
                        string formatstring = currtci.FormatString[j];
                        index_lastfield = index;
                        editor.UpdateLink(index - fieldDelCount, formatstring, url, screentip);
                    }
                    if (currtci.FormatString.Count < currtci.FieldIndexes.Count)
                    {
                        index_lastfield = currtci.FieldIndexes[currtci.FormatString.Count];
                        for (int j = 0; j < currtci.DuplicateCount; j++)
                        {
                            editor.RemoveCitation(index_lastfield - fieldDelCount);
                            fieldDelCount++;
                            index_lastfield++;
                        }
                    }
                    for (int j = 0; j < currtci.DeletedIndexes.Count; j++)
                    {
                        int indexNotFoundLink = currtci.DeletedIndexes[j];
                        editor.MissingCitation(indexNotFoundLink, "[missing]");
                        fieldDelCount++;
                    }
                }
                if (rci.FormatString.Length > 0)
                    editor.UpdateReferences(rci.FieldIndex - fieldDelCount, rci.FormatString, rci.URL);
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentFormatter::updateRCI", ex.ToString());
            }
        }

        #endregion
    }
}