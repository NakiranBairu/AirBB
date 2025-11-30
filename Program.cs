using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AirBB.Models;
using AirBB.Models.DataLayer.Repositories;
using AirBB.Models.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Determine SQLite path: prefer env var, otherwise config, otherwise Azure writable path
string? sqlitePath = Environment.GetEnvironmentVariable("SQLITE_PATH");
if (string.IsNullOrEmpty(sqlitePath))
{
	// Read from configuration (this respects appsettings.Production.json when running in Production)
	sqlitePath = builder.Configuration.GetConnectionString("DefaultConnection"); // typically Data Source=AirBB.db
	if (string.IsNullOrEmpty(sqlitePath))
	{
		// Default fallback for Azure App Service
		sqlitePath = "Data Source=D:/home/data/AirBB.db";
	}
}

// Register DbContext with SQLite
builder.Services.AddDbContext<AirBBContext>(options =>
	options.UseSqlite(sqlitePath));

// Add HttpContextAccessor and session management
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionManager, SessionManager>();
builder.Services.AddScoped<SessionWrapper>();
builder.Services.AddScoped<CookieWrapper>();
builder.Services.AddScoped<IDataSyncService, DataSyncService>();
// Generic repository registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(Repository<>));

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews()
	.AddViewOptions(options =>
	{
		options.HtmlHelperOptions.ClientValidationEnabled = true;
	});

// Configure DataProtection key storage to a writable location on Azure
if (builder.Environment.IsDevelopment())
{
	builder.Services.AddDataProtection()
		.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")));
}
else
{
	var dpPath = Environment.GetEnvironmentVariable("DATAPROTECTION_KEYS_PATH") ?? "D:/home/DataProtection-Keys";
	Directory.CreateDirectory(dpPath);
	builder.Services.AddDataProtection()
		.PersistKeysToFileSystem(new DirectoryInfo(dpPath));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

// Honor forwarded headers from reverse proxies (Azure Front Door / App Service)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapAreaControllerRoute(
	name: "Admin",
	areaName: "Admin",
	pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

// Optional: apply EF Core migrations at startup when `APPLY_MIGRATIONS=true` is set in the environment.
if (Environment.GetEnvironmentVariable("APPLY_MIGRATIONS") == "true")
{
	using (var scope = app.Services.CreateScope())
	{
		var db = scope.ServiceProvider.GetRequiredService<AirBBContext>();
		db.Database.Migrate();
	}
}

app.Run();

