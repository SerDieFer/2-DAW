using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The category name is required.")]
        public string Name { get; set; }

        // RELATIONS
        [Display(Name = "Products")]
        public ICollection<Product>? Products { get; set; }
    }
}
