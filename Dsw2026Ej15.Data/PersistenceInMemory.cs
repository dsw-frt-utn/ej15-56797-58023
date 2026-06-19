using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Data.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dsw2026Ej15.Data
{
    public class PersistenceInMemory : IPersistence
    {
        private List<Speciality> _specialities = new List<Speciality>();
        private List<Doctor> _doctors = new List<Doctor>();

        public PersistenceInMemory() {
            LoadSpecialities();
        }

        private void LoadSpecialities()
        {
            try 
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    "DataSources", "specialities.json");
                var json = File.ReadAllText(jsonPath);
                var specialities = JsonSerializer.Deserialize<List<SpecialityDto>>(json, 
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new List<SpecialityDto>();

                _specialities = specialities
                    .Select(s => {
                        // Acepta Id del DTO como string o cualquier tipo convertible a string
                        var idString = Convert.ToString(s.Id);
                        var idGuid = Guid.TryParse(idString, out var g) ? g : Guid.Empty;
                        return new Speciality(s.Name, s.Description, idGuid);
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading specialities: {ex.Message}");
            }
        }
        public Task AddDoctorAsync(Doctor doctor)
        {
            _doctors.Add(doctor);
            return Task.CompletedTask;
        }

        public Task UpdateDoctorAsync(Doctor doctor)
        {
           int index = _doctors.FindIndex(d => d.Id == doctor.Id);
            if (index != -1)
            {
                _doctors[index] = doctor;
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            var activeDoctors = _doctors.Where(d => d.IsActive).AsEnumerable();
            return Task.FromResult(activeDoctors);
        }

        public Task<Doctor?> GetDoctorByIdAsync(string id)
        {
            var doctor = _doctors.FirstOrDefault(d => d.LicenseNumber == id);
            return Task.FromResult(doctor);
        }

        public Task<Speciality?> GetSpecialityByIdAsync(Guid id)
        {
            var speciality = _specialities.SingleOrDefault(s => s.Id == id);
            return Task.FromResult(speciality);
        }
    }
}
