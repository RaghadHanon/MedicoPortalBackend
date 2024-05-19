namespace SoftwareProject.API.Entites
{
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string? BloodType { get; set; } = "";
        public float Height { get; set;}
        public float Weight { get; set;}

        public List<Request>? Requests { get; set; }


        public List<BloodPressure>? BloodPressure { get; set; }
        public List<BloodSugar>? BloodSugar { get; set; }

        public List<Allergy>? Allergys { get; set; }

        public List<ChronicDisease>? ChronicDiseases { get; set; }

        public List<GeneralReport>? GeneralReports { get; set; }

        public List<Medicine>? Medicines { get; set; }
    }
}
