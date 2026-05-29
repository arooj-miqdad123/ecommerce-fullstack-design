using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    // Restricts access to this entire controller exclusively to users with the 'Admin' role
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env; // Used to determine the physical path on the server for uploading files

        // Injecting Database Context and Web Host Environment via Dependency Injection
        public AdminController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Action to display the main admin dashboard metrics and analytics
        public async Task<IActionResult> Index()
        {
            // Aggregating key e-commerce business performance indicators into ViewBag
            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.TotalOrders = await _db.Orders.CountAsync();
            ViewBag.PendingOrders = await _db.Orders.CountAsync(o => o.Status == "Pending");

            // Calculate total revenue from all orders that have a payment status of 'Paid'
            ViewBag.TotalRevenue = await _db.Orders.Where(o => o.PaymentStatus == "Paid").SumAsync(o => o.Total);

            // Retrieve the 5 most recent orders with their line items to list on the dashboard overview
            ViewBag.RecentOrders = await _db.Orders.Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).Take(5).ToListAsync();
            return View();
        }

        #region Products Management

        // Action to view and search through the entire inventory catalog
        public async Task<IActionResult> Products(string? search)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();

            // Filter by name if a search keyword is provided
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            ViewBag.Products = await query.OrderByDescending(p => p.Id).ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync(); // Categories dropdown population
            return View();
        }

        // Action to render the 'Add Product' form view
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        // Action to process the creation of a new product along with an optional image upload
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery attacks
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile)
        {
            // Handling the raw file upload stream if an image is provided
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate a unique identifier filename to prevent overwriting existing assets
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "products", fileName);

                // Saving the file physically inside the wwwroot/images/products directory
                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                // Storing the relative web URL path inside the product record
                product.ImageUrl = "/images/products/" + fileName;
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product created successfully!";
            return RedirectToAction("Products");
        }

        // Action to render the update/edit form for a product based on its primary key
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(product);
        }

        // Action to process updates made to an existing product profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Upload and map the new image asset if replaced by the admin
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "products", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                product.ImageUrl = "/images/products/" + fileName;
            }

            // Flags the entity state as Modified to trigger an SQL UPDATE statement upon saving
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction("Products");
        }

        // Action to handle deletion requests for a product
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

        #endregion

        #region Orders Management

        // Action to view customer orders, optionally filtered by progress status (Pending, Shipped, etc.)
        public async Task<IActionResult> Orders(string? status)
        {
            var query = _db.Orders.Include(o => o.OrderItems).AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            ViewBag.Orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            return View();
        }

        // Action to switch or update order tracking fulfillment milestones
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

        #endregion

        #region Categories Management

        // Action to manage product categories and see counts of attached items
        public async Task<IActionResult> Categories()
        {
            ViewBag.Categories = await _db.Categories.Include(c => c.Products).ToListAsync();
            return View();
        }

        // Action to process new catalog classification nodes (categories) with thumbnail icons
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Unique generation for storing localized category banner graphics
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

        #endregion
    }
}