namespace pharmacareAPI.DTOs
{
    public class RemoveMedicineStockResultDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public int RemovedQuantity { get; set; }
        public int RemainingStock { get; set; }
        public bool Deleted { get; set; }
    }
}
