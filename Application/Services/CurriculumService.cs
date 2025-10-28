using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly ICurriculumRepository _repository;

        public CurriculumService(ICurriculumRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CurriculumDto>> GetAllAsync()
        {
            var curriculums = await _repository.GetAllAsync();

            return curriculums.Select(c => new CurriculumDto
            {
                CurriculumId = c.CurriculumId,
                UserId = c.UserId,
                FileName = c.FileName,
                FilePath = c.FilePath,
                UploadDate = c.UploadDate
            });
        }

        public async Task<CurriculumDto?> GetByIdAsync(int id)
        {
            var curriculum = await _repository.GetByIdAsync(id);
            if (curriculum == null)
                return null;

            return new CurriculumDto
            {
                CurriculumId = curriculum.CurriculumId,
                UserId = curriculum.UserId,
                FileName = curriculum.FileName,
                FilePath = curriculum.FilePath,
                UploadDate = curriculum.UploadDate
            };
        }

        public async Task<IEnumerable<CurriculumDto>> GetCurriculumsByUserIdAsync(int userId)
        {
            var curriculums = await _repository.GetByUserIdAsync(userId);

            return curriculums.Select(c => new CurriculumDto
            {
                CurriculumId = c.CurriculumId,
                UserId = c.UserId,
                FileName = c.FileName,
                FilePath = c.FilePath,
                UploadDate = c.UploadDate
            });
        }

        public async Task<CurriculumDto> AddCurriculumAsync(CurriculumDto dto)
        {
            var entity = new Curriculum
            {
                UserId = dto.UserId,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                UploadDate = dto.UploadDate ?? DateTime.UtcNow
            };

            var created = await _repository.AddAsync(entity);

            return new CurriculumDto
            {
                CurriculumId = created.CurriculumId,
                UserId = created.UserId,
                FileName = created.FileName,
                FilePath = created.FilePath,
                UploadDate = created.UploadDate
            };
        }

        public async Task DeleteCurriculumAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Curriculum not found");

            await _repository.DeleteAsync(entity);
        }
    }
}

