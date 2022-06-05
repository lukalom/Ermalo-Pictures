using EP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace EP.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst("Id");
                if (userId != null)
                {
                    context.Items["User"] = await userManager.GetUserAsync(context.User);
                }
                else
                {
                    throw new ApplicationException("Invalid User");
                }

            }

            //Pass to the next middleware
            await _next(context);
        }

    }
}
