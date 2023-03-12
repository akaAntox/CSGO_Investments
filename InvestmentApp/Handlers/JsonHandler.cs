using InvestmentApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace InvestmentApp.Handlers
{
    public static class JsonHandler
    {
        public static async void WriteItems(IEnumerable<Item> item, string fileName = @"objects.json")
            => await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(item));

        public static IEnumerable<Item>? ReadItems(string fileName = @"objects.json")
        {
            try
            {
                string json = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<IEnumerable<Item>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading categories from file: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        public static async void WriteCategory(IEnumerable<Category> cat, string fileName = @"categories.json")
            => await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(cat));

        public static IEnumerable<Category>? ReadCategory(string fileName = @"categories.json")
        {
            try
            {
                string json = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<IEnumerable<Category>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading categories from file: {ex.Message}");
                return Enumerable.Empty<Category>();
            }
        }
    }
}
