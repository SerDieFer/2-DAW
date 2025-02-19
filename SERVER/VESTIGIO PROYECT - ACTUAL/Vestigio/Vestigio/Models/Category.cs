using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The category name is required.")]
        public string Name { get; set; }

        // RELATIONSHIP WITH PRODUCTS (MANY-TO-MANY)
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
