using System.ComponentModel.DataAnnotations;
using Vestigio.Utilities;

namespace Vestigio.Models
{
    public class Challenge : IValidatableObject
    {

        // DEFAULT VALIDATION
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        // CHALLENGE INFO
        // ------------------------------------------------------------------------------------------------------ //

        public int Id { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "The challenge title is required.")]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Experience Points")]
        [Range(0, int.MaxValue, ErrorMessage = "Experience points cannot be negative.")]
        public int ExpPoints { get; set; }

        [Required]
        [Display(Name = "Coins")]
        [Range(0, int.MaxValue, ErrorMessage = "Coins cannot be negative.")]
        public int Coins { get; set; }

        [Required]
        [Display(Name = "Rarity Level")]
        public int RarityLevel { get; set; }

            // PROPERTY TO DISPLAY THE NAME OF THE RARITY LEVEL
            public string RarityName => LevelsNaming.GetLevelName(RarityLevel);

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        // LINKED IMAGES
        // ------------------------------------------------------------------------------------------------------ //

        [Display(Name = "Challenge Images")]
        public ICollection<Image>? Images { get; set; } = new List<Image>(); // 1:N RELATIONSHIP

        // LINKED PRODUCTS OR LEVEL WHICH UNLOCKS THE PRODUCTS OF THAT LEVEL
        // ------------------------------------------------------------------------------------------------------ //

        [Display(Name = "Product Level (if by rarity)")]
        public int? ProductLevel { get; set; }

        [Display(Name = "Product ID (if specific)")]
        public int? ProductId { get; set; }

            public Product? Product { get; set; }

        // VALIDATION FOR PRODUCT TYPE MODE SELECTION
        public IEnumerable<ValidationResult> ValidateAffectedProducts(ValidationContext validationContext)
        {
            if (!ProductLevel.HasValue && !ProductId.HasValue)
            {
                yield return new ValidationResult("Either Product Level or Product ID must be specified.", new[] { nameof(ProductLevel), nameof(ProductId) });
            }

            if (ProductLevel.HasValue && ProductId.HasValue)
            {
                yield return new ValidationResult("Only one of Product Level or Product ID can be specified.", new[] { nameof(ProductLevel), nameof(ProductId) });
            }
        }

        // SOLUTION
        // ------------------------------------------------------------------------------------------------------ //

        [Required]
        [Display(Name = "Solution Mode")]
        public SolutionMode SolutionMode { get; set; } // USES ENUM TO DEFINE MODE

            // VALIDATION FOR SOLUTION MODE ENUM SELECTION
            public IEnumerable<ValidationResult> ValidateEnum(ValidationContext validationContext)
            {
                if (!Enum.IsDefined(typeof(SolutionMode), SolutionMode))
                {
                    yield return new ValidationResult("Invalid solution mode.", new[] { nameof(SolutionMode) });
                }
            }

        [Display(Name = "Password (for password-based challenges)")]
        public string? Password { get; set; }

        [Display(Name = "Release Date (for time-based challenges)")]
        [DataType(DataType.DateTime)]
        public DateTime? ReleaseDate { get; set; }

            // INDICATES WHETHER THE CHALLENGE IS PUBLIC BASED ON THE RELEASE DATE
            public bool IsPublic => SolutionMode == SolutionMode.TimeBased && ReleaseDate.HasValue && DateTime.UtcNow >= ReleaseDate;

        // VALIDATION FOR SOLUTION MODE SELECTION
        public IEnumerable<ValidationResult> ValidateSolutionMode(ValidationContext validationContext)
        {
            if (SolutionMode == SolutionMode.Password && string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Password is required for password-based challenges.", new[] { nameof(Password) });
            }

            if (SolutionMode == SolutionMode.TimeBased && !ReleaseDate.HasValue)
            {
                yield return new ValidationResult("Release date is required for time-based challenges.", new[] { nameof(ReleaseDate) });
            }
        }
    }

    // ENUM FOR SOLUTION MODES
    public enum SolutionMode
    {
        Password,
        TimeBased
    }
}
