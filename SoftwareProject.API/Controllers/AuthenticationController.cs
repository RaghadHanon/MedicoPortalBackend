using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.Authentication;
using SoftwareProject.API.Entites;
using SoftwareProject.API.Enums;

namespace SoftwareProject.API.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly TokenGenerator tokenGenerator;
        private readonly PasswordHasher<User> passwordHasher;

        public AuthenticationController(ApplicationDbContext applicationDbContext, TokenGenerator tokenGenerator, PasswordHasher<User> passwordHasher)
        {
            this.applicationDbContext = applicationDbContext ??
                throw new ArgumentNullException(nameof(applicationDbContext));

            this.tokenGenerator = tokenGenerator ??
                throw new ArgumentNullException(nameof(tokenGenerator));

            this.passwordHasher = passwordHasher ??
                throw new ArgumentNullException(nameof(passwordHasher));

        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUsers(LoginRequest loginRequest)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if(user == null)
            {
                return Unauthorized(
                    new Response { Status="Error", Message = "User with this Email dose not exist !"}
                );
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized(
                    new Response { Status = "Error", Message = "Password dose not match !" }
                );
            }

            var token = tokenGenerator.generateToken(user);

            if (token == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Failed to generate token." });
            }

            return Ok(new
            {
                Status = "Success",
                Message = "Logged in successfully!",
                Token = token
            });
        }

        [HttpPost("register-doctor")]
        public async Task<ActionResult> AddNewDoctorUser(RegisterDoctorRequest request)
        {
            var medicalSpecificationExists = await applicationDbContext.MedicalSpecifications
                                 .AnyAsync(ms => ms.MedicalSpecificationId == request.MedicalSpecificationId);

            if (!medicalSpecificationExists)
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid MedicalSpecificationId provided." });
            }

            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if(user != null)
            {
                return Conflict(
                        new Response { Status = "Error", Message = "User with the same email already exists !" }
                    );
            }

            user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);
            if (user != null)
            {
                return Conflict(
                        new Response { Status = "Error", Message = "User with the same name already exists !" }
                    );
            }

            user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = passwordHasher.HashPassword(null, request.Password),
                Gender = request.Gender,
                Role = Role.Doctor,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,

            };

            applicationDbContext.Users.Add(user);
            await applicationDbContext.SaveChangesAsync();

            var doctor = new Doctor
            {
                UserId = user.UserId,
                MedicalSpecificationId = request.MedicalSpecificationId,
            };

            applicationDbContext.Doctors.Add(doctor);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                    new Response { Status = "Success", Message = $"New Doctor with id: {doctor.DoctorId} Registered Successfully !"}
                );
        }

        [HttpPost("register-patient")]
        public async Task<ActionResult> AddNewPatientUser(RegisterPatientRequest request)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
            {
                return Conflict(
                        new Response { Status = "Error", Message = "User with the same email already exists !" }
                    );
            }

            user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);
            if (user != null)
            {
                return Conflict(
                        new Response { Status = "Error", Message = "User with the same name already exists !" }
                    );
            }

            user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = passwordHasher.HashPassword(null, request.Password),
                Gender = request.Gender,
                Role = Role.Patient,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
            };

            applicationDbContext.Users.Add(user);
            await applicationDbContext.SaveChangesAsync();

            var patient = new Patient
            {
                UserId = user.UserId,
            };

            applicationDbContext.Patients.Add(patient);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                    new Response { Status = "Success", Message = $"New Patient with id: {patient.PatientId} Registered Successfully !" }
                );
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddNewAdminUser(RegisterPatientRequest request)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
            {
                return Conflict(
                        new Response { Status = "Error", Message = "User with the same email already exists !" }
                    );
            }

            user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);
            if (user != null)
            {
                return Conflict(
                        new Response { Status = "Error", Message = "User with the same name already exists !" }
                    );
            }

            user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = passwordHasher.HashPassword(null, request.Password),
                Gender = request.Gender,
                Role = Role.Admin
            };

            applicationDbContext.Users.Add(user);
            await applicationDbContext.SaveChangesAsync();

            var admin = new Admin
            {
                UserId = user.UserId,
            };

            applicationDbContext.Admins.Add(admin);
            await applicationDbContext.SaveChangesAsync();

            return Ok(
                    new Response { Status = "Success", Message = $"New Admin with id: {admin.AdminId} Registered Successfully !" }
                );
        }
    }
}
