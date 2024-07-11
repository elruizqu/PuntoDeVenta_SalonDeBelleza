using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Conexion") ?? throw new InvalidOperationException("Connection string 'Conexion' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SalonContext>(options => options.UseSqlServer("name=Conexion"));

// Identity
builder.Services.AddDbContext<AuthContext>(options => options.UseSqlServer("name=Conexion"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    //Lockout settings

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.MaxFailedAccessAttempts = 4;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<AuthContext>()
    .AddDefaultUI();

builder.Services.AddRazorPages();
// Identity

// EmailSender service
builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// Identity
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity
app.MapRazorPages();

app.Run();
