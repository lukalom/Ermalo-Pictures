using Microsoft.AspNetCore.Authorization;

namespace EP.API.Security.Requirements
{
    public class UserBlockedStatusRequirement : IAuthorizationRequirement
    {
        public bool IsBlocked { get; set; }

        public UserBlockedStatusRequirement(bool isBlocked)
        {
            IsBlocked = isBlocked;
        }

    }
}
