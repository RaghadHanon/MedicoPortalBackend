namespace SoftwareProject.API.Entites
{
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = "";
        public string Dosage { get; set; } = "";
        public string Frequency { get; set; } = "";
        public string Instructions { get; set; } = "";
        public string AddedBy { get; set; } = "";
        public bool IsMaintenace { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public int RequestId { get; set; }
        public Request Request { get; set; } = default!;
    }
}
