using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ModifyLoginWindow.xaml
    /// </summary>
    public partial class ModifyLoginWindow : Window
    {
        public ModifyLoginWindow()
        {
            InitializeComponent();
            setUseableColumns();
            bindDataGrid();
        }

        // Gets the info from CurrentUser and sets the ability to change the data table based on the users permissions
        private void setUseableColumns() {
            // Can be set visible or read only not sure which on at this time
            // Admin users can change permissions of all other groups exept other admin possitions
            // Super Admin user can change admin permissions as well
            if (CurrentUser.getAP()) {
                dgtcMRP.Visibility = Visibility.Visible;
                dgtcITRP.Visibility = Visibility.Visible;
                dgtcTP.Visibility = Visibility.Visible;
            }
            if (CurrentUser.getSAP()) {
                dgtcAP.Visibility = Visibility.Visible;
                dgLogins.CanUserAddRows = true;
                dgtcUsername.IsReadOnly = false;
            }
        }

        private void bindDataGrid()
        {
            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.ConnectionString += ";Connection Timeout=30";

            int retries = 0;
            while (true)
            {
                try
                {
                    sqlCon.Open();
                    break;
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    if (++retries == 3) throw;
                    System.Windows.Forms.MessageBox.Show("Connection Failed. Retry iteration " + (retries));
                    sqlCon.Close();
                    continue;
                }
            }

            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand();
            //This is where you write your query to populate the table
            //You can write any kind of query here
            if (CurrentUser.getAP() || CurrentUser.getSAP()) cmd.CommandText = "SELECT * FROM [login]";
            else cmd.CommandText = "SELECT * FROM [login] WHERE username='" + CurrentUser.getUsername() + "'";
            //Sets the commands connection
            cmd.Connection = sqlCon;

            //Creates a new SQL Data Adapter (not sure what this does)
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //Creates a new Data Table
            DataTable dtbl = new DataTable("Parts");
            //Fills the data adapter with the information in the data table
            da.Fill(dtbl);

            //Sets the xaml data grid to display the data adapted table
            dgLogins.ItemsSource = dtbl.DefaultView;
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.Open();
            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand();
            DataRowView rowSelected = dgLogins.SelectedItem as DataRowView;
            string strSelectedUsername = rowSelected["ID"].ToString();
            //This is where you write your query to populate the table
            //You can write any kind of query here
            cmd.CommandText = "DELETE FROM [login] WHERE ID = " + strSelectedUsername;
            //Sets the commands connection
            cmd.Connection = sqlCon;
            cmd.ExecuteNonQuery();
            bindDataGrid();
        }
    }
}
