using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Exceptions;
using Dsw2026Ej15.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dsw2026Ej15.Api.Models;

namespace Dsw2026Ej15.Api.Controllers
{
        [ApiController]
        [Route("api/doctors")]
        public class DoctorsController : ControllerBase
        {
            private readonly IPersistence _persistence;
            public DoctorsController(IPersistence persistence)
            {
                _persistence = persistence;
            }
            
            [HttpPost]
            public async Task<IActionResult> CreateDoctor(DoctorModel.Request dto)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new { error = "El nombre es requerido." });
                }

                if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
                {
                    return BadRequest(new { error = "El numero de licencia es requerido." });
                }

                var speciality = await _persistence.GetSpecialityByIdAsync(dto.SpecialityId);
                if (speciality == null)
                {
                    return BadRequest(new { error = "La especialidad indicada no existe." });
                }

                var doctor = new Doctor(dto.Name, dto.LicenseNumber, speciality);
                await _persistence.AddDoctorAsync(doctor);

                return StatusCode(StatusCodes.Status201Created);
            }
            
            [HttpGet]
            public async Task<IActionResult> Get()
            {
                var activeDoctors = await _persistence.GetActiveDoctorsAsync();

                var response = activeDoctors.Select(d => new DoctorResponseDto(
                    d.Name,
                    d.LicenseNumber,
                    d.Speciality?.Name ?? string.Empty
                ));

                return Ok(response);
            }
            
            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(string id)
            {
                var doctor = await _persistence.GetDoctorByIdAsync(id);

                if (doctor == null || !doctor.IsActive)
                {
                    return NotFound();
                }

                var response = new DoctorResponseDto(
                    doctor.Name,
                    doctor.LicenseNumber,
                    doctor.Speciality?.Name ?? string.Empty
                );

                return Ok(response);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(string id)
            {
                var doctor = await _persistence.GetDoctorByIdAsync(id);

                if (doctor == null || !doctor.IsActive)
                {
                    return NotFound();
                }

                doctor.Deactivate();

                return NoContent();
            }
        }
}