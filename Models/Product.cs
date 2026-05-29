using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{
    // Represents a product entity in the e-commerce system mapped to a database table
    public class Product
    {
        // Primary key for the Product table
        public int Id { get; set; }

        // The product name, configured as required with a maximum string length of 200 characters
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        // Current selling price, explicitly mapped to an SQL decimal data type with 18 digits precision and 2 decimal places
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Optional original/crossed-out price, used for displaying discounts on the user interface
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }

        // Web URL path pointing to the product's primary display image asset
        public string? ImageUrl { get; set; }

        // Full comprehensive text description of the product features and specs
        public string? Description { get; set; }

        // Brief summary description intended for product listing previews or quick-view cards
        public string? ShortDescription { get; set; }

        // Foreign key linking this product to its parent Category node
        public int CategoryId { get; set; }

        // Navigation property to access related Category entity details directly from a Product object
        public Category? Category { get; set; }

        // Current real-time available stock quantity in inventory management
        public int Stock { get; set; }

        // Flag to control whether the item highlights in special 'Featured' homepage promotional sliders
        public bool IsFeatured { get; set; }

        // Flag to control soft deletion or item visibility without removing data from the database
        public bool IsActive { get; set; } = true;

        // Average numerical star rating computed from customer reviews
        public double Rating { get; set; } = 0;

        // Cumulative count of individual written reviews or scores submitted by customers
        public int ReviewCount { get; set; } = 0;

        // Manufacturer or brand identifier label
        public string? Brand { get; set; }

        // Stock Keeping Unit - unique alphanumeric code used for internal tracking and warehouse identification
        public string? Sku { get; set; }

        // Timestamp indicating when the item record was first provisioned into storage (Defaults to current UTC time)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property representing a one-to-many relationship with order invoice lines
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}