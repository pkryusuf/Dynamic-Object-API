using System.ComponentModel.DataAnnotations;

namespace CRUD.API.Models
{
    public class Customer
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }


}
