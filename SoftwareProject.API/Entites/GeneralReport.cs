namespace SoftwareProject.API.Entites
{
    public class GeneralReport
    {
        public int GeneralReportId { get; set; }
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; } = "";
        public string TreatmentPlan { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Attachment { get; set; } = "";

        public string AddedBy { get; set; } = "";


        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public int RequestId { get; set; }
        public Request Request { get; set; } = default!;
    }
}
