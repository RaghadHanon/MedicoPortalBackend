using System.ComponentModel.DataAnnotations;

namespace SoftwareProject.API.Authentication
{
    public class LoginRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = default!;
    }
}
