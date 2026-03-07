using System.ComponentModel.DataAnnotations;

namespace pharmacareAPI.Models
{
    public class MedicineCategory
    {
        [Key]
        public int CategoryId { get; set; }

        
        [MaxLength(50)]
        public required string CategoryName { get; set; }

        // Navigation Property
        public ICollection<Medicine>? Medicines { get; set; }
    }
}
