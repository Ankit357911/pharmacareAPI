using pharmacareAPI.Models;
using pharmacareAPI.DTOs;
using System.Collections.Generic;

namespace pharmacareAPI.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user);
        Task<string> LoginAsync(string mobileNumber, string password);
        Task<List<UserAccountDto>> GetAllUsersAsync();
        Task<List<StaffAccountDto>> GetStaffAccountsAsync();
        Task<StaffAccountDto> CreateStaffAccountAsync(CreateStaffAccountDto dto);
        Task<StaffAccountDto?> UpdateStaffAccountAsync(int accountId, UpdateStaffAccountDto dto);
        Task<bool> DeleteStaffAccountAsync(int accountId);
    }
}