using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Data;
using pharmacareAPI.DTOs;
using Microsoft.Extensions.Logging;

namespace pharmacareAPI.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MedicineService> _logger;

        public MedicineService(ApplicationDbContext context, ILogger<MedicineService> logger)
        {
            _context = context;
            _logger = logger;
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

        public async Task<List<MedicineCategoryDto>> GetCategoriesAsync()
        {
            _logger.LogInformation("Loading medicine categories.");

            var categories = await _context.MedicineCategories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .Select(c => new MedicineCategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            _logger.LogInformation("Loaded {Count} medicine categories.", categories.Count);
            return categories;
        }

        public async Task<bool> UpdateMedicineRateAndStockAsync(UpdateMedicineInventoryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.MedicineName))
                throw new InvalidOperationException("Medicine name is required.");

            if (dto.NewSellingRate <= 0)
                throw new InvalidOperationException("New selling rate must be greater than zero.");

            if (dto.AddToStock < 0)
                throw new InvalidOperationException("Stock increment cannot be negative.");

            _logger.LogInformation(
                "Updating medicine '{MedicineName}' with selling rate {NewSellingRate} and stock increment {AddToStock}.",
                dto.MedicineName,
                dto.NewSellingRate,
                dto.AddToStock);

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Name == dto.MedicineName);

            if (medicine == null)
            {
                _logger.LogWarning("Medicine '{MedicineName}' was not found for update.", dto.MedicineName);
                return false;
            }

            medicine.SellingRate = dto.NewSellingRate;
            medicine.Stock += dto.AddToStock;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Medicine '{MedicineName}' updated successfully.", dto.MedicineName);
            return true;
        }

        public async Task<MedicineStockDto?> GetMedicineStockAsync(string medicineName)
        {
            if (string.IsNullOrWhiteSpace(medicineName))
                throw new InvalidOperationException("Medicine name is required.");

            var normalizedName = medicineName.Trim().ToLower();

            var medicine = await _context.Medicines
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Name.ToLower() == normalizedName);

            if (medicine == null)
            {
                _logger.LogWarning("Medicine '{MedicineName}' was not found for stock lookup.", medicineName);
                return null;
            }

            return new MedicineStockDto
            {
                MedicineName = medicine.Name,
                Stock = medicine.Stock
            };
        }

        public async Task<RemoveMedicineStockResultDto?> RemoveMedicineStockAsync(RemoveMedicineStockDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.MedicineName))
                throw new InvalidOperationException("Medicine name is required.");

            if (!dto.RemoveAll && dto.RemoveFromStock <= 0)
                throw new InvalidOperationException("Remove quantity must be greater than zero when RemoveAll is false.");

            var normalizedName = dto.MedicineName.Trim().ToLower();

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Name.ToLower() == normalizedName);

            if (medicine == null)
            {
                _logger.LogWarning("Medicine '{MedicineName}' was not found for removal.", dto.MedicineName);
                return null;
            }

            if (dto.RemoveAll)
            {
                var removedQuantity = medicine.Stock;
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Medicine '{MedicineName}' removed completely from inventory.", medicine.Name);

                return new RemoveMedicineStockResultDto
                {
                    MedicineName = medicine.Name,
                    RemovedQuantity = removedQuantity,
                    RemainingStock = 0,
                    Deleted = true
                };
            }

            if (dto.RemoveFromStock > medicine.Stock)
                throw new InvalidOperationException($"Cannot remove {dto.RemoveFromStock} units. Only {medicine.Stock} units available.");

            medicine.Stock -= dto.RemoveFromStock;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Removed {RemovedQuantity} units from medicine '{MedicineName}'. Remaining stock: {RemainingStock}.",
                dto.RemoveFromStock,
                medicine.Name,
                medicine.Stock);

            return new RemoveMedicineStockResultDto
            {
                MedicineName = medicine.Name,
                RemovedQuantity = dto.RemoveFromStock,
                RemainingStock = medicine.Stock,
                Deleted = false
            };
        }

        public async Task<UpsertMedicineResultDto> UpsertMedicineAsync(UpsertMedicineDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.MedicineName))
                throw new InvalidOperationException("Medicine name is required.");

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                throw new InvalidOperationException("Category name is required.");

            if (dto.PurchaseRate <= 0)
                throw new InvalidOperationException("Purchase rate must be greater than zero.");

            if (dto.SellingRate <= 0)
                throw new InvalidOperationException("Selling rate must be greater than zero.");

            if (dto.Stock < 0)
                throw new InvalidOperationException("Stock cannot be negative.");

            if (dto.ExpiryDate.Date < dto.ManufacturingDate.Date)
                throw new InvalidOperationException("Expiry date cannot be earlier than manufacturing date.");

            var normalizedCategoryName = dto.CategoryName.Trim().ToLower();
            var normalizedMedicineName = dto.MedicineName.Trim().ToLower();

            var category = await _context.MedicineCategories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == normalizedCategoryName);

            var categoryCreated = false;
            if (category == null)
            {
                category = new Models.MedicineCategory
                {
                    CategoryName = dto.CategoryName.Trim()
                };

                _context.MedicineCategories.Add(category);
                await _context.SaveChangesAsync();
                categoryCreated = true;

                _logger.LogInformation("Created new medicine category '{CategoryName}'.", category.CategoryName);
            }

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Name.ToLower() == normalizedMedicineName);

            var medicineCreated = false;

            if (medicine == null)
            {
                medicine = new Models.Medicine
                {
                    Name = dto.MedicineName.Trim(),
                    CategoryId = category.CategoryId,
                    PurchaseRate = dto.PurchaseRate,
                    SellingRate = dto.SellingRate,
                    Stock = dto.Stock,
                    ManufacturingDate = dto.ManufacturingDate.Date,
                    ExpiryDate = dto.ExpiryDate.Date,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Medicines.Add(medicine);
                medicineCreated = true;
            }
            else
            {
                medicine.CategoryId = category.CategoryId;
                medicine.PurchaseRate = dto.PurchaseRate;
                medicine.SellingRate = dto.SellingRate;
                medicine.Stock += dto.Stock;
                medicine.ManufacturingDate = dto.ManufacturingDate.Date;
                medicine.ExpiryDate = dto.ExpiryDate.Date;
                medicine.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Upsert completed for medicine '{MedicineName}'. CategoryCreated={CategoryCreated}, MedicineCreated={MedicineCreated}, CurrentStock={CurrentStock}",
                medicine.Name,
                categoryCreated,
                medicineCreated,
                medicine.Stock);

            return new UpsertMedicineResultDto
            {
                MedicineName = medicine.Name,
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryCreated = categoryCreated,
                MedicineCreated = medicineCreated,
                CurrentStock = medicine.Stock
            };
        }

        public async Task<List<MedicineSearchResultDto>> SearchMedicinesAsync(string query, int take)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new InvalidOperationException("Search query is required.");

            if (take < 1 || take > 50)
                throw new InvalidOperationException("take must be between 1 and 50.");

            var normalizedQuery = query.Trim().ToLower();

            var result = await _context.Medicines
                .AsNoTracking()
                .Where(m => m.Name.ToLower().Contains(normalizedQuery))
                .OrderBy(m => m.Name)
                .Take(take)
                .Select(m => new MedicineSearchResultDto
                {
                    Name = m.Name,
                    SellingRate = m.SellingRate,
                    Stock = m.Stock
                })
                .ToListAsync();

            _logger.LogInformation("Medicine search query '{Query}' returned {Count} row(s).", query, result.Count);

            return result;
        }
    }
}