using EcommerceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Categories - .png extension
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", ImageUrl = "/images/categories/electronics.png", Description = "Electronic devices and gadgets" },
                new Category { Id = 2, Name = "Clothes", ImageUrl = "/images/categories/clothes.png", Description = "Fashion and apparel" },
                new Category { Id = 3, Name = "Gadgets", ImageUrl = "/images/categories/gadgets.png", Description = "Smart gadgets and accessories" },
                new Category { Id = 4, Name = "Accessories", ImageUrl = "/images/categories/accessories.png", Description = "Bags, watches and accessories" },
                new Category { Id = 5, Name = "Smart Watches", ImageUrl = "/images/categories/smartwatches.png", Description = "Smart watches and wearables" }
            );

            // Seed Products - all .png extensions
            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "T-Shirts with multiple colors, for men", Price = 10.30m, OldPrice = 16.00m, ImageUrl = "/images/products/product-1.png", CategoryId = 2, Stock = 150, IsFeatured = true, Rating = 4.5, ReviewCount = 154, Brand = "ArseMarket", ShortDescription = "Size medium, Color: blue, Material: Cotton", Description = "High quality t-shirt available in multiple colors. Perfect for everyday wear. Made from premium cotton blend for comfort and durability." },
                new Product { Id = 2, Name = "Solid Backpack blue jeans large size", Price = 79.99m, OldPrice = null, ImageUrl = "/images/products/product-2.png", CategoryId = 4, Stock = 60, IsFeatured = true, Rating = 4.2, ReviewCount = 98, Brand = "ArseMarket", ShortDescription = "Large size, Color: blue, Denim material", Description = "Spacious and stylish backpack made from premium denim. Multiple compartments for all your storage needs." },
                new Product { Id = 3, Name = "Water Boiler Black for Kitchen 1200 Watt", Price = 79.99m, OldPrice = null, ImageUrl = "/images/products/product-3.png", CategoryId = 1, Stock = 45, IsFeatured = false, Rating = 4.0, ReviewCount = 77, Brand = "KitchenPro", ShortDescription = "1200W, Color: Black, Fast boiling", Description = "Powerful 1200W electric water boiler with auto shutoff. Perfect for kitchen use. BPA-free interior with 360-degree swivel base." },
                new Product { Id = 4, Name = "Regular Fit Resort Shirt", Price = 57.70m, OldPrice = 85.00m, ImageUrl = "/images/products/product-4.png", CategoryId = 2, Stock = 200, IsFeatured = true, Rating = 4.7, ReviewCount = 210, Brand = "FashionHub", ShortDescription = "Regular fit, Resort style, Multiple colors", Description = "Stylish resort shirt perfect for summer. Breathable fabric with vibrant prints. Available in multiple sizes." },
                new Product { Id = 5, Name = "GoPro HERO6 4K Action Camera - Black", Price = 99.50m, OldPrice = 129.00m, ImageUrl = "/images/products/product-5.png", CategoryId = 1, Stock = 30, IsFeatured = true, Rating = 4.8, ReviewCount = 340, Brand = "GoPro", ShortDescription = "4K, Waterproof, Action camera", Description = "Capture your adventures in stunning 4K. Waterproof to 33ft. Comes with voice control and image stabilization." },
                new Product { Id = 6, Name = "Canon EOS 2000D DSLR Camera", Price = 998.00m, OldPrice = null, ImageUrl = "/images/products/product-6.png", CategoryId = 1, Stock = 15, IsFeatured = false, Rating = 4.6, ReviewCount = 89, Brand = "Canon", ShortDescription = "24.1 MP, DSLR, Black", Description = "Entry-level DSLR camera with 24.1 megapixel CMOS sensor. Built-in Wi-Fi and NFC for easy sharing." },
                new Product { Id = 7, Name = "Apple Watch Series Smart Watch", Price = 299.00m, OldPrice = 350.00m, ImageUrl = "/images/products/product-7.png", CategoryId = 5, Stock = 50, IsFeatured = true, Rating = 4.9, ReviewCount = 520, Brand = "Apple", ShortDescription = "GPS, Heart rate, Fitness tracker", Description = "Advanced smartwatch with health monitoring, GPS, and seamless iPhone integration. All-day battery life." },
                new Product { Id = 8, Name = "Wireless Noise-Cancelling Headphones", Price = 77.00m, OldPrice = 99.00m, ImageUrl = "/images/products/product-8.png", CategoryId = 3, Stock = 80, IsFeatured = true, Rating = 4.4, ReviewCount = 185, Brand = "SoundMax", ShortDescription = "Wireless, ANC, 30hr battery", Description = "Premium over-ear headphones with active noise cancellation. Crystal clear sound with deep bass. 30 hours playtime." },
                new Product { Id = 9, Name = "iPhone 14 Pro 256GB Space Black", Price = 999.00m, OldPrice = 1099.00m, ImageUrl = "/images/products/product-9.png", CategoryId = 1, Stock = 25, IsFeatured = true, Rating = 4.9, ReviewCount = 650, Brand = "Apple", ShortDescription = "256GB, 5G, A16 Bionic chip", Description = "The most advanced iPhone ever. A16 Bionic chip, 48MP camera system, and Dynamic Island." },
                new Product { Id = 10, Name = "Laptop MacBook Pro 13 inch M2", Price = 1299.00m, OldPrice = 1499.00m, ImageUrl = "/images/products/product-10.png", CategoryId = 1, Stock = 20, IsFeatured = false, Rating = 4.8, ReviewCount = 290, Brand = "Apple", ShortDescription = "M2 chip, 256GB SSD, 8GB RAM", Description = "Supercharged by M2 chip. Up to 20 hours battery life. Stunning Liquid Retina display." },
                new Product { Id = 11, Name = "Samsung Galaxy S23 Ultra", Price = 1199.00m, OldPrice = 1299.00m, ImageUrl = "/images/products/product-11.png", CategoryId = 1, Stock = 35, IsFeatured = true, Rating = 4.7, ReviewCount = 430, Brand = "Samsung", ShortDescription = "200MP camera, S-Pen, 5G", Description = "Samsung's flagship with 200MP camera and built-in S Pen. 6.8 inch Dynamic AMOLED display." },
                new Product { Id = 12, Name = "Sony WH-1000XM5 Headphones", Price = 349.00m, OldPrice = 399.00m, ImageUrl = "/images/products/product-12.png", CategoryId = 3, Stock = 45, IsFeatured = true, Rating = 4.9, ReviewCount = 312, Brand = "Sony", ShortDescription = "Industry-best ANC, 30hr battery", Description = "Industry-leading noise cancellation with 8 mics. 30-hour battery. Multipoint connection." },
                new Product { Id = 13, Name = "Casual Denim Jacket Men", Price = 45.00m, OldPrice = 65.00m, ImageUrl = "/images/products/product-13.png", CategoryId = 2, Stock = 90, IsFeatured = false, Rating = 4.3, ReviewCount = 87, Brand = "DenimCo", ShortDescription = "Classic fit, Blue denim", Description = "Classic denim jacket for men. Premium quality denim fabric. Available in multiple sizes." },
                new Product { Id = 14, Name = "Smart LED TV 55 inch 4K", Price = 649.00m, OldPrice = 799.00m, ImageUrl = "/images/products/product-14.png", CategoryId = 1, Stock = 18, IsFeatured = true, Rating = 4.6, ReviewCount = 201, Brand = "TechVision", ShortDescription = "4K UHD, Smart TV, HDR", Description = "55-inch 4K Ultra HD Smart LED TV with HDR10+ support. Built-in streaming apps and voice control." },
                new Product { Id = 15, Name = "Nike Running Shoes Men", Price = 89.00m, OldPrice = 120.00m, ImageUrl = "/images/products/product-15.png", CategoryId = 4, Stock = 120, IsFeatured = false, Rating = 4.5, ReviewCount = 567, Brand = "Nike", ShortDescription = "Lightweight, Breathable, Running", Description = "Professional running shoes with advanced cushioning technology. Breathable mesh upper and durable rubber sole." },
                new Product { Id = 16, Name = "iPad Air 5th Gen 64GB", Price = 749.00m, OldPrice = null, ImageUrl = "/images/products/product-16.png", CategoryId = 1, Stock = 22, IsFeatured = true, Rating = 4.8, ReviewCount = 189, Brand = "Apple", ShortDescription = "M1 chip, 10.9 inch, WiFi", Description = "Powerful iPad Air with M1 chip. 10.9-inch Liquid Retina display. All-day battery life." },
                new Product { Id = 17, Name = "Leather Wallet Brown Men", Price = 29.99m, OldPrice = 45.00m, ImageUrl = "/images/products/product-17.png", CategoryId = 4, Stock = 200, IsFeatured = false, Rating = 4.1, ReviewCount = 134, Brand = "LeatherCraft", ShortDescription = "Genuine leather, RFID blocking", Description = "Premium genuine leather wallet with RFID blocking technology. Multiple card slots and cash compartment." }
            );
        }
    }
}
