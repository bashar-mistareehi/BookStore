using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // عرض محتويات السلة
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("ShoppingCart") ?? new List<CartItem>();
            return View(cart);
        }

        // إضافة كتاب للسلة
        // إضافة كتاب للسلة والبقاء في نفس الصفحة
        public async Task<IActionResult> AddToCart(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("ShoppingCart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.BookId == id);

            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Price = book.Price,
                    Image = book.BookImage,
                    Quantity = 1
                });
            }
            else
            {
                cartItem.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson("ShoppingCart", cart);

            // إرسال رسالة نجاح تظهر للمستخدم
            TempData["SuccessMessage"] = $"Added '{book.Title}' to cart!";

            // الرجوع إلى الصفحة التي جاء منها المستخدم
            string returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Books");
        }


        // حذف عنصر من السلة
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("ShoppingCart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.BookId == id);
            if (item != null) cart.Remove(item);

            HttpContext.Session.SetObjectAsJson("ShoppingCart", cart);
            return RedirectToAction("Index");
        }






    }
}



