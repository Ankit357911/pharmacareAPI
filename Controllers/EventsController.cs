using Microsoft.AspNetCore.Mvc;
using pharmacareAPI.Models;
using pharmacareAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace pharmacareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
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
    }
}