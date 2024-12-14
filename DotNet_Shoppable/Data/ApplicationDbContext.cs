using DotNet_Shoppable.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Shoppable.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
