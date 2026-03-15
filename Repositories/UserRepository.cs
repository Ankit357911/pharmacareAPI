using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Data;
using pharmacareAPI.Models;
using System.Collections.Generic;

namespace pharmacareAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByMobileAsync(string mobileNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);
        }

        public async Task<User?> GetByIdAsync(int accountId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.AccountID == accountId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AdminExistsAsync()
        {
            return await _context.Users
                .AnyAsync(u => u.AccountType == AccountType.Admin);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync(); // Fetch all users from the database
        }
    }
}