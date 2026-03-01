using System.ComponentModel.DataAnnotations;
using pharmacareAPI.Models;

namespace pharmacareAPI.DTOs
{
    public class RegisterUserDto
    {
        [MaxLength(100)]
        public required string FullName { get; set; }

        [MaxLength(15)]
        public required string MobileNumber { get; set; }

        [MaxLength(255)]
        public required string Password { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        public string? ProfilePicturePath { get; set; }
    }
}