using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacareAPI.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string TransactionCode { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        // Foreign Key to User
        public int AccountId { get; set; }

        public User Account { get; set; }

        [MaxLength(255)]
        public string? CustomerName { get; set; }

        [MaxLength(15)]
        public string? MobileNumber { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal GrandTotal { get; set; } = 0.00m;

        // Navigation
        public ICollection<TransactionItem>? TransactionItems { get; set; }
    }
}