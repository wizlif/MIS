using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MIS
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : MetroWindow
    {
        public static string cs = "Server="+GetServerAddress()+";Database=MIS;Uid=mis;Pwd=isaacobella;";
        MySqlConnection _Conn = new MySqlConnection(cs);
        public string acc;
        //MySqlCommand _cmd;
        public Admin()
        {
            InitializeComponent();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            (new Settings()).ShowDialog();
        }

        public async void DataGridBinding(String query, String binding, DataGrid datagrid)
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

                MySqlDataAdapter _Adapter = new MySqlDataAdapter(query, _Conn);

                DataSet _Bind = new DataSet();
                _Adapter.Fill(_Bind, binding);


                datagrid.DataContext = _Bind;

                // Close the Database Connection
                _Conn.Close();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection" + ex.ToString());
            }
        }

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridBinding("SELECT * FROM Employee", "Employees", DataGridEmployees);
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataGridBinding("SELECT * FROM Employee WHERE Name LIKE '%" + TextBoxSearch.Text + "%' OR `Date of Birth` LIKE '%" + TextBoxSearch.Text + "%' OR `Date of Joining` LIKE '%" + TextBoxSearch.Text + "%' OR `Current Station` LIKE '%" + TextBoxSearch.Text + "%' OR `Type of Station` LIKE '%" + TextBoxSearch.Text + "%' OR `Date of Rotation` LIKE '%" + TextBoxSearch.Text + "%' OR `Date of Retirement` LIKE '%" + TextBoxSearch.Text + "%' OR `Conduct History` LIKE '%" + TextBoxSearch.Text + "%' OR `Profession` LIKE '%" + TextBoxSearch.Text + "%' OR `Entered By` LIKE '%" + TextBoxSearch.Text + "%' OR `Entered When` LIKE '%" + TextBoxSearch.Text + "%'", "Employees", DataGridEmployees);
        }

        private void ButtonNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            (new NewEmployee()).ShowDialog();
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            (new Report()).ShowDialog();
        }

        private async void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridEmployees.SelectedValue != null)
            {
                DataRowView drv = (DataRowView)DataGridEmployees.SelectedItem;
                String res = (drv["Name"]).ToString();
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
                    string _Select = @"SELECT * FROM Employee WHERE Name='" + res + "'";

                    // Initialize the command query and connection
                    MySqlCommand _cmd = new MySqlCommand(_Select, _Conn);
                    MySqlDataReader _reader = _cmd.ExecuteReader();
                    NewEmployee ne = new NewEmployee();
                    while (_reader.Read())
                    {
                        //ne.TextBoxName.Text = _reader["Name"].ToString();
                        ne.DatePickerDOB.Text =Convert.ToDateTime(_reader["Date of Birth"].ToString()).ToString("MM/dd/yyyy");
                        //ne.DatePickerDOJ.Text =Convert.ToDateTime(_reader["Date of Joining"].ToString()).ToString("MM/dd/yyyy");
                        ne.DatePickerDOR.Text =Convert.ToDateTime(_reader["Date of Rotation"].ToString()).ToString("MM/dd/yyyy");
                        ne.DatePickerDORt.Text =Convert.ToDateTime(_reader["Date of Retirement"].ToString()).ToString("MM/dd/yyyy");
                        //ne.TextBoxCH.Text = _reader["Conduct History"].ToString();
                        ne.TextBoxCS.Text = _reader["Current Station"].ToString();
                        ne.TextBoxTOS.Text = _reader["Type of Station"].ToString();
                        ne.ComboBoxProf.Text = _reader["Profession"].ToString();
                        ne.Id = Convert.ToInt32(_reader["Id"].ToString());
                    }
                    ne.op = 1;
                    ne.acc = acc;
                    ne.ShowDialog();
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }
            }
            else
            {
                await this.ShowMessageAsync("Info", "Please select an item from the table below to edit");
            }
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            

            if (DataGridEmployees.SelectedItem != null)
            {
                
                DataRowView drv = (DataRowView)DataGridEmployees.SelectedItem;
                String res = (drv["Name"]).ToString();
                var DialogSettings = new MetroDialogSettings() { AffirmativeButtonText = "Yes", NegativeButtonText = "No" };
                MessageDialogResult result = await this.ShowMessageAsync("Warning", "Are you sure you want to delete employee " + res,MessageDialogStyle.AffirmativeAndNegative,DialogSettings);
                if (result == MessageDialogResult.Affirmative)
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
                        string _DelCmd = @"DELETE FROM Employee WHERE Name='" + res + "'";

                        // Initialize the command query and connection
                        MySqlCommand _CmdDelete = new MySqlCommand(_DelCmd, _Conn);
                        // Execute the command
                        if (_CmdDelete.ExecuteNonQuery() > 0)
                        {
                            await this.ShowMessageAsync("Warning", "Account Deleted");
                            DataGridBinding("SELECT * FROM Employee", "Employees", DataGridEmployees);
                        }
                        else
                        {
                            await this.ShowMessageAsync("Error", "Failed to delete account");
                        }
                    }
                    catch (Exception)
                    {
                        await this.ShowMessageAsync("Error", "Server Errror check your server connection");
                    }
                }
            }
            else
            {
                await this.ShowMessageAsync("Info", "Please select an item from the table below to delete");
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
