using Application.External;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet("appointments/{appointmentId:int}/review")]
        public async Task<ActionResult<ReviewDto>> GetByAppointment(int appointmentId)
        {
            var (userId, isAdmin) = GetAuthContext(User);
            var review = await _service.GetByAppointmentAsync(appointmentId, userId, isAdmin);
            return review is null ? NotFound() : Ok(review);
        }

        [HttpPost("appointments/{appointmentId:int}/review")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<ActionResult<ReviewDto>> Create(int appointmentId, [FromBody] CreateReviewDto dto)
        {
            var (userId, _) = GetAuthContext(User);
            var created = await _service.CreateAsync(appointmentId, userId, dto);
            return CreatedAtAction(nameof(GetByAppointment), new { appointmentId }, created);
        }

        [HttpPut("reviews/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto)
        {
            var (userId, isAdmin) = GetAuthContext(User);
            await _service.UpdateAsync(id, userId, dto, isAdmin);
            return NoContent();
        }

        [HttpDelete("reviews/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var (userId, isAdmin) = GetAuthContext(User);
            await _service.DeleteAsync(id, userId, isAdmin);
            return NoContent();
        }

        private static (int userId, bool isAdmin) GetAuthContext(ClaimsPrincipal user)
        {
            var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
            var id = int.TryParse(idStr, out var parsed) ? parsed : 0;
            var isAdmin = user.IsInRole("Admin");
            return (id, isAdmin);
        }
    }
}
