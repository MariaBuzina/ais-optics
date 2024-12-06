﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Optics
{
    public static class Data
    {
        public static string role;
        public static int userId;
        public static string name;
        public static string surname;
        public static string patronymic;
        static public Dictionary<string, int> MyBucket;
        public static int GetID(string sql)
        {
            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();

            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            int roleId = Convert.ToInt32(dataTable.Rows[0].ItemArray.GetValue(0));

            connection.Close();
            return roleId;
        }
        public static string GetHashPass(string password)
        {
            using (var sh = SHA256.Create())
            {
                var shbyte = sh.ComputeHash(Encoding.UTF8.GetBytes(password));
                password = BitConverter.ToString(shbyte).Replace("-", "").ToLower();
            }

            return password;
        }

        public static bool InsertUpdateDeleteData(string sql)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Connection.conn))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
