using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej15.Data;

public class PersistenceEf : IPersistence
{
    private readonly Dsw2026Ej15DbContext _context;
    public PersistenceEf(Dsw2026Ej15DbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Doctor>> GetAllDoctors()
    {
        return _context.Doctors.Where(d => d.IsActive);
    }
    public async Task<Doctor?> GetDoctorById(Guid id)
    {
        return await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
    }
    public async Task<Speciality?> GetSpecialityById(Guid id)
    {
        return await _context.Specialities.FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task SaveDoctor(Doctor doctor)
    {
        await _context.AddAsync(doctor);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateDoctor(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync();
    }
}
