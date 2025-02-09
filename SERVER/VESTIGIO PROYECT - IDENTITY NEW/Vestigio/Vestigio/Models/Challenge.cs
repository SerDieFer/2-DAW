using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using Vestigio.Utilities;

namespace Vestigio.Models
{
    public class Challenge
    {
        // PROPERTIES
        // =====================================================================

        // Basic Info
        public int Id { get; set; }

        [Required(ErrorMessage = "Challenge status is required")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        // Rewards
        [Required(ErrorMessage = "Experience points are required")]
        [Range(0, int.MaxValue, ErrorMessage = "Experience points must be positive")]
        public int ExpPoints { get; set; }

        [Required(ErrorMessage = "Coins are required")]
        [Range(0, int.MaxValue, ErrorMessage = "Coins must be positive")]
        public int Coins { get; set; }

        // Rarity
        [Required(ErrorMessage = "Rarity level is required")]
        [Range(1, 10, ErrorMessage = "Rarity must be between 1-10")]
        public int RarityLevel { get; set; }

        [Display(Name = "Rarity Name")]
        public string RarityName => LevelsNaming.GetLevelName(RarityLevel);

        // Timing
        [Display(Name = "Creation Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        // Media
        public ICollection<Image>? Images { get; set; } = new List<Image>();

        // Associations
        [Display(Name = "Product Level")]
        [Range(1, 10, ErrorMessage = "The level must be between 1 and 10")]
        public int? ProductLevel { get; set; }

        [Display(Name = "Specific Product")]
        public int? ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }

        // Solution Configuration
        [Required(ErrorMessage = "Solution type is required")]
        [Display(Name = "Solution Type")]
        public SolutionMode SolutionMode { get; set; }

        [Display(Name = "Access Password")]
        [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
        public string? Password { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Public Status")]
        public bool IsPublic => SolutionMode == SolutionMode.TimeBased &&
                              ReleaseDate.HasValue &&
                              DateTime.UtcNow >= ReleaseDate.Value.ToUniversalTime();

        [Display(Name = "Resolutions")]
        public ICollection<ChallengeResolution> Resolutions { get; set; } = new List<ChallengeResolution>();
    }

    public enum SolutionMode
    {
        [Display(Name = "Password Protected")]
        Password,

        [Display(Name = "Timed Release")]
        TimeBased
    }
}