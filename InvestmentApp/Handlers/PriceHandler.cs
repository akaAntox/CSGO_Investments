using InvestmentApp.Models;
using InvestmentApp.Models.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;

namespace InvestmentApp.Handlers
{
    internal static class PriceHandler
    {
        /// <summary>
        /// Aggiorna prezzo medio e minimo degli items in base al mercato di steam
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Item>> ScrapePricesAsync(IEnumerable<Item> items, ProgressBar progressBar)
        {
            return await Task.Run(() =>
            {
                HttpClient web = new HttpClient();
                Uri url = new Uri("https://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=");

                foreach (Item item in items)
                {
                    try
                    {
                        progressBar.Dispatcher.Invoke(() =>
                        {
                            progressBar.Value++;
                            progressBar.ToolTip = $"Scraping: {item.Name}";
                        });

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

                return items;
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
