using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace SPGenerator.Core
{
    class GetSPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName, string type, string dbname)
        {
            DbName = dbname;
            return "[dbo].[" + prefixGetSp + tableName + "_Get]";
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields)
        {
            StringBuilder sbFields = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();

            sb.Append(Environment.NewLine);
            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.Exclude)
                    continue;

                if (colInf.IsPrimaryKey == true)
                {
                    sbValues.Append(prefixInputParameter + colInf.ColumnName + ",");
                    sbFields.Append("[" + WrapIfKeyWord(colInf.ColumnName) + "],");
                }
            }
            sb.Append(Environment.NewLine + "\tSET NOCOUNT ON;");
            
            sb.Append(Environment.NewLine);

            sb.Append(Environment.NewLine + $"\tSELECT * FROM [" + DbName + "].[dbo].[" + WrapIfKeyWord(tableName) + "]");
            sb.Append(Environment.NewLine + $"\tWHERE {selectedFields[0].ColumnName} = {prefixInputParameter + selectedFields[0].ColumnName};");
        }

        public string DbName { get; set; }
    }
}
