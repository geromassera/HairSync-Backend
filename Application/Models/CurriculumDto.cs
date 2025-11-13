using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class CurriculumDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CV es obligatorio.")]
        // IFormFile se usa para manejar la subida del archivo binario
        public IFormFile CvFile { get; set; } = default!;
    }

    public class CurriculumSuccessDto
    {
        public int ApplicationId { get; set; }
        public string Message { get; set; } = "Postulación enviada exitosamente.";
    }
}
