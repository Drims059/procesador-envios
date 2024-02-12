using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcesadorEnvios.Models
{
    public class Envio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required()]
        public string[] direccionOrigen { get; set; }
        [Required()]
        public string[] direccionDestino { get; set; }
        [Required()]
        public string contactoComprador  { get; set; }
        public string? estadoEnvio { get; set;}
        [Required()]
        public string? detalleProducto { get; set;}
        public Operador? operadorLogistico { get; set;} 
    }
}