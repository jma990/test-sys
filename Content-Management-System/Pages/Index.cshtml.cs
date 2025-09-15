using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Content_Management_System.Data;

namespace Content_Management_System.Pages;

public class IndexModel : PageModel
{
    public IndexModel()
    {
    }
    public IActionResult OnGet()
    {
        return RedirectToPage(PathDirectory.LoginPage);
    }
}
