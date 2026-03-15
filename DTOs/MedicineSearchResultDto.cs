namespace pharmacareAPI.DTOs
{
    public class MedicineSearchResultDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal SellingRate { get; set; }
        public int Stock { get; set; }
    }
}
