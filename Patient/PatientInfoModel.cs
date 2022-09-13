using Malaffi.Models.MedicineRequest;
using Malaffi.Models.Review;
using Malaffi.Models.Test;
using System.Collections.Generic;

namespace Malaffi.Models.Patient
{
    public class PatientInfoModel
    {
        public PatientInfoModel()
        {
            PatientInfo = new PatientModel();
            ReviewList = new List<PatientReviewListModel>();
            TestList = new List<PatientTestListModel>();
            MedicineRequestList = new List<PatientMedicineRequestModel>();
        }
        public PatientModel PatientInfo { get; set; }
        public List<PatientReviewListModel> ReviewList { get; set; }
        public List<PatientTestListModel> TestList { get; set; }
        public List<PatientMedicineRequestModel> MedicineRequestList { get; set; }
    }
}