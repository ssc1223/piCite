using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Net;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MSWordpiCite.Enums;

namespace MSWordpiCite.Entities
{
    /// <summary>
    /// Summary description for Folder
    /// </summary>

    [Serializable]
    public class Folder
    {
        #region Properties

        private int _id = int.MinValue;
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private int _parentID = int.MinValue;
        public int ParentID
        {
            get
            {
                return _parentID;
            }
            set
            {
                _parentID = value;
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        private string _imgname = string.Empty;
        public string ImgName
        {
            get
            {
                return _imgname;
            }
            set
            {
                _imgname = value;
            }
        }

        private FolderType _type = FolderType.Normal;
        public FolderType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private bool _lastFocused;
        public bool LastFocused
        {
            get
            {
                return _lastFocused;
            }
            set
            {
                _lastFocused = value;
            }
        }

        #endregion

        public Folder()
        {
        }

        protected Folder(int id, int parentid, string name, string description, string imgname, bool lastfocused, FolderType foldertype)
        {
            ID = id;
            ParentID = parentid;
            Name = name;
            ImgName = imgname;
            Description = description;
            LastFocused = lastfocused;
            Type = foldertype;
        }
    }
}