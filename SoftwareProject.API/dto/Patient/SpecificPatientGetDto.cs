using SoftwareProject.API.dto.clinic;
using SoftwareProject.API.dto.MS;

namespace SoftwareProject.API.dto.Patient
{
    public class SpecificPatientGetDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }

        public string? BloodType { get; set; } = "";
        public float Height { get; set; }
        public float Weight { get; set; }
    }
}
