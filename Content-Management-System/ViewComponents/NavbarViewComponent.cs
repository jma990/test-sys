using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Content_Management_System.Data;
using Content_Management_System.Models.Shared;

namespace Content_Management_System.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public NavbarViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(ClaimsPrincipal user)
        {
            var navbarModel = new NavbarModel(_db);
            await navbarModel.LoadUserDataAsync(user);
            return View(navbarModel);
        }
    }
}
