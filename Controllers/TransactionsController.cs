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

        
        /// Create a new transaction/bill
        
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

        
        /// Get transaction details by transaction code
      
        [Authorize]
        [HttpGet("{code}")]
        public async Task<IActionResult> GetTransactionByCode(string code)
        {
            var result = await _transactionService.GetTransactionByCodeAsync(code);

            if (result == null)
                return NotFound("Transaction not found.");

            return Ok(result);
        }

        
        /// Get current user's transactions with pagination
    
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

       
        /// Get recent transactions (flattened for history grid)
       
        [Authorize]
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentTransactions([FromQuery] int take = 10)
        {
            if (take < 1 || take > 100)
                return BadRequest("take must be between 1 and 100.");

            var result = await _transactionService.GetRecentTransactionsAsync(take);
            return Ok(result);
        }

        
        /// Search transactions by customer name (flattened for history grid)
       
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchTransactions([FromQuery] string customerName, [FromQuery] int take = 50)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                return BadRequest("customerName is required.");

            if (take < 1 || take > 200)
                return BadRequest("take must be between 1 and 200.");

            var result = await _transactionService.SearchTransactionsByCustomerAsync(customerName, take);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("most-sold")]
        public async Task<IActionResult> GetMostSoldMedicine()
        {
            var result = await _transactionService.GetMostSoldMedicineAsync();

            if (result == null)
                return NotFound("No sales data available.");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("weekly-earnings")]
        public async Task<IActionResult> GetWeeklyEarnings()
        {
            var result = await _transactionService.GetWeeklyEarningsAsync();
            return Ok(result);
        }
    }
}