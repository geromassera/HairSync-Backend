using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Curriculums")]
    public class Curriculum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CurriculumId { get; set; }

        [Required]
        public int UserId { get; set; }  

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty; 

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [Required]
        public CurriculumStatus Status { get; set; } = CurriculumStatus.Pending;

    }
}

