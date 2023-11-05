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
using System.Globalization;

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
            Graph.Width = 600;
            Graph.Height = 350;
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
                    History(coinData);
                }
                item.Background = Brushes.Aqua;
                lastSelectedItem = item;
            }
        }
        private async Task History(CoinData coinData)
        {
            string body = await _httpClient.GetStringAsync($"/v2/assets/{coinData.id}/history?interval=d1");
            var response = JsonSerializer.Deserialize<HistoryResponse>(body);
            if (response == null || response.data == null)
            {
                MessageBox.Show("Ошибка загрузки данных");
                return;
            }
            Graph.Children.Clear();
            long minTime, maxTime;
            double minPrice, maxPrice;
            minTime = maxTime = response.data[0].time;
            minPrice = maxPrice = response.data[0].price;
            foreach (HistoryClass item in response.data)
            {
                if (item.time < minTime)
                {
                    minTime = item.time;
                }
                if (item.time > maxTime)
                {
                    maxTime = item.time;
                }
                if (item.price < minPrice)
                {
                    minPrice = item.price;
                }
                if (item.price > maxPrice)
                {
                    maxPrice = item.price;
                }
            }
            double yOffSet = 0;
            double graphH = Graph.ActualHeight - yOffSet;
            double x0 = (response.data[0].time - minTime) * Graph.ActualWidth / (maxTime - minTime);
            double y0 = graphH - ((response.data[0].price - minPrice) * graphH / (maxPrice - minPrice));
            foreach (HistoryClass item in response.data)
            {
                double x = (item.time - minTime) * Graph.ActualWidth / (maxTime - minTime);
                double y = graphH - ((item.price - minPrice) * graphH / (maxPrice - minPrice));
                DrLine(x0, x, y0, y);
                x0 = x;
                y0 = y;
            }
            DrawLabel(0, graphH + 5, Formatting.GetDateFromSeconds(minTime), Brushes.Green);
            DrawLabel(Graph.ActualWidth - 60, graphH + 5, Formatting.GetDateFromSeconds(maxTime), Brushes.Green);
            DrawLabel(0, graphH + 20, Formatting.GetStringFromPrice(minPrice));
            DrawLabel(Graph.ActualWidth - 60, graphH + 20, Formatting.GetStringFromPrice(maxPrice));
            DrLine(0, Graph.ActualWidth, graphH, graphH, Brushes.Violet);
        }
        private void DrLine(double x1, double x2, double y1, double y2, Brush brush = null)
        {
            brush ??= new SolidColorBrush(Colors.Black);
            Graph.Children.Add(new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = brush,
                StrokeThickness = 2
            });
        }
        private void DrawLabel(double x, double y, string text, Brush? brush = null)
        {
            brush ??= new SolidColorBrush(Colors.Red);
            TextBlock textBlock = new() { Text = text, Foreground = brush, };
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            Graph.Children.Add(textBlock);
        }
    }
    /* Д.З. Реалізувати "перенесення" виділення елемента списку:
     * при виділенні іншого елемента знімати виділення з попереднього.
     * Видати повідомлення (MessageBox) про id ассета, що виділяється.
     * Повторити роботу з WPF Canvas (принаймні рисування ліній)
     */

    ///////////////// ORM ////////////////////////
    public static class Formatting
    {
        private static DateTime epoch = new(1976, 1, 1, 0, 0, 0);

        public static string GetDateFromSeconds(long seconds)
        {
            return epoch.AddSeconds(seconds / 1000).ToString("dd.MM.yyyy");
        }

        public static string GetStringFromPrice(double price)
        {
            return Math.Round(price, 2).ToString();
        }
    }
    public class HistoryResponse
    {
        public List<HistoryClass> data { get; set; }
        public long timestamp { get; set; }
    }

    public class HistoryClass
    {
        public string priceUsd { get; set; }
        public long time { get; set; }
        public double price => Double.Parse(priceUsd,CultureInfo.InvariantCulture);
    }

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
