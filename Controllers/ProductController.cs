using EcommerceApp.Data;
using EcommerceApp.Services;
using EcommerceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly CartService _cartService;

        public ProductController(AppDbContext db, CartService cartService)
        {
            _db = db;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId, string? sortBy,
            decimal? minPrice, decimal? maxPrice, int page = 1, string viewMode = "grid")
        {
            ViewBag.AllCategories = await _db.Categories.ToListAsync();
            ViewBag.SearchQuery = search;
            int pageSize = 12;

            var query = _db.Products.Include(p => p.Category).Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) ||
                    (p.Description != null && p.Description.Contains(search)) ||
                    (p.Brand != null && p.Brand.Contains(search)));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);

            query = sortBy switch
            {
                "price_asc"  => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "rating"     => query.OrderByDescending(p => p.Rating),
                "newest"     => query.OrderByDescending(p => p.CreatedAt),
                _            => query.OrderByDescending(p => p.IsFeatured)
            };

            int totalCount = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

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
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount,
                ViewMode = viewMode
            };
            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            ViewBag.AllCategories = await _db.Categories.ToListAsync();
            var product = await _db.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null) return NotFound();

            ViewBag.RelatedProducts = await _db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive)
                .Take(6).ToListAsync();

            return View(product);
        }
    }
}
