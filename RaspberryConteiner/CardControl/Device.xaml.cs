using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RaspberryConteiner.CardControl
{
    public partial class Device
    {
        public Device()
        {
            InitializeComponent();
            // Set default temperature
            _iMaxTemp = Properties.Settings.Default.DefMaxTemp;
            SetTemp.Text = _iMaxTemp.ToString();
        }

        private DispatcherTimer _liveTime;
        private Stopwatch _stopwatch, _stopwatchRecover;


        private readonly List<Statistics> _statistics = new List<Statistics>();

        #region Variable
        //UP TIME variable
        private bool _isCompleted = false;

        //One reset
        private bool _clickReset = true;

        // Only one click for start
        private bool _clickOne = true;

        private bool _isWorking = true;
        private int _iMaxTemp;
        public string NameofDevice
        {
            get => NameDevice.Text;
            set => NameDevice.Text = value;
        }
        public double CurrentTemperature
        {
            get => double.Parse((string)LocalTemp.Content);
            set => LocalTemp.Content = value;
        }
        public int MaxTemp
        {
            get => _iMaxTemp;
            set
            {
                _iMaxTemp = value;
                SetTemp.Text = value.ToString();
            }
        }

        public string Nplatform
        {
            get => (string)NumberDevice.Content;
            set => NumberDevice.Content = value;
        }

        public string StatusDevice
        {
            set => SetStatusDevice(value);
        }

        // Status connecting
        //private bool statusConnect = true;
        //
        private bool _initTemperature = true;
        #endregion

        /// <summary>
        /// Remove current device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetBackBlur();
            //Show modal window
            ConfirmationRemoved.Visibility = Visibility.Visible;
        }
        private void SetBackBlur()
        {
            // Add background blur effect
            var blurEffect = new System.Windows.Media.Effects.BlurEffect
            {
                KernelType = System.Windows.Media.Effects.KernelType.Gaussian,
                Radius = 7
            };
            BlurCard.Effect = blurEffect;
        }

        /// <summary>
        /// Starting getting 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_clickOne) return;
            _clickOne = false;
            //
            _isWorking = true;
            StartGettingTemp();
        }
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_clickOne) return;
            _clickOne = false;
            //
            _isWorking = true;
            StartGettingTemp();
        }
        private void Rectangle_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            SetTemp.Focus();
        }
        #region Input only numeric
        void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private static bool IsGood(char c)
        {
            return c >= '0' && c <= '9';
        }
        #endregion
        private void SetTemp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SetTemp.Text.Length == 0)
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.Device_SetTemp_The_field_is_empty, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                using (var conn = new MySqlConnection(Parameters.ConnStr))
                {
                    try
                    {
                        conn.OpenAsync();
                        using (var cmd = new MySqlCommand("UPDATE tempmonitor2.Devices SET MaxTemp =@MaxTemp WHERE NPlatform = @NPlatform ;", conn))
                        {
                            cmd.Parameters.AddWithValue("@MaxTemp", int.Parse(SetTemp.Text));
                            cmd.Parameters.AddWithValue("@NPlatform", Nplatform);
                            cmd.ExecuteNonQueryAsync();
                            //Save installed temperature
                            MaxTemp = int.Parse(SetTemp.Text);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error,
                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                }
                HintMax.Visibility = Visibility.Hidden;
            }
        }

        private async void StartGettingTemp()
        {
            if (SetTemp.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.Device_StartGettingTemp_Set_max_temperature, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                _stopwatch = new Stopwatch();
                _liveTime = new DispatcherTimer(DispatcherPriority.Render) { Interval = new TimeSpan(0, 0, 1) };
                _liveTime.Tick += Ticks;

                if (_stopwatchRecover != null)
                {
                    _stopwatch = _stopwatchRecover;
                    _stopwatchRecover = null;

                    _stopwatch.Start();
                    _liveTime.Start();
                }
                else
                {
                    _stopwatch.Start();
                    _liveTime.Start();
                }
                
                // loop running program
                while (_isWorking)
                {
                    using (var conn = new MySqlConnection(Parameters.ConnStr))
                    {
                        await conn.OpenAsync();
                        try
                        {
                            using (var cmd = new MySqlCommand("SELECT * FROM tempmonitor2.Devices WHERE Name = @Name AND NPlatform = @NPlatform; ", conn))
                            {
                                cmd.Parameters.AddWithValue("@Name", NameofDevice);
                                cmd.Parameters.AddWithValue("@Nplatform", Nplatform);

                                var rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                                //
                                _clickReset = true;

                                while (await rdr.ReadAsync())
                                {
                                    // if device is off or he doesnt have connection to internet || value"CurrentTemperature = 0" 
                                    if (Convert.ToInt16(rdr[4].ToString()) == 0 || Convert.ToInt16(rdr[6].ToString()) == 0)
                                    {
                                        SetStatusDevice("NOTCONNECTED");

                                        //show DialogWindow
                                        SetBackBlur();
                                        ErrorCon.Visibility = Visibility.Visible;
                                        Titlee.Text = "Error connection, check your device. №" +  rdr[2];

                                        // Play a sound as a notification.
                                        SystemSounds.Beep.Play();

                                        _liveTime.Stop();
                                        _stopwatch.Stop();

                                        _initTemperature = true;
                                        _isWorking = false; // stop getting temp from database
                                        break;
                                    }
                                    //Initialization only 1 times
                                    if (_initTemperature)
                                    {
                                        _statistics.Add(new Statistics { UserName = Parameters.CurrentUser, Nplatform = Nplatform, TempStart = Convert.ToInt16(rdr[4]), TempEnd = 0, DataStart = DateTime.Now, DataEnd = DateTime.Now, UpTime = new TimeSpan(00, 00, 00), Enrollment = 0 });
                                        InitTemp.Content = int.Parse(rdr[4].ToString());//Show on initialization temp                                                        
                                        _initTemperature = false;
                                    }

                                    // Set value on board
                                    SetValueDb(int.Parse(rdr[4].ToString()), int.Parse(rdr[5].ToString()));

                                    //Set background color
                                    SetBackgroundTemp(int.Parse(rdr[4].ToString()));

                                    //
                                    SetStatusDevice("CONNECTED");

                                    //When temp it reached the specified
                                    if (int.Parse(rdr[4].ToString()) >= _iMaxTemp)
                                    {
                                        _liveTime.Stop();
                                        _stopwatch.Stop();
                                        //
                                        if (_stopwatch.Elapsed.Seconds > 1)
                                        {
                                            var timeTaken = _stopwatch.Elapsed;
                                            _statistics[0].TempEnd = Convert.ToInt16(rdr[4]);
                                            _statistics[0].DataEnd = DateTime.Now;
                                            _statistics[0].UpTime = timeTaken;
                                            _statistics[0].Enrollment = 1;

                                            // Enable reset button
                                            BtnRefresh.Visibility = Visibility.Visible;

                                            CompletedStats(_statistics[0].UserName, _statistics[0].Nplatform,
                                                _statistics[0].TempStart, _statistics[0].TempEnd, _statistics[0].DataStart,
                                                _statistics[0].DataEnd, _statistics[0].UpTime, _statistics[0].Enrollment);
                                            _isCompleted = true;
                                        }
                                        NotificationEndProcess();
                                        _initTemperature = true;
                                        _isWorking = false; // stop getting temp from database
                                        break;
                                    }
                                }
                            }
                        }
                        catch (MySqlException ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.Device_StartGettingTemp_Error_database, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        await Task.Delay(1000 * Parameters.Delay);
                    }
                }
            }
        }

        private void Ticks(object sender, EventArgs e)
        {
            LiveTimes.Content = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }

        /// <summary>
        /// Write statistics to Database
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="numPlatform"></param>
        /// <param name="startTemp"></param>
        /// <param name="endTemp"></param>
        /// <param name="dataStart"></param>
        /// <param name="dataEnd"></param>
        /// <param name="upTime"></param>
        /// <param name="enrollment"></param>
        private async void CompletedStats(string userName, string numPlatform, int startTemp, int endTemp, DateTime dataStart, DateTime dataEnd, TimeSpan upTime, int enrollment)
        {
            using (var conn = new MySqlConnection(Parameters.ConnStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand("INSERT INTO tempmonitor2.Statistics(UsersName, Nplatform, TempStart, TempEnd, DataStart, DataEnd, UpTime, Enrollment) VALUES (@UsersName, @Nplatform, @TempStart, @TempEnd, @DataStart, @DataEnd, @UpTime, @Enrollment);", conn))
                    {
                        cmd.Parameters.Add("@UsersName", MySqlDbType.VarChar).Value = userName;
                        cmd.Parameters.Add("@Nplatform", MySqlDbType.VarChar).Value = numPlatform;
                        cmd.Parameters.Add("@TempStart", MySqlDbType.Int16).Value = startTemp;
                        cmd.Parameters.Add("@TempEnd", MySqlDbType.Int16).Value = endTemp;
                        cmd.Parameters.Add("@DataStart", MySqlDbType.Date).Value = dataStart;
                        cmd.Parameters.Add("@DataEnd", MySqlDbType.Date).Value = dataEnd;
                        cmd.Parameters.Add("@UpTime", MySqlDbType.Time).Value = upTime;
                        cmd.Parameters.Add("@Enrollment", MySqlDbType.Byte).Value = enrollment;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (MySqlException ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.Device_CreateStatsAsync_DataBase_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Show message that device has reached the maximum set temperature.
        /// </summary>
        private void NotificationEndProcess()
        {
            // Play a sound as a notification.
            SystemSounds.Beep.Play();

            SetBackBlur();
            //Show modal window
            ConfirmationEndProcess.Visibility = Visibility.Visible;
        }

        private void SetValueDb(int currentTemp, int maxTemp)
        {
            CurrentTemperature = currentTemp;
            if (maxTemp != MaxTemp)
            {
                MaxTemp = maxTemp;
            }
        }
        private void SetBackgroundTemp(double temperature)
        {
            // We calculate percent
            var result = (temperature / Convert.ToDouble(_iMaxTemp)) * 100;

            if (result >= Parameters.Green && result < Parameters.Orange)
            {
                CurrentTemp.Fill = Brushes.LimeGreen;
            }
            if (result >= Parameters.Orange && result < Parameters.RedStart)
            {
                CurrentTemp.Fill = Brushes.Orange;
            }
            if (result >= Parameters.RedStart && result <= Parameters.RedEnd)
            {
                CurrentTemp.Fill = Brushes.Red;
            }
        }
        private void SetStatusDevice(string status)
        {
            BitmapImage bitmap;
            switch (status)
            {
                case "CONNECTED":
                    Status.Content = "CONNECTED";
                    //Change photo on green circle
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("UserControlImage/cicleGreen.png", UriKind.Relative);
                    bitmap.EndInit();
                    StatusImage.Source = bitmap;
                    break;
                case "NOTCONNECTED":
                    Status.Content = "NOT CONNECTED";
                    //Change photo on red circle
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("UserControlImage/cicleRed.png", UriKind.Relative);
                    bitmap.EndInit();
                    StatusImage.Source = bitmap;
                    break;
                    //case "CLOSED":
                    //    Status.Content = "NOT CONNECTED";
                    //    //Change photo on green circle
                    //    bitmap = new BitmapImage();
                    //    bitmap.BeginInit();
                    //    bitmap.UriSource = new Uri("UserControlImage/triangle.png", UriKind.Relative);
                    //    bitmap.EndInit();
                    //    StatusImage.Source = bitmap;
                    //    break;
            }
        }

        /// <summary>
        /// Reset value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_clickReset)
            {
                SetBackBlur();

                ConfirmationReset.Visibility = Visibility.Visible;
                _clickReset = false;
            }
        }

        /// <summary>
        /// Remove device(this)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var conn = new MySqlConnection(Parameters.ConnStr))
            {
                try
                {
                    conn.OpenAsync();
                    using (var cmd = new MySqlCommand("DELETE FROM Devices WHERE Name = @Name AND NPlatform = @NPlatform ;", conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", NameofDevice);
                        cmd.Parameters.AddWithValue("@NPlatform", Nplatform);
                        cmd.ExecuteNonQueryAsync();

                        // Remove device from wrapPanel
                        (Parent as WrapPanel)?.Children.Remove(this);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error_server, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }
        /// <summary>
        /// Cancel removing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ConfirmationRemoved.Visibility = Visibility.Hidden;

            // Remove effect
            BlurCard.Effect = null;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationEndProcess.Visibility = Visibility.Hidden;

            // Remove effect
            BlurCard.Effect = null;
        }

        private void BtnCancelReset_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationReset.Visibility = Visibility.Hidden;

            // Remove effect
            BlurCard.Effect = null;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetValueFormDevice();
        }
        private void ResetValueFormDevice()
        {
            // Show Hint
            HintMax.Visibility = Visibility.Visible;

            LocalTemp.Content = string.Empty;

            InitTemp.Content = string.Empty;

            if (!_isCompleted)
            {
                _statistics.RemoveAt(0);
            }
            //
            _clickOne = true;
            //
            _isWorking = false;

            //Reset background
            CurrentTemp.Fill = Brushes.LimeGreen;

            //
            LiveTimes.Content = string.Empty;
            //
            _initTemperature = true;

            //Stop timer
            _liveTime.Stop();
            _stopwatch.Reset();

            // Disable reset button
            BtnRefresh.Visibility = Visibility.Hidden;

            ConfirmationReset.Visibility = Visibility.Hidden; //Hide notification menu

            // Remove effect
            BlurCard.Effect = null;
        }

        private void BtnOkk_Click(object sender, RoutedEventArgs e)
        {
            ErrorCon.Visibility = Visibility.Hidden;

            //
            _clickOne = true;

            // Remove effect
            BlurCard.Effect = null;

            _isWorking = true;
            _stopwatchRecover = _stopwatch;
            StartGettingTemp();
        }
    }
}
