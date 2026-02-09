using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class ConfiguracionIA
    {
        [Key]
        public int id { get; set; }

        // interes
        [Required]
        [Range(0, 100)]
        public decimal interesMinimo { get; set; } = 5.0m; // 5%

        [Required]
        [Range(0, 100)]
        public decimal interesMaximo { get; set; } = 15.0m; // 15%

        [Required]
        [Range(0, 100)]
        public decimal interesRiesgoBajo { get; set; } = 5.0m;

        [Required]
        [Range(0, 100)]
        public decimal interesRiesgoMedio { get; set; } = 8.0m;

        [Required]
        [Range(0, 100)]
        public decimal interesRiesgoAlto { get; set; } = 12.0m;

        // morosidad
        [Required]
        public int diasParaMorosidad { get; set; } = 30; 

        [Required]
        public int diasParaRiesgoAlto { get; set; } = 60; 

        // para IA
        public decimal umbralRiesgoBajo { get; set; } = 0.3m; 

        public decimal umbralRiesgoMedio { get; set; } = 0.6m; 

        public decimal umbralRiesgoAlto { get; set; } = 1.0m; 

        // cuotas
        [Required]
        public int cuotasMinimasPorDefecto { get; set; } = 3;

        [Required]
        public int cuotasMaximasPorDefecto { get; set; } = 24;

        public DateTime fechaActualizacion { get; set; } = DateTime.Now;

        public string actualizadoPor { get; set; }
    }
}