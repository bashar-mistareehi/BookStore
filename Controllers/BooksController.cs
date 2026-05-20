using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        private string UploadFile(IFormFile file, string? oldImagePath = null)
        {
            if (file == null) return null;

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string uploadsFolder = Path.Combine(wwwRootPath, @"images/books");

            if (!string.IsNullOrEmpty(oldImagePath))
            {
                string oldFile = Path.Combine(wwwRootPath, oldImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
            }

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            using (var fileStream = new FileStream(Path.Combine(uploadsFolder, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return @"/images/books/" + fileName;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var booksQuery = _context.Books.Include(b => b.Category).AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                booksQuery = booksQuery.Where(b => b.CategoryId == categoryId);
                ViewBag.SelectedCategory = categoryId;
            }

            var books = await booksQuery.ToListAsync();
            return View(books);
        }


        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
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
                    Quantity = quantity 
                });
            }
            else
            {
                cartItem.Quantity += quantity; 
            }

            HttpContext.Session.SetObjectAsJson("ShoppingCart", cart);
            TempData["SuccessMessage"] = $"Added {quantity} copies of '{book.Title}' to cart!";

            return Redirect(Request.Headers["Referer"].ToString());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null) return NotFound();

            return View(book);
        }
    }
}
