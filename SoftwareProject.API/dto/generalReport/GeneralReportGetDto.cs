namespace SoftwareProject.API.dto.generalReport
{
    public class GeneralReportGetDto
    {
        public int GeneralReportId { get; set; }
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; } = "";
        public string TreatmentPlan { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Attachment { get; set; } = "";

        public string AddedBy { get; set; } = "";

        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int RequestId { get; set; }
    }
}
