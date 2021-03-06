using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models.DTO;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ViewOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        public ViewOrderController(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }
        [Authorize(Roles = "user")]
        [HttpGet]
        public async Task <IActionResult> viewOrder()
        {
            var orderHistoryDTO = new List<OrderHistoryDTO>();

            List<Object> list = new List<object>();
            var id = _userService.Id();
            var res=await _db.userOrderInfos.Where(t => t.UserId == id).ToListAsync();
            if(res.Count==0)
                return BadRequest("No order found against the user");
           
            foreach (var info in res)
            {
                var data = await _db.OrderHistories.Where(t => t.OrderId == info.OrderId)
                    .Select(x => new { x.OrderId, x.DishName,x.DishQuantity,x.Dishprice,x.CreatedAt }).ToListAsync();
                foreach (var item in data)
                {
                    orderHistoryDTO.Add(new OrderHistoryDTO
                    {
                        OrderId = item.OrderId,
                        DishName = item.DishName,
                        DishQuantity = item.DishQuantity,
                        Dishprice = item.Dishprice,
                        CreatedAt = item.CreatedAt
                    });
                   

                }
                var groupedBlogs = data.GroupBy(t => t.OrderId).ToList();
                list.AddRange(groupedBlogs);
               

            }
            return Ok(new { data=list});
        }
       
    }
}
