using Vestigio.Models;
namespace Vestigio.DTO_s
{
    public class ProductResponseDto
    {
        public IEnumerable<ProductDto> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<ProductSizeDto> Sizes { get; set; }
    } 
}
