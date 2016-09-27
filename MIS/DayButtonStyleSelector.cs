using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Calendar;

namespace MIS
{
    public class DayButtonStyleSelector : StyleSelector
    {
        public static string cs = "Server="+GetServerAddress()+";Database=MIS;Uid=mis;Pwd=isaacobella;";
        MySqlConnection _Conn = new MySqlConnection(cs);
        public Style SpecialStyleRetire { get; set; }
        public Style SpecialStyleRotate { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            // Retirement Style
            CalendarButtonContent content = item as CalendarButtonContent;
            List<DateTime> array = new List<DateTime>();
            List<String> names = new List<string>();
            try
            {
                DataTable table2 = GetDataTable();
                foreach (DataRow row in table2.Rows)
                {
                    if (row["Status"].ToString() != "Retired")
                    {
                        array.Add(Convert.ToDateTime(row["Date of Retirement"].ToString()));
                        names.Add(row["Name"].ToString());
                    }
                }
            }
            catch (Exception) { }
            Control control = container as Control;
            if (content != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    if (content.Date == array[i] && content.ButtonType == CalendarButtonType.Date)
                    {
                        control.ToolTip = new ToolTip() { Content = names[i] + " Retirement" };
                        return SpecialStyleRetire;
                    }
                }

            }

            // RotationStyle
            CalendarButtonContent content2 = item as CalendarButtonContent;
            List<DateTime> array2 = new List<DateTime>();
            List<String> names2 = new List<string>();
            try
            {
                DataTable table2 = GetDataTable();
                foreach (DataRow row1 in table2.Rows)
                {
                    if (row1["Status"].ToString() != "Retired")
                    {
                        array2.Add(Convert.ToDateTime(row1["Date of Rotation"].ToString()));
                        names2.Add(row1["Name"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Error",ex.ToString());
            }
            Control control2 = container as Control;
            if (content2 != null)
            {
                for (int i = 0; i < array2.Count; i++)
                {
                    if (content2.Date == array2[i] && content2.ButtonType == CalendarButtonType.Date)
                    {
                        control2.ToolTip = new ToolTip() { Content = names2[i] + " Rotation" };
                        return SpecialStyleRotate;
                    }
                }

            }


            return base.SelectStyle(item, container);
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
