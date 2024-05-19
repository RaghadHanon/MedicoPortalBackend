using SoftwareProject.API.dto.alergies;
using SoftwareProject.API.dto.CD;
using SoftwareProject.API.dto.generalReport;
using SoftwareProject.API.dto.medicine;

namespace SoftwareProject.API.dto
{
    public class ResponseCreationDto
    {
        public int RequestId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public AllergyCreationDto Allergy { get; set; }
        public ChronicDiseaseCreationDto ChronicDisease { get; set; }
        public MedicineCreationDto Medicine { get; set; }
        public GeneralReportCreationDto GeneralReport { get; set; }
    }
}
