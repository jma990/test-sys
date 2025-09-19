using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Content_Management_System.Utilities;
using Content_Management_System.Data;

namespace Content_Management_System.PageFilters
{
    public class AuthPageFilter : IAsyncPageFilter
    {
        private readonly bool _requireCleanPassword;

        public AuthPageFilter(bool requireCleanPassword = true)
        {
            _requireCleanPassword = requireCleanPassword;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            var path = context.ActionDescriptor.RelativePath?
                .Replace(".cshtml", "")
                .Replace("Pages", "")
                .TrimStart('/');
            var normalizedPath = "/" + path;

            // Not logged in
            if (!user.IsLoggedIn())
            {
                context.Result = new RedirectToPageResult(PathDirectory.ErrorPage);
                return Task.CompletedTask;
            }

            // Logged in but must change password
            if (_requireCleanPassword && user.MustChangePassword())
            {
                // Only allow the mandatory password change page
                if (normalizedPath != PathDirectory.MandatoryPasswordChangePage)
                {
                    context.Result = new RedirectToPageResult(PathDirectory.MandatoryPasswordChangePage);
                    return Task.CompletedTask;
                }
            }

            // Check role permissions
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (!IsPageAllowedForRole(path, role))
            {
                context.Result = new RedirectToPageResult(PathDirectory.ErrorPage);
                return Task.CompletedTask;
            }

            return next();
        }

        private bool IsPageAllowedForRole(string? path, string? role)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(role))
                return false;
 
            var normalizedPath = "/" + path;

            switch (role)
            {
                case nameof(UserRole.SuperAdmin):
                    return normalizedPath == PathDirectory.AccountCreationPage ||
                        normalizedPath == PathDirectory.DepartmentsPage;

                case nameof(UserRole.Admin):
                    return normalizedPath == PathDirectory.AnnouncementsPage ||
                        normalizedPath == PathDirectory.AccountCreationPage ||
                        normalizedPath == PathDirectory.AdminPostsPage ||
                        normalizedPath == PathDirectory.DepartmentsPage;

                case nameof(UserRole.Member):
                    return normalizedPath == PathDirectory.AnnouncementsPage;

                default:
                    return false;
            }
        }

    }
}
