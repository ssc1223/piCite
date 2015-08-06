using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSWordpiCite.Classes
{
    public class WordHyperLink
    {
        #region Properties

        private long _start;
        public long Start
        {
            get
            {
                return _start;
            }
            set
            {
                _start = value;
            }
        }

        private long _end;
        public long End
        {
            get
            {
                return _end;
            }
            set
            {
                _end = value;
            }
        }

        private string _url;
        public string URL
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        #endregion

        public WordHyperLink(int _start, int _end, string _url)
        {
            Start = _start;
            End = _end;
            URL = _url;
        }
    }
}
