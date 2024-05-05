using Microsoft.EntityFrameworkCore;
using SoftwareProject.API.Entites;
using SoftwareProject.API.Enums;

namespace SoftwareProject.API
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<MedicalSpecification> MedicalSpecifications { get; set; }
        public DbSet<Clinic> Clinics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Patient>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Patient>(a => a.UserId)
                .IsRequired();

            modelBuilder.Entity<Doctor>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Doctor>(o => o.UserId)
                .IsRequired();

            modelBuilder.Entity<Admin>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Admin>(o => o.UserId)
                .IsRequired();


            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.MedicalSpecification)
                .WithMany(ms => ms.Doctors)
                .HasForeignKey(d => d.MedicalSpecificationId)
                .IsRequired(false)  
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
