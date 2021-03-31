using MySql.Data.MySqlClient;
using System;

namespace RaspberryConteiner.Helpers
{
    public class MySqlHelper
    {
        /// <summary>
        /// Checking is value is exists
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlRequest"></param>
        /// <returns></returns>
        public static bool CheckExists(MySqlConnection connection, string sqlRequest)
        {
           // connection.OpenAsync();
            using (var cmd = new MySqlCommand(sqlRequest, connection))
            {
                var result = Convert.ToInt32(cmd.ExecuteScalar());
                return result > 0;
            }
        }

    }
}
