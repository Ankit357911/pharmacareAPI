namespace pharmacareAPI.DTOs
{
    public class TransactionResponseDto
    {
        public string TransactionCode { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? CustomerName { get; set; }
        public string? MobileNumber { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal Profit { get; set; }
        public List<TransactionItemResponseDto> Items { get; set; } = new();
    }

    public class TransactionItemResponseDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}