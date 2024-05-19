using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.dto.Doctor;
using SoftwareProject.API.Authentication;
using SoftwareProject.API.Entites;
using Microsoft.AspNetCore.Authorization;
using SoftwareProject.API.Enums;
using SoftwareProject.API.dto.clinic;
using SoftwareProject.API.dto.alergies;
using SoftwareProject.API.dto.MS;
using SoftwareProject.API.dto.Request;
using SoftwareProject.API.dto.CD;
using SoftwareProject.API.dto.generalReport;
using SoftwareProject.API.dto.medicine;


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
        public async Task<ActionResult<List<DoctorGetDto>>> GetAllDoctors(
            [FromQuery] string searchName = null,
            [FromQuery] int? medicalSpecificationId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = applicationDbContext.Doctors
                                            .Include(d => d.MedicalSpecification)
                                            .Join(applicationDbContext.Users,
                                                doctor => doctor.UserId,
                                                user => user.UserId,
                                                (doctor, user) => new
                                                {
                                                    doctor,
                                                    user
                                                });

            // Apply search filter
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(q => q.user.Name.Contains(searchName));
            }

            // Apply medical specification filter
            if (medicalSpecificationId.HasValue)
            {
                query = query.Where(q => q.doctor.MedicalSpecificationId == medicalSpecificationId.Value);
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var doctors = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new DoctorGetDto
                {
                    DoctorId = q.doctor.DoctorId,
                    Name = q.user.Name,
                    Gender = q.user.Gender,
                    MedicalSpecificationId = q.doctor.MedicalSpecification != null ?
                                             q.doctor.MedicalSpecificationId :
                                             null,
                    MedicalSpecificationName = q.doctor.MedicalSpecification != null ?
                                               q.doctor.MedicalSpecification.Name :
                                               "No Specification",
                    ClinicId = q.doctor.Clinic != null ?
                               q.doctor.ClinicId :
                               null,
                    ClinicName = q.doctor.Clinic != null ?
                                 q.doctor.Clinic.Name :
                                 "No Clinic",
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Doctors = doctors
            });
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
                Gender = user.Gender,
                Address =user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
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
                                                    Gender = user.Gender,
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
        public async Task<ActionResult> GetAllRequestsForLoggedInDoctor(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isAnswered = null)
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

            var query = applicationDbContext.Requests
                                             .Where(r => r.DoctorId == doctor.DoctorId)
                                             .Include(r => r.Patient)
                                             .Include(r => r.Allergy)
                                             .Include(r => r.ChronicDisease)
                                             .Include(r => r.Medicine)
                                             .Include(r => r.GeneralReport)
                                             .AsQueryable();

            // Apply filter on IsAnswered
            if (isAnswered.HasValue)
            {
                query = query.Where(r => r.IsAnswered == isAnswered.Value);
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var requests = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                    IsAnswered = request.IsAnswered,
                    GeneralReport = request.GeneralReport is null ? null : new GeneralReportGetDto
                    {
                        DoctorId = request.GeneralReport.DoctorId,
                        PatientId = request.GeneralReport.PatientId,
                        RequestId = request.RequestId,
                        GeneralReportId = request.GeneralReport.GeneralReportId,
                        Date = request.GeneralReport.Date,
                        Diagnosis = request.GeneralReport.Diagnosis,
                        TreatmentPlan = request.GeneralReport.TreatmentPlan,
                        Notes = request.GeneralReport.Notes,
                        Attachment = request.GeneralReport.Attachment,
                    },
                    Medicine = request.Medicine is null ? null : new MedicineGetDto
                    {
                        DoctorId = request.Medicine.DoctorId,
                        PatientId = request.Medicine.PatientId,
                        RequestId = request.RequestId,
                        MedicineId = request.Medicine.MedicineId,
                        MedicineName = request.Medicine.MedicineName,
                        Dosage = request.Medicine.Dosage,
                        Frequency = request.Medicine.Frequency,
                        Instructions = request.Medicine.Instructions,
                        AddedBy = request.Medicine.AddedBy,
                        IsMaintenace = request.Medicine.IsMaintenace,
                    },
                    ChronicDisease = request.ChronicDisease is null ? null : new ChronicDiseaseGetDto
                    {
                        DoctorId = request.ChronicDisease.DoctorId,
                        PatientId = request.ChronicDisease.PatientId,
                        RequestId = request.RequestId,
                        ChronicDiseaseId = request.ChronicDisease.ChronicDiseaseId,
                        ChronicDiseaseName = request.ChronicDisease.ChronicDiseaseName,
                        Description = request.ChronicDisease.Description,
                        Causes = request.ChronicDisease.Causes,
                        Symptoms = request.ChronicDisease.Symptoms,
                        Digonsis = request.ChronicDisease.Digonsis,
                        Treatment = request.ChronicDisease.Treatment,
                        DateOfDiagonsis = request.ChronicDisease.DateOfDiagonsis,
                        AddedBy = request.ChronicDisease.AddedBy,
                    },
                    Allergy = request.Allergy is null ? null : new AllergyGetDto
                    {
                        DoctorId = request.Allergy.DoctorId,
                        PatientId = request.Allergy.PatientId,
                        RequestId = request.RequestId,
                        AllergyId = request.Allergy.AllergyId,
                        AllergyName = request.Allergy.AllergyName,
                        Symptons = request.Allergy.Symptons,
                        AddedBy = request.Allergy.AddedBy,
                    }
                });
            }

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Requests = requestsToReturn
            });
        }


        [HttpGet("doctor/requests/{requestid}")]
        public async Task<ActionResult> GetRequestsForLoggedInDoctor(int requestid)
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

            var request = await applicationDbContext.Requests
                .Include(r => r.Patient)
                .Include(r => r.Allergy)
                .Include(r => r.ChronicDisease)
                .Include(r => r.Medicine)
                .Include(r => r.GeneralReport)
                .FirstOrDefaultAsync(r => r.RequestId == requestid && r.DoctorId == doctor.DoctorId);

            if (request == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Request not found." });
            }

            var doctorUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == doctor.UserId);

            var patientUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == request.Patient.UserId);

            var requestToReturn = new RequestGetDto
            {
                RequestId = request.RequestId,
                Description = request.Description,
                Date = request.Date,
                PatientId = request.PatientId,
                PatientName = patientUser.Name,
                DoctorId = request.DoctorId,
                DoctorName = doctorUser.Name,
                IsAnswered = request.IsAnswered,
                GeneralReport = request.GeneralReport is null ? null : new GeneralReportGetDto
                {
                    DoctorId = request.GeneralReport.DoctorId,
                    PatientId = request.GeneralReport.PatientId,
                    RequestId = request.RequestId,
                    GeneralReportId = request.GeneralReport.GeneralReportId,
                    Date = request.GeneralReport.Date,
                    Diagnosis = request.GeneralReport.Diagnosis,
                    TreatmentPlan = request.GeneralReport.TreatmentPlan,
                    Notes = request.GeneralReport.Notes,
                    Attachment = request.GeneralReport.Attachment,
                },
                Medicine = request.Medicine is null ? null : new MedicineGetDto
                {
                    DoctorId = request.Medicine.DoctorId,
                    PatientId = request.Medicine.PatientId,
                    RequestId = request.RequestId,
                    MedicineId = request.Medicine.MedicineId,
                    MedicineName = request.Medicine.MedicineName,
                    Dosage = request.Medicine.Dosage,
                    Frequency = request.Medicine.Frequency,
                    Instructions = request.Medicine.Instructions,
                    AddedBy = request.Medicine.AddedBy,
                    IsMaintenace = request.Medicine.IsMaintenace,
                },
                ChronicDisease = request.ChronicDisease is null ? null : new ChronicDiseaseGetDto
                {
                    DoctorId = request.ChronicDisease.DoctorId,
                    PatientId = request.ChronicDisease.PatientId,
                    RequestId = request.RequestId,
                    ChronicDiseaseId = request.ChronicDisease.ChronicDiseaseId,
                    ChronicDiseaseName = request.ChronicDisease.ChronicDiseaseName,
                    Description = request.ChronicDisease.Description,
                    Causes = request.ChronicDisease.Causes,
                    Symptoms = request.ChronicDisease.Symptoms,
                    Digonsis = request.ChronicDisease.Digonsis,
                    Treatment = request.ChronicDisease.Treatment,
                    DateOfDiagonsis = request.ChronicDisease.DateOfDiagonsis,
                    AddedBy = request.ChronicDisease.AddedBy,
                },
                Allergy = request.Allergy is null ? null : new AllergyGetDto
                {
                    DoctorId = request.Allergy.DoctorId,
                    PatientId = request.Allergy.PatientId,
                    RequestId = request.RequestId,
                    AllergyId = request.Allergy.AllergyId,
                    AllergyName = request.Allergy.AllergyName,
                    Symptons = request.Allergy.Symptons,
                    AddedBy = request.Allergy.AddedBy,
                }
            };
                

            return Ok(requestToReturn);
        }
    }
}
