namespace SoftwareProject.API.Entites
{
    public class Clinic
    {
        public int ClinicId { get; set; }
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string OpenHours { get; set; } = default!;

        public Doctor Doctor { get; set; }
    }
}
