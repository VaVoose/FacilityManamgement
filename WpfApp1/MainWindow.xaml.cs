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
            //Syntax of connection
            //SQL Authentication
            //@"Data Source=(MachineName)\(InstanceName); Initial Catalog=(DBName); User ID=(Username); Password=(UserPassword);"
            SqlConnection sqlCon = new SqlConnection(@"Data Source=99.153.22.230; Initial Catalog=master; User ID=SA; Password=sqlFacility2");
            SqlDataAdapter sqlDA = new SqlDataAdapter("Select * from equiptment", sqlCon);
            DataTable dtbl = new DataTable();
            sqlDA.Fill(dtbl);
            foreach (DataRow row in dtbl.Rows) {
                Console.WriteLine(row["name"]);
                Console.WriteLine(row["location"]);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
