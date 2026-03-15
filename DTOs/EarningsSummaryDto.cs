namespace pharmacareAPI.DTOs
{
    public class EarningsSummaryDto
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public decimal WeeklyEarnings { get; set; }
        public decimal WeeklyProfit { get; set; }

        public DateTime MonthStart { get; set; }
        public DateTime MonthEnd { get; set; }
        public decimal MonthlyEarnings { get; set; }
        public decimal MonthlyProfit { get; set; }
    }
}
