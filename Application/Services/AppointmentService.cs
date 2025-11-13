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
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IUserRepository _userRepo;

        private const int StandardAppointmentDurationMinutes = 60;

        private static readonly TimeSpan OpeningTime = new TimeSpan(10, 0, 0); // 10:00 AM
        private static readonly TimeSpan ClosingTime = new TimeSpan(19, 0, 0); // 7:00 PM (19:00)
        // El último turno es a las 18:00 (19:00 - 60 min)
        private static readonly TimeSpan LastAppointmentSlot = ClosingTime.Subtract(TimeSpan.FromMinutes(StandardAppointmentDurationMinutes));


        public AppointmentService(
            IAppointmentRepository appointmentRepo,
            IUserRepository userRepo)
        {
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
        }

        public async Task<AppointmentViewDto> CreateAppointmentAsync(AppointmentCreateDto createDto, int clientId)
        {
            DateTime appointmentStartTime = createDto.AppointmentDateTime;


            if (appointmentStartTime.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new Exception("La barbería está cerrada los domingos.");
            }

            if (appointmentStartTime.TimeOfDay < OpeningTime)
            {
                throw new Exception($"El horario de apertura es a las {OpeningTime:hh\\:mm} AM.");
            }

            if (appointmentStartTime.TimeOfDay > LastAppointmentSlot)
            {
                throw new Exception($"El turno debe finalizar antes de las {ClosingTime:hh\\:mm}. El último turno disponible es a las {LastAppointmentSlot:hh\\:mm}.");
            }



            if (appointmentStartTime <= DateTime.Now)
            {
                throw new Exception("La fecha del turno debe ser en el futuro.");
            }

            var barber = await _userRepo.GetByIdAsync(createDto.BarberId);
            if (barber == null)
            {
                throw new Exception("Barbero no encontrado.");
            }
            if (barber.BranchId != createDto.BranchId)
            {
                throw new Exception("El barbero no trabaja en la sucursal seleccionada.");
            }

            var isAvailable = await _appointmentRepo.CheckBarberAvailabilityAsync(
                createDto.BarberId,
                appointmentStartTime,
                StandardAppointmentDurationMinutes);

            if (!isAvailable)
            {
                throw new Exception($"El barbero no está disponible. Ya tiene un turno que se solapa con el rango de {StandardAppointmentDurationMinutes} minutos.");
            }

            var appointment = new Appointment
            {
                AppointmentDateTime = appointmentStartTime,
                BarberId = createDto.BarberId,
                BranchId = createDto.BranchId,
                TreatmentId = createDto.TreatmentId,
                ClientId = clientId,
                Status = AppointmentStatus.Confirmed
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

            if (appointment.ClientId != userId && appointment.BarberId != userId)
            { throw new Exception("No autorizado para cancelar este turno."); }


            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new Exception("El turno ya está cancelado.");
            }

            DateTime appointmentEndTime = appointment.AppointmentDateTime.AddMinutes(StandardAppointmentDurationMinutes);
            if (appointmentEndTime < DateTime.UtcNow)
            {
                throw new Exception("No se puede cancelar un turno que ya ha pasado (está completado).");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            _appointmentRepo.Update(appointment);
            await _appointmentRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetMyAppointmentsAsync(int clientId)
        {
            var appointments = await _appointmentRepo.GetFutureAppointmentsByClientIdAsync(clientId);
            return appointments.Select(MapToViewDto);
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetBarberScheduleAsync(int barberId, DateTime date)
        {
            var appointments = await _appointmentRepo.GetByBarberIdAndDateAsync(barberId, date);
            return appointments.Select(MapToViewDto);
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetAllAppointmentsHistoryAsync()
        {
            var appointments = await _appointmentRepo.GetAllWithDetailsAsync();
            return appointments.Select(MapToViewDto);
        }
        private AppointmentViewDto MapToViewDto(Appointment appointment)
        {
            if (appointment == null) return null;

            string status;
            DateTime appointmentEndTime = appointment.AppointmentDateTime.AddMinutes(StandardAppointmentDurationMinutes);

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                status = AppointmentStatus.Cancelled.ToString();
            }
            else if (appointmentEndTime < DateTime.UtcNow)
            {
                status = "Completed";
            }
            else
            {
                status = AppointmentStatus.Confirmed.ToString();
            }

            return new AppointmentViewDto
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = status,

                ClientName = $"{appointment.Client?.Name} {appointment.Client?.Surname}".Trim(),
                BarberName = $"{appointment.Barber?.Name} {appointment.Barber?.Surname}".Trim(),
                BranchName = appointment.Branch?.Name ?? "N/D",
                TreatmentName = appointment.Treatment?.Name ?? "N/D",
                TreatmentPrice = appointment.Treatment?.Price ?? 0
            };
        }

        public async Task<List<string>> GetAvailableHoursAsync(int branchId, DateOnly date, int? barberId = null)
        {
            // 1) Sin domingos
            if (date.ToDateTime(TimeOnly.MinValue).DayOfWeek == DayOfWeek.Sunday)
                return new List<string>();

            // 2) Config: 1 turno por hora, de 10 a 19 (último inicio 18:00)
            const int SlotMinutes = 60;
            var opening = new TimeSpan(10, 0, 0);
            var closing = new TimeSpan(19, 0, 0);
            var lastStart = closing - TimeSpan.FromMinutes(SlotMinutes);

            // 3) Barberos de la sucursal (y filtrar si mandan uno)
            var barbers = await _userRepo.GetBarbersByBranchAsync(branchId);
            if (barberId.HasValue)
                barbers = barbers.Where(b => b.UserId == barberId.Value).ToList(); // 👈 usa tu PK real (UserId)

            if (barbers.Count == 0)
                return new List<string>();

            // 4) Generar horas: 10:00, 11:00, ..., 18:00 (hora local)
            var startLocal = date.ToDateTime(TimeOnly.FromTimeSpan(opening));
            var lastStartLocal = date.ToDateTime(TimeOnly.FromTimeSpan(lastStart));
            var availableHours = new List<string>(); // devolveremos ISO-8601 en UTC

            for (var t = startLocal; t <= lastStartLocal; t = t.AddHours(1))
            {
                var startUtc = DateTime.SpecifyKind(t, DateTimeKind.Local).ToUniversalTime();

                bool slotFree;
                if (barberId.HasValue)
                {
                    // chequear solo ese barbero
                    slotFree = await _appointmentRepo.CheckBarberAvailabilityAsync(barberId.Value, startUtc, SlotMinutes);
                }
                else
                {
                    // chequear si ALGÚN barbero de la sucursal está libre en ese horario
                    slotFree = false;
                    foreach (var b in barbers)
                    {
                        if (await _appointmentRepo.CheckBarberAvailabilityAsync(b.UserId, startUtc, SlotMinutes)) // 👈 usa tu PK real (UserId)
                        {
                            slotFree = true;
                            break;
                        }
                    }
                }

                if (slotFree)
                    availableHours.Add(startUtc.ToString("o")); // ISO-8601 en UTC (ej: 2025-11-14T13:00:00.0000000Z)
            }

            return availableHours;
        }
    }
}

