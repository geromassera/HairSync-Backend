using Application.Interfaces;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _service.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [Authorize]
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role).Value;

            IEnumerable<AppointmentDto> appointments;

            if (role == "Client")
            {
                appointments = await _service.GetAppointmentForCustomerAsync(userId);
            }
            else if (role == "Barber")
            {
                appointments = await _service.GetAppointmentsForBarberAsync(userId);
            }
            else
            {
                return Forbid("Solo clientes o barberos pueden ver sus turnos.");
            }

            return Ok(appointments);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _service.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = $"Appointment with id {id} not found" });

            return Ok(appointment);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var appointments = await _service.GetAppointmentByUserIdAsync(userId);
            return Ok(appointments);
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            dto.CustomerId = userId;

            var created = await _service.CreateAppointmentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, created);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentDto dto)
        {
            if (dto == null || id != dto.AppointmentId)
                return BadRequest("Appointment data mismatch");

            try
            {
                await _service.UpdateAppointmentAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAppointmentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}


