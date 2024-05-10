namespace SoftwareProject.API.Entites
{
    public class BloodPressure
    {
        public int BloodPressureId { get; set; }
        public string Value { get; set; } = default!;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public Patient? Patient { get; set; }
        public int? PatientId { get; set; }
    }
}
