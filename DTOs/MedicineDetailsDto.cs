namespace pharmacareAPI.DTOs
{
    public class MedicineDetailsDto
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
    }
}