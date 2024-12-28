using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
                var productId = int.Parse(Request.Form["ProductId"]);
                DeleteProduct(productId);
            }
            else
            {
                if (Product.Id == 0)
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
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            StockQuantity = reader.GetInt32(2),
                            UnitPrice = reader.GetDecimal(3),
                            Storeroom = reader.GetInt32(4)
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
                string query = "INSERT INTO Products (Name, StockQuantity, UnitPrice, Storeroom) VALUES (@Name, @StockQuantity, @UnitPrice, @Storeroom)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Storeroom", product.Storeroom);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Products SET Name = @Name, StockQuantity = @StockQuantity, UnitPrice = @UnitPrice, Storeroom = @Storeroom WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", product.Id);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Storeroom", product.Storeroom);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteProduct(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    
}
