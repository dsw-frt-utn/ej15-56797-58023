using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Data;
using Dsw2026Ej15.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Dsw2026Ej15.Api.Extensions;

namespace Dsw2026Ej15.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString= "Data Source=(localdb)\\MSSQLLocalDB;Database=Dsw2026Ej15;Integrated Security=True;Connection Timeout=30;Encrypt=True;TrustServerCertificate=True;";

            builder.Services.AddDbContext<Dsw2026Ej15DbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            // Registrar controladores
            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();
            // Configurar Health Check básico
            builder.Services.AddHealthChecks();
            // Inyección de dependencia (Singleton requerido por el enunciado f)
            builder.Services.AddScoped<IPersistence, PersistenceEf>();

            var app = builder.Build();

            await app.SeedSpecialitiesAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middleware de manejo de excepciones
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            // Mapear endpoint de Health Check
            app.MapHealthChecks("/health-check");

            app.Run();
        }
    }
}
