using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcesadorEnvios.Models;

public class Operador {
    public int id { get; set;}
    public string nombre { get; set;}
    public string url { get; set;}
    public string[] cobertura { get; set;}
    public string auth0url {get; set;}
    public string clientId {get; set;}
    public string clientSecret {get; set;}
    public string audience {get; set;}
    

    public Operador(string nombre, string url, string[] cobertura, string auth0url, string clientId, string clientSecret, string audience)
    {
        this.nombre = nombre;
        this.url = url;
        this.cobertura = cobertura;
        this.auth0url = auth0url;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.audience = audience;
    }
}