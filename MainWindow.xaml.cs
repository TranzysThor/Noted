using System;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Windows.Documents;
using System.Management.Instrumentation;
using System.Windows.Media;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private SqlConnection _conn = new SqlConnection();
        private SqlCommand _command = new SqlCommand();
        private SqlDataReader _dr;
        public MainWindow()
        {
            InitializeComponent();
            _conn.ConnectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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

        private void RegisterText_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {                
                RegisterWindow newWind = new RegisterWindow();
                this.Close();
                newWind.ShowDialog();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(_conn.State == System.Data.ConnectionState.Open)
            {
                _conn.Close();
            }

            if (UserName.Text != "" && UserPassword.Password != "")
            {
                List<bool> check = VerifyUser(UserName.Text, UserPassword.Password);
                if (check[0] == true && check[1] == false)
                {
                    Second newWind = new Second(UserName.Text);
                    this.Close();
                    newWind.ShowDialog();
                }
                else if (check[0] == true && check[1] == true)
                {
                    AdminWindow newWind = new AdminWindow(UserName.Text);
                    this.Close();
                    newWind.ShowDialog();
                }
                else
                {
                    UserName.Foreground = Brushes.Red;
                    UserName.ToolTip = "Username or Password is incorrect";
                    UserPassword.Foreground = Brushes.Red;
                    UserPassword.ToolTip = "Username or Password is incorrect";
                }
            }
            else
            {
                if (UserName.Text == "")
                {
                    UserName.Foreground = Brushes.Red;
                    UserName.ToolTip = "Username Must Be Filled";
                }
                else
                {
                    UserName.Foreground = btnLogin.Foreground;
                    UserName.ClearValue(ToolTipProperty);
                }

                if (UserPassword.Password == "")
                {
                    UserPassword.Foreground = Brushes.Red;
                    UserPassword.ToolTip = "Password Must Be Filled";
                }
                else
                {
                    UserPassword.Foreground = btnLogin.Foreground;
                    UserPassword.ClearValue(ToolTipProperty);
                }
            }
        }

        private List<bool> VerifyUser(string username, string password)
        {
            _conn.Open();
            _command.Connection = _conn;
            _command.CommandText = "select Status, Role from Users where Username='" + username + "' and password='" +
                                   password.GetHashCode() + "'";
            _dr = _command.ExecuteReader();
            if (_dr.Read())
            {
                if (Convert.ToBoolean(_dr["Status"]) == true && Convert.ToString(_dr["Role"]) == "Admin")
                {
                    return new List<bool>(2){true, true};
                }
                else if (Convert.ToBoolean(_dr["Status"]) == true && Convert.ToString(_dr["Role"]) == "User")
                {
                    return new List<bool>(2){true, false};
                }
                else
                {
                    MessageBox.Show("Your Account Has Been Suspended or Doesn't Exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<bool>(2){false, false};
                }
            }
            else
            {
                return new List<bool>(2){false, false};
            }
        }
    }
}