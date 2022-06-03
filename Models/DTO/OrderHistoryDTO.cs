namespace RestaurantManagementSystem.Models.DTO
{
    public class OrderHistoryDTO
    {
        public int OrderId { get; set; }
        public string DishName { get; set; }

        public int DishQuantity { get; set; }
        public int Dishprice { get; set; }

        public DateTime CreatedAt { get; set; }


    }

}
