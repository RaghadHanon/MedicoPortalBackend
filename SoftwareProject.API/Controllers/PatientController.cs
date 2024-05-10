using SoftwareProject.API.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.Entites;
using Microsoft.AspNetCore.Authorization;
using SoftwareProject.API.dto.Request;
using SoftwareProject.API.dto.clinic;
using SoftwareProject.API.dto.BloodPressure;
using SoftwareProject.API.dto.BloodSugar;
using SoftwareProject.API.dto.Doctor;
using SoftwareProject.API.dto.MS;
using SoftwareProject.API.dto.Patient;

namespace SoftwareProject.API.Controllers
{
    [Route("api/patient")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public PatientController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext ??
                throw new ArgumentNullException(nameof(applicationDbContext));
        }
        [HttpGet("patient/{userName}")]
        public async Task<ActionResult<SpecificDoctorGetDto>> GetSpecificPatient(string userName)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Name == userName);

            if (user == null)
            {
                return NotFound(new Response { Status = "error", Message = "User with this user name not exist !" });
            }

            var patient = await applicationDbContext.Patients.FirstOrDefaultAsync(d => d.UserId == user.UserId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "error", Message = "Patient with this name not exist !" });
            }

            var patientToReturn = new SpecificPatientGetDto
            {
                PatientId = patient.PatientId,
                Name = user.Name,
                Gender = user.Gender,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                BloodType = patient.BloodType,
                Height = patient.Height,
                Weight = patient.Weight,
            };

            return Ok(patientToReturn);
        }
        [HttpPost("patient/profileData")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SetProfileData(PatientDataDto patientDataDto)
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var patient = await applicationDbContext.Patients
                                    .FirstOrDefaultAsync(d => d.UserId == userId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Patient not found." });
            }

            patient.BloodType = patientDataDto.BloodType;
            patient.Height = patientDataDto.Height;
            patient.Weight = patientDataDto.Weight;

            await applicationDbContext.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Patient Data updated successfully." });
        }
        [HttpGet("requests")]
        public async Task<ActionResult> GetAllRequestsForLoggedInPatient()
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var patient = await applicationDbContext.Patients
                                    .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Patient not found." });
            }

            var requests = await applicationDbContext.Requests
                                                     .Where(r => r.PatientId == patient.PatientId)
                                                     .Include(r => r.Doctor)
                                                     .ToListAsync();

            var patientUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == patient.UserId);

            var requestsToReturn = new List<RequestGetDto>();

            foreach (var request in requests)
            {
                var doctorUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == request.Doctor.UserId);

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
                ) ;
            }

            return Ok(requestsToReturn);
        }

        [HttpPost("request/{doctorid}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult> AddRequest(RequestCreationDto DtoRequest, int doctorid)
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var patient = await applicationDbContext.Patients
                                    .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Patient not found." });
            }

            var request = new Request
            {
                Description = DtoRequest.Description,
                Date = DateTime.UtcNow,
                PatientId = patient.PatientId,
                DoctorId = doctorid,
            };

            applicationDbContext.Requests.Add(request);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                    new Response { Status="Success", Message=$"New Request with id: {request.RequestId} added successfully"}
                );
        }

        [HttpPost("patient/bloodPressure")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdateBloodPressure(BloodPressureCreationDto BloodPressureDto)
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var patient = await applicationDbContext.Patients
                                    .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Patient not found." });
            }


            var bloodPressure = new BloodPressure
            {

                PatientId = patient.PatientId,
                Value = BloodPressureDto.Value,
                Date = DateTime.UtcNow,
            };

            applicationDbContext.BloodPressures.Add(bloodPressure);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                    new Response { Status = "Success", Message = $"New BloodPressure with id: {bloodPressure.BloodPressureId} added successfully" }
                );
        }

        [HttpGet("bloodPressures")]
        public async Task<ActionResult> GetAlBloodBressuresForLoggedInPatient()
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var patient = await applicationDbContext.Patients
                                    .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Patient not found." });
            }

            var bloodBressures = await applicationDbContext.BloodPressures
                                                     .Where(r => r.PatientId == patient.PatientId)
                                                     .ToListAsync();

            var patientUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == patient.UserId);

            var bloodBressuresToReturn = new List<BloodPressureGetDto>();

            foreach (var bloodBressure in bloodBressures)
            {

                bloodBressuresToReturn.Add(new BloodPressureGetDto
                {
                    BloodPressureId = bloodBressure.BloodPressureId,
                    Value = bloodBressure.Value,
                    Date = bloodBressure.Date,
                }
                );
            }

            return Ok(bloodBressuresToReturn);
        }

        [HttpPost("patient/bloodSugar")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdateBloodSugar(BloodSugarCreationDto BloodSugarDto)
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var patient = await applicationDbContext.Patients
                                    .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Patient not found." });
            }


            var bloodSugar = new BloodSugar
            {

                PatientId = patient.PatientId,
                Value = BloodSugarDto.Value,
                Date = DateTime.UtcNow,
            };

            applicationDbContext.BloodSugars.Add(bloodSugar);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                    new Response { Status = "Success", Message = $"New BloodPressure with id: {bloodSugar.BloodSugarId} added successfully" }
                );
        }

        [HttpGet("bloodSugars")]
        public async Task<ActionResult> GetAlBloodSugarsForLoggedInPatient()
        {
            var userIdClaim = User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token." });
            }

            var patient = await applicationDbContext.Patients
                                    .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Patient not found." });
            }

            var bloodSugars = await applicationDbContext.BloodSugars
                                                     .Where(r => r.PatientId == patient.PatientId)
                                                     .ToListAsync();

            var patientUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == patient.UserId);

            var bloodSugarsToReturn = new List<BloodSugarGetDto>();

            foreach (var bloodBressure in bloodSugars)
            {

                bloodSugarsToReturn.Add(new BloodSugarGetDto
                {
                    BloodSugarId = bloodBressure.BloodSugarId,
                    Value = bloodBressure.Value,
                    Date = bloodBressure.Date,
                }
                );
            }

            return Ok(bloodSugarsToReturn);
        }


    }
}
