using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Shoppable.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";

        [Required, MaxLength(100)]
        public string Category { get; set; } = "";

        [Required]
        public decimal Price { get; set; }

        [Required, MaxLength(600), StringLength(600, ErrorMessage = "The description must be less than 600 characters long")]
        public string Description { get; set; } = "";

        public IFormFile? ImageFile { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        //public DateTime CreatedAt { get; set; }

    }
}

// Model for creating new products