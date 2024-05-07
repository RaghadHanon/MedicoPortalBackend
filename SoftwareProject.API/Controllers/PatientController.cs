using SoftwareProject.API.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.Entites;
using Microsoft.AspNetCore.Authorization;
using SoftwareProject.API.dto.Request;

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
    }
}
