using Vestigio.Models;
namespace Vestigio.DTO_s
{
    public class ProductFilterDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinLevel { get; set; }
        public int? MaxLevel { get; set; }
        public List<int> Categories { get; set; }
        public List<string> Sizes { get; set; }
        public string Sort { get; set; } = "price_asc";
    }
}
