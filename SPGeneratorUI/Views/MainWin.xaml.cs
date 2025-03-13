using SPGenerator.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TreeViewWithCheckBoxes;


namespace SPGenerator.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        public MainWin()
        {
            InitializeComponent();
            this.DataContext = new MainWinVM();
            MainWinVM.SPGenVM = true;
        }

        private void SPGen_Checked(object sender, RoutedEventArgs e)
        {
            MainWinVM.SPGenVM = SPGen.IsChecked.Value;
            MainWinVM.ModelGenVM = false;

        }

        private void modelGen_Checked(object sender, RoutedEventArgs e)
        {
            MainWinVM.ModelGenVM = modelGen.IsChecked.Value;
            MainWinVM.SPGenVM = false;
        }
    }
}
