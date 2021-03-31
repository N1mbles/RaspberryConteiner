using System.Windows.Input;

namespace RaspberryConteiner.CardControl
{
    public partial class UserCard
    {
        public UserCard()
        {
            InitializeComponent();
        }

        public string NameofUser
        {
            get => NameUser.Content.ToString();
            set => NameUser.Content = value;
        }

        private void TestControl_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetCurrentUser();
            System.Windows.Forms.MessageBox.Show(Properties.Resources.UserCard_TestControl_You_changed_user_on__ + NameofUser + Properties.Resources.UserCard_TestControl_Restart_The_device, Properties.Resources.MainWindow_Information, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        private void SetCurrentUser()
        {
            Parameters.CurrentUser = NameofUser;
        }
        private void TestControl_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetCurrentUser();
            System.Windows.Forms.MessageBox.Show(Properties.Resources.UserCard_TestControl_You_changed_user_on__ + NameofUser + Properties.Resources.UserCard_TestControl_Restart_The_device, Properties.Resources.MainWindow_Information, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        private void TestControl_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetCurrentUser();
            System.Windows.Forms.MessageBox.Show(Properties.Resources.UserCard_TestControl_You_changed_user_on__ + NameofUser + Properties.Resources.UserCard_TestControl_Restart_The_device, Properties.Resources.MainWindow_Information, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }
}
