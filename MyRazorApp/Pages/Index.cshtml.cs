using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly string connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

        // Properties to hold the list of products and revenue
        public List<ProductStockSummary> TotalProducts { get; set; }
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
            TotalProducts = GetTotalProductsFromDatabase();
            LowStockProducts = GetLowStockProductsFromDatabase();
            TotalRevenue = GetTotalRevenueFromDatabase();
        }

        private List<ProductStockSummary> GetTotalProductsFromDatabase()
        {
            var products = new List<ProductStockSummary>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductName, TotalStockLevel, ColorVariants FROM ProductStockSummary"; // Query the view
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new ProductStockSummary
                        {
                            Name = reader.GetString(0), // ProductName
                            StockQuantity = reader.GetInt32(1), // StockLevel
                            ColorVariants = reader.GetInt32(2) //
                        });
                    }
                }
            }


            return products;
        }

        private List<Product> GetLowStockProductsFromDatabase()
        {
            var products = new List<Product>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductName, StockLevel FROM ProductStockView Where StockLevel < 51"; // Query the view
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Name = reader.GetString(0), // ProductName
                            StockQuantity = reader.GetInt32(1), // StockLevel
                        });
                    }
                }
            }

            return products;
        }

        private decimal GetTotalRevenueFromDatabase()
        {
            decimal totalRevenue = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT SUM(UnitPrice * StockLevel) FROM Product";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    totalRevenue = (decimal)result;
                }
            }

            return totalRevenue;
        }
    }
    public class ProductStockSummary
    {
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public int ColorVariants {get; set; }
    }
}
