using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Interfaces
{
    public interface ICurriculumService
    {
        Task<CurriculumSuccessDto> ProcessCurriculumApplicationAsync(CurriculumDto applicationDto);
        Task<(Stream FileStream, string FileName, string ContentType)> GetCvFileAsync(int curriculumId);
        Task<IEnumerable<CurriculumListDto>> GetAllCurriculumsAsync();
    }
}
