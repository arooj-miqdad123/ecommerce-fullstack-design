using EcommerceApp.Data;
using EcommerceApp.Services;
using EcommerceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    // Controller to manage product catalog features like browsing, searching, and viewing details
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly CartService _cartService;

        // Injecting Database Context and Cart Service via Dependency Injection
        public ProductController(AppDbContext db, CartService cartService)
        {
            _db = db;
            _cartService = cartService;
        }

        // Action to display the product catalog listing with search, filters, sorting, and pagination
        public async Task<IActionResult> Index(string? search, int? categoryId, string? sortBy,
            decimal? minPrice, decimal? maxPrice, int page = 1, string viewMode = "grid")
        {
            // Populating ViewBag data for layout categories and preserving search term in UI
            ViewBag.AllCategories = await _db.Categories.ToListAsync();
            ViewBag.SearchQuery = search;
            int pageSize = 12; // Number of items per page

            // Base query to fetch active products and their corresponding category data
            var query = _db.Products.Include(p => p.Category).Where(p => p.IsActive);

            // Filter: Search keyword matching product Name, Description, or Brand
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) ||
                    (p.Description != null && p.Description.Contains(search)) ||
                    (p.Brand != null && p.Brand.Contains(search)));

            // Filter: Specific Category if selected
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            // Filter: Minimum and Maximum Price range
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);

            // Sorting logic based on user selection, defaults to featuring products first
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "rating" => query.OrderByDescending(p => p.Rating),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.IsFeatured)
            };

            // Get total matching product count for pagination calculations
            int totalCount = await query.CountAsync();

            // Fetch only the specific subset of products for the current page
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Constructing the View Model to bind data to the UI View
            var model = new ProductListViewModel
            {
                Products = products,
                Categories = await _db.Categories.ToListAsync(),
                SearchTerm = search,
                CategoryId = categoryId,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize), // Calculating total page count
                TotalCount = totalCount,
                ViewMode = viewMode
            };
            return View(model);
        }

        // Action to display the detailed page of a specific product
        public async Task<IActionResult> Detail(int id)
        {
            // Category list for navigation/layout elements
            ViewBag.AllCategories = await _db.Categories.ToListAsync();

            // Find active product matching the given ID
            var product = await _db.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            // Return 404 page if product is not found or is inactive
            if (product == null) return NotFound();

            // Fetch up to 6 related active products from the same category, excluding the current product
            ViewBag.RelatedProducts = await _db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive)
                .Take(6).ToListAsync();

            return View(product);
        }
    }
}