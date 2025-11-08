using Application.Exceptions;
using Application.External;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // (GET sigue igual, AllowAnonymous)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        // --- ENDPOINT ACTUALIZADO ---
        [HttpPost]
        // REGLA 2: Ahora solo pueden entrar usuarios AUTORIZADOS
        // Y que además tengan el ROL "client".
        // Esto asume que tu JWT/Cookie tiene un claim de "role" con el valor "client".
        [Authorize(Roles = "client")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                // Llamamos al servicio (que ahora tiene la validación de "una sola vez")
                await _reviewService.CreateReviewAsync(reviewDto, userId);

                return StatusCode(201, "Review creada exitosamente.");
            }
            catch (AlreadyReviewedException ex)
            {
                // REGLA 1: Manejamos la excepción del servicio.
                // HTTP 409 Conflict es ideal para "no se puede crear porque ya existe".
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Un catch-all por si algo más falla
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.Message });
            }
        }
    }
}
