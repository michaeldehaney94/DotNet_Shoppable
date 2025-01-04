using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet_Shoppable.Models
{
    [Table("OrderItems")] // stores items added to user's cart
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        [Precision(16, 2)]
        public decimal UnitPrice { get; set; }

        public Product Product { get; set; } = new Product();  // navigation property - foreign key
        
    }
}
