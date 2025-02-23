using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Vestigio.Utilities;

namespace Vestigio.Models
{
    public class User : IdentityUser
    {

        public string? Nickname { get; set; }

        [Display(Name = "Level")]
        [Range(1, int.MaxValue, ErrorMessage = "Level must be at least 1.")]
        public int Level { get; set; } = 1;

        [Display(Name = "Experience Points")]
        [Range(0, int.MaxValue, ErrorMessage = "Experience points cannot be negative.")]
        public int ExpPoints { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public double LevelProgress { get; set; } = 0;

        [Display(Name = "Vestigios")]
        [Range(0, int.MaxValue, ErrorMessage = "Coins cannot be negative.")]
        public int Coins { get; set; } = 0;

        [Display(Name = "Enrollment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // PERSONAL INFO
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "ID")]
        [StringLength(9, MinimumLength = 9)]
        [RegularExpression(@"^((([A-Za-z])\d{8})|(\d{8}([A-Za-z])))$", ErrorMessage = "ID must contain a letter at the or at the beginning")]
        public string? DNI { get; set; }

        [Display(Name = "Adress")]
        public string? Address { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "Postal Code")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Postal code must be a 5-digit number")]
        public string? PostalCode { get; set; }


        // RELATIONS
        [Display(Name = "Orders")]
        public ICollection<Order>? Orders { get; set; }

        [Display(Name = "Challenges Solved")]
        public ICollection<ChallengeResolution>? ChallengesResolutions { get; set; } = new List<ChallengeResolution>();


        // PRODUCT UNLOCKING RELATIONS
        public ICollection<UserUnlockedProduct> UnlockedProducts { get; set; } = new List<UserUnlockedProduct>();
        public ICollection<UserUnlockedProductByLevel> UnlockedProductLevels { get; set; } = new List<UserUnlockedProductByLevel>();

        // Este método se encarga de actualizar la EXP y el nivel
        public void GainExp(int expGained)
        {
            ExpPoints += expGained;

            // Calcular el nuevo nivel
            int newLevel = 1;
            while (LevelsNaming.GetExpRequiredForLevel(newLevel + 1) <= ExpPoints)
            {
                newLevel++;
            }
            Level = newLevel;

            // Calcular el progreso del nivel en función de la experiencia total acumulada
            int nextLevelExp = LevelsNaming.GetExpRequiredForLevel(Level + 1);

            if (nextLevelExp > 0) // Si hay un siguiente nivel
            {
                LevelProgress = (double)ExpPoints / nextLevelExp * 100;
                LevelProgress = Math.Max(0, Math.Min(100, LevelProgress)); // Asegurar que esté entre 0% y 100%
            }
            else
            {
                // Si no hay un siguiente nivel (nivel máximo alcanzado)
                LevelProgress = 100;
            }
        }
    }
}
