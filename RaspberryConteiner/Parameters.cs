namespace RaspberryConteiner
{
    public class Parameters
    {
        public static int MaxTemperature
        {
            get => Properties.Settings.Default.DefMaxTemp;
            set
            {
                Properties.Settings.Default["DefMaxTemp"] = value;
                Properties.Settings.Default.Save();
            }
        }
        public static int Delay
        {
            get => Properties.Settings.Default.DelayTemp;
            set
            {
                Properties.Settings.Default["DelayTemp"] = value;
                Properties.Settings.Default.Save();
            }
        }

        #region ColorBack
        public static int Green = 0;
        public static int Yellow = 90;
        public static int RedStart = 95;
        public static int RedEnd = 100;
        #endregion

        #region UsersSettings
        public static int UserWidth = 300;
        public static int UserHeight = 300;
        #endregion

        public static string ConnStr = "server=databasetemp.clbmsuilb1nx.eu-west-3.rds.amazonaws.com;Port=3306;user=admin;database=tempmonitor2;password=3NqKPVNUCeGQ8gRJ;";

        public static string CurrentUser
        {
            get => Properties.Settings.Default.CurrentUser;
            set
            {
                Properties.Settings.Default.CurrentUser = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
