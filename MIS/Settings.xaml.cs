using System.Windows;
using MahApps.Metro.Controls;
using System;
using MahApps.Metro;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace MIS
{
    /// <summary>
    /// Interaction logic for Sign_up.xaml
    /// </summary>
    public partial class Settings : MetroWindow
    {
        public static string cs = "Server="+GetServerAddress()+";Database=MIS;Uid=mis;Pwd=isaacobella;";
        MySqlConnection _conn = new MySqlConnection(cs);
        MySqlCommand _cmd;
        public Settings()
        {
            InitializeComponent();
        }

        

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //set the Red accent and dark theme only to the current window
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent(((RadioButton)sender).Content.ToString()),
                                        ThemeManager.GetAppTheme("BaseLight"));
            SetStyle(((RadioButton)sender).Content.ToString());
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            // MySQL Statement
            string _Query = @"SELECT COUNT(*) FROM Accounts WHERE `Account Type`='" + ComboBoxAccount.Text + "' AND Username='" + TextBoxUsername.Text + "' AND Password='" + PasswordBoxOld.Password + "'";
            _cmd = new MySqlCommand(_Query, _conn);

            // Check If Account Exists
            if (PasswordBoxNew.Password == PasswordBoxConfirm.Password)
            {
                try
                {
                    _conn.Open();
                    if (Convert.ToInt32(_cmd.ExecuteScalar()) == 1)
                    {
                        _conn.Close();
                        UPDATE_ACCOUNT(PasswordBoxNew.Password,TextBoxUsername.Text);

                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Username or Password incorrect");
                    }
                    _conn.Close();

                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "Server Errror check your server connection");

                }
            }
            else
            {
                await this.ShowMessageAsync("Error", "Passwords do not match");
            }
        }

        public async void UPDATE_ACCOUNT(string _Password, string _Username)
        {
            // MySQL Statement
            string _Query = @"UPDATE Accounts SET Password='" + _Password + "' WHERE Username='"+_Username+"'";
            _cmd = new MySqlCommand(_Query, _conn);

            // Open and Execute

            try
            {
                _conn.Open();

                if (_cmd.ExecuteNonQuery() > 0)
                {
                    await this.ShowMessageAsync("Password", "Password updated successfully");

                    TextBoxUsername.Text="";
                    PasswordBoxConfirm.Password = "";
                    PasswordBoxNew.Password = "";
                    PasswordBoxOld.Password = "";

                    this.Close();
                }

                _conn.Close();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection");
            }
        }

        private async void TextBoxIPAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName).ToString() + "\\Settings.txt"))
                {
                    writer.WriteLine(TextBoxIPAddress.Text);
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Error occured while changing server ipaddress");
            }
        }

        private void TextBoxIPAddress_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxIPAddress.Text = GetServerAddress();
        }
        public static string GetServerAddress()
        {
            string ipaddress = "";
            using (StreamReader reader = new StreamReader(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName).ToString() + "\\Settings.txt"))
            {
                ipaddress = reader.ReadLine();
            }
            return ipaddress;
        }

        public async void SetStyle(string style)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName).ToString() + "\\Style.txt"))
                {
                    writer.WriteLine(style);
                }
                await this.ShowMessageAsync("Success", "Style has been changed successfully");

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Error occured while changing style");
            }
        }

        
    }
}
