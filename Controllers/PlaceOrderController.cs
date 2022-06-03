using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Models.DTO;
using RestaurantManagementSystem.Services;
using System.Collections.Generic;
using System.Net;

namespace RestaurantManagementSystem.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        public PlaceOrderController(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }
        [Authorize(Roles = "user")]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(PlaceOrderDTO [] order)
        {
           /* var cost = 0;*/
            string id = _userService.Id();
          /*  var userOrderInfo = new List<UserOrderInfo>();*/
            var orderHistory=new List<OrderHistory>();
          
           
            UserOrderInfo userOrderInfo = new UserOrderInfo() { UserId = id};
            foreach (var item in order)
            {
               
                var dish = await _db.DishCreate.Where(t => t.Id == item.DishId).FirstOrDefaultAsync();
                if (dish == null)
                {
                    return BadRequest(new { message="Dish with id " + item.DishId + " not found" });
                }
               
              
             }
           

            /*  await _db.Dishes.AddRangeAsync(dishorders);*/

            await _db.userOrderInfos.AddAsync(userOrderInfo);
            await _db.SaveChangesAsync();
            /*var orderid = 0;
            if (_db.userOrderInfos.Count() > 0)
            {
                orderid = _db.userOrderInfos.Max(p => p.OrderId) ;
            }*/

            foreach (var item in order)
            {

                var dish = await _db.DishCreate.Where(t => t.Id == item.DishId).FirstOrDefaultAsync();
                orderHistory.Add(new OrderHistory
                {
                    DishName = dish.DishName,
                    DishCategory = dish.DishCategory,
                    Dishprice = dish.price,

                    DishQuantity = item.DishQuantity,
                    CreatedAt = DateTime.Now,
                    OrderId = userOrderInfo.OrderId,


                });


            }
            await _db.OrderHistories.AddRangeAsync(orderHistory);
            await _db.SaveChangesAsync();

            return Ok("order placed sucessfully");

        }

    }
}
