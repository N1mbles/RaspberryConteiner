using System;
using System.Windows;
using System.Windows.Input;
using RaspberryConteiner.CardControl;
using System.Linq;
using MySql.Data.MySqlClient;

namespace RaspberryConteiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Status Dashboard
        private bool isDevices = false;
        private bool isUsers = false;
        public string CcurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //
            ListDevice.Visibility = Visibility.Visible;
            // Enable icon "Add"
            isDevices = true;
            // Set default Port
            PortAddres.Text = Parameters.Port.ToString();
            //Set default max temperature
            MaxTemp.Text = Parameters.MaxTemperature.ToString();
            //Set default delay
            Delay.Text = Parameters.Delay.ToString();
            // Init users
            InitUsers();
            // Init Devices
            InitDevices();
        }
        /// <summary>
        /// Initialization Users from Db on load window
        /// </summary>
        private void InitUsers()
        {
            // Show current user in system
            _CurrentUser.Text = Parameters.CurrentUser;

            MySqlConnection conn = new MySqlConnection(Parameters.connStr);
            try
            {
                conn.Open();

                string sql = "SELECT * FROM tempmonitor.Users;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AddOneUser(rdr[1].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }

            conn.Close();
        }

        private void InitDevices()
        {
            MySqlConnection conn = new MySqlConnection(Parameters.connStr);
            try
            {
                conn.Open();

                string sql = "SELECT * FROM tempmonitor.Devices;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AddOneDevice(rdr[1].ToString(),rdr[2].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }

            conn.Close();
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
                Ip.Text = "192.168.0.3";
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
        public string IpAddres
        {
            get { return Ip.Text; }
            set { Ip.Text = value; }
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
                    if (IpAddres.Length != 0)
                    {
                        Device device = new Device
                        {
                            Width = Convert.ToInt32("400"),
                            Height = Convert.ToInt32("180"),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(10, 18, 0, 0),
                            NameofDevice = NameDevice,
                            Nplatform = NumberPlatform.Text,
                            Url = Ip.Text
                        };
                        Cards.Children.Add(device); // Add to the stack panel.

                        System.Windows.Forms.MessageBox.Show("New Device Added!", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        AddNewItem.Visibility = Visibility.Hidden;
                        ListDevice.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("The Ip  is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("The Number Platform is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The Name is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }

            // null string 
            NameOfDevice.Text = string.Empty;
            Ip.Text = string.Empty;
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
                MySqlConnection conn = new MySqlConnection(Parameters.connStr);

                try
                {
                    conn.Open();

                    var sql = "INSERT INTO `tempmonitor`.`Users` (`Name`) VALUES ('" + NameOfUser.Text + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    AddOneUser(NameOfUser.Text);

                    AddNewUser.Visibility = Visibility.Hidden;
                    ListOfUsers.Visibility = Visibility.Visible;
                    System.Windows.Forms.MessageBox.Show("New User Added!", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }

                conn.Close();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The Name is empty", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }

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
        private void AddOneDevice(string nameOfDevice, string nameOfPlatform)
        {
            Device device = new Device
            {
                Width = Convert.ToInt32("400"),
                Height = Convert.ToInt32("180"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(11, 20, 0, 0),
                NameofDevice = nameOfDevice,
                Nplatform = nameOfPlatform
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
            if (PortAddres.Text != null)
            {
                if (MaxTemp.Text != null)
                {
                    if (Delay.Text != null)
                    {
                        Parameters.Port = int.Parse(PortAddres.Text);
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
            else
            {
                System.Windows.Forms.MessageBox.Show("\"Port\" сannot be empty", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// Export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Excel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Device device = new Device();
            device.ExportDataToExcel();
            System.Windows.Forms.MessageBox.Show("Success");
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
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {

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
        }

        // Shutdown
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            Exit exit = new Exit();
            exit.Show();
        }
    }
}
