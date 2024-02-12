using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProcesadorEnvios.Models;

namespace ProcesadorEnvios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OperadoresController : ControllerBase
    {
        private readonly OperadoresContext _context;

        public OperadoresController(OperadoresContext context)
        {
            _context = context;
        }

        //400 Bad Request FALTA
        [HttpGet("{id}")]
        [Authorize("read:procesadorenvios")]
        public async Task<ActionResult<Operador>> obtenerOperador(int id)
        {
            var operador = await _context.Operadores.FindAsync(id);

            if (operador == null)
            {
                throw new KeyNotFoundException("No se encontró Operador con ese ID");
            }

            return operador;
        }

        //ERROR 405 falta
        [HttpPost]
        [Authorize("write:procesadorenvios")]
        public async Task<ActionResult<Operador>> agregarOperador(Operador operador)
        {
            _context.Operadores.Add(operador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(obtenerOperador), new { id = operador.id }, operador);
        }

        //400 Bad Request FALTA
        [HttpDelete("{id}")]
        [Authorize("write:procesadorenvios")]
        public async Task<ActionResult<Operador>> eliminarOperador(int id)
        {
            var operador = await _context.Operadores.FindAsync(id);

            if (operador == null)
            {
                throw new KeyNotFoundException("No se encontró Operador con ese ID");
            }

            _context.Operadores.Remove(operador);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}