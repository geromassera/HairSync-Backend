using Application.Interfaces;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentController : Controller
    {
        private readonly ITreatmentService _treatmentService;

        public TreatmentController(ITreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTreatments()
        {
            var treatments = await _treatmentService.GetAllTreatmentsAsync();
            return Ok(treatments);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTreatmentPrice(int id, [FromBody] UpdateTreatmentPriceDto dto)
        {
            await _treatmentService.UpdateTreatmentPriceAsync(id, dto);

            return NoContent();
        }
    }
}
