using pharmacareAPI.Models;
using System.Collections.Generic;

namespace pharmacareAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User?> GetByMobileAsync(string mobileNumber);
        Task<bool> AdminExistsAsync();
        Task<IEnumerable<User>> GetAllUsersAsync(); //method to fetch all users
    }
}