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
using System.Windows.Media;

namespace InvestmentApp.Handlers
{
    internal static class PriceHandler
    {
        /// <summary>
        /// Aggiorna prezzo medio e minimo degli items in base al mercato di steam
        /// </summary>
        public static async Task<IEnumerable<Item>> ScrapePricesAsync(IEnumerable<Item> items, ProgressBar progressBar, Label informationLabel)
        {
            return await Task.Run(() =>
            {
                HttpClient web = new();
                Uri url = new("https://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=");
                bool exception = false;

                foreach (Item item in items)
                {
                    try
                    {
                        progressBar.Dispatcher.Invoke(() =>
                        {
                            progressBar.Value++;
                            progressBar.Foreground = Brushes.Green;
                            informationLabel.Foreground = Brushes.Black;
                            progressBar.ToolTip = informationLabel.Content = $"Scraping: {item.Name}";
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
                        Debug.WriteLine($"{requestException.StatusCode} on item \"{item.Name}\" <- {requestException.Source}");

                        progressBar.Dispatcher.Invoke(() =>
                        {
                            progressBar.Foreground = Brushes.Red;
                            informationLabel.Foreground = Brushes.Red;
                            progressBar.ToolTip = informationLabel.Content;
                            informationLabel.Content = progressBar.ToolTip = $"{requestException.StatusCode} on item \"{item.Name}\"";
                        });

                        exception = true;
                        break;
                    }
                }

                if (!exception)
                {
                    progressBar.Dispatcher.Invoke(() =>
                    {
                        progressBar.Value = 0;
                        informationLabel.Foreground = Brushes.Green;
                        informationLabel.Content = "Done scraping!";
                        progressBar.ToolTip = null;
                    });
                }

                return items;
            });
        }

        /// <summary>
        /// Aggiorna prezzo medio e minimo dell'oggetto item in base al mercato di steam
        /// </summary>
        public static async Task ScrapePriceAsync(Item item)
        {
            await Task.Run(() =>
            {
                HttpClient web = new();
                Uri url = new("https://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=");

                try
                {
                    string? tmpItemName = HttpUtility.UrlEncode(item?.Name);
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
