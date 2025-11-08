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
    [Authorize] // Todos los endpoints requieren autenticación
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
        [Authorize(Roles = "Client")] // Solo Clientes
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto createDto)
        {
            try
            {
                var clientId = GetCurrentUserId(); // Helper para obtener ID del token JWT
                var newAppointment = await _appointmentService.CreateAppointmentAsync(createDto, clientId);

                // Retornamos el DTO de vista (AppointmentViewDto)
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
        [Authorize(Roles = "Client,Barber")] // Clientes o Barberos
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
                // Manejar excepciones específicas (ej. Unauthorized, NotFound)
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- VISTAS ---

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
            var scheduleDate = date ?? DateTime.Today; // Si no provee fecha, usa hoy

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

        // (Helper para obtener un turno por ID, usado por CreatedAtAction)
        [HttpGet("{id}")]
        [Authorize] // General
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            // Este endpoint debería usar el servicio para obtener un DTO
            // y verificar permisos (ej. que solo el cliente, barbero o admin puedan verlo)
            // Por simplicidad, lo dejamos pendiente.
            return Ok();
        }


        // --- Helper ---
        private int GetCurrentUserId()
        {
            // Simulación de obtención de ID desde el Token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            // Esto no debería pasar si [Authorize] está activo
            throw new UnauthorizedAccessException("Usuario no autenticado.");
        }
    }
}


