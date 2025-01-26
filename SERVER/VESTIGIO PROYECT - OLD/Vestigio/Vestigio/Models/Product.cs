using System.ComponentModel.DataAnnotations;
using Vestigio.Utilities;

namespace Vestigio.Models
{
    public class Product
    {

        // PRODUCT INFO
        // ------------------------------------------------------------------------------------------------------ //

        public int Id { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "The product name is required.")]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The price is required.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "The stock amount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        [Required]
        [Display(Name = "Rarity Level")]
        public int RarityLevel { get; set; }

            // DYNAMICALLY DISPLAYS THE NAME OF THE RARITY LEVEL
            public string RarityName => LevelsNaming.GetLevelName(RarityLevel);

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        // LINKED CATEGORIES, CHALLENGES, ORDER DETAILS, IMAGES, ETC
        // ------------------------------------------------------------------------------------------------------ //

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

            public Category? Category { get; set; }

        [Display(Name = "Order Details")]
        public ICollection<OrderDetail>? OrderDetails { get; set; }

        [Display(Name = "Product Images")]
        public ICollection<Image>? Images { get; set; } = new List<Image>(); // 1:N RELATIONSHIP

        [Display(Name = "Associated Challenges")]
        public ICollection<Challenge>? Challenges { get; set; } = new List<Challenge>();
    }
}
