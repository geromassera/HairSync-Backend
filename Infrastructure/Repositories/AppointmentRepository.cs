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
    public class AppointmentRepository : RepositoryBase<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Appointments
                .Where(a => a.CustomerId == customerId)
                .Include(a => a.Customer)
                .Include(a => a.Barber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByBarberIdAsync(int barberId)
        {
            return await _context.Appointments
                .Where(a => a.BarberId == barberId)
                .Include(a => a.Customer)
                .Include(a => a.Barber)
                .ToListAsync();
        }
    }
}

