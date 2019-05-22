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
using System.IO;
using Microsoft.Win32;
using System.Drawing;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ZoneWindow.xaml
    /// </summary>
    public partial class ZoneWindow : Window
    {
        public int roomNumber;
        String filePath;
        public Byte[] imageByteArray;
        public SqlConnection sqlCon;

        // Constructor for the Zone Window
        // Param zoneID: The id of the zone that the information will be queried from
        public ZoneWindow(int zoneID)
        {
            InitializeComponent();
            //If there is some way to pass in data to the new window the title can be dynamically set here so that
            //it displays the proper zone name
            //The zone data id is also required for the query below to get accurate results
            this.Title = "Zone " + zoneID;
            roomNumber = zoneID;
            openSQLConnection();
            bindDataGrid();
        }

        public void openSQLConnection() {

        }

        // This overrides the red 'X' button to shutdown the application without having to close the previous window
        // This should be in every windows code
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        // -------------This is an example of how to bind data to a data grid --------------- //
        // -------------                  Use it for reference                --------------- //
        private void bindDataGrid() {
            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;

            System.Windows.Forms.MessageBox.Show("Connecting to Database...");

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

                    System.Windows.Forms.MessageBox.Show("Connection Failed. Retry iteration " + (retries + 1));

                    sqlCon.Close();
                }
            }
            
            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand();
            //This is where you write your query to populate the table
            //You can write any kind of query here
            cmd.CommandText = "SELECT * FROM [parts] WHERE roomID = " + roomNumber;
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

        // This function updates when a new row is selected from the data grid
        // This function will be used to update the picture and the details that are saved with each part
        private void DgParts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Gets the datagrid that is being changed
            DataGrid gd = (DataGrid)sender;
            //Gets all the data from the row that is selected
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            //If the row is not null
            if (row_selected != null) {
                //changed the text of the text box to the the part number
                //"partNo" is the column header of the specific column that is used in the DATABASE (not set in the program)
                txtTest.Text = row_selected["partNo"].ToString();
            }

            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.Open();
            //Instantiates a new sql command string
            SqlCommand cmd = new SqlCommand();
            DataRowView rowSelected = dgParts.SelectedItem as DataRowView;
            string strCurrentRowID = rowSelected["ID"].ToString();
            //This is where you write your query to populate the table
            //You can write any kind of query here
            cmd.CommandText = "SELECT * FROM [partDocuments] WHERE partID = " + strCurrentRowID ;
            //Sets the commands connectio
            cmd.Connection = sqlCon;

            //Creates a new SQL Data Adapter (not sure what this does)
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //Creates a new Data Table
            DataTable dtbl = new DataTable("Documents");
            //Fills the data adapter with the information in the data table
            da.Fill(dtbl);

            //Sets the xaml data grid to display the data adapted table
            lvDetails.ItemsSource = dtbl.DefaultView;
            gvcType.DisplayMemberBinding = new Binding("ID");
            gvcText.DisplayMemberBinding = new Binding("documentText");  
        }

        //Opens dialog box to select a picture to be added
        private void BtnAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg, *.png)|*.png; *.jpg";
            //Currently assumes valid file
            ofd.ShowDialog();
            filePath = ofd.FileName;
            lblFileName.Content = System.IO.Path.GetFileName(filePath);
        }

        // Submits the text as a new "document" in the listview
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {

        }

        //
        // Image submit functionallity currently does not work
        // Possibly to be completely reworked
        //
        private void BtnSubmitPic_Click(object sender, RoutedEventArgs e)
        {
            // ------ 
            //        For some reason I have to reinstatiate the connection string everytime I want to use it in the same window
            //        In not sure how to fix this as of now
            //        maybe a specific static class that holds the sqlConnection?
            //        Somehow instatiate it for the whole window? (tried this with no luck)
            //------- 

            //Instantiates a Connection String
            SqlConnection sqlCon = new SqlConnection();
            //Sets the connection string to point to the master connection set in "App.config"
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["masterConnection"].ConnectionString;
            sqlCon.Open();
            System.Drawing.Image temp = new Bitmap(filePath);
            MemoryStream strm = new MemoryStream();
            temp.Save(strm, System.Drawing.Imaging.ImageFormat.Png);
            imageByteArray = strm.ToArray();
            if (sqlCon.State == ConnectionState.Closed) sqlCon.Open();
            SqlCommand sqlCmdAddImage = new SqlCommand("sp_ImageAddOrEdit", sqlCon) { CommandType = CommandType.StoredProcedure };
            DataRowView rowSelected = dgParts.SelectedItem as DataRowView;
            string strCurrentRowID = rowSelected["ID"].ToString();
            foreach (int i in imageByteArray)
            {
                Console.Write(imageByteArray[i]);
            }
            sqlCmdAddImage.Parameters.Add("@ID", strCurrentRowID);
            sqlCmdAddImage.Parameters.Add("@image", imageByteArray);

            sqlCmdAddImage.ExecuteNonQuery();
        }
    }
}
