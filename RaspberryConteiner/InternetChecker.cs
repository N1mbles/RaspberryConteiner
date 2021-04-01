using System.Runtime.InteropServices;

namespace RaspberryConteiner
{
   public class InternetChecker
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);
        //Creating a function that uses the API function...  
        public static bool IsConnectedToInternet()
        {
            return InternetGetConnectedState(out var desc, 0);
        }

    }
}
