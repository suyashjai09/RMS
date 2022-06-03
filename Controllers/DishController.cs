using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Models.DTO;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DishController:ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public DishController(ApplicationDbContext db, IUserService userService,
                    UserManager<IdentityUser> userManager)
        {

            _db = db;
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllDishes()
        {

            List<DishCreate>dishes = await _db.DishCreate.ToListAsync();
            return Ok(new { data = dishes });

        }

        [HttpPost("create-dish")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateDish([FromBody] DishCreateDTO dishitem)
        {
           
            DishCreate dish = new DishCreate() { DishName = dishitem.DishName, DishCategory = dishitem.DishCategory, price = dishitem.price };
            await _db.DishCreate.AddAsync(dish);
            await _db.SaveChangesAsync();
            return StatusCode(201, "dish is created successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish=await _db.DishCreate.Where(t=>t.Id == id).FirstOrDefaultAsync();
            if(dish==null)
            {
                return BadRequest("Dish not found");
            }
            else
            {
                _db.DishCreate.Remove(dish);
                await _db.SaveChangesAsync();
                return Ok(new { msg = "Dish removed succesfully" });
            }

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDish(int id, [FromBody] DishCreate item)
        {
            var dish = await _db.DishCreate.Where(t => t.Id == id).FirstOrDefaultAsync();
            if (dish == null)
            {
                return BadRequest("Dish not found");
            }
            else
            {
                if (item.DishName != null) { dish.DishName = item.DishName.Trim(); }
                if (item.DishCategory != null) { dish.DishCategory = item.DishCategory.Trim(); }
                if (item.price > 0) { dish.price = item.price; }

                await _db.SaveChangesAsync();
                return Ok(new { msg = "Dish updated succesfully" });
            }

        }



    }
}
