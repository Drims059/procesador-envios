using Microsoft.AspNetCore.Mvc;
using ProcesadorEnvios.Models;

namespace ProcesadorEnvios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuscriptoresController : ControllerBase
    {
        private readonly SuscriptoresWebhookContext _context;

        public SuscriptoresController(SuscriptoresWebhookContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuscriptorWebhook>> obtenerSuscriptor(int id)
        {
            var suscriptor = await _context.Suscriptores.FindAsync(id);

            if (suscriptor == null)
            {
                throw new KeyNotFoundException("No se encontró Suscriptor con ese ID");
            }

            return suscriptor;
        }

        [HttpGet]
        public IEnumerable<SuscriptorWebhook> obtenerSuscriptores()
        {
            var suscriptores = _context.Suscriptores.ToList();

            if (suscriptores == null)
            {
                throw new KeyNotFoundException("No se encontraron Suscriptores");
            }

            return suscriptores;
        }

        [HttpPost]
        public async Task<ActionResult<SuscriptorWebhook>> agregarSuscriptor(SuscriptorWebhook suscriptor)
        {
            var suscriptores = _context.Suscriptores.ToList();

            _context.Suscriptores.Add(suscriptor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(obtenerSuscriptor), new { id = suscriptor.id }, suscriptor);
        }
    }
}
