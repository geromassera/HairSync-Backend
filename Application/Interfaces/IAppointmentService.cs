using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Interfaces
{
    /// <summary>
    /// Interfaz del servicio para manejar la lógica de negocio de Turnos.
    /// </summary>
    public interface IAppointmentService
    {
        /// <summary>
        /// Cliente crea un nuevo turno.
        /// </summary>
        /// <param name="createDto">Datos del turno</param>
        /// <param name="clientId">ID del cliente (obtenido de autenticación)</param>
        /// <returns>El turno creado como DTO</returns>
        Task<AppointmentViewDto> CreateAppointmentAsync(AppointmentCreateDto createDto, int clientId);

        /// <summary>
        /// Cliente cancela un turno.
        /// </summary>
        /// <param name="appointmentId">ID del turno a cancelar</param>
        /// <param name="userId">ID del usuario que cancela (para verificar permisos)</param>
        Task CancelAppointmentAsync(int appointmentId, int userId);

        // --- Vistas ---

        /// <summary>
        /// Cliente ve sus turnos pendientes.
        /// </summary>
        Task<IEnumerable<AppointmentViewDto>> GetMyAppointmentsAsync(int clientId);

        /// <summary>
        /// Barbero ve sus turnos asignados para un día.
        /// </summary>
        Task<IEnumerable<AppointmentViewDto>> GetBarberScheduleAsync(int barberId, DateTime date);

        /// <summary>
        /// Admin ve el historial de todos los turnos.
        /// </summary>
        Task<IEnumerable<AppointmentViewDto>> GetAllAppointmentsHistoryAsync();
    }
}

