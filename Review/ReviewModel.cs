using System;

namespace Malaffi.Models.Review
{
    public class ReviewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string ReviewDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Diagnosis { get; set; }
    }
}