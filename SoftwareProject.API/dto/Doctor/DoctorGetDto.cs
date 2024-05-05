namespace SoftwareProject.API.dto.Doctor
{
    public class DoctorGetDto
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public int? ClinicId { get; set; }
        public string? ClinicName { get; set; }
        public int? MedicalSpecificationId { get; set; }
        public string? MedicalSpecificationName { get; set; }
    }
}
