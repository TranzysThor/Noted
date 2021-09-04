using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Windows;

namespace Capstone.MVVM.View
{
    public partial class BrowseView : UserControl
    {
        private SqlConnection _conn = new SqlConnection();
        private SqlCommand _command = new SqlCommand();
        private SqlDataAdapter _adapter;
        private DataTable _dt;
        public BrowseView()
        {
            InitializeComponent();
            _conn.ConnectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            
        }

        private void ListFiles(object sender, RoutedEventArgs e)
        {
            _conn.Open();
            TextBlock tmp = Application.Current.Windows[0].FindName("Username") as TextBlock;
            string command = "select Filename, LastModified from Files where UserID = (select ID from Users where Username = '" + tmp.Text + "')";
            _command.Connection = _conn;
            _command.CommandText = command;
            _adapter = new SqlDataAdapter(_command);
            _dt = new DataTable("Files");
            _adapter.Fill(_dt);
            FilesGrid.ItemsSource = _dt.DefaultView;
            FilesGrid.Columns[0].Width = 350;
            FilesGrid.Columns[1].Width = 350;
            _adapter.Update(_dt);
            _conn.Close();
        }
    }
}