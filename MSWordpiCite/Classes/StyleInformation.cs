using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSWordpiCite.Classes
{
    public class StyleInformation
    {
        private int _userid;
        public int UserID
        {
            get
            {
                return _userid;
            }
            set
            {
                _userid = value;
            }
        }

        private string _stylename;
        public string StyleName
        {
            get
            {
                return _stylename;
            }
            set
            {
                _stylename = value;
            }
        }

        public StyleInformation(int userid, string stylename)
        {
            UserID = userid;
            StyleName = stylename;
        }
    }
}
