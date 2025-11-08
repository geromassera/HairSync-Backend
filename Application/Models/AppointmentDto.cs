using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    /// <summary>
    /// DTO para crear un nuevo turno.
    /// </summary>
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

    /// <summary>
    /// DTO para mostrar la información de un turno.
    /// Actualizado para no exponer IDs de usuarios.
    /// </summary>
    public class AppointmentViewDto
    {
        // El ID del turno (Appointment) sí lo dejamos, 
        // para que el front pueda hacer acciones (ej. cancelar)
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }

        // Info del Cliente (sin ID)
        public string ClientName { get; set; }

        // Info del Barbero (sin ID)
        public string BarberName { get; set; }

        // Info del Turno
        public string BranchName { get; set; }
        public string TreatmentName { get; set; }
        public decimal TreatmentPrice { get; set; }
    }
}

