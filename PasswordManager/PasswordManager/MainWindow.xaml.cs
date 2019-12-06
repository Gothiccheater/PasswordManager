using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PasswordManager.Encryption;
using PasswordManager.Tools;
using System.IO;

namespace PasswordManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyAes myAes = new MyAes();
        Assistant assistant = new Assistant();
        SQLHelper sqlHelper = new SQLHelper();
        private readonly string dataDir = Directory.GetCurrentDirectory() + "\\data";
        private readonly string cryptoData = Directory.GetCurrentDirectory() + "\\data\\crypto.data";
        private readonly string managerData = Directory.GetCurrentDirectory() + "\\data\\manager.data";
        private readonly string pass = "iy#W$NkUTi@jYoBRuA%%Dk5vdL5mmT%";
        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(dataDir)) {
                Directory.CreateDirectory(dataDir);
                assistant.CreateSalt();
                assistant.SetIV(assistant.RandomGen(16));
                StreamWriter sw = File.AppendText(cryptoData);
                sw.Write("IV:" + assistant.GetIV() + Environment.NewLine);
                sw.Write("Salt:" + assistant.GetSalt());
                sw.Close();
            }
            else
            {
            }
            string content = File.ReadAllText(cryptoData);
            assistant.SetIV(content.Substring(3, 16));
            assistant.SetSalt(content.Substring(content.LastIndexOf("Salt:") + 5));
            assistant.SetPW(pass);
            myAes.SetParams(assistant.GetPW(), assistant.GetIV());
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            TextBoxPassword.Text = assistant.RandomGen(16);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxEntryname.Text) || string.IsNullOrEmpty(TextBoxPassword.Text))
            {
                MessageBox.Show("Eintragsname oder Password darf nicht leer sein!", "Fehler!", MessageBoxButton.OK);
            }
            else
            {
                sqlHelper.AddPassword(myAes.Encrypt(TextBoxUsername.Text), myAes.Encrypt(TextBoxPassword.Text), TextBoxEntryname.Text);
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(sqlHelper.GetEntries(TextBoxEntryname.Text), "Einträge");
        }
    }
}
