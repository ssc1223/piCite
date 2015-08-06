using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using MSWordpiCite.Forms;
using MSWordpiCite.Classes;
using System.Threading;
using MSWordpiCite.Tools;

namespace MSWordpiCite.Threads
{
    class StyleControlThread
    {
        private CitationStyleWindow citationstyleWindow;

        public StyleControlThread(CitationStyleWindow styleWindow)
        {
            this.citationstyleWindow = styleWindow;
        }

        public void LoadStyles(object obj)
        {
            try
            {
                Dictionary<string, object> data = (Dictionary<string, object>) obj;
                bool bFiltering = (bool) data["Filtering"];
                List<Dictionary<string, string>> listStyleNames = new List<Dictionary<string, string>>();
                if (bFiltering)
                {
                    Thread.Sleep(250);
                    string strFilter = (string)data["FilterString"];
                    List<Dictionary<string, string>> listStyles = (List<Dictionary<string, string>>) data["Styles"];
                    foreach (Dictionary<string, string> style in listStyles)
                    {
                        strFilter = Regex.Replace(strFilter, @"[^\w\d\s]", "");
                        string[] terms = Regex.Split(strFilter, @"\s+");
                        if (stringValidated(style["StyleName"], terms))
                        {
                            listStyleNames.Add(style);
                        }
                    }
                }
                else
                {
                    List<StyleInformation> listFavoriteStyles = loadFavoriteStyleNames();
                    List<string> listUserStyleNames = Globals.ThisAddIn.user.LoadUserCitationStyleNames();
                    List<string> listOfficialStyleNames = Globals.ThisAddIn.user.LoadOfficialStyleNames();
                    listStyleNames = new List<Dictionary<string, string>>();
                    for (int i = listFavoriteStyles.Count - 1; i >= 0; i--)
                    {
                        Dictionary<string, string> Data = new Dictionary<string, string>();
                        Data["Type"] = "fav";
                        Data["StyleName"] = listFavoriteStyles[i].StyleName;
                        listStyleNames.Add(Data);
                    }
                    foreach (string style in listUserStyleNames)
                    {
                        Dictionary<string, string> Data = new Dictionary<string, string>();
                        Data["Type"] = "user";
                        Data["StyleName"] = style;
                        listStyleNames.Add(Data);
                    }
                    foreach (string style in listOfficialStyleNames)
                    {
                        Dictionary<string, string> Data = new Dictionary<string, string>();
                        Data["Type"] = "official";
                        Data["StyleName"] = style;
                        listStyleNames.Add(Data);
                    }
                }                
                this.citationstyleWindow.Invoke(citationstyleWindow.delegateLoadStyleFinished, new Object[] { listStyleNames, bFiltering });
            }
            catch { }
        }

        public void LoadAllStyles()
        {
            try
            {
                List<Dictionary<string, string>> listStyleNames = new List<Dictionary<string, string>>();
                List<StyleInformation> listFavoriteStyles = loadFavoriteStyleNames();
                List<string> listUserStyleNames = Globals.ThisAddIn.user.LoadUserCitationStyleNames();
                List<string> listAllOfficialStyleNames = Globals.ThisAddIn.user.LoadAllOfficialStyleNames();
                listStyleNames = new List<Dictionary<string, string>>();
                for (int i = listFavoriteStyles.Count - 1; i >= 0; i--)
                {
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    Data["Type"] = "fav";
                    Data["StyleName"] = listFavoriteStyles[i].StyleName;
                    listStyleNames.Add(Data);
                }
                foreach (string style in listUserStyleNames)
                {
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    Data["Type"] = "user";
                    Data["StyleName"] = style;
                    listStyleNames.Add(Data);
                }
                foreach (string style in listAllOfficialStyleNames)
                {
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    Data["Type"] = "official";
                    Data["StyleName"] = style;
                    listStyleNames.Add(Data);
                }
                this.citationstyleWindow.Invoke(citationstyleWindow.delegateLoadStyleFinished, new Object[] { listStyleNames, false });
            }
            catch{}            
        }

        private List<StyleInformation> loadFavoriteStyleNames()
        {
            List<StyleInformation> favoriteStyles = new List<StyleInformation>();
            JavaScriptSerializer sr = new JavaScriptSerializer();
            ListDictionary ld = null;
            try
            {
                ld = sr.Deserialize<ListDictionary>(Properties.Settings.Default.LAST_USED_CITATIONSTYLES);
            }
            catch { }
            if (ld != null)
            {
                IDictionaryEnumerator myEnumerator = ld.GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    StyleInformation styleInfo = new StyleInformation(int.Parse(myEnumerator.Value + ""), myEnumerator.Key + "");
                    if (styleInfo.UserID == Globals.ThisAddIn.user.UserID || styleInfo.UserID == -1)
                        favoriteStyles.Add(styleInfo);
                }
            }
            return favoriteStyles;
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
    }
}
