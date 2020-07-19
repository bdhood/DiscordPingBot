using DiscordRPC;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordPingBot
{
    class DbApi
    {
        private DbConnection conn;
        public DbApi(DbConnection conn)
        {
            this.conn = conn;
        }

        public string GetLastSent()
        {
            return SQL_GetString("last_sent");
        }

        public void SetLastSent(string time)
        {
            using var cmd = new MySqlCommand();
            cmd.Connection = conn.connection;
            cmd.CommandText = "UPDATE settings SET value='" + time + "' where name='last_sent';";
            cmd.ExecuteNonQuery();
        }

        public string GetCooldown()
        {
            return SQL_GetString("cooldown");
        }
        public string GetChannel()
        {
            return SQL_GetString("channel");
        }

        public string GetMessage()
        {
            return SQL_GetString("message");
        }

        private string SQL_GetString(string name)
        {
            try
            {
                string sql = "SELECT value FROM settings WHERE name='" + name + "';";
                using MySqlCommand cmd = new MySqlCommand(sql, conn.connection);
                using MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                return rdr.GetString(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }
    }
}
