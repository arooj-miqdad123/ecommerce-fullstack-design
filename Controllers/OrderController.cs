using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.Services;
using EcommerceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly CartService _cartService;

        public OrderController(AppDbContext db, CartService cartService)
        {
            _db = db;
            _cartService = cartService;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            var cart = _cartService.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }
            var subtotal = _cartService.GetCartTotal();
            var model = new CheckoutViewModel
            {
                CartItems = cart,
                Subtotal = subtotal,
                ShippingCost = subtotal >= 100 ? 0m : 10m,
                Discount = 0,
                Total = subtotal + (subtotal >= 100 ? 0m : 10m)
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            var cart = _cartService.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            model.CartItems = cart;
            model.Subtotal = _cartService.GetCartTotal();
            model.ShippingCost = model.Subtotal >= 100 ? 0m : 10m;
            model.Total = model.Subtotal + model.ShippingCost - model.Discount;

            if (!ModelState.IsValid)
                return View("Checkout", model);

            // Card: token must be present and valid
            if (model.PaymentMethod == "Card")
            {
                if (string.IsNullOrEmpty(model.StripeToken) || !model.StripeToken.StartsWith("tok_test_"))
                {
                    TempData["Error"] = "Please fill in valid card details and click the Pay button.";
                    return View("Checkout", model);
                }
            }

            var order = new Order
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                CustomerName    = model.CustomerName,
                CustomerEmail   = model.CustomerEmail,
                CustomerPhone   = model.CustomerPhone,
                ShippingAddress = model.ShippingAddress,
                City            = model.City,
                Country         = model.Country,
                PaymentMethod   = model.PaymentMethod,
                Subtotal        = model.Subtotal,
                ShippingCost    = model.ShippingCost,
                Discount        = model.Discount,
                Total           = model.Total,
                OrderItems      = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity  = c.Quantity,
                    UnitPrice = c.Price
                }).ToList()
            };

            if (model.PaymentMethod == "Card")
            {
                order.PaymentStatus = "Paid";
                order.Status = "Processing";
                order.StripePaymentIntentId = model.StripeToken;
            }
            else
            {
                order.PaymentStatus = "Pending";
                order.Status = "Pending";
            }

            // Reduce stock
            foreach (var item in cart)
            {
                var p = await _db.Products.FindAsync(item.ProductId);
                if (p != null && p.Stock >= item.Quantity)
                    p.Stock -= item.Quantity;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            _cartService.ClearCart();

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                .Where(o => User.IsInRole("Admin") || o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }
    }
}
