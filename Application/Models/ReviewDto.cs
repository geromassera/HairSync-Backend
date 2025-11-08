using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class ReviewDto
    {
        public int Rating { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        // Acá va el nombre formateado del cliente
        public string ClientName { get; set; }
    }

    public class CreateReviewDto
    {
        [Required(ErrorMessage = "El rating es obligatorio.")]
        [Range(1, 5, ErrorMessage = "El rating debe estar entre 1 y 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "El texto de la review no puede estar vacío.")]
        [MaxLength(1000, ErrorMessage = "La review no puede exceder los 1000 caracteres.")]
        public string Text { get; set; }
    }
}

