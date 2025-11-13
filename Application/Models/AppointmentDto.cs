using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class AppointmentCreateDto
    {
        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public int BarberId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int TreatmentId { get; set; }
    }

    public class AppointmentViewDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }

        public string ClientName { get; set; }

        public string BarberName { get; set; }

        public string BranchName { get; set; }
        public string TreatmentName { get; set; }
        public decimal TreatmentPrice { get; set; }
        public string ClientPhone { get; set; }
    }
}

