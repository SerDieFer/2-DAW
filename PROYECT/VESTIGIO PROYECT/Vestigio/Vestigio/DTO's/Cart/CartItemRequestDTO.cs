using System.ComponentModel.DataAnnotations;

namespace Vestigio.DTO_s
{
    public class CartItemRequestDto
    {
        [Required]
        public int ProductSizeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }
}
