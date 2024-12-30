using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//connects the database to the web app backend 
//install Npgsql.EntityFrameworkCore.PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
        //add connectionstring from appsettings.json
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
//        //add connectionstring from appsettings.json
//        builder.Configuration.GetConnectionString("DefaultConnection")
//    ));


// Identity Services rules for User login accounts
// First install Microsoft.AspNetCore.Identity.EntityFrameworkCore
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options => 
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false; // if special characters is needed
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// run data seeding
// create the roles and the first admin user if not available yet
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

    var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;

    await DatabaseInitializer.SeedDataAsync(userManager, roleManager);
}

app.Run();
