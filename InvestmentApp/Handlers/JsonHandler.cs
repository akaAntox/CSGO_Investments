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
        public static async void WriteItemsAsync(IEnumerable<Item> items, string fileName = @"objects.json")
        {
            try
            {
                var json = JsonConvert.SerializeObject(items);
                await File.WriteAllTextAsync(fileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing items to file: {ex.Message}");
            }
        }

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

        public static async void WriteCategoryAsync(IEnumerable<Category> categories, string fileName = @"categories.json")
        {
            try
            {
                var json = JsonConvert.SerializeObject(categories);
                await File.WriteAllTextAsync(fileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing categories to file: {ex.Message}");
            }
        }

        public static IEnumerable<Category>? ReadCategoryAsync(string fileName = @"categories.json")
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
