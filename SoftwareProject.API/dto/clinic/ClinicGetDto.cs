namespace SoftwareProject.API.dto.clinic
{
    public class ClinicGetDto
    {
        public int? ClinicId { get; set; }
        public string ClinicName { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string OpenHours { get; set; } = default!;
    }
}
