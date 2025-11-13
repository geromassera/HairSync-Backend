using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int BarberId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int TreatmentId { get; set; }

        [Required]
        public decimal Price { get; set; }



        [ForeignKey("ClientId")]
        public virtual User Client { get; set; }

        [ForeignKey("BarberId")]
        public virtual User Barber { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Treatment Treatment { get; set; }
    }

    public enum AppointmentStatus
    {
        Confirmed,  
        Cancelled  
    }
}
