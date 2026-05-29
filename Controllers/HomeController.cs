using EcommerceApp.Data;
using EcommerceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    // Controller to handle the main landing page and public static routes
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        // Injecting Database Context via Dependency Injection
        public HomeController(AppDbContext db) { _db = db; }

        // Action to populate and display the application's homepage
        public async Task<IActionResult> Index()
        {
            // Populating ViewBag with all categories for the global layout/navigation menu
            ViewBag.AllCategories = await _db.Categories.ToListAsync();

            // Constructing the HomeViewModel by fetching specific product sections concurrently/sequentially
            var model = new HomeViewModel
            {
                // Fetch up to 8 active products marked as 'Featured'
                FeaturedProducts = await _db.Products.Include(p => p.Category)
                    .Where(p => p.IsFeatured && p.IsActive).Take(8).ToListAsync(),

                // Fetch up to 8 of the latest active products based on creation timestamp
                NewArrivals = await _db.Products.Include(p => p.Category)
                    .Where(p => p.IsActive).OrderByDescending(p => p.CreatedAt).Take(8).ToListAsync(),

                // Fetch all categories to showcase in the category section of the homepage
                Categories = await _db.Categories.ToListAsync(),

                // Fetch up to 6 top-rated active products for the recommendation widget
                RecommendedProducts = await _db.Products.Include(p => p.Category)
                    .Where(p => p.IsActive).OrderByDescending(p => p.Rating).Take(6).ToListAsync()
            };

            return View(model);
        }

        // Action to handle and display global application errors or exceptions
        public IActionResult Error() => View();
    }
}