using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public bool IsLocked { get; set; } = false;

        public UserProfile Profile { get; set; }

    }
}
