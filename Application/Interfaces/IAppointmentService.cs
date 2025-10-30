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
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentDto>> GetAppointmentByUserIdAsync(int userId);
        Task<AppointmentDto> CreateAppointmentAsync(AppointmentDto dto);
        Task UpdateAppointmentAsync(AppointmentDto dto);
        Task DeleteAppointmentAsync(int id);
    }
}

