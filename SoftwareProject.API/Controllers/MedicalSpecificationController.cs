using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.Authentication;
using SoftwareProject.API.Entites;
using SoftwareProject.API.dto.MedicalSpecification;

namespace SoftwareProject.API.Controllers
{
    [Route("api/MedicalSpecification")]
    [ApiController]
    public class MedicalSpecificationController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public MedicalSpecificationController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext ??
                throw new ArgumentNullException(nameof(applicationDbContext));
        }

        [HttpGet]
        public async Task< ActionResult< List< MedicalSpecificationGetDto > > > GetAllSpecifications()
        {
            var MS =  await applicationDbContext.MedicalSpecifications
                            .Include(ms => ms.Doctors)
                            .ToListAsync();

            var MSList = new List<MedicalSpecificationGetDto>();

            foreach (var ms in MS)
            {
                MSList.Add(new MedicalSpecificationGetDto
                {
                    MedicalSpecificationId = ms.MedicalSpecificationId,
                    Name = ms.Name
                });
            }

            return Ok(MSList);
        }

        [HttpPost]
        public async Task< ActionResult > AddMedicalSpecification(MedicalSpecificationCreationDto medicalSpecificationDto)
        {
            var medicalSpecification = await applicationDbContext.MedicalSpecifications
                                                                 .FirstOrDefaultAsync(ms => ms.Name == medicalSpecificationDto.Name);
        
            if (medicalSpecification != null)
            {
                return Conflict(
                    new Response { Status = "Error", Message = "Medical Specification Already Exist !" }
                );
            }

            var ms = new MedicalSpecification
            {
                Name = medicalSpecificationDto.Name
            };

            applicationDbContext.MedicalSpecifications.Add(ms);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                new Response { Status = "success", Message = $"Medical Specification with id: {ms.MedicalSpecificationId} added successfully!" }
            );
        }

        [HttpDelete("{medicalSpecificationId}")]
        public async Task< ActionResult > RemoveMedicalSpecification(int medicalSpecificationId)
        {
            var medicalSpecification = await applicationDbContext.MedicalSpecifications
                                                                 .FirstOrDefaultAsync(ms => ms.MedicalSpecificationId == medicalSpecificationId);

            if (medicalSpecification == null)
            {
                return BadRequest(
                    new Response { Status = "Error", Message = "Medical Specification does not Exist !" }
                );
            }

            applicationDbContext.MedicalSpecifications.Remove(medicalSpecification);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                new Response { Status = "success", Message = $"Medical Specification with id: {medicalSpecificationId} removed successfully!" }
            );
        }
    }
}
