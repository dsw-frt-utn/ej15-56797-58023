using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Data;
using Dsw2026Ej15.Api.Middleware;

namespace Dsw2026Ej15.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Registrar controladores
            builder.Services.AddControllers();
            
            builder.Services.AddSwaggerGen();
            // Configurar Health Check básico
            builder.Services.AddHealthChecks();
            // Inyección de dependencia (Singleton requerido por el enunciado f)
            builder.Services.AddSingleton<IPersistence, PersistenceInMemory>();

            var app = builder.Build();
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
