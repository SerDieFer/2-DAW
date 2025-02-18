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

        [Display(Name = "Name")]
        [Required(ErrorMessage = "The product name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The name must have betweeen 3 and 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The description is obligatory")]
        [StringLength(500, ErrorMessage = "It musn't have more than 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The prise should be bigger than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Rarity Level")]
        [Range(1, 10, ErrorMessage = "The level must be between 1 y 10")]
        public int RarityLevel { get; set; }

            // DYNAMICALLY DISPLAYS THE NAME OF THE RARITY LEVEL
            public string RarityName => LevelsNaming.GetLevelName(RarityLevel);

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        // LINKED CATEGORIES, CHALLENGES, ORDER DETAILS, IMAGES, ETC
        // ------------------------------------------------------------------------------------------------------ //

        // RELATIONSHIP WITH SIZES (1:N)
        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

        // METHOD TO GET THE PRODUCT'S TOTAL STOCK FROM ALL SIZES
        public int TotalStock => ProductSizes.Sum(s => s.Stock);

        // METHOD TO ADD SIZE WITH STOCK
        public void AddSize(string size, int stock)
        {
            if (ClothingSizes.Sizes.ContainsKey(size))
            {
                ProductSizes.Add(new ProductSize { Size = size, Stock = stock });
            }
            else
            {
                throw new ArgumentException($"Invalid size: {size}");
            }
        }

        // METHOD TO UPDATE STOCK FOR A SPECIFIC SIZE
        public void UpdateSizeStock(string size, int quantity)
        {
            var productSize = ProductSizes.FirstOrDefault(s => s.Size == size);
            if (productSize != null)
            {
                productSize.UpdateStock(quantity);
            }
            else
            {
                throw new ArgumentException($"Size not found: {size}");
            }
        }

        // RELATIONSHIP WITH CATEGORIES (MANY-TO-MANY)
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        [Display(Name = "Order Details")]
        public ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();

        [Display(Name = "Product Images")]
        public ICollection<Image>? Images { get; set; } = new List<Image>();

        [Display(Name = "Associated Challenges")]
        public ICollection<Challenge>? Challenges { get; set; } = new List<Challenge>();
    }
}
