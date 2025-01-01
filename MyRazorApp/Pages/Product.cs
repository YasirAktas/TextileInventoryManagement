namespace MyRazorApp.Pages
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int Storeroom { get; set; }
        public decimal Discount {get; set;}
        public string AgeGroup{get; set;}
        // public string Color { get; set; }
        public decimal TotalPrice {get; set; }
        public decimal NetPrice {get; set; }
    }
}
