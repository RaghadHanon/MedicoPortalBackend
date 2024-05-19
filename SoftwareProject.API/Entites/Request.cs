namespace SoftwareProject.API.Entites
{
    public class Request
    {
        public int RequestId { get; set; }
        public string Description { get; set; } = default!;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsAnswered { get; set; }

        public Patient? Patient { get; set; }
        public int? PatientId { get; set; }

        public Doctor? Doctor { get; set; }
        public int? DoctorId { get; set;}

        public Allergy? Allergy { get; set; }

        public ChronicDisease? ChronicDisease { get; set; }

        public GeneralReport? GeneralReport { get; set; }

        public Medicine? Medicine { get; set; }
    }
}
