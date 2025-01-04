using Microsoft.EntityFrameworkCore;

namespace DotNet_Shoppable.Models
{
    public class Order // shows the individual users order list
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = ""; // foreign key
        public ApplicationUser Client { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>(); // navigation property that will create entity relationship with orderItem

        [Precision(16, 2)]
        public decimal ShippingFee { get; set; }
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string PaymentDetails { get; set; } = ""; // to store paypal details
        public string OrderStatus { get; set; } = "";
        public DateTimeOffset CreatedAt { get; set; } // comment out when using sqlserver

        //public DateTime CreatedAt { get; set; } // <== uncomment when using sqlserver
    }
}

// This order model will have an entity-relationship with order items