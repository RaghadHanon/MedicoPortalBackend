using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.dto.clinic
{
    public class ClinicCreationDto
    {
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string OpenHours { get; set; } = default!;
    }
}
