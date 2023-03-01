namespace InvestmentApp.Models
{
    public class Item
    {
        public string? Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal SellPrice { get; set; }
        public decimal MediumPrice { get; set; }
        public string? Category { get; set; }
        public decimal NetProfit => SellPrice - (Price * 1.2M);
        public decimal NetTotalProfit => NetProfit * Qty;
        public decimal Total => Price * Qty;

        public bool? IsPositiveMin
        {
            get
            {
                if (Price < SellPrice)
                    return true;
                else if (Price > SellPrice)
                    return false;
                else
                    return null;
            }
        }

        public bool? IsPositiveMedium
        {
            get
            {
                if (Price < MediumPrice)
                    return true;
                else if (Price > MediumPrice)
                    return false;
                else
                    return null;
            }
        }

        public bool? IsPositiveNet
        {
            get
            {
                if (NetProfit > 0)
                    return true;
                else if (NetProfit < 0)
                    return false;
                else
                    return null;
            }
        }
        public bool? IsPositiveTotalNet
        {
            get
            {
                if (NetTotalProfit > 0)
                    return true;
                else if (NetTotalProfit < 0)
                    return false;
                else
                    return null;
            }
        }
    }
}
