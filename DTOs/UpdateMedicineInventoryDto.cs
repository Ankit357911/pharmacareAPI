using System.ComponentModel.DataAnnotations;

namespace pharmacareAPI.DTOs
{
    public class UpdateMedicineInventoryDto
    {
        [Required]
        [MaxLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999999.99")]
        public decimal NewSellingRate { get; set; }

        [Range(0, int.MaxValue)]
        public int AddToStock { get; set; }
    }
}
