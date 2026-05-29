using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{
    // Represents the main Order entity containing customer information and financial summaries
    public class Order
    {
        // Primary key for the Order table
        public int Id { get; set; }

        // Optional link to the Identity User ID if the purchase is made by a logged-in user
        public string? UserId { get; set; }

        // Full name of the customer receiving the delivery
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        // Email address for sending notifications and invoices
        [Required]
        public string CustomerEmail { get; set; } = string.Empty;

        // Contact number for shipping updates or delivery riders
        public string? CustomerPhone { get; set; }

        // Street-level physical address for shipping fulfillment
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        // City region for localized delivery zone classification
        public string? City { get; set; }

        // Destination country, defaulting to Pakistan
        public string? Country { get; set; } = "Pakistan";

        // Precise system timestamp marking when the transaction occurred (defaults to UTC)
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Total calculated value of all attached products before accounting for extra fees
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // Standard delivery logistics fee charged to the customer
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; } = 0;

        // Deductible amount calculated from promotional vouchers or system markdown rules
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0;

        // Net transactional total calculated as: Subtotal + ShippingCost - Discount
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // System identifier for the financial collection track (Defaults to Cash on Delivery)
        public string PaymentMethod { get; set; } = "COD"; // Expected values: "COD" or "Card"

        // Tracking field to see if money has been settled (Expected values: "Pending", "Paid", "Failed")
        public string PaymentStatus { get; set; } = "Pending";

        // Reference reference handle provided by Stripe API gateway upon processing credit card forms
        public string? StripePaymentIntentId { get; set; }

        // Internal fulfillment status milestones (Expected values: "Pending", "Processing", "Shipped", "Delivered", "Cancelled")
        public string Status { get; set; } = "Pending";

        // Optional special instructions or customization fields specified by the buyer
        public string? Notes { get; set; }

        // Navigation property mapping out the parent record side of a one-to-many child relation loop
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    // Represents a single transaction line item mapped inside a specific customer order receipt
    public class OrderItem
    {
        // Primary key for the individual order line item records table
        public int Id { get; set; }

        // Foreign key property referencing the parent Order object block node
        public int OrderId { get; set; }

        // Navigation object mapping relational database integrity constraints back to the order entry
        public Order? Order { get; set; }

        // Foreign key property referencing the target inventory Product being purchased
        public int ProductId { get; set; }

        // Navigation object referencing full master tracking property maps for the inventory product
        public Product? Product { get; set; }

        // Total multiplier unit instances bought of the specified product
        public int Quantity { get; set; }

        // Historical locked-in purchase rate per item, preserving data context if base model prices adjust over time
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Computed property yielding the absolute subtotal price for this individual product transaction line
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total => UnitPrice * Quantity;
    }

    // DTO (Data Transfer Object) class used to manage ephemeral cart item snapshots in sessions or cookies
    public class CartItem
    {
        // Primary tracking reference key to check match boundaries against standard Product tables
        public int ProductId { get; set; }

        // Product name snapshot for lighter rendering cycles inside mini-cart UI segments
        public string ProductName { get; set; } = string.Empty;

        // Product price point value captured at active entry
        public decimal Price { get; set; }

        // Relative reference location token path string pointing to product thumbnail graphic structures
        public string? ImageUrl { get; set; }

        // Total instances targeted for reservation inside shopping structures
        public int Quantity { get; set; }

        // Computed absolute sub-total field evaluating targeted weight against baseline values
        public decimal Total => Price * Quantity;
    }
}