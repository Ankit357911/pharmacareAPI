using System.ComponentModel.DataAnnotations;

namespace pharmacareAPI.DTOs
{
    public class RemoveMedicineStockDto
    {
        [Required]
        [MaxLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int RemoveFromStock { get; set; }

        public bool RemoveAll { get; set; }
    }
}
