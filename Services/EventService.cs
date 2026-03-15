using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Data;
using pharmacareAPI.DTOs;
using pharmacareAPI.Models;

namespace pharmacareAPI.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventService> _logger;

        public EventService(ApplicationDbContext context, ILogger<EventService> logger)
        {
            _context = context;
            _logger = logger;
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

        public async Task<List<string>> GetMedicineOrCategoryOptionsAsync()
        {
            var medicineNames = await _context.Medicines
                .AsNoTracking()
                .Select(m => m.Name)
                .ToListAsync();

            var categoryNames = await _context.MedicineCategories
                .AsNoTracking()
                .Select(c => c.CategoryName)
                .ToListAsync();

            return medicineNames
                .Concat(categoryNames)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        public async Task<OngoingEventDto> CreateEventAsync(CreateEventDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.EventName))
                throw new InvalidOperationException("Event name is required.");

            if (string.IsNullOrWhiteSpace(dto.MedicineOrCategory))
                throw new InvalidOperationException("Medicine or category is required.");

            if (dto.DiscountPercentage < 0 || dto.DiscountPercentage > 100)
                throw new InvalidOperationException("Discount percentage must be between 0 and 100.");

            if (dto.ExpiryDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Expiry date must be in the future.");

            var target = dto.MedicineOrCategory.Trim().ToLower();

            var exists = await _context.Medicines.AnyAsync(m => m.Name.ToLower() == target)
                || await _context.MedicineCategories.AnyAsync(c => c.CategoryName.ToLower() == target);

            if (!exists)
                throw new InvalidOperationException("Medicine or category does not exist.");

            var ev = new Event
            {
                EventName = dto.EventName.Trim(),
                MedicineOrCategory = dto.MedicineOrCategory.Trim(),
                DiscountPercentage = dto.DiscountPercentage,
                ExpiryDate = dto.ExpiryDate
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created event '{EventName}' for '{MedicineOrCategory}'.", ev.EventName, ev.MedicineOrCategory);

            return new OngoingEventDto
            {
                EventName = ev.EventName,
                MedicineOrCategory = ev.MedicineOrCategory,
                DiscountPercentage = ev.DiscountPercentage
            };
        }

        public async Task<bool> DeleteOngoingEventAsync()
        {
            var ev = await _context.Events
                .Where(e => e.ExpiryDate > DateTime.UtcNow)
                .OrderBy(e => e.ExpiryDate)
                .FirstOrDefaultAsync();

            if (ev == null)
                return false;

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted ongoing event '{EventName}'.", ev.EventName);
            return true;
        }
    }
}