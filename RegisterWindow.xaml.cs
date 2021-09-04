using System;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Media;

namespace Capstone
{
    public partial class RegisterWindow : Window
    {
        private SqlConnection _conn = new SqlConnection();
        private SqlCommand _command = new SqlCommand();
        private SqlDataReader _dr;
        public RegisterWindow()
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to go back to login window?",
                "Exit Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow newWind = new MainWindow();
                this.Close();
                newWind.ShowDialog();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if(_conn.State == System.Data.ConnectionState.Open)
            {
                _conn.Close();
            }

            if (UserName.Text != "" && Email.Text != "" && UserPassword.Password != "" &&
                ConfirmUserPassword.Password != "" && new EmailAddressAttribute().IsValid(Email.Text))
            {
                if (VerifyNewUser(UserName.Text, UserPassword.Password, ConfirmUserPassword.Password))
                {
                    SendEMail(Email.Text);
                    MessageBox.Show("Registered Successfully", "Registration", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    MainWindow newWind = new MainWindow();
                    this.Close();
                    newWind.ShowDialog();
                }
            }
            else
            {
                if (!new EmailAddressAttribute().IsValid(Email.Text) && Email.Text != "")
                {
                    Email.Foreground = Brushes.Red;
                    Email.ToolTip = "Incorrect EMail Type";
                }
                else if (Email.Text == "")
                {
                    Email.Foreground = Brushes.Red;
                    Email.ToolTip = "Email Must Be Filled";
                }
                else
                {
                    Email.Foreground = btnRegister.Foreground;
                    Email.ClearValue(ToolTipProperty);
                }

                if (UserName.Text == "")
                {
                    UserName.Foreground = Brushes.Red;
                    UserName.ToolTip = "Username Must Be Filled";
                }
                else
                {
                    UserName.Foreground = btnRegister.Foreground;
                    UserName.ClearValue(ToolTipProperty);
                }

                if (UserPassword.Password == "")
                {
                    UserPassword.Foreground = Brushes.Red;
                    UserPassword.ToolTip = "Password Must Be Filled";
                }
                else
                {
                    UserPassword.Foreground = btnRegister.Foreground;
                    UserPassword.ClearValue(ToolTipProperty);
                }

                if (ConfirmUserPassword.Password == "")
                {
                    ConfirmUserPassword.Foreground = Brushes.Red;
                    ConfirmUserPassword.ToolTip = "Confirm Password Must Be Filled";
                }
                else
                {
                    ConfirmUserPassword.Foreground = btnRegister.Foreground;
                    ConfirmUserPassword.ClearValue(ToolTipProperty);
                }
            }
        }

        private void SendEMail(string email)
        {
            MailAddress from = new MailAddress("maxim.akulevich@gmail.com");
            MailAddress to = new MailAddress(email);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Registration";
            message.Body = "<h2>You Have Been Successfully Registered on Noted!</h2>";
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("maxim.akulevich@gmail.com", "jvbmpcuzfrsqpzwk");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            try
            { 
                smtp.Send(message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private bool VerifyNewUser(string username, string password, string confirmPassword)
        {
            _conn.Open();
            _command.Connection = _conn;
            _command.CommandText = "select Username from Users where Username='" + username + "'";
            _dr = _command.ExecuteReader();
            if (!_dr.Read())
            {
                if (password.Length >= 5 && password.Length <= 20)
                {
                    UserPassword.Foreground = btnRegister.Foreground;
                    UserPassword.ClearValue(ToolTipProperty);
                    if (password == confirmPassword)
                    {
                        string dir = $@"D:\Noted!\{username}";
                        Directory.CreateDirectory(dir);
                        _conn.Close();
                        _conn.Open();
                        _command.Connection = _conn;
                        _command.CommandText = "insert into Users(Username, Password, Status, Role, FolderPath) values('" +
                                               UserName.Text +
                                               "','" + UserPassword.Password.GetHashCode() + "', 1, 'User'" + ",'" + dir + "')";
                        _command.ExecuteNonQuery();
                        return true;
                    }
                    else
                    {
                        ConfirmUserPassword.ToolTip = "Passwords Don't Match";
                        ConfirmUserPassword.Foreground = Brushes.Red;
                        return false;
                    }
                }
                else
                {
                    UserPassword.Foreground = Brushes.Red;
                    UserPassword.ToolTip = "Password Length Must Be Between 5 and 20 Characters";
                    return false;
                }
            }
            else
            { 
                return false;
            }
        }
    }
}