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
            if (NameofUser == SettingUser.User1)
            {
                main.CcurrentUser = SettingUser.User1;
            }
            if (NameofUser == SettingUser.User2)
            {
                main.CcurrentUser = SettingUser.User2;
            }
            if (NameofUser == SettingUser.User3)
            {
                main.CcurrentUser = SettingUser.User3;
            }
            if (NameofUser == SettingUser.User4)
            {
                main.CcurrentUser = SettingUser.User4;
            }
            if (NameofUser == SettingUser.User5)
            {
                main.CcurrentUser = SettingUser.User5;
            }
            if (NameofUser == SettingUser.User6)
            {
                main.CcurrentUser = SettingUser.User6;
            }
            if (NameofUser == SettingUser.User7)
            {
                main.CcurrentUser = SettingUser.User7;
            }
            else
            {
                main.CcurrentUser = NameofUser;
            }
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
