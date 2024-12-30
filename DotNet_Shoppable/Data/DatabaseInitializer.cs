using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Identity;

namespace DotNet_Shoppable.Data
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager, RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }

            // check if the admin role exist
            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            // check if the seller role exist
            exists = await roleManager.RoleExistsAsync("seller");
            if (!exists)
            {
                Console.WriteLine("Seller role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }

            // check if the client role exist
            exists = await roleManager.RoleExistsAsync("client");
            if (!exists)
            {
                Console.WriteLine("Client role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }

            // check if we have at least one admin user or not
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any()) 
            {
                //If admin user already exists output message
                Console.WriteLine("Admin user already exists => exit");
                return;
            }

            // create admin user
            var user = new ApplicationUser() 
            {
                FirstName = "System",
                LastName = "Admin",
                UserName = "SysAdmin",
                Email = "sys@admin.com",
                //CreatedAt = DateTime.Now, // uncomment when using sqlserver
                CreatedAt = DateTime.UtcNow,
            };

            string initialPassword = "admin1234";

            // create user account
            var result = await userManager.CreateAsync(user, initialPassword);
            if (result.Succeeded) 
            {
                // set the user role
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin user created successfully! Please update the initial password!");
                Console.WriteLine("Email: " + user.Email);
                Console.WriteLine("Initial password: " + initialPassword);
            }



        }
 
    }
}

// This initializer will create seed data user record if admin role, seller role does not exits in table(s)