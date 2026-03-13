using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Data;
using pharmacareAPI.DTOs;
using pharmacareAPI.Models;

namespace pharmacareAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionResponseDto> CreateTransactionAsync(int accountId, CreateTransactionDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new InvalidOperationException("Transaction must contain at least one item.");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var transaction = new Transaction
                {
                    TransactionCode = Guid.NewGuid().ToString(),
                    TransactionDate = DateTime.UtcNow,
                    AccountId = accountId,
                    Account = null!,
                    CustomerName = dto.CustomerName,
                    MobileNumber = dto.MobileNumber,
                    TransactionItems = new List<TransactionItem>()
                };

                decimal totalInvestment = 0m;
                decimal grandTotal = 0m;

                foreach (var item in dto.Items)
                {
                    // Get medicine with stock and pricing info
                    var medicine = await _context.Medicines
                        .FirstOrDefaultAsync(m => m.Name == item.MedicineName);

                    if (medicine == null)
                        throw new InvalidOperationException($"Medicine '{item.MedicineName}' not found.");

                    if (medicine.Stock < item.Quantity)
                        throw new InvalidOperationException(
                            $"Insufficient stock for '{item.MedicineName}'. Available: {medicine.Stock}, Requested: {item.Quantity}");

                    // Calculate prices
                    decimal itemTotal = medicine.SellingRate * item.Quantity;
                    decimal itemInvestment = medicine.PurchaseRate * item.Quantity;

                    // Update medicine stock
                    medicine.Stock -= item.Quantity;
                    medicine.UpdatedAt = DateTime.UtcNow;

                    // Create transaction item
                    var transactionItem = new TransactionItem
                    {
                        MedicineId = medicine.MedicineId,
                        Medicine = medicine,
                        Transaction = transaction,
                        Quantity = item.Quantity,
                        UnitPrice = medicine.SellingRate,
                        TotalPrice = itemTotal
                    };

                    transaction.TransactionItems.Add(transactionItem);

                    totalInvestment += itemInvestment;
                    grandTotal += itemTotal;
                }

                transaction.GrandTotal = grandTotal;
                decimal profit = grandTotal - totalInvestment;

                // Save transaction
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                // Update earnings
                await UpdateEarningsAsync(totalInvestment, grandTotal, profit);

                await dbTransaction.CommitAsync();

                // Return response
                return await GetTransactionResponseAsync(transaction, totalInvestment, profit);
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TransactionResponseDto?> GetTransactionByCodeAsync(string transactionCode)
        {
            var transaction = await _context.Transactions
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.Medicine)
                .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);

            if (transaction == null)
                return null;

            decimal totalInvestment = transaction.TransactionItems?.Sum(ti =>
                ti.Medicine.PurchaseRate * ti.Quantity) ?? 0;

            decimal profit = transaction.GrandTotal - totalInvestment;

            return await GetTransactionResponseAsync(transaction, totalInvestment, profit);
        }

        public async Task<List<TransactionResponseDto>> GetUserTransactionsAsync(int accountId, int page = 1, int pageSize = 10)
        {
            var transactions = await _context.Transactions
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.Medicine)
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var responses = new List<TransactionResponseDto>();

            foreach (var transaction in transactions)
            {
                decimal totalInvestment = transaction.TransactionItems?.Sum(ti =>
                    ti.Medicine.PurchaseRate * ti.Quantity) ?? 0;

                decimal profit = transaction.GrandTotal - totalInvestment;

                responses.Add(await GetTransactionResponseAsync(transaction, totalInvestment, profit));
            }

            return responses;
        }

        private async Task UpdateEarningsAsync(decimal investment, decimal earnings, decimal profit)
        {
            var now = DateTime.UtcNow;

            // Calculate period boundaries
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Sunday).Date;
            var endOfWeek = startOfWeek.AddDays(6);

            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var startOfYear = new DateTime(now.Year, 1, 1);
            var endOfYear = new DateTime(now.Year, 12, 31);

            var overallStart = new DateTime(2000, 1, 1);
            var overallEnd = new DateTime(9999, 12, 31);

            // Update or create earnings for each period
            await UpsertEarningAsync(PeriodType.Weekly, startOfWeek, endOfWeek, investment, earnings, profit);
            await UpsertEarningAsync(PeriodType.Monthly, startOfMonth, endOfMonth, investment, earnings, profit);
            await UpsertEarningAsync(PeriodType.Yearly, startOfYear, endOfYear, investment, earnings, profit);
            await UpsertEarningAsync(PeriodType.Overall, overallStart, overallEnd, investment, earnings, profit);
        }

        private async Task UpsertEarningAsync(
            PeriodType periodType,
            DateTime periodStart,
            DateTime periodEnd,
            decimal investment,
            decimal earnings,
            decimal profit)
        {
            var existing = await _context.Earnings
                .FirstOrDefaultAsync(e =>
                    e.PeriodType == periodType &&
                    e.PeriodStart == periodStart &&
                    e.PeriodEnd == periodEnd);

            if (existing != null)
            {
                existing.Investment += investment;
                existing.Earnings += earnings;
                existing.Profit += profit;
            }
            else
            {
                _context.Earnings.Add(new Earning
                {
                    PeriodType = periodType,
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd,
                    Investment = investment,
                    Earnings = earnings,
                    Profit = profit
                });
            }

            await _context.SaveChangesAsync();
        }

        private Task<TransactionResponseDto> GetTransactionResponseAsync(
            Transaction transaction,
            decimal totalInvestment,
            decimal profit)
        {
            return Task.FromResult(new TransactionResponseDto
            {
                TransactionCode = transaction.TransactionCode,
                TransactionDate = transaction.TransactionDate,
                CustomerName = transaction.CustomerName,
                MobileNumber = transaction.MobileNumber,
                GrandTotal = transaction.GrandTotal,
                TotalInvestment = totalInvestment,
                Profit = profit,
                Items = transaction.TransactionItems?.Select(ti => new TransactionItemResponseDto
                {
                    MedicineName = ti.Medicine?.Name ?? string.Empty,
                    Quantity = ti.Quantity,
                    UnitPrice = ti.UnitPrice,
                    TotalPrice = ti.TotalPrice
                }).ToList() ?? new List<TransactionItemResponseDto>()
            });
        }
    }
}
