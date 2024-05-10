namespace SoftwareProject.API.dto.BloodPressure
{
    public class BloodPressureGetDto
    {
        public int BloodPressureId { get; set; }
        public string? Value { get; set; }
        public DateTime Date { get; set; }
    }
}
