using pharmacareAPI.Models;
using pharmacareAPI.Repositories;
using pharmacareAPI.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace pharmacareAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            // Business Rule: Only one Admin allowed
            if (user.AccountType == AccountType.Admin)
            {
                var adminExists = await _userRepository.AdminExistsAsync();

                if (adminExists)
                    throw new InvalidOperationException("Only one Admin is allowed.");
            }

            // Check duplicate mobile number
            var existingUser = await _userRepository.GetByMobileAsync(user.MobileNumber);
            if (existingUser != null)
                throw new InvalidOperationException("Mobile number already registered.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<string> LoginAsync(string mobileNumber, string password)
        {
            var user = await _userRepository.GetByMobileAsync(mobileNumber);

            if (user == null)
                throw new InvalidOperationException("Invalid mobile number or password.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordValid)
                throw new InvalidOperationException("Invalid mobile number or password.");

            // Generate JWT
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString()),
                new Claim(ClaimTypes.Role, user.AccountType.ToString()),
                new Claim("FullName", user.FullName),
                new Claim("Email", user.Email),
                new Claim("MobileNumber", user.MobileNumber)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<List<UserAccountDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users
                .OrderBy(u => u.FullName)
                .Select(u => new UserAccountDto
                {
                    AccountId = u.AccountID,
                    FullName = u.FullName,
                    MobileNumber = u.MobileNumber,
                    Email = u.Email,
                    AccountType = u.AccountType.ToString()
                })
                .ToList();
        }

        public async Task<List<StaffAccountDto>> GetStaffAccountsAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users
                .Where(u => u.AccountType == AccountType.Staff)
                .OrderBy(u => u.FullName)
                .Select(u => new StaffAccountDto
                {
                    AccountId = u.AccountID,
                    FullName = u.FullName,
                    MobileNumber = u.MobileNumber,
                    Email = u.Email,
                    AccountType = u.AccountType.ToString()
                })
                .ToList();
        }

        public async Task<StaffAccountDto> CreateStaffAccountAsync(CreateStaffAccountDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                MobileNumber = dto.MobileNumber.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                PasswordHash = dto.Password,
                AccountType = AccountType.Staff,
                ProfilePicturePath = null
            };

            var created = await RegisterUserAsync(user);

            return new StaffAccountDto
            {
                AccountId = created.AccountID,
                FullName = created.FullName,
                MobileNumber = created.MobileNumber,
                Email = created.Email,
                AccountType = created.AccountType.ToString()
            };
        }

        public async Task<StaffAccountDto?> UpdateStaffAccountAsync(int accountId, UpdateStaffAccountDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _userRepository.GetByIdAsync(accountId);
            if (user == null || user.AccountType != AccountType.Staff)
                return null;

            var normalizedMobile = dto.MobileNumber.Trim();
            var existingWithMobile = await _userRepository.GetByMobileAsync(normalizedMobile);

            if (existingWithMobile != null && existingWithMobile.AccountID != accountId)
                throw new InvalidOperationException("Mobile number already registered.");

            user.FullName = dto.FullName.Trim();
            user.MobileNumber = normalizedMobile;
            user.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.UpdateUserAsync(user);

            return new StaffAccountDto
            {
                AccountId = user.AccountID,
                FullName = user.FullName,
                MobileNumber = user.MobileNumber,
                Email = user.Email,
                AccountType = user.AccountType.ToString()
            };
        }

        public async Task<bool> DeleteStaffAccountAsync(int accountId)
        {
            var user = await _userRepository.GetByIdAsync(accountId);
            if (user == null || user.AccountType != AccountType.Staff)
                return false;

            await _userRepository.DeleteUserAsync(user);
            return true;
        }
    }
}