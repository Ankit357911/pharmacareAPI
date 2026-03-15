using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacareAPI.Services;

namespace pharmacareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EarningsController : ControllerBase
    {
        private readonly IEarningService _earningService;
        private readonly ILogger<EarningsController> _logger;

        public EarningsController(IEarningService earningService, ILogger<EarningsController> logger)
        {
            _earningService = earningService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("summary/current")]
        public async Task<IActionResult> GetCurrentSummary()
        {
            try
            {
                var result = await _earningService.GetCurrentSummaryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading current earnings summary.");
                return StatusCode(500, "An error occurred while loading earnings summary.");
            }
        }
    }
}
