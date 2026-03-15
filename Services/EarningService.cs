using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Data;
using pharmacareAPI.DTOs;
using pharmacareAPI.Models;

namespace pharmacareAPI.Services
{
    public class EarningService : IEarningService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EarningService> _logger;

        public EarningService(ApplicationDbContext context, ILogger<EarningService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EarningsSummaryDto> GetCurrentSummaryAsync()
        {
            var now = DateTime.UtcNow;

            var weekStart = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Sunday).Date;
            var weekEnd = weekStart.AddDays(6);

            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var weekly = await _context.Earnings
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.PeriodType == PeriodType.Weekly &&
                    e.PeriodStart == weekStart &&
                    e.PeriodEnd == weekEnd);

            var monthly = await _context.Earnings
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.PeriodType == PeriodType.Monthly &&
                    e.PeriodStart == monthStart &&
                    e.PeriodEnd == monthEnd);

            var result = new EarningsSummaryDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                WeeklyEarnings = weekly?.Earnings ?? 0m,
                WeeklyProfit = weekly?.Profit ?? 0m,
                MonthStart = monthStart,
                MonthEnd = monthEnd,
                MonthlyEarnings = monthly?.Earnings ?? 0m,
                MonthlyProfit = monthly?.Profit ?? 0m
            };

            _logger.LogInformation(
                "Loaded earnings summary. Weekly(Earnings={WeeklyEarnings}, Profit={WeeklyProfit}), Monthly(Earnings={MonthlyEarnings}, Profit={MonthlyProfit}).",
                result.WeeklyEarnings,
                result.WeeklyProfit,
                result.MonthlyEarnings,
                result.MonthlyProfit);

            return result;
        }
    }
}
