using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models
{
    public class Dish
    {
        public int Id { get; set; }

        [Required]
        public string DishName { get; set; }

        [Required]
        public string DishCategory { get; set; }
        [Required]
        public int price { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
