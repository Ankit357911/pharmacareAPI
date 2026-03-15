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
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
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

                return Ok(new UserAccountDto
                {
                    AccountId = createdUser.AccountID,
                    FullName = createdUser.FullName,
                    MobileNumber = createdUser.MobileNumber,
                    Email = createdUser.Email,
                    AccountType = createdUser.AccountType.ToString()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while registering user with mobile '{MobileNumber}'.", dto.MobileNumber);
                return StatusCode(500, new { message = "An unexpected error occurred." });
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
            catch (InvalidOperationException)
            {
                return Unauthorized(new { message = "Invalid mobile number or password." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging in mobile '{MobileNumber}'.", dto.MobileNumber);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("staff")]
        public async Task<IActionResult> GetStaffAccounts()
        {
            var staff = await _userService.GetStaffAccountsAsync();
            return Ok(staff);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("staff")]
        public async Task<IActionResult> CreateStaffAccount([FromBody] CreateStaffAccountDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = await _userService.CreateStaffAccountAsync(dto);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating staff account for mobile '{MobileNumber}'.", dto.MobileNumber);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("staff/{accountId:int}")]
        public async Task<IActionResult> UpdateStaffAccount(int accountId, [FromBody] UpdateStaffAccountDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var updated = await _userService.UpdateStaffAccountAsync(accountId, dto);
                if (updated == null)
                    return NotFound("Staff account not found.");

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating staff account '{AccountId}'.", accountId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("staff/{accountId:int}")]
        public async Task<IActionResult> DeleteStaffAccount(int accountId)
        {
            var deleted = await _userService.DeleteStaffAccountAsync(accountId);
            if (!deleted)
                return NotFound("Staff account not found.");

            return Ok(new { message = "Staff account deleted successfully." });
        }


    }
}