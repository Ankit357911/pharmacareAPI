namespace pharmacareAPI.DTOs
{
    public class StaffAccountDto
    {
        public int AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string AccountType { get; set; } = string.Empty;
    }
}
