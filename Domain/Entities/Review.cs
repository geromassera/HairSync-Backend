using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Reviews")]
public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReviewId { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    [ForeignKey("AppointmentId")]
    public Appointment Appointment { get; set; } = null!;

    [Required, Range(1, 5)]
    public int Rating { get; set; }

    [Required, MaxLength(255)]
    public string Comment { get; set; } = string.Empty;
}
