using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Shoppable.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // 'DbContext' (changed to implement Identity and Access Management control)
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
