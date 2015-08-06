using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using MSWordpiCite.Enums;
using MSWordpiCite.Entities;

namespace MSWordpiCite.Search
{
    class ScholarsPortal : BaseClass
    {
        const string SCHOLARSPORTAL_SEARCHURL = "http://api.scholarsportal.info/journals/opensearch/1.1/search.xqy?q={term}&p={page}&c={number}";
        public ScholarsPortal(string query) : base(query) { }

        public ScholarsPortal() { }

        public Dictionary<string, object> GetResults(int iStart)
        {
            if (SearchQuery == string.Empty || SearchQuery.Length == 0)
                return null;

            Dictionary<string, object> Data = new Dictionary<string, object>();

            int iPageNumber = (int)Math.Floor((double)(iStart / Properties.Settings.Default.NUM_OF_SEARCH_RESULTS_PER_PAGE)) + 1;

            string strSPSearchURL = SCHOLARSPORTAL_SEARCHURL.Replace("{term}", HttpUtility.UrlEncode(SearchQuery));
            strSPSearchURL = strSPSearchURL.Replace("{page}", iPageNumber + "");
            strSPSearchURL = strSPSearchURL.Replace("{number}", "20");
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strSPSearchURL);
                XmlNamespaceManager nm = new XmlNamespaceManager(xmlDoc.NameTable);
                nm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                nm.AddNamespace("os", "http://a9.com/-/spec/opensearch/1.1/");
                nm.AddNamespace("sp", "http://scholarsportal.info/metadata");
                //Find Total Number
                List<ItemMasterRow> listItems = new List<ItemMasterRow>();
                XmlNode nodeTotalNumber = xmlDoc.SelectSingleNode("//os:totalResults", nm);
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
                        nm.AddNamespace("os", "http://a9.com/-/spec/opensearch/1.1/");
                        nm.AddNamespace("sp", "http://scholarsportal.info/metadata");
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

        /// <summary>
        /// _s the processing node.
        /// </summary>
        /// <param name="ProcessingNode">The processing node.</param>
        /// <param name="nm">The nm.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool _ProcessingNode(XmlNode ProcessingNode, XmlNamespaceManager nm, ref ItemMasterRow item)
        {
            try
            {
                item.ItemTypeID = ItemTypes.JournalArticle;
                XmlNode node = ProcessingNode.SelectSingleNode("//atom:title", nm);
                if (node != null)
                    item.Title = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//atom:link", nm);
                if (node != null)
                    item.Links += "mainLink|" + node.Attributes["href"].Value + "|";

                nm.AddNamespace("atom", "http://scholarsportal.info/metadata");

                node = ProcessingNode.SelectSingleNode("//atom:article/atom:publication-date", nm);
                if (node != null)
                {
                    item.PubDate = node.InnerText;
                    item.PubYear = Regex.Match(item.PubDate, @"\d{4}").Value;
                }

                node = ProcessingNode.SelectSingleNode("//atom:journal/atom:title", nm);
                if (node != null)
                    item.Title2 = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//atom:journal/atom:volume", nm);
                if (node != null)
                    item.Volume = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//atom:journal/atom:issue", nm);
                if (node != null)
                    item.Volume2 = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//atom:pages", nm);
                if (node != null)
                {
                    XmlNode fpageNode = node.SelectSingleNode("atom:fpage", nm);
                    XmlNode lpageNode = node.SelectSingleNode("atom:lpage", nm);
                    if (fpageNode != null && lpageNode != null)
                    {
                        item.Pages = fpageNode.InnerText + "-" + lpageNode.InnerText;
                    }
                }

                node = ProcessingNode.SelectSingleNode("//atom:journal/atom:issn-ppub", nm);
                if (node != null)
                {
                    item.ID1 = node.InnerText;
                }
                int count = 0;
                foreach (XmlNode sNode in ProcessingNode.SelectNodes("//atom:article/atom:authors/atom:author", nm))
                {
                    XmlNode givenNameNode = sNode.SelectSingleNode("atom:givenname", nm);
                    XmlNode surNameNode = sNode.SelectSingleNode("atom:surname", nm);
                    if (givenNameNode != null && surNameNode != null)
                    {
                        NameMasterRow nmr = new NameMasterRow();
                        nmr.NameTypeID = NameTypes.Author;
                        nmr.SequenceNo = count;
                        count++;
                        nmr.LastName = surNameNode.InnerText;
                        nmr.ForeName = givenNameNode.InnerText;
                        item.Authors.Add(nmr);
                    }
                }

                node = ProcessingNode.SelectSingleNode("//atom:journal/atom:publisher", nm);
                if (node != null)
                {
                    NameMasterRow nmr = new NameMasterRow();
                    nmr.NameTypeID = NameTypes.Publisher;
                    nmr.LastName = node.InnerText;
                    nmr.ForeName = "";
                    item.Authors.Add(nmr);
                }

                node = ProcessingNode.SelectSingleNode("//atom:article/atom:abstract", nm);
                if (node != null)
                {
                    string strContent = node.InnerText;
                    item.Abstract = strContent;
                }

                XmlNodeList nodesKeyword = ProcessingNode.SelectNodes("//atom:article/atom:keywords/atom:keyword", nm);
                foreach (XmlNode keyword in nodesKeyword)
                {
                    item.Keywords += keyword.InnerText + "|";
                }

                return true;
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// Find result nodes.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private XmlNodeList _FindResultNodes(XmlDocument xmlDoc, XmlNamespaceManager nm)
        {
            return xmlDoc.DocumentElement.SelectNodes("//atom:entry", nm);
        }
    }
}