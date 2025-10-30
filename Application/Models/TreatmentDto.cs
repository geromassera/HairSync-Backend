using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class TreatmentDto
    {
        public int TreatmentId { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }

        public static TreatmentDto Create(Treatment treatment)
        {
            if (treatment == null)
            {
                return null;
            }

            return new TreatmentDto
            {
                TreatmentId = treatment.TreatmentId,
                Name = treatment.Name,
                Price = treatment.Price
            };

        }
    }

    public class UpdateTreatmentPriceDto
    {
        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El precio debe ser positivo.")]
        public decimal NewPrice { get; set; }
    }


}
