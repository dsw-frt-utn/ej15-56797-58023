using Dsw2026Ej15.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej15.Domain.Interfaces
{
    public interface IPersistence
    {
        Task AddDoctorAsync(Doctor doctor);
        Task UpdateDoctorAsync(Doctor doctor);
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
        Task<Doctor?> GetDoctorByIdAsync(string id);
        Task<Speciality?> GetSpecialityByIdAsync(Guid id);
    }
}
