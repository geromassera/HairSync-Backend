using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        // ... (dependencias y constantes de horario sin cambios) ...
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IUserRepository _userRepo;

        // --- DEFINICIÓN DE DURACIÓN ESTÁNDAR ---
        private const int StandardAppointmentDurationMinutes = 60;

        // --- NUEVO: HORARIOS DE LA BARBERÍA ---
        private static readonly TimeSpan OpeningTime = new TimeSpan(10, 0, 0); // 10:00 AM
        private static readonly TimeSpan ClosingTime = new TimeSpan(19, 0, 0); // 7:00 PM (19:00)
        // El último turno es a las 18:00 (19:00 - 60 min)
        private static readonly TimeSpan LastAppointmentSlot = ClosingTime.Subtract(TimeSpan.FromMinutes(StandardAppointmentDurationMinutes));


        public AppointmentService(
            // ... (código del constructor sin cambios) ...
            IAppointmentRepository appointmentRepo,
            IUserRepository userRepo)
        {
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
        }

        public async Task<AppointmentViewDto> CreateAppointmentAsync(AppointmentCreateDto createDto, int clientId)
        {
            // ... (Validaciones de horario, fecha futura, barbero-sucursal y solapamiento sin cambios) ...
            DateTime appointmentStartTime = createDto.AppointmentDateTime;

            // --- NUEVA VALIDACIÓN: HORARIO COMERCIAL ---

            // 1. Validar Día (Lunes a Sábado)
            if (appointmentStartTime.DayOfWeek == DayOfWeek.Sunday)
            // ... (código de validación sin cambios) ...
            {
                throw new Exception("La barbería está cerrada los domingos.");
            }

            // 2. Validar Hora de Apertura
            if (appointmentStartTime.TimeOfDay < OpeningTime)
            // ... (código de validación sin cambios) ...
            {
                throw new Exception($"El horario de apertura es a las {OpeningTime:hh\\:mm} AM.");
            }

            // 3. Validar Hora de Cierre (considerando la duración)
            // Usamos TimeOfDay para comparar solo la hora.
            if (appointmentStartTime.TimeOfDay > LastAppointmentSlot)
            // ... (código de validación sin cambios) ...
            {
                // Si el turno empieza DESPUÉS del último slot posible
                // Ej: Si el último slot es 18:00, y pide 18:01, falla.
                throw new Exception($"El turno debe finalizar antes de las {ClosingTime:hh\\:mm}. El último turno disponible es a las {LastAppointmentSlot:hh\\:mm}.");
            }

            // --- FIN NUEVA VALIDACIÓN ---


            // --- Validación de Fecha Futura ---
            if (appointmentStartTime <= DateTime.Now) // Asumimos que la fecha del DTO es local
                                                      // ... (código de validación sin cambios) ...
            {
                throw new Exception("La fecha del turno debe ser en el futuro.");
            }

            // --- Validación 1: Barbero vs Sucursal ---
            var barber = await _userRepo.GetByIdAsync(createDto.BarberId);
            if (barber == null)
            // ... (código de validación sin cambios) ...
            {
                throw new Exception("Barbero no encontrado.");
            }
            if (barber.BranchId != createDto.BranchId)
            // ... (código de validación sin cambios) ...
            {
                throw new Exception("El barbero no trabaja en la sucursal seleccionada.");
            }

            // --- Validación 2: Solapamiento de Turno ---
            var isAvailable = await _appointmentRepo.CheckBarberAvailabilityAsync(
                // ... (código de validación sin cambios) ...
                createDto.BarberId,
                appointmentStartTime,
                StandardAppointmentDurationMinutes);

            if (!isAvailable)
            // ... (código de validación sin cambios) ...
            {
                throw new Exception($"El barbero no está disponible. Ya tiene un turno que se solapa con el rango de {StandardAppointmentDurationMinutes} minutos.");
            }

            // --- Creación de la Entidad ---
            var appointment = new Appointment
            {
                AppointmentDateTime = appointmentStartTime,
                BarberId = createDto.BarberId,
                BranchId = createDto.BranchId,
                TreatmentId = createDto.TreatmentId,
                ClientId = clientId,
                Status = AppointmentStatus.Confirmed // <-- CAMBIO: Ahora se Confirma al crear
            };

            await _appointmentRepo.AddAsync(appointment);
            await _appointmentRepo.SaveChangesAsync();

            var newAppointment = await _appointmentRepo.GetByIdWithDetailsAsync(appointment.Id);
            return MapToViewDto(newAppointment);
        }


        public async Task CancelAppointmentAsync(int appointmentId, int userId)
        {
            var appointment = await _appointmentRepo.GetByIdWithDetailsAsync(appointmentId);

            if (appointment == null) { throw new Exception("Turno no encontrado."); }

            // Verificación de permiso
            if (appointment.ClientId != userId && appointment.BarberId != userId)
            { throw new Exception("No autorizado para cancelar este turno."); }

            // --- NUEVA LÓGICA DE CANCELACIÓN ---

            // 1. Validar si ya está cancelado
            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new Exception("El turno ya está cancelado.");
            }

            // 2. Validar si ya está "Completado" (si su fecha ya pasó)
            DateTime appointmentEndTime = appointment.AppointmentDateTime.AddMinutes(StandardAppointmentDurationMinutes);
            // Usamos DateTime.UtcNow si las fechas se guardan en UTC
            if (appointmentEndTime < DateTime.UtcNow)
            {
                throw new Exception("No se puede cancelar un turno que ya ha pasado (está completado).");
            }

            // Si llegó acá, es 'Confirmed' y en el futuro, así que se puede cancelar.
            appointment.Status = AppointmentStatus.Cancelled;
            _appointmentRepo.Update(appointment);
            await _appointmentRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetMyAppointmentsAsync(int clientId)
        {
            // Usamos el método renombrado del repositorio
            var appointments = await _appointmentRepo.GetFutureAppointmentsByClientIdAsync(clientId);
            return appointments.Select(MapToViewDto);
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetBarberScheduleAsync(int barberId, DateTime date)
        // ... (código sin cambios) ...
        {
            var appointments = await _appointmentRepo.GetByBarberIdAndDateAsync(barberId, date);
            return appointments.Select(MapToViewDto);
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetAllAppointmentsHistoryAsync()
        // ... (código sin cambios) ...
        {
            var appointments = await _appointmentRepo.GetAllWithDetailsAsync();
            return appointments.Select(MapToViewDto);
        }

        // --- MÉTODO CLAVE: Mapeo con Estado Calculado ---
        private AppointmentViewDto MapToViewDto(Appointment appointment)
        {
            if (appointment == null) return null;

            // --- LÓGICA DE ESTADO CALCULADO ---
            string status;
            // Calculamos la hora de fin (asumiendo UTC si la DB está en UTC)
            DateTime appointmentEndTime = appointment.AppointmentDateTime.AddMinutes(StandardAppointmentDurationMinutes);

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                status = AppointmentStatus.Cancelled.ToString(); // "Cancelled"
            }
            // Si la hora de fin es anterior a AHORA, está completado
            else if (appointmentEndTime < DateTime.UtcNow)
            {
                status = "Completed"; // Estado calculado
            }
            else // Si no está cancelado y no pasó, está confirmado
            {
                status = AppointmentStatus.Confirmed.ToString(); // "Confirmed"
            }
            // --- FIN LÓGICA ---

            return new AppointmentViewDto
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = status, // <-- Usamos la variable 'status' calculada

                ClientName = $"{appointment.Client?.Name} {appointment.Client?.Surname}".Trim(),
                BarberName = $"{appointment.Barber?.Name} {appointment.Barber?.Surname}".Trim(),
                BranchName = appointment.Branch?.Name ?? "N/D",
                TreatmentName = appointment.Treatment?.Name ?? "N/D",
                TreatmentPrice = appointment.Treatment?.Price ?? 0
            };
        }
    }
}

