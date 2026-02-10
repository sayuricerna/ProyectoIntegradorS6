//using System.ComponentModel.DataAnnotations;

//namespace ProyectoIntegradorS6G7.Models
//{
//    public class Usuario
//    {
//        [Key]
//        public int id { get; set; }
//        [Required]
//        public string email { get; set; }
//        [Required]
//        public string password { get; set; }
//        public string rol { get; set; } 
//        public string? rucAsociado { get; set; } 
//    }
//}

using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class Usuario
    {
        [Key]
        public int id { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string rol { get; set; } // "Administrador" o "Cliente"

        // CLIENTES
        public string? rucAsociado { get; set; }

        // ADMINISTRADORES
        public string? nombreCompleto { get; set; }
        public string? cedula { get; set; }
        public string? telefono { get; set; }
        public bool activo { get; set; } = true;
        public DateTime fechaCreacion { get; set; } = DateTime.Now;
        public string? creadoPor { get; set; }

        // Permisos (para futuro)
        public bool puedeCrearCreditos { get; set; } = true;
        public bool puedeGestionarCobranzas { get; set; } = true;
        public bool puedeVerReportes { get; set; } = true;
        public bool puedeConfigurar { get; set; } = false;
    }
}