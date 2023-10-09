using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectStore.Models;
using System.Text;

namespace ProjectStore.Controllers
{
    public class UserProductController : Controller
    {
        private readonly AppDbContext _db;

        public UserProductController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            IQueryable<Product> products = _db.Products;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            return View(await products.AsNoTracking().ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> UserDetails(Guid id)
        {
            var product = await _db.Products.SingleOrDefaultAsync(p => p.Id == id);
            return View(product);
        }

        public IActionResult Cart()
        {
            List<Product?>? cartItems = GetCartItems()!;

            return View(cartItems);
        }

        public IActionResult RemoveFromCart(Guid productId)
        {
            List<Product?>? cartItems = GetCartItems()!;

            Product? productToRemove = cartItems.FirstOrDefault(p => p.Id == productId);
            if (productToRemove != null)
            {
                cartItems.Remove(productToRemove);

                SaveCartItems(cartItems);
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid id)
        {
            List<Product>? cartItems = GetCartItems();
            var product = await _db.Products.FindAsync(id);

            if (product != null && cartItems != null && cartItems.Contains(product))
            {
                return RedirectToAction(nameof(Index));
            }

            cartItems?.Add(product ?? throw new InvalidOperationException("Product is null"));

            SaveCartItems(cartItems!);

            return RedirectToAction("Cart");
        }

        private List<Product>? GetCartItems()
        {
            if (HttpContext.Session.TryGetValue("CartItems", out byte[]? cartItemsBytes))
            {
                string cartItemsJson = Encoding.UTF8.GetString(cartItemsBytes);
                return JsonConvert.DeserializeObject<List<Product>>(cartItemsJson);
            }

            return new List<Product>();
        }

        private void SaveCartItems(List<Product?>? cartItems)
        {
            string cartItemsJson = JsonConvert.SerializeObject(cartItems);

            byte[] cartItemsBytes = Encoding.UTF8.GetBytes(cartItemsJson);
            HttpContext.Session.Set("CartItems", cartItemsBytes);
        }
    }
}