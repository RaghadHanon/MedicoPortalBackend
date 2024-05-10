using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.dto.BloodPressure
{
    public class BloodPressureCreationDto
    {

        [Required(ErrorMessage = "Value is requered!")]
        public string Value { get; set; } = default!;
    }
}
