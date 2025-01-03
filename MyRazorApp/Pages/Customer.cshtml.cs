using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MyRazorApp.Pages
{
    public class CustomerModel : PageModel
    {
        public List<Customer> Customers { get; set; }
        [BindProperty]
        public Customer Customer { get; set; }

        private readonly string connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

        public void OnGet()
        {
            Customers = GetCustomersFromDatabase();
        }

        public IActionResult OnPost()
        {
            if (Request.Form["action"] == "delete")
            {
                var customerId = int.Parse(Request.Form["CustomerId"]);
                DeleteCustomer(customerId);
            }
            else
            {
                if (Customer.CustomerID == 0)
                {
                    AddCustomer(Customer);
                }
            }

            return RedirectToPage();
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

       private void AddCustomer(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string procedureName = "AddCustomer"; // Name of the stored procedure

                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters for the stored procedure
                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);

                    // Open the connection and execute the procedure
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void DeleteCustomer(int customerId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Customer WHERE CustomerID = @CustomerID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }



    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
