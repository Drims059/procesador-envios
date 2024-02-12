using Microsoft.EntityFrameworkCore;

namespace ProcesadorEnvios.Models
{
    public class SuscriptoresWebhookContext : DbContext
    {
        public SuscriptoresWebhookContext(DbContextOptions<SuscriptoresWebhookContext> options) : base(options)
        {
        }

        public DbSet<SuscriptorWebhook> Suscriptores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder ModelBuilder)
        {
            ModelBuilder.Entity<SuscriptorWebhook>()
            .Property(e => e.servicios)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}