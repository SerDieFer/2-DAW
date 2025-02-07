﻿using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The username is required.")]
        public string? Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "The user email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

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
