using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int BarberId { get; set; }
        public int BranchId { get; set; }
        public string? BranchName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

