namespace SoftwareProject.API.Entites
{
    public class Allergy
    {
        public int AllergyId { get; set; }
        public string AllergyName { get; set; } = "";
        public string Symptons { get; set; } = "";
        public string AddedBy { get; set; } = "";

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public int RequestId { get; set; }
        public Request Request { get; set; } = default!;
    }
}
