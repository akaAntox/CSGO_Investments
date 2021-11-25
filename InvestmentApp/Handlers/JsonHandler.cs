using InvestmentApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace InvestmentApp.Handlers
{
    public static class JsonHandler
    {
        public static void WriteItems(IEnumerable<Item> item, string fileName = @"objects.json")
            => File.WriteAllText(fileName, JsonConvert.SerializeObject(item));

        public static IEnumerable<Item>? ReadItems(string fileName = @"objects.json")
        {
            return JsonConvert.DeserializeObject<IEnumerable<Item>>(File.ReadAllText(fileName));
        }

        public static void WriteCategory(IEnumerable<Category> cat, string fileName = @"categories.json")
            => File.WriteAllText(fileName, JsonConvert.SerializeObject(cat));

        public static IEnumerable<Category>? ReadCategory(string fileName = @"categories.json")
        {
            return JsonConvert.DeserializeObject<IEnumerable<Category>>(File.ReadAllText(fileName));
        }
    }
}
