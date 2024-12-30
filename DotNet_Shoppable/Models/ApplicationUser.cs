using Microsoft.AspNetCore.Identity;

namespace DotNet_Shoppable.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTimeOffset CreatedAt { get; set; }
        //public DateTime CreatedAt { get; set; } // <==== uncomment when using sqlserver
    }
}

// Model used to register users
// Ctrl + Left Click on 'IdentityUser' to see function defintions