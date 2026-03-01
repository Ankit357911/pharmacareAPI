using Microsoft.AspNetCore.Mvc;
using pharmacareAPI.Models;
using pharmacareAPI.Services;
using pharmacareAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace pharmacareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            try
            {
                var user = new User
                {
                    FullName = dto.FullName,
                    MobileNumber = dto.MobileNumber,
                    PasswordHash = dto.Password,
                    Email = dto.Email,
                    AccountType = dto.AccountType,
                    ProfilePicturePath = dto.ProfilePicturePath
                };

                var createdUser = await _userService.RegisterUserAsync(user);

                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var token = await _userService.LoginAsync(dto.MobileNumber, dto.Password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync(); // Fetch all users
            return Ok(users);
        }


    }
}