using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models
{
    public class DishCreate
    {
        public int Id { get; set; }

        [Required]
        public string DishName { get; set; }

        [Required]
        public string DishCategory { get; set; }
        [Required]
        public int price { get; set; }
    }
}
