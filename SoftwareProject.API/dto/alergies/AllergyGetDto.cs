namespace SoftwareProject.API.dto.alergies
{
    public class AllergyGetDto
    {
        public int AllergyId { get; set; }
        public string AllergyName { get; set; } = "";
        public string Symptons { get; set; } = "";
        public string AddedBy { get; set; } = "";

        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int RequestId { get; set; }
    }
}
