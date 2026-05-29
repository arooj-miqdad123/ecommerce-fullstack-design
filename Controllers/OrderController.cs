using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.Services;
using EcommerceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    // Controller to manage checkout processes, order placement, and order history
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly CartService _cartService;

        // Injecting Database Context and Cart Service via Dependency Injection
        public OrderController(AppDbContext db, CartService cartService)
        {
            _db = db;
            _cartService = cartService;
        }

        // Action to load the checkout page for logged-in users
        [Authorize]
        public IActionResult Checkout()
        {
            // Populating layout categories
            ViewBag.AllCategories = _db.Categories.ToList();

            // Get current user's shopping cart items
            var cart = _cartService.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var subtotal = _cartService.GetCartTotal();

            // Preparing checkout calculations (Free shipping if total is $100 or above)
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

        // Action to process and save the placed order
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            var cart = _cartService.GetCart();

            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Re-populating view model data in case the form validation fails and needs to re-render
            model.CartItems = cart;
            model.Subtotal = _cartService.GetCartTotal();
            model.ShippingCost = model.Subtotal >= 100 ? 0m : 10m;
            model.Total = model.Subtotal + model.ShippingCost - model.Discount;

            // Check if server-side validation rules are met
            if (!ModelState.IsValid)
                return View("Checkout", model);

            // Card Validation: Ensuring Stripe token is generated and valid for test mode
            if (model.PaymentMethod == "Card")
            {
                if (string.IsNullOrEmpty(model.StripeToken) || !model.StripeToken.StartsWith("tok_test_"))
                {
                    TempData["Error"] = "Please fill in valid card details and click the Pay button.";
                    return View("Checkout", model);
                }
            }

            // Initializing new Order entity mapping from View Model and Cart data
            var order = new Order
            {
                // Retrieving the unique ID of the currently logged-in user
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                ShippingAddress = model.ShippingAddress,
                City = model.City,
                Country = model.Country,
                PaymentMethod = model.PaymentMethod,
                Subtotal = model.Subtotal,
                ShippingCost = model.ShippingCost,
                Discount = model.Discount,
                Total = model.Total,
                // Mapping cart items to OrderItems collection
                OrderItems = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Price
                }).ToList()
            };

            // Setting statuses based on the chosen payment method
            if (model.PaymentMethod == "Card")
            {
                order.PaymentStatus = "Paid";
                order.Status = "Processing";
                order.StripePaymentIntentId = model.StripeToken;
            }
            else // e.g., Cash on Delivery (COD)
            {
                order.PaymentStatus = "Pending";
                order.Status = "Pending";
            }

            // Inventory management: Reduce available product stock based on items ordered
            foreach (var item in cart)
            {
                var p = await _db.Products.FindAsync(item.ProductId);
                if (p != null && p.Stock >= item.Quantity)
                    p.Stock -= item.Quantity;
            }

            // Saving order to database and clearing session/cookie cart
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            _cartService.ClearCart();

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        // Action to display order summary page after a successful checkout
        public async Task<IActionResult> Confirmation(int id)
        {
            ViewBag.AllCategories = _db.Categories.ToList();

            // Eager loading order items and nested product details for the invoice view
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // Action to show order history for the logged-in user or all orders if Admin
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Admin can view all orders, standard users can only view their own orders
            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                .Where(o => User.IsInRole("Admin") || o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}