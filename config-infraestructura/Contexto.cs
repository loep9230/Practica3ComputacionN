using config_domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace config_infraestructura
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entornos>(entity =>
            {
                entity.HasKey(e => e.name);
                entity.Property(e => e.description);
                entity.Property(e => e.created_at).IsRequired();
                entity.Property(e => e.updated_at).IsRequired();
            });
            modelBuilder.Entity<Variables>(entity =>
            {
                entity.HasKey(e => new { e.name, e.name_entorno });
                entity.Property(e => e.value).IsRequired();
                entity.Property(e => e.description);
                entity.Property(e => e.is_sensitive).IsRequired();
                entity.Property(e => e.created_at).IsRequired();
                entity.Property(e => e.updated_at).IsRequired();
                entity.HasOne<Entornos>()
                      .WithMany()
                      .HasForeignKey(e => e.name_entorno)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<Entornos> Entornos { get; set; }
        public DbSet<Variables> Variables { get; set; }
}

}
