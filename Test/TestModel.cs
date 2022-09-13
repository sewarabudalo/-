using System;

namespace Malaffi.Models.Test
{
    public class TestModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public Nullable<int> LabTechnicianId { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsDownloaded { get; set; }
        public System.DateTime CreationDate { get; set; }
    }
}