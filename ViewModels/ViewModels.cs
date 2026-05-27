using EcommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = "Pakistan";

        [Required(ErrorMessage = "Please select payment method")]
        public string PaymentMethod { get; set; } = "COD";

        public string? StripeToken { get; set; }

        public string? CouponCode { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string ViewMode { get; set; } = "grid"; // grid or list
    }

    public class HomeViewModel
    {
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
        public List<Product> NewArrivals { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> RecommendedProducts { get; set; } = new List<Product>();
    }
}
