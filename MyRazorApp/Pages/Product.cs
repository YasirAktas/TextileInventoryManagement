namespace MyRazorApp.Pages
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => StockQuantity * UnitPrice;
    }
}
