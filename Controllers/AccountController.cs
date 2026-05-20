using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();  
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {

            if(ModelState.IsValid)
            {
                var hasher = new PasswordHasher<User>();
                var hashedPassword = hasher.HashPassword(null, model.Password);

                User user = new()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = hashedPassword,
                    RoleId = 2
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var newProfile = new UserProfile
                {
                    UserId = user.UserId, 
                    FullName = null, 
                    CreatedAt = DateTime.Now,
                };

                _context.UserProfiles.Add(newProfile);
                _context.SaveChanges();

                return RedirectToAction("Login","Account");
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == model.Email);

            if (user != null)
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Password, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    if (user.IsLocked)
                    {
                        ModelState.AddModelError("", "This account is blocked.");
                        return View(model);
                    }

                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("Role", user.Role.RoleName);
                    HttpContext.Session.SetString("UserName", user.UserName); 

                    if (user.Role.RoleName == "Admin")
                    {
                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Books");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid Email or Password.");
            return View(model);
        }



    }
}
