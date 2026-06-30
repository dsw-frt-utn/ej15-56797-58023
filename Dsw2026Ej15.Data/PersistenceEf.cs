using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej15.Data
{
    public class PersistenceEf: IPersistence
    {
        private readonly Dsw2026Ej15DbContext _context;

        public PersistenceEf(Dsw2026Ej15DbContext context)
        {
            _context = context;
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
           await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            var activeDoctors = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Speciality)
                .Where(d => d.IsActive)
                .ToListAsync();
            return activeDoctors;
        }

        public async Task<Doctor?> GetDoctorByIdAsync(string id)
        {
            if (Guid.TryParse(id, out Guid doctorId))
            {
                var activeDoctor = await _context.Doctors
                    .AsNoTracking()
                    .Include(d => d.Speciality)
                    .FirstOrDefaultAsync(d => d.Id == doctorId);
                return activeDoctor;
            }
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync(d => d.LicenseNumber == id);
            
        }

        public async Task<Speciality?> GetSpecialityByIdAsync(Guid id)
        {
            return await _context.Specialities.FindAsync(id);
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }
    }
}
