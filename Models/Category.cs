using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Display(Name ="Category")]
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
