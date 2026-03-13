namespace pharmacareAPI.DTOs
{
    public class CreateTransactionDto
    {
        public string? CustomerName { get; set; }
        public string? MobileNumber { get; set; }
        public List<TransactionItemDto> Items { get; set; } = new();
    }

    public class TransactionItemDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}