using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using PasswordManager.Encryption;
using PasswordManager.Tools;
using System.IO;

namespace PasswordManager.Tools
{
    class SQLHelper
    {
        MyAes myAes = new MyAes();
        Assistant assistant = new Assistant();
        private readonly string cryptoData = Directory.GetCurrentDirectory() + "\\data\\crypto.data";
        private readonly string pass = "iy#W$NkUTi@jYoBRuA%%Dk5vdL5mmT%";
        private string localhost = "Server=127.0.0.1;Database=PasswordManager;port=3306;User Id=root;password=;SslMode=preferred";

        public SQLHelper()
        {
            string content = File.ReadAllText(cryptoData);
            assistant.SetIV(content.Substring(3, 16));
            assistant.SetSalt(content.Substring(content.LastIndexOf("Salt:") + 5));
            assistant.SetPW(pass);
            myAes.SetParams(assistant.GetPW(), assistant.GetIV());
        }

        public string TestConnection()
        {
            string result;
            try
            {
                MySqlConnection conn = new MySqlConnection(localhost);
                conn.Open();
                result = "Erfolgreich!";
                conn.Close();
            }
            catch(Exception e)
            {
                result = "Fehler: " + e.Message;
            }
            return result;
        }
        public void AddPassword(string user, string password, string entryName)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(localhost);
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT INTO passwords (EntryName, Username, Password) VALUES ('" + entryName + "', '" + user + "', '" + password + "');";
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Eintrag hinzugefügt!", "Erfolgreich!");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Es ist ein Fehler aufgetreten!", MessageBoxButton.OK);
            }
        }
        public string GetEntries(string entryname)
        {
            string result = null;
            try
            {
                MySqlConnection conn = new MySqlConnection(localhost);
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM passwords WHERE EntryName LIKE '%" + entryname + "%';";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result += "Eintrag: " + reader[1].ToString() + Environment.NewLine;
                    result += "Username: " + myAes.Decrypt(reader[2].ToString()) + Environment.NewLine;
                    result += "Passwort: " + myAes.Decrypt(reader[3].ToString()) + Environment.NewLine;
                }
                conn.Close();
            }
            catch(Exception e)
            {
                result = e.Message;
            }
            return result;
        }
    }
}
