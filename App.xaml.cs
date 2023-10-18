using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;


namespace Занятие_в_аудитории_1_05._10._2023__Сетевое_программирование_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static String configFilename = "email-settings.json";
        private static JsonElement? settings = null;
        public static String? GetConfiguration(String name)
        {
            if(settings == null)
            {
                if (!System.IO.File.Exists(configFilename))
                {
                    MessageBox.Show($"Файл конфигурации '{configFilename}' не найдено\n",
                        "Операция не может быть завершена!",MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                settings = JsonSerializer.Deserialize<dynamic>(
                System.IO.File.ReadAllText("email-settings.json"));
            }

            JsonElement? jsonElement = settings;
            try
            {
                foreach (String key in name.Split(":"))
                {
                    jsonElement = jsonElement?.GetProperty(key);
                }
            }
            catch
            {
                return null;
            }
            // String? host =
            // settings?.GetProperty("smtp").GetProperty("host").GetString();
            return jsonElement?.GetString();

        }
    }
}
