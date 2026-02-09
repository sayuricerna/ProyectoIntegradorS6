using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class Administrador
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string nombreCompleto { get; set; }

        [Required]
        public string cedula { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; } 

        public string telefono { get; set; }

        public bool activo { get; set; } = true;

        public DateTime fechaCreacion { get; set; } = DateTime.Now;

        public string creadoPor { get; set; }

        //permisos en un futuro booleanos
        public bool puedeCrearCreditos { get; set; } = true;
        public bool puedeGestionarCobranzas { get; set; } = true;
        public bool puedeVerReportes { get; set; } = true;

        public bool puedeConfigurar { get; set; } = false; // Solo admin principal
    }
}