using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Models
{
    public class CurriculumDto
    {
        public int? CurriculumId { get; set; } 
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime? UploadDate { get; set; } 
    }
}


