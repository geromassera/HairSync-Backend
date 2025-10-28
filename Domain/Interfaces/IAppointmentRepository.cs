using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAppointmentRepository : IRepositoryBase<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Appointment>> GetByBarberIdAsync(int barberId);
    }
}


