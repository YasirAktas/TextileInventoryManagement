using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyRazorApp.Pages
{
    public class ProductModel : PageModel
    {
        public List<Product> Products { get; set; }
        [BindProperty]
        public Product Product { get; set; }

        private readonly string connectionString = "Data Source=Yasir;Initial Catalog=TIMS;Integrated Security=True;";

        public void OnGet()
        {
            Products = GetProductsFromDatabase();
        }

        public IActionResult OnPost()
        {
            if (Request.Form["action"] == "delete")
            {
                var productId = int.Parse(Request.Form["ProductCode"]);
                DeleteProduct(productId);
            }
            else
            {
                if (Product.ProductCode == 0)
                {
                    AddProduct(Product);
                }
                else
                {
                    UpdateProduct(Product);
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            DeleteProduct(id);
            return RedirectToPage();
        }

        private List<Product> GetProductsFromDatabase()
        {
            var products = new List<Product>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductCode, ProductName, StockLevel, UnitPrice, StoreroomID FROM Products";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductCode = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            StockLevel = reader.GetInt32(2),
                            UnitPrice = reader.GetDecimal(3),
                            StoreroomID = reader.GetInt32(4)
                        });
                    }
                }
            }

            return products;
        }

        private void AddProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Products (ProductName, StockLevel, UnitPrice, StoreroomID) VALUES (@ProductName, @StockLevel, @UnitPrice, @StoreroomID)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@StockLevel", product.StockLevel);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@StoreroomID", product.StoreroomID);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Products SET ProductName = @ProductName, StockLevel = @StockLevel, UnitPrice = @UnitPrice, StoreroomID = @StoreroomID WHERE ProductCode = @ProductCode";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@StockLevel", product.StockLevel);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@StoreroomID", product.StoreroomID);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteProduct(int productCode)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE ProductCode = @ProductCode";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProductCode", productCode);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

   
}
