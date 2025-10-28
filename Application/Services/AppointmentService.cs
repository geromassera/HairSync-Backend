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
    public class AppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.ListAsync();
            return appointments.Select(MapToDto);
        }

        public async Task<AppointmentDto?> GetByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return appointment == null ? null : MapToDto(appointment);
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentDto dto)
        {
            var entity = MapToEntity(dto);
            var added = await _appointmentRepository.AddAsync(entity);
            await _appointmentRepository.SaveChangesAsync();
            return MapToDto(added);
        }

        public async Task UpdateAsync(int id, AppointmentDto dto)
        {
            var existing = await _appointmentRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Appointment {id} not found.");

            existing.AppointmentDate = dto.AppointmentDate;
            existing.AppointmentTime = dto.AppointmentTime;
            existing.CustomerId = dto.CustomerId;
            existing.BarberId = dto.BarberId;
            existing.BranchId = dto.BranchId;

            await _appointmentRepository.UpdateAsync(existing);
            await _appointmentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
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

