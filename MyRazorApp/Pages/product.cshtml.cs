using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using MyRazorApp.Pages;  // Make sure this is added to the file

namespace MyRazorApp.Pages
{
    public class ProductModel : PageModel
    {
        public List<Product> Products { get; set; }
        [BindProperty]
        public Product Product { get; set; }

        public void OnGet()
        {
            // Populate Products list (this can be replaced by data from your database)
           Products = new List<Product>
{
    new Product { Id = 1, Name = "Shirt", StockQuantity = 100, UnitPrice = 20 },
    new Product { Id = 2, Name = "Pants", StockQuantity = 50, UnitPrice = 40 }
};


        }

        public IActionResult OnPost()
        {
            if (Request.Form["action"] == "delete")
            {
                // Handle delete logic
                var productId = int.Parse(Request.Form["ProductId"]);
                DeleteProduct(productId);
            }
            else
            {
                // Handle add or update logic
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
            // Delete the product by ID
            DeleteProduct(id);
            return RedirectToPage();
        }

        private void AddProduct(Product product)
        {
            // Add product logic (e.g., save to database)
        }

        private void UpdateProduct(Product product)
        {
            // Update product logic (e.g., update the database)
        }

        private void DeleteProduct(int id)
        {
            // Delete product logic (e.g., remove from database)
        }
    }

}
