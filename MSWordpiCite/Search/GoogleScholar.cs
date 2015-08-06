using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.IO;
using MSWordpiCite.Entities;
using MSWordpiCite.Enums;

namespace MSWordpiCite.Search
{
    class GoogleScholar : BaseClass
    {
        const string __USERAGENT = "Mozilla/5.0";
        const string GOOGLESCHOLAR_FINDBIBTEX_PATTERN = @"(?<=href=\"")/scholar\.bib.*?(?=\"")";
        const string GOOGLESCHOLAR_FINDMAINLINK_PATTERN = @"(?<=class=gs_rt.*?href=\"").*?(?=\"".*?div)";
        const string GOOGLESCHOLAR_CITEDBY_PATTERN = @"(<=href=>)/scholar\?cites=.*?(?="")";
        const string GOOGLESCHOLAR_FINDBRIEF_PATTERN = @"<span\s*class=gs_a.*?/span>(.*?)<span\s*class=gs_fl.*?/span>";
        const string GOOGLESCHOLAR_URL = @"http://scholar.google.com/scholar?start={start}&q={query}&hl=en";
        const string GOOGLESCHOLAR_FINDCOUNT_PATTERN = @"results.*?of\sabout\s<b>(.*?)</b>";
        const string GoogleCookieDefaultHex = "";

        private Dictionary<string, string> mappingLatex2UnicodeTable;
        private Dictionary<string, ItemTypes> mappingBibTex2WizFolio;
        private Dictionary<string, string> regexPatterns;

        public GoogleScholar(string query) : base(query) { }

        public GoogleScholar()
        {}

        public Dictionary<string, object> GetResults(int iStart)
        {
            if (SearchQuery == string.Empty || SearchQuery.Length == 0)
                return null;

            Dictionary<string, object> Data = new Dictionary<string, object>();

            try
            {                
                HttpWebRequest req = _PrepareRequest(iStart);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                string strResp = sr.ReadToEnd();                
                resp.Close();
                List<ItemMasterRow> listItems = new List<ItemMasterRow>();
                List<string> listBibTexLinks = _FindBibTexLinks(strResp);
                List<string> listMainLinks = _FindMainLinks(strResp);
                List<string> listBriefs = _FindBriefs(strResp);
                for (int i = 0; i < listBibTexLinks.Count; i++)
                {
                    ItemMasterRow item = GetResult(listBibTexLinks[i]);
                    item.Links += (listMainLinks.Count > i) ? ("Link to Source|" + listMainLinks[i] + "|") : "";
                    item.Abstract = (listBriefs.Count > i) ? listBriefs[i] : "";
                    listItems.Add(item);
                }
                Data["ItemList"] = listItems;
            }
            catch {}
            return Data;
        }

        public ItemMasterRow GetResult(string strBibTexURI)
        {
            ItemMasterRow item = new ItemMasterRow();
            if (strBibTexURI == string.Empty || strBibTexURI.Length == 0)
                return null;

            try
            {
                HttpWebRequest req = _PrepareBibTeXRequest(strBibTexURI);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                string strResp = sr.ReadToEnd();
                resp.Close();
                if (!_ProcessBibTexBlock(ref item, strResp))
                {
                    return null;
                }
            }
            catch {}
            return item;
        }

        private List<string> _FindBibTexLinks(string strHtml)
        {
            List<string> listBibTexLinks = new List<string>();
            Regex regexBib = new Regex(GOOGLESCHOLAR_FINDBIBTEX_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            MatchCollection colBibs = regexBib.Matches(strHtml);
            for (int i = 0; i < colBibs.Count; i++)
            {
                Match matchBibTeX = colBibs[i];
                string strBibURI = matchBibTeX.Value.Replace("&amp;", "&");
                listBibTexLinks.Add(strBibURI);
            }
            return listBibTexLinks;
        }

        private List<string> _FindMainLinks(string strHtml)
        {
            List<string> listMainLinks = new List<string>();
            Regex regexMainLink = new Regex(GOOGLESCHOLAR_FINDMAINLINK_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            MatchCollection colMainLinks = regexMainLink.Matches(strHtml);
            for (int i = 0; i < colMainLinks.Count; i++)
            {
                listMainLinks.Add(colMainLinks[i].Value);
            }
            return listMainLinks;
        }

        private List<string> _FindCitedByLinks(string strHtml)
        {
            List<string> listCitedByLinks = new List<string>();
            
            Regex regexCitedBy = new Regex(GOOGLESCHOLAR_CITEDBY_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            MatchCollection colCitedBys = regexCitedBy.Matches(strHtml);

            for (int i = 0; i < colCitedBys.Count; i++)
            {
                listCitedByLinks.Add(colCitedBys[i].Value);
            }
            return listCitedByLinks;
        }

        private List<string> _FindBriefs(string strHtml)
        {
            List<string> listBriefs = new List<string>();

            strHtml = Regex.Replace(strHtml, @"\n|\t|\r", "", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            MatchCollection colBriefs = Regex.Matches(strHtml, GOOGLESCHOLAR_FINDBRIEF_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            for (int i = 0; i < colBriefs.Count; i++)
            {
                listBriefs.Add(colBriefs[i].Groups[1].Value);
            }
            return listBriefs;
        }

        private List<string> _FindCitedByNumbers(string strHtml)
        {
            List<string> listCitedByNumbers = new List<string>();

            Regex regexCitedByNumber = new Regex(@"(?<=cited\sby\s).*?(?=<\/a)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            MatchCollection colCitedByNumbers = regexCitedByNumber.Matches(strHtml);

            for (int i = 0; i < colCitedByNumbers.Count; i++)
            {
                listCitedByNumbers.Add(colCitedByNumbers[i].Value);
            }
            return listCitedByNumbers;
        }
        
        private bool _ProcessBibTexBlock(ref ItemMasterRow item, string strBibTexBlock)
        {
            _InitializeBibTexParser();
            Dictionary<string, string> dataBibTex = new Dictionary<string, string>();
            try
            {

                if (_ParseBibTexBlock(ref dataBibTex, strBibTexBlock))
                {
                    item = new ItemMasterRow();

                    item.ItemTypeID = ItemTypes.JournalArticle;
                    if (dataBibTex.ContainsKey("ItemType"))
                        _ConvertBibTex2WizFolioItemType(ref item, dataBibTex["ItemType"].ToLower());

                    item.Title = dataBibTex.ContainsKey("Title") ? dataBibTex["Title"] : item.Title;
                    item.Title2 = dataBibTex.ContainsKey("Title2") ? dataBibTex["Title2"] : item.Title2;
                    item.Affiliation = dataBibTex.ContainsKey("Affiliation") ? dataBibTex["Affiliation"] : item.Affiliation;
                    item.Edition = dataBibTex.ContainsKey("Edition") ? dataBibTex["Edition"] : item.Edition;
                    item.Volume = dataBibTex.ContainsKey("Volume") ? dataBibTex["Volume"] : item.Volume;
                    item.Volume2 = dataBibTex.ContainsKey("Volume2") ? dataBibTex["Volume2"] : item.Volume2;
                    item.Abstract = dataBibTex.ContainsKey("Abstract") ? dataBibTex["Abstract"] : item.Abstract;
                    item.Notes = dataBibTex.ContainsKey("Notes") ? dataBibTex["Notes"] : item.Notes;
                    item.PubPlace = dataBibTex.ContainsKey("PubPlace") ? dataBibTex["PubPlace"] : item.PubPlace;
                    item.PubDate = dataBibTex.ContainsKey("PubDate") ? dataBibTex["PubDate"] : item.PubDate;
                    item.Pages = dataBibTex.ContainsKey("Pages") ? dataBibTex["Pages"] : item.Pages;
                    item.ID1 = dataBibTex.ContainsKey("ID1") ? dataBibTex["ID1"] : item.ID1;
                    item.Links = dataBibTex.ContainsKey("Links") ? ("Source|" + dataBibTex["Links"] + "|") : item.Links;
                    item.Keywords = dataBibTex.ContainsKey("Keywords") ? dataBibTex["Keywords"] : item.Keywords;
                    if (dataBibTex.ContainsKey("Authors"))
                    {
                        string[] Contributors = dataBibTex["Authors"].Split('|');
                        for (int i = 0; i < Contributors.Length - 1; i = i + 2)
                        {
                            NameMasterRow Contributor = new NameMasterRow();
                            Contributor.LastName = Contributors[i];
                            Contributor.ForeName = Contributors[i + 1];
                            Contributor.SequenceNo = i / 2;
                            item.Authors.Add(Contributor);
                        }
                    }
                    if (item.ItemTypeID == ItemTypes.BookWhole || item.ItemTypeID == ItemTypes.BookChapter)
                    {
                        if (dataBibTex.ContainsKey("Editors"))
                        {
                            string[] editors = dataBibTex["Editors"].Split('|');
                            for (int i = 0; i < editors.Length - 1; i = i + 2)
                            {
                                NameMasterRow editor = new NameMasterRow();
                                editor.NameTypeID = NameTypes.Editor;
                                editor.LastName = editors[i];
                                editor.ForeName = editors[i + 1];
                                editor.SequenceNo = i / 2;
                                item.Authors.Add(editor);
                            }
                        }
                    }
                    if (item.ItemTypeID == ItemTypes.BookWhole || item.ItemTypeID == ItemTypes.BookChapter || item.ItemTypeID == ItemTypes.Proceeding)
                    {
                        if (dataBibTex.ContainsKey("Publisher"))
                        {
                            NameMasterRow publisher = new NameMasterRow();
                            publisher.NameTypeID = NameTypes.Publisher;
                            publisher.LastName = dataBibTex["Publisher"];
                            publisher.ForeName = "";
                            item.Authors.Add(publisher);
                        }
                    }
                    item.Trim();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool _ParseBibTexBlock(ref Dictionary<string, string> dataBibTex, string strBibTexBlock)
        {
            RegexOptions rOptions = RegexOptions.IgnoreCase;
            string strBibTex = strBibTexBlock;
            try
            {
                strBibTex = Regex.Replace(strBibTex, regexPatterns["RemoveStartingChar"], "");

                Match m = Regex.Match(strBibTex, regexPatterns["StartEntry"], rOptions);
                if (m.Success)
                    dataBibTex["ItemType"] = Regex.Replace(m.Value, regexPatterns["ParseItemType"], "");

                m = Regex.Match(strBibTex, regexPatterns["Title"], rOptions);
                if (m.Success)
                    dataBibTex["Title"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Title2"], rOptions);
                if (m.Success)
                    dataBibTex["Title2"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Affiliation"], rOptions);
                if (m.Success)
                    dataBibTex["Affiliation"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Edition"], rOptions);
                if (m.Success)
                    dataBibTex["Edition"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Volume"], rOptions);
                if (m.Success)
                    dataBibTex["Volume"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Volume2"], rOptions);
                if (m.Success)
                    dataBibTex["Volume2"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Abstract"], rOptions);
                if (m.Success)
                    dataBibTex["Abstract"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Notes"], rOptions);
                if (m.Success)
                    dataBibTex["Notes"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["PubPlace"], rOptions);
                if (m.Success)
                    dataBibTex["PubPlace"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["PubDate"], rOptions);
                if (m.Success)
                    dataBibTex["PubDate"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Pages"], rOptions);
                if (m.Success)
                {
                    dataBibTex["Pages"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");
                    dataBibTex["Pages"] = Regex.Replace(dataBibTex["Pages"], @"\-\-", "-");
                }

                m = Regex.Match(strBibTex, regexPatterns["ID1"], rOptions);
                if (m.Success)
                    dataBibTex["ID1"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Links"], rOptions);
                if (m.Success)
                    dataBibTex["Links"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                m = Regex.Match(strBibTex, regexPatterns["Keywords"], rOptions);
                if (m.Success)
                {
                    string strKeywords = Regex.Replace(m.Value, regexPatterns["ParseField"], "");
                    string[] keywords = Regex.Split(strKeywords, @"\,");
                    dataBibTex["Keywords"] = "";
                    foreach (string keyword in keywords)
                        dataBibTex["Keywords"] += keyword + "|";
                }

                m = Regex.Match(strBibTex, regexPatterns["Author"], rOptions);
                if (m.Success)
                {
                    string strAuthors = Regex.Replace(m.Value, regexPatterns["ParseField"], "");
                    string[] authors = Regex.Split(strAuthors, regexPatterns["ParseAuthorField"]);
                    dataBibTex["Authors"] = "";
                    foreach (string author in authors)
                    {
                        string[] name = Regex.Split(author, @"\,");
                        if (name.Length == 1)
                        {
                            name = new string[2];
                            MatchCollection mc = Regex.Matches(author, @"\s+");
                            if (mc.Count > 0)
                            {
                                name[0] = author.Substring(mc[mc.Count - 1].Index + 1);
                                name[1] = author.Substring(0, mc[mc.Count - 1].Index);
                            }
                            else
                            {
                                name[0] = author;
                                name[1] = "";
                            }
                        }
                        dataBibTex["Authors"] += name[0] + "|" + name[1] + "|";
                    }
                }

                m = Regex.Match(strBibTex, regexPatterns["Editor"], rOptions);
                if (m.Success)
                {
                    string strEditors = Regex.Replace(m.Value, regexPatterns["ParseField"], "");
                    string[] editors = Regex.Split(strEditors, regexPatterns["ParseAuthorField"]);
                    dataBibTex["Editors"] = "";
                    foreach (string editor in editors)
                    {
                        string[] name = Regex.Split(editor, @"\,");
                        if (name.Length == 1)
                        {
                            name = new string[2];
                            MatchCollection mc = Regex.Matches(editor, @"\s+");
                            if (mc.Count > 0)
                            {
                                name[0] = editor.Substring(mc[mc.Count - 1].Index + 1);
                                name[1] = editor.Substring(0, mc[mc.Count - 1].Index);
                            }
                            else
                            {
                                name[0] = editor;
                                name[1] = "";
                            }
                            dataBibTex["Editors"] += name[0] + "|" + name[1] + "|";
                        }
                    }
                }

                m = Regex.Match(strBibTex, regexPatterns["Publisher"], rOptions);
                if (m.Success)
                    dataBibTex["Publisher"] = Regex.Replace(m.Value, regexPatterns["ParseField"], "");

                _ProcessLatexToUnicode(ref dataBibTex);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void _ProcessLatexToUnicode(ref Dictionary<string, string> dataBibTex)
        {
            try
            {
                string[] keys = new string[dataBibTex.Keys.Count];
                dataBibTex.Keys.CopyTo(keys, 0);
                foreach (string strKey in keys)
                {
                    Match m = Regex.Match(dataBibTex[strKey], @"{.*?}");
                    while (m.Success)
                    {
                        string strTemp = Regex.Match(m.Groups[0].Value, @"{.*?}", RegexOptions.RightToLeft).Value;
                        string strLatex = Regex.Replace(strTemp, @"\{\s*|\s*\}", "");
                        string strUnicode = _GetUnicodeFromLatex(strLatex);
                        dataBibTex[strKey] = dataBibTex[strKey].Replace(strTemp, strUnicode);
                        m = Regex.Match(dataBibTex[strKey], @"{.*?}");
                    }
                    dataBibTex[strKey] = Regex.Replace(dataBibTex[strKey], @"\{|\}", "");
                }
            }
            catch
            { }
        }

        private string _GetUnicodeFromLatex(string strLatex)
        {
            if (this.mappingLatex2UnicodeTable.ContainsKey(strLatex))
                return Char.Parse(this.mappingLatex2UnicodeTable[strLatex]).ToString();
            else
                return strLatex;
        }
        
        private Dictionary<string, string> _GetRegexPatterns()
        {
            Dictionary<string, string> regexPatterns = new Dictionary<string, string>{   {"RemoveTab", @"\t+|\r+"}, 
                                                                                        {"RemoveStartingChar", @"^\n*[\s|\n]*|^\s*[\n|\s]*"},
                                                                                        {"StartEntry", @"\n*\s*@[a-zA-Z]+[\(\{]"},
                                                                                        {"ParseItemType", @"@|\{\s*"},
                                                                                        {"ParseField", @"^\s*[a-zA-Z]+\s*\=\s*\(*\{{1}\s*|\}{1}\)*\s*\,*\n*$"},
                                                                                        {"Title", @"(title)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Title2", @"((journal)|(booktitle))+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Affiliation", @"(affiliation)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Edition", @"(edition)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Volume", @"((volume)|(school)|(institution))+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Volume2", @"(number)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Abstract", @"(abstract)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Notes", @"(note)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"PubPlace", @"(location)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"PubDate", @"(year)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Pages", @"(pages)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"ID1", @"((issn)|(isbn))+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Links", @"(url)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Keywords", @"(keywords)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Author", @"(author)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Publisher", @"(publisher)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"Editor", @"(editor)+\s*\=+\s*[\(\{](.*?)\n+"},
                                                                                        {"ParseAuthorField", @"\,*\s+and\s+\,*"}
                                                                                    };
            return regexPatterns;
        }

        private HttpWebRequest _PrepareRequest(int iStart)
        {
            string strRequestURL = GOOGLESCHOLAR_URL;
            strRequestURL = strRequestURL.Replace("{start}", iStart + "");
            strRequestURL = strRequestURL.Replace("{query}", HttpUtility.UrlEncode(SearchQuery));

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strRequestURL);
            req.UserAgent = __USERAGENT;
            string proxy = Properties.Settings.Default.PROXY_SETTING;
            if (proxy.Length > 0 && proxy != null)
                req.Proxy = new WebProxy(proxy, true);

            req.Headers.Add("Cookie", "GSP=ID=" + _GenerateRandom16DigitHex() + ":CF=4");
            req.Timeout = 60000;

            return req;
        }

        private HttpWebRequest _PrepareBibTeXRequest(string strURI)
        {
            string strBibURL = "http://scholar.google.com" + strURI;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strBibURL);
            req.UserAgent = __USERAGENT;
            string proxy = Properties.Settings.Default.PROXY_SETTING;
            if (proxy.Length > 0 && proxy != null)
                req.Proxy = new WebProxy(proxy, true);

            req.Headers.Add("Cookie", "GSP=ID=" + _GenerateRandom16DigitHex() + ":CF=4");

            return req;
        }

        private Int64 _FindTotalNumber(string strHtml)
        {
            Int64 iTotal = 0;
            Match match = Regex.Match(strHtml, GOOGLESCHOLAR_FINDCOUNT_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            if (match.Success)
            {
                foreach (Group group in match.Groups)
                {
                    try
                    {
                        string temp = group.Value;
                        temp = Regex.Replace(temp, @"\,", "");
                        iTotal = Int64.Parse(temp);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return iTotal;
        }

        private string _GenerateRandom16DigitHex()
        {
            Random rdm = new Random();
            int iMax = 65535;
            string strRandomHex = GoogleCookieDefaultHex;
            try
            {
                strRandomHex = String.Format("{0:X4}", rdm.Next(iMax));
                strRandomHex += String.Format("{0:X4}", rdm.Next(iMax));
                strRandomHex += String.Format("{0:X4}", rdm.Next(iMax));
                strRandomHex += String.Format("{0:X4}", rdm.Next(iMax));
            }
            catch
            {
                strRandomHex = GoogleCookieDefaultHex;
            }
            return strRandomHex;
        }

        private void _ConvertBibTex2WizFolioItemType(ref ItemMasterRow item, string strBibTexItemType)
        {
            item.ItemTypeID = mappingBibTex2WizFolio.ContainsKey(strBibTexItemType) ? mappingBibTex2WizFolio[strBibTexItemType] : ItemTypes.JournalArticle;
        }

        private void _InitializeBibTexParser()
        {
            regexPatterns = _GetRegexPatterns();
            _SetMapBibTex2WizFolio();
            _SetMapLatex2UnicodeTable();
        }

        private void _SetMapLatex2UnicodeTable()
        {
            Dictionary<string, string> mappingLatex2UnicodeTable = new Dictionary<string, string>{{"\\url"                         , ""},       // strip 'url'
                                                                                                {"\\href"                          , ""},       // strip 'href'
                                                                                                {"~"                               , "\u00A0"}, // NO-BREAK SPACE
                                                                                                {"{\\textexclamdown}"              , "\u00A1"}, // INVERTED EXCLAMATION MARK
                                                                                                {"{\\textcent}"                    , "\u00A2"}, // CENT SIGN
                                                                                                {"{\\textsterling}"                , "\u00A3"}, // POUND SIGN
                                                                                                {"{\\textyen}"                     , "\u00A5"}, // YEN SIGN
                                                                                                {"{\\textbrokenbar}"               , "\u00A6"}, // BROKEN BAR
                                                                                                {"{\\textsection}"                 , "\u00A7"}, // SECTION SIGN
                                                                                                {"{\\textasciidieresis}"           , "\u00A8"}, // DIAERESIS
                                                                                                {"{\\textcopyright}"               , "\u00A9"}, // COPYRIGHT SIGN
                                                                                                {"{\\textordfeminine}"             , "\u00AA"}, // FEMININE ORDINAL INDICATOR
                                                                                                {"{\\guillemotleft}"               , "\u00AB"}, // LEFT-POINTING DOUBLE ANGLE QUOTATION MARK
                                                                                                {"{\\textlnot}"                    , "\u00AC"}, // NOT SIGN
                                                                                                {"{\\textregistered}"              , "\u00AE"}, // REGISTERED SIGN
                                                                                                {"{\\textasciimacron}"             , "\u00AF"}, // MACRON
                                                                                                {"{\\textdegree}"                  , "\u00B0"}, // DEGREE SIGN
                                                                                                {"{\\textpm}"                      , "\u00B1"}, // PLUS-MINUS SIGN
                                                                                                {"{\\texttwosuperior}"             , "\u00B2"}, // SUPERSCRIPT TWO
                                                                                                {"{\\textthreesuperior}"           , "\u00B3"}, // SUPERSCRIPT THREE
                                                                                                {"{\\textasciiacute}"              , "\u00B4"}, // ACUTE ACCENT
                                                                                                {"{\\textmu}"                      , "\u00B5"}, // MICRO SIGN
                                                                                                {"{\\textparagraph}"               , "\u00B6"}, // PILCROW SIGN
                                                                                                {"{\\textperiodcentered}"          , "\u00B7"}, // MIDDLE DOT
                                                                                                {"{\\c\\ }"                        , "\u00B8"}, // CEDILLA
                                                                                                {"{\\textonesuperior}"             , "\u00B9"}, // SUPERSCRIPT ONE
                                                                                                {"{\\textordmasculine}"            , "\u00BA"}, // MASCULINE ORDINAL INDICATOR
                                                                                                {"{\\guillemotright}"              , "\u00BB"}, // RIGHT-POINTING DOUBLE ANGLE QUOTATION MARK
                                                                                                {"{\\textonequarter}"              , "\u00BC"}, // VULGAR FRACTION ONE QUARTER
                                                                                                {"{\\textonehalf}"                 , "\u00BD"}, // VULGAR FRACTION ONE HALF
                                                                                                {"{\\textthreequarters}"           , "\u00BE"}, // VULGAR FRACTION THREE QUARTERS
                                                                                                {"{\\textquestiondown}"            , "\u00BF"}, // INVERTED QUESTION MARK
                                                                                                {"{\\AE}"                          , "\u00C6"}, // LATIN CAPITAL LETTER AE
                                                                                                {"{\\DH}"                          , "\u00D0"}, // LATIN CAPITAL LETTER ETH
                                                                                                {"{\\texttimes}"                   , "\u00D7"}, // MULTIPLICATION SIGN
                                                                                                {"{\\TH}"                          , "\u00DE"}, // LATIN CAPITAL LETTER THORN
                                                                                                {"{\\ss}"                          , "\u00DF"}, // LATIN SMALL LETTER SHARP S
                                                                                                {"{\\ae}"                          , "\u00E6"}, // LATIN SMALL LETTER AE
                                                                                                {"{\\dh}"                          , "\u00F0"}, // LATIN SMALL LETTER ETH
                                                                                                {"{\\textdiv}"                     , "\u00F7"}, // DIVISION SIGN
                                                                                                {"{\\th}"                          , "\u00FE"}, // LATIN SMALL LETTER THORN
                                                                                                {"{\\i}"                           , "\u0131"}, // LATIN SMALL LETTER DOTLESS I
                                                                                                {"'n"                              , "\u0149"}, // LATIN SMALL LETTER N PRECEDED BY APOSTROPHE
                                                                                                {"{\\NG}"                          , "\u014A"}, // LATIN CAPITAL LETTER ENG
                                                                                                {"{\\ng}"                          , "\u014B"}, // LATIN SMALL LETTER ENG
                                                                                                {"{\\OE}"                          , "\u0152"}, // LATIN CAPITAL LIGATURE OE
                                                                                                {"{\\oe}"                          , "\u0153"}, // LATIN SMALL LIGATURE OE
                                                                                                {"{\\textasciicircum}"             , "\u02C6"}, // MODIFIER LETTER CIRCUMFLEX ACCENT
                                                                                            //    {"\\~{}"                           , "\u02DC"}, // SMALL TILDE
                                                                                                {"{\\textacutedbl}"                , "\u02DD"}, // DOUBLE ACUTE ACCENT
                                                                                                {"{\\textendash}"                  , "\u2013"}, // EN DASH
                                                                                                {"{\\textemdash}"                  , "\u2014"}, // EM DASH
                                                                                                {"---"                             , "\u2014"}, // EM DASH
                                                                                                {"--"                              , "\u2013"}, // EN DASH
                                                                                                {"{\\textbardbl}"                  , "\u2016"}, // DOUBLE VERTICAL LINE
                                                                                                {"{\\textunderscore}"              , "\u2017"}, // DOUBLE LOW LINE
                                                                                                {"{\\textquoteleft}"               , "\u2018"}, // LEFT SINGLE QUOTATION MARK
                                                                                                {"{\\textquoteright}"              , "\u2019"}, // RIGHT SINGLE QUOTATION MARK
                                                                                                {"{\\quotesinglbase}"              , "\u201A"}, // SINGLE LOW-9 QUOTATION MARK
                                                                                                {"{\\textquotedblleft}"            , "\u201C"}, // LEFT DOUBLE QUOTATION MARK
                                                                                                {"{\\textquotedblright}"           , "\u201D"}, // RIGHT DOUBLE QUOTATION MARK
                                                                                                {"{\\quotedblbase}"                , "\u201E"}, // DOUBLE LOW-9 QUOTATION MARK
                                                                                            //    {"{\\quotedblbase}"                , "\u201F"}, // DOUBLE HIGH-REVERSED-9 QUOTATION MARK
                                                                                                {"{\\textdagger}"                  , "\u2020"}, // DAGGER
                                                                                                {"{\\textdaggerdbl}"               , "\u2021"}, // DOUBLE DAGGER
                                                                                                {"{\\textbullet}"                  , "\u2022"}, // BULLET
                                                                                                {"{\\textellipsis}"                , "\u2026"}, // HORIZONTAL ELLIPSIS
                                                                                                {"{\\textperthousand}"             , "\u2030"}, // PER MILLE SIGN
                                                                                                {"'''"                             , "\u2034"}, // TRIPLE PRIME
                                                                                                {"''"                              , "\u201D"}, // RIGHT DOUBLE QUOTATION MARK (could be a double prime)
                                                                                                {"``"                              , "\u201C"}, // LEFT DOUBLE QUOTATION MARK (could be a reversed double prime)
                                                                                                {"```"                             , "\u2037"}, // REVERSED TRIPLE PRIME
                                                                                                {"{\\guilsinglleft}"               , "\u2039"}, // SINGLE LEFT-POINTING ANGLE QUOTATION MARK
                                                                                                {"{\\guilsinglright}"              , "\u203A"}, // SINGLE RIGHT-POINTING ANGLE QUOTATION MARK
                                                                                                {"!!"                              , "\u203C"}, // DOUBLE EXCLAMATION MARK
                                                                                                {"{\\textfractionsolidus}"         , "\u2044"}, // FRACTION SLASH
                                                                                                {"?!"                              , "\u2048"}, // QUESTION EXCLAMATION MARK
                                                                                                {"!?"                              , "\u2049"}, // EXCLAMATION QUESTION MARK
                                                                                                {"$^{0}$"                          , "\u2070"}, // SUPERSCRIPT ZERO
                                                                                                {"$^{4}$"                          , "\u2074"}, // SUPERSCRIPT FOUR
                                                                                                {"$^{5}$"                          , "\u2075"}, // SUPERSCRIPT FIVE
                                                                                                {"$^{6}$"                          , "\u2076"}, // SUPERSCRIPT SIX
                                                                                                {"$^{7}$"                          , "\u2077"}, // SUPERSCRIPT SEVEN
                                                                                                {"$^{8}$"                          , "\u2078"}, // SUPERSCRIPT EIGHT
                                                                                                {"$^{9}$"                          , "\u2079"}, // SUPERSCRIPT NINE
                                                                                                {"$^{+}$"                          , "\u207A"}, // SUPERSCRIPT PLUS SIGN
                                                                                                {"$^{-}$"                          , "\u207B"}, // SUPERSCRIPT MINUS
                                                                                                {"$^{=}$"                          , "\u207C"}, // SUPERSCRIPT EQUALS SIGN
                                                                                                {"$^{(}$"                          , "\u207D"}, // SUPERSCRIPT LEFT PARENTHESIS
                                                                                                {"$^{)}$"                          , "\u207E"}, // SUPERSCRIPT RIGHT PARENTHESIS
                                                                                                {"$^{n}$"                          , "\u207F"}, // SUPERSCRIPT LATIN SMALL LETTER N
                                                                                                {"$_{0}$"                          , "\u2080"}, // SUBSCRIPT ZERO
                                                                                                {"$_{1}$"                          , "\u2081"}, // SUBSCRIPT ONE
                                                                                                {"$_{2}$"                          , "\u2082"}, // SUBSCRIPT TWO
                                                                                                {"$_{3}$"                          , "\u2083"}, // SUBSCRIPT THREE
                                                                                                {"$_{4}$"                          , "\u2084"}, // SUBSCRIPT FOUR
                                                                                                {"$_{5}$"                          , "\u2085"}, // SUBSCRIPT FIVE
                                                                                                {"$_{6}$"                          , "\u2086"}, // SUBSCRIPT SIX
                                                                                                {"$_{7}$"                          , "\u2087"}, // SUBSCRIPT SEVEN
                                                                                                {"$_{8}$"                          , "\u2088"}, // SUBSCRIPT EIGHT
                                                                                                {"$_{9}$"                          , "\u2089"}, // SUBSCRIPT NINE
                                                                                                {"$_{+}$"                          , "\u208A"}, // SUBSCRIPT PLUS SIGN
                                                                                                {"$_{-}$"                          , "\u208B"}, // SUBSCRIPT MINUS
                                                                                                {"$_{=}$"                          , "\u208C"}, // SUBSCRIPT EQUALS SIGN
                                                                                                {"$_{(}$"                          , "\u208D"}, // SUBSCRIPT LEFT PARENTHESIS
                                                                                                {"$_{)}$"                          , "\u208E"}, // SUBSCRIPT RIGHT PARENTHESIS
                                                                                                {"{\\texteuro}"                    , "\u20AC"}, // EURO SIGN
                                                                                                {"a/c"                             , "\u2100"}, // ACCOUNT OF
                                                                                                {"a/s"                             , "\u2101"}, // ADDRESSED TO THE SUBJECT
                                                                                                {"{\\textcelsius}"                 , "\u2103"}, // DEGREE CELSIUS
                                                                                                {"c/o"                             , "\u2105"}, // CARE OF
                                                                                                {"c/u"                             , "\u2106"}, // CADA UNA
                                                                                                {"{\\textnumero}"                  , "\u2116"}, // NUMERO SIGN
                                                                                                {"{\\textcircledP}"                , "\u2117"}, // SOUND RECORDING COPYRIGHT
                                                                                                {"{\\textservicemark}"             , "\u2120"}, // SERVICE MARK
                                                                                                {"{TEL}"                           , "\u2121"}, // TELEPHONE SIGN
                                                                                                {"{\\texttrademark}"               , "\u2122"}, // TRADE MARK SIGN
                                                                                                {"{\\textohm}"                     , "\u2126"}, // OHM SIGN
                                                                                                {"{\\textestimated}"               , "\u212E"}, // ESTIMATED SYMBOL
                                                                                                {" 1/3"                            , "\u2153"}, // VULGAR FRACTION ONE THIRD
                                                                                                {" 2/3"                            , "\u2154"}, // VULGAR FRACTION TWO THIRDS
                                                                                                {" 1/5"                            , "\u2155"}, // VULGAR FRACTION ONE FIFTH
                                                                                                {" 2/5"                            , "\u2156"}, // VULGAR FRACTION TWO FIFTHS
                                                                                                {" 3/5"                            , "\u2157"}, // VULGAR FRACTION THREE FIFTHS
                                                                                                {" 4/5"                            , "\u2158"}, // VULGAR FRACTION FOUR FIFTHS
                                                                                                {" 1/6"                            , "\u2159"}, // VULGAR FRACTION ONE SIXTH
                                                                                                {" 5/6"                            , "\u215A"}, // VULGAR FRACTION FIVE SIXTHS
                                                                                                {" 1/8"                            , "\u215B"}, // VULGAR FRACTION ONE EIGHTH
                                                                                                {" 3/8"                            , "\u215C"}, // VULGAR FRACTION THREE EIGHTHS
                                                                                                {" 5/8"                            , "\u215D"}, // VULGAR FRACTION FIVE EIGHTHS
                                                                                                {" 7/8"                            , "\u215E"}, // VULGAR FRACTION SEVEN EIGHTHS
                                                                                                {" 1/"                             , "\u215F"}, // FRACTION NUMERATOR ONE
                                                                                                {"{\\textleftarrow}"               , "\u2190"}, // LEFTWARDS ARROW
                                                                                                {"{\\textuparrow}"                 , "\u2191"}, // UPWARDS ARROW
                                                                                                {"{\\textrightarrow}"              , "\u2192"}, // RIGHTWARDS ARROW
                                                                                                {"{\\textdownarrow}"               , "\u2193"}, // DOWNWARDS ARROW
                                                                                                {"<->"                             , "\u2194"}, // LEFT RIGHT ARROW
                                                                                                {"<="                              , "\u21D0"}, // LEFTWARDS DOUBLE ARROW
                                                                                                {"=>"                              , "\u21D2"}, // RIGHTWARDS DOUBLE ARROW
                                                                                                {"<=>"                             , "\u21D4"}, // LEFT RIGHT DOUBLE ARROW
                                                                                                {"$\\infty$"                       , "\u221E"}, // INFINITY
                                                                                                {"||"                              , "\u2225"}, // PARALLEL TO
                                                                                                {"\\~{}"                           , "\u223C"}, // TILDE OPERATOR
                                                                                                {"/="                              , "\u2260"}, // NOT EQUAL TO
                                                                                            //    "<="                              , "\u2264"}, // LESS-THAN OR EQUAL TO
                                                                                                {">="                              , "\u2265"}, // GREATER-THAN OR EQUAL TO
                                                                                                {"<<"                              , "\u226A"}, // MUCH LESS-THAN
                                                                                                {">>"                              , "\u226B"}, // MUCH GREATER-THAN
                                                                                                {"(+)"                             , "\u2295"}, // CIRCLED PLUS
                                                                                                {"(-)"                             , "\u2296"}, // CIRCLED MINUS
                                                                                                {"(x)"                             , "\u2297"}, // CIRCLED TIMES
                                                                                                {"(/)"                             , "\u2298"}, // CIRCLED DIVISION SLASH
                                                                                                {"|-"                              , "\u22A2"}, // RIGHT TACK
                                                                                                {"-|"                              , "\u22A3"}, // LEFT TACK
                                                                                            //    "|-"                              , "\u22A6"}, // ASSERTION
                                                                                                {"|="                              , "\u22A7"}, // MODELS
                                                                                            //    "|="                              , "\u22A8"}, // TRUE
                                                                                                {"||-"                             , "\u22A9"}, // FORCES
                                                                                                {"$\\#$"                           , "\u22D5"}, // EQUAL AND PARALLEL TO
                                                                                                {"<<<"                             , "\u22D8"}, // VERY MUCH LESS-THAN
                                                                                                {">>>"                             , "\u22D9"}, // VERY MUCH GREATER-THAN
                                                                                                {"{\\textlangle}"                  , "\u2329"}, // LEFT-POINTING ANGLE BRACKET
                                                                                                {"{\\textrangle}"                  , "\u232A"}, // RIGHT-POINTING ANGLE BRACKET
                                                                                                {"{\\textvisiblespace}"            , "\u2423"}, // OPEN BOX
                                                                                                {"///"                             , "\u2425"}, // SYMBOL FOR DELETE FORM TWO
                                                                                                {"{\\textopenbullet}"              , "\u25E6"}, // WHITE BULLET
                                                                                                {":-("                             , "\u2639"}, // WHITE FROWNING FACE
                                                                                                {":-)"                             , "\u263A"}, // WHITE SMILING FACE
                                                                                                {"(-: "                            , "\u263B"}, // BLACK SMILING FACE
                                                                                            //    {"$\\#$"                           , "\u266F"}, // MUSIC SHARP SIGN
                                                                                                {"$\\%<$"                          , "\u2701"}, // UPPER BLADE SCISSORS
                                                                                            /*    {"$\\%<$"                          , "\u2702"}, // BLACK SCISSORS
                                                                                                {"$\\%<$"                          , "\u2703"}, // LOWER BLADE SCISSORS
                                                                                                {"$\\%<$"                          , "\u2704"}, // WHITE SCISSORS */
                                                                                            /* Derived accented characters */
                                                                                                {"\\`{A}"                          , "\u00C0"}, // LATIN CAPITAL LETTER A WITH GRAVE
                                                                                                {"\\'{A}"                          , "\u00C1"}, // LATIN CAPITAL LETTER A WITH ACUTE
                                                                                                {"\\^{A}"                          , "\u00C2"}, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX
                                                                                                {"\\~{A}"                          , "\u00C3"}, // LATIN CAPITAL LETTER A WITH TILDE
                                                                                                {"\\\"{A}"                         , "\u00C4"}, // LATIN CAPITAL LETTER A WITH DIAERESIS
                                                                                                {"\\r{A}"                          , "\u00C5"}, // LATIN CAPITAL LETTER A WITH RING ABOVE
                                                                                                {"\\c{C}"                          , "\u00C7"}, // LATIN CAPITAL LETTER C WITH CEDILLA
                                                                                                {"\\`{E}"                          , "\u00C8"}, // LATIN CAPITAL LETTER E WITH GRAVE
                                                                                                {"\\'{E}"                          , "\u00C9"}, // LATIN CAPITAL LETTER E WITH ACUTE
                                                                                                {"\\^{E}"                          , "\u00CA"}, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX
                                                                                                {"\\\"{E}"                         , "\u00CB"}, // LATIN CAPITAL LETTER E WITH DIAERESIS
                                                                                                {"\\`{I}"                          , "\u00CC"}, // LATIN CAPITAL LETTER I WITH GRAVE
                                                                                                {"\\'{I}"                          , "\u00CD"}, // LATIN CAPITAL LETTER I WITH ACUTE
                                                                                                {"\\^{I}"                          , "\u00CE"}, // LATIN CAPITAL LETTER I WITH CIRCUMFLEX
                                                                                                {"\\\"{I}"                         , "\u00CF"}, // LATIN CAPITAL LETTER I WITH DIAERESIS
                                                                                                {"\\~{N}"                          , "\u00D1"}, // LATIN CAPITAL LETTER N WITH TILDE
                                                                                                {"\\`{O}"                          , "\u00D2"}, // LATIN CAPITAL LETTER O WITH GRAVE
                                                                                                {"\\'{O}"                          , "\u00D3"}, // LATIN CAPITAL LETTER O WITH ACUTE
                                                                                                {"\\^{O}"                          , "\u00D4"}, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX
                                                                                                {"\\~{O}"                          , "\u00D5"}, // LATIN CAPITAL LETTER O WITH TILDE
                                                                                                {"\\\"{O}"                         , "\u00D6"}, // LATIN CAPITAL LETTER O WITH DIAERESIS
                                                                                                {"\\`{U}"                          , "\u00D9"}, // LATIN CAPITAL LETTER U WITH GRAVE
                                                                                                {"\\'{U}"                          , "\u00DA"}, // LATIN CAPITAL LETTER U WITH ACUTE
                                                                                                {"\\^{U}"                          , "\u00DB"}, // LATIN CAPITAL LETTER U WITH CIRCUMFLEX
                                                                                                {"\\\"{U}"                         , "\u00DC"}, // LATIN CAPITAL LETTER U WITH DIAERESIS
                                                                                                {"\\'{Y}"                          , "\u00DD"}, // LATIN CAPITAL LETTER Y WITH ACUTE
                                                                                                {"\\`{a}"                          , "\u00E0"}, // LATIN SMALL LETTER A WITH GRAVE
                                                                                                {"\\'{a}"                          , "\u00E1"}, // LATIN SMALL LETTER A WITH ACUTE
                                                                                                {"\\^{a}"                          , "\u00E2"}, // LATIN SMALL LETTER A WITH CIRCUMFLEX
                                                                                                {"\\~{a}"                          , "\u00E3"}, // LATIN SMALL LETTER A WITH TILDE
                                                                                                {"\\\"{a}"                         , "\u00E4"}, // LATIN SMALL LETTER A WITH DIAERESIS
                                                                                                {"\\r{a}"                          , "\u00E5"}, // LATIN SMALL LETTER A WITH RING ABOVE
                                                                                                {"\\c{c}"                          , "\u00E7"}, // LATIN SMALL LETTER C WITH CEDILLA
                                                                                                {"\\`{e}"                          , "\u00E8"}, // LATIN SMALL LETTER E WITH GRAVE
                                                                                                {"\\'{e}"                          , "\u00E9"}, // LATIN SMALL LETTER E WITH ACUTE
                                                                                                {"\\^{e}"                          , "\u00EA"}, // LATIN SMALL LETTER E WITH CIRCUMFLEX
                                                                                                {"\\\"{e}"                         , "\u00EB"}, // LATIN SMALL LETTER E WITH DIAERESIS
                                                                                                {"\\`{i}"                          , "\u00EC"}, // LATIN SMALL LETTER I WITH GRAVE
                                                                                                {"\\'{i}"                          , "\u00ED"}, // LATIN SMALL LETTER I WITH ACUTE
                                                                                                {"\\^{i}"                          , "\u00EE"}, // LATIN SMALL LETTER I WITH CIRCUMFLEX
                                                                                                {"\\\"{i}"                         , "\u00EF"}, // LATIN SMALL LETTER I WITH DIAERESIS
                                                                                                {"\\~{n}"                          , "\u00F1"}, // LATIN SMALL LETTER N WITH TILDE
                                                                                                {"\\`{o}"                          , "\u00F2"}, // LATIN SMALL LETTER O WITH GRAVE
                                                                                                {"\\'{o}"                          , "\u00F3"}, // LATIN SMALL LETTER O WITH ACUTE
                                                                                                {"\\^{o}"                          , "\u00F4"}, // LATIN SMALL LETTER O WITH CIRCUMFLEX
                                                                                                {"\\~{o}"                          , "\u00F5"}, // LATIN SMALL LETTER O WITH TILDE
                                                                                                {"\\\"{o}"                         , "\u00F6"}, // LATIN SMALL LETTER O WITH DIAERESIS
                                                                                                {"\\`{u}"                          , "\u00F9"}, // LATIN SMALL LETTER U WITH GRAVE
                                                                                                {"\\'{u}"                          , "\u00FA"}, // LATIN SMALL LETTER U WITH ACUTE
                                                                                                {"\\^{u}"                          , "\u00FB"}, // LATIN SMALL LETTER U WITH CIRCUMFLEX
                                                                                                {"\\\"{u}"                         , "\u00FC"}, // LATIN SMALL LETTER U WITH DIAERESIS
                                                                                                {"\\'{y}"                          , "\u00FD"}, // LATIN SMALL LETTER Y WITH ACUTE
                                                                                                {"\\\"{y}"                         , "\u00FF"}, // LATIN SMALL LETTER Y WITH DIAERESIS
                                                                                                {"\\={A}"                          , "\u0100"}, // LATIN CAPITAL LETTER A WITH MACRON
                                                                                                {"\\={a}"                          , "\u0101"}, // LATIN SMALL LETTER A WITH MACRON
                                                                                                {"\\u{A}"                          , "\u0102"}, // LATIN CAPITAL LETTER A WITH BREVE
                                                                                                {"\\u{a}"                          , "\u0103"}, // LATIN SMALL LETTER A WITH BREVE
                                                                                                {"\\k{A}"                          , "\u0104"}, // LATIN CAPITAL LETTER A WITH OGONEK
                                                                                                {"\\k{a}"                          , "\u0105"}, // LATIN SMALL LETTER A WITH OGONEK
                                                                                                {"\\'{C}"                          , "\u0106"}, // LATIN CAPITAL LETTER C WITH ACUTE
                                                                                                {"\\'{c}"                          , "\u0107"}, // LATIN SMALL LETTER C WITH ACUTE
                                                                                                {"\\^{C}"                          , "\u0108"}, // LATIN CAPITAL LETTER C WITH CIRCUMFLEX
                                                                                                {"\\^{c}"                          , "\u0109"}, // LATIN SMALL LETTER C WITH CIRCUMFLEX
                                                                                                {"\\.{C}"                          , "\u010A"}, // LATIN CAPITAL LETTER C WITH DOT ABOVE
                                                                                                {"\\.{c}"                          , "\u010B"}, // LATIN SMALL LETTER C WITH DOT ABOVE
                                                                                                {"\\v{C}"                          , "\u010C"}, // LATIN CAPITAL LETTER C WITH CARON
                                                                                                {"\\v{c}"                          , "\u010D"}, // LATIN SMALL LETTER C WITH CARON
                                                                                                {"\\v{D}"                          , "\u010E"}, // LATIN CAPITAL LETTER D WITH CARON
                                                                                                {"\\v{d}"                          , "\u010F"}, // LATIN SMALL LETTER D WITH CARON
                                                                                                {"\\={E}"                          , "\u0112"}, // LATIN CAPITAL LETTER E WITH MACRON
                                                                                                {"\\={e}"                          , "\u0113"}, // LATIN SMALL LETTER E WITH MACRON
                                                                                                {"\\u{E}"                          , "\u0114"}, // LATIN CAPITAL LETTER E WITH BREVE
                                                                                                {"\\u{e}"                          , "\u0115"}, // LATIN SMALL LETTER E WITH BREVE
                                                                                                {"\\.{E}"                          , "\u0116"}, // LATIN CAPITAL LETTER E WITH DOT ABOVE
                                                                                                {"\\.{e}"                          , "\u0117"}, // LATIN SMALL LETTER E WITH DOT ABOVE
                                                                                                {"\\k{E}"                          , "\u0118"}, // LATIN CAPITAL LETTER E WITH OGONEK
                                                                                                {"\\k{e}"                          , "\u0119"}, // LATIN SMALL LETTER E WITH OGONEK
                                                                                                {"\\v{E}"                          , "\u011A"}, // LATIN CAPITAL LETTER E WITH CARON
                                                                                                {"\\v{e}"                          , "\u011B"}, // LATIN SMALL LETTER E WITH CARON
                                                                                                {"\\^{G}"                          , "\u011C"}, // LATIN CAPITAL LETTER G WITH CIRCUMFLEX
                                                                                                {"\\^{g}"                          , "\u011D"}, // LATIN SMALL LETTER G WITH CIRCUMFLEX
                                                                                                {"\\u{G}"                          , "\u011E"}, // LATIN CAPITAL LETTER G WITH BREVE
                                                                                                {"\\u{g}"                          , "\u011F"}, // LATIN SMALL LETTER G WITH BREVE
                                                                                                {"\\.{G}"                          , "\u0120"}, // LATIN CAPITAL LETTER G WITH DOT ABOVE
                                                                                                {"\\.{g}"                          , "\u0121"}, // LATIN SMALL LETTER G WITH DOT ABOVE
                                                                                                {"\\c{G}"                          , "\u0122"}, // LATIN CAPITAL LETTER G WITH CEDILLA
                                                                                                {"\\c{g}"                          , "\u0123"}, // LATIN SMALL LETTER G WITH CEDILLA
                                                                                                {"\\^{H}"                          , "\u0124"}, // LATIN CAPITAL LETTER H WITH CIRCUMFLEX
                                                                                                {"\\^{h}"                          , "\u0125"}, // LATIN SMALL LETTER H WITH CIRCUMFLEX
                                                                                                {"\\~{I}"                          , "\u0128"}, // LATIN CAPITAL LETTER I WITH TILDE
                                                                                                {"\\~{i}"                          , "\u0129"}, // LATIN SMALL LETTER I WITH TILDE
                                                                                                {"\\={I}"                          , "\u012A"}, // LATIN CAPITAL LETTER I WITH MACRON
                                                                                                {"\\={i}"                          , "\u012B"}, // LATIN SMALL LETTER I WITH MACRON
                                                                                                {"\\u{I}"                          , "\u012C"}, // LATIN CAPITAL LETTER I WITH BREVE
                                                                                                {"\\u{i}"                          , "\u012D"}, // LATIN SMALL LETTER I WITH BREVE
                                                                                                {"\\k{I}"                          , "\u012E"}, // LATIN CAPITAL LETTER I WITH OGONEK
                                                                                                {"\\k{i}"                          , "\u012F"}, // LATIN SMALL LETTER I WITH OGONEK
                                                                                                {"\\.{I}"                          , "\u0130"}, // LATIN CAPITAL LETTER I WITH DOT ABOVE
                                                                                                {"\\^{J}"                          , "\u0134"}, // LATIN CAPITAL LETTER J WITH CIRCUMFLEX
                                                                                                {"\\^{j}"                          , "\u0135"}, // LATIN SMALL LETTER J WITH CIRCUMFLEX
                                                                                                {"\\c{K}"                          , "\u0136"}, // LATIN CAPITAL LETTER K WITH CEDILLA
                                                                                                {"\\c{k}"                          , "\u0137"}, // LATIN SMALL LETTER K WITH CEDILLA
                                                                                                {"\\'{L}"                          , "\u0139"}, // LATIN CAPITAL LETTER L WITH ACUTE
                                                                                                {"\\'{l}"                          , "\u013A"}, // LATIN SMALL LETTER L WITH ACUTE
                                                                                                {"\\c{L}"                          , "\u013B"}, // LATIN CAPITAL LETTER L WITH CEDILLA
                                                                                                {"\\c{l}"                          , "\u013C"}, // LATIN SMALL LETTER L WITH CEDILLA
                                                                                                {"\\v{L}"                          , "\u013D"}, // LATIN CAPITAL LETTER L WITH CARON
                                                                                                {"\\v{l}"                          , "\u013E"}, // LATIN SMALL LETTER L WITH CARON
                                                                                                {"\\L{}"                           , "\u0141"}, //LATIN CAPITAL LETTER L WITH STROKE
                                                                                                {"\\l{}"                           , "\u0142"}, //LATIN SMALL LETTER L WITH STROKE
                                                                                                {"\\'{N}"                          , "\u0143"}, // LATIN CAPITAL LETTER N WITH ACUTE
                                                                                                {"\\'{n}"                          , "\u0144"}, // LATIN SMALL LETTER N WITH ACUTE
                                                                                                {"\\c{N}"                          , "\u0145"}, // LATIN CAPITAL LETTER N WITH CEDILLA
                                                                                                {"\\c{n}"                          , "\u0146"}, // LATIN SMALL LETTER N WITH CEDILLA
                                                                                                {"\\v{N}"                          , "\u0147"}, // LATIN CAPITAL LETTER N WITH CARON
                                                                                                {"\\v{n}"                          , "\u0148"}, // LATIN SMALL LETTER N WITH CARON
                                                                                                {"\\={O}"                          , "\u014C"}, // LATIN CAPITAL LETTER O WITH MACRON
                                                                                                {"\\={o}"                          , "\u014D"}, // LATIN SMALL LETTER O WITH MACRON
                                                                                                {"\\u{O}"                          , "\u014E"}, // LATIN CAPITAL LETTER O WITH BREVE
                                                                                                {"\\u{o}"                          , "\u014F"}, // LATIN SMALL LETTER O WITH BREVE
                                                                                                {"\\H{O}"                          , "\u0150"}, // LATIN CAPITAL LETTER O WITH DOUBLE ACUTE
                                                                                                {"\\H{o}"                          , "\u0151"}, // LATIN SMALL LETTER O WITH DOUBLE ACUTE
                                                                                                {"\\'{R}"                          , "\u0154"}, // LATIN CAPITAL LETTER R WITH ACUTE
                                                                                                {"\\'{r}"                          , "\u0155"}, // LATIN SMALL LETTER R WITH ACUTE
                                                                                                {"\\c{R}"                          , "\u0156"}, // LATIN CAPITAL LETTER R WITH CEDILLA
                                                                                                {"\\c{r}"                          , "\u0157"}, // LATIN SMALL LETTER R WITH CEDILLA
                                                                                                {"\\v{R}"                          , "\u0158"}, // LATIN CAPITAL LETTER R WITH CARON
                                                                                                {"\\v{r}"                          , "\u0159"}, // LATIN SMALL LETTER R WITH CARON
                                                                                                {"\\'{S}"                          , "\u015A"}, // LATIN CAPITAL LETTER S WITH ACUTE
                                                                                                {"\\'{s}"                          , "\u015B"}, // LATIN SMALL LETTER S WITH ACUTE
                                                                                                {"\\^{S}"                          , "\u015C"}, // LATIN CAPITAL LETTER S WITH CIRCUMFLEX
                                                                                                {"\\^{s}"                          , "\u015D"}, // LATIN SMALL LETTER S WITH CIRCUMFLEX
                                                                                                {"\\c{S}"                          , "\u015E"}, // LATIN CAPITAL LETTER S WITH CEDILLA
                                                                                                {"\\c{s}"                          , "\u015F"}, // LATIN SMALL LETTER S WITH CEDILLA
                                                                                                {"\\v{S}"                          , "\u0160"}, // LATIN CAPITAL LETTER S WITH CARON
                                                                                                {"\\v{s}"                          , "\u0161"}, // LATIN SMALL LETTER S WITH CARON
                                                                                                {"\\c{T}"                          , "\u0162"}, // LATIN CAPITAL LETTER T WITH CEDILLA
                                                                                                {"\\c{t}"                          , "\u0163"}, // LATIN SMALL LETTER T WITH CEDILLA
                                                                                                {"\\v{T}"                          , "\u0164"}, // LATIN CAPITAL LETTER T WITH CARON
                                                                                                {"\\v{t}"                          , "\u0165"}, // LATIN SMALL LETTER T WITH CARON
                                                                                                {"\\~{U}"                          , "\u0168"}, // LATIN CAPITAL LETTER U WITH TILDE
                                                                                                {"\\~{u}"                          , "\u0169"}, // LATIN SMALL LETTER U WITH TILDE
                                                                                                {"\\={U}"                          , "\u016A"}, // LATIN CAPITAL LETTER U WITH MACRON
                                                                                                {"\\={u}"                          , "\u016B"}, // LATIN SMALL LETTER U WITH MACRON
                                                                                                {"\\u{U}"                          , "\u016C"}, // LATIN CAPITAL LETTER U WITH BREVE
                                                                                                {"\\u{u}"                          , "\u016D"}, // LATIN SMALL LETTER U WITH BREVE
                                                                                                {"\\H{U}"                          , "\u0170"}, // LATIN CAPITAL LETTER U WITH DOUBLE ACUTE
                                                                                                {"\\H{u}"                          , "\u0171"}, // LATIN SMALL LETTER U WITH DOUBLE ACUTE
                                                                                                {"\\k{U}"                          , "\u0172"}, // LATIN CAPITAL LETTER U WITH OGONEK
                                                                                                {"\\k{u}"                          , "\u0173"}, // LATIN SMALL LETTER U WITH OGONEK
                                                                                                {"\\^{W}"                          , "\u0174"}, // LATIN CAPITAL LETTER W WITH CIRCUMFLEX
                                                                                                {"\\^{w}"                          , "\u0175"}, // LATIN SMALL LETTER W WITH CIRCUMFLEX
                                                                                                {"\\^{Y}"                          , "\u0176"}, // LATIN CAPITAL LETTER Y WITH CIRCUMFLEX
                                                                                                {"\\^{y}"                          , "\u0177"}, // LATIN SMALL LETTER Y WITH CIRCUMFLEX
                                                                                                {"\\\"{Y}"                         , "\u0178"}, // LATIN CAPITAL LETTER Y WITH DIAERESIS
                                                                                                {"\\'{Z}"                          , "\u0179"}, // LATIN CAPITAL LETTER Z WITH ACUTE
                                                                                                {"\\'{z}"                          , "\u017A"}, // LATIN SMALL LETTER Z WITH ACUTE
                                                                                                {"\\.{Z}"                          , "\u017B"}, // LATIN CAPITAL LETTER Z WITH DOT ABOVE
                                                                                                {"\\.{z}"                          , "\u017C"}, // LATIN SMALL LETTER Z WITH DOT ABOVE
                                                                                                {"\\v{Z}"                          , "\u017D"}, // LATIN CAPITAL LETTER Z WITH CARON
                                                                                                {"\\v{z}"                          , "\u017E"}, // LATIN SMALL LETTER Z WITH CARON
                                                                                                {"\\v{A}"                          , "\u01CD"}, // LATIN CAPITAL LETTER A WITH CARON
                                                                                                {"\\v{a}"                          , "\u01CE"}, // LATIN SMALL LETTER A WITH CARON
                                                                                                {"\\v{I}"                          , "\u01CF"}, // LATIN CAPITAL LETTER I WITH CARON
                                                                                                {"\\v{i}"                          , "\u01D0"}, // LATIN SMALL LETTER I WITH CARON
                                                                                                {"\\v{O}"                          , "\u01D1"}, // LATIN CAPITAL LETTER O WITH CARON
                                                                                                {"\\v{o}"                          , "\u01D2"}, // LATIN SMALL LETTER O WITH CARON
                                                                                                {"\\v{U}"                          , "\u01D3"}, // LATIN CAPITAL LETTER U WITH CARON
                                                                                                {"\\v{u}"                          , "\u01D4"}, // LATIN SMALL LETTER U WITH CARON
                                                                                                {"\\v{G}"                          , "\u01E6"}, // LATIN CAPITAL LETTER G WITH CARON
                                                                                                {"\\v{g}"                          , "\u01E7"}, // LATIN SMALL LETTER G WITH CARON
                                                                                                {"\\v{K}"                          , "\u01E8"}, // LATIN CAPITAL LETTER K WITH CARON
                                                                                                {"\\v{k}"                          , "\u01E9"}, // LATIN SMALL LETTER K WITH CARON
                                                                                                {"\\k{O}"                          , "\u01EA"}, // LATIN CAPITAL LETTER O WITH OGONEK
                                                                                                {"\\k{o}"                          , "\u01EB"}, // LATIN SMALL LETTER O WITH OGONEK
                                                                                                {"\\v{j}"                          , "\u01F0"}, // LATIN SMALL LETTER J WITH CARON
                                                                                                {"\\'{G}"                          , "\u01F4"}, // LATIN CAPITAL LETTER G WITH ACUTE
                                                                                                {"\\'{g}"                          , "\u01F5"}, // LATIN SMALL LETTER G WITH ACUTE
                                                                                                {"\\.{B}"                          , "\u1E02"}, // LATIN CAPITAL LETTER B WITH DOT ABOVE
                                                                                                {"\\.{b}"                          , "\u1E03"}, // LATIN SMALL LETTER B WITH DOT ABOVE
                                                                                                {"\\d{B}"                          , "\u1E04"}, // LATIN CAPITAL LETTER B WITH DOT BELOW
                                                                                                {"\\d{b}"                          , "\u1E05"}, // LATIN SMALL LETTER B WITH DOT BELOW
                                                                                                {"\\b{B}"                          , "\u1E06"}, // LATIN CAPITAL LETTER B WITH LINE BELOW
                                                                                                {"\\b{b}"                          , "\u1E07"}, // LATIN SMALL LETTER B WITH LINE BELOW
                                                                                                {"\\.{D}"                          , "\u1E0A"}, // LATIN CAPITAL LETTER D WITH DOT ABOVE
                                                                                                {"\\.{d}"                          , "\u1E0B"}, // LATIN SMALL LETTER D WITH DOT ABOVE
                                                                                                {"\\d{D}"                          , "\u1E0C"}, // LATIN CAPITAL LETTER D WITH DOT BELOW
                                                                                                {"\\d{d}"                          , "\u1E0D"}, // LATIN SMALL LETTER D WITH DOT BELOW
                                                                                                {"\\b{D}"                          , "\u1E0E"}, // LATIN CAPITAL LETTER D WITH LINE BELOW
                                                                                                {"\\b{d}"                          , "\u1E0F"}, // LATIN SMALL LETTER D WITH LINE BELOW
                                                                                                {"\\c{D}"                          , "\u1E10"}, // LATIN CAPITAL LETTER D WITH CEDILLA
                                                                                                {"\\c{d}"                          , "\u1E11"}, // LATIN SMALL LETTER D WITH CEDILLA
                                                                                                {"\\.{F}"                          , "\u1E1E"}, // LATIN CAPITAL LETTER F WITH DOT ABOVE
                                                                                                {"\\.{f}"                          , "\u1E1F"}, // LATIN SMALL LETTER F WITH DOT ABOVE
                                                                                                {"\\={G}"                          , "\u1E20"}, // LATIN CAPITAL LETTER G WITH MACRON
                                                                                                {"\\={g}"                          , "\u1E21"}, // LATIN SMALL LETTER G WITH MACRON
                                                                                                {"\\.{H}"                          , "\u1E22"}, // LATIN CAPITAL LETTER H WITH DOT ABOVE
                                                                                                {"\\.{h}"                          , "\u1E23"}, // LATIN SMALL LETTER H WITH DOT ABOVE
                                                                                                {"\\d{H}"                          , "\u1E24"}, // LATIN CAPITAL LETTER H WITH DOT BELOW
                                                                                                {"\\d{h}"                          , "\u1E25"}, // LATIN SMALL LETTER H WITH DOT BELOW
                                                                                                {"\\\"{H}"                         , "\u1E26"}, // LATIN CAPITAL LETTER H WITH DIAERESIS
                                                                                                {"\\\"{h}"                         , "\u1E27"}, // LATIN SMALL LETTER H WITH DIAERESIS
                                                                                                {"\\c{H}"                          , "\u1E28"}, // LATIN CAPITAL LETTER H WITH CEDILLA
                                                                                                {"\\c{h}"                          , "\u1E29"}, // LATIN SMALL LETTER H WITH CEDILLA
                                                                                                {"\\'{K}"                          , "\u1E30"}, // LATIN CAPITAL LETTER K WITH ACUTE
                                                                                                {"\\'{k}"                          , "\u1E31"}, // LATIN SMALL LETTER K WITH ACUTE
                                                                                                {"\\d{K}"                          , "\u1E32"}, // LATIN CAPITAL LETTER K WITH DOT BELOW
                                                                                                {"\\d{k}"                          , "\u1E33"}, // LATIN SMALL LETTER K WITH DOT BELOW
                                                                                                {"\\b{K}"                          , "\u1E34"}, // LATIN CAPITAL LETTER K WITH LINE BELOW
                                                                                                {"\\b{k}"                          , "\u1E35"}, // LATIN SMALL LETTER K WITH LINE BELOW
                                                                                                {"\\d{L}"                          , "\u1E36"}, // LATIN CAPITAL LETTER L WITH DOT BELOW
                                                                                                {"\\d{l}"                          , "\u1E37"}, // LATIN SMALL LETTER L WITH DOT BELOW
                                                                                                {"\\b{L}"                          , "\u1E3A"}, // LATIN CAPITAL LETTER L WITH LINE BELOW
                                                                                                {"\\b{l}"                          , "\u1E3B"}, // LATIN SMALL LETTER L WITH LINE BELOW
                                                                                                {"\\'{M}"                          , "\u1E3E"}, // LATIN CAPITAL LETTER M WITH ACUTE
                                                                                                {"\\'{m}"                          , "\u1E3F"}, // LATIN SMALL LETTER M WITH ACUTE
                                                                                                {"\\.{M}"                          , "\u1E40"}, // LATIN CAPITAL LETTER M WITH DOT ABOVE
                                                                                                {"\\.{m}"                          , "\u1E41"}, // LATIN SMALL LETTER M WITH DOT ABOVE
                                                                                                {"\\d{M}"                          , "\u1E42"}, // LATIN CAPITAL LETTER M WITH DOT BELOW
                                                                                                {"\\d{m}"                          , "\u1E43"}, // LATIN SMALL LETTER M WITH DOT BELOW
                                                                                                {"\\.{N}"                          , "\u1E44"}, // LATIN CAPITAL LETTER N WITH DOT ABOVE
                                                                                                {"\\.{n}"                          , "\u1E45"}, // LATIN SMALL LETTER N WITH DOT ABOVE
                                                                                                {"\\d{N}"                          , "\u1E46"}, // LATIN CAPITAL LETTER N WITH DOT BELOW
                                                                                                {"\\d{n}"                          , "\u1E47"}, // LATIN SMALL LETTER N WITH DOT BELOW
                                                                                                {"\\b{N}"                          , "\u1E48"}, // LATIN CAPITAL LETTER N WITH LINE BELOW
                                                                                                {"\\b{n}"                          , "\u1E49"}, // LATIN SMALL LETTER N WITH LINE BELOW
                                                                                                {"\\'{P}"                          , "\u1E54"}, // LATIN CAPITAL LETTER P WITH ACUTE
                                                                                                {"\\'{p}"                          , "\u1E55"}, // LATIN SMALL LETTER P WITH ACUTE
                                                                                                {"\\.{P}"                          , "\u1E56"}, // LATIN CAPITAL LETTER P WITH DOT ABOVE
                                                                                                {"\\.{p}"                          , "\u1E57"}, // LATIN SMALL LETTER P WITH DOT ABOVE
                                                                                                {"\\.{R}"                          , "\u1E58"}, // LATIN CAPITAL LETTER R WITH DOT ABOVE
                                                                                                {"\\.{r}"                          , "\u1E59"}, // LATIN SMALL LETTER R WITH DOT ABOVE
                                                                                                {"\\d{R}"                          , "\u1E5A"}, // LATIN CAPITAL LETTER R WITH DOT BELOW
                                                                                                {"\\d{r}"                          , "\u1E5B"}, // LATIN SMALL LETTER R WITH DOT BELOW
                                                                                                {"\\b{R}"                          , "\u1E5E"}, // LATIN CAPITAL LETTER R WITH LINE BELOW
                                                                                                {"\\b{r}"                          , "\u1E5F"}, // LATIN SMALL LETTER R WITH LINE BELOW
                                                                                                {"\\.{S}"                          , "\u1E60"}, // LATIN CAPITAL LETTER S WITH DOT ABOVE
                                                                                                {"\\.{s}"                          , "\u1E61"}, // LATIN SMALL LETTER S WITH DOT ABOVE
                                                                                                {"\\d{S}"                          , "\u1E62"}, // LATIN CAPITAL LETTER S WITH DOT BELOW
                                                                                                {"\\d{s}"                          , "\u1E63"}, // LATIN SMALL LETTER S WITH DOT BELOW
                                                                                                {"\\.{T}"                          , "\u1E6A"}, // LATIN CAPITAL LETTER T WITH DOT ABOVE
                                                                                                {"\\.{t}"                          , "\u1E6B"}, // LATIN SMALL LETTER T WITH DOT ABOVE
                                                                                                {"\\d{T}"                          , "\u1E6C"}, // LATIN CAPITAL LETTER T WITH DOT BELOW
                                                                                                {"\\d{t}"                          , "\u1E6D"}, // LATIN SMALL LETTER T WITH DOT BELOW
                                                                                                {"\\b{T}"                          , "\u1E6E"}, // LATIN CAPITAL LETTER T WITH LINE BELOW
                                                                                                {"\\b{t}"                          , "\u1E6F"}, // LATIN SMALL LETTER T WITH LINE BELOW
                                                                                                {"\\~{V}"                          , "\u1E7C"}, // LATIN CAPITAL LETTER V WITH TILDE
                                                                                                {"\\~{v}"                          , "\u1E7D"}, // LATIN SMALL LETTER V WITH TILDE
                                                                                                {"\\d{V}"                          , "\u1E7E"}, // LATIN CAPITAL LETTER V WITH DOT BELOW
                                                                                                {"\\d{v}"                          , "\u1E7F"}, // LATIN SMALL LETTER V WITH DOT BELOW
                                                                                                {"\\`{W}"                          , "\u1E80"}, // LATIN CAPITAL LETTER W WITH GRAVE
                                                                                                {"\\`{w}"                          , "\u1E81"}, // LATIN SMALL LETTER W WITH GRAVE
                                                                                                {"\\'{W}"                          , "\u1E82"}, // LATIN CAPITAL LETTER W WITH ACUTE
                                                                                                {"\\'{w}"                          , "\u1E83"}, // LATIN SMALL LETTER W WITH ACUTE
                                                                                                {"\\\"{W}"                         , "\u1E84"}, // LATIN CAPITAL LETTER W WITH DIAERESIS
                                                                                                {"\\\"{w}"                         , "\u1E85"}, // LATIN SMALL LETTER W WITH DIAERESIS
                                                                                                {"\\.{W}"                          , "\u1E86"}, // LATIN CAPITAL LETTER W WITH DOT ABOVE
                                                                                                {"\\.{w}"                          , "\u1E87"}, // LATIN SMALL LETTER W WITH DOT ABOVE
                                                                                                {"\\d{W}"                          , "\u1E88"}, // LATIN CAPITAL LETTER W WITH DOT BELOW
                                                                                                {"\\d{w}"                          , "\u1E89"}, // LATIN SMALL LETTER W WITH DOT BELOW
                                                                                                {"\\.{X}"                          , "\u1E8A"}, // LATIN CAPITAL LETTER X WITH DOT ABOVE
                                                                                                {"\\.{x}"                          , "\u1E8B"}, // LATIN SMALL LETTER X WITH DOT ABOVE
                                                                                                {"\\\"{X}"                         , "\u1E8C"}, // LATIN CAPITAL LETTER X WITH DIAERESIS
                                                                                                {"\\\"{x}"                         , "\u1E8D"}, // LATIN SMALL LETTER X WITH DIAERESIS
                                                                                                {"\\.{Y}"                          , "\u1E8E"}, // LATIN CAPITAL LETTER Y WITH DOT ABOVE
                                                                                                {"\\.{y}"                          , "\u1E8F"}, // LATIN SMALL LETTER Y WITH DOT ABOVE
                                                                                                {"\\^{Z}"                          , "\u1E90"}, // LATIN CAPITAL LETTER Z WITH CIRCUMFLEX
                                                                                                {"\\^{z}"                          , "\u1E91"}, // LATIN SMALL LETTER Z WITH CIRCUMFLEX
                                                                                                {"\\d{Z}", "\u1E92"}, // LATIN CAPITAL LETTER Z WITH DOT BELOW
                                                                                                {"\\d{z}", "\u1E93"}, // LATIN SMALL LETTER Z WITH DOT BELOW
                                                                                                {"\\b{Z}", "\u1E94"}, // LATIN CAPITAL LETTER Z WITH LINE BELOW
                                                                                                {"\\b{z}", "\u1E95"}, // LATIN SMALL LETTER Z WITH LINE BELOW
                                                                                                {"\\b{h}", "\u1E96"}, // LATIN SMALL LETTER H WITH LINE BELOW
                                                                                                {"\\\"{t}", "\u1E97"}, // LATIN SMALL LETTER T WITH DIAERESIS
                                                                                                {"\\d{A}", "\u1EA0"}, // LATIN CAPITAL LETTER A WITH DOT BELOW
                                                                                                {"\\d{a}", "\u1EA1"}, // LATIN SMALL LETTER A WITH DOT BELOW
                                                                                                {"\\d{E}", "\u1EB8"}, // LATIN CAPITAL LETTER E WITH DOT BELOW
                                                                                                {"\\d{e}", "\u1EB9"}, // LATIN SMALL LETTER E WITH DOT BELOW
                                                                                                {"\\~{E}", "\u1EBC"}, // LATIN CAPITAL LETTER E WITH TILDE
                                                                                                {"\\~{e}", "\u1EBD"}, // LATIN SMALL LETTER E WITH TILDE
                                                                                                {"\\d{I}", "\u1ECA"}, // LATIN CAPITAL LETTER I WITH DOT BELOW
                                                                                                {"\\d{i}", "\u1ECB"}, // LATIN SMALL LETTER I WITH DOT BELOW
                                                                                                {"\\d{O}", "\u1ECC"}, // LATIN CAPITAL LETTER O WITH DOT BELOW
                                                                                                {"\\d{o}", "\u1ECD"}, // LATIN SMALL LETTER O WITH DOT BELOW
                                                                                                {"\\d{U}", "\u1EE4"}, // LATIN CAPITAL LETTER U WITH DOT BELOW
                                                                                                {"\\d{u}", "\u1EE5"}, // LATIN SMALL LETTER U WITH DOT BELOW
                                                                                                {"\\`{Y}", "\u1EF2"}, // LATIN CAPITAL LETTER Y WITH GRAVE
                                                                                                {"\\`{y}", "\u1EF3"}, // LATIN SMALL LETTER Y WITH GRAVE
                                                                                                {"\\d{Y}", "\u1EF4"}, // LATIN CAPITAL LETTER Y WITH DOT BELOW
                                                                                                {"\\d{y}", "\u1EF5"}, // LATIN SMALL LETTER Y WITH DOT BELOW
                                                                                                {"\\~{Y}", "\u1EF8"}, // LATIN CAPITAL LETTER Y WITH TILDE
                                                                                                {"\\~{y}", "\u1EF9"},  // LATIN SMALL LETTER Y WITH TILDE
                                                                                                {"\\\\`A"                          , "\u00C0"}, // LATIN CAPITAL LETTER A WITH GRAVE
                                                                                                {"\\\\'A"                          , "\u00C1"}, // LATIN CAPITAL LETTER A WITH ACUTE
                                                                                                {"\\\\^A"                          , "\u00C2"}, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX
                                                                                                {"\\\\~A"                          , "\u00C3"}, // LATIN CAPITAL LETTER A WITH TILDE
                                                                                                {"\\\\\"A"                         , "\u00C4"}, // LATIN CAPITAL LETTER A WITH DIAERESIS
                                                                                                {"\\\\rA"                          , "\u00C5"}, // LATIN CAPITAL LETTER A WITH RING ABOVE
                                                                                                {"\\\\cC"                          , "\u00C7"}, // LATIN CAPITAL LETTER C WITH CEDILLA
                                                                                                {"\\\\`E"                          , "\u00C8"}, // LATIN CAPITAL LETTER E WITH GRAVE
                                                                                                {"\\\\'E"                          , "\u00C9"}, // LATIN CAPITAL LETTER E WITH ACUTE
                                                                                                {"\\\\^E"                          , "\u00CA"}, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX
                                                                                                {"\\\\\"E"                         , "\u00CB"}, // LATIN CAPITAL LETTER E WITH DIAERESIS
                                                                                                {"\\\\`I"                          , "\u00CC"}, // LATIN CAPITAL LETTER I WITH GRAVE
                                                                                                {"\\\\'I"                          , "\u00CD"}, // LATIN CAPITAL LETTER I WITH ACUTE
                                                                                                {"\\\\^I"                          , "\u00CE"}, // LATIN CAPITAL LETTER I WITH CIRCUMFLEX
                                                                                                {"\\\\\"I"                         , "\u00CF"}, // LATIN CAPITAL LETTER I WITH DIAERESIS
                                                                                                {"\\\\~N"                          , "\u00D1"}, // LATIN CAPITAL LETTER N WITH TILDE
                                                                                                {"\\\\`O"                          , "\u00D2"}, // LATIN CAPITAL LETTER O WITH GRAVE
                                                                                                {"\\\\'O"                          , "\u00D3"}, // LATIN CAPITAL LETTER O WITH ACUTE
                                                                                                {"\\\\^O"                          , "\u00D4"}, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX
                                                                                                {"\\\\~O"                          , "\u00D5"}, // LATIN CAPITAL LETTER O WITH TILDE
                                                                                                {"\\\\\"O"                         , "\u00D6"}, // LATIN CAPITAL LETTER O WITH DIAERESIS
                                                                                                {"\\\\`U"                          , "\u00D9"}, // LATIN CAPITAL LETTER U WITH GRAVE
                                                                                                {"\\\\'U"                          , "\u00DA"}, // LATIN CAPITAL LETTER U WITH ACUTE
                                                                                                {"\\\\^U"                          , "\u00DB"}, // LATIN CAPITAL LETTER U WITH CIRCUMFLEX
                                                                                                {"\\\\\"U"                         , "\u00DC"}, // LATIN CAPITAL LETTER U WITH DIAERESIS
                                                                                                {"\\\\'Y"                          , "\u00DD"}, // LATIN CAPITAL LETTER Y WITH ACUTE
                                                                                                {"\\\\`a"                          , "\u00E0"}, // LATIN SMALL LETTER A WITH GRAVE
                                                                                                {"\\\\'a"                          , "\u00E1"}, // LATIN SMALL LETTER A WITH ACUTE
                                                                                                {"\\\\^a"                          , "\u00E2"}, // LATIN SMALL LETTER A WITH CIRCUMFLEX
                                                                                                {"\\\\~a"                          , "\u00E3"}, // LATIN SMALL LETTER A WITH TILDE
                                                                                                {"\\\\\"a"                         , "\u00E4"}, // LATIN SMALL LETTER A WITH DIAERESIS
                                                                                                {"\\\\ra"                          , "\u00E5"}, // LATIN SMALL LETTER A WITH RING ABOVE
                                                                                                {"\\\\cc"                          , "\u00E7"}, // LATIN SMALL LETTER C WITH CEDILLA
                                                                                                {"\\\\`e"                          , "\u00E8"}, // LATIN SMALL LETTER E WITH GRAVE
                                                                                                {"\\\\'e"                          , "\u00E9"}, // LATIN SMALL LETTER E WITH ACUTE
                                                                                                {"\\\\^e"                          , "\u00EA"}, // LATIN SMALL LETTER E WITH CIRCUMFLEX
                                                                                                {"\\\\\"e"                         , "\u00EB"}, // LATIN SMALL LETTER E WITH DIAERESIS
                                                                                                {"\\\\`i"                          , "\u00EC"}, // LATIN SMALL LETTER I WITH GRAVE
                                                                                                {"\\\\'i"                          , "\u00ED"}, // LATIN SMALL LETTER I WITH ACUTE
                                                                                                {"\\\\^i"                          , "\u00EE"}, // LATIN SMALL LETTER I WITH CIRCUMFLEX
                                                                                                {"\\\\\"i"                         , "\u00EF"}, // LATIN SMALL LETTER I WITH DIAERESIS
                                                                                                {"\\\\~n"                          , "\u00F1"}, // LATIN SMALL LETTER N WITH TILDE
                                                                                                {"\\\\`o"                          , "\u00F2"}, // LATIN SMALL LETTER O WITH GRAVE
                                                                                                {"\\\\'o"                          , "\u00F3"}, // LATIN SMALL LETTER O WITH ACUTE
                                                                                                {"\\\\^o"                          , "\u00F4"}, // LATIN SMALL LETTER O WITH CIRCUMFLEX
                                                                                                {"\\\\~o"                          , "\u00F5"}, // LATIN SMALL LETTER O WITH TILDE
                                                                                                {"\\\\\"o"                         , "\u00F6"}, // LATIN SMALL LETTER O WITH DIAERESIS
                                                                                                {"\\\\`u"                          , "\u00F9"}, // LATIN SMALL LETTER U WITH GRAVE
                                                                                                {"\\\\'u"                          , "\u00FA"}, // LATIN SMALL LETTER U WITH ACUTE
                                                                                                {"\\\\^u"                          , "\u00FB"}, // LATIN SMALL LETTER U WITH CIRCUMFLEX
                                                                                                {"\\\\\"u"                         , "\u00FC"}, // LATIN SMALL LETTER U WITH DIAERESIS
                                                                                                {"\\\\'y"                          , "\u00FD"}, // LATIN SMALL LETTER Y WITH ACUTE
                                                                                                {"\\\\\"y"                         , "\u00FF"}, // LATIN SMALL LETTER Y WITH DIAERESIS
                                                                                                {"\\\\=A"                          , "\u0100"}, // LATIN CAPITAL LETTER A WITH MACRON
                                                                                                {"\\\\=a"                          , "\u0101"}, // LATIN SMALL LETTER A WITH MACRON
                                                                                                {"\\\\uA"                          , "\u0102"}, // LATIN CAPITAL LETTER A WITH BREVE
                                                                                                {"\\\\ua"                          , "\u0103"}, // LATIN SMALL LETTER A WITH BREVE
                                                                                                {"\\\\kA"                          , "\u0104"}, // LATIN CAPITAL LETTER A WITH OGONEK
                                                                                                {"\\\\ka"                          , "\u0105"}, // LATIN SMALL LETTER A WITH OGONEK
                                                                                                {"\\\\'C"                          , "\u0106"}, // LATIN CAPITAL LETTER C WITH ACUTE
                                                                                                {"\\\\'c"                          , "\u0107"}, // LATIN SMALL LETTER C WITH ACUTE
                                                                                                {"\\\\^C"                          , "\u0108"}, // LATIN CAPITAL LETTER C WITH CIRCUMFLEX
                                                                                                {"\\\\^c"                          , "\u0109"}, // LATIN SMALL LETTER C WITH CIRCUMFLEX
                                                                                                {"\\\\.C"                          , "\u010A"}, // LATIN CAPITAL LETTER C WITH DOT ABOVE
                                                                                                {"\\\\.c"                          , "\u010B"}, // LATIN SMALL LETTER C WITH DOT ABOVE
                                                                                                {"\\\\vC"                          , "\u010C"}, // LATIN CAPITAL LETTER C WITH CARON
                                                                                                {"\\\\vc"                          , "\u010D"}, // LATIN SMALL LETTER C WITH CARON
                                                                                                {"\\\\vD"                          , "\u010E"}, // LATIN CAPITAL LETTER D WITH CARON
                                                                                                {"\\\\vd"                          , "\u010F"}, // LATIN SMALL LETTER D WITH CARON
                                                                                                {"\\\\=E"                          , "\u0112"}, // LATIN CAPITAL LETTER E WITH MACRON
                                                                                                {"\\\\=e"                          , "\u0113"}, // LATIN SMALL LETTER E WITH MACRON
                                                                                                {"\\\\uE"                          , "\u0114"}, // LATIN CAPITAL LETTER E WITH BREVE
                                                                                                {"\\\\ue"                          , "\u0115"}, // LATIN SMALL LETTER E WITH BREVE
                                                                                                {"\\\\.E"                          , "\u0116"}, // LATIN CAPITAL LETTER E WITH DOT ABOVE
                                                                                                {"\\\\.e"                          , "\u0117"}, // LATIN SMALL LETTER E WITH DOT ABOVE
                                                                                                {"\\\\kE"                          , "\u0118"}, // LATIN CAPITAL LETTER E WITH OGONEK
                                                                                                {"\\\\ke"                          , "\u0119"}, // LATIN SMALL LETTER E WITH OGONEK
                                                                                                {"\\\\vE"                          , "\u011A"}, // LATIN CAPITAL LETTER E WITH CARON
                                                                                                {"\\\\ve"                          , "\u011B"}, // LATIN SMALL LETTER E WITH CARON
                                                                                                {"\\\\^G"                          , "\u011C"}, // LATIN CAPITAL LETTER G WITH CIRCUMFLEX
                                                                                                {"\\\\^g"                          , "\u011D"}, // LATIN SMALL LETTER G WITH CIRCUMFLEX
                                                                                                {"\\\\uG"                          , "\u011E"}, // LATIN CAPITAL LETTER G WITH BREVE
                                                                                                {"\\\\ug"                          , "\u011F"}, // LATIN SMALL LETTER G WITH BREVE
                                                                                                {"\\\\.G"                          , "\u0120"}, // LATIN CAPITAL LETTER G WITH DOT ABOVE
                                                                                                {"\\\\.g"                          , "\u0121"}, // LATIN SMALL LETTER G WITH DOT ABOVE
                                                                                                {"\\\\cG"                          , "\u0122"}, // LATIN CAPITAL LETTER G WITH CEDILLA
                                                                                                {"\\\\cg"                          , "\u0123"}, // LATIN SMALL LETTER G WITH CEDILLA
                                                                                                {"\\\\^H"                          , "\u0124"}, // LATIN CAPITAL LETTER H WITH CIRCUMFLEX
                                                                                                {"\\\\^h"                          , "\u0125"}, // LATIN SMALL LETTER H WITH CIRCUMFLEX
                                                                                                {"\\\\~I"                          , "\u0128"}, // LATIN CAPITAL LETTER I WITH TILDE
                                                                                                {"\\\\~i"                          , "\u0129"}, // LATIN SMALL LETTER I WITH TILDE
                                                                                                {"\\\\=I"                          , "\u012A"}, // LATIN CAPITAL LETTER I WITH MACRON
                                                                                                {"\\\\=i"                          , "\u012B"}, // LATIN SMALL LETTER I WITH MACRON
                                                                                                {"\\\\uI"                          , "\u012C"}, // LATIN CAPITAL LETTER I WITH BREVE
                                                                                                {"\\\\ui"                          , "\u012D"}, // LATIN SMALL LETTER I WITH BREVE
                                                                                                {"\\\\kI"                          , "\u012E"}, // LATIN CAPITAL LETTER I WITH OGONEK
                                                                                                {"\\\\ki"                          , "\u012F"}, // LATIN SMALL LETTER I WITH OGONEK
                                                                                                {"\\\\.I"                          , "\u0130"}, // LATIN CAPITAL LETTER I WITH DOT ABOVE
                                                                                                {"\\\\^J"                          , "\u0134"}, // LATIN CAPITAL LETTER J WITH CIRCUMFLEX
                                                                                                {"\\\\^j"                          , "\u0135"}, // LATIN SMALL LETTER J WITH CIRCUMFLEX
                                                                                                {"\\\\cK"                          , "\u0136"}, // LATIN CAPITAL LETTER K WITH CEDILLA
                                                                                                {"\\\\ck"                          , "\u0137"}, // LATIN SMALL LETTER K WITH CEDILLA
                                                                                                {"\\\\'L"                          , "\u0139"}, // LATIN CAPITAL LETTER L WITH ACUTE
                                                                                                {"\\\\'l"                          , "\u013A"}, // LATIN SMALL LETTER L WITH ACUTE
                                                                                                {"\\\\cL"                          , "\u013B"}, // LATIN CAPITAL LETTER L WITH CEDILLA
                                                                                                {"\\\\cl"                          , "\u013C"}, // LATIN SMALL LETTER L WITH CEDILLA
                                                                                                {"\\\\vL"                          , "\u013D"}, // LATIN CAPITAL LETTER L WITH CARON
                                                                                                {"\\\\vl"                          , "\u013E"}, // LATIN SMALL LETTER L WITH CARON
                                                                                                {"\\\\L{"                           , "\u0141"}, //LATIN CAPITAL LETTER L WITH STROKE
                                                                                                {"\\\\l{"                           , "\u0142"}, //LATIN SMALL LETTER L WITH STROKE
                                                                                                {"\\\\'N"                          , "\u0143"}, // LATIN CAPITAL LETTER N WITH ACUTE
                                                                                                {"\\\\'n"                          , "\u0144"}, // LATIN SMALL LETTER N WITH ACUTE
                                                                                                {"\\\\cN"                          , "\u0145"}, // LATIN CAPITAL LETTER N WITH CEDILLA
                                                                                                {"\\\\cn"                          , "\u0146"}, // LATIN SMALL LETTER N WITH CEDILLA
                                                                                                {"\\\\vN"                          , "\u0147"}, // LATIN CAPITAL LETTER N WITH CARON
                                                                                                {"\\\\vn"                          , "\u0148"}, // LATIN SMALL LETTER N WITH CARON
                                                                                                {"\\\\=O"                          , "\u014C"}, // LATIN CAPITAL LETTER O WITH MACRON
                                                                                                {"\\\\=o"                          , "\u014D"}, // LATIN SMALL LETTER O WITH MACRON
                                                                                                {"\\\\uO"                          , "\u014E"}, // LATIN CAPITAL LETTER O WITH BREVE
                                                                                                {"\\\\uo"                          , "\u014F"}, // LATIN SMALL LETTER O WITH BREVE
                                                                                                {"\\\\HO"                          , "\u0150"}, // LATIN CAPITAL LETTER O WITH DOUBLE ACUTE
                                                                                                {"\\\\Ho"                          , "\u0151"}, // LATIN SMALL LETTER O WITH DOUBLE ACUTE
                                                                                                {"\\\\'R"                          , "\u0154"}, // LATIN CAPITAL LETTER R WITH ACUTE
                                                                                                {"\\\\'r"                          , "\u0155"}, // LATIN SMALL LETTER R WITH ACUTE
                                                                                                {"\\\\cR"                          , "\u0156"}, // LATIN CAPITAL LETTER R WITH CEDILLA
                                                                                                {"\\\\cr"                          , "\u0157"}, // LATIN SMALL LETTER R WITH CEDILLA
                                                                                                {"\\\\vR"                          , "\u0158"}, // LATIN CAPITAL LETTER R WITH CARON
                                                                                                {"\\\\vr"                          , "\u0159"}, // LATIN SMALL LETTER R WITH CARON
                                                                                                {"\\\\'S"                          , "\u015A"}, // LATIN CAPITAL LETTER S WITH ACUTE
                                                                                                {"\\\\'s"                          , "\u015B"}, // LATIN SMALL LETTER S WITH ACUTE
                                                                                                {"\\\\^S"                          , "\u015C"}, // LATIN CAPITAL LETTER S WITH CIRCUMFLEX
                                                                                                {"\\\\^s"                          , "\u015D"}, // LATIN SMALL LETTER S WITH CIRCUMFLEX
                                                                                                {"\\\\cS"                          , "\u015E"}, // LATIN CAPITAL LETTER S WITH CEDILLA
                                                                                                {"\\\\cs"                          , "\u015F"}, // LATIN SMALL LETTER S WITH CEDILLA
                                                                                                {"\\\\vS"                          , "\u0160"}, // LATIN CAPITAL LETTER S WITH CARON
                                                                                                {"\\\\vs"                          , "\u0161"}, // LATIN SMALL LETTER S WITH CARON
                                                                                                {"\\\\cT"                          , "\u0162"}, // LATIN CAPITAL LETTER T WITH CEDILLA
                                                                                                {"\\\\ct"                          , "\u0163"}, // LATIN SMALL LETTER T WITH CEDILLA
                                                                                                {"\\\\vT"                          , "\u0164"}, // LATIN CAPITAL LETTER T WITH CARON
                                                                                                {"\\\\vt"                          , "\u0165"}, // LATIN SMALL LETTER T WITH CARON
                                                                                                {"\\\\~U"                          , "\u0168"}, // LATIN CAPITAL LETTER U WITH TILDE
                                                                                                {"\\\\~u"                          , "\u0169"}, // LATIN SMALL LETTER U WITH TILDE
                                                                                                {"\\\\=U"                          , "\u016A"}, // LATIN CAPITAL LETTER U WITH MACRON
                                                                                                {"\\\\=u"                          , "\u016B"}, // LATIN SMALL LETTER U WITH MACRON
                                                                                                {"\\\\uU"                          , "\u016C"}, // LATIN CAPITAL LETTER U WITH BREVE
                                                                                                {"\\\\uu"                          , "\u016D"}, // LATIN SMALL LETTER U WITH BREVE
                                                                                                {"\\\\HU"                          , "\u0170"}, // LATIN CAPITAL LETTER U WITH DOUBLE ACUTE
                                                                                                {"\\\\Hu"                          , "\u0171"}, // LATIN SMALL LETTER U WITH DOUBLE ACUTE
                                                                                                {"\\\\kU"                          , "\u0172"}, // LATIN CAPITAL LETTER U WITH OGONEK
                                                                                                {"\\\\ku"                          , "\u0173"}, // LATIN SMALL LETTER U WITH OGONEK
                                                                                                {"\\\\^W"                          , "\u0174"}, // LATIN CAPITAL LETTER W WITH CIRCUMFLEX
                                                                                                {"\\\\^w"                          , "\u0175"}, // LATIN SMALL LETTER W WITH CIRCUMFLEX
                                                                                                {"\\\\^Y"                          , "\u0176"}, // LATIN CAPITAL LETTER Y WITH CIRCUMFLEX
                                                                                                {"\\\\^y"                          , "\u0177"}, // LATIN SMALL LETTER Y WITH CIRCUMFLEX
                                                                                                {"\\\\\"Y"                         , "\u0178"}, // LATIN CAPITAL LETTER Y WITH DIAERESIS
                                                                                                {"\\\\'Z"                          , "\u0179"}, // LATIN CAPITAL LETTER Z WITH ACUTE
                                                                                                {"\\\\'z"                          , "\u017A"}, // LATIN SMALL LETTER Z WITH ACUTE
                                                                                                {"\\\\.Z"                          , "\u017B"}, // LATIN CAPITAL LETTER Z WITH DOT ABOVE
                                                                                                {"\\\\.z"                          , "\u017C"}, // LATIN SMALL LETTER Z WITH DOT ABOVE
                                                                                                {"\\\\vZ"                          , "\u017D"}, // LATIN CAPITAL LETTER Z WITH CARON
                                                                                                {"\\\\vz"                          , "\u017E"}, // LATIN SMALL LETTER Z WITH CARON
                                                                                                {"\\\\vA"                          , "\u01CD"}, // LATIN CAPITAL LETTER A WITH CARON
                                                                                                {"\\\\va"                          , "\u01CE"}, // LATIN SMALL LETTER A WITH CARON
                                                                                                {"\\\\vI"                          , "\u01CF"}, // LATIN CAPITAL LETTER I WITH CARON
                                                                                                {"\\\\vi"                          , "\u01D0"}, // LATIN SMALL LETTER I WITH CARON
                                                                                                {"\\\\vO"                          , "\u01D1"}, // LATIN CAPITAL LETTER O WITH CARON
                                                                                                {"\\\\vo"                          , "\u01D2"}, // LATIN SMALL LETTER O WITH CARON
                                                                                                {"\\\\vU"                          , "\u01D3"}, // LATIN CAPITAL LETTER U WITH CARON
                                                                                                {"\\\\vu"                          , "\u01D4"}, // LATIN SMALL LETTER U WITH CARON
                                                                                                {"\\\\vG"                          , "\u01E6"}, // LATIN CAPITAL LETTER G WITH CARON
                                                                                                {"\\\\vg"                          , "\u01E7"}, // LATIN SMALL LETTER G WITH CARON
                                                                                                {"\\\\vK"                          , "\u01E8"}, // LATIN CAPITAL LETTER K WITH CARON
                                                                                                {"\\\\vk"                          , "\u01E9"}, // LATIN SMALL LETTER K WITH CARON
                                                                                                {"\\\\kO"                          , "\u01EA"}, // LATIN CAPITAL LETTER O WITH OGONEK
                                                                                                {"\\\\ko"                          , "\u01EB"}, // LATIN SMALL LETTER O WITH OGONEK
                                                                                                {"\\\\vj"                          , "\u01F0"}, // LATIN SMALL LETTER J WITH CARON
                                                                                                {"\\\\'G"                          , "\u01F4"}, // LATIN CAPITAL LETTER G WITH ACUTE
                                                                                                {"\\\\'g"                          , "\u01F5"}, // LATIN SMALL LETTER G WITH ACUTE
                                                                                                {"\\\\.B"                          , "\u1E02"}, // LATIN CAPITAL LETTER B WITH DOT ABOVE
                                                                                                {"\\\\.b"                          , "\u1E03"}, // LATIN SMALL LETTER B WITH DOT ABOVE
                                                                                                {"\\\\dB"                          , "\u1E04"}, // LATIN CAPITAL LETTER B WITH DOT BELOW
                                                                                                {"\\\\db"                          , "\u1E05"}, // LATIN SMALL LETTER B WITH DOT BELOW
                                                                                                {"\\\\bB"                          , "\u1E06"}, // LATIN CAPITAL LETTER B WITH LINE BELOW
                                                                                                {"\\\\bb"                          , "\u1E07"}, // LATIN SMALL LETTER B WITH LINE BELOW
                                                                                                {"\\\\.D"                          , "\u1E0A"}, // LATIN CAPITAL LETTER D WITH DOT ABOVE
                                                                                                {"\\\\.d"                          , "\u1E0B"}, // LATIN SMALL LETTER D WITH DOT ABOVE
                                                                                                {"\\\\dD"                          , "\u1E0C"}, // LATIN CAPITAL LETTER D WITH DOT BELOW
                                                                                                {"\\\\dd"                          , "\u1E0D"}, // LATIN SMALL LETTER D WITH DOT BELOW
                                                                                                {"\\\\bD"                          , "\u1E0E"}, // LATIN CAPITAL LETTER D WITH LINE BELOW
                                                                                                {"\\\\bd"                          , "\u1E0F"}, // LATIN SMALL LETTER D WITH LINE BELOW
                                                                                                {"\\\\cD"                          , "\u1E10"}, // LATIN CAPITAL LETTER D WITH CEDILLA
                                                                                                {"\\\\cd"                          , "\u1E11"}, // LATIN SMALL LETTER D WITH CEDILLA
                                                                                                {"\\\\.F"                          , "\u1E1E"}, // LATIN CAPITAL LETTER F WITH DOT ABOVE
                                                                                                {"\\\\.f"                          , "\u1E1F"}, // LATIN SMALL LETTER F WITH DOT ABOVE
                                                                                                {"\\\\=G"                          , "\u1E20"}, // LATIN CAPITAL LETTER G WITH MACRON
                                                                                                {"\\\\=g"                          , "\u1E21"}, // LATIN SMALL LETTER G WITH MACRON
                                                                                                {"\\\\.H"                          , "\u1E22"}, // LATIN CAPITAL LETTER H WITH DOT ABOVE
                                                                                                {"\\\\.h"                          , "\u1E23"}, // LATIN SMALL LETTER H WITH DOT ABOVE
                                                                                                {"\\\\dH"                          , "\u1E24"}, // LATIN CAPITAL LETTER H WITH DOT BELOW
                                                                                                {"\\\\dh"                          , "\u1E25"}, // LATIN SMALL LETTER H WITH DOT BELOW
                                                                                                {"\\\\\"H"                         , "\u1E26"}, // LATIN CAPITAL LETTER H WITH DIAERESIS
                                                                                                {"\\\\\"h"                         , "\u1E27"}, // LATIN SMALL LETTER H WITH DIAERESIS
                                                                                                {"\\\\cH"                          , "\u1E28"}, // LATIN CAPITAL LETTER H WITH CEDILLA
                                                                                                {"\\\\ch"                          , "\u1E29"}, // LATIN SMALL LETTER H WITH CEDILLA
                                                                                                {"\\\\'K"                          , "\u1E30"}, // LATIN CAPITAL LETTER K WITH ACUTE
                                                                                                {"\\\\'k"                          , "\u1E31"}, // LATIN SMALL LETTER K WITH ACUTE
                                                                                                {"\\\\dK"                          , "\u1E32"}, // LATIN CAPITAL LETTER K WITH DOT BELOW
                                                                                                {"\\\\dk"                          , "\u1E33"}, // LATIN SMALL LETTER K WITH DOT BELOW
                                                                                                {"\\\\bK"                          , "\u1E34"}, // LATIN CAPITAL LETTER K WITH LINE BELOW
                                                                                                {"\\\\bk"                          , "\u1E35"}, // LATIN SMALL LETTER K WITH LINE BELOW
                                                                                                {"\\\\dL"                          , "\u1E36"}, // LATIN CAPITAL LETTER L WITH DOT BELOW
                                                                                                {"\\\\dl"                          , "\u1E37"}, // LATIN SMALL LETTER L WITH DOT BELOW
                                                                                                {"\\\\bL"                          , "\u1E3A"}, // LATIN CAPITAL LETTER L WITH LINE BELOW
                                                                                                {"\\\\bl"                          , "\u1E3B"}, // LATIN SMALL LETTER L WITH LINE BELOW
                                                                                                {"\\\\'M"                          , "\u1E3E"}, // LATIN CAPITAL LETTER M WITH ACUTE
                                                                                                {"\\\\'m"                          , "\u1E3F"}, // LATIN SMALL LETTER M WITH ACUTE
                                                                                                {"\\\\.M"                          , "\u1E40"}, // LATIN CAPITAL LETTER M WITH DOT ABOVE
                                                                                                {"\\\\.m"                          , "\u1E41"}, // LATIN SMALL LETTER M WITH DOT ABOVE
                                                                                                {"\\\\dM"                          , "\u1E42"}, // LATIN CAPITAL LETTER M WITH DOT BELOW
                                                                                                {"\\\\dm"                          , "\u1E43"}, // LATIN SMALL LETTER M WITH DOT BELOW
                                                                                                {"\\\\.N"                          , "\u1E44"}, // LATIN CAPITAL LETTER N WITH DOT ABOVE
                                                                                                {"\\\\.n"                          , "\u1E45"}, // LATIN SMALL LETTER N WITH DOT ABOVE
                                                                                                {"\\\\dN"                          , "\u1E46"}, // LATIN CAPITAL LETTER N WITH DOT BELOW
                                                                                                {"\\\\dn"                          , "\u1E47"}, // LATIN SMALL LETTER N WITH DOT BELOW
                                                                                                {"\\\\bN"                          , "\u1E48"}, // LATIN CAPITAL LETTER N WITH LINE BELOW
                                                                                                {"\\\\bn"                          , "\u1E49"}, // LATIN SMALL LETTER N WITH LINE BELOW
                                                                                                {"\\\\'P"                          , "\u1E54"}, // LATIN CAPITAL LETTER P WITH ACUTE
                                                                                                {"\\\\'p"                          , "\u1E55"}, // LATIN SMALL LETTER P WITH ACUTE
                                                                                                {"\\\\.P"                          , "\u1E56"}, // LATIN CAPITAL LETTER P WITH DOT ABOVE
                                                                                                {"\\\\.p"                          , "\u1E57"}, // LATIN SMALL LETTER P WITH DOT ABOVE
                                                                                                {"\\\\.R"                          , "\u1E58"}, // LATIN CAPITAL LETTER R WITH DOT ABOVE
                                                                                                {"\\\\.r"                          , "\u1E59"}, // LATIN SMALL LETTER R WITH DOT ABOVE
                                                                                                {"\\\\dR"                          , "\u1E5A"}, // LATIN CAPITAL LETTER R WITH DOT BELOW
                                                                                                {"\\\\dr"                          , "\u1E5B"}, // LATIN SMALL LETTER R WITH DOT BELOW
                                                                                                {"\\\\bR"                          , "\u1E5E"}, // LATIN CAPITAL LETTER R WITH LINE BELOW
                                                                                                {"\\\\br"                          , "\u1E5F"}, // LATIN SMALL LETTER R WITH LINE BELOW
                                                                                                {"\\\\.S"                          , "\u1E60"}, // LATIN CAPITAL LETTER S WITH DOT ABOVE
                                                                                                {"\\\\.s"                          , "\u1E61"}, // LATIN SMALL LETTER S WITH DOT ABOVE
                                                                                                {"\\\\dS"                          , "\u1E62"}, // LATIN CAPITAL LETTER S WITH DOT BELOW
                                                                                                {"\\\\ds"                          , "\u1E63"}, // LATIN SMALL LETTER S WITH DOT BELOW
                                                                                                {"\\\\.T"                          , "\u1E6A"}, // LATIN CAPITAL LETTER T WITH DOT ABOVE
                                                                                                {"\\\\.t"                          , "\u1E6B"}, // LATIN SMALL LETTER T WITH DOT ABOVE
                                                                                                {"\\\\dT"                          , "\u1E6C"}, // LATIN CAPITAL LETTER T WITH DOT BELOW
                                                                                                {"\\\\dt"                          , "\u1E6D"}, // LATIN SMALL LETTER T WITH DOT BELOW
                                                                                                {"\\\\bT"                          , "\u1E6E"}, // LATIN CAPITAL LETTER T WITH LINE BELOW
                                                                                                {"\\\\bt"                          , "\u1E6F"}, // LATIN SMALL LETTER T WITH LINE BELOW
                                                                                                {"\\\\~V"                          , "\u1E7C"}, // LATIN CAPITAL LETTER V WITH TILDE
                                                                                                {"\\\\~v"                          , "\u1E7D"}, // LATIN SMALL LETTER V WITH TILDE
                                                                                                {"\\\\dV"                          , "\u1E7E"}, // LATIN CAPITAL LETTER V WITH DOT BELOW
                                                                                                {"\\\\dv"                          , "\u1E7F"}, // LATIN SMALL LETTER V WITH DOT BELOW
                                                                                                {"\\\\`W"                          , "\u1E80"}, // LATIN CAPITAL LETTER W WITH GRAVE
                                                                                                {"\\\\`w"                          , "\u1E81"}, // LATIN SMALL LETTER W WITH GRAVE
                                                                                                {"\\\\'W"                          , "\u1E82"}, // LATIN CAPITAL LETTER W WITH ACUTE
                                                                                                {"\\\\'w"                          , "\u1E83"}, // LATIN SMALL LETTER W WITH ACUTE
                                                                                                {"\\\\\"W"                         , "\u1E84"}, // LATIN CAPITAL LETTER W WITH DIAERESIS
                                                                                                {"\\\\\"w"                         , "\u1E85"}, // LATIN SMALL LETTER W WITH DIAERESIS
                                                                                                {"\\\\.W"                          , "\u1E86"}, // LATIN CAPITAL LETTER W WITH DOT ABOVE
                                                                                                {"\\\\.w"                          , "\u1E87"}, // LATIN SMALL LETTER W WITH DOT ABOVE
                                                                                                {"\\\\dW"                          , "\u1E88"}, // LATIN CAPITAL LETTER W WITH DOT BELOW
                                                                                                {"\\\\dw"                          , "\u1E89"}, // LATIN SMALL LETTER W WITH DOT BELOW
                                                                                                {"\\\\.X"                          , "\u1E8A"}, // LATIN CAPITAL LETTER X WITH DOT ABOVE
                                                                                                {"\\\\.x"                          , "\u1E8B"}, // LATIN SMALL LETTER X WITH DOT ABOVE
                                                                                                {"\\\\\"X"                         , "\u1E8C"}, // LATIN CAPITAL LETTER X WITH DIAERESIS
                                                                                                {"\\\\\"x"                         , "\u1E8D"}, // LATIN SMALL LETTER X WITH DIAERESIS
                                                                                                {"\\\\.Y"                          , "\u1E8E"}, // LATIN CAPITAL LETTER Y WITH DOT ABOVE
                                                                                                {"\\\\.y"                          , "\u1E8F"}, // LATIN SMALL LETTER Y WITH DOT ABOVE
                                                                                                {"\\\\^Z"                          , "\u1E90"}, // LATIN CAPITAL LETTER Z WITH CIRCUMFLEX
                                                                                                {"\\\\^z"                          , "\u1E91"}, // LATIN SMALL LETTER Z WITH CIRCUMFLEX
                                                                                                {"\\\\dZ", "\u1E92"}, // LATIN CAPITAL LETTER Z WITH DOT BELOW
                                                                                                {"\\\\dz", "\u1E93"}, // LATIN SMALL LETTER Z WITH DOT BELOW
                                                                                                {"\\\\bZ", "\u1E94"}, // LATIN CAPITAL LETTER Z WITH LINE BELOW
                                                                                                {"\\\\bz", "\u1E95"}, // LATIN SMALL LETTER Z WITH LINE BELOW
                                                                                                {"\\\\bh", "\u1E96"}, // LATIN SMALL LETTER H WITH LINE BELOW
                                                                                                {"\\\\\"t", "\u1E97"}, // LATIN SMALL LETTER T WITH DIAERESIS
                                                                                                {"\\\\dA", "\u1EA0"}, // LATIN CAPITAL LETTER A WITH DOT BELOW
                                                                                                {"\\\\da", "\u1EA1"}, // LATIN SMALL LETTER A WITH DOT BELOW
                                                                                                {"\\\\dE", "\u1EB8"}, // LATIN CAPITAL LETTER E WITH DOT BELOW
                                                                                                {"\\\\de", "\u1EB9"}, // LATIN SMALL LETTER E WITH DOT BELOW
                                                                                                {"\\\\~E", "\u1EBC"}, // LATIN CAPITAL LETTER E WITH TILDE
                                                                                                {"\\\\~e", "\u1EBD"}, // LATIN SMALL LETTER E WITH TILDE
                                                                                                {"\\\\dI", "\u1ECA"}, // LATIN CAPITAL LETTER I WITH DOT BELOW
                                                                                                {"\\\\di", "\u1ECB"}, // LATIN SMALL LETTER I WITH DOT BELOW
                                                                                                {"\\\\dO", "\u1ECC"}, // LATIN CAPITAL LETTER O WITH DOT BELOW
                                                                                                {"\\\\do", "\u1ECD"}, // LATIN SMALL LETTER O WITH DOT BELOW
                                                                                                {"\\\\dU", "\u1EE4"}, // LATIN CAPITAL LETTER U WITH DOT BELOW
                                                                                                {"\\\\du", "\u1EE5"}, // LATIN SMALL LETTER U WITH DOT BELOW
                                                                                                {"\\\\`Y", "\u1EF2"}, // LATIN CAPITAL LETTER Y WITH GRAVE
                                                                                                {"\\\\`y", "\u1EF3"}, // LATIN SMALL LETTER Y WITH GRAVE
                                                                                                {"\\\\dY", "\u1EF4"}, // LATIN CAPITAL LETTER Y WITH DOT BELOW
                                                                                                {"\\\\dy", "\u1EF5"}, // LATIN SMALL LETTER Y WITH DOT BELOW
                                                                                                {"\\\\~Y", "\u1EF8"}, // LATIN CAPITAL LETTER Y WITH TILDE
                                                                                                {"\\\\~y", "\u1EF9"}  // LATIN SMALL LETTER Y WITH TILDE
                                                                                                };
            this.mappingLatex2UnicodeTable = mappingLatex2UnicodeTable;
        }

        private void _SetMapBibTex2WizFolio()
        {
            Dictionary<string, ItemTypes> mapBibTex2WizFolio = new Dictionary<string, ItemTypes>{ {"book", ItemTypes.BookWhole},
                                                                                                                    {"inbook", ItemTypes.BookChapter},
                                                                                                                    {"booklet", ItemTypes.BookChapter},
                                                                                                                    {"article", ItemTypes.JournalArticle},
                                                                                                                    {"unpublished",ItemTypes.JournalArticle},
                                                                                                                    {"patent", ItemTypes.Patent},
                                                                                                                    {"inproceedings", ItemTypes.Proceeding},
                                                                                                                    {"conference", ItemTypes.Proceeding},
                                                                                                                    {"proceedings", ItemTypes.Proceeding},
                                                                                                                    {"phdthesis", ItemTypes.Thesis},
                                                                                                                    {"mastersthesis", ItemTypes.Thesis},
                                                                                                                    {"techreport", ItemTypes.Thesis},
                                                                                                                    {"manual", ItemTypes.Document},
                                                                                                                    {"collection", ItemTypes.Document},
                                                                                                                    {"misc", ItemTypes.Document}
                                                                                                                };
            this.mappingBibTex2WizFolio = mapBibTex2WizFolio;
        }
    }
}
