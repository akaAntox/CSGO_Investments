using Newtonsoft.Json;
using System;

namespace InvestmentApp.Models.Http
{
    public class ApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("lowest_price")]
        public string? LowestPriceString { get; set; }

        public decimal LowestPrice => Convert.ToDecimal(LowestPriceString != null ? LowestPriceString.Replace("€", string.Empty).Replace("-", "0") : 0);

        [JsonProperty("median_price")]
        public string? MedianPriceString { get; set; }

        public decimal MedianPrice => Convert.ToDecimal(MedianPriceString != null ? MedianPriceString.Replace("€", string.Empty).Replace("-", "0") : 0);
    }
}
