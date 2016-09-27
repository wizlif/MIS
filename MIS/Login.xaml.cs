using System.Windows;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data;
using MahApps.Metro;

namespace MIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static string cs = "Server="+GetServerAddress()+";Database=MIS;Uid=mis;Pwd=isaacobella;";
        MySqlConnection _conn = new MySqlConnection(cs);
        MySqlCommand _cmd;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void StartStyle()
        {

                    ThemeManager.ChangeAppStyle(Application.Current,
                                            ThemeManager.GetAccent(GetStyle()),
                                            ThemeManager.GetAppTheme("BaseLight"));
                
        }
        // Open Signup window
        private void ButtonSignUp_Click(object sender, RoutedEventArgs e)
        {
            (new Sign_up()).Show();
            this.Close();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            (new Settings()).ShowDialog();
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            // MySQL Statement
            string _Query = @"SELECT COUNT(*) FROM Accounts WHERE `Account Type`='"+ComboBoxAccount.Text+"' AND Username='" + TextBoxUsername.Text + "' AND Password='"+PasswordBox.Password+"'";
            _cmd = new MySqlCommand(_Query, _conn);

            // Check If Account Exists

            try
            {
                _conn.Open();
                if (Convert.ToInt32(_cmd.ExecuteScalar()) ==1)
                {
                    var controller = await this.ShowProgressAsync("Please wait...", "Checking account ...");
                    await Task.Delay(2000);
                    controller.SetCancelable(false);
                    controller.SetProgress(.25);
                    controller.SetMessage("Loging in ...");
                    await Task.Delay(2000);
                    controller.SetProgress(.50);
                    controller.SetMessage("Closing dependant processes ...");
                    await Task.Delay(2000);
                    controller.SetProgress(1.00);

                    if (ComboBoxAccount.Text == "Admin")
                    {
                        Admin na=new Admin();
                        na.acc= TextBoxUsername.Text;
                        na.Show();
                        this.Close();
                    }else if (ComboBoxAccount.Text == "PHRO")
                    {
                        NewEmployee ne=new NewEmployee();
                        ne.acc = TextBoxUsername.Text;
                        ne.Show();
                        this.Close();
                    }else if(ComboBoxAccount.Text == "AC/HRM")
                    {
                        (new WindowHR()).Show();

                        this.Close();
                    }
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

        public static string GetServerAddress()
        {
            string ipaddress = "";
            using(StreamReader reader=new StreamReader(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName).ToString() + "\\Settings.txt"))
            {
                ipaddress = reader.ReadLine();
            }
            return ipaddress;
        }

        public static string GetStyle()
        {
            string style = "";
            using (StreamReader reader = new StreamReader(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName).ToString() + "\\Style.txt"))
            {
                style = reader.ReadLine();
            }
            return style;
        }


        private void WindowLogin_Loaded(object sender, RoutedEventArgs e)
        {
            StartStyle();
        }
    }
}
