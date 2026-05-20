using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BookStore.Controllers
{
    public class AdminBooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminBooksController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
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

        public async Task<IActionResult> Index()
        {
            var allData = await _context.Books.Include(b => b.Category).ToListAsync();
            return View(allData);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new BookViewModel
            {
                Categories = _context.Categories.ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook(BookViewModel bookVM)
        {
            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    Title = bookVM.Title,
                    Description = bookVM.Description,
                    Author = bookVM.Author,
                    Price = bookVM.Price,
                    BookImage = UploadFile(bookVM.ImageFile),
                    Quantity = bookVM.Quantity,
                    CategoryId = bookVM.CategoryId
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            bookVM.Categories = _context.Categories.ToList();
            return View(bookVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var bookVM = new BookViewModel
            {

                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author,
                Price = book.Price,
                BookImage = book.BookImage,
                Quantity = book.Quantity,
                CategoryId = book.CategoryId

            };


            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", book.CategoryId);
            return View(bookVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel bookVM)
        {
            if (id != bookVM.BookId) return NotFound();

            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    BookId = bookVM.BookId, // هذا السطر هو مفتاح الحل لكي لا يتحول لـ Add
                    Title = bookVM.Title,
                    Description = bookVM.Description,
                    Author = bookVM.Author,
                    Price = bookVM.Price,
                    BookImage = UploadFile(bookVM.ImageFile),
                    Quantity = bookVM.Quantity,
                    CategoryId = bookVM.CategoryId
                };

                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(e => e.BookId == book.BookId)) return NotFound();
                    throw;
                }
            }

            bookVM.Categories = _context.Categories.ToList();
            return View(bookVM);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
