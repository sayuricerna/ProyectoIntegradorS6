using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradorS6G7.Models
{
    public class Pago
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string idObligacion { get; set; }

        [Required]
        public int numeroCuota { get; set; }

        [Required]
        public decimal montoPagado { get; set; }

        [Required]
        public DateTime fechaPago { get; set; } = DateTime.Now;

        public string? metodoPago { get; set; }  

        public string? observaciones { get; set; } 

        public string? reciboNumero { get; set; }  

        public string? comprobanteRuta { get; set; } 

        public string? firmaBase64 { get; set; } 
        public string? registradoPor { get; set; } 

        public virtual Credito Credito { get; set; }
    }
}