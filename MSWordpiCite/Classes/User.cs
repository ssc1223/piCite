using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Net;
using System.Configuration;
using System.IO;
using System.Text;
using System.Data;
using System.ComponentModel;
using MSWordpiCite.Enums;
using MSWordpiCite.Entities;
using MSWordpiCite.CitationGenerator;
using MSWordpiCite.Tools;
using System.Text.RegularExpressions;
using MSWordpiCite.Controls;

namespace MSWordpiCite.Classes
{    
    public class User
    {
        #region Properties

        private int _userID;
        public int UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
            }
        }

        private string _serverAddress;
        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
            set
            {
                _serverAddress = value;
            }
        }
        
        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }

        private string _foreName;
        public string ForeName
        {
            get
            {
                return _foreName;
            }
            set
            {
                _foreName = value;
            }
        }

        private string _authentication;
        public string Authentication
        {
            get
            {
                return _authentication;
            }
            set
            {
                _authentication = value;
            }
        }

        private AccountType _accType;
        public AccountType AccType
        {
            get
            {
                return _accType;
            }
            set
            {
                _accType = value;
            }
        }

        private string localPhysicalPath;
        private Logger log = Globals.ThisAddIn.log;
        private MasterControl MasterControl;

        #endregion

        #region Initialization

        public User(string userid, string lastname, string forename, string authentication, string serveraddress, int iacctype)
        {
            UserID = int.Parse(userid);
            Authentication = authentication;
            ServerAddress = serveraddress;
            LastName = lastname;
            ForeName = forename;
            AccType = (AccountType)iacctype;
        }
        public User(int userid)
        {
            UserID = userid;
        }

        #endregion

        #region Public Functions

        public void DownloadFile(string strFileName)
        {
            DownloadFile(strFileName, null, null);
        }
        public void DownloadFile(string strFileName, MasterControl mastercontrol, string strRequestURL)
        {
            try
            {
                MasterControl = mastercontrol;
                if (!System.IO.Directory.Exists(Properties.Settings.Default.DEFAULT_FILES_FOLDER))
                {
                    try
                    {
                        string strLocalFileStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\πFolio";
                        System.IO.Directory.CreateDirectory(strLocalFileStoragePath);
                        Properties.Settings.Default.DEFAULT_FILES_FOLDER = strLocalFileStoragePath;
                        Properties.Settings.Default.Save();
                        localPhysicalPath = Properties.Settings.Default.DEFAULT_FILES_FOLDER + @"\" + Regex.Replace(strFileName, @"^\d+_\d+_|_$", string.Empty);
                    }
                    catch
                    {
                        if(strRequestURL == null)
                        {
                            localPhysicalPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + @"\" + Regex.Replace(strFileName, @"^\d+_\d+_|_$", string.Empty);
                        }
                        else
                        {
                            localPhysicalPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + @"\Templates\" + Regex.Replace(strFileName, @"^\d+_\d+_|_$", string.Empty);
                        }
                    }
                }
                else
                {
                    if (strRequestURL == null)
                    {
	                    localPhysicalPath = Properties.Settings.Default.DEFAULT_FILES_FOLDER + @"\" + Regex.Replace(strFileName, @"^\d+_\d+_|_$", string.Empty);
                    }
                    else
                    {
                        localPhysicalPath = Properties.Settings.Default.DEFAULT_FILES_FOLDER + @"\Templates\" + Regex.Replace(strFileName, @"^\d+_\d+_|_$", string.Empty);

                    }
                }

                if (strRequestURL == null)
                {
                    strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=PDF&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication + "&file=" + strFileName;
                }
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(fileCompletedHandler);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                webClient.DownloadFileAsync(new Uri(strRequestURL), localPhysicalPath);
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::DownloadFile", ex.ToString());
            }
        }
        public IEnumerable<Folder> LoadFolders()
        {
            IEnumerable<Folder> enumFolders = null;            
            try
            {
                List<Folder> listFolders = new List<Folder>();
                string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=ownfolder&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication="+Authentication;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                listFolders = js.Deserialize<List<Folder>>(req.GetResponse());                
                enumFolders = listFolders.OrderBy(Folder => Folder.Name);
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::LoadFolders", ex.ToString());
            }
            return enumFolders;
        }
        public IEnumerable<Folder> LoadColleagueFolders(string serveraddress, int userid, int parentid)
        {
            IEnumerable<Folder> enumFolders = null;
            try
            {
                List<Folder> listFolders = new List<Folder>();
                string strRequestURL = "http://" + serveraddress + "/" + Properties.Settings.Default.URI_LOCALSHARESERVICE + "?op=getsharedfoldersalt&targetid=" + userid + "&authentication=" + Authentication + "&parentid=" + parentid;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                listFolders = js.Deserialize<List<Folder>>(Regex.Replace(req.GetResponse(), @"^\(|\)$", ""));
                enumFolders = listFolders.OrderBy(Folder => Folder.Name);
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::LoadColleagueFolders", ex.ToString());
            }
            return enumFolders;
        }
        public IEnumerable<Colleague> LoadColleagues()
        {
            IEnumerable<Colleague> enumColleagues = null;            
            //string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER.Replace("http://", "https://") + Properties.Settings.Default.URI_ROOTDATAHANDLER + "?op=getallcolleagues&userid=" + UserID;
            string strRequestURL = Properties.Settings.Default.URL_ROOTSERVER + Properties.Settings.Default.URI_ROOTDATAHANDLER + "?op=getallcolleagues&userid=" + UserID;
            List<Colleague> listColleagues = new List<Colleague>();
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            listColleagues = js.Deserialize<List<Colleague>>(Regex.Replace(req.GetResponse(), @"^\(|\)$", ""));
            enumColleagues = listColleagues.OrderBy(Colleague => Colleague.ForeName);
            return enumColleagues;
        }
        public List<ItemMasterRow> LoadItems(int iFolderID)
        {
            List<ItemMasterRow> listItems = new List<ItemMasterRow>();            
            string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=items&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication + "&folderid=" + iFolderID;
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            listItems = js.Deserialize<List<ItemMasterRow>>(req.GetResponse());            
            return listItems;
        }
        public List<ItemMasterRow> LoadItemsInRange(int iFolderID, int iStart, int iCount)
        {
            List<ItemMasterRow> listItems = new List<ItemMasterRow>();
            string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=itemsinrange&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication + "&folderid=" + iFolderID +"&start=" + iStart + "&count=" + iCount;            
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            listItems = js.Deserialize<List<ItemMasterRow>>(req.GetResponse());
            return listItems;
        }
        public List<ItemMasterRow> LoadColleagueItems(string serveraddress, int userid, string folderid)
        {
            List<ItemMasterRow> listItems = new List<ItemMasterRow>();            
            string strRequestURL = "http://" + serveraddress + "/" + Properties.Settings.Default.URI_LOCALSHARESERVICE + "?op=getitemsbyfolderalt&targetid=" + userid + "&authentication=" + Authentication + "&folderid=" + folderid;
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            listItems = js.Deserialize<List<ItemMasterRow>>(Regex.Replace(req.GetResponse(), @"^\(|\)$", ""));            
            return listItems;
        }
        public List<ItemMasterRow> LoadColleagueItemsInRange(string serveraddress, int userid, string folderid, int start, int count)
        {
            List<ItemMasterRow> listItems = new List<ItemMasterRow>();
            string strRequestURL = "http://" + serveraddress + "/" + Properties.Settings.Default.URI_LOCALSHARESERVICE + "?op=getitemsbyfolder&targetid=" + userid + "&authentication=" + Authentication + "&folderid=" + folderid + "&from=" + start + "&count=" + count + "&sortorder=0&direction=0&filterfield=2&filterby=&noabstract=false";
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            listItems = js.Deserialize<List<ItemMasterRow>>(Regex.Replace(req.GetResponse(), @"^\(|\)$", ""));
            return listItems;
        }
        public List<string> LoadUserCitationStyleNames()
        {
            List<string> listStyles = new List<string>();
            try
            {
                string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=userstylenames&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                listStyles = js.Deserialize<List<string>>(req.GetResponse());
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::LoadUserCitationStyleNames", ex.ToString());
            }
            return listStyles;
        }
        public List<string> LoadOfficialStyleNames()
        {
            List<string> listStyles = new List<string>();
            try
            {
                string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=officialstylenames&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                listStyles = js.Deserialize<List<string>>(req.GetResponse());
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::LoadOfficialStyleNames", ex.ToString());
            }
            return listStyles;
        }
        public List<string> LoadAllOfficialStyleNames()
        {
            List<string> listStyles = new List<string>();
            try
            {
                string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=allofficialstylenames&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                listStyles = js.Deserialize<List<string>>(req.GetResponse());
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::LoadOfficialStyleNames", ex.ToString());
            }
            return listStyles;
        }
        public CitationStyle LoadStyle(StyleOwner owner, string name)
        {
            string type = (owner == StyleOwner.Public) ? "official" : "user";
            CitationStyle style = new CitationStyle();
            string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=style&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication + "&type=" + type + "&name=" + name;
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            style = js.Deserialize<CitationStyle>(req.GetResponse());
            return style;
        }
        public bool CheckStyleAccess(string name)
        {
            string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=styleaccess&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication + "&name=" + name;
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            return (req.GetResponse() == "1");            
        }
        public ItemMasterRow GetItem(string serveraddress, string userid, int itemid, string accesscode)
        {
            ItemMasterRow item = null;
            if(Globals.ThisAddIn.GetItem(ref item, itemid))
            {
                return item;
            }
            try
            {
                string strRequestURL = "http://" + serveraddress + "/" + Properties.Settings.Default.URI_LOCALSHARESERVICE + "?op=getitem&targetid=" + userid + "&itemid=" + itemid + "&accesscode=" + accesscode + "&authentication=" + Authentication;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                item = js.Deserialize<ItemMasterRow>(Regex.Replace(req.GetResponse(), @"^\(|\)$", ""));
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::LoadItems", ex.ToString());
            }
            return item;
        }
        public int GetItemCount(int iFolderID)
        {
            int count = 0;
            string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=itemscount&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication + "&folderid=" + iFolderID;
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            count = int.Parse(req.GetResponse());
            return count;
        }
        public int GetColleagueItemCount(string serveraddress, int userid, string folderid)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            string strRequestURL = "http://" + serveraddress + "/" + Properties.Settings.Default.URI_LOCALSHARESERVICE + "?op=getfolderinfo&targetid=" + userid + "&authentication=" + Authentication + "&folderid=" + folderid + "&sortorder=0&direction=0&filterfield=2&filterby=";
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
            JavaScriptSerializer js = new JavaScriptSerializer();
            string serial = Regex.Replace(req.GetResponse(), @"^\(""|""\)$", "");
            data = js.Deserialize<Dictionary<string, object>>(Regex.Replace(serial, @"\\", ""));
            if (data.ContainsKey("Count"))
                return (int)data["Count"];
            else
                return 0;
        }
        public List<ItemMasterRow> AddItems(object listItems)
        {
            string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=additems&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strPostData = "items=" + HttpUtility.UrlEncode(js.Serialize(listItems));
            CustomHttpRequest req = new CustomHttpRequest(strRequestURL, strPostData);
            List<ItemMasterRow> items = new List<ItemMasterRow>();
            items = js.Deserialize<List<ItemMasterRow>>(req.GetResponse());
            return items;
        }
        public List<ItemMasterRow> SearchWizFolioCollection(string terms, int start, int count)
        {
            List<ItemMasterRow> listItems = null;
            try
            {
                string strRequestURL = ServerAddress + Properties.Settings.Default.URI_LOCALDATASERVICE + "?op=wizfoliosearch&from=" + DeviceTypes.MSWordWin.ToString() + "&authentication=" + Authentication + "&terms=" + HttpUtility.UrlEncode(terms) + "&start=" + start + "&count=" + count;
                CustomHttpRequest req = new CustomHttpRequest(strRequestURL);
                JavaScriptSerializer js = new JavaScriptSerializer();
                listItems = js.Deserialize<List<ItemMasterRow>>(req.GetResponse());
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::SearchWizFolioCollection", ex.ToString());
            }
            return listItems;
        }
        #endregion

        #region Handlers

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                if(MasterControl != null)
                {
                    MasterControl.Invoke(MasterControl.delegateDownloadingProgressChanged, new Object[] { e.ProgressPercentage });
                }
            }
            catch(Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::webClient_DownloadProgressChanged", ex.ToString());
            }
        }
        private void fileCompletedHandler(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(localPhysicalPath);
                if (MasterControl != null)
                {
                    MasterControl.Invoke(MasterControl.delegateFileDownloaded, new Object[] { });
                    MasterControl = null;
                }
            }
            catch (Exception ex)
            {
                this.log.WriteLine(LogType.Error, "User::fileCompletedHandler", ex.ToString());
            }
        }

        #endregion
    }
}