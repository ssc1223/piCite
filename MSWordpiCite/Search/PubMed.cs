using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Script.Serialization;
using MSWordpiCite.Enums;
using MSWordpiCite.Entities;

namespace MSWordpiCite.Search
{
    class PubMed : BaseClass
    {
        const string PUBMED_SEARCHURL = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&sort=pub+date&tool=WizFolio&email=techsupport@wizfolio.com&amp;term={term}&retstart={start}&retmax={max}";
        const string PUBMED_FETCHURL = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&retmode=xml&tool=WizFolio&email=techsupport@wizfolio.com&id={pmid}";
        const string PUBMED_SUMMARYURL = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils/esummary.fcgi?db=pubmed&tool=WizFolio&email=techsupport@wizfolio.com&id={pmid}";
        const string PUBMED_RELATEDLINK = "http://www.ncbi.nlm.nih.gov/sites/entrez?Db=pubmed&DbFrom=pubmed&Cmd=Link&LinkName=pubmed_pubmed&LinkReadableName=Related%20Articles&IdsFromResult={pmid}&ordinalpos=3&itool=EntrezSystem2.PEntrez.Pubmed.Pubmed_ResultsPanel.Pubmed_RVDocSum";
        const string PUBMED_LINK = "http://www.ncbi.nlm.nih.gov/sites/entrez?Db=pubmed&Cmd=ShowDetailView&TermToSearch={pmid}&ordinalpos=3&itool=EntrezSystem2.PEntrez.Pubmed.Pubmed_ResultsPanel.Pubmed_RVDocSum";

        public PubMed(string query) : base(query){}

        public PubMed() { }

        /// <summary>
        /// Gets the total.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetResults(int iStart)
        {
            if (SearchQuery == string.Empty || SearchQuery.Length == 0)
                return null;

            Dictionary<string, object> Data = new Dictionary<string, object>();
            Int64 iTotal = 0;
            string strPubMedSearchURL = PUBMED_SEARCHURL.Replace("{term}", SearchQuery);
            strPubMedSearchURL = strPubMedSearchURL.Replace("{start}", iStart + "");
            strPubMedSearchURL = strPubMedSearchURL.Replace("{max}", Properties.Settings.Default.NUM_OF_SEARCH_RESULTS_PER_PAGE + "");
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strPubMedSearchURL);
                XmlNodeList resultNodeList = xmlDoc.SelectNodes("/eSearchResult/Count");
                if (resultNodeList.Count > 0)
                {
                    iTotal = Int64.Parse(resultNodeList[0].InnerText);                   
                }
                Data["Count"] = iTotal;
                List<string> listPMID = new List<string>();
                if (iTotal > 0)
                {
                    XmlNodeList pubmedIDList = xmlDoc.SelectNodes("/eSearchResult/IdList/Id");
                    if (pubmedIDList.Count > 0)
                    {
                        foreach (XmlNode node in pubmedIDList)
                            listPMID.Add(node.InnerText);
                    }
                }
                JavaScriptSerializer sr = new JavaScriptSerializer();
                string pmids = String.Join(",", listPMID.ToArray());

                strPubMedSearchURL = PUBMED_FETCHURL.Replace("{pmid}", pmids);
                xmlDoc = new XmlDocument();
                xmlDoc.Load(strPubMedSearchURL);
                List<ItemMasterRow> listItems = new List<ItemMasterRow>();
                XmlNodeList resultNodes = _FindResultNodes(xmlDoc);
                if (resultNodes != null)
                {
                    foreach (XmlNode node in resultNodes)
                    {
                        ItemMasterRow item = new ItemMasterRow();
                        XmlDocument newXmlDoc = new XmlDocument();
                        newXmlDoc.LoadXml(node.OuterXml);
                        if (_ProcessingNode(newXmlDoc, ref item))
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
            { }
            return Data;
        }

        /// <summary>
        /// Find result nodes.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private XmlNodeList _FindResultNodes(XmlDocument xmlDoc)
        {
            return xmlDoc.DocumentElement.SelectNodes("//PubmedArticle");
        }
               
        /// <summary>
        /// _s the fetch item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="strPMID">The STR PMID.</param>
        /// <returns></returns>
        private bool _ProcessingNode(XmlDocument xmlDoc, ref ItemMasterRow item)
        {
            Dictionary<string, string> Data = new Dictionary<string, string>();
            item.ItemTypeID = ItemTypes.JournalArticle;

            XmlNodeList authorsList = xmlDoc.SelectNodes("//Article/AuthorList/Author");
            int count = 0;
            foreach (XmlNode author in authorsList)
            {
                XmlNode lastname = author.SelectSingleNode("LastName");
                XmlNode firstname = author.SelectSingleNode("ForeName");
                if(lastname != null && firstname != null){
                    NameMasterRow nmr = new NameMasterRow();
                    nmr.NameTypeID = NameTypes.Author;
                    nmr.SequenceNo = count;
                    count++;
                    nmr.LastName = lastname.InnerText;
                    nmr.ForeName = firstname.InnerText;
                    item.Authors.Add(nmr);
                }
            }

            XmlNode node = xmlDoc.SelectSingleNode("//Article/Abstract/AbstractText");
            if(node != null)
                item.Abstract = node.InnerText;

            node = xmlDoc.SelectSingleNode("//Article/Affiliation");
            if (node != null)
                item.Affiliation = node.InnerText;

            node = xmlDoc.SelectSingleNode("//PMID");
            if (node != null)
                item.ID2 = node.InnerText;

            node = xmlDoc.SelectSingleNode("//Article/ArticleTitle");
            if (node != null)
                item.Title = node.InnerText;

            node = xmlDoc.SelectSingleNode("//Article/Journal/ISSN");
            if (node != null)
                item.ID1 = node.InnerText;

            node = xmlDoc.SelectSingleNode("//Article/Journal//Volume");
            if (node != null)
                item.Volume = node.InnerText;

            node = xmlDoc.SelectSingleNode("//Article/Journal//Issue");
            if (node != null)
                item.Volume2 = node.InnerText;

            node = xmlDoc.SelectSingleNode("//Article/Journal//PubDate/Year");
            if (node != null)
            {
                item.PubDate = node.InnerText;
                item.PubYear = node.InnerText;
            }

            node = xmlDoc.SelectSingleNode("//Article/Journal/Title");
            if (node != null)
                item.Title2 = node.InnerText;

            node = xmlDoc.SelectSingleNode("//Article/Pagination/MedlinePgn");
            if (node != null)
                item.Pages = node.InnerText;

            node = xmlDoc.SelectSingleNode("//ArticleIdList/ArticleId[@IdType=doi]");
            if (node != null)
                item.DOI = node.InnerText;

            item.Links += "mainLink|" + PUBMED_LINK.Replace("{pmid}", item.ID2) + "|";
            item.Links += "relatedLinks|" + PUBMED_RELATEDLINK.Replace("{pmid}", item.ID2);

            return true;
        }
    }
}