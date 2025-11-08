using Application.Interfaces;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Endpoint para que un Cliente cree un turno.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Client")] 
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto createDto)
        {
            try
            {
                var clientId = GetCurrentUserId();
                var newAppointment = await _appointmentService.CreateAppointmentAsync(createDto, clientId);

                return CreatedAtAction(nameof(GetAppointmentById), new { id = newAppointment.Id }, newAppointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para cancelar un turno (Cliente o Barbero).
        /// </summary>
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Client,Barber")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _appointmentService.CancelAppointmentAsync(id, userId);
                return Ok(new { message = "Turno cancelado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para que el Cliente vea sus turnos pendientes.
        /// </summary>
        [HttpGet("my-appointments")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var clientId = GetCurrentUserId();
            var appointments = await _appointmentService.GetMyAppointmentsAsync(clientId);
            return Ok(appointments);
        }

        /// <summary>
        /// Endpoint para que el Barbero vea su agenda del día.
        /// </summary>
        [HttpGet("barber/schedule")]
        [Authorize(Roles = "Barber")]
        public async Task<IActionResult> GetBarberSchedule([FromQuery] DateTime? date)
        {
            var barberId = GetCurrentUserId();
            var scheduleDate = date ?? DateTime.Today;

            var appointments = await _appointmentService.GetBarberScheduleAsync(barberId, scheduleDate);
            return Ok(appointments);
        }

        /// <summary>
        /// Endpoint para que el Admin vea TODO el historial.
        /// </summary>
        [HttpGet("history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAppointmentsHistory()
        {
            var appointments = await _appointmentService.GetAllAppointmentsHistoryAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        [Authorize] // General
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            return Ok();
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Usuario no autenticado.");
        }
    }
}


