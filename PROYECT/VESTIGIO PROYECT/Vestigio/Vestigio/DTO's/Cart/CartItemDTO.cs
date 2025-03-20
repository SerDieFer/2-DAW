using Vestigio.Models;

namespace Vestigio.DTO_s
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public CartItemDto(OrderDetail detail)
        {
            ProductId = detail.ProductId;
            ProductName = detail.Product?.Name ?? "Unknown";
            Size = detail.ProductSize?.Size ?? "N/A";
            Quantity = detail.Quantity;
            Price = detail.Price;
        }
    }
}
