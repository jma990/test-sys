using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Content_Management_System.Data;
using Content_Management_System.Utilities;
using Content_Management_System.PageFilters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers(); 
builder.Services.AddSession();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<SuperAdminService>();
builder.Services.AddScoped<AuthPageFilter>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = PathDirectory.LoginPage;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // If user must change password, allow redirect to that page instead of login
                if (context.HttpContext.User.HasClaim(c => c.Type == "MustChangePassword" && c.Value == "True"))
                {
                    context.Response.Redirect(PathDirectory.MandatoryPasswordChangePage);
                }
                else
                {
                    context.Response.Redirect(PathDirectory.LoginPage);
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";  
});

var app = builder.Build();

// Create the two only super admins if none exists
using (var scope = app.Services.CreateScope())
{
    var superAdminService = scope.ServiceProvider.GetRequiredService<SuperAdminService>();
    await superAdminService.CreateSuperAdmin();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); 

// Note: This shall be changed if actual hosting is made other than NGROK
var ngrokMode = Environment.GetEnvironmentVariable("USE_NGROK");
if (ngrokMode == "true")
{
    app.Run("http://0.0.0.0:5282");
}
else
{
    app.Run();
}


