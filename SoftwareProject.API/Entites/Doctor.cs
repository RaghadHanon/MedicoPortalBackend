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

        public List<Request>? Requests { get; set; }

        public List<Allergy>? Allergys { get; set; }

        public List<ChronicDisease>? ChronicDiseases { get; set; }

        public List<GeneralReport>? GeneralReports { get; set; }

        public List<Medicine>? Medicines { get; set;}
    }
}
