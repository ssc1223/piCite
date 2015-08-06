namespace MSWordpiCite.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using MSWordpiCite.Enums;

    [Serializable]
    public enum NameMasterColumns
    {
        NameID,
        NameTypeID,
        LastName,
        ForeName,
        Initials,
        DisplayName
    }

    [Serializable]
    public class NameMasterRow : IComparable
    {
        private int _NameID = 0;
        private NameTypes _NameTypeID;
        private string _LastName;
        private string _ForeName;
        private string _Initials;
        private string _DisplayName;
        private int _SequenceNo;

        public NameMasterRow()
        {
            _NameID = int.MinValue;
            _NameTypeID = NameTypes.Author;
            _LastName = _ForeName = _Initials = _DisplayName = string.Empty;
        }

        public int NameID
        {
            get { return _NameID; }
            set { _NameID = value; }
        }

        public NameTypes NameTypeID
        {
            get { return _NameTypeID; }
            set { _NameTypeID = value; }
        }

        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }

        public string ForeName
        {
            get { return _ForeName; }
            set { _ForeName = value; }
        }

        public string Initials
        {
            get { return _Initials; }
            set { _Initials = value; }
        }

        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        public int SequenceNo
        {
            get
            {
                return _SequenceNo;
            }
            set
            {
                _SequenceNo = value;
            }
        }

        public static string[] TableColumns
        {
            get
            {
                return Enum.GetNames(typeof(NameMasterColumns));
            }
        }

        public static List<NameMasterRow> Convert(DataView dv)
        {
            List<NameMasterRow> NameList = new List<NameMasterRow>();

            string[] strColumns = TableColumns;
            if (dv.Count > 0)
            {
                for (int i = 0; i < strColumns.Length; i++)
                {
                    if (!dv.Table.Columns.Contains(strColumns[i]))
                        return NameList;
                }
            }

            for (int i = 0; i < dv.Count; i++)
            {
                NameMasterRow nmr = new NameMasterRow();
                nmr.DisplayName = dv[i][NameMasterColumns.DisplayName.ToString()] + "";
                nmr.ForeName = dv[i][NameMasterColumns.ForeName.ToString()] + "";
                nmr.Initials = dv[i][NameMasterColumns.Initials.ToString()] + "";
                nmr.LastName = dv[i][NameMasterColumns.LastName.ToString()] + "";
                nmr.NameTypeID = (NameTypes)int.Parse(dv[i][NameMasterColumns.NameTypeID.ToString()] + "");
                nmr.NameID = int.Parse(dv[i][NameMasterColumns.NameID.ToString()] + "");
                nmr.SequenceNo = i;

                NameList.Add(nmr);
            }

            return NameList;
        }

        public static List<NameMasterRow> ConvertWithoutNameID(DataView dv)
        {
            List<NameMasterRow> NameList = new List<NameMasterRow>();

            string[] strColumns = TableColumns;
            if (dv.Count > 0)
            {
                for (int i = 0; i < strColumns.Length; i++)
                {
                    if (strColumns[i] == NameMasterColumns.NameID.ToString())
                        continue;

                    if (!dv.Table.Columns.Contains(strColumns[i]))
                        return NameList;
                }
            }

            for (int i = 0; i < dv.Count; i++)
            {
                NameMasterRow nmr = new NameMasterRow();
                nmr.NameID = int.MinValue;
                nmr.DisplayName = dv[i][NameMasterColumns.DisplayName.ToString()] + "";
                nmr.ForeName = dv[i][NameMasterColumns.ForeName.ToString()] + "";
                nmr.Initials = dv[i][NameMasterColumns.Initials.ToString()] + "";
                nmr.LastName = dv[i][NameMasterColumns.LastName.ToString()] + "";
                nmr.NameTypeID = (NameTypes)int.Parse(dv[i][NameMasterColumns.NameTypeID.ToString()] + "");

                NameList.Add(nmr);
            }

            return NameList;
        }

        public int CompareTo(object obj2)
        {
            NameMasterRow nm2 = (NameMasterRow)obj2;
            int r = this._NameTypeID.CompareTo(nm2._NameTypeID);
            if (r == 0)
                r = this._SequenceNo.CompareTo(nm2._SequenceNo);
            return r;
        }
    }
}
