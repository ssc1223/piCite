using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSWordpiCite.Search
{
    class BaseClass
    {
        private string _searchQuery;
        public string SearchQuery
        {
            get
            {
                return _searchQuery;
            }
            set
            {
                _searchQuery = value;
            }
        }

        public int NumberResultsPerPage = Properties.Settings.Default.NUM_OF_SEARCH_RESULTS_PER_PAGE;
        
        public BaseClass(string query)
        {
            SearchQuery = query;
        }
        
        public BaseClass(string query, int count)
        {
            SearchQuery = query;
            NumberResultsPerPage = count;
        }

        public BaseClass()
        {}
    }
}
