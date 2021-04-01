using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
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
        private Stopwatch _stopwatch;

        #region Variable
        //UP TIME variable
        private bool _isCompleted = false;

        // Only one click for start
        private bool _clickOne = true;

        private bool _isWorking = true;
        private long _id;
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
            ConfirmationRemoved.Visibility = System.Windows.Visibility.Visible;
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
            SetTemp.Focusable = true;
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
                SetTemp.Focusable = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private async void StartGettingTemp()
        {
            if (SetTemp.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.Device_StartGettingTemp_Set_max_temperature, Properties.Resources.MainWindow_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                _stopwatch = new Stopwatch();
                _liveTime = new DispatcherTimer(DispatcherPriority.Render) {Interval = new TimeSpan(0, 0, 1)};
                _liveTime.Tick += Ticks;

                _stopwatch.Start();
                _liveTime.Start();

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

                                while (await rdr.ReadAsync())
                                {
                                    //Initialization only 1 times
                                    if (_initTemperature)
                                    {
                                        CreateStatsAsync(Parameters.CurrentUser, Nplatform, int.Parse(rdr[4].ToString()));
                                        InitTemp.Content = int.Parse(rdr[4].ToString());//Show on initialization temp                                                        
                                        _initTemperature = false;
                                    }

                                    // Set value on board
                                    SetValueDb(int.Parse(rdr[4].ToString()), int.Parse(rdr[5].ToString()));

                                    //Set background color
                                    SetBackgroundTemp(int.Parse(rdr[4].ToString()));

                                    //
                                    SetStatusDevice(true);

                                    //When temp it reached the specified
                                    if (int.Parse(rdr[4].ToString()) >= _iMaxTemp)
                                    {
                                        if (_stopwatch.ElapsedTicks > 1)
                                        {
                                            CompletedsStats(int.Parse(rdr[4].ToString()), LiveTimes.Content.ToString());
                                            _isCompleted = true;
                                        }

                                        _liveTime.Stop();
                                        _stopwatch.Reset();
                                        NotificationEndProcess();
                                        _initTemperature = true;
                                        _isWorking = false; // stop getting temp from database
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //
                            SetStatusDevice(false);

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
        /// Create statistics
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="numPlatform"></param>
        /// <param name="startTemp"></param>
        private async void CreateStatsAsync(string userName, string numPlatform, int startTemp = 0)
        {
            using (var conn = new MySqlConnection(Parameters.ConnStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand("INSERT INTO tempmonitor2.Statistics(UsersName, Nplatform, TempStart, DataStart, Enrollment) VALUES (?UsersName,?Nplatform,?TempStart,?DataStart,?Enrollment);", conn))
                    {
                        cmd.Parameters.Add("?UsersName", MySqlDbType.VarChar).Value = userName;
                        cmd.Parameters.Add("?Nplatform", MySqlDbType.VarChar).Value = numPlatform;
                        cmd.Parameters.Add("?TempStart", MySqlDbType.Int16).Value = startTemp;
                        cmd.Parameters.Add("?DataStart", MySqlDbType.Date).Value = DateTime.Now.ToString("yyyy-MM-dd");
                        cmd.Parameters.Add("?Enrollment", MySqlDbType.Byte).Value = "0";
                        await cmd.ExecuteNonQueryAsync();
                        _id = cmd.LastInsertedId;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.Device_CreateStatsAsync_DataBase_Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// End write statistics
        /// </summary>
        /// <param name="endTemp"></param>
        /// <param name="upTime"></param>
        private async void CompletedsStats(int endTemp, string upTime)
        {
            using (var conn = new MySqlConnection(Parameters.ConnStr))
            {
                try
                {
                    await conn.OpenAsync();

                    using (var cmd = new MySqlCommand("UPDATE tempmonitor2.Statistics SET TempEnd = @TempEnd, DataEnd =@DataEnd, UpTime = @UpTime, Enrollment =@Enrollment WHERE idStatistics = @idStatistics;", conn))
                    {
                        cmd.Parameters.AddWithValue("@TempEnd", endTemp);
                        cmd.Parameters.AddWithValue("@DataEnd", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@UpTime", upTime);
                        cmd.Parameters.AddWithValue("@Enrollment", "1");
                        cmd.Parameters.AddWithValue("@idStatistics", _id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
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
            ConfirmationEndProcess.Visibility = System.Windows.Visibility.Visible;
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

            if (result >= Parameters.Green && result < Parameters.Yellow)
            {
                CurrentTemp.Fill = Brushes.LimeGreen;
            }
            if (result >= Parameters.Yellow && result < Parameters.RedStart)
            {
                CurrentTemp.Fill = Brushes.Orange;
            }
            if (result >= Parameters.RedStart && result <= Parameters.RedEnd)
            {
                CurrentTemp.Fill = Brushes.Red;
            }
        }
        private void SetStatusDevice(bool status)
        {
            if (status)
            {
                Status.Content = "CONNECTED";
                //Change photo on green circle
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("UserControlImage/cicleGreen.png", UriKind.Relative);
                bitmap.EndInit();
                StatusImage.Source = bitmap;
            }
            else
            {
                Status.Content = "NOT CONNECTED";
                //Change photo on red circle
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("UserControlImage/cicleRed.png", UriKind.Relative);
                bitmap.EndInit();
                StatusImage.Source = bitmap;
            }
        }

        /// <summary>
        /// Reset value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetBackBlur();

            ConfirmationReset.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Remove device(this)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
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
        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmationRemoved.Visibility = System.Windows.Visibility.Hidden;

            // Remove effect
            BlurCard.Effect = null;
        }

        private void btnOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmationEndProcess.Visibility = System.Windows.Visibility.Hidden;

            // Remove effect
            BlurCard.Effect = null;
        }

        private void btnCancelReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmationReset.Visibility = System.Windows.Visibility.Hidden;

            // Remove effect
            BlurCard.Effect = null;
        }

        private void btnReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LocalTemp.Content = string.Empty;

            InitTemp.Content = string.Empty;

            SetTemp.Text = Parameters.MaxTemperature.ToString();

            if (!_isCompleted)
            {
                // Delete statistics from the database if it is not complete
                using (var conn = new MySqlConnection(Parameters.ConnStr))
                {
                    try
                    {
                        conn.OpenAsync();
                        using (var cmd =
                            new MySqlCommand("DELETE FROM tempmonitor2.Statistics WHERE idStatistics = @idStatistics; ",
                                conn))
                        {
                            cmd.Parameters.AddWithValue("@idStatistics", _id);
                            cmd.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, Properties.Resources.MainWindow_Error_server,
                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                }
            }

            //
            _clickOne = true;
            //
            _isWorking = false;

            //
            LiveTimes.Content = string.Empty;
            //
            _initTemperature = true;

            //Stop timer
            _liveTime.Stop();
            _stopwatch.Reset();

            ConfirmationReset.Visibility = System.Windows.Visibility.Hidden; //Hide notification menu

            // Remove effect
            BlurCard.Effect = null;
        }
    }
}
