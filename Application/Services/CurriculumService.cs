using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models;
using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IRepositoryBase<Curriculum> _curriculumRepository;

        // Carpeta de almacenamiento segura fuera de wwwroot
        private const string CvsFolderName = "Curriculums_Storage";

        public CurriculumService(
            IWebHostEnvironment environment,
            IRepositoryBase<Curriculum> curriculumRepository)
        {
            _environment = environment;
            _curriculumRepository = curriculumRepository;
        }

        public async Task<CurriculumSuccessDto> ProcessCurriculumApplicationAsync(CurriculumDto applicationDto)
        {
            // 1. Validación de Archivo (tipo y tamaño)
            if (applicationDto.CvFile.ContentType != "application/pdf")
            {
                throw new BadRequestException("Solo se permiten archivos en formato PDF.");
            }

            if (applicationDto.CvFile.Length > 5 * 1024 * 1024) // Límite de 5MB
            {
                throw new BadRequestException("El archivo no debe exceder los 5MB.");
            }

            // 2. Definir Ruta Segura (fuera del alcance público)
            // La ruta base es el directorio del proyecto Presentation
            string rootPath = _environment.ContentRootPath;
            string cvsFolderPath = Path.Combine(rootPath, CvsFolderName);

            if (!Directory.Exists(cvsFolderPath))
            {
                Directory.CreateDirectory(cvsFolderPath);
            }

            // 3. Guardar Archivo (con nombre único para evitar colisiones)
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(applicationDto.CvFile.FileName);
            string filePath = Path.Combine(cvsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await applicationDto.CvFile.CopyToAsync(stream);
            }

            // 4. Guardar Metadatos en la Base de Datos
            var curriculum = new Curriculum
            {
                Name = applicationDto.Name,
                Surname = applicationDto.Surname,
                Email = applicationDto.Email,
                Phone = applicationDto.Phone,
                FilePath = filePath,
                FileName = applicationDto.CvFile.FileName,
                UploadDate = DateTime.UtcNow
            };

            await _curriculumRepository.AddAsync(curriculum);
            // Ya que IRepositoryBase hereda de IRepositoryBase<T>, asumimos que AddAsync y SaveChangesAsync están disponibles.
            await _curriculumRepository.SaveChangesAsync();

            return new CurriculumSuccessDto { ApplicationId = curriculum.Id };
        }
    }
}
