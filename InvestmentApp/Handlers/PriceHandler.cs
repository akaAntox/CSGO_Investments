using InvestmentApp.Models;
using InvestmentApp.Models.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InvestmentApp.Handlers
{
    internal static class PriceHandler
    {
        private const string PricesLink = "https://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=";

        /// <summary>
        /// Aggiorna prezzo medio e minimo degli items in base al mercato di steam
        /// </summary>
        public static async Task<IEnumerable<Item>> ScrapePricesAsync(IEnumerable<Item> items, ProgressBar progressBar, Label informationLabel)
        {
            HttpClient web = new();
            Uri url = new(PricesLink);
            int counter = 0;
            int maxRetries = 3;
            int delay = 1000;
            Random random = new();

            foreach (Item item in items)
            {
                int retries = 0;
                bool success = false;

                while (!success && retries < maxRetries)
                {
                    try
                    {
                        string? tmpItemName = HttpUtility.UrlEncode(item.Name);
                        string tmpUrl = url + tmpItemName;

                        ApiResponse? doc = JsonConvert.DeserializeObject<ApiResponse>(await web.GetStringAsync(tmpUrl));
                        if (doc != null && doc.Success)
                        {
                            item.SellPrice = doc.LowestPrice;
                            item.MediumPrice = doc.MedianPrice;
                            success = true;

                            progressBar.Dispatcher.Invoke(() =>
                            {
                                progressBar.Value++;
                                progressBar.Foreground = Brushes.Green;
                                informationLabel.Foreground = Brushes.Black;
                                informationLabel.Content = $"Scraping: {item.Name}";
                                progressBar.ToolTip = $"{counter++} / {items.Count()}";
                            });
                        }

                        delay = 3000;
                        await Task.Delay(delay);
                    }
                    catch (HttpRequestException requestException)
                    {
                        Debug.WriteLine($"{requestException.StatusCode}: {requestException.Message}");
                        Debug.WriteLine($"{requestException.StatusCode} on item \"{item.Name}\" <- {requestException.Source}");

                        progressBar.Dispatcher.Invoke(() =>
                        {
                            informationLabel.Foreground = Brushes.Red;
                            progressBar.ToolTip = informationLabel.Content;
                            informationLabel.Content = progressBar.ToolTip = $"{requestException.StatusCode} on item \"{item.Name}\" (retry {retries + 1}/{maxRetries})";
                        });

                        retries++;
                        delay = (int)Math.Pow(2, retries) * 1000; // Exponential backoff: double the delay after each retry
                        await Task.Delay(delay);
                    }
                }

                if (!success)
                {
                    progressBar.Dispatcher.Invoke(() =>
                    {
                        progressBar.Foreground = Brushes.Red;
                        progressBar.ToolTip = informationLabel.Content;
                        informationLabel.Foreground = Brushes.Red;
                        informationLabel.Content = progressBar.ToolTip = $"Scraping failed for item \"{item.Name}\"";
                    });
                }
            }

            progressBar.Dispatcher.Invoke(() =>
            {
                progressBar.Value = 0;
                informationLabel.Foreground = Brushes.Green;
                informationLabel.Content = "Done scraping!";
                progressBar.ToolTip = null;
            });

            return items;
        }

        /// <summary>
        /// Aggiorna prezzo medio e minimo dell'oggetto item in base al mercato di steam
        /// </summary>
        public static async Task ScrapePriceAsync(Item item)
        {
            await Task.Run(() =>
            {
                HttpClient web = new();
                Uri url = new(PricesLink);

                try
                {
                    string? tmpItemName = HttpUtility.UrlEncode(item?.Name);
                    string tmpUrl = url + tmpItemName;
                    ApiResponse? doc = JsonConvert.DeserializeObject<ApiResponse>(web.GetStringAsync(tmpUrl).GetAwaiter().GetResult());
                    if (doc != null && doc.Success && item != null)
                    {
                        item.SellPrice = doc.LowestPrice;
                        item.MediumPrice = doc.MedianPrice;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error scraping item: {ex.Message}");
                }
            });
        }
    }
}
