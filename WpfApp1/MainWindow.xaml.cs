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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // This overrides the red 'X' button to shutdown the application without having to close the previous window
        // This should be in every windows code
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void FileNewClick(object sender, RoutedEventArgs e)
        {

        }

        private void FileOpenClick(object sender, RoutedEventArgs e)
        {

        }

        private void FileSaveClick(object sender, RoutedEventArgs e)
        {

        }

        private void FileExitClick(object sender, RoutedEventArgs e)
        {

        }

        private void ToolsModifyClick(object sender, RoutedEventArgs e)
        {

        }

        private void ToolsBlueprintClick(object sender, RoutedEventArgs e)
        {

        }

        private void ToolsSaveClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            ZoneWindow zw = new ZoneWindow(1);
            zw.Owner = this;
            //this.Hide();
            zw.Show();
        }
    }
}
