using System.ComponentModel.DataAnnotations;

namespace CRUD.API.Models
{
    public class OrderProduct
    {
        [Required]
        public int OrderProductId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public Order Order { get; set; }
        [Required]
        public Product Product { get; set; }

    }

}
