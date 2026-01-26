using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class Cliente
    {
        [Key]
        public int id { get; set; }
        public string ruc { get; set; } 
        public string razonSocial { get; set; }
        public string contactoPrincipal { get; set; }
        public string telefono { get; set; }
        public string estado { get; set; } // Mora, Activo, Inactivo
        public decimal saldoPendiente { get; set; }
    }
}
