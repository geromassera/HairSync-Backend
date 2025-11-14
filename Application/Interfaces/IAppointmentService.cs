using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentViewDto> CreateAppointmentAsync(AppointmentCreateDto createDto, int clientId);

        Task CancelAppointmentAsync(int appointmentId, int userId);

        Task<IEnumerable<AppointmentViewDto>> GetMyAppointmentsAsync(int clientId);

        Task<IEnumerable<AppointmentViewDto>> GetBarberScheduleAsync(int barberId, DateTime date);

        Task<IEnumerable<AppointmentViewDto>> GetAllAppointmentsHistoryAsync();

        Task<List<string>> GetAvailableHoursAsync(int branchId, DateOnly date, int? barberId = null);

        Task<List<BarberDto>> GetBarbersByBranchAsync(int branchId);
    }
}

