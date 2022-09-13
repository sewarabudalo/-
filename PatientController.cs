using Malaffi.Models.Data;
using Malaffi.Models.Patient;
using Malaffi.Models.Test;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Malaffi.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult PatientInfo(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Doctor doctor = new Doctor();
            PatientInfoModel model = new PatientInfoModel();
            using (var db = new MalaffiEntities())
            {
                doctor = db.Doctors.SingleOrDefault(d => d.UserId == id);
                var dbPatient = db.Patients.SingleOrDefault(d => d.Id == Id);
                model.PatientInfo.FullName = dbPatient.FullName;
                model.PatientInfo.DBO = dbPatient.DBO.ToString("dd-MM-yyyy");
                model.PatientInfo.GenderString = dbPatient.Gender == 1 ? "Male" : "Female";
                model.PatientInfo.NationalNumber = dbPatient.NationalNumber;
                dbPatient.Reviews.Where(r => r.DoctorId == doctor.Id).Where(r => r.IsDeleted == false).ToList().ForEach(r => 
                {
                    model.ReviewList.Add(new Models.Review.PatientReviewListModel()
                    {
                        Id = r.Id,
                        Diagnosis = r.Diagnosis,
                        ReviewDate = r.ReviewDate.ToString("dd-MM-yyyy hh:mm"),
                    });
                });
                dbPatient.Tests.Where(t => t.IsDeleted == false && t.DoctorId == doctor.Id).ToList().ForEach(t => 
                {
                    model.TestList.Add(new Models.Test.PatientTestListModel()
                    {
                        Id = t.Id,
                        Description = t.Description,
                        LabTechnicianName = t.LabTechnicianId == null ? "-" : t.LabTechnician.FullName,
                        StatusString = ((TestStatusType)t.Status).ToString(),
                        CreationDate = t.CreationDate.ToString("dd-MM-yyyy hh:mm")
                    });
                });
                dbPatient.MedicineRequests.Where(m => m.IsDeleted == false && m.DoctorId == doctor.Id).ToList().ForEach(m =>
                {
                    model.MedicineRequestList.Add(new Models.MedicineRequest.PatientMedicineRequestModel()
                    {
                        Id = m.Id,
                        TotalQuantity = m.TotalQuantity,
                        PerDayQuantity = m.PerDayQuantity,
                        MedicineName = m.Medicine.Name,
                        PharmacistName = m.PharmacistId == null ? "-" : m.Pharmacist.FullName,
                        CreationDate = m.CreationDate.ToString("dd-MM-yyyy hh:mm")
                    });
                });
            }
            return View(model);
        }
    }
}