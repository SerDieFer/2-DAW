using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class User : IdentityUser
    {
        public string? Nickname { get; set; }

        [Display(Name = "Level")]
        [Range(1, int.MaxValue, ErrorMessage = "Level must be at least 1.")]
        public int Level { get; set; }

        [Display(Name = "Experience Points")]
        [Range(0, int.MaxValue, ErrorMessage = "Experience points cannot be negative.")]
        public int ExpPoints { get; set; }

        [Display(Name = "Vestigios")]
        [Range(0, int.MaxValue, ErrorMessage = "Coins cannot be negative.")]
        public int Coins { get; set; }

        [Display(Name = "Enrollment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }


        // RELATIONS
        [Display(Name = "Orders")]
        public ICollection<Order>? Orders { get; set; }

        [Display(Name = "Challenges Solved")]
        public ICollection<ChallengeResolution>? ChallengesResolutions { get; set; }

    }
}
