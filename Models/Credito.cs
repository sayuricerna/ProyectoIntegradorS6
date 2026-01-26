using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class Credito
    {
        [Key]
        public string idObligacion { get; set; } // Ej: #OBL-001
        public string rucCliente { get; set; }
        public decimal montoTotal { get; set; }
        public int cuotas { get; set; }
        public decimal tasaInteres { get; set; }
        public DateTime fechaRegistro { get; set; } = DateTime.Now;

        // Campos para API en py
        public string nivelRiesgoIA { get; set; } // Alto, Medio, Bajo
        public string recomendacionIA { get; set; }
        public string estado { get; set; } // Pendiente, Pagado, Vencido
    }
}
