using pharmacareAPI.DTOs;
using pharmacareAPI.Models;

namespace pharmacareAPI.Services
{
    public interface IEventService
    {
        Task<OngoingEventDto?> GetOngoingEventAsync();
    }
}