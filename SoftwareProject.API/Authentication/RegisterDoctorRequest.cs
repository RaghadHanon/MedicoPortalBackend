using SoftwareProject.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.Authentication
{
    public class RegisterDoctorRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = default!;
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "Gender required")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'")]
        public string Gender { get; set; } = default!;
        [Required(ErrorMessage = "MedicalSpecificationId required")]
        public int MedicalSpecificationId { get; set; }



        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;


    }
}
