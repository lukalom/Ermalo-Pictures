using EP.API.Security.Requirements;
using EP.Shared;
using EP.Shared.Configuration;
using EP.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace EP.API.Security.Handlers
{
    public class UserBlockedStatusHandler : AuthorizationHandler<UserBlockedStatusRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserBlockedStatusRequirement requirement)
        {

            if (!context.User.HasClaim(x => x.Type == "isBlocked" && x.Issuer == JwtConfig.Issuer))
            {
                return Task.CompletedTask;
            }

            var value = context.User
                .FindFirst(x => x.Type == "isBlocked" && x.Issuer == JwtConfig.Issuer).Value;

            var userBlockedStatus = Convert.ToBoolean(value);

            if (userBlockedStatus == requirement.IsBlocked)
            {
                context.Succeed(requirement);
            }
            else
            {
                throw new AppException("You Are Blocked ;)");
            }

            return Task.CompletedTask;
        }
    }
}