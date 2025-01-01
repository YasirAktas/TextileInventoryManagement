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
         [BindProperty]
        public int SelectedProductId { get; set; }


        public void OnGet()
        {
            Sales = GetSalesFromDatabase();
            Products = GetProductsFromDatabase(); // Get the list of products
            LoadColors(SelectedProductId);
            Customers = GetCustomersFromDatabase();
        }

        public IActionResult OnPost()
        {
            if (Sale.Id == 0) // Only add if it's a new sale
            {   
                GetProductColor(Sale);
                AddSale(Sale);
            }

            // Filter colors dynamically based on selected product
            var productId = int.Parse(Request.Form["SelectedProductId"]);
            LoadColors(productId);
            return RedirectToPage();
        }
        public JsonResult OnGetProductColors(int productId)
        {
            var colors = getColorsByProductId(productId);
            return new JsonResult(colors.Select(color => new
            {
                ColorId = color.ColorID,
                ColorName = color.ColorName
            }));
        }

        private void LoadColors(int productId)
        {
            Colors = (productId==0)
                ? getColors()
                : getColorsByProductId(productId);
        }

         private List<Sale> GetSalesFromDatabase()
        {
            var sales = new List<Sale>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT SaleID, ProductName,ProductColorID, ColorName, Quantity, SaleDate from salesView";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sales.Add(new Sale
                        {
                            Id = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            ProductColorID = reader.GetInt32(2),
                            Color = reader.GetString(3),
                            Quantity = reader.GetInt32(4),
                            SaleDate = reader.GetDateTime(5)
                        });
                    }
                }
            }

            return sales;
        }
        public void GetProductColor(Sale sale)
        {
            
            string query = "SELECT pc.ProductColorID FROM Product_Color pc WHERE pc.ProductCode = @ProductCode AND ColorID = @ColorID; ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                     SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductCode", SelectedProductId);
                    cmd.Parameters.AddWithValue("@ColorID", sale.ColorID);
                    

                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            sale.ProductColorID = reader.GetInt32(reader.GetOrdinal("ProductColorID"));
                            
                        }
                    }

                
            }

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
        private List<Color> getColorsByProductId(int ProductId)
        {
            var colors = new List<Color>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT C.ColorID, C.ColorName from Color C inner join Product_Color Pc on C.ColorID = Pc.ColorID where pc.ProductCode = @ProductId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", ProductId);

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
        private List<Color> getColors()
        {
            var colors = new List<Color>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT C.ColorID, C.ColorName from Color C inner join Product_Color P on C.ColorID = P.ColorID";
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
        public string ProductName { get; set; }
        public string Color {get; set;}
        public int ColorID {get; set;}
        public int ProductColorID { get; set; } // Maps to ProductColorID
        public int Quantity { get; set; } // Maps to Quantity
        public DateTime SaleDate { get; set; } // Maps to SaleDate
    }
}