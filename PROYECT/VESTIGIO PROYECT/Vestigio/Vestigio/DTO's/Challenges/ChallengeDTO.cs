using Vestigio.Models;

namespace Vestigio.DTO_s
{
 public class ChallengeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int RarityLevel { get; set; }
        public int ExpPoints { get; set; }
        public int Coins { get; set; }
        public bool IsSolved { get; set; }

        public ChallengeDto(Challenge challenge, bool isSolved)
        {
            Id = challenge.Id;
            Title = challenge.Title;
            RarityLevel = challenge.RarityLevel;
            ExpPoints = challenge.ExpPoints;
            Coins = challenge.Coins;
            IsSolved = isSolved;
        }
    }
}
