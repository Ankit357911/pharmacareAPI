using System.ComponentModel.DataAnnotations;

namespace pharmacareAPI.DTOs
{
    public class CreateEventDto
    {
        [Required]
        [MaxLength(255)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string MedicineOrCategory { get; set; } = string.Empty;

        [Range(0, 100)]
        public int DiscountPercentage { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
