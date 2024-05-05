using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.dto.MedicalSpecification
{
    public class MedicalSpecificationCreationDto
    {
        [Required(ErrorMessage = "Medical Specification name is requered!")]
        public string Name { get; set; } = default!;
    }
}
