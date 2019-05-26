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
        private bool isUserBeingAdded = false;
        //DataGridRow pimpDaddy;

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

            // Take a look at this query later, OK for primary injection, but could be suceptable to secondary sql injection specifically the ELSE statement in this IF
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
            sqlCon.Close();
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (!isUserBeingAdded)
            {
                isUserBeingAdded = true;
                dgLogins.CanUserAddRows = true;
                btnAddUser.Content = "End Edit";
            }
            else {
                isUserBeingAdded = false;
                dgLogins.CanUserAddRows = false;
                btnAddUser.Content = "Add Users";
            }
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
            string strSelectedID = rowSelected["ID"].ToString();
            string strSelectedUsername = rowSelected["username"].ToString();
            //This is where you write your query to populate the table
            //You can write any kind of query here
            //Delete statements are OK for security, and its useing the ID to delete which is database created
            //Could potentially be a stored proc but i dont see the need as of now
            cmd.CommandText = "DELETE FROM [login] WHERE ID = " + strSelectedID;
            cmd.Connection = sqlCon;
            if (MessageBox.Show("Are you sure you want to delete user " + strSelectedUsername + "?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                cmd.ExecuteNonQuery();
                bindDataGrid();
            }
            else {
                MessageBox.Show("Deletion Canceled");
            }
            sqlCon.Close();
        }

        private void DgLogins_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            ////Gets all the data from the row that is selected
            DataRowView row_Selected = gd.SelectedItem as DataRowView;

            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.Open();
            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand("sp_AddOrEditLoginPermissions", sqlCon) { CommandType = CommandType.StoredProcedure }; ;
            //DataRowView row_Selected = dgLogins.SelectedItem as DataRowView;
            DataRowView rowAdded = dgLogins.CurrentItem as DataRowView;

            // -------- These Parameters work so it proves that parameters and the sql command is working correctly //
            cmd.Parameters.AddWithValue("@username", "sp");
            cmd.Parameters.AddWithValue("@password", "sp");
            cmd.Parameters.AddWithValue("@MRP", true);
            cmd.Parameters.AddWithValue("@ITRP", 1);
            cmd.Parameters.AddWithValue("@TP", 0);
            cmd.Parameters.AddWithValue("@AP", 0);

            // ---------- On the other hand this should work but everytime I try to use the current selected rows values everything returns null
            //cmd.Parameters.AddWithValue("@username", row_Selected["username"].ToString());
            //cmd.Parameters.AddWithValue("@password", row_Selected["password"].ToString());
            //cmd.Parameters.AddWithValue("@MRP", row_Selected["maintenanceRecordsPermission"]);
            //cmd.Parameters.AddWithValue("@ITRP", row_Selected["itRecordsPermissions"]);
            //cmd.Parameters.AddWithValue("@TP", row_Selected["teacherPermissions"]);
            //cmd.Parameters.AddWithValue("@AP", row_Selected["adminPermissions"]);

            // This statement prints all of the parameters values (guess what? they return null and I dont know how to fix it)
            Console.WriteLine(row_Selected["username"].ToString() + row_Selected["password"].ToString() + row_Selected["maintenanceRecordsPermission"] + row_Selected["itRecordsPermissions"] + row_Selected["teacherPermissions"] + row_Selected["adminPermissions"]);

            cmd.ExecuteNonQuery();
            sqlCon.Close();
        }

        private void DgLogins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataGrid gd = (DataGrid)sender;
            ////Gets all the data from the row that is selected
            //pimpDaddy = gd.SelectedItem as DataRowView;
            //DataGridRow dgr = gd.SelectedItem as DataGridRow;
        }
    }
}
