using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.DataModel
{
    public class DBTableColumnInfo
    {
        public string ColumnName;
        public string DataType;
        public string CharacterMaximumLength;
        public int NumericPrecision;
        public int NumericPrecisionRadix;
        public int NumericScale;
        public bool IsIdentity;
        public bool IsPrimaryKey;
        public bool Exclude;
        public string DbName;
        public string Schema;
    }
}
