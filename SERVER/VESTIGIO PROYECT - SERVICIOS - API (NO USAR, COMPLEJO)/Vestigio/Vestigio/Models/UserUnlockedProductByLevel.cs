using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class UserUnlockedProductByLevel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        [Range(1, 10)]
        public int Level { get; set; }

        public DateTime UnlockedDate { get; set; } = DateTime.UtcNow;
    }
}
