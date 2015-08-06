using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using MSWordpiCite.Entities;
using MSWordpiCite.Enums;

namespace MSWordpiCite.Search
{
    class WorldCat : BaseClass
    {
        const string WORLDCAT_SEARCHURL = "http://worldcat.org/webservices/catalog/search/opensearch?q={term}&format=atom&start={start}&count={count}&wskey={key}";
        const string WORLDCAT_APIKEY = "Zw7eOwVKBW1MvQ4XZzr444jwZcUQB6xtUPKx4ZLRbCYpAbonh7gUNOekxjIE36xFvuDr0si5GpDhr5HZ";
        const string WORLDCAT_ITEMURL = "http://www.worldcat.org/webservices/catalog/content/{identifier}?recordSchema=info%3Asrw%2Fschema%2F1%2Fdc&servicelevel=full&wskey={key}";

        public WorldCat(string query, int count) : base(query, count) { }
        
        public WorldCat(string query) : base(query) { }
                
        public WorldCat() { }
                
        public Dictionary<string, object> GetResults(int iStart)
        {
            if (SearchQuery == string.Empty || SearchQuery.Length == 0)
                return null;

            Dictionary<string, object> Data = new Dictionary<string, object>();

            string strWCSearchURL = WORLDCAT_SEARCHURL.Replace("{term}", HttpUtility.UrlEncode(SearchQuery));
            strWCSearchURL = strWCSearchURL.Replace("{start}", iStart + "");
            strWCSearchURL = strWCSearchURL.Replace("{count}", NumberResultsPerPage + "");
            strWCSearchURL = strWCSearchURL.Replace("{key}", WORLDCAT_APIKEY);
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strWCSearchURL);
                XmlNamespaceManager nm = new XmlNamespaceManager(xmlDoc.NameTable);
                nm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                nm.AddNamespace("opensearch", "http://a9.com/-/spec/opensearch/1.1/");
                //Find Total Number
                List<ItemMasterRow> listItems = new List<ItemMasterRow>();
                XmlNode nodeTotalNumber = xmlDoc.SelectSingleNode("//opensearch:totalResults", nm);
                if (nodeTotalNumber != null)
                {
                    Data["Count"] = nodeTotalNumber.InnerText;
                }

                XmlNodeList resultNodes = _FindResultNodes(xmlDoc, nm);
                if (resultNodes != null)
                {
                    foreach (XmlNode node in resultNodes)
                    {
                        ItemMasterRow item = new ItemMasterRow();
                        XmlDocument newXmlDoc = new XmlDocument();
                        newXmlDoc.LoadXml(node.OuterXml);
                        nm = new XmlNamespaceManager(newXmlDoc.NameTable);
                        nm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                        nm.AddNamespace("opensearch", "http://a9.com/-/spec/opensearch/1.1/");
                        nm.AddNamespace("oclcterms", "http://purl.org/oclc/terms/");
                        if (_ProcessingNode(newXmlDoc, nm, ref item))
                        {
                            listItems.Add(item);
                        }
                    }
                }
                if (listItems.Count > 0)
                {
                    Data["ItemList"] = listItems;
                }
            }
            catch
            {
                Data["Count"] = "0";
            }

            return Data;
        }
                
        private bool _ProcessingNode(XmlNode ProcessingNode, XmlNamespaceManager nm, ref ItemMasterRow item)
        {
            try
            {
                item.ItemTypeID = ItemTypes.BookWhole;
                XmlNode node = ProcessingNode.SelectSingleNode("//atom:title", nm);
                if (node != null)
                    item.Title = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//atom:link", nm);
                if (node != null)
                    item.Links += "mainLink|" + node.Attributes["href"].Value + "|";

                node = ProcessingNode.SelectSingleNode("//atom:summary", nm);
                if (node != null)
                {
                    string strContent = node.InnerText;
                    item.Abstract = strContent;                    
                }

                node = ProcessingNode.SelectSingleNode("//oclcterms:recordIdentifier", nm);
                if (node != null)
                {
                    string id = node.InnerText;
                    string strItemDetailsURL = WORLDCAT_ITEMURL.Replace("{identifier}", id);
                    strItemDetailsURL = strItemDetailsURL.Replace("{key}", WORLDCAT_APIKEY);
                    XmlDocument xmlItemDoc = new XmlDocument();
                    xmlItemDoc.Load(strItemDetailsURL);
                    XmlNamespaceManager nmItem = new XmlNamespaceManager(xmlItemDoc.NameTable);
                    nmItem.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

                    int count = 0;
                    foreach (XmlNode sNode in xmlItemDoc.SelectNodes("//dc:creator", nmItem))
                    {
                        string text = sNode.InnerText;
                        string[] split = Regex.Split(text, @"\s*\,\s*");
                        if(split.Length > 1)
                        {
                            NameMasterRow nmr = new NameMasterRow();
                            nmr.NameTypeID = NameTypes.Author;
                            nmr.SequenceNo = count;
                            count++;
                            nmr.LastName = Regex.Replace(split[0], @"^\s+|\s+$|\.\s*$", "");
                            nmr.ForeName = Regex.Replace(split[1], @"^\s+|\s+$|\.\s*$", "");
                            item.Authors.Add(nmr);
                        }
                    }
                    foreach (XmlNode sNode in xmlItemDoc.SelectNodes("//dc:contributor", nmItem))
                    {
                        string text = sNode.InnerText;
                        string[] split = Regex.Split(text, @"\s*\,\s*");
                        if (split.Length > 1)
                        {
                            NameMasterRow nmr = new NameMasterRow();
                            nmr.NameTypeID = NameTypes.Author;
                            nmr.SequenceNo = count;
                            count++;
                            nmr.LastName = Regex.Replace(split[0], @"^\s+|\s+$|\.\s*$", "");
                            nmr.ForeName = Regex.Replace(split[1], @"^\s+|\s+$|\.\s*$", "");
                            item.Authors.Add(nmr);
                        }
                    }
                    foreach (XmlNode sNode in xmlItemDoc.SelectNodes("//dc:publisher", nmItem))
                    {
                        NameMasterRow nmr = new NameMasterRow();
                        nmr.NameTypeID = NameTypes.Publisher;
                        nmr.SequenceNo = count;
                        count++;
                        nmr.LastName = Regex.Replace(sNode.InnerText, @"^\s+|\s+$|\.\s*$", "");
                        item.Authors.Add(nmr);
                    }

                    XmlNode tempNode = xmlItemDoc.SelectSingleNode("//dc:date", nmItem);
                    if(tempNode != null)
                    {
                        if (Regex.Match(tempNode.InnerText, @"\d{4}").Success)
                        {
                            item.PubYear = item.PubDate = Regex.Match(tempNode.InnerText, @"\d{4}").Value;
                        }
                    }

                    tempNode = xmlItemDoc.SelectSingleNode("//dc:format", nmItem);
                    if(tempNode != null)
                    {
                        if (Regex.Match(tempNode.InnerText, @"\d+\sp\.\s").Success)
                        {
                            string temp = Regex.Match(tempNode.InnerText, @"\d+\sp\.\s").Value;
                            item.Pages = Regex.Match(temp, @"\d+").Value;
                        }
                    }

                    //XmlNodeList listKeyNodes = xmlItemDoc.SelectNodes("//dc:subject", nmItem);
                    //foreach (XmlNode sNode in listKeyNodes)
                    //{
                    //    if (sNode.Attributes["xsi:type"].Value.Contains("LCSH"))
                    //        item.Keywords += sNode.InnerText + "|";
                    //}
                }

                return true;
            }
            catch
            { }

            return false;
        }
                
        private XmlNodeList _FindResultNodes(XmlDocument xmlDoc, XmlNamespaceManager nm)
        {
            return xmlDoc.DocumentElement.SelectNodes("//atom:entry", nm);
        }
    }
}
