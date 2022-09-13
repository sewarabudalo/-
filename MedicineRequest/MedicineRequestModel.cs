using System;

namespace Malaffi.Models.MedicineRequest
{
    public class MedicineRequestModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string TotalQuantity { get; set; }
        public string PerDayQuantity { get; set; }
        public Nullable<int> PharmacistId { get; set; }
        public int MedicineId { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> Status { get; set; }
    }
}