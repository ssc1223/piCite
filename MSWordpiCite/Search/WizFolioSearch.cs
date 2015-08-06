using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSWordpiCite.Entities;

namespace MSWordpiCite.Search
{
    class WizFolioSearch : BaseClass
    {    
        public WizFolioSearch(string query) : base(query){}

        public WizFolioSearch() { }

        public List<ItemMasterRow> GetResults(int iStart)
        {
            if (SearchQuery == string.Empty || SearchQuery.Length == 0)
                return null;

            return Globals.ThisAddIn.user.SearchWizFolioCollection(SearchQuery, iStart, base.NumberResultsPerPage);            
        }       
    }
}
