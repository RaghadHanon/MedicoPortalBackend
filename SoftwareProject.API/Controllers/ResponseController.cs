using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.Authentication;
using SoftwareProject.API.dto;
using SoftwareProject.API.Entites;

namespace SoftwareProject.API.Controllers
{
    [Route("api/response")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public ResponseController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext ??
                throw new ArgumentNullException(nameof(applicationDbContext));
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> CreateResponseToSpecificRequest(ResponseCreationDto requestCreation)
        {
            var request = await applicationDbContext.Requests
                            .FirstOrDefaultAsync(r => r.RequestId == requestCreation.RequestId);

            if(request == null)
            {
                return NotFound(new Response { Status = "error", Message = "Request with this id not exist !" });
            }

            var doctor = await applicationDbContext.Doctors
                            .FirstOrDefaultAsync(d => d.DoctorId == requestCreation.DoctorId);

            if(doctor == null)
            {
                return NotFound(new Response { Status = "error", Message = "Doctor with this id not exist !" });
            }

            var patient = await applicationDbContext.Patients
                            .FirstOrDefaultAsync(p => p.PatientId == requestCreation.PatientId);

            if (patient == null)
            {
                return NotFound(new Response { Status = "error", Message = "Patient with this id not exist !" });
            }

            var temp1 = requestCreation.Allergy;
            var ok1 = temp1.AllergyName != "" || temp1.Symptons != "";

            if (ok1)
            {
                var allergy = new Allergy
                {
                    RequestId = requestCreation.RequestId,
                    DoctorId = requestCreation.DoctorId,
                    PatientId = requestCreation.PatientId,
                    AddedBy = requestCreation.DoctorName,
                    AllergyName = requestCreation.Allergy.AllergyName,
                    Symptons = requestCreation.Allergy.Symptons,
                };

                applicationDbContext.Allergies.Add(allergy);
                await applicationDbContext.SaveChangesAsync();
            }

            var temp2 = requestCreation.ChronicDisease;
            var ok2 = temp2.ChronicDiseaseName != "" || temp2.Treatment != "" 
                || temp2.Causes != "" || temp2.Description != ""
                || temp2.Symptoms != "" || temp2.Digonsis != "" 
                || temp2.DateOfDiagonsis != "";
            if (ok2)
            {
                var CD = new ChronicDisease
                {
                    RequestId = requestCreation.RequestId,
                    DoctorId = requestCreation.DoctorId,
                    PatientId = requestCreation.PatientId,
                    AddedBy = requestCreation.DoctorName,
                    ChronicDiseaseName = requestCreation.ChronicDisease.ChronicDiseaseName,
                    Description = requestCreation.ChronicDisease.Description,
                    Causes = requestCreation.ChronicDisease.Causes,
                    Symptoms = requestCreation.ChronicDisease.Symptoms,
                    Digonsis = requestCreation.ChronicDisease.Digonsis,
                    Treatment = requestCreation.ChronicDisease.Treatment,
                    DateOfDiagonsis = requestCreation.ChronicDisease.DateOfDiagonsis
                };

                applicationDbContext.ChronicDiseases.Add(CD);
                await applicationDbContext.SaveChangesAsync();
            }

            var temp3 = requestCreation.Medicine;
            var ok3 = temp3.MedicinName != "" || temp3.Dosage != "" ||
                temp3.Frequency != "";
            if (ok3)
            {
                var medicine = new Medicine
                {
                    RequestId = requestCreation.RequestId,
                    DoctorId = requestCreation.DoctorId,
                    PatientId = requestCreation.PatientId,
                    AddedBy = requestCreation.DoctorName,
                    MedicineName = requestCreation.Medicine.MedicinName,
                    Dosage = requestCreation.Medicine.Dosage,
                    Frequency = requestCreation.Medicine.Frequency,
                    Instructions = requestCreation.Medicine.Instructions,
                    IsMaintenace = requestCreation.Medicine.IsMaintenace,
                };

                applicationDbContext.Medicines.Add(medicine);
                await applicationDbContext.SaveChangesAsync();
            }

            var temp4 = requestCreation.GeneralReport;

            var ok4 = temp4.Diagnosis != "" || temp4.Notes != "" 
                || temp4.TreatmentPlan != "" || temp4.Attachment != "";
            if (ok4)
            {
                var GR = new GeneralReport
                {
                    RequestId = requestCreation.RequestId,
                    DoctorId = requestCreation.DoctorId,
                    PatientId = requestCreation.PatientId,
                    AddedBy = requestCreation.DoctorName,
                    Date = DateTime.UtcNow,
                    Diagnosis = requestCreation.GeneralReport.Diagnosis,
                    TreatmentPlan = requestCreation.GeneralReport.TreatmentPlan,
                    Notes = requestCreation.GeneralReport.Notes,
                    Attachment = requestCreation.GeneralReport.Attachment,
                };

                applicationDbContext.GeneralReports.Add(GR);
                await applicationDbContext.SaveChangesAsync();
            }

            if(!ok1 && !ok2 && !ok3 && !ok4)
            {
                return BadRequest(new Response { Status = "error", Message = "You must add at least one response type !" });
            }

            request.IsAnswered = true;
            await applicationDbContext.SaveChangesAsync();

            return Ok(new Response
            {
                Status = "success",
                Message = "Responses added successfully"
            });
        }
    }
}
