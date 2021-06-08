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
    public partial class MainWindow
    {
        // Status Dashboard
        private bool _isDevices;
        private bool _isUsers;

        //Table with statistics
        private System.Data.DataTable _stats;

        public MainWindow()
        {
            InitializeComponent();

            //
            ListDevice.Visibility = Visibility.Visible;
            // Enable icon "Add"
            Add.Visibility = Visibility.Visible;
            _isDevices = true;
            //Set default max temperature
            MaxTemp.Text = Parameters.MaxTemperature.ToString();
            //Set default delay
            Delay.Text = Parameters.Delay.ToString();
        }
        private void InitUsers()
        {
            // Show current user in system
            CurrentUser.Content = Parameters.CurrentUser;

            using (var conn = new MySqlConnection(Parameters.ConnStr))
            {
                try
                {
                    conn.OpenAsync();
                    using (var cmd = new MySqlCommand("SELECT * FROM tempmonitor2.Users;", conn))
                    {
                        var rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            AddOneUser(rdr[1].ToString());
                        }
                        rdr.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }

        private void InitDevices()
        {
            using (var conn = new MySqlConnection(Parameters.ConnStr))
            {
                try
                {
                    conn.OpenAsync();
                    using (var cmd = new MySqlCommand("SELECT * FROM tempmonitor2.Devices;", conn))
                    {
                        var rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            var statusString = Convert.ToInt16(rdr[6].ToString()) == 0 ? "NOTCONNECTED" : "CONNECTED";

                            AddOneDevice(rdr[1].ToString(), rdr[2].ToString(), statusString, int.Parse(rdr[5].ToString()));
                        }
                        rdr.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Check Internet connection
            if (!InternetChecker.IsConnectedToInternet())
            {
                NotConnection.Visibility = Visibility.Visible;
            }
            else
            {
                // Init users
                InitUsers();
                // Init Devices
                InitDevices();
            }
        }
        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_4(object sender, MouseButtonEventArgs e)
        {
            if (_isDevices)
            {
                ListDevice.Visibility = Visibility.Hidden;
                AddNewDevice.Visibility = Visibility.Visible;
            }

            if (!_isUsers) return;
            ListOfUsers.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Exit from dashboard Key "Esc"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;
            var exit = new Exit();
            exit.Show();
        }
        public string NameDevice
        {
            get => NameOfDevice.Text;
            set => NameOfDevice.Text = value;
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
                    using (var conn = new MySqlConnection(Parameters.ConnStr))
                    {
                        conn.OpenAsync();
                        if (Helpers.MySqlHelper.CheckExists(conn, "SELECT COUNT(*) FROM `tempmonitor2`.`Devices` WHERE Name = '" + NameDevice + "';"))
                        {
                            System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_This_name_already_exists_in_the_database_, Properties.Resources.MainWindow_Error_server, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        else
                        {
                            try
                            {
                                using (var cmd = new MySqlCommand("INSERT INTO tempmonitor2.Devices (Name, NPlatform, MaxTemp, CurrentTemp) VALUES (@NameDevice,@NumberPlatform,@MaxTemp, @CurrentTemp);", conn))
                                {
                                    cmd.Parameters.AddWithValue("@NameDevice", NameDevice);
                                    cmd.Parameters.AddWithValue("@NumberPlatform", NumberPlatform.Text);
                                    cmd.Parameters.AddWithValue("@@CurrentTemp", 0);
                                    cmd.Parameters.AddWithValue("@MaxTemp", Parameters.MaxTemperature);
                                    cmd.ExecuteNonQueryAsync();

                                    AddOneDevice(NameDevice, NumberPlatform.Text);

                                    System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_Button_Click_New_Device_Added_, Properties.Resources.MainWindow_Information, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                                    //Hide from
                                    AddNewDevice.Visibility = Visibility.Hidden;
                                    ListDevice.Visibility = Visibility.Visible;

                                    // null string 
                                    NameOfDevice.Text = string.Empty;
                                    NumberPlatform.Text = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error_server, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_The_Number_Platform_is_empty, Properties.Resources.MainWindow_Error_adding_new_device, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_The_Name_is_empty, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddNewDevice.Visibility = Visibility.Hidden;
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
                using (var conn = new MySqlConnection(Parameters.ConnStr))
                {
                    conn.Open();

                    if (Helpers.MySqlHelper.CheckExists(conn, "SELECT COUNT(*) FROM `tempmonitor2`.`Users` WHERE Name = '" + NameOfUser.Text + "';"))
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_This_name_already_exists_in_the_database_, Properties.Resources.MainWindow_Error_server, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    else
                    {
                        try
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO tempmonitor2.Users (Name) VALUES (@User);", conn))
                            {
                                cmd.Parameters.AddWithValue("@User", NameOfUser.Text);
                                cmd.ExecuteNonQueryAsync();

                                AddOneUser(NameOfUser.Text);

                                AddNewUser.Visibility = Visibility.Hidden;
                                ListOfUsers.Visibility = Visibility.Visible;
                                System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_New_User_Added_, Properties.Resources.MainWindow_Information, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                                NameOfUser.Text = string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_The_Name_is_empty, Properties.Resources.MainWindow_Information, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
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

        #region Methods

        /// <summary>
        /// Add new users
        /// </summary>
        private void AddOneUser(string name)
        {
            var users = new UserCard
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
        private void AddOneDevice(string nameOfDevice, string nameOfPlatform, string statusDevice = "NOTCONNECTED", int maxTemp = 80)
        {
            var device = new Device
            {
                Width = Convert.ToInt32("400"),
                Height = Convert.ToInt32("180"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(11, 20, 0, 0),
                NameofDevice = nameOfDevice,
                Nplatform = nameOfPlatform,
                StatusDevice = statusDevice,
                MaxTemp = maxTemp
            };
            Cards.Children.Add(device); // Add to the stack panel.            
        }
        #endregion

        #region Input only numeric method
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }
        private static bool IsGood(char c)
        {
            return c >= '0' && c <= '9';
        }
        #endregion

        /// <summary>
        /// Save default settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Parameters.MaxTemperature = int.Parse(MaxTemp.Text);
            Parameters.Delay = int.Parse(Delay.Text);
            System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_Settings_saved_);
        }


        //Dashboard
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //
            Add.Visibility = Visibility.Visible;
            //
            _isUsers = false;
            _isDevices = true;
            //
            GrdSettings.Visibility = Visibility.Hidden;
            //
            ListDevice.Visibility = Visibility.Visible;
            ListOfUsers.Visibility = Visibility.Hidden;
            //
            Statistics.Visibility = Visibility.Hidden;
            // Hide Add new item
            AddNewDevice.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Hidden;
        }
        // Users
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            // Show icon Add
            Add.Visibility = Visibility.Visible;
            //
            _isDevices = false;
            _isUsers = true;
            //Hide settings
            GrdSettings.Visibility = Visibility.Hidden;
            //Visible panel of users
            ListDevice.Visibility = Visibility.Hidden;
            ListOfUsers.Visibility = Visibility.Visible;
            //
            Statistics.Visibility = Visibility.Hidden;
            // Hide Add new item
            AddNewDevice.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Hidden;
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
            // Hide button Add
            Add.Visibility = Visibility.Hidden;
            //Hide  ListDevice && Devices
            ListOfUsers.Visibility = Visibility.Hidden;
            ListDevice.Visibility = Visibility.Hidden;

            GrdSettings.Visibility = Visibility.Hidden;

            AddNewDevice.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Hidden;

            GetStats();
        }
        // Show Settings
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            Add.Visibility = Visibility.Hidden;

            GrdSettings.Visibility = Visibility.Visible; // Show

            ListOfUsers.Visibility = Visibility.Hidden;
            ListDevice.Visibility = Visibility.Hidden;

            // Hide Add new item
            AddNewDevice.Visibility = Visibility.Hidden;
            AddNewUser.Visibility = Visibility.Hidden;

            Statistics.Visibility = Visibility.Hidden;
        }

        // Shutdown
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            var exit = new Exit();
            exit.Show();
        }
        /// <summary>
        /// Button "refresh" in form statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            CbxInterval.Text = string.Empty;
            GetStats();
        }
        private void GetStats(string option = null)
        {
            using (var conn = new MySqlConnection(Parameters.ConnStr))
            {
                try
                {
                    conn.OpenAsync();
                    var sql = "SELECT * FROM tempmonitor2.Statistics WHERE Enrollment = 1;";

                    switch (option)
                    {
                        case "Last month":
                            sql = "SELECT * FROM tempmonitor2.Statistics  WHERE Enrollment = 1 or DataEnd BETWEEN CURDATE()-INTERVAL 1 MONTH AND CURDATE();";
                            break;
                        case "Last week":
                            sql = "SELECT * FROM tempmonitor2.Statistics  WHERE Enrollment = 1 or DataEnd BETWEEN CURDATE()-INTERVAL 1 WEEK AND CURDATE();";
                            break;
                        case "Last 3 month":
                            sql = "SELECT * FROM tempmonitor2.Statistics  WHERE Enrollment = 1 or DataEnd BETWEEN CURDATE()-INTERVAL 3 MONTH AND CURDATE();";
                            break;
                        case "Half year":
                            sql = "SELECT * FROM tempmonitor2.Statistics  WHERE Enrollment = 1 or DataEnd BETWEEN CURDATE()-INTERVAL 6 MONTH AND CURDATE();";
                            break;
                        case "Last Year":
                            sql = "SELECT * FROM tempmonitor2.Statistics  WHERE Enrollment = 1 or DataEnd BETWEEN CURDATE()-INTERVAL 1 YEAR AND CURDATE();";
                            break;
                    }

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        var sda = new MySqlDataAdapter(cmd);

                        _stats = new System.Data.DataTable("Statistics");

                        sda.FillAsync(_stats);
                        GrdStats.ItemsSource = _stats.DefaultView;

                        //Show stats
                        InfoBest(conn);
                        CompletedTasks.Content = "Number of completed tasks: " + _stats.DefaultView.Count.ToString();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }

        private void InfoBest(MySqlConnection connection)
        {
            using (var cmd = new MySqlCommand("SELECT UsersName FROM tempmonitor2.Statistics WHERE Enrollment = 1 GROUP BY UsersName ORDER BY COUNT(*) DESC LIMIT 1; ", connection))
            {
                // Get name 
                var queryResultName = cmd.ExecuteScalar();
                if (queryResultName != null)
                {
                    BestUser.Content = "Most completed tasks: " + queryResultName;
                }
                else
                {
                    BestUser.Content = "Most completed tasks: " + Properties.Resources.MainWindow_Error;
                }
                // Get count of name 
                cmd.CommandText = "SELECT COUNT(UsersName) FROM tempmonitor2.Statistics WHERE Enrollment = 1 GROUP BY UsersName ORDER BY COUNT(*) DESC LIMIT 1; ";
                var queryResultCount = cmd.ExecuteScalar();
                if (queryResultCount != null)
                {
                    BestUserCount.Content = "Count: " + queryResultCount;
                }
                else
                {
                    BestUserCount.Content = "Count: " + Properties.Resources.MainWindow_Error;
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
            if (_stats != null)
            {
                var openDlg = new SaveFileDialog
                {
                    FileName = "Statistics",
                    Filter = "Excel (.xls)|*.xls |Excel (.xlsx)|*.xlsx |All files (*.*)|*.*",
                    FilterIndex = 2,
                    RestoreDirectory = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (openDlg.ShowDialog() == true)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            workbook.Worksheets.Add(_stats, "Statistics"); // Create new xlmsx document, Get data from DataTable
                            workbook.SaveAs(openDlg.FileName);
                        }
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_You_are_successfully_exported_tour_Data_, Properties.Resources.MainWindow_Export_data, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error_export_data, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.MainWindow_Statistics_is_empty, Properties.Resources.MainWindow_Error_export_data, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void cbxInterval_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CbxInterval.SelectedValue == null)
            {
                return;
            }
            var value = CbxInterval.SelectedValue.ToString();

            var result = value.Replace("System.Windows.Controls.ComboBoxItem: ", "");
            GetStats(result);

        }

        /// <summary>
        /// Try again reconnect to network
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (!InternetChecker.IsConnectedToInternet()) return;

            // Show wifi connected
            Wifi.Visibility = Visibility.Visible;
            Connected.Visibility = Visibility.Visible;
            // Init users
            InitUsers();
            // Init Devices
            InitDevices();

            NotConnection.Visibility = Visibility.Hidden;
        }

        private void CurrentUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Show icon Add
            Add.Visibility = Visibility.Visible;
            //
            _isDevices = false;
            _isUsers = true;
            //Hide settings
            GrdSettings.Visibility = Visibility.Hidden;
            //Visible panel of users
            ListDevice.Visibility = Visibility.Hidden;
            ListOfUsers.Visibility = Visibility.Visible;
            //
            Statistics.Visibility = Visibility.Hidden;
        }
    }
}