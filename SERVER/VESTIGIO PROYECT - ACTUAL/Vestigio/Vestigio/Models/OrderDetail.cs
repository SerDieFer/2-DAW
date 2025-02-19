using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vestigio.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // RELATION WITH ORDER
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // RELATION WITH PRODUCT
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // RELATION WITH PRODUCT SIZE
        [Required]
        public int ProductSizeId { get; set; }
        public ProductSize ProductSize { get; set; }

    }
}
