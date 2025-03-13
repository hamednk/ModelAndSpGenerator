using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace SPGenerator.Core
{
    class ListSPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName, string type, string dbname)
        {
            DbName = dbname;
            return "[dbo].[" + prefixListSp + tableName + "_List]";
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields)
        {
            StringBuilder sbFields = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            sb.Append(Environment.NewLine);
            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.IsPrimaryKey != true)
                {
                    sbValues.Append(prefixInputParameter + colInf.ColumnName + ",");
                    sbFields.Append("[" + WrapIfKeyWord(colInf.ColumnName) + "],");
                }
            }

            sb.Append(Environment.NewLine + "\tSET NOCOUNT ON;");
            sb.Append(Environment.NewLine + Environment.NewLine + "\tDECLARE @SQL AS NVARCHAR(4000);");
            sb.Append(Environment.NewLine + $"\tSET @SQL =' SELECT COUNT({selectedFields[0].ColumnName}) FROM [{DbName}].[dbo].[{WrapIfKeyWord(tableName)}] WHERE (1=1) '" + Environment.NewLine);

            GenerateList(tableName, sb, selectedFields);

            sb.Append(Environment.NewLine + Environment.NewLine + $"\tSET @SQL = @SQL + '; SELECT * FROM [{DbName}].[dbo].[{WrapIfKeyWord(tableName)}] WHERE (1=1) '" + Environment.NewLine);

            GenerateList(tableName, sb, selectedFields);

            sb.Append(Environment.NewLine + Environment.NewLine + $"\tSET @SQL = @SQL + ' ORDER BY [dbo].[{WrapIfKeyWord(tableName)}].' + @OrderBy + ' ' + @Order + ' OFFSET (' + STR(@PageIndex) + ') ROWS FETCH NEXT ' + STR(@PageSize) + ' ROWS ONLY;'");
            sb.Append(Environment.NewLine + Environment.NewLine + $"\tEXECUTE(@SQL)");

        }

        private void GenerateList(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields)
        {
            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                switch (colInf.DataType.ToUpper())
                {
                    case "INT":
                    case "TINYINT":
                    case "REAL":
                    case "BIGINT":
                    case "DECIMAL":
                    case "FLOAT":
                    case "SMALLINT":
                        sb.Append(Environment.NewLine + $"\tIF ({prefixInputParameter + colInf.ColumnName} IS NOT NULL) SET @SQL = @SQL + ' AND [dbo].[{WrapIfKeyWord(tableName)}].[{WrapIfKeyWord(colInf.ColumnName)}] = ' + STR({prefixInputParameter + colInf.ColumnName}) ");
                        break;

                    case "VARCHAR":
                    case "NVARCHAR":
                    case "NTEXT":
                        sb.Append(Environment.NewLine + $"\tIF ({prefixInputParameter + colInf.ColumnName} IS NOT NULL) SET @SQL = @SQL + ' AND [dbo].[{WrapIfKeyWord(tableName)}].[{WrapIfKeyWord(colInf.ColumnName)}] LIKE N''%'+ {prefixInputParameter + colInf.ColumnName} + N'%''' ");
                        break;

                    case "SMALLDATETIME":
                    case "TIME":
                    case "DATE":
                    case "DATETIME":
                    case "UNIQUEIDENTIFIER":
                        sb.Append(Environment.NewLine + $"\tIF ({prefixInputParameter + colInf.ColumnName} IS NOT NULL) SET @SQL = @SQL + ' AND [dbo].[{WrapIfKeyWord(tableName)}].[{WrapIfKeyWord(colInf.ColumnName)}] = '''+ CONVERT(VARCHAR(50), {prefixInputParameter + colInf.ColumnName}) + '''' ");
                        break;

                    case "BIT":
                        sb.Append(Environment.NewLine + $"\tIF ({prefixInputParameter + colInf.ColumnName} IS NOT NULL) IF({prefixInputParameter + colInf.ColumnName} = 0) SET @SQL = @SQL + ' AND ([dbo].[{WrapIfKeyWord(tableName)}].[{WrapIfKeyWord(colInf.ColumnName)}] = 0 OR [dbo].[{WrapIfKeyWord(tableName)}].[{WrapIfKeyWord(colInf.ColumnName)}] IS NULL )' ELSE SET @SQL = @SQL + ' AND [dbo].[{WrapIfKeyWord(tableName)}].[{WrapIfKeyWord(colInf.ColumnName)}] = 1' ");
                        break;
                }

            }
        }

        public string DbName { get; set; }
    }
}
