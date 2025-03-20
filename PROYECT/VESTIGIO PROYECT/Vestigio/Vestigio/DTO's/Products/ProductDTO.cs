using Vestigio.Models;
namespace Vestigio.DTO_s
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int RarityLevel { get; set; }
        public List<string> Images { get; set; }
        public List<ProductSizeDto> Sizes { get; set; }

        public ProductDto(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            RarityLevel = product.RarityLevel;
            Images = product.Images.Select(i => i.Url).ToList();
            Sizes = product.ProductSizes
                .Where(ps => ps.Stock > 0)
                .Select(ps => new ProductSizeDto { Id = ps.Id, Size = ps.Size })
                .ToList();
        }
    }
}
