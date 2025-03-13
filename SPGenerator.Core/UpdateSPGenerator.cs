using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace SPGenerator.Core
{
    class UpdateSPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName, string type, string dbname)
        {
            DbName = dbname;
            return "[dbo].[" + prefixUpdateSp + tableName + "_Update]";
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields)
        {
            var schema = "";
            sb.Append(Environment.NewLine);

            if (!string.IsNullOrEmpty(selectedFields[0].Schema))
                schema = "[" + selectedFields[0].Schema + "].";

            sb.Append(Environment.NewLine + "\tSET NOCOUNT ON;");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine + "\tUPDATE [" + DbName + "]." + schema + "[" + WrapIfKeyWord(tableName) + "]");
            sb.Append(Environment.NewLine + "\tSET");

            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.Exclude)
                    continue;
                if (colInf.IsPrimaryKey != true)
                {
                    sb.Append(Environment.NewLine + "\t\t[" + WrapIfKeyWord(colInf.ColumnName) + "] = ISNULL(" + prefixInputParameter + colInf.ColumnName + ",[" + WrapIfKeyWord(colInf.ColumnName) + "])");
                    sb.Append(",");
                }
            }
            sb[sb.Length - 1] = ' ';

            sb.Append(Environment.NewLine + "\tWHERE [" + WrapIfKeyWord(selectedFields[0].ColumnName) + "] = " + prefixInputParameter + selectedFields[0].ColumnName + ";");
            sb.Append(Environment.NewLine + $"\tSELECT {prefixInputParameter + selectedFields[0].ColumnName} AS {WrapIfKeyWord(selectedFields[0].ColumnName)}");
        }

        public string DbName { get; set; }
    }
}
