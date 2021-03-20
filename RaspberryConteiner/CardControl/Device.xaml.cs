using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using Rasberry.Api.Excel;

namespace RaspberryConteiner.CardControl
{
    /// <summary>
    /// Логика взаимодействия для Device.xaml
    /// </summary>
    public partial class Device : UserControl
    {
        List<TempData> tempereture = new List<TempData>();

        public Device()
        {
            InitializeComponent();
            // Set default temperature
            iMaxTemp = Properties.Settings.Default.DefMaxTemp;
            SetTemp.Text = iMaxTemp.ToString();
        }

        #region Variable
        private int iMaxTemp;
        public string NameofDevice
        {
            get { return NameDevice.Text; }
            set { NameDevice.Text = value; }
        }
        public double CurrentTemperature
        {
            get { return double.Parse((string)LocalTemp.Content); }
            set { LocalTemp.Content = value; }
        }
        public int MaxTemp
        {
            get { return iMaxTemp; }
            set
            {
                iMaxTemp = value;
                SetTemp.Text = value.ToString();
            }
        }

        public string Nplatform
        {
            get { return (string)NumberDevice.Content; }
            set { NumberDevice.Content = value; }
        }

        // Status connecting
        private bool statusConnect = true;
        //
        private bool initTemperature = true;
        #endregion

        /// <summary>
        /// Refresh devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            statusConnect = true;
            StartGettingTemp();
        }
        private int time = 0;
        void Timer_Tick(object sender, EventArgs e)
        {
            //UPTIME.Content = DateTime.Now.ToString("HH:mm:ss");
            time++;
            if (time > 0)
            {
                LiveTime.Content = string.Format("00:0{0}:0{1}", time / 60, time % 60);
                if (time >= 10)
                {
                    LiveTime.Content = string.Format("00:0{0}:{1}", time / 60, time % 60);
                }
                if (time >= 600)
                {
                    if (time % 60 < 10)
                    {
                        LiveTime.Content = string.Format("00:{0}:0{1}", time / 60, time % 60);
                    }
                    else
                    {
                        LiveTime.Content = string.Format("00:{0}:{1}", time / 60, time % 60);
                    }
                }
                if (time >= 3600)
                {
                    if (time % 60 < 10)
                    {
                        LiveTime.Content = string.Format("0{0}:{1}:0{2}", time / 3600, (time / 60) % 60, time % 60);
                    }
                    if ((time / 60) / 60 < 10)
                    {
                        if (time % 60 < 10)
                        {
                            LiveTime.Content = string.Format("0{0}:0{1}:0{2}", time / 3600, (time / 60) % 60, time % 60);
                        }
                        else
                            LiveTime.Content = string.Format("0{0}:0{1}:{2}", time / 3600, (time / 60) % 60, time % 60);
                    }
                    else
                        LiveTime.Content = string.Format("0{0}:{1}:{2}", time / 3600, (time / 60) % 60, time % 60);
                }
                if (time >= 36000)
                {
                    LiveTime.Content = string.Format("{0}:{1}:{2}", time / 3600, (time / 60) % 60, time % 60);
                }
            }
        }
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
            // Add backgroud blur effect
            System.Windows.Media.Effects.BlurEffect blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.KernelType = System.Windows.Media.Effects.KernelType.Gaussian;
            blurEffect.Radius = 7;
            this.BlurCard.Effect = blurEffect;
        }
        // Only one click for start
        private bool clickOne = true;
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (clickOne)
            {
                clickOne = false;
                StartGettingTemp();
            }
        }
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (clickOne)
            {
                clickOne = false;
                StartGettingTemp();
            }

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
        bool IsGood(char c)
        {
            if (c >= '0' && c <= '9')
                return true;
            return false;
        }
        #endregion
        private void SetTemp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SetTemp.Text.Length == 0)
                    System.Windows.Forms.MessageBox.Show("The field is empty", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                MySqlConnection conn = new MySqlConnection(Parameters.connStr);
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE tempmonitor2.Devices SET MaxTemp ='" + int.Parse(SetTemp.Text) + "' WHERE NPlatform = '" + Nplatform + "' ;", conn);
                    MaxTemp = int.Parse(SetTemp.Text);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }

                conn.Close();
            }
        }
        private async void StartGettingTemp()
        {
            if (SetTemp.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("Set max temperature", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                DispatcherTimer LiveTime = new DispatcherTimer
                {
                    Interval = new TimeSpan(0, 0, 1)
                };
                LiveTime.Tick += Timer_Tick;
                LiveTime.Start();

                while (true)
                {
                    try
                    {
                        MySqlConnection conn = new MySqlConnection(Parameters.connStr);
                        await conn.OpenAsync();
                        MySqlCommand cmd = new MySqlCommand("SELECT * FROM tempmonitor2.Devices WHERE Name = '" + NameofDevice + "' AND NPlatform = '" + Nplatform + "' ; ", conn);
                        MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                        while (await rdr.ReadAsync())
                        {
                            // Set value on board
                            SetValueDB(int.Parse(rdr[4].ToString()), int.Parse(rdr[5].ToString()));

                            //
                            if (initTemperature)
                            {
                                InitTemp.Content = int.Parse(rdr[4].ToString());//Show on initialization temp

                                //MySqlCommand cmd2 = new MySqlCommand("INSERT INTO `tempmonitor2`.`Devices` (`InitTemp`) VALUES ('"+ InitTemp.Content + "');", conn);

                                // cmd2.ExecuteReader();
                                initTemperature = false;
                            }

                            //
                            SetStatusDevice(true);

                            //
                            if (int.Parse(rdr[4].ToString()) >= iMaxTemp)
                            {
                                LiveTime.Stop();
                                NotificationEndProcess();
                            }

                            SetBackgroundTemp(int.Parse(rdr[4].ToString()));
                        }
                        await conn.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    await Task.Delay(1000 * Parameters.Delay);
                }
                // GetTemperatureFromServer();
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

            //Change Info
            title.Text = "The device has reached the maximum set temperature!";
            //
            Description.Visibility = System.Windows.Visibility.Hidden;
            btnRemove.Visibility = System.Windows.Visibility.Hidden;

            //Button ok
            btnCancel.Content = "Ok";
            btnCancel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;         
        }

        private void SetValueDB(int _currentTemp, int _maxTemp)
        {
            CurrentTemperature = _currentTemp;

            MaxTemp = _maxTemp;
        }

        private void SetBackgroundTemp(double temperature)
        {
            // We calculate percent
            double result = (temperature / Convert.ToDouble(iMaxTemp)) * 100;

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
            if (status == true)
            {
                Status.Content = "CONNECTED";
                //Change photo on green cicle
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("UserControlImage/cicleGreen.png", UriKind.Relative);
                bitmap.EndInit();
                StatusImage.Source = bitmap;
            }
            else
            {
                Status.Content = "NOT CONNECTED";
                //Change photo on red cicle
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("UserControlImage/cicleRed.png", UriKind.Relative);
                bitmap.EndInit();
                StatusImage.Source = bitmap;
            }
        }
        public void ExportDataToExcel()
        {
            try
            {
                MainWindow main = new MainWindow();

                List<TempData> test = new List<TempData>
                    {
                        new TempData(NameDevice.Text,17.600),
                        new TempData(NameDevice.Text,17.600),
                        new TempData(NameDevice.Text,17.625),
                        new TempData(NameDevice.Text,17.75),
                        new TempData(NameDevice.Text,17.75),
                        new TempData(NameDevice.Text,17.800),
                        new TempData(NameDevice.Text,17.800)
                    };

                ExcelHelper.Save(main._CurrentUser.Text, test);
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
            MySqlConnection conn = new MySqlConnection(Parameters.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("DELETE FROM Devices WHERE Name = '" + NameofDevice + "' AND NPlatform = '" + Nplatform + "' ; ", conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                (this.Parent as WrapPanel).Children.Remove(this);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            conn.Close();

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
            this.BlurCard.Effect = null;
        }

        private void btnOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmationEndProcess.Visibility = System.Windows.Visibility.Hidden;

            // Remove effect
            this.BlurCard.Effect = null;
        }

        private void btnCancelReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmationReset.Visibility = System.Windows.Visibility.Hidden;

            // Remove effect
            this.BlurCard.Effect = null;
        }

        private void btnReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           LocalTemp.Content = string.Empty;

           InitTemp.Content = string.Empty;

           SetTemp.Text = string.Empty;


            ConfirmationReset.Visibility = System.Windows.Visibility.Hidden; //Hide notification menu

            // Remove effect
            this.BlurCard.Effect = null;
        }
    }
}
