using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    // Represents a product classification category entity mapped to a database table
    public class Category
    {
        // Primary key for the Category table
        public int Id { get; set; }

        // The category name, configured as required with a maximum string length of 100 characters
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Web URL path pointing to the category's display thumbnail or banner image asset
        public string? ImageUrl { get; set; }

        // Descriptive text explaining what types of items are grouped under this category
        public string? Description { get; set; }

        // Navigation property representing a one-to-many relationship with related Product records
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}