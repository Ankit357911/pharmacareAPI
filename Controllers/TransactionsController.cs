using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacareAPI.DTOs;
using pharmacareAPI.Services;
using System.Security.Claims;

namespace pharmacareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Create a new transaction/bill
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            try
            {
                // Get accountId from JWT claims
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                    return Unauthorized("Invalid token.");

                var result = await _transactionService.CreateTransactionAsync(accountId, dto);
                return CreatedAtAction(nameof(GetTransactionByCode), new { code = result.TransactionCode }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the transaction: {ex.Message}");
            }
        }

        /// <summary>
        /// Get transaction details by transaction code
        /// </summary>
        [Authorize]
        [HttpGet("{code}")]
        public async Task<IActionResult> GetTransactionByCode(string code)
        {
            var result = await _transactionService.GetTransactionByCodeAsync(code);

            if (result == null)
                return NotFound("Transaction not found.");

            return Ok(result);
        }

        /// <summary>
        /// Get current user's transactions with pagination
        /// </summary>
        [Authorize]
        [HttpGet("my-transactions")]
        public async Task<IActionResult> GetMyTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                return Unauthorized("Invalid token.");

            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Invalid pagination parameters.");

            var result = await _transactionService.GetUserTransactionsAsync(accountId, page, pageSize);
            return Ok(result);
        }
    }
}