namespace SoftwareProject.API.Entites
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string? Bio { get; set; } = "";
        public string? CVUrl { get; set; } = "";

        public int? MedicalSpecificationId { get; set; }
        public MedicalSpecification? MedicalSpecification { get; set; }

        public int? ClinicId { get; set; }
        public Clinic? Clinic { get; set; }
    }
}
