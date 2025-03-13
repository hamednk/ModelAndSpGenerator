using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace SPGenerator.Core
{
    class InsertSPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName, string type, string dbname)
        {
            DbName = dbname;
            return "[dbo].[" + prefixInsertSp + tableName + "_Create]";
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

                sbValues.Append(prefixInputParameter + colInf.ColumnName + ",");
                sbFields.Append("[" + WrapIfKeyWord(colInf.ColumnName) + "],");
            }
            sb.Append(Environment.NewLine + "\tSET NOCOUNT ON;");

            if (selectedFields[0].DataType == "uniqueidentifier")
            {
                sb.Append(Environment.NewLine + $"\tIF {prefixInputParameter + selectedFields[0].ColumnName} IS NULL");
                sb.Append(Environment.NewLine + $"\t\tSET {prefixInputParameter + selectedFields[0].ColumnName} = NEWID()");
            }

            sb.Append(Environment.NewLine + Environment.NewLine + $"\tINSERT INTO [{DbName}].[dbo].[" + WrapIfKeyWord(tableName) + "]");
            sb.Append(Environment.NewLine + "\t\t(" + sbFields.ToString().TrimEnd(',') + ")");

            sb.Append(Environment.NewLine + "\tVALUES");
            sb.Append(Environment.NewLine + "\t\t(" + sbValues.ToString().TrimEnd(',') + ")");

            if (selectedFields[0].DataType == "int")
            {
                sb.Append(Environment.NewLine + $"\tSET {prefixInputParameter + selectedFields[0].ColumnName} = @@IDENTITY");
            }
              
            sb.Append(Environment.NewLine + Environment.NewLine);
            sb.Append(Environment.NewLine + $"\tSELECT {prefixInputParameter + selectedFields[0].ColumnName} AS {selectedFields[0].ColumnName}");

        }

        public string DbName { get; set; }
    }
}
