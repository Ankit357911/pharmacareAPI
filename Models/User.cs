using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacareAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountID { get; set; }

        [MaxLength(100)]
        public required string FullName { get; set; }

        [MaxLength(15)]
        public required string MobileNumber { get; set; }

        [MaxLength(255)]
        public required string PasswordHash { get; set; } 

        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        [MaxLength(255)]
        public string? ProfilePicturePath { get; set; }
    }
}