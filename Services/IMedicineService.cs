using pharmacareAPI.DTOs;

namespace pharmacareAPI.Services
{
    public interface IMedicineService
    {
        Task<List<string>> GetMedicineNamesAsync();
        Task<MedicineDetailsDto?> GetMedicineDetailsAsync(string name);
        Task<List<MedicineCategoryDto>> GetCategoriesAsync();
        Task<bool> UpdateMedicineRateAndStockAsync(UpdateMedicineInventoryDto dto);
        Task<MedicineStockDto?> GetMedicineStockAsync(string medicineName);
        Task<RemoveMedicineStockResultDto?> RemoveMedicineStockAsync(RemoveMedicineStockDto dto);
        Task<UpsertMedicineResultDto> UpsertMedicineAsync(UpsertMedicineDto dto);
        Task<List<MedicineSearchResultDto>> SearchMedicinesAsync(string query, int take);
    }
}