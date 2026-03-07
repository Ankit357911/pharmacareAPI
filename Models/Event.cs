using System.ComponentModel.DataAnnotations;

namespace pharmacareAPI.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [MaxLength(255)]
        public required string EventName { get; set; }

        [Required]
        [MaxLength(255)]
        public required string MedicineOrCategory { get; set; }

        [Required]
        public int DiscountPercentage { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}