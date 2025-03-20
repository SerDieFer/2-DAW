using Vestigio.Models;

namespace Vestigio.DTO_s
{
    public class CartDto
    {
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemDto> Items { get; set; }

        public CartDto(Order order)
        {
            TotalItems = order.OrderDetails.Sum(od => od.Quantity);
            TotalPrice = order.OrderDetails.Sum(od => od.Price * od.Quantity);
            Items = order.OrderDetails.Select(od => new CartItemDto(od)).ToList();
        }
    }

}
