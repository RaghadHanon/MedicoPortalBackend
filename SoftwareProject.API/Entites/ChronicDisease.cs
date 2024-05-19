namespace SoftwareProject.API.Entites
{
    public class ChronicDisease
    {
        public int ChronicDiseaseId { get; set; }
        public string ChronicDiseaseName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Causes { get; set; } = "";
        public string Symptoms { get; set; } = "";
        public string Digonsis { get; set; } = "";
        public string Treatment { get; set; } = "";
        public string DateOfDiagonsis { get; set; } = "";
        public string AddedBy { get; set; } = "";

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public int RequestId { get; set; }
        public Request Request { get; set; } = default!;
    }
}
