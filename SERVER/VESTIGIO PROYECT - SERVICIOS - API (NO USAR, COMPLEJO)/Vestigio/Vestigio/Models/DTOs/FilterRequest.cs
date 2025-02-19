using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models.DTOs
{
    public class FilterRequest
    {
        // Campos genéricos
        public string ActiveTab { get; set; } = "challenges";


        // Campos de Challenges
        public int? MinLevel { get; set; }
        public int? MaxLevel { get; set; }
        [RegularExpression("Password|TimeBased|^$")]
        public string SolutionType { get; set; } = "";
        public int? MinXP { get; set; }
        public int? MaxXP { get; set; }
        public int? MinCoins { get; set; }
        public int? MaxCoins { get; set; }
        public bool? ShowSolved { get; set; } = false;
        public string ChallengeSort { get; set; } = "level_asc";


        // Campos de Products
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<int> Categories { get; set; } = new List<int>();
        public string ProductSort { get; set; } = "price_asc";


        // Datos del usuario
        public int UserLevel { get; set; } = 1;
        public List<int> SolvedChallenges { get; set; } = new List<int>();
        public List<int> UnlockedProductIds { get; set; } = new List<int>();
        public List<int> UnlockedProductLevels { get; set; } = new List<int>();
    }
}
