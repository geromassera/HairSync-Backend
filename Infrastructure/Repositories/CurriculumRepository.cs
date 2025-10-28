using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CurriculumRepository : RepositoryBase<Curriculum>, ICurriculumRepository
    {
        private readonly ApplicationDbContext _context;

        public CurriculumRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Curriculum>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<Curriculum>()
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .ToListAsync();
        }

    }
}

