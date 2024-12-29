using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyRazorApp.Pages
{
    public class InventoryModel : PageModel
    {
        private readonly ILogger<InventoryModel> _logger;
        private readonly string connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

        // Properties to hold the list of products in each storeroom
        public List<StoreroomProduct> Storeroom1Products { get; set; }
        public List<StoreroomProduct> Storeroom2Products { get; set; }
        public List<StoreroomProduct> Storeroom3Products { get; set; }

        // Constructor to initialize the logger
        public InventoryModel(ILogger<InventoryModel> logger)
        {
            _logger = logger;
        }

        // The OnGet method that populates the data for the page
        public void OnGet()
        {
            // Fetch products for each storeroom
            Storeroom1Products = GetStoreroomProductsFromDatabase(1);
            Storeroom2Products = GetStoreroomProductsFromDatabase(2);
            Storeroom3Products = GetStoreroomProductsFromDatabase(3);
        }

        // Method to get products from a specific storeroom
        private List<StoreroomProduct> GetStoreroomProductsFromDatabase(int id)
        {
            var products = new List<StoreroomProduct>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductName, StockLevel FROM Product WHERE StoreroomId = @StoreroomId"; // Adjust query based on your database schema
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StoreroomId",id);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new StoreroomProduct
                        {
                            Name = reader.GetString(0), // ProductName
                            StockQuantity = reader.GetInt32(1) // StockLevel
                        });
                    }
                }
            }

            return products;
        }
    }

    // Product class definition
    public class StoreroomProduct
    {
        public string Name { get; set; }
        public int StockQuantity { get; set; }
    }
}