using EcommerceApp.Models;
using Newtonsoft.Json;

namespace EcommerceApp.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartKey = "ShoppingCart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public List<CartItem> GetCart()
        {
            var json = Session.GetString(CartKey);
            return json == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            Session.SetString(CartKey, JsonConvert.SerializeObject(cart));
        }

        public void AddToCart(Product product, int quantity = 1)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductId == product.Id);
            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = quantity
                });
            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.ProductId == productId);
            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            SaveCart(cart);
        }

        public void ClearCart()
        {
            Session.Remove(CartKey);
        }

        public int GetCartCount()
        {
            return GetCart().Sum(x => x.Quantity);
        }

        public decimal GetCartTotal()
        {
            return GetCart().Sum(x => x.Total);
        }
    }
}
