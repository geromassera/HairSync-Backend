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
        private readonly ITreatmentRepository _treatmentRepo;


        private const int StandardAppointmentDurationMinutes = 60;

        private static readonly TimeSpan OpeningTime = new TimeSpan(12, 0, 0); // 10:00 AM
        private static readonly TimeSpan ClosingTime = new TimeSpan(22, 0, 0); // 7:00 PM (19:00)
        // El último turno es a las 18:00 (19:00 - 60 min)
        private static readonly TimeSpan LastAppointmentSlot = ClosingTime.Subtract(TimeSpan.FromMinutes(StandardAppointmentDurationMinutes));


        public AppointmentService(
            IAppointmentRepository appointmentRepo,
            IUserRepository userRepo,
            ITreatmentRepository treatmentRepo)
        {
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
            _treatmentRepo = treatmentRepo;
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

            var treatment = await _treatmentRepo.GetByIdAsync(createDto.TreatmentId);
            if (treatment == null)
            {
                throw new Exception("Tratamiento no encontrado.");
            }

            var appointment = new Appointment
            {
                AppointmentDateTime = appointmentStartTime,
                BarberId = createDto.BarberId,
                BranchId = createDto.BranchId,
                TreatmentId = createDto.TreatmentId,
                ClientId = clientId,
                Status = AppointmentStatus.Confirmed,
                Price = treatment.Price
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
                ClientPhone = appointment.Client?.Phone ?? "N/D",
                BarberName = $"{appointment.Barber?.Name} {appointment.Barber?.Surname}".Trim(),
                BranchName = appointment.Branch?.Name ?? "N/D",
                TreatmentName = appointment.Treatment?.Name ?? "N/D",
                Price = appointment.Price
            };
        }

        public async Task<List<string>> GetAvailableHoursAsync(int branchId, DateOnly date, int? barberId)
        public async Task<AppointmentViewDto> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _appointmentRepo.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Turno no encontrado.");
            }
            return MapToViewDto(appointment);
        }

        public async Task<List<string>> GetAvailableHoursAsync(int branchId, DateOnly date, int? barberId = null)
        {
            if (date.ToDateTime(TimeOnly.MinValue).DayOfWeek == DayOfWeek.Sunday)
                return new List<string>();

            var barbers = await _userRepo.GetBarbersByBranchAsync(branchId);

            if (barberId.HasValue)
                barbers = barbers.Where(b => b.UserId == barberId.Value).ToList();

            if (barbers.Count == 0)
                return new List<string>();

            var opening = new TimeSpan(10, 0, 0);
            var closing = new TimeSpan(19, 0, 0);

            var result = new List<string>();

            for (var t = opening; t < closing; t += TimeSpan.FromHours(1))
            {
                var local = date.ToDateTime(TimeOnly.FromTimeSpan(t));
                var utc = DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();

                bool free = false;

                if (barberId.HasValue)
                {
                    free = await _appointmentRepo.CheckBarberAvailabilityAsync(barberId.Value, utc, 60);
                }
                else
                {
                    foreach (var b in barbers)
                    {
                        if (await _appointmentRepo.CheckBarberAvailabilityAsync(b.UserId, utc, 60))
                        {
                            free = true;
                            break;
                        }
                    }
                }

                if (free)
                    result.Add(utc.ToString("o"));
            }

            return result;
        }


        public async Task<List<BarberDto>> GetBarbersByBranchAsync(int branchId)
        {
            var barbers = await _userRepo.GetBarbersByBranchAsync(branchId);

            return barbers.Select(b => new BarberDto
            {
                Id = b.UserId,
                Name = $"{b.Name} {b.Surname}".Trim()
            }).ToList();
        }

    }
}

