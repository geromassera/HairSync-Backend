using Application.Models;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICurriculumService
    {
        Task<IEnumerable<CurriculumDto>> GetAllAsync();
        Task<CurriculumDto?> GetByIdAsync(int id);
        Task<IEnumerable<CurriculumDto>> GetCurriculumsByUserIdAsync(int userId);
        Task<CurriculumDto> AddCurriculumAsync(CurriculumDto dto);
        Task DeleteCurriculumAsync(int id);
    }
}


