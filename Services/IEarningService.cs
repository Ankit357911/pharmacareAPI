using pharmacareAPI.DTOs;

namespace pharmacareAPI.Services
{
    public interface IEarningService
    {
        Task<EarningsSummaryDto> GetCurrentSummaryAsync();
    }
}
