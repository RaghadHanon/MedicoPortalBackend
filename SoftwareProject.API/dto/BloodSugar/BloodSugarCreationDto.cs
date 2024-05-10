using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.dto.BloodSugar
{
    public class BloodSugarCreationDto
    {
        [Required(ErrorMessage = "Value is requered!")]
        public string Value { get; set; } = default!;
    }
}
