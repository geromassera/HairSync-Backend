using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        // ... (constructor y GetAppointmentsWithDetails sin cambios) ...
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método privado para reutilizar los Includes
        private IQueryable<Appointment> GetAppointmentsWithDetails()
        {
            return _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Barber)
                .Include(a => a.Branch)
                .Include(a => a.Treatment)
                .AsQueryable();
        }

        public async Task<Appointment> GetByIdWithDetailsAsync(int id)
        // ... (código sin cambios) ...
        {
            return await GetAppointmentsWithDetails()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
        // ... (código sin cambios) ...
        {
            return await GetAppointmentsWithDetails()
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        // Renombramos el método e T quitamos 'Pending'
        public async Task<IEnumerable<Appointment>> GetFutureAppointmentsByClientIdAsync(int clientId)
        {
            // Solo buscamos turnos Confirmados (ya no hay Pending)
            var pendingStatus = new[] { AppointmentStatus.Confirmed };

            return await GetAppointmentsWithDetails()
                .Where(a => a.ClientId == clientId &&
                             pendingStatus.Contains(a.Status) &&
                             a.AppointmentDateTime >= DateTime.UtcNow) // Solo futuros
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByBarberIdAndDateAsync(int barberId, DateTime date)
        {
            // Solo buscamos turnos Confirmados (ya no hay Pending)
            var relevantStatus = new[] { AppointmentStatus.Confirmed };
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            // ... (código de la consulta sin cambios) ...
            return await GetAppointmentsWithDetails()
                .Where(a => a.BarberId == barberId &&
                             relevantStatus.Contains(a.Status) &&
                             a.AppointmentDateTime >= startOfDay &&
                             a.AppointmentDateTime < endOfDay)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        /// <summary>
        /// Implementación de la lógica de solapamiento.
        /// </summary>
        public async Task<bool> CheckBarberAvailabilityAsync(int barberId, DateTime requestedStartTime, int durationInMinutes)
        {
            // Solo chequeamos contra turnos Confirmados
            var relevantStatus = new[] { AppointmentStatus.Confirmed };

            // Hora de fin del turno solicitado
            DateTime requestedEndTime = requestedStartTime.AddMinutes(durationInMinutes);

            // ... (código de la consulta AnyAsync sin cambios) ...
            // 
            // Busca si existe ALGUN turno en la DB que se solape
            var isOverlapping = await _context.Appointments
                // No necesitamos Include(Treatment) porque asumimos duración estándar
                .Where(a => a.BarberId == barberId && relevantStatus.Contains(a.Status))
                .AnyAsync(a =>
                    // El turno existente (a) empieza ANTES de que el nuevo TERMINE
                    (a.AppointmentDateTime < requestedEndTime) &&

                    // Y el turno existente (a) termina (asumiendo 60min) DESPUÉS de que el nuevo EMPIECE
                    (a.AppointmentDateTime.AddMinutes(durationInMinutes) > requestedStartTime)
                );

            // Devuelve 'true' (está disponible) si NO hay solapamiento ('isOverlapping' es false)
            return !isOverlapping;
        }

        public async Task AddAsync(Appointment appointment)
        // ... (código sin cambios) ...
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public void Update(Appointment appointment)
        // ... (código sin cambios) ...
        {
            _context.Appointments.Update(appointment);
        }

        public async Task<bool> SaveChangesAsync()
        // ... (código sin cambios) ...
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
