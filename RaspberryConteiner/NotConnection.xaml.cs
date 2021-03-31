using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RaspberryConteiner
{
    /// <summary>
    /// Interaction logic for NotConnection.xaml
    /// </summary>
    public partial class NotConnection 
    {
        public NotConnection()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!InternetConnection.CheckForInternetConnection())
            {
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
