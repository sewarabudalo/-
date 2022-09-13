namespace Malaffi.Models.Patient
{
    public class PatientModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string DBO { get; set; }
        public string GenderString { get; set; }
        public string NationalNumber { get; set; }
    }
}