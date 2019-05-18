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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ZoneWindow.xaml
    /// </summary>
    public partial class ZoneWindow : Window
    {
        public ZoneWindow()
        {
            InitializeComponent();
            bindDataGrid();
            //If there is some way to pass in data to the new window the title can be dynamically set here so that
            //it displays the proper zone name
            //The zone data id is also required for the query below to get accurate results
            this.Title = "Poopy Test";

        }

        // -------------This is an example of how to bind data to a data grid --------------- //
        // -------------                  Use it for reference                --------------- //
        private void bindDataGrid() {
            //This proves that you can set the room ID dynamically
            int roomNumber = 1;

            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.Open();

            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand();
            //This is where you write your query to populate the table
            //You can write any kind of query here
            cmd.CommandText = "SELECT partNo FROM [parts] WHERE roomID = " + roomNumber;
            //Sets the commands connectio
            cmd.Connection = sqlCon;

            //Creates a new SQL Data Adapter (not sure what this does)
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //Creates a new Data Table
            DataTable dtbl = new DataTable("Parts");
            //Fills the data adapter with the information in the data table
            da.Fill(dtbl);

            //Sets the xaml data grid to display the data adapted table
            dgParts.ItemsSource = dtbl.DefaultView;
        }
    }
}
