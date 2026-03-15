using System.ComponentModel.DataAnnotations;

namespace pharmacareAPI.DTOs
{
    public class UpsertMedicineDto
    {
        [Required]
        [MaxLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999999.99")]
        public decimal PurchaseRate { get; set; }

        [Range(typeof(decimal), "0.01", "999999.99")]
        public decimal SellingRate { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public DateTime ManufacturingDate { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
