using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace config_infraestructura
{
    public class FactoryContexto : IDesignTimeDbContextFactory<Contexto>
    {
        public Contexto CreateDbContext(String[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Contexto>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Practica3CN;Username=postgres;Password=1234");
            return new Contexto(optionsBuilder.Options);
        }

    }
}
