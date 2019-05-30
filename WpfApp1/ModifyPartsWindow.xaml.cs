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
    /// Interaction logic for ModifyPartsWindow.xaml
    /// </summary>
    public partial class ModifyPartsWindow : Window
    {
        private bool isUserBeingAdded = false;

        public ModifyPartsWindow()
        {
            InitializeComponent();
            bindDataGrid();
        }

        private void bindDataGrid() {
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
            SqlCommand cmd = new SqlCommand("sp_QueryParts", sqlCon) { CommandType = CommandType.StoredProcedure};
            //Creates a new SQL Data Adapter (not sure what this does)
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //Creates a new Data Table
            DataTable dtbl = new DataTable("Parts");
            //Fills the data adapter with the information in the data table
            da.Fill(dtbl);

            //Sets the xaml data grid to display the data adapted table
            dgParts.ItemsSource = dtbl.DefaultView;
            sqlCon.Close();
        }

        private void BtnAddParts_Click(object sender, RoutedEventArgs e)
        {
            if (!isUserBeingAdded)
            {
                isUserBeingAdded = true;
                dgParts.CanUserAddRows = true;
                btnAddParts.Content = "End Edit";
            }
            else
            {
                isUserBeingAdded = false;
                dgParts.CanUserAddRows = false;
                btnAddParts.Content = "Add Users";
            }
        }

        private void BtnDeletePart_Click(object sender, RoutedEventArgs e)
        {
            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.Open();
            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand();
            DataRowView rowSelected = dgParts.SelectedItem as DataRowView;
            string strSelectedID = rowSelected["ID"].ToString();
            string strSelectedPartNo = rowSelected["PartNo"].ToString();
            //This is where you write your query to populate the table
            //You can write any kind of query here
            //Delete statements are OK for security, and its useing the ID to delete which is database created
            //Could potentially be a stored proc but i dont see the need as of now
            cmd.CommandText = "DELETE FROM [parts] WHERE ID = " + strSelectedID;
            cmd.Connection = sqlCon;
            if (MessageBox.Show("Are you sure you want to delete " + strSelectedPartNo + "?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                cmd.ExecuteNonQuery();
                bindDataGrid();
            }
            else
            {
                MessageBox.Show("Deletion Canceled");
            }
            sqlCon.Close();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {

        }

        // This currently does not work, just copied and pasted code from modify login window
        private void DgParts_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            //Console.WriteLine(grdItem);
            ////Gets all the data from the row that is selected
            DataRowView row_Selected = gd.SelectedItem as DataRowView;

            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.Open();
            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand("sp_AddOrEditParts", sqlCon) { CommandType = CommandType.StoredProcedure }; ;

            //Sets parameters based on the new rows values
            cmd.Parameters.AddWithValue("@ID", row_Selected["ID"]);
            cmd.Parameters.AddWithValue("@username", row_Selected["username"].ToString());
            cmd.Parameters.AddWithValue("@password", row_Selected["password"].ToString());
            cmd.Parameters.AddWithValue("@first", row_Selected["firstName"].ToString());
            cmd.Parameters.AddWithValue("@last", row_Selected["lastName"].ToString());

            // If the inputted value is null it returns false, this is needed because the default value when creating a new row is null
            if (row_Selected["maintenanceRecordsPermission"] == DBNull.Value) cmd.Parameters.AddWithValue("@MRP", 0);
            else cmd.Parameters.AddWithValue("@MRP", row_Selected["maintenanceRecordsPermission"]);
            if (row_Selected["itRecordsPermissions"] == DBNull.Value) cmd.Parameters.AddWithValue("@ITRP", 0);
            else cmd.Parameters.AddWithValue("@ITRP", row_Selected["itRecordsPermissions"]);
            if (row_Selected["teacherPermissions"] == DBNull.Value) cmd.Parameters.AddWithValue("@TP", 0);
            else cmd.Parameters.AddWithValue("@TP", row_Selected["teacherPermissions"]);
            if (row_Selected["adminPermissions"] == DBNull.Value) cmd.Parameters.AddWithValue("@AP", 0);
            else cmd.Parameters.AddWithValue("@AP", row_Selected["adminPermissions"]);

            MessageBox.Show("Row edit ended");
            //This statement prints all of the parameters values for debug
            //Console.WriteLine(row_Selected["ID"] + row_Selected["username"].ToString() + row_Selected["password"].ToString() + row_Selected["maintenanceRecordsPermission"] + row_Selected["itRecordsPermissions"] + row_Selected["teacherPermissions"] + row_Selected["adminPermissions"]);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                //handleSqlException(exp);
            }

            sqlCon.Close();
            //Rebinding the datagrid is needed because if the value is inputted as null is will continue to look like its null in the grid unless updated, then it will become false
            bindDataGrid();
        }
    }
}
