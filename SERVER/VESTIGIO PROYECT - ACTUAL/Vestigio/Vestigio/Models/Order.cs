using System.ComponentModel.DataAnnotations;

namespace Vestigio.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        // RELATION WITH ORDER STATUS
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        // RELATION WITH USER
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        // RELATION WITH ORDER DETAILS
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
