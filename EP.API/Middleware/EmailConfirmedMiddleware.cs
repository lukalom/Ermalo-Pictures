using EP.Application.Services.Account.Auth;
using EP.Infrastructure.Entities;
using EP.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace EP.API.Middleware
{
    public class EmailConfirmedMiddleware
    {
        private readonly RequestDelegate _next;

        public EmailConfirmedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            var user = await userManager.FindByIdAsync(context.User.FindFirst("Id")?.Value);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    if (!string.IsNullOrEmpty(emailConfirmToken))
                    {
                        await authService.SendEmailConfirmation(user, emailConfirmToken);
                    }
                    throw new AppException("Your Account is not verified pls check email and verify");
                }
            }

            await _next(context);
        }
    }
}
