using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Exceptions;
using Dsw2026Ej15.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dsw2026Ej15.Api.Models;

namespace Dsw2026Ej15.Api.Controllers;

[Route("doctors")]
public class DoctorsController : AppController
{
    private readonly IPersistence _persistence;
    public DoctorsController(IPersistence persistence)
    {
        _persistence = persistence;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateDoctor(DoctorModel.Request request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("El nombre es requerido");
        }

        if (string.IsNullOrWhiteSpace(request.LicenseNumber))
        {
            throw new ValidationException("El numero de licencia es requerido");
        }

        var speciality = await _persistence.GetSpecialityById(request.SpecialityId);
        if (speciality is null)
        {
            throw new ValidationException("La especialidad indicada no existe");
        }

        var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
        await _persistence.SaveDoctor(doctor);

        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        var doctors = await _persistence.GetAllDoctors();

        return Ok(doctors.Select(d => new DoctorModel.Response(d.Name,
            d.LicenseNumber, d.Speciality?.Name)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var doctor = (await GetDoctor(id))!;
        return Ok(new DoctorModel.Response(
            doctor.Name, 
            doctor.LicenseNumber, 
            doctor.Speciality?.Name
            ));
    }
    

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var doctor = (await GetDoctor(id))!;
        doctor.Deactivate();
        await _persistence.UpdateDoctor(doctor);
        return NoContent();
    }

    private async Task<Doctor?> GetDoctor(Guid id)
    {
        return await _persistence.GetDoctorById(id) ?? throw new EntityNotFoundException("Medico no encontrado");
    }
}
