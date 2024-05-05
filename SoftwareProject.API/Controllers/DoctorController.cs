using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.dto.Doctor;
using SoftwareProject.API.Authentication;
using SoftwareProject.API.Entites;
using SoftwareProject.API.dto;
using Microsoft.AspNetCore.Authorization;
using SoftwareProject.API.Enums;

namespace SoftwareProject.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public DoctorController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext ??
                throw new ArgumentNullException(nameof(applicationDbContext));
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<List<DoctorGetDto>>> GetAllDoctors()
        {
            return await applicationDbContext.Doctors

                                        .Include(d => d.MedicalSpecification)

                                        .Join(applicationDbContext.Users,
                                                doctor => doctor.UserId,
                                                user => user.UserId,
                                                (doctor, user) => new DoctorGetDto
                                                {
                                                    DoctorId = doctor.DoctorId,
                                                    Name = user.Name,
                                                    MedicalSpecificationId = doctor.MedicalSpecification != null ?
                                                                               doctor.MedicalSpecificationId :
                                                                               null,
                                                    MedicalSpecificationName = doctor.MedicalSpecification != null ?
                                                                               doctor.MedicalSpecification.Name :
                                                                               "No Specification",
                                                    ClinicId = doctor.Clinic != null ?
                                                               doctor.ClinicId :
                                                               null,
                                                    ClinicName = doctor.Clinic != null ?
                                                                 doctor.Clinic.Name :
                                                                 "No Clinic",
                                                }
                                                )
                                        .ToListAsync();
        }

        [HttpGet("medicalSpecification/{medicalSpecificationId}/doctors")]
        public async Task<ActionResult<List<DoctorGetDto>>> GetAllDoctorsWithSpecificSpecification(int medicalSpecificationId)
        {
            var validMsId = await applicationDbContext.MedicalSpecifications
                    .AnyAsync(ms => ms.MedicalSpecificationId == medicalSpecificationId);

            if (!validMsId)
            {
                return BadRequest(
                    new Response { Status = "Error", Message = "Medical Specification does not exist !" }
                );
            }

            return await applicationDbContext.Doctors

                                        .Include(d => d.MedicalSpecification)

                                        .Where(d => d.MedicalSpecification != null &&
                                                    d.MedicalSpecificationId == medicalSpecificationId)

                                        .Join(applicationDbContext.Users,
                                                doctor => doctor.UserId,
                                                user => user.UserId,
                                                (doctor, user) => new DoctorGetDto
                                                {
                                                    DoctorId = doctor.DoctorId,
                                                    Name = user.Name,
                                                    MedicalSpecificationId = medicalSpecificationId,
                                                    MedicalSpecificationName = doctor.MedicalSpecification.Name
                                                }
                                                )

                                        .ToListAsync();
        }

        [HttpPost("doctor/clinic")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateClinic(ClinicDto clinicDto)
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var doctor = await applicationDbContext.Doctors
                                    .Include(d => d.Clinic)
                                    .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Doctor not found." });
            }

            if (doctor.Clinic == null)
            {
                doctor.Clinic = new Clinic
                {
                    Name = clinicDto.Name,
                    Location = clinicDto.Location,
                    OpenHours = clinicDto.OpenHours,
                    Doctor = doctor
                };
                applicationDbContext.Clinics.Add(doctor.Clinic);
            }
            else
            {
                doctor.Clinic.Name = clinicDto.Name;
                doctor.Clinic.Location = clinicDto.Location;
                doctor.Clinic.OpenHours = clinicDto.OpenHours;
            }

            await applicationDbContext.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Clinic information updated successfully." });
        }

        [HttpPost("doctor/profileData")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SetProfileData(DoctorDataDto doctorDataDto)
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var doctor = await applicationDbContext.Doctors
                                    .Include(d => d.Clinic)
                                    .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Doctor not found." });
            }

            doctor.CVUrl = doctorDataDto.CVUrl;
            doctor.Bio = doctorDataDto.Bio;

            await applicationDbContext.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Doctor Data updated successfully." });
        }
    }
}
