using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.Text.Json;

namespace Занятие_в_аудитории_1_05._10._2023__Сетевое_программирование_
{
    /// <summary>
    /// Логика взаимодействия для HttpWindow.xaml
    /// </summary>
    public partial class HttpWindow : Window
    {
        private List<NbuRate>? rates;
        private String[] popularCc = { "XAU", "USD", "EUR" };
        public HttpWindow()
        {
            InitializeComponent();
        }

        private async void get1Button_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync("https://itstep.org/");
            textBlock1.Text = "";
            textBlock1.Text += (int)response.StatusCode + " " + response.ReasonPhrase + "\r\n";
            foreach (var header in response.Headers)
            {
                textBlock1.Text += $"{header.Key, -20}" + ": " + 
                    String.Join(',', header.Value).Ellipsis(40)
                    + "\r\n";
            }
            String body = await response.Content.ReadAsStringAsync();
            textBlock1.Text += $"\r\n{body}";
        }

        private async void getXMLButton_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange");
            String body = await response.Content.ReadAsStringAsync();
            textBlock1.Text = $"\r\n{body}";
        }

        private async void getJSONButton_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            String body = await response.Content.ReadAsStringAsync();
            textBlock1.Text = $"\r\n{body}";
        }

        private async void ratesButton_Click(object sender, RoutedEventArgs e)
        {
            if (rates == null)
            {
                await loadRatesAsync();
            }
            if (rates == null)
            {
                return;
            }
            foreach (var rate in rates)
            {
                textBlock1.Text += $"\t {rate.cc} - {rate.txt} - {rate.rate}\n";
            }
        }
        private async Task loadRatesAsync()
        {
            using HttpClient httpclient = new();
            String body = await httpclient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            var rates = JsonSerializer.Deserialize<List<NbuRate>>(body);

            if (rates == null)
            {
                MessageBox.Show("Error Deserializing");
                return;
            }
        }
        private async void popularButton_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpclient = new();
            String body = await httpclient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            var rates = JsonSerializer.Deserialize<List<NbuRate>>(body);
            foreach (var rate in rates)
            {
                if (popularCc.Contains(rate.cc))
                textBlock1.Text += $"\t {rate.cc} - {rate.txt} - {rate.rate}\n";
            }
        }
    }
    class NbuRate
    {
        public int R030 { get; set; }
        public string txt { get; set; }
        public double rate { get; set; }
        public string cc { get; set; }
        public string exchangedate { get; set; }
    }

    public static class EllipsisExtensions
    {
        public static string Ellipsis(this string str, int maxLength)
        {
            return str.Length > maxLength
                ? str[..(maxLength - 3)] + "..."
                : str;
        }
    }
}
