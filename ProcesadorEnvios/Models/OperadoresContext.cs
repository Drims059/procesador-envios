using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ProcesadorEnvios.Models
{
    public class OperadoresContext : DbContext
    {
        public OperadoresContext(DbContextOptions<OperadoresContext> options)
            : base(options)
        {
        }

        public DbSet<Operador> Operadores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder ModelBuilder){
            ModelBuilder.Entity<Operador>()
            .Property(e => e.cobertura)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}