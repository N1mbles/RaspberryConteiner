using System.Windows;

namespace RaspberryConteiner
{
    public partial class Exit
    {    
        public Exit()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           Application.Current.Shutdown();
        }  
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           Close();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}