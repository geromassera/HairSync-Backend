using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int AppointmentId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class CreateReviewDto
    {
        [Required(ErrorMessage = "La calificación es obligatoria.")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [MaxLength(255, ErrorMessage = "El comentario no puede superar los 255 caracteres.")]
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateReviewDto
    {
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int? Rating { get; set; }

        [MaxLength(255, ErrorMessage = "El comentario no puede superar los 255 caracteres.")]
        public string? Comment { get; set; }
    }
}

