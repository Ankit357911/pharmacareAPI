using Microsoft.AspNetCore.Mvc;
using pharmacareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using pharmacareAPI.DTOs;

namespace pharmacareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;
        private readonly ILogger<MedicinesController> _logger;

        public MedicinesController(IMedicineService medicineService, ILogger<MedicinesController> logger)
        {
            _medicineService = medicineService;
            _logger = logger;
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

        [Authorize]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _medicineService.GetCategoriesAsync();

                if (categories.Count == 0)
                {
                    _logger.LogWarning("No medicine categories were found.");
                    return NotFound("No categories found. Please add categories first.");
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching medicine categories.");
                return StatusCode(500, "An error occurred while loading categories.");
            }
        }

        [Authorize]
        [HttpPut("update-rate-stock")]
        public async Task<IActionResult> UpdateRateAndStock([FromBody] UpdateMedicineInventoryDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var updated = await _medicineService.UpdateMedicineRateAndStockAsync(dto);

                if (!updated)
                    return NotFound($"Medicine '{dto.MedicineName}' not found.");

                return Ok(new { message = "Medicine updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating medicine '{MedicineName}'.", dto.MedicineName);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating medicine '{MedicineName}'.", dto.MedicineName);
                return StatusCode(500, "An error occurred while updating medicine.");
            }
        }

        [Authorize]
        [HttpGet("stock/{name}")]
        public async Task<IActionResult> GetMedicineStock(string name)
        {
            try
            {
                var result = await _medicineService.GetMedicineStockAsync(name);

                if (result == null)
                    return NotFound("Medicine not found.");

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while reading stock for medicine '{MedicineName}'.", name);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while reading stock for medicine '{MedicineName}'.", name);
                return StatusCode(500, "An error occurred while loading medicine stock.");
            }
        }

        [Authorize]
        [HttpPost("remove-stock")]
        public async Task<IActionResult> RemoveStock([FromBody] RemoveMedicineStockDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var result = await _medicineService.RemoveMedicineStockAsync(dto);

                if (result == null)
                    return NotFound($"Medicine '{dto.MedicineName}' not found.");

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while removing stock for medicine '{MedicineName}'.", dto.MedicineName);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while removing stock for medicine '{MedicineName}'.", dto.MedicineName);
                return StatusCode(500, "An error occurred while removing medicine stock.");
            }
        }

        [Authorize]
        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertMedicine([FromBody] UpsertMedicineDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var result = await _medicineService.UpsertMedicineAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while upserting medicine '{MedicineName}'.", dto.MedicineName);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while upserting medicine '{MedicineName}'.", dto.MedicineName);
                return StatusCode(500, "An error occurred while adding/updating medicine.");
            }
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchMedicines([FromQuery] string query, [FromQuery] int take = 10)
        {
            try
            {
                var result = await _medicineService.SearchMedicinesAsync(query, take);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while searching medicines with query '{Query}'.", query);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching medicines with query '{Query}'.", query);
                return StatusCode(500, "An error occurred while searching medicines.");
            }
        }
    }
}