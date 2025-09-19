using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;  
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Content_Management_System.Data;
using Content_Management_System.Utilities;

namespace Content_Management_System.Pages;

public class IndexModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.HasClaim(c => c.Type == "MustChangePassword" && c.Value == "True"))
                return RedirectToPage(PathDirectory.MandatoryPasswordChangePage);

            return RedirectToPage(PathDirectory.AnnouncementsPage);
        }

        return RedirectToPage(PathDirectory.LoginPage);
    }

}
