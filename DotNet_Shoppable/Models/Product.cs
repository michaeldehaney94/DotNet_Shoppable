using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Shoppable.Models
{
    public class Product
    {
        // add a empty string to get rid of the Null error message
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Brand { get; set; } = "";

        [MaxLength(100)]
        public string Category { get; set; } = "";

        [Precision(16, 2)]
        public decimal Price { get; set; }

        [MaxLength(600)]
        public string Description { get; set; } = "";

        [MaxLength (100)]
        public string ImageFileName { get; set; } = "";

        public DateTimeOffset CreatedAt { get; set; }

        //public DateTime CreatedAt { get; set; }




    }
}
