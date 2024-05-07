using SoftwareProject.API.Entites;
using SoftwareProject.API.dto.clinic;
using SoftwareProject.API.dto.MS;

namespace SoftwareProject.API.dto.Doctor
{
    public class SpecificDoctorGetDto
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; } 
        public string CVUrl { get; set; } 
        public ClinicGetDto Clinic { get; set; }
        public MedicalSpecificationGetDto MedicalSpecification { get; set; }
    }
}
