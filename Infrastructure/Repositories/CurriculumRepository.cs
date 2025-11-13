using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Repositories
{
    public class CurriculumRepository : RepositoryBase<Curriculum>, ICurriculumRepository
    {
        public CurriculumRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
