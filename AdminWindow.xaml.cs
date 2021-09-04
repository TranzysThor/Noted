using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Windows.Controls;
using System.Windows.Data;

namespace Capstone
{
    public partial class AdminWindow : Window
    {
        private SqlConnection _conn = new SqlConnection();
        private SqlCommand _command = new SqlCommand();
        private SqlDataAdapter _adapter;
        private DataTable _dt;
        public AdminWindow()
        {
            InitializeComponent();
            _conn.ConnectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        public AdminWindow(string username)
        {
            InitializeComponent();
            Username.Text = username;
            _conn.ConnectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void LoadGrid_Click(object sender, RoutedEventArgs e)
        {
            _conn.Open();
            string command = "select ID, Username, Status, Role from Users where Role = 'User'";
            _command.Connection = _conn;
            _command.CommandText = command;
            _adapter = new SqlDataAdapter(_command);
            _dt = new DataTable("Users");
            _adapter.Fill(_dt);
            UsersGrid.ItemsSource = _dt.DefaultView;
            UsersGrid.Columns[0].Width = 180;
            UsersGrid.Columns[0].IsReadOnly = true;
            UsersGrid.Columns[1].Width = 180;
            UsersGrid.Columns[2].Width = 180;
            UsersGrid.Columns[3].Width = 180;
            UsersGrid.Columns[3].IsReadOnly = true;
            _adapter.Update(_dt);
            _conn.Close();
        }

        private void UpdateDb(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable changes = _dt.GetChanges();
                if (changes != null)
                {
                    _adapter.UpdateCommand = new SqlCommandBuilder(_adapter).GetUpdateCommand();
                    _adapter.Update(changes);
                    MessageBox.Show("Updated");
                    _dt.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the application?",
                "Exit Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to leave your account?",
                "Exit Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow newWind = new MainWindow();
                this.Close();
                newWind.ShowDialog();
            }
        }
    }
}