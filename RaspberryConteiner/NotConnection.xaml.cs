using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RaspberryConteiner
{
    /// <summary>
    /// Interaction logic for NotConnection.xaml
    /// </summary>
    public partial class NotConnection : Window
    {
        public NotConnection()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!InternetConnection.CheckForInternetConnection())
            {
                return;
            }
            else
            {
                //Hide Spinner
                Loading.Visibility = Visibility.Hidden;
                // Show wifi connected
                Wifi.Visibility = Visibility.Visible;
                Connected.Visibility = Visibility.Visible;
                Thread.Sleep(3000);
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                if (!InternetConnection.CheckForInternetConnection())
                {
                    return;
                }
                else
                {
                    //Hide Spinner
                    Loading.Visibility = Visibility.Hidden;
                    // Show wifi connected
                    Wifi.Visibility = Visibility.Visible;
                    Connected.Visibility = Visibility.Visible;
                    Task.Delay(2000);
                    this.Close();
                }
            }
        }
    }
}
