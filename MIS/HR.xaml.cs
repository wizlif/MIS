using System.Windows;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Telerik.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.Diagnostics;

namespace MIS
{
    /// <summary>
    /// Interaction logic for Sign_up.xaml
    /// </summary>
    public partial class WindowHR : MetroWindow
    {
        public static string cs = "Server="+GetServerAddress()+";Database=MIS;Uid=mis;Pwd=isaacobella;";
        MySqlConnection _Conn = new MySqlConnection(cs);
        DateTime outDate = DateTime.Now;
        //MySqlCommand _cmd;
        public WindowHR()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close();
        }

        private async void WindowHumanR_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            (new Admin()).DataGridBinding("SELECT * FROM Employee", "Employees", DataGridEmployees);
            popLink.IsOpen = true;
            try { 
            DataTable table = GetDataTable();
                foreach (DataRow row in table.Rows)
                {
                    DateTime datevalue = (Convert.ToDateTime(row["Date of Rotation"].ToString()));
                    

                    DateTime datevalueRetire = (Convert.ToDateTime(row["Date of Retirement"].ToString()));
                   

                  

                    if (Convert.ToDateTime(datevalueRetire.ToString()).Date <= Convert.ToDateTime(DateTime.Now.ToString()).Date && row["Status"].ToString() != "Retired")
                    {
                        var DialogSettings = new MetroDialogSettings() { AffirmativeButtonText = "Retire", NegativeButtonText = "Cancel" };
                        MessageDialogResult result=await this.ShowMessageAsync("Retire", row["Name"] + " currently working at " + row["Current Station"] + " under " + row["Type of Station"] + " station is supposed to have retired on "+ row["Date of Retirement"].ToString() + ". Please setup a job advert for his/her vacancy as soon as possible.", MessageDialogStyle.AffirmativeAndNegative, DialogSettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            RETIRE_EMPLOYEE(row["Name"].ToString(), Convert.ToInt32(row["Id"].ToString()));
                        }
                    }
                    else if (Convert.ToDateTime(datevalue.ToString()).Date <= Convert.ToDateTime(DateTime.Now.ToString()).Date && row["Status"].ToString() != "Retired")
                    {
                        var DialogSettings = new MetroDialogSettings() { AffirmativeButtonText = "Rotate", NegativeButtonText = "Cancel" };
                        MessageDialogResult result = await this.ShowMessageAsync("Rotate", row["Name"] + " currently working at " + row["Current Station"] + " under " + row["Type of Station"] + " station is supposed to be moved soon, most preferably to any other station.", MessageDialogStyle.AffirmativeAndNegative, DialogSettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            ROTATE_EMPLOYEE(row["Name"].ToString(), Convert.ToInt32(row["Id"].ToString()), DOR(Convert.ToDateTime(Convert.ToDateTime(row["Date of Rotation"].ToString()).ToString())).ToString("yyyy-MM-dd"), row["Type of Station"].ToString());
                        }
                        //MessageBox.Show(DOR(Convert.ToDateTime(row["Date of Rotation"].ToString())).ToString());
                    }

                    //RadCalendarDay NewDay = new RadCalendarDay(HRCalendar);
                    //NewDay.Date = new DateTime(year, month, day);
                    //NewDay.Repeatable = RecurringEvents.DayAndMonth;
                    //NewDay.ToolTip = "Your ToolTip Text";
                    //NewDay.ItemStyle.CssClass = "day-style";
                    //HRCalendar.SpecialDays.Add(NewDay);

                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection"+ex.ToString());
            }
        }

        public DataTable GetDataTable()
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = new MySqlCommand("SELECT * FROM Employee", _Conn);

            DataTable table = new DataTable();

            _Conn.Open();
            try
            {
                adapter.Fill(table);
            }
            finally
            {
                _Conn.Close();
            }

            return table;
        }

        public DateTime DOR(DateTime _DOR)
        {
            if (_DOR.Date > DateTime.Now.Date)
            {
                outDate = _DOR;
            }
            else if (_DOR <= DateTime.Now.Date)
            {
              outDate=DOR(_DOR.AddYears(3));
            }
            return outDate;
        }
        private async void ROTATE_EMPLOYEE(string name,int id,string _DOR,string station)
        {
            if (station == "Ministry")
            {
                station = "Agency";
            }else if(station == "Agency")
            {
                station = "Referal Hospital";
            }else if(station== "Referal Hospital")
            {
                station = "Ministry";
            }
            try
            {

                // Open the Database Connection
                if (_Conn.State == ConnectionState.Closed)
                {
                    _Conn.Open();
                }
                else
                {
                    _Conn.Close();
                    _Conn.Open();
                }

                // Command String
                string _Update = @"UPDATE Employee SET `Current Station`='"+station+"',`Date of Rotation`='" + _DOR+ "' WHERE Id='" + id.ToString() + "'";

                // Initialize the command query and connection
                MySqlCommand _cmd = new MySqlCommand(_Update, _Conn);

                // Execute the command
                if (_cmd.ExecuteNonQuery() > 0)
                {
                    await this.ShowMessageAsync("Success", name+" moved to "+station);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Failed to rotate "+name+" !!");
                }
                _Conn.Close();

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection");
            }
        }

        public async void RETIRE_EMPLOYEE(string name, int Id)
        {
            try
            {

                // Open the Database Connection
                if (_Conn.State == ConnectionState.Closed)
                {
                    _Conn.Open();
                }
                else
                {
                    _Conn.Close();
                    _Conn.Open();
                }

                // Command String
                string _Update = @"UPDATE Employee SET Status='Retired' WHERE Id='" + Id.ToString() + "'";

                // Initialize the command query and connection
                MySqlCommand _cmd = new MySqlCommand(_Update, _Conn);

                // Execute the command
                if (_cmd.ExecuteNonQuery() > 0)
                {
                    await this.ShowMessageAsync("Success", name + " has successfully retired");
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Failed to retire " + name + " !!");
                }
                _Conn.Close();

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection");
            }
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
    }
}
