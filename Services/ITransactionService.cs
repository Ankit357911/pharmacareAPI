using pharmacareAPI.DTOs;
namespace pharmacareAPI.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> CreateTransactionAsync(int accountId, CreateTransactionDto dto);
        Task<TransactionResponseDto?> GetTransactionByCodeAsync(string transactionCode);
        Task<List<TransactionResponseDto>> GetUserTransactionsAsync(int accountId, int page = 1, int pageSize = 10);
        Task<List<RecentTransactionDto>> GetRecentTransactionsAsync(int take = 10);
        Task<List<RecentTransactionDto>> SearchTransactionsByCustomerAsync(string customerName, int take = 50);
        Task<MostSoldMedicineDto?> GetMostSoldMedicineAsync();
        Task<List<WeeklyEarningsPointDto>> GetWeeklyEarningsAsync();
    }
}
