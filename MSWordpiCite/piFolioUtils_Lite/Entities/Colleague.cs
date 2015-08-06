using System;
using System.Collections.Generic;
using System.Text;

namespace MSWordpiCite.Entities
{
    [Serializable]
    public class Colleague
    {
        private int _userID;
        public int UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
            }
        }

        private string _serverAddress;
        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
            set
            {
                _serverAddress = value;
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }

        private string _foreName;
        public string ForeName
        {
            get
            {
                return _foreName;
            }
            set
            {
                _foreName = value;
            }
        }

        public Colleague()
        { }
    }
}
