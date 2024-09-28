using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CRUD.API.Models
{
    public class Order
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public string Status { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        public Customer Customer { get; set; }

    }

}
