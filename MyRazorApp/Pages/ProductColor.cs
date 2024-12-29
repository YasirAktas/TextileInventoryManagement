using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Pages
{
    public class ProductColor
    {

        public int ProductColorID { get; set; } // Primary key


        public int ProductCode { get; set; } // Foreign key to Product


        public int ColorID { get; set; } // Foreign key to Color


        public int Quantity { get; set; } // Quantity validation


        public decimal PricePerUnit { get; set; } // Price validation

    }
}