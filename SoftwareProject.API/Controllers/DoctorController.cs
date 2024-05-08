using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.dto.Doctor;
using SoftwareProject.API.Authentication;
using SoftwareProject.API.Entites;
using Microsoft.AspNetCore.Authorization;
using SoftwareProject.API.Enums;
using SoftwareProject.API.dto.clinic;
using SoftwareProject.API.dto.MS;
using SoftwareProject.API.dto.Request;

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

        [HttpGet("doctor/{userName}")]
        public async Task<ActionResult<SpecificDoctorGetDto>> GetSpecificDoctor(string userName)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Name == userName);

            if(user == null)
            {
                return NotFound(new Response { Status="error", Message="User with this user name not exist !"});
            }

            var doctor = await applicationDbContext.Doctors.Include(d => d.MedicalSpecification)
                                                           .Include(d => d.Clinic)
                                                           .FirstOrDefaultAsync(d => d.UserId == user.UserId);

            if(doctor == null)
            {
                return NotFound(new Response { Status = "error", Message = "Doctor with this name not exist !" });
            }

            var clinicToReturn = new ClinicGetDto
            {
                ClinicId = doctor.Clinic is null ? -1 : doctor.Clinic.ClinicId,
                ClinicName = doctor.Clinic is null ? "No Clinic Yet !" : doctor.Clinic.Name,
                Location = doctor.Clinic is null ? "No Clinic Yet !" : doctor.Clinic.Location,
                OpenHours = doctor.Clinic is null ? "No Clinic Yet !" : doctor.Clinic.OpenHours,
            };

            var msToReturn = new MedicalSpecificationGetDto
            {
                MSId = doctor.MedicalSpecificationId,
                MSName = doctor.MedicalSpecification.Name,
            };

            var doctorToReturn = new SpecificDoctorGetDto
            {
                DoctorId = doctor.DoctorId,
                Name = user.Name,
                Bio = doctor.Bio,
                CVUrl = doctor.CVUrl,
                Clinic = clinicToReturn,
                MedicalSpecification = msToReturn,
            };

            return Ok( doctorToReturn );
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
        public async Task<IActionResult> UpdateClinic(ClinicCreationDto clinicDto)
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
        [HttpGet("doctor/requests")]
        public async Task<ActionResult> GetAllRequestsForLoggedInDoctor()
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var doctor = await applicationDbContext.Doctors
                                    .FirstOrDefaultAsync(p => p.UserId == userId);

            if (doctor == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Doctor not found." });
            }

            var requests = await applicationDbContext.Requests
                                                     .Where(r => r.DoctorId == doctor.DoctorId)
                                                     .Include(r => r.Patient)
                                                     .ToListAsync();

            var doctorUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == doctor.UserId);

            var requestsToReturn = new List<RequestGetDto>();

            foreach (var request in requests)
            {
                var patientUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == request.Patient.UserId);

                requestsToReturn.Add(new RequestGetDto
                {
                    RequestId = request.RequestId,
                    Description = request.Description,
                    Date = request.Date,
                    PatientId = request.PatientId,
                    PatientName = patientUser.Name,
                    DoctorId = request.DoctorId,
                    DoctorName = doctorUser.Name,
                }
                );
            }

            return Ok(requestsToReturn);
        }
    }
}
