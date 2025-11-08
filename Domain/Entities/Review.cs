using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Reviews")]
public class Review
{
    public int Id { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    public string Text { get; set; }

    public DateTime CreatedAt { get; set; }

    // --- Relación con el Usuario (Cliente) ---

    // CORRECCIÓN: Este es el Foreign Key, ahora como int
    public int UserId { get; set; }

    // Propiedad de navegación para traer los datos del usuario
    public User User { get; set; }
}
