using Dsw2026Ej15.Data;
using Dsw2026Ej15.Data.Dtos;
using Dsw2026Ej15.Domain.Entities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Ej15.Api.Extensions
{
    public static class DataSeederExtension
    {
        public static async Task SeedSpecialitiesAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Dsw2026Ej15DbContext>();

            if (!await context.Specialities.AnyAsync())
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "DataSources", "specialities.json");
                var json = await File.ReadAllTextAsync(jsonPath);
                var specialitiesDto = JsonSerializer.Deserialize<List<SpecialityDto>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (specialitiesDto != null && specialitiesDto.Any())
                {
                    // El mapeo ahora es directo y seguro gracias a la definición estricta del record
                    var specialities = specialitiesDto.Select(s =>
                        new Speciality(s.Name, s.Description, s.Id)
                    );

                    await context.Specialities.AddRangeAsync(specialities);
                    await context.SaveChangesAsync();
                }
            }
        }

    }
}
