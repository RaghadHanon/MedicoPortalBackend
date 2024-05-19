using SoftwareProject.API.dto.alergies;
using SoftwareProject.API.dto.CD;
using SoftwareProject.API.dto.generalReport;
using SoftwareProject.API.dto.medicine;

namespace SoftwareProject.API.dto.Request
{
    public class RequestGetDto
    {
        public int RequestId { get; set; }
        public string? Description { get; set; } 
        public DateTime Date { get; set; } 
        public int? PatientId { get; set; }
        public string? PatientName { get; set; }
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set;}
        public bool IsAnswered { get; set; }

        public AllergyGetDto? Allergy { get; set; }
        public ChronicDiseaseGetDto? ChronicDisease { get; set; }
        public MedicineGetDto? Medicine { get; set; }
        public GeneralReportGetDto? GeneralReport { get; set; }
    }
}
