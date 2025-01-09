using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class Challenge
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The challenge title is required.")]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The solution is required.")]
        public string Solution { get; set; }

        [Required]
        [Display(Name = "Experience Points")]
        [Range(0, int.MaxValue, ErrorMessage = "Experience points cannot be negative.")]
        public int ExpPoints { get; set; }

        [Required]
        [Display(Name = "Vestigios")]
        [Range(0, int.MaxValue, ErrorMessage = "Coins cannot be negative.")]
        public int Coins { get; set; }

        [Required]
        [Display(Name = "Rarity Level")]
        public int RarityLevel { get; set; }

        [Required]
        public bool Active { get; set; }

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }


        // SPECIFIC PRODUCT OR PRODUCTS BY RARITY LEVEL
        [Display(Name = "Product ID (if specific)")]
        public int? ProductId { get; set; }

        [Display(Name = "Product Level (if by rarity)")]
        public int? ProductLevel { get; set; }

        public Product? Product { get; set; }
    }
}
