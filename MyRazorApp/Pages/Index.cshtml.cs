using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        // Properties to hold the list of products and revenue
        public List<Product> TotalProducts { get; set; }
        public List<Product> LowStockProducts { get; set; }
        public decimal TotalRevenue { get; set; }

        // Constructor to initialize the logger
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        // The OnGet method that populates the data
        public void OnGet()
        {
            // Sample data for Total Products
            TotalProducts = GetTotalProducts();

            // Sample data for Low Stock Products
            LowStockProducts = GetLowStockProducts();

            // Sample data for Total Revenue
            TotalRevenue = GetTotalRevenue();
        }

        // Method to return a list of all products (sample data)
        private List<Product> GetTotalProducts()
        {
            return new List<Product>
            {
                new Product { Name = "Shirt", StockQuantity = 100 },
                new Product { Name = "Pants", StockQuantity = 50 },
                new Product { Name = "Jacket", StockQuantity = 30 },
            };
        }

        // Method to return a list of low stock products (sample data)
        private List<Product> GetLowStockProducts()
        {
            return new List<Product>
            {
                new Product { Name = "Socks", StockQuantity = 5 },
                new Product { Name = "Gloves", StockQuantity = 2 },
                new Product { Name = "Scarf", StockQuantity = 8 }
            };
        }

        // Method to return total revenue (sample data)
        private decimal GetTotalRevenue()
        {
            return 5000.00m; // Example total revenue
        }
    }

}

