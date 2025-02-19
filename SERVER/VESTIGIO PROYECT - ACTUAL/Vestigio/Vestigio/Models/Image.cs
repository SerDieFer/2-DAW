namespace Vestigio.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public int? ChallengeId { get; set; }
        public Challenge Challenge { get; set; }
    }
}
