using pharmacareAPI.DTOs;

namespace pharmacareAPI.Services
{
    public interface IEventService
    {
        Task<OngoingEventDto?> GetOngoingEventAsync();
        Task<List<string>> GetMedicineOrCategoryOptionsAsync();
        Task<OngoingEventDto> CreateEventAsync(CreateEventDto dto);
        Task<bool> DeleteOngoingEventAsync();
    }
}