using EcommerceApp.Data;
using EcommerceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    // Controller to handle shopping cart operations like viewing, adding, removing, and updating items
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly AppDbContext _db;

        // Injecting Cart Service and Database Context via Dependency Injection
        public CartController(CartService cartService, AppDbContext db)
        {
            _cartService = cartService;
            _db = db;
        }

        // Action to display the main shopping cart page
        public IActionResult Index()
        {
            // Populating layout categories for navigation
            ViewBag.AllCategories = _db.Categories.ToList();

            // Fetching current cart items from session/cookie storage
            var cart = _cartService.GetCart();

            // Calculating and storing the accumulated price of all items in the cart
            ViewBag.Subtotal = _cartService.GetCartTotal();

            return View(cart);
        }

        // Action to add a product to the shopping cart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            // Restricting cart operations to authenticated/logged-in users only
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["Error"] = "Please login first to add items to cart.";
                return RedirectToAction("Login", "Account", new { returnUrl = "/Cart" });
            }

            // Verifying if the requested product actually exists in the database
            var product = _db.Products.Find(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index", "Product");
            }

            // Invoking service logic to push the item into the session/cookie cart
            _cartService.AddToCart(product, quantity);
            TempData["Success"] = $"{product.Name} added to cart!";

            return RedirectToAction("Index");
        }

        // Action to completely remove an item from the cart
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction("Index");
        }

        // Action to adjust the specific quantity of a product already in the cart
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            return RedirectToAction("Index");
        }

        // API Endpoint (AJAX-friendly) to dynamically fetch the total count of items in the badge header
        [HttpGet]
        public IActionResult GetCartCount()
        {
            return Json(new { count = _cartService.GetCartCount() });
        }

        // Action to empty all items out of the current user's shopping cart
        [HttpPost]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}