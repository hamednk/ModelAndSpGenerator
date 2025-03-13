using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SPGenerator.Core
{
    public abstract class BaseSPGenerator
    {
        #region Abstract Method
        protected abstract string GetSpName(string tableName, string type, string dbname);
        protected abstract void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields);
        #endregion

        #region Static Members
        internal static string prefixWhereParameter;
        internal static string prefixInputParameter;
        internal static string prefixInsertSp;
        internal static string prefixListSp;
        internal static string prefixGetSp;
        internal static string prefixUpdateSp;
        internal static string prefixModelSp;
        internal static bool errorHandling;
        internal static string[] sqlKeyWords;
        static BaseSPGenerator()
        {
            sqlKeyWords = File.ReadAllLines("SqlKeyWords.txt").Select(p => p.Trim().ToUpperInvariant()).ToArray();
        }
        public static void SetSettings(Settings setting)
        {
            prefixWhereParameter = setting.prefixWhereParameter;
            prefixInputParameter = setting.prefixInputParameter;
            prefixInsertSp = "usp_";
            prefixUpdateSp = "usp_";
            prefixListSp = "usp_";
            prefixGetSp = "usp_";
            prefixModelSp = "usp_";
            errorHandling = setting.errorHandling == "No" ? true : false;

        }
        #endregion

        #region GenerateSP
        public void GenerateSp(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, string type)
        {

            string spName = GetSpName(tableName, type, selectedFields[0].DbName);
            if (spName.ToLower().Contains("model") == true)
            {
                GenerateStatement(tableName, sb, selectedFields);
            }
            else
            {
                GenerateDropScript(spName, sb);
                sb.Append(Environment.NewLine + " CREATE PROCEDURE " + spName);
                GenerateErrorNumberOutParameter(sb);
                GenerateInputParameters(selectedFields, sb, spName);
                //GenerateWhereParameters(whereConditionFields, sb);
                sb.Append(Environment.NewLine + "AS" + Environment.NewLine + "BEGIN");
                GenerateStartTryBlock(sb);
                GenerateStatement(tableName, sb, selectedFields);
                GenerateEndTryBlock(sb);
                GenerateCatchBlock(sb);
                sb.Append(Environment.NewLine + "END");
                sb.Append(Environment.NewLine + "GO");
                sb.Append(Environment.NewLine);
            }
        }

        protected virtual void GenerateDropScript(string spName, StringBuilder sb)
        {
            sb.Append(Environment.NewLine + "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'");
            sb.Append(spName);
            sb.Append("')AND type in (N'P', N'PC'))");
            sb.Append(Environment.NewLine + "DROP PROCEDURE ");
            sb.Append(spName);
            sb.Append(Environment.NewLine + "GO" + Environment.NewLine);
        }

        protected virtual void GenerateInputParameters(List<DBTableColumnInfo> tableFields, StringBuilder sb, string spName)
        {
            if (spName.Contains("Get"))
            {
                foreach (DBTableColumnInfo colInf in tableFields)
                {
                    sb.Append(Environment.NewLine + "\t" + prefixInputParameter + colInf.ColumnName);
                    sb.Append(" AS " + colInf.DataType.ToUpper());
                    if (colInf.CharacterMaximumLength?.Length > 0)
                    {
                        sb.Append("(" + colInf.CharacterMaximumLength.ToString() + ")");
                    }
                    sb.Append(" = NULL ");
                    return;
                }
            }
            else
            {
                foreach (DBTableColumnInfo colInf in tableFields)
                {
                    sb.Append(Environment.NewLine + "\t" + prefixInputParameter + colInf.ColumnName);
                    sb.Append(" AS " + colInf.DataType.ToUpper());
                    if (colInf.CharacterMaximumLength?.Length > 0)
                    {
                        sb.Append("(" + colInf.CharacterMaximumLength.ToString() + ")");
                    }
                    sb.Append(" = NULL ,");
                }
            }


            if (spName.Contains("List"))
            {
                sb.Append(Environment.NewLine + $"\t@OrderBy AS VARCHAR(128) = 'CreatedOn' ,");
                sb.Append(Environment.NewLine + $"\t@Order AS VARCHAR(4) = 'DESC' ,");
                sb.Append(Environment.NewLine + $"\t@PageSize AS INT = 10 ,");
                sb.Append(Environment.NewLine + $"\t@PageIndex AS INT = 0 ,");
                sb.Append(Environment.NewLine + $"\t@RowCount AS INT = NULL ,");
            }

            //Remove Commma from end
            sb[sb.Length - 1] = ' ';
        }

        protected void GenerateWhereParameters(List<DBTableColumnInfo> whereConditionFields, StringBuilder sb)
        {
            sb.Append(",");
            foreach (DBTableColumnInfo colInf in whereConditionFields)
            {
                sb.Append(Environment.NewLine + prefixWhereParameter + colInf.ColumnName);
                sb.Append(" AS " + colInf.DataType.ToUpper());
                if (colInf.CharacterMaximumLength.Length > 0)
                {
                    sb.Append("(" + colInf.CharacterMaximumLength.ToString() + ")");
                }
                sb.Append(" = NULL ,");
            }
            //Remove Commma from end
            sb[sb.Length - 1] = ' ';
        }

        protected void GenerateWhereStatement(List<DBTableColumnInfo> whereConditionFields, StringBuilder sb)
        {
            sb.Append(Environment.NewLine + $"\tWHERE {whereConditionFields[0].ColumnName} = @{whereConditionFields[0].ColumnName};");
        }

        #region ErrorHandling
        private void GenerateStartTryBlock(StringBuilder sb)
        {
            if (!errorHandling)
                return;
            sb.Append(Environment.NewLine + "BEGIN TRY");
        }

        private void GenerateEndTryBlock(StringBuilder sb)
        {
            if (!errorHandling)
                return;
            sb.Append(Environment.NewLine + "END TRY");
        }
        private void GenerateErrorNumberOutParameter(StringBuilder sb)
        {
            if (!errorHandling)
                return;
            sb.Append(Environment.NewLine + "@out_error_number INT = 0 OUTPUT,");
        }

        private void GenerateCatchBlock(StringBuilder sb)
        {
            if (!errorHandling)
                return;
            sb.Append(Environment.NewLine + "BEGIN CATCH");
            sb.Append(Environment.NewLine + "\tSELECT @out_error_number=ERROR_NUMBER()");
            sb.Append(Environment.NewLine + "END CATCH");
        }
        #endregion

        protected string WrapIfKeyWord(string name)
        {
            if (sqlKeyWords.Contains(name.Trim().ToUpperInvariant()))
            {
                name = "[" + name + "]";
            }
            return name;
        }
        #endregion
    }
}
