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
        private readonly IHttpContextAccessor _contextAccessor;

        public UserProductController(AppDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.ToListAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UserDetails(Guid id)
        {
            var product = await _db.Products.SingleOrDefaultAsync(p => p.Id == id);
            return View(product);
        }

        public IActionResult Cart()
        {
            // Получение текущего содержимого корзины
            List<Product> cartItems = GetCartItems();

            return View(cartItems);
        }

        public IActionResult RemoveFromCart(Guid productId)
        {
            // Получение текущего содержимого корзины
            List<Product> cartItems = GetCartItems();

            // Удаление товара из корзины по идентификатору
            Product productToRemove = cartItems.FirstOrDefault(p => p.Id == productId);
            if (productToRemove != null)
            {
                cartItems.Remove(productToRemove);

                // Сохранение обновленного содержимого корзины
                SaveCartItems(cartItems);
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid id)
        {
            // Получение текущего содержимого корзины из локального хранилища или файлов cookie
            List<Product> cartItems = GetCartItems();
            var product = await _db.Products.FindAsync(id);

            if (cartItems.Contains(product))
            {
                return RedirectToAction(nameof(Index));
            }
            // Добавление товара в корзину
            cartItems.Add(product);

            // Сохранение обновленного содержимого корзины в локальное хранилище или файлы cookie
            SaveCartItems(cartItems);

            return RedirectToAction("Cart");
        }

        private List<Product> GetCartItems()
        {
            // Проверка, есть ли уже данные корзины в локальном хранилище или файлах cookie
            if (HttpContext.Session.TryGetValue("CartItems", out byte[] cartItemsBytes))
            {
                // Если данные найдены, выполняется десериализация
                string cartItemsJson = Encoding.UTF8.GetString(cartItemsBytes);
                return JsonConvert.DeserializeObject<List<Product>>(cartItemsJson);
            }

            // Если данные не найдены, возвращается пустой список
            return new List<Product>();
        }

        private void SaveCartItems(List<Product> cartItems)
        {
            // Сериализация списка товаров в формат JSON
            string cartItemsJson = JsonConvert.SerializeObject(cartItems);

            // Сохранение сериализованных данных в локальное хранилище или файлы cookie
            byte[] cartItemsBytes = Encoding.UTF8.GetBytes(cartItemsJson);
            HttpContext.Session.Set("CartItems", cartItemsBytes);
        }

    }
}
