using System.Security.Claims;

namespace EP.Application.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetEmail(this ClaimsPrincipal? user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(x => x.Type == "Id")?.Value;
        }
    }
}
