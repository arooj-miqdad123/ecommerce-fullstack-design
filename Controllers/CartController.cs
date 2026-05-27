using EcommerceApp.Data;
using EcommerceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly AppDbContext _db;

        public CartController(CartService cartService, AppDbContext db)
        {
            _cartService = cartService;
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            var cart = _cartService.GetCart();
            ViewBag.Subtotal = _cartService.GetCartTotal();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["Error"] = "Please login first to add items to cart.";
                return RedirectToAction("Login", "Account", new { returnUrl = "/Cart" });
            }
            var product = _db.Products.Find(productId);
            if (product == null) { TempData["Error"] = "Product not found."; return RedirectToAction("Index", "Product"); }
            _cartService.AddToCart(product, quantity);
            TempData["Success"] = $"{product.Name} added to cart!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            return Json(new { count = _cartService.GetCartCount() });
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}
