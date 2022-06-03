using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RestaurantManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController:ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext db, IConfiguration configuration,
                    IUserService userService,
                    UserManager<IdentityUser> userManager,
                    RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _configuration = configuration;
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AdminDTO admin)
        {
            if (ModelState.IsValid)
            {

                // We can utilise the model
                var existingUser = await _userManager.FindByEmailAsync(admin.Email);

                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        Errors = new List<string>() {
                                "Email already in use"
                            },
                        Success = false
                    });
                }

                var newUser = new IdentityUser() { Email = admin.Email, UserName = admin.Email };
                var isCreated = await _userManager.CreateAsync(newUser, admin.Password);
                if (isCreated.Succeeded)
                {
                   /* if (!roleManager.RoleExists(Role.Name))
                    {
                        _context.Roles.Add(Role);
                        _context.SaveChanges(); //error points here
                        return RedirectToAction("Index");
                    }*/
                    var role = new IdentityRole();
                    role.Name = "admin";
                    await _roleManager.CreateAsync(role);
                    await _userManager.AddToRoleAsync(newUser, "admin");
                    var jwtToken = CreateToken(newUser);

                    return Ok(new
                    {
                        Success = true,
                        Token = jwtToken
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                        Success = false
                    });
                }
            }

            return BadRequest(new
            {
                Errors = new List<string>() {
                        "Invalid payload"
                    },
                Success = false
            });


        }

        [HttpPost("login")]

        public async Task<IActionResult> login(AdminDTO admin)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(admin.Email);

                if (existingUser == null)
                {
                    return BadRequest(new
                    {
                        Errors = new List<string>() {
                                "Invalid login request"
                            },
                        Success = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, admin.Password);

                if (!isCorrect)
                {
                    return BadRequest(new
                    {
                        Errors = new List<string>() {
                                "Invalid login request"
                            },
                        Success = false
                    });
                }

                string jwtToken = CreateToken(existingUser);
            
                return Ok(new
                {
                    Success = true,
                    Token = jwtToken
                });
            }

            return BadRequest(new
            {
                Errors = new List<string>() {
                        "Invalid payload"
                    },
                Success = false
            });
        }

        //[HttpGet("get-id"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> Id()
        //{

        //    return Ok(new { id = _userService.Id() });
        //}
        //[HttpGet("get-id"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> Id()
        //{
        //    var id = _userService.Id();
        //    var user = await _userManager.FindByIdAsync(id);
        //    var roles = await _userManager.GetRolesAsync(user);

        //    return OK(new { User = user, Roles = roles });
        //}

       

        private string CreateToken(IdentityUser user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim("email", user.Email),
                new Claim("id", user.Id),
                new Claim("role", "admin")

            };

            Console.WriteLine(claims);
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Secret-Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
