using Microsoft.AspNetCore.Mvc;
using pharmacareAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace pharmacareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // GET: api/medicines/names
        [Authorize]
        [HttpGet("names")]
        public async Task<IActionResult> GetMedicineNames()
        {
            var names = await _medicineService.GetMedicineNamesAsync();
            return Ok(names);
        }

        [Authorize]
        [HttpGet("details/{name}")]
        public async Task<IActionResult> GetMedicineDetails(string name)
        {
            var result = await _medicineService.GetMedicineDetailsAsync(name);

            if (result == null)
                return NotFound("Medicine not found");

            return Ok(result);
        }
    }
}