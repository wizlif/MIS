using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MIS
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Report : MetroWindow
    {
        public static string cs = "Server=localhost;Database=MIS;Uid=mis;Pwd=isaacobella;";
        MySqlConnection _Conn = new MySqlConnection(cs);
        MySqlCommand _cmd;
        public Report()
        {
            InitializeComponent();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            (new Settings()).ShowDialog();
        }

        private void Reports(FixedDocument fd,string Query,string Header)
        {
            // MySQL Statement
            string _Query = @"SELECT COUNT(*) FROM Employee";
            int stop = 0, start = 0;
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
            int rows = Convert.ToInt32(_cmd.ExecuteScalar());
            if (rows > 28)
            {
                start = 0;
                stop = 28;
                for (int i = 0; i <= Math.Ceiling(Convert.ToDecimal(rows / 28)); i++)
                {

                    fd.Pages.Add(Page(Query+" LIMIT " + start + "," + stop,Header));
                    if ((rows - stop) > 28)
                    {
                        start = stop + 1;
                        stop += 28;
                    }
                    else
                    {
                        start = stop + 1;
                        stop = rows;
                    }
                }
            }
            else
            {
                //add the PageContent to the FixedDocument
                fd.Pages.Add(Page(Query,Header));
            }
        }
        private PageContent Page(string query,string header)
        {
            //Declare fixed document
            PageContent pc = new PageContent();
            FixedPage fp = new FixedPage();
            fp.RenderTransformOrigin = new Point(0.5, 0.5);
            fp.RenderTransform = new RotateTransform(90);

            //Declare Main Dockanel
            DockPanel dp = new DockPanel();
            dp.Margin = new Thickness(-100, 150, 0, 0);
            dp.LastChildFill = true;

            ///
            /// Begining of Header
            ///  

            //Header Dockanel
            DockPanel headerDp = new DockPanel();
            DockPanel.SetDock(headerDp, Dock.Top);

            //Header DockPanel Background
            LinearGradientBrush bg = new LinearGradientBrush();
            bg.EndPoint = new Point(0.5, 1);
            bg.StartPoint = new Point(0.5, 0);

            //Bg Scale Transform
            ScaleTransform st = new ScaleTransform();
            st.CenterX = 0.5;
            st.CenterY = 0.5;

            //Bg Skew Transform
            SkewTransform sk = new SkewTransform();
            sk.CenterX = 0.5;
            sk.CenterY = 0.5;

            //Bg Rotate Transform
            RotateTransform rt = new RotateTransform();
            rt.CenterX = 0.5;
            rt.CenterY = 0.5;
            rt.Angle = 90;

            //Transform Group
            TransformGroup tg = new TransformGroup();
            tg.Children.Add(st);
            tg.Children.Add(sk);
            tg.Children.Add(rt);

            bg.RelativeTransform = tg;
            //Gradient Stops
            GradientStop one = new GradientStop();
            one.Color = (Color)ColorConverter.ConvertFromString("#FF005DFF");
            one.Offset = 0;

            GradientStop two = new GradientStop();
            two.Color = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
            two.Offset = 1;

            bg.GradientStops.Add(one);
            bg.GradientStops.Add(two);

            headerDp.Background = bg;

            ///Header Content
            Label left = new Label();
            DockPanel.SetDock(left, Dock.Left);
            left.VerticalAlignment = VerticalAlignment.Center;
            left.FontFamily = new FontFamily("Arial Black");
            left.FontStyle = FontStyles.Italic;
            left.FontSize = 20;
            left.Content = header;

            Label right = new Label();
            DockPanel.SetDock(right, Dock.Right);
            right.VerticalAlignment = VerticalAlignment.Center;
            right.FontFamily = new FontFamily("Arial Black");
            right.Foreground = Brushes.White;
            right.FontStyle = FontStyles.Italic;
            right.FontSize = 16;
            right.HorizontalAlignment = HorizontalAlignment.Right;
            right.Content = "MoFPED Human Resource Department";

            headerDp.Children.Add(left);
            headerDp.Children.Add(right);

            ///
            /// End of Header
            ///  

            ///
            ///Main DataGrid Dockanel
            /// 
            DockPanel dgdp = new DockPanel();

            DataGrid dg = new DataGrid();
            dg.Width = 1000;
            dg.AutoGeneratingColumn += DG_AutoGeneratingColumn;


            //Populate DataGrid
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=MIS;Uid=mis;Pwd=isaacobella;Allow Zero Datetime=True;Convert Zero Datetime=True"))
            {
                conn.Open();
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, conn))
                    da.Fill(dt);
            }
            dg.ItemsSource = dt.DefaultView;
            dgdp.Children.Add(dg);




            //Dock Children into main dockanel
            dp.Children.Add(headerDp);
            dp.Children.Add(dgdp);
            //add the main Dockanel to the FixedPage
            fp.Children.Add(dp);
            //add the FixedPage to the PageContent 
            pc.Child = fp;

            return pc;
        }

        private void DG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
                e.Column.Width = 100;
            
        }

        private void WindowSignUp_Loaded(object sender, RoutedEventArgs e)
        {
            Reports(FdStaffList, "SELECT CONCAT(Surname,' ',Othername) AS `Name`,`Date of Birth` AS DOB,`Date of Joining`,`Current Station` AS `Current Deployment`,`Type of Station` AS `Station Type`,`Date of Rotation`,Profession,Phone,Email,TIN FROM Employee", "Staff List");
            Reports(FdRotationList, "SELECT CONCAT(Surname,' ',Othername) AS `Name`,`Current Station`,`Type of Station`,`Next Station`,`Date of Rotation`,`Next Date of Rotation`,`Next Station`,`Month's To Rotation` FROM Employee WHERE `Month's To Rotation` <= 6 AND `Month's To Retirement` > 6", "Rotation List");

        }
    }
}
