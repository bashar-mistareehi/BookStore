using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class UserProfile
    {
        [Key]
        public int UserProfileId { get; set; }
        public string? FullName { get; set; }
       public string? ExistingImage { get; set; }
        [MaxLength(10)]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
