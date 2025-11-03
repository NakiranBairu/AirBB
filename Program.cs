using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AirBB.Models;
<<<<<<< HEAD
using Microsoft.AspNetCore.DataProtection;
using System.IO;
=======
>>>>>>> 3c1ada2 (chore: commit AirBB project changes)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext - matches the context class name
builder.Services.AddDbContext<AirBBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add session management
builder.Services.AddScoped<ISessionManager, SessionManager>();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

<<<<<<< HEAD
// Persist DataProtection keys to avoid "Error unprotecting the session cookie" when running locally
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")));

=======
>>>>>>> 3c1ada2 (chore: commit AirBB project changes)
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.UseSession();

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    

app.Run();