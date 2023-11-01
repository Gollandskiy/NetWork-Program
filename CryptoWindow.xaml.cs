using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Занятие_в_аудитории_1_05._10._2023__Сетевое_программирование_
{
    /// <summary>
    /// Логика взаимодействия для CryptoWindow.xaml
    /// </summary>
    public partial class CryptoWindow : Window
    {
        private readonly HttpClient _httpClient;
        private ListViewItem lastSelectedItem = null!;
        public ObservableCollection<CoinData> CoinsData { get; set; }
        public CryptoWindow()
        {
            InitializeComponent();
            CoinsData = new();
            this.DataContext = this;
            _httpClient = new()
            {
                BaseAddress = new Uri("https://api.coincap.io/")
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAssetsAsync();
        }

        private async Task LoadAssetsAsync()
        {
            var response = JsonSerializer.Deserialize<CoincapResponse>(
                await _httpClient.GetStringAsync("/v2/assets?limit=10")
            );
            if (response == null)
            {
                MessageBox.Show("Deserialization error");
                return;
            }
            CoinsData.Clear();
            foreach (var coinData in response.data)
            {
                CoinsData.Add(coinData);
            }

        }

        private void FrameworkElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (lastSelectedItem != null)
                {
                    lastSelectedItem.Background = null;
                }

                if (item.Content is CoinData coinData)
                {
                    MessageBox.Show($"{coinData.id} - {coinData.symbol}");
                }

                item.Background = Brushes.Aqua;
                lastSelectedItem = item;
            }
        }
    }
    /* Д.З. Реалізувати "перенесення" виділення елемента списку:
     * при виділенні іншого елемента знімати виділення з попереднього.
     * Видати повідомлення (MessageBox) про id ассета, що виділяється.
     * Повторити роботу з WPF Canvas (принаймні рисування ліній)
     */

    ///////////////// ORM ////////////////////////
    public class CoincapResponse
    {
        public List<CoinData> data { get; set; }
        public long timestamp { get; set; }
    }
    public class CoinData
    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string supply { get; set; }
        public string maxSupply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }
        public string explorer { get; set; }
    }
}
