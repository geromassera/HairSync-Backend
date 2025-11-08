using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Domain.Entities
{
    // ... (código de la clase Appointment sin cambios) ...
    public class Appointment
    {
        // ... (propiedades Id, DateTime, Status, Keys, etc.) ...
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        // --- Claves Foráneas ---

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int BarberId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int TreatmentId { get; set; }

        // --- Propiedades de Navegación ---
        // Asumiendo que tenés una entidad User para Cliente y Barbero
        // y entidades Branch y Treatment.

        [ForeignKey("ClientId")]
        public virtual User Client { get; set; }

        [ForeignKey("BarberId")]
        public virtual User Barber { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Treatment Treatment { get; set; }
    }

    /// <summary>
    /// Define los posibles estados de un turno.
    /// Simplificado según la nueva lógica.
    /// </summary>
    public enum AppointmentStatus
    {
        Confirmed,  // Creado y confirmado si pasa validaciones
        Cancelled   // Cancelado (por cliente o barbero)
        // 'Completed' ahora se calculará, no se almacenará.
    }
}
