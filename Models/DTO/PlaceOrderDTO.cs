using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models.DTO
{
    public class PlaceOrderDTO
    {
        [Required]
        public int DishId { get; set; }
        [Required]
        public int DishQuantity { get; set; }

    }
}
