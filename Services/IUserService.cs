using pharmacareAPI.Models;
using System.Collections.Generic;

namespace pharmacareAPI.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user);
        Task<string> LoginAsync(string mobileNumber, string password);
        Task<IEnumerable<User>> GetAllUsersAsync(); // New method to fetch all users
    }
}