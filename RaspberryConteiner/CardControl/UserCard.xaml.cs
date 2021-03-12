using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace RaspberryConteiner.CardControl
{
    /// <summary>
    /// Логика взаимодействия для UserCard.xaml
    /// </summary>
    public partial class UserCard : UserControl
    {
        public UserCard()
        {
            InitializeComponent();
        }

        public String NameofUser
        {
            get { return NameUser.Content.ToString(); }
            set { NameUser.Content = value; }
        }

        private void TestControl_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetCurrentUser();
            System.Windows.Forms.MessageBox.Show("You changed user on: " + NameofUser + "\n     Restart the device!", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        private void SetCurrentUser()
        {
            MainWindow main = new MainWindow();
            main.CcurrentUser = NameofUser;
        }
        private void TestControl_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetCurrentUser();
            System.Windows.Forms.MessageBox.Show("You changed user on: " + NameofUser + "\n     Restart the device!", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        private void TestControl_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetCurrentUser();
            System.Windows.Forms.MessageBox.Show("You changed user on: " + NameofUser + "\n     Restart the device!", "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }
}
