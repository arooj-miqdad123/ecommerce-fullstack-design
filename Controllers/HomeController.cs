using EcommerceApp.Data;
using EcommerceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            ViewBag.AllCategories = await _db.Categories.ToListAsync();
            var model = new HomeViewModel
            {
                FeaturedProducts = await _db.Products.Include(p => p.Category)
                    .Where(p => p.IsFeatured && p.IsActive).Take(8).ToListAsync(),
                NewArrivals = await _db.Products.Include(p => p.Category)
                    .Where(p => p.IsActive).OrderByDescending(p => p.CreatedAt).Take(8).ToListAsync(),
                Categories = await _db.Categories.ToListAsync(),
                RecommendedProducts = await _db.Products.Include(p => p.Category)
                    .Where(p => p.IsActive).OrderByDescending(p => p.Rating).Take(6).ToListAsync()
            };
            return View(model);
        }

        public IActionResult Error() => View();
    }
}
