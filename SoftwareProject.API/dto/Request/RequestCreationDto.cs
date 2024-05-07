using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.dto.Request
{
    public class RequestCreationDto
    {
        [Required(ErrorMessage = "Description is requered!")]
        public string Description { get; set; } = default!;
    }
}
