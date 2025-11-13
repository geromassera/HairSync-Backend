using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
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
        private readonly ApplicationDbContext _dbcontext;
        public CurriculumRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<Curriculum>> GetAllCurriculumsAsync()
        {
            return await _context.Set<Curriculum>()
                                 .OrderByDescending(c => c.UploadDate)
                                 .ToListAsync();
        }
    }
}
