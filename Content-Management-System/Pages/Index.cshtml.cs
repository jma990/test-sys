using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;  
using Microsoft.AspNetCore.Authentication.Cookies;
using Content_Management_System.Data;

namespace Content_Management_System.Pages;

public class IndexModel : PageModel
{
    public IndexModel()
    {
    }
    public async Task<IActionResult> OnGet()
    {
        // If user is already authenticated, redirect to dashboard
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToPage(PathDirectory.DashboardPage);
        }

        // Check for authentication cookie
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (result.Succeeded && result.Principal != null)
        {
            return RedirectToPage(PathDirectory.DashboardPage);
        }

        return RedirectToPage(PathDirectory.LoginPage);
    }
}
