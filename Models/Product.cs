using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        public string? ShortDescription { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int Stock { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; } = true;

        public double Rating { get; set; } = 0;

        public int ReviewCount { get; set; } = 0;

        public string? Brand { get; set; }

        public string? Sku { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
