namespace SoftwareProject.API.dto.CD
{
    public class ChronicDiseaseGetDto
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

        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int RequestId { get; set; }
    }
}
