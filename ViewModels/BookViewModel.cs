using BookStore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        [Required]
        public decimal Price { get; set; }

        [Display(Name = "Book Image")]
        public string? BookImage { get; set; }  // 1. لعرض الصورة الحالية المخزنة سابقاً (للقراءة)
        [ValidateNever]
        public IFormFile ImageFile { get; set; }  // 2. لاستقبال الصورة الجديدة عند الرفع (للكتابة)
        public int? Quantity { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public List<Category> Categories { get; set; }
    }
}
