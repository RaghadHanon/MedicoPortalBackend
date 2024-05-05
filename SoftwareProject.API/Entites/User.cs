using Microsoft.AspNetCore.Identity;
using SoftwareProject.API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftwareProject.API.Entites
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? PhoneNumber { get; set; } = "";
        public string? Address { get; set; } = "";
        public string? UserImageUrl { get; set; } = "";
        public string Gender { get; set; } = default!;
        public Role Role { get; set; } = default!;

    }
}
