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

        public decimal Total => Price * Qty;

        public bool? IsPositive
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
    }
}
