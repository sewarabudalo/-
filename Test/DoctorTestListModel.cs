namespace Malaffi.Models.Test
{
    public class DoctorTestListModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string LabTechnicianName { get; set; }
        public string StatusString { get; set; }
        public string CreationDate { get; set; }
        public string FileUrl { get; set; }
    }
}