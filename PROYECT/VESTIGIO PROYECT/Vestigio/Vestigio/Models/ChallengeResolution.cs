using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class ChallengeResolution
    {
        public int Id { get; set; }

        [Required]
        public int ChallengeId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Display(Name = "Resolution Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ResolutionDate { get; set; }

        [Required]
        [Display(Name = "Coins Earned")]
        [Range(0, int.MaxValue, ErrorMessage = "Coins cannot be negative.")]
        public int CoinsEarned { get; set; }

        [Required]
        [Display(Name = "Experience Points Earned")]
        [Range(0, int.MaxValue, ErrorMessage = "Experience points cannot be negative.")]
        public int PointsEarned { get; set; }

        // RELATIONS
        public Challenge Challenge { get; set; }
        public User User { get; set; }
    }
}
