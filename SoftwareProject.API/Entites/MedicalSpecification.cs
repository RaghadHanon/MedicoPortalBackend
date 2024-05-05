namespace SoftwareProject.API.Entites
{
    public class MedicalSpecification
    {
        public int MedicalSpecificationId { get; set; }
        public string Name { get; set; } = default!;

        public List<Doctor>? Doctors { get; set; }
    }
}
