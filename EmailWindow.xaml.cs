using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Занятие_в_аудитории_1_05._10._2023__Сетевое_программирование_
{
    /// <summary>
    /// Логика взаимодействия для EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        public EmailWindow()
        {
            InitializeComponent();
        }
        string jsonFilePath = @"D:\Visual Studio ШАГ\C#\Занятие в аудитории 1(05.10.2023)(Сетевое программирование)\email-settings.json";

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SmtpClient? smtpClient = GetSmtpClient();
            if (smtpClient == null) { return; }
            smtpClient.Send(
                App.GetConfiguration("smtp:email")!,
                textBoxTo.Text,
                textBoxSubj.Text,
                TextBoxMessage.Text
            );
            checkValidJSON();
            MessageBox.Show("Отправлено");
        }
        private SmtpClient GetSmtpClient()
        {
            String? host = App.GetConfiguration("smtp:host");
            if (host == null)
            {
                MessageBox.Show("Error getting host");
                return null;
            }
            String? portString = App.GetConfiguration("smtp:port");
            if (portString == null)
            {
                MessageBox.Show("Error getting port");
                return null;
            }
            int port;
            try { port = int.Parse(portString); }
            catch
            {
                MessageBox.Show("Error parsing port");
                return null;
            }
            String? email = App.GetConfiguration("smtp:email");
            if (email == null)
            {
                MessageBox.Show("Error getting email");
                return null;
            }
            String? password = App.GetConfiguration("smtp:pass");
            if (password == null)
            {
                MessageBox.Show("Error getting password");
                return null;
            }
            String? sslString = App.GetConfiguration("smtp:ssl");
            if (sslString == null)
            {
                MessageBox.Show("Error getting ssl");
                return null;
            }
            bool ssl;
            try { ssl = bool.Parse(sslString); }
            catch
            {
                MessageBox.Show("Error parsing ssl");
                return null;
            }

            if (!textBoxTo.Text.Contains('@'))
            {
                MessageBox.Show("Введите правильную почту!");
                return null;
            }
            return new(host, port)
            {
                EnableSsl = ssl,
                Credentials = new NetworkCredential(email, password)
            };
        }

        private void SendButton2_Click(object sender, RoutedEventArgs e)
        {
            SmtpClient? smtpClient = GetSmtpClient();
            if (smtpClient == null) { return; }

            MailMessage mailMessage = new(
                App.GetConfiguration("smtp:email")!,
                textBoxTo.Text,
                textBoxSubj.Text,
                textBoxHtml.Text)
            {
                IsBodyHtml = true,
            };
            ContentType pngType = new("image/png");
            mailMessage.Attachments.Add(new Attachment("coin25.png",pngType));
            ContentType mp3Type = new("audio/mpeg");
            mailMessage.Attachments.Add(new Attachment("Jump_01.mp3", mp3Type));
            checkValidJSON();
            smtpClient.Send(mailMessage);
            MessageBox.Show("Отправлено");
        }
        private void checkValidJSON()
        {
            try
            {
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                JToken json = JToken.Parse(jsonText);
                if (json != null)
                {
                    MessageBox.Show("Файл JSON валиден.");
                }
                else
                {
                    MessageBox.Show("Файл поврежден или записан неправильно!");
                }
            }
            catch (JsonReaderException)
            {
                MessageBox.Show("Файл поврежден или записан неправильно!");
            }
        }

        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            SmtpClient? smtpClient = GetSmtpClient();
            int lengthCode = 8;
            string rCode = GenerateRandomCode(lengthCode);
            string message = "<h2>Добрый день!</h2> Вас приветствует программа отправки <b style='color:#0EC617'>кода аутентификации</b>: " + $"<h2>{rCode}</h2>";
            string subj = "Приветствуем!";
            if (smtpClient == null) { return; }
            MailMessage mailMessage = new(
                App.GetConfiguration("smtp:email")!,
                textBoxTo.Text,
                subj,
                message)
            {
                IsBodyHtml = true,
            };
            smtpClient.Send(mailMessage);
            MessageBox.Show("Отправлено");
        }
        static string GenerateRandomCode(int length)
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = characters[random.Next(characters.Length)];
            }

            return new string(code);
        }
    }
}
