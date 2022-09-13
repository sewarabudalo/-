namespace Malaffi.Models.Review
{
    public class DoctorReviewListModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        public string PatientGender { get; set; }
        public string Diagnosis { get; set; }
        public string ReviewDate { get; set; }
    }
}