using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.TotalOrders = await _db.Orders.CountAsync();
            ViewBag.PendingOrders = await _db.Orders.CountAsync(o => o.Status == "Pending");
            ViewBag.TotalRevenue = await _db.Orders.Where(o => o.PaymentStatus == "Paid").SumAsync(o => o.Total);
            ViewBag.RecentOrders = await _db.Orders.Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).Take(5).ToListAsync();
            return View();
        }

        // Products
        public async Task<IActionResult> Products(string? search)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));
            ViewBag.Products = await query.OrderByDescending(p => p.Id).ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "products", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                product.ImageUrl = "/images/products/" + fileName;
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product created successfully!";
            return RedirectToAction("Products");
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "products", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                product.ImageUrl = "/images/products/" + fileName;
            }

            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Product deleted.";
            }
            return RedirectToAction("Products");
        }

        // Orders
        public async Task<IActionResult> Orders(string? status)
        {
            var query = _db.Orders.Include(o => o.OrderItems).AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);
            ViewBag.Orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Orders");
        }

        // Categories
        public async Task<IActionResult> Categories()
        {
            ViewBag.Categories = await _db.Categories.Include(c => c.Products).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "categories", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                category.ImageUrl = "/images/categories/" + fileName;
            }
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Category created!";
            return RedirectToAction("Categories");
        }
    }
}
