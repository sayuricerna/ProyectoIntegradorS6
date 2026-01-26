using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class Usuario
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        public string rol { get; set; } 
        public string? rucAsociado { get; set; } // en caso de ser cliente
    }
}
