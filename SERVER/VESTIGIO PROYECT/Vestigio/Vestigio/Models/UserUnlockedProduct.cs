namespace Vestigio.Models
{
    public class UserUnlockedProduct
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime UnlockedDate { get; set; } = DateTime.UtcNow;
    }
}

