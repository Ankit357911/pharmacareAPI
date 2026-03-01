using pharmacareAPI.Models;
using pharmacareAPI.Repositories;
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
                    throw new Exception("Only one Admin is allowed.");
            }

            // Check duplicate mobile number
            var existingUser = await _userRepository.GetByMobileAsync(user.MobileNumber);
            if (existingUser != null)
                throw new Exception("Mobile number already registered.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<string> LoginAsync(string mobileNumber, string password)
        {
            var user = await _userRepository.GetByMobileAsync(mobileNumber);

            if (user == null)
                throw new Exception("Invalid mobile number or password.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordValid)
                throw new Exception("Invalid mobile number or password.");

            // Generate JWT
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString()),
                new Claim(ClaimTypes.Role, user.AccountType.ToString())
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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync(); // Fetch all users from repository
        }
    }
}