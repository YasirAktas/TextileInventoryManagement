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
        private readonly string _connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

        // Properties to hold the list of products in each storeroom
        public List<StoreroomProduct> Storeroom1Products { get; private set; }
        public List<StoreroomProduct> Storeroom2Products { get; private set; }
        public List<StoreroomProduct> Storeroom3Products { get; private set; }
        public List<ProductColorView> ProductColors { get; private set; }

        // Property to capture the selected product name from the form
        [BindProperty]
        public string SelectedProductName { get; set; }

        public InventoryModel(ILogger<InventoryModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            LoadStoreroomData();
            LoadProductColors(SelectedProductName);
        }

        public void OnPost()
        {
            SelectedProductName = Request.Form["SelectedProductName"];
            LoadStoreroomData();
            LoadProductColors(SelectedProductName);
        }

        private void LoadStoreroomData()
        {
            Storeroom1Products = GetStoreroomProductsFromDatabase(1);
            Storeroom2Products = GetStoreroomProductsFromDatabase(2);
            Storeroom3Products = GetStoreroomProductsFromDatabase(3);
        }

        private void LoadProductColors(string productName)
        {
            ProductColors = string.IsNullOrEmpty(productName)
                ? GetAllProductColorsFromDatabase()
                : GetProductColorsByProductName(productName);
        }

        private List<StoreroomProduct> GetStoreroomProductsFromDatabase(int storeroomId)
        {
            return ExecuteQuery(
                "SELECT ProductName, StockLevel FROM Product WHERE StoreroomId = @StoreroomId",
                cmd => cmd.Parameters.AddWithValue("@StoreroomId", storeroomId),
                reader => new StoreroomProduct
                {
                    Name = reader.GetString(0),
                    StockQuantity = reader.GetInt32(1)
                });
        }

        private List<ProductColorView> GetProductColorsByProductName(string productName)
        {
            return ExecuteQuery(
                "SELECT ProductName, AgeGroup, Color, Quantity FROM ProductColorView WHERE ProductName = @ProductName",
                cmd => cmd.Parameters.AddWithValue("@ProductName", productName),
                reader => new ProductColorView
                {
                    ProductName = reader.GetString(0),
                    AgeGroup = reader.GetString(1),
                    Color = reader.GetString(2),
                    Quantity = reader.GetInt32(3)
                });
        }

        private List<ProductColorView> GetAllProductColorsFromDatabase()
        {
            return ExecuteQuery(
                "SELECT ProductName, AgeGroup, Color, Quantity FROM ProductColorView",
                null,
                reader => new ProductColorView
                {
                    ProductName = reader.GetString(0),
                    AgeGroup = reader.GetString(1),
                    Color = reader.GetString(2),
                    Quantity = reader.GetInt32(3)
                });
        }

        private List<T> ExecuteQuery<T>(string query, Action<SqlCommand> configureCommand, Func<SqlDataReader, T> map)
        {
            var results = new List<T>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, con);
            configureCommand?.Invoke(cmd);

            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(map(reader));
            }

            return results;
        }
    }

    public class ProductColorView
    {
        public string ProductName { get; set; }
        public string AgeGroup { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
    }

    public class StoreroomProduct
    {
        public string Name { get; set; }
        public int StockQuantity { get; set; }
    }
}
