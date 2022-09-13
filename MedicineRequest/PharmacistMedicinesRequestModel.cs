namespace Malaffi.Models.MedicineRequest
{
    public class PharmacistMedicinesRequestModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string TotalQuantity { get; set; }
        public string PerDayQuantity { get; set; }
        public string MedicineName { get; set; }
        public string CreationDate { get; set; }
        public string StatusString { get; set; }
    }
}