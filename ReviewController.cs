using Malaffi.Models;
using Malaffi.Models.Data;
using Malaffi.Models.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Malaffi.Controllers
{
    public class ReviewController : Controller
    {
        [Authorize]
        public ActionResult DoctorReview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDoctorReviewData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Doctor doctor = new Doctor();
            using (var db = new MalaffiEntities())
            {
                doctor = db.Doctors.SingleOrDefault(d => d.UserId == id);
                List<Review> dbReviews = new List<Review>();
                dbReviews = db.Reviews.Where(r => r.IsDeleted == false && r.DoctorId == doctor.Id 
                && (r.PatientId.ToString().Contains(param.sSearch)
                || r.Patient.NationalNumber.Contains(param.sSearch)
                || r.Patient.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<DoctorReviewListModel> Reviews = new List<DoctorReviewListModel>();
                dbReviews.ForEach(r => {
                    Reviews.Add(new DoctorReviewListModel()
                    {
                        Id = r.Id,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        PatientGender = r.Patient.Gender == 1 ? "Male" : "Female",
                        Diagnosis = r.Diagnosis,
                        PatientAge = DateTime.Now.Year - r.Patient.DBO.Year,
                        ReviewDate = r.ReviewDate.ToString("dd-MM-yyyy hh:mm"),
                    });
                });
                return Json(new
                {
                    aaData = Reviews.OrderByDescending(c => c.ReviewDate),
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = Reviews.Count,
                    iTotalRecords = Reviews.Count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ReviewModel model)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Doctor doctor = new Doctor();
            using (var db = new MalaffiEntities())
            {
                doctor = db.Doctors.SingleOrDefault(d => d.UserId == id);
                db.Reviews.Add(new Review()
                {
                    PatientId = model.PatientId,
                    ReviewDate = DateTime.Now,
                    DoctorId = doctor.Id,
                    Diagnosis = model.Diagnosis,
                    IsDeleted = false
                });
                db.SaveChanges();
            }
            return RedirectToAction("DoctorReview");
        }

        [Authorize]
        public ActionResult PatientReview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPatientReviewData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Patient patient = new Patient();
            using (var db = new MalaffiEntities())
            {
                patient = db.Patients.SingleOrDefault(d => d.UserId == id);
                List<Review> dbReviews = new List<Review>();
                dbReviews = db.Reviews.Where(r => r.IsDeleted == false && r.PatientId == patient.Id
                && (r.Doctor.FullName.ToString().Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<PatientReviewListModel> Reviews = new List<PatientReviewListModel>();
                dbReviews.ForEach(r => {
                    Reviews.Add(new PatientReviewListModel()
                    {
                        Id = r.Id,
                        DoctorName = r.Doctor.FullName,
                        ReviewDate = r.ReviewDate.ToString("dd-MM-yyyy hh:mm"),
                        Diagnosis = r.Diagnosis,
                    });
                });
                return Json(new
                {
                    aaData = Reviews.OrderByDescending(c => c.ReviewDate),
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = Reviews.Count,
                    iTotalRecords = Reviews.Count
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}