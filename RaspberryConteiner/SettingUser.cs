using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryConteiner
{
    public class SettingUser
    {
        public static int MaxUser = 7;
        public static int CountUser
        {
            get
            {
                return Properties.Settings.Default.NumbersUser;
            }
            set
            {
                if (value >= 0)
                    Properties.Settings.Default.NumbersUser = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string CurrentUser
        {
            get
            {
                return Properties.Settings.Default.CurrentUser;
            }
            set
            {
                Properties.Settings.Default.CurrentUser = value;
                Properties.Settings.Default.Save();
            }
        }

        #region Users
        public static string User1
        {
            get
            {
                return Properties.Settings.Default.UserName1;
            }
            set
            {
                Properties.Settings.Default.UserName1 = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string User2
        {
            get
            {
                return Properties.Settings.Default.UserName2;
            }
            set
            {
                Properties.Settings.Default.UserName2 = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string User3
        {
            get
            {
                return Properties.Settings.Default.UserName3;
            }
            set
            {
                Properties.Settings.Default["UserName3"] = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string User4
        {
            get
            {
                return Properties.Settings.Default.UserName4;
            }
            set
            {
                Properties.Settings.Default["UserName4"] = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string User5
        {
            get
            {
                return Properties.Settings.Default.UserName5;
            }
            set
            {
                Properties.Settings.Default["UserName5"] = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string User6
        {
            get
            {
                return Properties.Settings.Default.UserName6;
            }
            set
            {
                Properties.Settings.Default["UserName6"] = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string User7
        {
            get
            {
                return Properties.Settings.Default.UserName7;
            }
            set
            {
                Properties.Settings.Default["UserName7"] = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion
        public static void AddUser(string Name)
        {
            if (CountUser > MaxUser)
            {
                return;
            }
            // add counter
            CountUser++;

            // Set name of user
            switch (CountUser)
            {
                case 1:
                    User1 = Name;
                    break;
                case 2:
                    User2 = Name;
                    break;
                case 3:
                    User3 = Name;
                    break;
                case 4:
                    User4 = Name;
                    break;
                case 5:
                    User5 = Name;
                    break;
                case 6:
                    User6 = Name;
                    break;
                case 7:
                    User7 = Name;
                    break;

                default:
                    break;
            }
        }
        public static void RemoveUser(string Name)
        {
            // minus 1 user
            CountUser--;

            switch (CountUser)
            {
                case 1:
                    User1 = null;
                    break;
                case 2:
                    User2 = null;
                    break;
                case 3:
                    User3 = null;
                    break;
                case 4:
                    User4 = null;
                    break;
                case 5:
                    User5 = null;
                    break;
                case 6:
                    User6 = null;
                    break;
                case 7:
                    User7 = null;
                    break;

                default:
                    break;
            }
        }
    }
}
