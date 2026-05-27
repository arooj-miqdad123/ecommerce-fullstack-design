using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string CustomerEmail { get; set; } = string.Empty;

        public string? CustomerPhone { get; set; }

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; } = "Pakistan";

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string PaymentMethod { get; set; } = "COD"; // COD or Card

        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed

        public string? StripePaymentIntentId { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled

        public string? Notes { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total => UnitPrice * Quantity;
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
