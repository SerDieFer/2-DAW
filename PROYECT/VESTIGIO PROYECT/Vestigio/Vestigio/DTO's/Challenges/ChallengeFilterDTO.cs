using Vestigio.Models;

namespace Vestigio.DTO_s
{
    public class ChallengeFilterDto
    {
        public int? MinLevel { get; set; }
        public int? MaxLevel { get; set; }
        public string SolutionType { get; set; }
        public int? MinXP { get; set; }
        public int? MaxXP { get; set; }
        public int? MinCoins { get; set; }
        public int? MaxCoins { get; set; }
        public bool? ShowSolved { get; set; }
        public string Sort { get; set; } = "level_asc";
    }
    
}
