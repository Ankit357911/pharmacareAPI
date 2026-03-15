using pharmacareAPI.Models;
using System.Collections.Generic;

namespace pharmacareAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User?> GetByMobileAsync(string mobileNumber);
        Task<User?> GetByIdAsync(int accountId);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<bool> AdminExistsAsync();
        Task<IEnumerable<User>> GetAllUsersAsync(); //method to fetch all users
    }
}