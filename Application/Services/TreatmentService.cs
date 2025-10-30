using Application.Interfaces;
using Application.Models;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;

namespace Application.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly ITreatmentRepository _treatmentRepository;

        public TreatmentService(ITreatmentRepository treatmentRepository)
        {
            _treatmentRepository = treatmentRepository;
        }

        public async Task<IEnumerable<TreatmentDto>> GetAllTreatmentsAsync()
        {
            var treatments = await _treatmentRepository.GetAllAsync();

            if (treatments == null || !treatments.Any())
            {
                return Enumerable.Empty<TreatmentDto>();
            }

            return treatments.Select(TreatmentDto.Create);  
        }

        public async Task UpdateTreatmentPriceAsync(int treatmentId, UpdateTreatmentPriceDto priceDto)
        {
            var treatment = await _treatmentRepository.GetByIdAsync(treatmentId);

            if (treatment == null)
            {
                throw new NotFoundException($"No se encontro el servicio con el ID: {treatmentId}");
            }

            treatment.Price = priceDto.NewPrice;

            await _treatmentRepository.SaveChangesAsync();
        }
    }
}
