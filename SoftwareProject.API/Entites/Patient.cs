namespace SoftwareProject.API.Entites
{
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string? BloodType { get; set; } = "";
        public float Height { get; set;}
        public float Weight { get; set;}
    }
}
