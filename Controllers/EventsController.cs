using Microsoft.AspNetCore.Mvc;
using pharmacareAPI.DTOs;
using pharmacareAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace pharmacareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        // api/events/ongoing   
        [Authorize]
        [HttpGet("ongoing")]
        public async Task<IActionResult> GetOngoingEvent()
        {
            var ongoingEvent = await _eventService.GetOngoingEventAsync();

            if (ongoingEvent == null)
            {
                return Ok(new { message = "No event for now" });
            }

            return Ok(ongoingEvent);
        }

        [Authorize]
        [HttpGet("options")]
        public async Task<IActionResult> GetMedicineOrCategoryOptions()
        {
            var options = await _eventService.GetMedicineOrCategoryOptionsAsync();
            return Ok(options);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = await _eventService.CreateEventAsync(dto);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating event '{EventName}'.", dto.EventName);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating event '{EventName}'.", dto.EventName);
                return StatusCode(500, "An error occurred while creating event.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("ongoing")]
        public async Task<IActionResult> DeleteOngoingEvent()
        {
            var deleted = await _eventService.DeleteOngoingEventAsync();

            if (!deleted)
                return NotFound("No ongoing event found.");

            return Ok(new { message = "Ongoing event deleted successfully." });
        }
    }
}