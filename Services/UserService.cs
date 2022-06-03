using System.Security.Claims;

namespace RestaurantManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string Id()
        {
            string Id = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                Id = _httpContextAccessor.HttpContext.User.FindFirstValue("id");
            }
            return Id;

        }


    }
}
