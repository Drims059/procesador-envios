using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcesadorEnvios.Models
{
    public class SuscriptorWebhook
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required()]
        public string urlRespuesta { get; set; }
        [Required()]
        public string[] servicios { get; set; }

        public SuscriptorWebhook(string urlRespuesta, string[] servicios)
        {
            this.urlRespuesta = urlRespuesta;
            this.servicios = servicios;
        }
    }
}