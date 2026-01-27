using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class Cliente
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string ruc { get; set; }
        [Required]
        public string razonSocial { get; set; }
        [Required]
        public string contactoPrincipal { get; set; }
        [Required]
        [EmailAddress] // Validación de formato de correo
        public string email { get; set; }
        public string telefono { get; set; }
        public string estado { get; set; } = "Activo";
        public decimal saldoPendiente { get; set; } = 0;
    }
}
