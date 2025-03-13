using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace SPGenerator.Core
{
    class ModelSPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName, string type, string dbname)
        {
            DbName = dbname;
            return "[dbo].[" + prefixModelSp + tableName + "_MODELGEN]";
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields)
        {
            //sb.Clear(); شسی

            sb.Append(Environment.NewLine + $"namespace {DbName}.DomainClass");
            sb.Append(Environment.NewLine + "{");
            sb.Append(Environment.NewLine + "using System;");
            sb.Append(Environment.NewLine + $"public class {tableName}");
            sb.Append(Environment.NewLine + "{");

            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                switch (colInf.DataType.ToUpper())
                {
                    case "INT":
                    case "SMALLINT":
                        sb.Append(Environment.NewLine + $"public int? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "TINYINT":
                        sb.Append(Environment.NewLine + $"public byte? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "REAL":
                        sb.Append(Environment.NewLine + $"public float? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "BIGINT":
                    case "FLOAT":
                        sb.Append(Environment.NewLine + $"public Int64? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "DECIMAL":
                        sb.Append(Environment.NewLine + $"public decimal? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "VARCHAR":
                    case "NVARCHAR":
                    case "NTEXT":
                        sb.Append(Environment.NewLine + $"public string {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "SMALLDATETIME":
                    case "TIME":
                    case "DATE":
                    case "DATETIME":
                        sb.Append(Environment.NewLine + $"public DateTime? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "UNIQUEIDENTIFIER":
                        sb.Append(Environment.NewLine + $"public Guid? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "BIT":
                        sb.Append(Environment.NewLine + $"public bool? {colInf.ColumnName}" + " { get; set; }");
                        break;
                    case "TIMESTAMP":
                        sb.Append(Environment.NewLine + $"public byte[] {colInf.ColumnName}" + " { get; set; }");
                        break;
                }
            }

            sb.Append(Environment.NewLine + "}");
            sb.Append(Environment.NewLine + "}");

            sb.Append(Environment.NewLine + "*************************************************************");
        }

        public string DbName { get; set; }
    }
}
