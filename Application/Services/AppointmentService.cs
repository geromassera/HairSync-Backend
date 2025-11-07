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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.ListAsync();
            return appointments.Select(MapToDto);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return appointment == null ? null : MapToDto(appointment);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentByUserIdAsync(int userId)
        {
            var customerAppointments = await _appointmentRepository.GetByCustomerIdAsync(userId);
            var barberAppointments = await _appointmentRepository.GetByBarberIdAsync(userId);

            var allAppointments = customerAppointments.Concat(barberAppointments);

            return allAppointments.Select(MapToDto);
        }


        public async Task<AppointmentDto> CreateAppointmentAsync(AppointmentDto dto)
        {
            var customer = await _userRepository.GetByIdAsync(dto.CustomerId);
            var barber = await _userRepository.GetByIdAsync(dto.BarberId);

            if (customer == null)
                throw new Exception($"No existe un usuario con ID {dto.CustomerId}");

            if (barber == null)
                throw new Exception($"No existe un usuario con ID {dto.BarberId}");

            // 2️⃣ Verificar que los roles sean correctos
            if (customer.Role != Domain.Enums.UserRole.Client)
                throw new Exception($"El usuario con ID {dto.CustomerId} no tiene rol de Customer");

            if (barber.Role != Domain.Enums.UserRole.Barber)
                throw new Exception($"El usuario con ID {dto.BarberId} no tiene rol de Barber");

            // 3️⃣ Verificar que no haya conflicto de horario (opcional)
            var existing = await _appointmentRepository.GetAppointmentsByBarberAndDateAsync(dto.BarberId, dto.AppointmentDate);
            if (existing.Any(a => a.AppointmentTime == dto.AppointmentTime))
                throw new Exception("El barbero ya tiene un turno asignado en ese horario.");

            // 4️⃣ Crear la cita
            var appointment = new Appointment
            {
                CustomerId = dto.CustomerId,
                BarberId = dto.BarberId,
                BranchId = dto.BranchId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                CreatedAt = DateTime.UtcNow
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            dto.AppointmentId = appointment.AppointmentId;
            dto.CreatedAt = appointment.CreatedAt;

            return dto;
        }


        public async Task UpdateAppointmentAsync(AppointmentDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            appointment.CustomerId = dto.CustomerId;
            appointment.BarberId = dto.BarberId;
            appointment.BranchId = dto.BranchId;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.AppointmentTime = dto.AppointmentTime;

            await _appointmentRepository.UpdateAsync(appointment);
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment {id} not found.");

            await _appointmentRepository.DeleteAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }

        private static AppointmentDto MapToDto(Appointment entity) => new AppointmentDto
        {
            AppointmentId = entity.AppointmentId,
            CustomerId = entity.CustomerId,
            BarberId = entity.BarberId,
            BranchId = entity.BranchId,
            AppointmentDate = entity.AppointmentDate,
            AppointmentTime = entity.AppointmentTime,
            CreatedAt = entity.CreatedAt
        };
    }
}

