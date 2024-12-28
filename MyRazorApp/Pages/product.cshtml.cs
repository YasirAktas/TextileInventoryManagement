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

        private readonly string connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

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
                string query = "SELECT ProductCode, ProductName, StockLevel, UnitPrice, StoreroomID FROM Product";
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
                // Define the stored procedure name
                string procedureName = "AddProduct";

                // Create the SQL command
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@ProductName", product.Name);
                    cmd.Parameters.AddWithValue("@AgeGroup", product.AgeGroup);
                    cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    cmd.Parameters.AddWithValue("@StockLevel", product.StockQuantity);
                    cmd.Parameters.AddWithValue("@Discount", product.Discount);
                    cmd.Parameters.AddWithValue("@StoreroomID", product.Storeroom);

                    // Open the connection and execute the command
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void UpdateProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Define the stored procedure name
                string procedureName = "UpdateProduct";

                // Create the SQL command
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@ProductCode", product.Id);
                    cmd.Parameters.AddWithValue("@ProductName", product.Name);
                    cmd.Parameters.AddWithValue("@AgeGroup", product.AgeGroup);
                    cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    cmd.Parameters.AddWithValue("@StockLevel", product.StockQuantity);
                    cmd.Parameters.AddWithValue("@Discount", product.Discount);
                    cmd.Parameters.AddWithValue("@StoreroomID", product.Storeroom);

                    // Open the connection and execute the command
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteProduct(int productCode)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Define the stored procedure name
                string procedureName = "DeleteProduct";

                // Create the SQL command
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameter to specify which product to delete
                    cmd.Parameters.AddWithValue("@ProductCode",productCode);

                    // Open the connection and execute the command
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }

    
}