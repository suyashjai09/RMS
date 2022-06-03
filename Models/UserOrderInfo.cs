using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models
{
    public class UserOrderInfo
    {
        [Key]
        public int OrderId { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }
    }
}
