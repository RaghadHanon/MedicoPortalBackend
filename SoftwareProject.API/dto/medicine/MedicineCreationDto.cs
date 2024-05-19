namespace SoftwareProject.API.dto.medicine
{
    public class MedicineCreationDto
    {
        public string MedicinName { get; set; } = "";
        public string Dosage { get; set; } = "";
        public string Frequency { get; set; } = "";
        public string Instructions { get; set; } = "";
        public bool IsMaintenace { get; set; }
    }
}
