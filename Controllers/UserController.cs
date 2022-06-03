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
    public class UserController:ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
       

        public UserController(
                    ApplicationDbContext db,
                    IConfiguration configuration,
                    IUserService userService,
                    UserManager<IdentityUser> userManager,
                    RoleManager<IdentityRole> roleManager
                )
        {
            _db = db;
            _configuration = configuration;
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //public async Task CreateUserRole()
        //{
        //    var role = new IdentityRole();
        //    role.Name = "user";
        //    await _roleManager.CreateAsync(role);

        //}


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            if (ModelState.IsValid)
            {
               
                // We can utilise the model
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

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

                var newUser = new IdentityUser() { Email = user.Email, UserName = user.Email };
                var isCreated = await _userManager.CreateAsync(newUser, user.Password);
                if (isCreated.Succeeded)
                {

                    var role = new IdentityRole();
                    role.Name = "user";
                    await _roleManager.CreateAsync(role);
                    await _userManager.AddToRoleAsync(newUser,"user");
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

        public async Task<IActionResult> Login(UserDTO user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

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

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

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
                Console.WriteLine($"token {jwtToken}");
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
        



        private string CreateToken(IdentityUser user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim("email", user.Email),
                new Claim("id", user.Id),
                new Claim("role","user"),
               
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
