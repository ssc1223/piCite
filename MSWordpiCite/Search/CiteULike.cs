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
    class CiteULike : BaseClass
    {
        const string __USERAGENT = "wizfolio";
        const string CITEULIKE_NUMOFRESULTSPERPAGE = "50";
        const string CITEULIKE_SEARCHURL = "http://www.citeulike.org/rss/search/all/page/{page}?t=1&q={term}";

        public CiteULike(string query) : base(query){}

        public CiteULike() { }

        public Dictionary<string, object> GetResults(int iStart)
        {
            if (SearchQuery == string.Empty || SearchQuery.Length == 0)
                return null;

            Dictionary<string, object> Data = new Dictionary<string, object>();

            int iPageNumber = (int)Math.Floor((double)(iStart / int.Parse(CITEULIKE_NUMOFRESULTSPERPAGE))) + 1;
            string strSearchURL = CITEULIKE_SEARCHURL.Replace("{term}", HttpUtility.UrlEncode(SearchQuery));
            strSearchURL = strSearchURL.Replace("{page}", iPageNumber + "");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(strSearchURL);
            XmlNamespaceManager nm = new XmlNamespaceManager(xmlDoc.NameTable);
            nm.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            nm.AddNamespace("rss", "http://purl.org/rss/1.0/");
            nm.AddNamespace("rdfs", "http://www.w3.org/2000/01/rdf-schema#");
            nm.AddNamespace("prism", "http://prismstandard.org/namespaces/1.2/basic/");
            nm.AddNamespace("dcterms", "http://purl.org/dc/terms/");
            nm.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            List<ItemMasterRow> listItems = new List<ItemMasterRow>();
            XmlNodeList resultNodes = _FindResultNodes(xmlDoc, nm);
            if (resultNodes != null)
            {
                for (int i = 0; i < resultNodes.Count; i++)
                {
                    XmlNode node = resultNodes[i];
                    ItemMasterRow item = new ItemMasterRow();
                    XmlDocument newXmlDoc = new XmlDocument();
                    newXmlDoc.LoadXml(node.OuterXml);
                    nm = new XmlNamespaceManager(newXmlDoc.NameTable);
                    nm.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                    nm.AddNamespace("rss", "http://purl.org/rss/1.0/");
                    nm.AddNamespace("rdfs", "http://www.w3.org/2000/01/rdf-schema#");
                    nm.AddNamespace("prism", "http://prismstandard.org/namespaces/1.2/basic/");
                    nm.AddNamespace("dcterms", "http://purl.org/dc/terms/");
                    nm.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
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
            return Data;
        }

        /// <summary>
        /// _s the processing node.
        /// </summary>
        /// <param name="ProcessingNode">The processing node.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool _ProcessingNode(XmlNode ProcessingNode, XmlNamespaceManager nm, ref ItemMasterRow item)
        {
            try
            {
                item.ItemTypeID = ItemTypes.JournalArticle;
                XmlNode node = ProcessingNode.SelectSingleNode("//rss:title", nm);
                if(node!=null)
                    item.Title = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//rss:link", nm);
                if (node != null)
                    item.Links += "mainLink|" + node.InnerText + "|";

                node = ProcessingNode.SelectSingleNode("//rss:description", nm);
                if (node != null)
                {
                    string strContent = node.InnerText;
                    strContent = Regex.Replace(strContent, @"[\n|\t|\r]", "");
                    strContent = Regex.Replace(strContent, @"<i>.*?</i>", "");
                    strContent = Regex.Replace(strContent, @"<.*?>", "");
                    item.Abstract = strContent;                    
                }

                node = ProcessingNode.SelectSingleNode("//prism:publicationYear", nm);
                if (node != null)
                {
                    item.PubDate = node.InnerText;
                    item.PubYear = Regex.Match(item.PubDate, @"\d{4}").Value;
                }

                node = ProcessingNode.SelectSingleNode("//prism:publicationName", nm);
                if (node != null)
                    item.Title2 = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//prism:volume", nm);
                if (node != null)
                    item.Volume = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//prism:number", nm);
                if (node != null)
                    item.Volume2 = node.InnerText;

                node = ProcessingNode.SelectSingleNode("//dc:identifier", nm);
                if (node != null)
                {
                    string strContent = node.InnerText;
                    item.DOI = Regex.Replace(strContent, @"^doi:", "", RegexOptions.IgnoreCase);
                }
                int count = 0;
                foreach(XmlNode sNode in ProcessingNode.SelectNodes("//dc:creator", nm))
                {
                    NameMasterRow nmr = new NameMasterRow();
                    nmr.NameTypeID = NameTypes.Author;
                    nmr.SequenceNo = count;
                    count++;
                    string strContent = sNode.InnerText;
                    int index = strContent.LastIndexOf(" ");
                    nmr.LastName = strContent.Substring(index);
                    nmr.ForeName = strContent.Substring(0, index);
                    item.Authors.Add(nmr);
                }

                node = ProcessingNode.SelectSingleNode("//prism:publisher", nm);
                if(node!=null)
                {
                    NameMasterRow nmr = new NameMasterRow();
                    nmr.NameTypeID = NameTypes.Publisher;
                    nmr.LastName = node.InnerText;
                    nmr.ForeName = "";
                    item.Authors.Add(nmr);
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
            return xmlDoc.DocumentElement.SelectNodes("//rss:item", nm);            
        }        
    }
}
