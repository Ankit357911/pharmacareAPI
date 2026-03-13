using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacareAPI.Models
{
    public class TransactionItem
    {
        [Key]
        public int Id { get; set; }

        // FK Transaction
        public int TransactionId { get; set; }
        public required Transaction Transaction { get; set; }

        // FK Medicine
        public int MedicineId { get; set; }
        public required Medicine Medicine { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
    }
}