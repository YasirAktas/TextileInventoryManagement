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
        public List<Color> Colors {get; set; }
        public List<ProductColor> productColors {get;set;}

        [BindProperty]
        public ProductColor productColor {get; set;} 
        public Color color {get; set; }

        private readonly string connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

        public void OnGet()
        {
            Products = GetProductsFromDatabase();
            Colors = getColors();
        }

        public IActionResult OnPost()
        {
            if (Request.Form["action"] == "delete")
            {
                var productId = int.Parse(Request.Form["ProductId"]);
                DeleteProduct(productId);
            }
            else if (Request.Form["action"] == "removeQuantity")
            {
                // Extract data for removing a product color quantity
                int productCode = int.Parse(Request.Form["Product.Id"]);
                int colorID = int.Parse(Request.Form["productColor.ColorID"]);
                int quantityToRemove = int.Parse(Request.Form["productColor.Quantity"]);

                RemoveProductColorQuantity(productCode, colorID, quantityToRemove);
            }
            else if (Product.Id > 0)
            {
                // Extract data for adding a product color
                int productCode = int.Parse(Request.Form["Product.Id"]);
                int colorID = int.Parse(Request.Form["productColor.ColorID"]);
                int quantity = int.Parse(Request.Form["productColor.Quantity"]);
                decimal pricePerUnit = Product.UnitPrice; // Assuming UnitPrice is passed
                //HATALI DUZELTILMESI LAZIM

                UpdateProduct(productCode, colorID, quantity, pricePerUnit);
            }
            

            else
            {
                AddProduct(Product);
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
                string query = "SELECT ProductCode, ProductName, AgeGroup, UnitPrice, StockLevel, StoreroomID, TotalPrice, NetPrice, Discount FROM ProductView";
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
                            AgeGroup = reader.GetString(2),
                            UnitPrice = reader.GetDecimal(3),
                            StockQuantity = reader.GetInt32(4),
                            Storeroom = reader.GetInt32(5),
                            TotalPrice = reader.GetDecimal(6),
                            NetPrice = reader.GetDecimal(7),
                            Discount = reader.GetDecimal(8)
                        });
                    }
                }
            }

            return products;
        }

        private List<Color> getColors()
        {
            var colors = new List<Color>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ColorID, ColorName from Color";
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
                    cmd.Parameters.AddWithValue("@Discount", product.Discount);
                    cmd.Parameters.AddWithValue("@StoreroomID", product.Storeroom);

                    // Open the connection and execute the command
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void UpdateProduct(int productCode, int colorID, int quantity, decimal pricePerUnit)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string procedureName = "AddProductColor";

                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductCode", productCode);
                    cmd.Parameters.AddWithValue("@ColorID", colorID);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);

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
        private void RemoveProductColorQuantity(int productCode, int colorID, int quantityToRemove)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string procedureName = "RemoveProductColorQuantity";

                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductCode", productCode);
                    cmd.Parameters.AddWithValue("@ColorID", colorID);
                    cmd.Parameters.AddWithValue("@QuantityToRemove", quantityToRemove);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public JsonResult OnGetProductColors(int productId)
        {
            var colors = getColorsByProductId(productId); // Fetch colors based on productId
            return new JsonResult(colors.Select(color => new
            {
                ColorId = color.ColorID,     // Return the ColorID
                ColorName = color.ColorName  // Return the ColorName
            }));
        }

        private List<Color> getColorsByProductId(int productId)
        {
            var colors = new List<Color>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT C.ColorID, C.ColorName " +
                            "FROM Color C " +
                            "INNER JOIN Product_Color Pc ON C.ColorID = Pc.ColorID " +
                            "WHERE Pc.ProductCode = @ProductId"; // Ensure the correct ProductCode is used
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", productId);

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

            return colors; // Return the list of colors for the product
        }



    }

    
}