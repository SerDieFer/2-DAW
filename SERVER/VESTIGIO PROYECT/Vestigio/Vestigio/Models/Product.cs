using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The product name is required.")]
        public string Name { get; set; }


        [Display(Name = "Description")]
        public string? Description { get; set; }


        [Required(ErrorMessage = "The price is required.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "The stock amount is required.")]
        public int Stock { get; set; }

        [Required]
        [Display(Name = "Rarity Level")]
        public int RarityLevel { get; set; }

        // RELATIONS
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }


        [Display(Name = "Order Details")]
        public ICollection<OrderDetail>? OrderDetails { get; set; }

        public ICollection<Image>? Images { get; set; }
    }
}
