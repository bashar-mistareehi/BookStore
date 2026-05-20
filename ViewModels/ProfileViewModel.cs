using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        public string? ExistingImagePath { get; set; }
        public IFormFile? NewImageFile { get; set; }

        [MaxLength(10, ErrorMessage = "Phone number cannot exceed 10 digits")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Country { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }

   
}
