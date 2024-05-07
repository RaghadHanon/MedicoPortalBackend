using SoftwareProject.API.Entites;

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

    }
}
