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
    /// Interaction logic for NewEmloyee.xaml
    /// </summary>
    public partial class NewEmployee : MetroWindow
    {
        public string acc;
        public int Id, op;
        DateTime outDate = DateTime.Now;
        public static string cs = "Server="+GetServerAddress()+";Database=MIS;Uid=mis;Pwd=isaacobella;";
        MySqlConnection _Conn = new MySqlConnection(cs);
        MySqlCommand _cmd;
        public NewEmployee()
        {
            InitializeComponent();
}

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            (new Settings()).ShowDialog();
        }

        private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
           

            try
            {
                if (op == 1)
                {
                    //Update Record
                    UPDATE_EMPLOYEE(Id);
                } else
                {
                    // MySQL Statement
                    string _Query = @"SELECT COUNT(*) FROM Employee WHERE Surname='" + TextBoxSurname.Text + "' AND Othername='"+TextBoxOthernames.Text+"'";
                    _cmd = new MySqlCommand(_Query, _Conn);


                    //Check If Account Exists
                     //Open the Database Connection
                    if (_Conn.State == ConnectionState.Closed)
                    {
                        _Conn.Open();
                    }
                    else
                    {
                        _Conn.Close();
                        _Conn.Open();
                    }
                    if (Convert.ToInt32(_cmd.ExecuteScalar()) > 0)
                    {
                        _Conn.Close();
                        await this.ShowMessageAsync("Error", "Employee Exists");
                    }
                    else {
                        /// Record Employee

                        NEW_EMPLOYEE(TextBoxSurname.Text,TextBoxOthernames.Text, Convert.ToDateTime(DatePickerDOB.ToString()),Nationality(),TextBoxReligion.Text,TextBoxOrigin.Text,MaritalStatus(),TextBoxResidence.Text,TextBoxPhone.Text,TextBoxEmail.Text,TextBoxTIN.Text, Convert.ToDateTime(DatePickerFA.ToString()), Convert.ToDateTime(DatePickerCA.ToString()), TextBoxCS.Text,TextBoxPost.Text, TextBoxTOS.Text, DOR(Convert.ToDateTime(DatePickerDOR.ToString())), Convert.ToDateTime(DatePickerDORt.ToString()), ComboBoxProf.Text,TextBoxEduc.Text,TextBoxName1.Text,TextBoxRelationship1.Text,TextBoxTel1.Text,TextBoxResd1.Text, TextBoxName2.Text, TextBoxRelationship2.Text, TextBoxTel2.Text, TextBoxResd2.Text);
                    }
                }
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(Admin))
                        {
                            (window as Admin).DataGridBinding("SELECT * FROM Employee", "Employees", (window as Admin).DataGridEmployees);
                        }
                    }
                    //TextBoxName.Text = "";
                    DatePickerDOB.Text = "";
                    //DatePickerDOJ.Text = "";
                    DatePickerDOR.Text = "";
                    DatePickerDORt.Text = "";
                    //TextBoxCH.Text = "";
                    TextBoxCS.Text = "";
                    TextBoxTOS.Text = "";
                    ComboBoxProf.Text = "";

                }catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection"+ex.ToString());

            }
        }

        public DateTime DOR(DateTime _DOR)
        {
            if (_DOR.Date > DateTime.Now.Date)
            {
                outDate = _DOR;
            }
            else if (_DOR <= DateTime.Now.Date)
            {
                outDate = DOR(_DOR.AddYears(3));
            }
            return outDate;
        }

        public string Nationality()
        {
            string nt="";
            if (RadioButtonNUgFemale.IsChecked == true)
            {
                nt = RadioButtonNUgFemale.Content.ToString();
            }
            else if (RadioButtonNUgMale.IsChecked == true)
            {
                nt = RadioButtonNUgMale.Content.ToString();
            }
            else if (RadioButtonUgFemale.IsChecked == true)
            {
                nt = RadioButtonUgFemale.Content.ToString();
            }
            else
            {
                nt = RadioButtonUgMale.Content.ToString();
            }
            return nt;
        }

        public string MaritalStatus()
        {
            string ms = "";
            if (RadioButtonWidowed.IsChecked == true)
            {
                ms = RadioButtonWidowed.Content.ToString();
            }else if (RadioButtonSingle.IsChecked == true) {
                ms = RadioButtonSingle.Content.ToString();

                }else if (RadioButtonSeparated.IsChecked == true)
            {
                ms = RadioButtonSeparated.Content.ToString();
            }else if (RadioButtonDivorced.IsChecked == true)
            {
                ms = RadioButtonDivorced.Content.ToString();
            }
            else
            {
                ms = RadioButtonMarried.Content.ToString();
            }
            return ms;
        }
        private async void UPDATE_EMPLOYEE(int id)
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
                //string _Update = @"UPDATE Employee SET Name='" + TextBoxName.Text + "',`Date of Birth`='" + Convert.ToDateTime(DatePickerDOB.ToString()).ToString("yyyy-MM-dd") + "',`Date of Joining`='" + Convert.ToDateTime(DatePickerDOJ.ToString()).ToString("yyyy-MM-dd") + "',`Current Station`='" + TextBoxCS.Text + "',`Type of Station`='" + TextBoxTOS.Text + "',`Date of Rotation`='" + DOR(Convert.ToDateTime(DatePickerDOR.ToString())).ToString("yyyy-MM-dd") + "',`Date of Retirement`='" + Convert.ToDateTime(DatePickerDORt.ToString()).ToString("yyyy-MM-dd") + "',`Conduct History`='" + TextBoxCH.Text + "',Profession='" + ComboBoxProf.Text + "',Status='Active',`Entered By`='"+acc+"' WHERE Id='" + Id.ToString() + "'";

                // Initialize the command query and connection
                //_cmd = new MySqlCommand(_Update, _Conn);

                // Execute the command
               if( _cmd.ExecuteNonQuery() > 0)
                {
                    await this.ShowMessageAsync("Success", "Account Updated !!");
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Failed to update account !!");
                }
                _Conn.Close();
                
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection");
            }
        }
        

        private void DatePickerDOJ_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePickerDOR.Text = DatePickerCA.DisplayDate.AddYears(3).ToString("MM/dd/yyyy");
        }

        

        public async void NEW_EMPLOYEE(string _SName, string _OName, DateTime _DateOfBirth, string _NationalitySex, string _Religion, string _HD, string _MaritalStatus, string _Residence, string _Phone, string _Email, string _TIN, DateTime _DateOfJoining, DateTime _DOCA, string _CurrentWorkPlace, string _PostTitle, string _TypeOfStation,DateTime _DateOfRotation,DateTime _DateOfRetirement,string _Profession,string _HQ, string _KinName1, string _KinRshp1, string _KinPhone1, string _KinResd1, string _KinName2, string _KinRshp2, string _KinPhone2, string _KinResd2)
        {

            // MySQL Statement
            string _Query = @"INSERT INTO Employee VALUES(NULL,'" + _SName + "','" + _OName + "','" + Convert.ToDateTime(_DateOfBirth.ToString()).ToString("yyyy-MM-dd") + "','" + _NationalitySex + "','" + _Religion + "','" + _HD + "','" + _MaritalStatus + "','" + _Residence + "','" + _Phone + "','" + _Email + "','" + _TIN + "','" + Convert.ToDateTime(_DateOfJoining.ToString()).ToString("yyyy-MM-dd") + "','" + Convert.ToDateTime(_DOCA.ToString()).ToString("yyyy-MM-dd") + "','" + _CurrentWorkPlace + "','" + _PostTitle + "','" + _TypeOfStation + "','" + Convert.ToDateTime(_DateOfRotation.ToString()).ToString("yyyy-MM-dd") + "','" + Convert.ToDateTime(_DateOfRetirement.ToString()).ToString("yyyy-MM-dd") + "', '" + _Profession + "','" + _HQ + "','" + _KinName1 + "','" + _KinRshp1 + "','" + _KinPhone1 + "','" + _KinResd1 + "','" + _KinName2 + "','" + _KinRshp2 + "','" + _KinPhone2 + "','" + _KinResd2 + "','Active','" + acc + "','" + Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "',DATE_ADD(`Date of Rotation`, INTERVAL 3 YEAR),'" + NextStation(_TypeOfStation) + "',TIMESTAMPDIFF(MONTH,NOW(),`Date of Rotation`),TIMESTAMPDIFF(MONTH,NOW(),`Date of Retirement`))";

            _cmd = new MySqlCommand(_Query, _Conn);
            //await this.ShowMessageAsync("Saved", _Query);
            // Open and Execute

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

                if (_cmd.ExecuteNonQuery() > 0)
                {
                    await this.ShowMessageAsync("Saved", "Employee recorded successfully");
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Error occured while recording employee");
                }

                _Conn.Close();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", "Server Errror check your server connection"+ex.ToString());
            }
        }
        public string NextStation(string station)
        {
            if (station == "Ministry")
            {
                station = "Agency";
            }
            else if (station == "Agency")
            {
                station = "Referal Hospital";
            }
            else if (station == "Referal Hospital")
            {
                station = "Ministry";
            }
            return station;
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
