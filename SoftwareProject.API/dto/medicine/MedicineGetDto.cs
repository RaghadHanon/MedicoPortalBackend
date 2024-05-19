namespace SoftwareProject.API.dto.medicine
{
    public class MedicineGetDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = "";
        public string Dosage { get; set; } = "";
        public string Frequency { get; set; } = "";
        public string Instructions { get; set; } = "";
        public string AddedBy { get; set; } = "";
        public bool IsMaintenace { get; set; }

        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int RequestId { get; set; }
    }
}
