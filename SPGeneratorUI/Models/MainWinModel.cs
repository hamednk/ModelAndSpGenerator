using SPGenerator.Core;
using SPGenerator.DAL;
using SPGenerator.DataModel;
using SPGenerator.Interface;
using System.Collections.Generic;
using System.Text;

namespace SPGenerator.UI.Models
{
    internal class MainWinModel
    {
        public List<DBTableInfo> ConnectToServer(string connectionString)
        {
            IDataBase dataBase = GetDataBaseObject(connectionString);
            return dataBase.GetDataBaseTables();
        }
        internal void RefreshSettings()
        {
            BaseSPGenerator.SetSettings(Comman.Settings.GetSettings());
        }
        public void GenerateSp(string tableName, string nodeName, ref StringBuilder sb, List<DBTableColumnInfo> selectedFields, string type)
        {
            BaseSPGenerator spGenerator = SPFactory.GetSpGeneratorObject(nodeName);
            spGenerator.GenerateSp(tableName, sb, selectedFields, type);
        }
        private IDataBase GetDataBaseObject(string connectionString)
        {
            return new SqlDataBase(connectionString);
        }
    }
}
