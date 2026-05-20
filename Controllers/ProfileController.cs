using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index() 
            {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdString);

                var user = _context.Users
                 .Include(u => u.Profile)
                 .FirstOrDefault(u => u.UserId == userId);

                if (user == null || user.Profile == null)
                {
                    return NotFound("complete your account first");
                }

                var model = new ProfileViewModel
                {
                    Email = user.Email,
                    FullName = user.Profile.FullName,
                    ExistingImagePath = user.Profile.ExistingImage,

                    Phone = user.Profile?.Phone,
                    Address = user.Profile?.Address,
                    Country = user.Profile?.Country
                };

                return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) 
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdString);

            var user = _context.Users
                .Include(u => u.Profile)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.Profile == null)
                return NotFound();

            if (model.NewImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");

                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewImageFile.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(user.Profile.ExistingImage))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Profile.ExistingImage);
                    if (System.IO.File.Exists(oldPath) && !oldPath.Contains("default-avatar.png"))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                user.Profile.ExistingImage = "images/profiles/" + uniqueFileName;
            }

            user.Profile.FullName = model.FullName;
            user.Profile.Phone = model.Phone;
            user.Profile.Address = model.Address;
            user.Profile.Country = model.Country;
            user.Email = model.Email; 

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Index","Profile");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,Password,Role,Email")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
