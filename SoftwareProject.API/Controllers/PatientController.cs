using SoftwareProject.API.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.Entites;
using Microsoft.AspNetCore.Authorization;
using SoftwareProject.API.dto.Request;
using SoftwareProject.API.dto.BloodPressure;
using SoftwareProject.API.dto.BloodSugar;
using SoftwareProject.API.dto.Doctor;
using SoftwareProject.API.dto.Patient;
using SoftwareProject.API.dto.alergies;
using SoftwareProject.API.dto.CD;
using SoftwareProject.API.dto.medicine;
using SoftwareProject.API.dto.generalReport;

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
        [HttpGet("{userName}")]
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
        
        [HttpPost("profileData")]
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
        public async Task<ActionResult> GetAllRequestsForLoggedInPatient(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isAnswered = null)
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

            var query = applicationDbContext.Requests
                                             .Where(r => r.PatientId == patient.PatientId)
                                             .Include(r => r.Doctor)
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


        [HttpGet("requests/{requestid}")]
        public async Task<ActionResult> GetRequestsForLoggedInPatient(int requestid)
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

            var request = await applicationDbContext.Requests
                .Include(r => r.Doctor)
                .Include(r => r.Allergy)
                .Include(r => r.ChronicDisease)
                .Include(r => r.Medicine)
                .Include(r => r.GeneralReport)
                .FirstOrDefaultAsync(r => r.RequestId == requestid && r.PatientId == patient.PatientId);

            if (request == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Request not found." });
            }

            var doctorUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == request.Doctor.UserId);

            var patientUser = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserId == patient.UserId);

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

            var request = new Entites.Request
            {
                Description = DtoRequest.Description,
                Date = DateTime.UtcNow,
                PatientId = patient.PatientId,
                DoctorId = doctorid,
                IsAnswered = false,
            };

            applicationDbContext.Requests.Add(request);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                    new Response { Status="Success", Message=$"New Request with id: {request.RequestId} added successfully"}
                );
        }

        [HttpPost("bloodPressure")]
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

        [HttpGet("bloodPressures/{userName}")]
        public async Task<ActionResult> GetAlBloodBressuresForLoggedInPatient(string userName)
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

        [HttpPost("bloodSugar")]
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

        [HttpGet("bloodSugars/{userName}")]
        public async Task<ActionResult> GetAlBloodSugarsForLoggedInPatient(string userName)
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

        [HttpGet("allergies/{userName}")]
        public async Task<ActionResult<List<AllergyGetDto>>> GetAllAllergiesForLoggedInPatient(string userName)
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


            var allergies = await applicationDbContext.Allergies.
                                Where(a => a.PatientId == patient.PatientId)
                                .ToListAsync();

            var allergiesToReturn = new List<AllergyGetDto>();

            foreach(var a in allergies)
            {
                allergiesToReturn.Add(new AllergyGetDto
                {
                    AllergyId = a.AllergyId,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    RequestId = a.RequestId,
                    AllergyName = a.AllergyName,
                    Symptons = a.Symptons,
                    AddedBy = a.AddedBy,
                });
            }

            return Ok(allergiesToReturn);
        }

        [HttpGet("chronicDisease/{userName}")]
        public async Task<ActionResult<List<ChronicDiseaseGetDto>>> GetAllChronicDiseaseForLoggedInPatient(string userName)
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

            var CDs = await applicationDbContext.ChronicDiseases.
                                Where(cd => cd.PatientId == patient.PatientId)
                                .ToListAsync();

            var CDsToReturn = new List<ChronicDiseaseGetDto>();

            foreach (var cd in CDs)
            {
                CDsToReturn.Add(new ChronicDiseaseGetDto
                {
                    ChronicDiseaseId = cd.ChronicDiseaseId,
                    DoctorId = cd.DoctorId,
                    PatientId = cd.PatientId,
                    RequestId = cd.RequestId,
                    ChronicDiseaseName = cd.ChronicDiseaseName,
                    Description = cd.Description,
                    Causes = cd.Causes,
                    Symptoms = cd.Symptoms,
                    Digonsis = cd.Digonsis,
                    Treatment = cd.Treatment,
                    DateOfDiagonsis = cd.DateOfDiagonsis,
                    AddedBy = cd.AddedBy
                });
            }

            return Ok(CDsToReturn);
        }

        [HttpGet("medicine/{userName}")]
        public async Task<ActionResult<List<MedicineGetDto>>> GetAllMedicineForLoggedInPatientt(string userName)
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

            var medicines = await applicationDbContext.Medicines.
                                Where(m => m.PatientId == patient.PatientId)
                                .ToListAsync();

            var medicinesToReturn = new List<MedicineGetDto>();

            foreach (var m in medicines)
            {
                medicinesToReturn.Add(new MedicineGetDto
                {
                    MedicineId = m.MedicineId,
                    DoctorId = m.DoctorId,
                    RequestId = m.RequestId,
                    PatientId = m.PatientId,
                    MedicineName = m.MedicineName,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    Instructions = m.Instructions,
                    AddedBy = m.AddedBy,
                    IsMaintenace = m.IsMaintenace
                });
            }

            return Ok(medicinesToReturn);
        }
        
    }
}





/*
 [HttpGet("generalReport")]
        public async Task<ActionResult<List<ChronicDiseaseGetDto>>> GetAllGeneralReportsForLoggedInPatient()
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

            var GRs = await applicationDbContext.GeneralReports.
                                Where(cd => cd.PatientId == patient.PatientId)
                                .ToListAsync();

            var GRsToReturn = new List<GeneralReportGetDto>();

            foreach (var gr in GRs)
            {
                GRsToReturn.Add(new GeneralReportGetDto
                {
                    GeneralReportId = gr.GeneralReportId,
                    DoctorId = gr.DoctorId,
                    PatientId = gr.PatientId,
                    RequestId = gr.RequestId,
                    Date = gr.Date,
                    Diagnosis = gr.Diagnosis,
                    TreatmentPlan = gr.TreatmentPlan,
                    Notes = gr.Notes,
                    AddedBy = gr.AddedBy,
                });
            }

            return Ok(GRsToReturn);
        }
*/