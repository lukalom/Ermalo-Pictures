using Microsoft.AspNetCore.Authorization;

namespace EP.API.Security.Handlers
{

    public class AccessShowManagementRequirement : IAuthorizationRequirement
    { }

    public class ShowManagementHandler : AuthorizationHandler<AccessShowManagementRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccessShowManagementRequirement requirement)
        {
            var user = context.User;
            if (user.HasClaim("Show Management", "Show Management") || user.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
