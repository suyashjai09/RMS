using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

     /*  public DbSet<Dish>Dishes { get; set; }*/

        public DbSet<DishCreate> DishCreate { get; set; }

        public DbSet<UserOrderInfo> userOrderInfos { get; set; }

        public DbSet<OrderHistory> OrderHistories { get; set; } 

       


    }
}

