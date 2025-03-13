using SPGenerator.Common;
using SPGenerator.DataModel;
using SPGenerator.UI.Commands;
using SPGenerator.UI.Models;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using TreeViewWithCheckBoxes;


namespace SPGenerator.UI.ViewModels
{
    internal class MainWinVM : ViewModelBase
    {

        #region vairable
        TreeViewNode rootNode;
        MainWinModel model;
        string defaultDisplayText = "Data Source=.;Initial Catalog=database;Integrated Security=true";
        #endregion

        public MainWinVM()
        {
            model = new MainWinModel();
            ConnectionString = defaultDisplayText;
        }

        #region Properties
        string sqlScript;
        public string SqlScript
        {
            get
            {
                return sqlScript;
            }
            set
            {
                sqlScript = value;
                NotifyPropertyChanged("SqlScript");
            }
        }

        string connectionString;
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
                NotifyPropertyChanged("ConnectionString");
            }
        }

        bool isConnectedToServer;
        public bool IsConnectedToServer
        {
            get
            {
                return isConnectedToServer;
            }
            set
            {
                isConnectedToServer = value;
                NotifyPropertyChanged("IsConnectedToServer");
            }
        }

        #endregion

        #region Commands
        private RelayCommand connectServerCommand;
        public ICommand ConnectServerCommand
        {
            get
            {
                if (connectServerCommand == null) connectServerCommand = new RelayCommand(param => this.ConnectToServer(param));
                return connectServerCommand;
            }
        }
        private void ConnectToServer(object param)
        {
            var currentCursor = Mouse.OverrideCursor;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var lstTabales = model.ConnectToServer(ConnectionString);
                PopulateTree(lstTabales, (SPGenerator.UserControls.TreeViewWithCheckBox)param);
                IsConnectedToServer = true;
            }

            finally
            {
                Mouse.OverrideCursor = currentCursor;
            }
        }

        private RelayCommand generateSPCommand;
        public ICommand GenerateSPCommand
        {
            get
            {
                if (generateSPCommand == null) generateSPCommand = new RelayCommand(param => this.GenerateSps());
                return generateSPCommand;
            }
        }



        private RelayCommand settingCommand;
        public ICommand SettingCommand
        {
            get
            {
                if (settingCommand == null) settingCommand = new RelayCommand(param => this.Settings());
                return settingCommand;
            }
        }
        private void Settings()
        {
            Views.Settings setting = new Views.Settings();
            setting.Show();
        }

        private RelayCommand copyCommand;
        public ICommand CopyCommand
        {
            get
            {
                if (copyCommand == null) copyCommand = new RelayCommand(param => this.Copy());
                return copyCommand;
            }
        }
        private void Copy()
        {
            if (!string.IsNullOrEmpty(SqlScript))
            {
                Clipboard.SetText(SqlScript);
            }
        }

        private RelayCommand gotFocusConnectionStringCommand;
        public ICommand GotFocusConnectionStringCommand
        {
            get
            {
                if (gotFocusConnectionStringCommand == null) gotFocusConnectionStringCommand = new RelayCommand(param => this.GotFocusConnectionString());
                return gotFocusConnectionStringCommand;
            }
        }
        private void GotFocusConnectionString()
        {
            if (ConnectionString.Trim() == defaultDisplayText)
                ConnectionString = "";
        }

        private RelayCommand lostFocusConnectionStringCommand;
        public ICommand LostFocusConnectionStringCommand
        {
            get
            {
                if (lostFocusConnectionStringCommand == null) lostFocusConnectionStringCommand = new RelayCommand(param => this.LostFocusConnectionString());
                return lostFocusConnectionStringCommand;
            }
        }
        private void LostFocusConnectionString()
        {
            if (ConnectionString.Trim() == "")
                ConnectionString = defaultDisplayText;
        }



        #endregion

        #region SP Generation

        private void GenerateSps()
        {

            StringBuilder sb = new StringBuilder(1000);
            model.RefreshSettings();
            foreach (TreeViewNode tblNode in rootNode.Children)
            {
                if (tblNode.IsChecked ?? true)
                {
                    GenerateSPForSingleTable(tblNode, sb, tblNode.Children[0].Name);
                }
            }
            SqlScript = sb.ToString();

        }

        private void GenerateSPForSingleTable(TreeViewNode tblNode, StringBuilder sb, string type)
        {
            foreach (TreeViewNode childNode in tblNode.Children)
            {
                var selectedFields = GetSelectedFields(childNode);
                //var whereClauseSelectedFields = GetWhereClauseFields(childNode);

                if (childNode.IsChecked ?? true)
                {
                    model.GenerateSp(tblNode.Name, childNode.Name, ref sb, selectedFields, type);
                }
            }
        }

        private List<DBTableColumnInfo> GetSelectedFields(TreeViewNode tableChildNode)
        {
            var selectedFields = new List<DBTableColumnInfo>();
            foreach (TreeViewNode fieldNode in tableChildNode.Children)
            {
                if ((fieldNode.IsChecked ?? true)/* && fieldNode.Name != Constants.whereConditionTreeNodeText*/)
                {
                    selectedFields.Add((DBTableColumnInfo)fieldNode.Tag);
                }
            }

            return selectedFields;
        }

        private List<DBTableColumnInfo> ListSelectedFields(TreeViewNode tableChildNode)
        {
            var selectedFields = new List<DBTableColumnInfo>();
            foreach (TreeViewNode fieldNode in tableChildNode.Children)
            {
                if ((fieldNode.IsChecked ?? true)/* && fieldNode.Name != Constants.whereConditionTreeNodeText*/)
                {
                    selectedFields.Add((DBTableColumnInfo)fieldNode.Tag);
                }
            }

            return selectedFields;
        }

        //private List<DBTableColumnInfo> GetWhereClauseFields(TreeViewNode tableChildNode)
        //{
        //    var selectedFields = new List<DBTableColumnInfo>();
        //    var whereClauseNode = GetChildNodeByText(tableChildNode, Constants.whereConditionTreeNodeText);
        //    if (whereClauseNode == null)
        //        return selectedFields;

        //    foreach (TreeViewNode fieldNode in whereClauseNode.Children)
        //    {
        //        if (fieldNode.IsChecked ?? true)
        //        {
        //            selectedFields.Add((DBTableColumnInfo)fieldNode.Tag);
        //        }
        //    }

        //    return selectedFields;
        //}

        private TreeViewNode GetChildNodeByText(TreeViewNode parentNode, string childNodeText)
        {
            var childNode = parentNode.Children.OfType<TreeViewNode>()
                          .FirstOrDefault(node => node.Name.Equals(childNodeText));

            return childNode;
        }

        #endregion

        #region PopulateTree
        private void PopulateTree(List<DBTableInfo> sqlTableList, SPGenerator.UserControls.TreeViewWithCheckBox treeView1)
        {
            TreeViewNode root = new TreeViewNode(Constants.rootTreeNodeText, null);
            foreach (var tbl in sqlTableList)
            {
                root.Children.Add(CreateTableNode(tbl, root));
            }
            root.IsNodeExpanded = true;
            treeView1.DataContext = new List<TreeViewNode> { root };
            rootNode = root;
        }


        /// <summary>
        /// create check box for treeview
        /// </summary>
        /// <param name="sqlTableInfo"></param>
        /// <param name="parent"></param>
        /// <returns></returns>

        public static bool ModelGenVM { get; set; }
        public static bool SPGenVM { get; set; }

        private TreeViewNode CreateTableNode(DBTableInfo sqlTableInfo, TreeViewNode parent)
        {
            Views.MainWin mainWinModel = new Views.MainWin();

            TreeViewNode tblNode = new TreeViewNode(sqlTableInfo.TableName, parent);
            tblNode.Tag = sqlTableInfo;

            TreeViewNode insertSp = new TreeViewNode(Constants.insertTreeNodeText, tblNode);
            TreeViewNode updateSp = new TreeViewNode(Constants.updateTreeNodeText, tblNode);
            TreeViewNode getSp = new TreeViewNode(Constants.getTreeNodeText, tblNode);
            TreeViewNode listSp = new TreeViewNode(Constants.listTreeNodeText, tblNode);
            TreeViewNode modelSp = new TreeViewNode(Constants.modelSpTreeNodeText, tblNode);

            if (ModelGenVM != true)
            {
                AddColumnNodes(insertSp, sqlTableInfo.Columns, true);
                AddColumnNodes(updateSp, sqlTableInfo.Columns, true);
                AddColumnNodes(getSp, sqlTableInfo.Columns, true);
                AddColumnNodes(listSp, sqlTableInfo.Columns, true);
            }
            else
                AddColumnNodes(modelSp, sqlTableInfo.Columns, true);

            if (ModelGenVM != true)
            {
                tblNode.Children.Add(insertSp);
                tblNode.Children.Add(updateSp);
                tblNode.Children.Add(getSp);
                tblNode.Children.Add(listSp);
            }
            else
                tblNode.Children.Add(modelSp);

            return tblNode;
        }

        private TreeViewNode AddColumnNodes(TreeViewNode parentNode, List<DBTableColumnInfo> columns, bool disableExludeColumn)
        {
            foreach (var colInfo in columns)
            {
                TreeViewNode colNode = new TreeViewNode(GetColumnDispalyName(colInfo), parentNode);
                colNode.Tag = colInfo;
                if (disableExludeColumn)
                {
                    colNode.IsNodeEnabled = !colInfo.Exclude;
                }
                parentNode.Children.Add(colNode);
            }
            return parentNode;
        }

        private string GetColumnDispalyName(DBTableColumnInfo colInfo)
        {
            string diaplayName = colInfo.ColumnName;
            if (colInfo.Exclude)
            {
                if (colInfo.IsIdentity)
                    diaplayName += " (IDENTITY)";
                if (colInfo.DataType.ToUpperInvariant() == "TIMESTAMP")
                    diaplayName += " (TIMESTAMP)";

            }

            return diaplayName;
        }

        #endregion

    }
}
