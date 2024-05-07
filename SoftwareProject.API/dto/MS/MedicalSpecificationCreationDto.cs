using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.dto.MS
{
    public class MedicalSpecificationCreationDto
    {
        [Required(ErrorMessage = "Medical Specification name is requered!")]
        public string Name { get; set; } = default!;
    }
}
