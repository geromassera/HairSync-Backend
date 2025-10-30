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
    public class TreatmentRepository : ITreatmentRepository
    {
        private readonly ApplicationDbContext _context;

        public TreatmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Treatment>> GetAllAsync()
        {
            return await _context.Treatments.AsNoTracking().ToListAsync();
        }

        public async Task<Treatment> GetByIdAsync(int treatmentId)
        {
            return await _context.Treatments.FindAsync(treatmentId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
