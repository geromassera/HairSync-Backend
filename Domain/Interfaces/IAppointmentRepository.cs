using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();

        // Renombramos GetPendingByClientIdAsync para que sea más claro
        Task<IEnumerable<Appointment>> GetFutureAppointmentsByClientIdAsync(int clientId);

        Task<IEnumerable<Appointment>> GetByBarberIdAndDateAsync(int barberId, DateTime date);

        /// <summary>
        /// Verifica si un barbero tiene disponibilidad en un rango de tiempo.
        /// Chequea que no haya turnos (Confirmed) que se solapen,
        /// asumiendo una duración estándar.
        /// </summary>
        Task<bool> CheckBarberAvailabilityAsync(int barberId, DateTime requestedStartTime, int durationInMinutes);

        Task AddAsync(Appointment appointment);
        void Update(Appointment appointment);
        Task<bool> SaveChangesAsync();
    }
}


