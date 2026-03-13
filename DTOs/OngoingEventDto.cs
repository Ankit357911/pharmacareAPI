namespace pharmacareAPI.DTOs
{
    public class OngoingEventDto
    {
        public string EventName { get; set; } = string.Empty;
        public string MedicineOrCategory { get; set; } = string.Empty;
        public int DiscountPercentage { get; set; }
    }
}