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

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
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
            var appointments = await _appointmentRepository.GetByCustomerIdAsync(userId);

            return appointments.Select(a => new AppointmentDto
            {
                AppointmentId = a.AppointmentId,
                CustomerId = a.CustomerId,
                BarberId = a.BarberId,
                BranchId = a.BranchId,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(AppointmentDto dto)
        {
            var entity = MapToEntity(dto);
            var added = await _appointmentRepository.AddAsync(entity);
            await _appointmentRepository.SaveChangesAsync();
            return MapToDto(added);
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

        private static Appointment MapToEntity(AppointmentDto dto) => new Appointment
        {
            AppointmentId = dto.AppointmentId,
            CustomerId = dto.CustomerId,
            BarberId = dto.BarberId,
            BranchId = dto.BranchId,
            AppointmentDate = dto.AppointmentDate,
            AppointmentTime = dto.AppointmentTime,
            CreatedAt = dto.CreatedAt
        };
    }
}

