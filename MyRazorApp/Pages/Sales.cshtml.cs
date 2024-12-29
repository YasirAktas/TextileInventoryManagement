using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyRazorApp.Pages
{
    public class SaleModel : PageModel
    {
       public List<Sale> Sales { get; set; }
        public List<Product> Products { get; set; } // List of products for dropdown selection
        [BindProperty]
        public Sale Sale { get; set; }

        private readonly string connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

        public void OnGet()
        {
            Sales = GetSalesFromDatabase();
            Products = GetProductsFromDatabase(); // Get the list of products
        }

        public IActionResult OnPost()
        {
            if (Sale.Id == 0) // Only add if it's a new sale
            {
                AddSale(Sale);
            }
            return RedirectToPage();
        }

        private List<Sale> GetSalesFromDatabase()
        {
            var sales = new List<Sale>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT SaleID, ProductColorID, Quantity, SaleDate FROM Sale";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sales.Add(new Sale
                        {
                            Id = reader.GetInt32(0),
                            ProductColorID = reader.GetInt32(1),
                            Quantity = reader.GetInt32(2),
                            SaleDate = reader.GetDateTime(3)
                        });
                    }
                }
            }

            return sales;
        }

        private List<Product> GetProductsFromDatabase()
        {
            var products = new List<Product>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductCode, ProductName FROM Product";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0), // ProductCode
                            Name = reader.GetString(1) // ProductName
                        });
                    }
                }
            }

            return products;
        }


        private void AddSale(Sale sale)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string procedureName = "RecordSale"; // Assuming you have a stored procedure named RecordSale

                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductColorID", sale.ProductColorID); // ProductColorID
                    cmd.Parameters.AddWithValue("@Quantity", sale.Quantity); // Quantity
                    cmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate); // SaleDate

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class Sale
    {
        public int Id { get; set; } // Maps to SaleID
        public int ProductColorID { get; set; } // Maps to ProductColorID
        public int Quantity { get; set; } // Maps to Quantity
        public DateTime SaleDate { get; set; } // Maps to SaleDate
    }
}
