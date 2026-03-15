namespace pharmacareAPI.DTOs
{
    public class RecentTransactionDto
    {
        public DateTime TransactionDate { get; set; }
        public string? CustomerName { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
