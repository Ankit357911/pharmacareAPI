using pharmacareAPI.DTOs;

namespace pharmacareAPI.Services
{
    public interface IMedicineService
    {
        Task<List<string>> GetMedicineNamesAsync();
        Task<MedicineDetailsDto?> GetMedicineDetailsAsync(string name);
    }
}