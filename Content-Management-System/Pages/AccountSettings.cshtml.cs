using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Content_Management_System.Data;
using Content_Management_System.Utilities;
using Content_Management_System.PageFilters;

namespace Content_Management_System.Pages
{
    public class AccountSettingsModel(AppDbContext db) : PageModel
    {
        private readonly AppDbContext _db = db;

        
    }
}
