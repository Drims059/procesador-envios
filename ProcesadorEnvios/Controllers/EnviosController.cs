using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProcesadorEnvios.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ProcesadorEnvios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnviosController : ControllerBase
    {
        private static HttpClient client = new HttpClient();
        private readonly EnviosContext _context;
        private readonly OperadoresContext _operadoresContext;
        private readonly SuscriptoresWebhookContext _suscriptoresContext;

        public EnviosController(EnviosContext context, OperadoresContext operadoresContext, SuscriptoresWebhookContext suscriptoresContext)
        {
            _context = context;
            _operadoresContext = operadoresContext;
            _suscriptoresContext = suscriptoresContext;
        }

        [HttpGet("{id}")]
        [Authorize("read:procesadorenvios")]
        //400 Bad Request FALTA
        public async Task<ActionResult<Envio>> ObtenerEnvio(int id)
        {
            var envio = await _context.Envios.FindAsync(id);

            if(envio == null)
            {
                throw new KeyNotFoundException("No se encontró Envío con ese ID");
            }

            _context.Entry(envio).Reference(e => e.operadorLogistico).Load();

            return envio;
        }

        [Authorize("write:procesadorenvios")]
        [HttpPost("{id}/novedades")]
        public async Task<IActionResult> ActualizacionEstado(int id, [FromBody] Dictionary<string, string> data)
        {
            var envio = await _context.Envios.FindAsync(id);
            _context.Entry(envio).Reference(e => e.operadorLogistico).Load();
            if (envio == null)
            {
                throw new KeyNotFoundException("No se encontró Envío con ese ID");
            }

            envio.estadoEnvio = data["estadoEnvio"];

            try
            {
                await _context.SaveChangesAsync();
                await NotificarCambioEstado(envio);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnvioExists(id))
                {
                    throw new KeyNotFoundException("No se encontró Envío con ese ID");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        
        [HttpPost]
        [Authorize("write:procesadorenvios")]
        //ERROR 405 falta
        public async Task<ActionResult<Envio>> AgregarEnvio(Envio envio)
        {
                envio.estadoEnvio = "creado";
                Operador operador = _operadoresContext.Operadores.AsEnumerable().Where(
                    o => o.cobertura.Contains(envio.direccionDestino[2])).FirstOrDefault();
                
                envio.operadorLogistico = new Operador(operador.nombre, operador.url, operador.cobertura, operador.auth0url,
                operador.clientId, operador.clientSecret, operador.audience);

                _context.Envios.Add(envio);
                
                await _context.SaveChangesAsync();

                //Obtencion del token
                var clientAuth0 = new RestClient(operador.auth0url);
                var requestAuth0 = new RestRequest("/oauth/token", Method.Post);
                requestAuth0.AddHeader("content-type", "application/json");
                requestAuth0.AddHeader("cache-control", "no-cache");
                requestAuth0.AddParameter("application/json", "{\"client_id\":\""+operador.clientId.ToString()+"\",\"client_secret\":\""+operador.clientSecret+
                "\",\"audience\":\""+operador.audience+"\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
                var responseAuth0 = clientAuth0.Execute(requestAuth0);
                dynamic respAuth0 = JObject.Parse(responseAuth0.Content);
                string token = respAuth0.access_token;

                client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", "bearer " + token);

                //POST al operador
                dynamic envioJSON = new JObject();
                envioJSON.id = envio.id;
                envioJSON.direccionOrigen = new JArray(envio.direccionOrigen);
                envioJSON.direccionDestino = new JArray(envio.direccionDestino);
                envioJSON.contactoComprador = envio.contactoComprador;
                envioJSON.detalleProducto = envio.detalleProducto;

                var content = new StringContent(envioJSON.ToString());
                
                var response = await client.PostAsync(envio.operadorLogistico.url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                return CreatedAtAction(nameof(ObtenerEnvio), new { id = envio.id }, envio);
        }

        private bool EnvioExists(long id)
        {
            return _context.Envios.Any(e => e.id == id);
        }

        private async Task NotificarCambioEstado(Envio envio)
        {
            var suscriptores = _suscriptoresContext.Suscriptores.AsEnumerable().Where(suscriptor => suscriptor.servicios.Contains("ENVIO_UPDATE"));

            dynamic notificacion = new JObject();
                notificacion.Id = envio.id;
                notificacion.Detalle = envio.detalleProducto;
                notificacion.operador = envio.operadorLogistico.nombre;
                notificacion.nuevoEstado = envio.estadoEnvio;

            foreach(SuscriptorWebhook s in suscriptores){
                EnviarNotificacion(s.urlRespuesta, notificacion.ToString());
            }
        }

        private async void EnviarNotificacion(string url, string notificacion)
        {
            client = new HttpClient();
            var response = await client.PostAsync(url, new StringContent(notificacion));
            Console.Write(response);
        }
    }
}
