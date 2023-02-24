using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Diagnostics;
using System.Security.Cryptography;

namespace GameWIndowTest1.Global.SQL
{
    class users_sql : base_sql
    {
        public users_sql(string E_table, string E_filename): base(E_table, E_filename)
        {
            con.Open();
        }

        public void createTable()
        {
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {_table}(userID INTEGER PRIMARY KEY, username TEXT, password TEXT, salt TEXT)";
            cmd.ExecuteNonQuery();

        }
        public void insert(string username, string password, string salt)
        {
            con.Open();
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = $"INSERT INTO TABLE {_table} VALUES ('{username}, {password}, {salt}')";
            cmd.ExecuteNonQuery();
        }
        public string[] select(string username)
        {
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = $"SELECT userID from {_table} WHERE username='{username}'";
            var reader = cmd.ExecuteReader();

            // if the number of fields returned == 0
            // then no user was found and so the username is not in use
            // so return false
            List<string> items = new List<string>();

            while (reader.Read())
            {
                Trace.WriteLine(reader);
                for (int columnID = 0; columnID < reader.GetValues().Count; columnID++)
                {
                    Trace.WriteLine(reader.GetString(columnID));
                    items.Add(reader.GetString(columnID));
                }
            }
            MessageBox.Show(items.ToString());
            return new string[0];
        }
    }
}
