namespace pharmacareAPI.DTOs
{
    public class UpsertMedicineResultDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool CategoryCreated { get; set; }
        public bool MedicineCreated { get; set; }
        public int CurrentStock { get; set; }
    }
}
