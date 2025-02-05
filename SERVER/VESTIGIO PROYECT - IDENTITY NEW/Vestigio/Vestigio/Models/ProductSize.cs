using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.ComponentModel.DataAnnotations;
using Vestigio.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Vestigio.Models
{
    public class ProductSize
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Size is required")]
        public string Size { get; set; }

        // METHOD TO CHECK IF SELECTED SIZE IS VALID
        public bool IsValidSize()
        {
            return ClothingSizes.Sizes.ContainsKey(Size);
        }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        // METHOD TO UPDATE STOCK AND DEACTIVATE IF NECESSARY
        public void UpdateStock(int quantity)
        {
            Stock += quantity;
            if (Stock <= 0)
            {
                Stock = 0; // ENSURE STOCK DOES NOT GO NEGATIVE
                IsActive = false; // DEACTIVATE SIZE
            }
            else
            {
                IsActive = true; // REACTIVATE SIZE IF STOCK IS RESTORED
            }
        }
        // AUTOMATICALLY DEACTIVATE SIZE WHEN STOCK REACHES 0
        public bool IsActive { get; set; } = true;

        // RELATIONSHIP WITH PRODUCT
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // RELATIONSHIP WITH ORDER DETAILS
        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
