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


    public int UserId { get; set; }

    public User User { get; set; }
}
