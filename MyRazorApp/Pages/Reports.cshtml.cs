using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyRazorApp.Pages
{
    public class ReportsModel : PageModel
    {
        public List<SaleReport> saleReports { get; set; }

        private readonly string connectionString = "Server=127.0.0.1,1433; Database=TIMS; User ID=sa; Password=reallyStrongPwd123; Encrypt=false;";

        public void OnGet()
        {
            saleReports = GetSaleReportsFromDatabase();  // Corrected the assignment
        }

        private List<SaleReport> GetSaleReportsFromDatabase()
        {
            var reports = new List<SaleReport>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductCode, ProductName, TotalSold, TotalSalesValue, Discount, netAmount FROM SalesOverview";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reports.Add(new SaleReport
                        {
                            Id = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            TotalSold = reader.GetInt32(2),
                            TotalSalesValue = reader.GetDecimal(3),
                            Discount = reader.GetDecimal(4),
                            NetAmount = reader.GetDecimal(5)
                        });
                    }
                }
            }

            return reports;
        }
    }

    public class SaleReport
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalSalesValue { get; set; } // Changed to decimal
        public decimal Discount { get; set; } // Changed to decimal
        public decimal NetAmount { get; set; } // Changed to decimal
    }
}
