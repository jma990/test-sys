using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Content_Management_System.Data;
using Content_Management_System.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers(); 
builder.Services.AddSession();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<SuperAdminService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = PathDirectory.LoginPage;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
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
app.Run();
