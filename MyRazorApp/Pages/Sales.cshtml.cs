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
        public List<Color> Colors {get; set; }
        public List<ProductColor> productColors {get;set;}
         public List<Customer> Customers { get; set; }
        [BindProperty]
        public Customer Customer { get; set; }
        private readonly string connectionString = "Data Source=Yasir;Database=TIMS;Integrated Security=True;";

        public void OnGet()
        {
            Sales = GetSalesFromDatabase();
            Products = GetProductsFromDatabase(); // Get the list of products
            Colors = getColors(Sale);
            Customers = GetCustomersFromDatabase();
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
         private List<Customer> GetCustomersFromDatabase()
        {
            var customers = new List<Customer>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT CustomerID, CustomerName, PhoneNumber, Email, Address, CreatedAt FROM Customer";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerID = reader.GetInt32(0),
                            CustomerName = reader.GetString(1),
                            PhoneNumber = reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }

            return customers;
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
        private List<Color> getColors(Sale sale)
        {
            var colors = new List<Color>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                int x = sale.ProductColorID;
                string query = "SELECT C.ColorID, C.ColorName from Color C inner join Product_Color P on C.ColorID = P.ColorID inner join Product on p.ProductID = @x";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        colors.Add(new Color
                        {
                            ColorID = reader.GetInt32(0),
                            ColorName = reader.GetString(1),
                        });
                    }
                }
            }

            return colors;
        }


        private void AddSale(Sale sale)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string procedureName = "AddSale"; // Assuming you have a stored procedure named RecordSale

                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Parameters for AddSale procedure
                    cmd.Parameters.AddWithValue("@ProductColorID", sale.ProductColorID); // ProductColorID
                    cmd.Parameters.AddWithValue("@Quantity", sale.Quantity); // Quantity

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