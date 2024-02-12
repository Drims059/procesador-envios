using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ProcesadorEnvios.Models
{
    public class EnviosContext : DbContext
    {
        public EnviosContext(DbContextOptions<EnviosContext> options)
            : base(options)
        {
        }

        public DbSet<Envio> Envios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder ModelBuilder){
            ModelBuilder.Entity<Envio>()
            .Property(e => e.direccionDestino)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
                
            ModelBuilder.Entity<Envio>()
            .Property(e => e.direccionOrigen)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            ModelBuilder.Entity<Operador>()
            .Property(e => e.cobertura)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}