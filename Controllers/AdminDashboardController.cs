using BookStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // جلب إحصائيات سريعة للعرض
            ViewBag.TotalBooks = _context.Books.Count();
            ViewBag.TotalUsers = _context.Users.Count(u => u.Role.RoleName == "User");
            ViewBag.LowStock = _context.Books.Count(b => b.Quantity < 5);

            return View();
        }
    }
}
