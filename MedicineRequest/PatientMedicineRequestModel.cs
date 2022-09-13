namespace Malaffi.Models.MedicineRequest
{
    public class PatientMedicineRequestModel
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string TotalQuantity { get; set; }
        public string PerDayQuantity { get; set; }
        public string PharmacistName { get; set; }
        public string MedicineName { get; set; }
        public string CreationDate { get; set; }
    }
}