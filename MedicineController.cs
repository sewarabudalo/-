using Malaffi.Models;
using Malaffi.Models.Data;
using Malaffi.Models.MedicineRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Malaffi.Controllers
{
    public class MedicineController : Controller
    {
        [Authorize]
        public ActionResult DoctorMedicines()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDoctorMedicineData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Doctor doctor = new Doctor();
            using (var db = new MalaffiEntities())
            {
                doctor = db.Doctors.SingleOrDefault(d => d.UserId == id);
                List<MedicineRequest> dbMedicineRequests = new List<MedicineRequest>();
                dbMedicineRequests = db.MedicineRequests.Where(t => t.DoctorId == doctor.Id && t.IsDeleted == false
                && (t.PatientId.ToString().Contains(param.sSearch)
                || t.Patient.NationalNumber.Contains(param.sSearch)
                || t.Patient.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<DoctorMedicineRequestModel> MedicineRequests = new List<DoctorMedicineRequestModel>();
                dbMedicineRequests.ForEach(r => {
                    MedicineRequests.Add(new DoctorMedicineRequestModel()
                    {
                        Id = r.Id,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        PharmacistName = r.PharmacistId == null ? "-" : r.Pharmacist.FullName,
                        MedicineName = r.Medicine.Name,
                        TotalQuantity = r.TotalQuantity,
                        PerDayQuantity = r.PerDayQuantity,
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm"),
                        StatusString = r.Status == 1 ? "Active" : "Assigned"
                    });
                });
                return Json(new
                {
                    aaData = MedicineRequests,
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = MedicineRequests.Count,
                    iTotalRecords = MedicineRequests.Count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (var db = new MalaffiEntities())
            {
                db.Medicines.ToList().ForEach(m => {
                    list.Add(new SelectListItem()
                    {
                        Text = m.Name,
                        Value = m.Id.ToString()
                    });
                });

                ViewBag.MedicinesList = new SelectList(list, "Value", "Text"); ;

                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(MedicineRequestModel model)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Doctor doctor = new Doctor();
            using (var db = new MalaffiEntities())
            {
                doctor = db.Doctors.SingleOrDefault(d => d.UserId == id);
                db.MedicineRequests.Add(new MedicineRequest()
                {
                    PatientId = model.PatientId,
                    DoctorId = doctor.Id,
                    TotalQuantity = model.TotalQuantity,
                    PerDayQuantity = model.PerDayQuantity,
                    MedicineId = model.MedicineId,
                    IsDeleted = false,
                    CreationDate = DateTime.Now,
                    Status = 1
                });
                db.SaveChanges();
            }
            return RedirectToAction("DoctorMedicines");
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            using (var db = new MalaffiEntities())
            {
                var medicineRequests = db.MedicineRequests.SingleOrDefault(t => t.Id == id);
                medicineRequests.IsDeleted = true;
                db.SaveChanges();
            }
            return RedirectToAction("DoctorMedicines");
        }

        [Authorize]
        public ActionResult PatientMedicines()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPatientMedicineData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Patient patient = new Patient();
            using (var db = new MalaffiEntities())
            {
                patient = db.Patients.SingleOrDefault(p => p.UserId == id);
                List<MedicineRequest> dbMedicineRequests = new List<MedicineRequest>();
                dbMedicineRequests = db.MedicineRequests.Where(t => t.IsDeleted == false && t.PatientId == patient.Id
                && (t.Doctor.FullName.Contains(param.sSearch)
                || t.Medicine.Name.Contains(param.sSearch)
                || t.Pharmacist.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<PatientMedicineRequestModel> MedicineRequests = new List<PatientMedicineRequestModel>();
                dbMedicineRequests.ForEach(r => {
                    MedicineRequests.Add(new PatientMedicineRequestModel()
                    {
                        Id = r.Id,
                        DoctorName = r.Doctor.FullName,
                        TotalQuantity = r.TotalQuantity,
                        PerDayQuantity = r.PerDayQuantity,
                        MedicineName = r.Medicine.Name,
                        PharmacistName = r.PharmacistId == null ? "-" : r.Pharmacist.FullName,
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm")
                    });
                });
                return Json(new
                {
                    aaData = MedicineRequests,
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = MedicineRequests.Count,
                    iTotalRecords = MedicineRequests.Count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult PharmacistMedicines()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPharmacistMedicineData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Pharmacist pharmacist = new Pharmacist();
            using (var db = new MalaffiEntities())
            {
                pharmacist = db.Pharmacists.SingleOrDefault(p => p.UserId == id);
                List<MedicineRequest> dbMedicineRequest = new List<MedicineRequest>();
                dbMedicineRequest = db.MedicineRequests.Where(t => t.PharmacistId == null && t.IsDeleted == false
                && (t.PatientId.ToString().Contains(param.sSearch)
                || t.Patient.NationalNumber.Contains(param.sSearch)
                || t.Patient.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<PharmacistMedicinesRequestModel> Tests = new List<PharmacistMedicinesRequestModel>();
                dbMedicineRequest.ForEach(r => {
                    Tests.Add(new PharmacistMedicinesRequestModel()
                    {
                        Id = r.Id,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        DoctorName = r.Doctor.FullName,
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm"),
                        TotalQuantity = r.TotalQuantity,
                        PerDayQuantity = r.PerDayQuantity,
                        MedicineName = r.Medicine.Name,
                        StatusString = r.Status == 1 ? "Active" : "Assigned"
                    });
                });
                return Json(new
                {
                    aaData = Tests,
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = Tests.Count,
                    iTotalRecords = Tests.Count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult PharmacistMedicinesHistory()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPharmacistMedicineHistoryData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Pharmacist pharmacist = new Pharmacist();
            using (var db = new MalaffiEntities())
            {
                pharmacist = db.Pharmacists.SingleOrDefault(p => p.UserId == id);
                List<MedicineRequest> dbMedicineRequest = new List<MedicineRequest>();
                dbMedicineRequest = db.MedicineRequests.Where(t => t.PharmacistId == pharmacist.Id && t.IsDeleted == false
               && (t.PatientId.ToString().Contains(param.sSearch)
                || t.Patient.NationalNumber.Contains(param.sSearch)
                || t.Patient.FullName.Contains(param.sSearch)
                || t.Doctor.FullName.Contains(param.sSearch)
                || t.Medicine.Name.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<PharmacistMedicinesRequestModel> Tests = new List<PharmacistMedicinesRequestModel>();
                dbMedicineRequest.ForEach(r => {
                    Tests.Add(new PharmacistMedicinesRequestModel()
                    {
                        Id = r.Id,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        DoctorName = r.Doctor.FullName,
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm"),
                        TotalQuantity = r.TotalQuantity,
                        PerDayQuantity = r.PerDayQuantity,
                        MedicineName = r.Medicine.Name
                    });
                });
                return Json(new
                {
                    aaData = Tests,
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = Tests.Count,
                    iTotalRecords = Tests.Count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult Assign(int id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var pharmacistId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Pharmacist pharmacist = new Pharmacist();
            using (var db = new MalaffiEntities())
            {
                pharmacist = db.Pharmacists.SingleOrDefault(l => l.UserId == pharmacistId);
                var medicineRequests = db.MedicineRequests.SingleOrDefault(t => t.Id == id);
                medicineRequests.PharmacistId = pharmacist.Id;
                medicineRequests.Status = 2;
                db.SaveChanges();
            }
            return RedirectToAction("PharmacistMedicines");
        }
    }
}