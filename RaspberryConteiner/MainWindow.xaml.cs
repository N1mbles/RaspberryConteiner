using System;
using System.Windows;
using System.Windows.Input;
using RaspberryConteiner.CardControl;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace RaspberryConteiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // Status Dashboard
        private bool isDevices = false;
        private bool isUsers = false;
        public string CcurrentUser { get; set; }

        //Table with statistics
        System.Data.DataTable stats;


        public MainWindow()
        {
            InitializeComponent();

            //
            ListDevice.Visibility = Visibility.Visible;
            // Enable icon "Add"
            Add.Visibility = Visibility.Visible;
            isDevices = true;
            //Set default max temperature
            MaxTemp.Text = Parameters.MaxTemperature.ToString();
            //Set default delay
            Delay.Text = Parameters.Delay.ToString();
            // Init users
            InitUsers();
            // Init Devices
            InitDevices();

            ////Check Internet connection
            //if(!InternetConnection.CheckForInternetConnection())
            //{
            //    NotConnection notConnection = new NotConnection();
            //    notConnection.Show();
            //}
        }
        /// <summary>
        /// Initialization Users from Db on load window
        /// </summary>
        private void InitUsers()
        {
            // Show current user in system
            _CurrentUser.Text = Parameters.CurrentUser;

            using (MySqlConnection conn = new MySqlConnection(Parameters.connStr))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM tempmonitor2.Users;", conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            AddOneUser(rdr[1].ToString());
                        }
                        rdr.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }

        private void InitDevices()
        {
            using (MySqlConnection conn = new MySqlConnection(Parameters.connStr))
            {
                try
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM tempmonitor2.Devices;", conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            AddOneDevice(rdr[1].ToString(), rdr[2].ToString(), int.Parse(rdr[5].ToString())); ;
                        }
                        rdr.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Test");
        }
        /// <summary>
        /// Users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            // Show icon Add
            Add.Visibility = Visibility.Visible;
            //
            isDevices = false;
            isUsers = true;
            //Hide settings
            Settingss.Visibility = Visibility.Hidden;
            //Visible panel of users
            ListDevice.Visibility = Visibility.Hidden;
            ListOfUsers.Visibility = Visibility.Visible;
            // AddOneUser();
        }

        /// <summary>
        /// Devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            //
            Add.Visibility = Visibility.Visible;
            //
            isUsers = false;
            isDevices = true;
            //
            Settingss.Visibility = Visibility.Hidden;
            //
            ListDevice.Visibility = Visibility.Visible;
            ListOfUsers.Visibility = Visibility.Hidden;

            // AddOneDevice();
        }

        private void Quit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Exit exit = new Exit();
            exit.Show();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_4(object sender, MouseButtonEventArgs e)
        {
            if (isDevices)
            {
                ListDevice.Visibility = Visibility.Hidden;
                AddNewItem.Visibility = Visibility.Visible;
            }

            if (isUsers)
            {
                ListOfUsers.Visibility = Visibility.Hidden;
                AddNewUser.Visibility = Visibility.Visible;
            }

        }
        /// <summary>
        /// Exit from dashboard Key "Esc"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Exit exit = new Exit();
                exit.Show();
            }
        }
        public string NameDevice
        {
            get { return NameOfDevice.Text; }
            set { NameOfDevice.Text = value; }
        }

        /// <summary>
        /// Add new device button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NameDevice.Length != 0)
            {
                if (NumberPlatform.Text.Length != 0)
                {
                    using (MySqlConnection conn = new MySqlConnection(Parameters.connStr))
                    {
                        conn.OpenAsync();
                        if (Helpers.MySqlHelper.CheckExists(conn, "SELECT COUNT(*) FROM `tempmonitor2`.`Devices` WHERE Name = '" + NameDevice + "';"))
                        {
                            System.Windows.Forms.MessageBox.Show("This name already exists in the database.", "Error server", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        else
                        {
                            try
                            {
                                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO tempmonitor2.Devices (Name, NPlatform) VALUES (@NameDevice,@NumberPlatform); ", conn))
                                {
                                    cmd.Parameters.AddWithValue("@NameDevice", NameDevice);
                                    cmd.Parameters.AddWithValue("@NumberPlatform", NumberPlatform.Text);
                                    cmd.ExecuteNonQueryAsync();

                                    AddOneDevice(NameDevice, NumberPlatform.Text);

                                    System.Windows.Forms.MessageBox.Show("New Device Added!", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                                    //Hide from
                                    AddNewItem.Visibility = Visibility.Hidden;
                                    ListDevice.Visibility = Visibility.Visible;

                                    // null string 
                                    NameOfDevice.Text = string.Empty;
                                    NumberPlatform.Text = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(ex.Message, "Error server", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("The Number Platform is empty", "Error adding new device", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The Name is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddNewItem.Visibility = Visibility.Hidden;
            ListDevice.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Add new User button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (NameOfUser.Text.Length != 0)
            {
                using (MySqlConnection conn = new MySqlConnection(Parameters.connStr))
                {
                    conn.Open();

                    if (Helpers.MySqlHelper.CheckExists(conn, "SELECT COUNT(*) FROM `tempmonitor2`.`Users` WHERE Name = '" + NameOfUser.Text + "';"))
                    {
                        System.Windows.Forms.MessageBox.Show("This name already exists in the database.", "Error server", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    else
                    {
                        try
                        {
                            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO tempmonitor2.Users (Name) VALUES (@User);", conn))
                            {
                                cmd.Parameters.AddWithValue("@User", NameOfUser.Text);
                                cmd.ExecuteNonQueryAsync();

                                AddOneUser(NameOfUser.Text);

                                AddNewUser.Visibility = Visibility.Hidden;
                                ListOfUsers.Visibility = Visibility.Visible;
                                System.Windows.Forms.MessageBox.Show("New User Added!", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                                NameOfUser.Text = string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The Name is empty", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Cancel adding user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AddNewUser.Visibility = Visibility.Hidden;
            ListOfUsers.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Show settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Add.Visibility = Visibility.Hidden;

            Settingss.Visibility = Visibility.Visible;

            ListOfUsers.Visibility = Visibility.Hidden;
            ListDevice.Visibility = Visibility.Hidden;

            AddNewItem.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Hidden;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(CcurrentUser))
            {
                Parameters.CurrentUser = CcurrentUser;
            }
        }
        #region Methods
        /// <summary>
        /// Add new users
        /// </summary>
        /// <param name="count"> Count of users</param>
        private void AddOneUser(string name)
        {
            UserCard users = new UserCard
            {
                Width = Parameters.UserWidth,
                Height = Parameters.UserHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 20, 0, 0),
                NameofUser = name
            };
            CardsUsers.Children.Add(users); // Add to the stack panel.
                                            //  SaveAddedUser(name);
        }

        /// <summary>
        /// Add new devices
        /// </summary>
        /// <param name="count"> Count of users</param>
        private void AddOneDevice(string nameOfDevice, string nameOfPlatform, int maxTemp = 80)
        {
            Device device = new Device
            {
                Width = Convert.ToInt32("400"),
                Height = Convert.ToInt32("180"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(11, 20, 0, 0),
                NameofDevice = nameOfDevice,
                Nplatform = nameOfPlatform,
                MaxTemp = maxTemp
            };
            Cards.Children.Add(device); // Add to the stack panel.            
        }
        #endregion

        #region Input only numeric method
        void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }
        bool IsGood(char c)
        {
            if (c >= '0' && c <= '9')
                return true;
            return false;
        }
        #endregion

        /// <summary>
        /// Save default settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

            if (MaxTemp.Text != null)
            {
                if (Delay.Text != null)
                {
                    Parameters.MaxTemperature = int.Parse(MaxTemp.Text);
                    Parameters.Delay = int.Parse(Delay.Text);
                    System.Windows.Forms.MessageBox.Show("Settings saved!");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("\"Delay\" сannot be empty", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("\"Max temperature\" сannot be empty", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }


        //Dashboard
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //
            Add.Visibility = Visibility.Visible;
            //
            isUsers = false;
            isDevices = true;
            //
            Settingss.Visibility = Visibility.Hidden;
            //
            ListDevice.Visibility = Visibility.Visible;
            ListOfUsers.Visibility = Visibility.Hidden;
            //
            Statistics.Visibility = Visibility.Hidden;
        }
        // Users
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            // Show icon Add
            Add.Visibility = Visibility.Visible;
            //
            isDevices = false;
            isUsers = true;
            //Hide settings
            Settingss.Visibility = Visibility.Hidden;
            //Visible panel of users
            ListDevice.Visibility = Visibility.Hidden;
            ListOfUsers.Visibility = Visibility.Visible;
            //
            Statistics.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            //Show statistics
            Statistics.Visibility = Visibility.Visible;

            Add.Visibility = Visibility.Hidden;

            ListOfUsers.Visibility = Visibility.Hidden;
            ListDevice.Visibility = Visibility.Hidden;

            Settingss.Visibility = Visibility.Hidden;

            AddNewItem.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Hidden;

            GetStats();
        }
        // Show Settings
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            Add.Visibility = Visibility.Hidden;

            Settingss.Visibility = Visibility.Visible;

            ListOfUsers.Visibility = Visibility.Hidden;
            ListDevice.Visibility = Visibility.Hidden;

            AddNewItem.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Hidden;

            Statistics.Visibility = Visibility.Hidden;
        }

        // Shutdown
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            Exit exit = new Exit();
            exit.Show();
        }
        /// <summary>
        /// Button "refresh" in form statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            cbxInterval.Text = string.Empty;
            GetStats();
        }
        private void GetStats(string option = null)
        {
            using (MySqlConnection conn = new MySqlConnection(Parameters.connStr))
            {
                try
                {
                    conn.OpenAsync();
                    string sql = "SELECT * FROM tempmonitor2.Statistics WHERE Enrollment = 1;";

                    if (option == "Last month")
                    {
                        sql = "SELECT * FROM tempmonitor2.Statistics  WHERE DataEnd BETWEEN CURDATE()-INTERVAL 1 MONTH AND CURDATE();";
                    }
                    if (option == "Last week")
                    {
                        sql = "SELECT * FROM tempmonitor2.Statistics  WHERE DataEnd BETWEEN CURDATE()-INTERVAL 1 WEEK AND CURDATE();";
                    }
                    if (option == "Last 3 month")
                    {
                        sql = "SELECT * FROM tempmonitor2.Statistics  WHERE DataEnd BETWEEN CURDATE()-INTERVAL 3 MONTH AND CURDATE();";
                    }
                    if (option == "Half year")
                    {
                        sql = "SELECT * FROM tempmonitor2.Statistics  WHERE DataEnd BETWEEN CURDATE()-INTERVAL 6 MONTH AND CURDATE();";
                    }
                    if (option == "Last Year")
                    {
                        sql = "SELECT * FROM tempmonitor2.Statistics  WHERE DataEnd BETWEEN CURDATE()-INTERVAL 1 YEAR AND CURDATE();";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);

                        stats = new System.Data.DataTable("Statistics");

                        sda.FillAsync(stats);
                        grdStats.ItemsSource = stats.DefaultView;

                        //Show stats
                        InfoBest(conn);
                        completedTasks.Content = "Number of completed tasks: " + stats.DefaultView.Count.ToString();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }

        private void InfoBest(MySqlConnection connection)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT UsersName FROM tempmonitor2.Statistics WHERE Enrollment = 1 GROUP BY UsersName ORDER BY COUNT(*) DESC LIMIT 1; ", connection))
            {
                //
                var queryResultName = cmd.ExecuteScalar();
                if (queryResultName != null)
                {
                    bestUser.Content = "Most completed tasks: " + queryResultName.ToString();
                }
                else
                {
                    bestUser.Content = "Most completed tasks: " + "Error"; ;
                }
                //
                cmd.CommandText = "SELECT COUNT(UsersName) FROM tempmonitor2.Statistics WHERE Enrollment = 1 GROUP BY UsersName ORDER BY COUNT(*) DESC LIMIT 1; ";
                var queryResultCount = cmd.ExecuteScalar();
                if (queryResultCount != null)
                {
                    bestUserCount.Content = "Count: " + queryResultCount.ToString();
                }
                else
                {
                    bestUserCount.Content = "Count: " + "Error";
                }
            }
        }

        /// <summary>
        /// Export data to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (stats != null)
            {
                SaveFileDialog openDlg = new SaveFileDialog();
                openDlg.FileName = "Statistics";
                openDlg.Filter = "Excel (.xls)|*.xls |Excel (.xlsx)|*.xlsx |All files (*.*)|*.*";
                openDlg.FilterIndex = 2;
                openDlg.RestoreDirectory = true;
                openDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (openDlg.ShowDialog() == true)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            workbook.Worksheets.Add(stats, "Statistics"); // Create new xlmsx document, Get data from DataTable
                            workbook.SaveAs(openDlg.FileName);
                        }
                        System.Windows.Forms.MessageBox.Show("You are successfully exported tour Data!", "Export data", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Error export data", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Statistics is empty", "Error export data", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void cbxInterval_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbxInterval.SelectedValue == null)
            {
                return;
            }
            var value = cbxInterval.SelectedValue.ToString();

            string result = value.Replace("System.Windows.Controls.ComboBoxItem: ", "");
            GetStats(result);

        }
        private void _CurrentUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Show icon Add
            Add.Visibility = Visibility.Visible;
            //
            isDevices = false;
            isUsers = true;
            //Hide settings
            Settingss.Visibility = Visibility.Hidden;
            //Visible panel of users
            ListDevice.Visibility = Visibility.Hidden;
            ListOfUsers.Visibility = Visibility.Visible;
            //
            Statistics.Visibility = Visibility.Hidden;
        }
    }
}

