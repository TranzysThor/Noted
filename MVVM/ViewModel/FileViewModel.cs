using System;
using System.Windows.Input;
using Capstone.Core;
using Capstone.MVVM.Model;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Controls;

namespace Capstone.MVVM.ViewModel
{
    public class FileViewModel
    {
        private SqlConnection _conn = new SqlConnection();
        private SqlCommand _command = new SqlCommand();
        public DocumentModel Document { get; private set; }
        
        //Toolbar commands
        public ICommand NewCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand OpenCommand { get; }

        public FileViewModel(DocumentModel document)
        {
            _conn.ConnectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            Document = document;
            NewCommand = new RelayCommand(NewFile);
            SaveAsCommand = new RelayCommand(SaveFileAs);
            OpenCommand = new RelayCommand(OpenFile);
        }

        private void OpenFile(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            TextBlock tmp = Application.Current.Windows[0].FindName("Username") as TextBlock;
            openFileDialog.InitialDirectory = $@"D:\Noted!\{tmp.Text}";
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName.Contains($@"D:\Noted!\{tmp.Text}"))
                {
                    DockFile(openFileDialog);
                    Document.Text = File.ReadAllText(openFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("You may only access files within your own folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void SaveFileAs(object obj)
        {
            var saveFileDialog = new SaveFileDialog();
            TextBlock tmp = Application.Current.Windows[0].FindName("Username") as TextBlock;
            saveFileDialog.InitialDirectory = $@"D:\Noted!\{tmp.Text}";
            saveFileDialog.Filter = "Text File (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FileName.Contains($@"D:\Noted!\{tmp.Text}"))
                {
                    DockFile(saveFileDialog);
                    File.WriteAllText(saveFileDialog.FileName, Document.Text);
                    _conn.Open();
                    _command.Connection = _conn;
                    _command.CommandText = "select * from Files";
                    SqlDataReader reader = _command.ExecuteReader();
                    bool checkExisting = false;
                    while (reader.Read())
                    {
                        if (Convert.ToString(reader["Filename"]) == saveFileDialog.FileName)
                        {
                            checkExisting = true;
                        }
                    }

                    _conn.Close();
                    _conn.Open();
                    var date = Convert.ToDateTime(File.GetLastWriteTime(saveFileDialog.FileName));
                    string lastWriteTime =
                        $"{date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second}";
                    if (checkExisting == false)
                    {
                        _command.CommandText =
                            "insert into Files(UserID, Filename, LastModified) values((select ID from Users where Username = '" +
                            tmp.Text + "'), '" + saveFileDialog.FileName + "', '" +
                            lastWriteTime + "')";
                        _command.ExecuteNonQuery();
                    }
                    else if (checkExisting)
                    {
                        _command.CommandText = "update Files set LastModified = '" + lastWriteTime +
                                               "' where Filename = '" + saveFileDialog.FileName + "'";
                        _command.ExecuteNonQuery();
                    }

                    _conn.Close();
                }
                else
                {
                    MessageBox.Show("You may only save files within your own folder!", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                }
            }
        }

        private void NewFile(object obj)
        {
            Document.FileName = string.Empty;
            TextBlock tmp = Application.Current.Windows[0].FindName("Username") as TextBlock;
            Document.FilePath = $@"D:\Noted!\{tmp.Text}";
            Document.Text = string.Empty;
        }

        private void DockFile<T>(T dialog) where T : FileDialog
        {
            Document.FilePath = dialog.FileName;
            Document.FileName = dialog.SafeFileName;
        }
    }
}