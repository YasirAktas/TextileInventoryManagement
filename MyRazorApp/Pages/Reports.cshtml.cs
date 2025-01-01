using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyRazorApp.Pages
{
    public class ReportsModel : PageModel
    {
        public List<SaleReport> saleReports { get; set; }
        public List<InventoryTransactionReport> transactionReports { get; set; } = new List<InventoryTransactionReport>();

        private readonly string connectionString = "Data Source=Yasir;Database=TIMS;Integrated Security=True;";

        public void OnGet()
        {
            saleReports = GetSaleReportsFromDatabase();  // Corrected the assignment
            transactionReports = GetInventoryTransactionReports();
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
         private List<InventoryTransactionReport> GetInventoryTransactionReports()
        {
            var reports = new List<InventoryTransactionReport>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        ProductCode,
                        ProductName,
                        ColorName,
                        TransactionType,
                        TotalQuantity,
                        LastTransactionDate,
                        StoreroomName,
                        StockLevelAfterTransaction
                    FROM InventoryTransactionReport";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reports.Add(new InventoryTransactionReport
                        {
                            ProductCode = reader.GetString(0),
                            ProductName = reader.GetString(1),
                            ColorName = reader.GetString(2),
                            TransactionType = reader.GetString(3),
                            TotalQuantity = reader.GetInt32(4),
                            LastTransactionDate = reader.GetDateTime(5),
                            StoreroomName = reader.GetString(6),
                            StockLevelAfterTransaction = reader.GetInt32(7),
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
    public class InventoryTransactionReport
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string ColorName { get; set; }
            public string TransactionType { get; set; }
            public int TotalQuantity { get; set; }
            public DateTime LastTransactionDate { get; set; }
            public string StoreroomName { get; set; }
            public int StockLevelAfterTransaction { get; set; }
        }
    
}
