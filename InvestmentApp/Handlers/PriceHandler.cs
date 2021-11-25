using InvestmentApp.Models;
using InvestmentApp.Models.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InvestmentApp.Handlers
{
    internal static class PriceHandler
    {
        /// <summary>
        /// Aggiorna prezzo medio e minimo degli items in base al mercato di steam
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static async Task ScrapePricesAsync(IEnumerable<Item> items)
        {
            await Task.Run(() =>
            {
                HttpClient web = new HttpClient();
                Uri url = new Uri("https://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=");

                foreach (Item item in items)
                {
                    try
                    {
                        string? tmpItemName = HttpUtility.UrlEncode(item.Name);
                        string tmpUrl = url + tmpItemName;
                        ApiResponse? doc = JsonConvert.DeserializeObject<ApiResponse>(web.GetStringAsync(tmpUrl).GetAwaiter().GetResult());
                        if (doc != null && doc.Success)
                        {
                            item.SellPrice = doc.LowestPrice;
                            item.MediumPrice = doc.MedianPrice;
                        }
                    }
                    catch (HttpRequestException requestException)
                    {
                        Debug.WriteLine($"{requestException.StatusCode}: {requestException.Message}");
                    }
                }

                JsonHandler.WriteItems(items);
            });
        }

        /// <summary>
        /// Aggiorna prezzo medio e minimo dell'oggetto item in base al mercato di steam
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task ScrapePriceAsync(Item? item)
        {
            await Task.Run(() =>
            {
                HttpClient web = new HttpClient();
                Uri url = new Uri("https://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=");

                try
                {
                    string? tmpItemName = HttpUtility.UrlEncode(item.Name);
                    string tmpUrl = url + tmpItemName;
                    ApiResponse? doc = JsonConvert.DeserializeObject<ApiResponse>(web.GetStringAsync(tmpUrl).GetAwaiter().GetResult());
                    if (doc != null && doc.Success)
                    {
                        item.SellPrice = doc.LowestPrice;
                        item.MediumPrice = doc.MedianPrice;
                    }
                }
                catch (Exception) { }
            });
        }
    }
}
