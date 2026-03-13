using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Data;
using pharmacareAPI.DTOs;

namespace pharmacareAPI.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly ApplicationDbContext _context;

        public MedicineService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetMedicineNamesAsync()
        {
            return await _context.Medicines
                .Select(m => m.Name)
                .ToListAsync();
        }

        public async Task<MedicineDetailsDto?> GetMedicineDetailsAsync(string name)
        {
            var medicine = await _context.Medicines
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Name == name);

            if (medicine == null)
                return null;

            int discount = await _context.Events
                .Where(e =>
                    (e.MedicineOrCategory == medicine.Name ||
                     e.MedicineOrCategory == medicine.Category.CategoryName)
                    && e.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(e => e.DiscountPercentage)
                .Select(e => e.DiscountPercentage)
                .FirstOrDefaultAsync();

            decimal finalPrice = medicine.SellingRate;

            if (discount > 0)
                finalPrice = medicine.SellingRate * (1 - (discount / 100m));

            return new MedicineDetailsDto
            {
                Name = medicine.Name,
                Price = medicine.SellingRate,
                Stock = medicine.Stock,
                ManufacturingDate = medicine.ManufacturingDate ?? DateTime.MinValue,
                ExpiryDate = medicine.ExpiryDate ?? DateTime.MinValue,
                DiscountPercentage = discount,
                FinalPrice = finalPrice
            };
        }
    }
}