using KnustHallMaintenance.Data;
using KnustHallMaintenance.Models;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);


// =========================================================
// MVC
// =========================================================

builder.Services.AddControllersWithViews();


// =========================================================
// FILE UPLOAD LIMITS
// =========================================================

// Allow multipart/form-data requests up to 10 MB.
// Individual uploaded photos are still limited to 5 MB
// inside ComplaintsController.
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});


// Allow Kestrel to receive requests up to 10 MB.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
});


// =========================================================
// DATABASE
// =========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\MSSQLLocalDB;Database=KnustHallMaintenance;Trusted_Connection=True;TrustServerCertificate=True"
    ));


// =========================================================
// SERVICES
// =========================================================

builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();

builder.Services.AddScoped<IComplaintService, ComplaintService>();

builder.Services.AddScoped<IUserService, UserService>();


// =========================================================
// AUTHENTICATION
// =========================================================

builder.Services.AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme
    )
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";

        options.AccessDeniedPath = "/Account/Login";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });


// =========================================================
// AUTHORIZATION
// =========================================================

builder.Services.AddAuthorization();


var app = builder.Build();


// =========================================================
// ERROR HANDLING
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}


// =========================================================
// HTTP PIPELINE
// =========================================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


// =========================================================
// ROUTING
// =========================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


// =========================================================
// DATABASE SEEDING
// =========================================================

using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    var hasher =
        scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<ApplicationUser>>();

    DbInitializer.Seed(
        db,
        hasher
    );
}


// =========================================================
// START APPLICATION
// =========================================================

app.Run();