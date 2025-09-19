using System.Security.Claims;

namespace Content_Management_System.Utilities
{
    public static class UserExtensions
    {
        public static bool MustChangePassword(this ClaimsPrincipal user)
        {
            return user.FindFirst("MustChangePassword")?.Value == "true";
        }

        public static bool IsLoggedIn(this ClaimsPrincipal user)
        {
            return user.Identity?.IsAuthenticated ?? false;
        }
    }
}
