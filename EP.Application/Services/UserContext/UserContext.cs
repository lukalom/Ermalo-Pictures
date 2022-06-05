using EP.Application.Extensions;
using Microsoft.AspNetCore.Http;

namespace EP.Application.Services.UserContext
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                UserId = Guid.Parse(httpContextAccessor.HttpContext.User.GetUserId());
                Email = httpContextAccessor.HttpContext.User.GetEmail();
            }
        }

        public Guid? UserId { get; init; }
        public string Email { get; init; }
    }
}
