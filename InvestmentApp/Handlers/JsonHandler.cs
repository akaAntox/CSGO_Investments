using InvestmentApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace InvestmentApp.Handlers
{
    public static class JsonHandler
    {
        public static async void WriteItems(IEnumerable<Item> item, string fileName = @"objects.json")
            => await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(item));

        public static IEnumerable<Item>? ReadItems(string fileName = @"objects.json") =>        
            JsonConvert.DeserializeObject<IEnumerable<Item>>(File.ReadAllText(fileName));

        public static async void WriteCategory(IEnumerable<Category> cat, string fileName = @"categories.json")
            => await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(cat));

        public static IEnumerable<Category>? ReadCategory(string fileName = @"categories.json") => 
            JsonConvert.DeserializeObject<IEnumerable<Category>>(File.ReadAllText(fileName));
    }
}
