using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Data;
using pharmacareAPI.DTOs;
using pharmacareAPI.Models;

namespace pharmacareAPI.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OngoingEventDto?> GetOngoingEventAsync()
        {
            var ev = await _context.Events
                .Where(e => e.ExpiryDate > DateTime.UtcNow)
                .OrderBy(e => e.ExpiryDate)
                .FirstOrDefaultAsync();

            if (ev == null)
                return null;

            return new OngoingEventDto
            {
                EventName = ev.EventName,
                MedicineOrCategory = ev.MedicineOrCategory,
                DiscountPercentage = ev.DiscountPercentage
            };
        }
    }
}