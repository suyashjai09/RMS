using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models
{
    public class OrderHistory
    {

        public int Id { get; set; }

        [Required]
        public string DishName { get; set; }

        [Required]
        public string DishCategory { get; set; }
        [Required]
        public int Dishprice { get; set; }

        /* public string UserId { get; set; }
         public IdentityUser User { get; set; }*/

        [Required]
        public int DishQuantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public UserOrderInfo UserOrderInfo { get; set; }
    }
}
