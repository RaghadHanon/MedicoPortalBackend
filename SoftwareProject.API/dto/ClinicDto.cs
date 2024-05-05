using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.dto
{
    public class ClinicDto
    {
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string OpenHours { get; set; } = default!;
    }
}
