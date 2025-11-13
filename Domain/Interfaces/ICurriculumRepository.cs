using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICurriculumRepository : IRepositoryBase<Curriculum>
    {
        Task<IEnumerable<Curriculum>> GetAllCurriculumsAsync();
    }
}
