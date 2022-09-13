using Malaffi.Models;
using Malaffi.Models.Data;
using Malaffi.Models.Test;
using Malaffi.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Malaffi.Controllers
{
    public class TestsController : Controller
    {
        [Authorize]
        public ActionResult DoctorTests()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDoctorTestData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Doctor doctor = new Doctor();
            using (var db = new MalaffiEntities())
            {
                doctor = db.Doctors.SingleOrDefault(d => d.UserId == id);
                List<Test> dbTests = new List<Test>();
                dbTests = db.Tests.Where(t => t.IsDeleted == false && t.DoctorId == doctor.Id
                && (t.PatientId.ToString().Contains(param.sSearch)
                || t.Patient.NationalNumber.Contains(param.sSearch)
                || t.Patient.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<DoctorTestListModel> Tests = new List<DoctorTestListModel>();
                dbTests.ForEach(r => {
                    Tests.Add(new DoctorTestListModel()
                    {
                        Id = r.Id,
                        Description = r.Description,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        LabTechnicianName = r.LabTechnicianId == null ? "-" : r.LabTechnician.FullName,
                        StatusString = ((TestStatusType)r.Status).ToString(),
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm"),
                        FileUrl = r.Status == 4 ? GetAttachmentURLByReferanceId(AttachmentTypes.TestFile, r.Id.ToString()) : ""
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TestModel model)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Doctor doctor = new Doctor();
            using (var db = new MalaffiEntities())
            {
                doctor = db.Doctors.SingleOrDefault(d => d.UserId == id);
                db.Tests.Add(new Test()
                {
                    Description = model.Description,
                    CreationDate = DateTime.Now,
                    PatientId = model.PatientId,
                    DoctorId = doctor.Id,
                    IsActive = true,
                    IsDeleted = false,
                    Status = 1,
                    IsDownloaded = false,
                });
                db.SaveChanges();
            }
            return RedirectToAction("DoctorTests");
        }

        [Authorize]
        public ActionResult PatientTests()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPatientTestData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Patient patient = new Patient();
            using (var db = new MalaffiEntities())
            {
                patient = db.Patients.SingleOrDefault(p => p.UserId == id);
                List<Test> dbTests = new List<Test>();
                dbTests = db.Tests.Where(t => t.IsDeleted == false && t.PatientId == patient.Id
                && (t.Doctor.FullName.Contains(param.sSearch)
                || t.LabTechnician.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<PatientTestListModel> Tests = new List<PatientTestListModel>();
                dbTests.ForEach(r => {
                    Tests.Add(new PatientTestListModel()
                    {
                        Id = r.Id,
                        Description = r.Description,
                        DoctorName = r.Doctor.FullName,
                        LabTechnicianName = r.LabTechnicianId == null ? "-" : r.LabTechnician.FullName,
                        StatusString = ((TestStatusType)r.Status).ToString(),
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm"),
                        FileUrl = r.Status == 4 ? GetAttachmentURLByReferanceId(AttachmentTypes.TestFile, r.Id.ToString()) : ""
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
        public ActionResult LabTechnicianTests()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetLabTechnicianData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            LabTechnician labTechnician = new LabTechnician();
            using (var db = new MalaffiEntities())
            {
                List<Test> dbTests = new List<Test>();
                dbTests = db.Tests.Where(t => (t.Status == 1 || t.Status == 2) && t.LabTechnicianId == null && t.IsDeleted == false
                && (t.PatientId.ToString().Contains(param.sSearch)
                || t.Patient.NationalNumber.Contains(param.sSearch)
                || t.Patient.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<LabTechnicianTestListModel> Tests = new List<LabTechnicianTestListModel>();
                dbTests.ForEach(r => {
                    Tests.Add(new LabTechnicianTestListModel()
                    {
                        Id = r.Id,
                        Description = r.Description,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        DoctorName = r.Doctor.FullName,
                        StatusString = ((TestStatusType)r.Status).ToString(),
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm")
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
        public ActionResult LabTechnicianMyTests()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetLabTechnicianMyData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            LabTechnician labTechnician = new LabTechnician();
            using (var db = new MalaffiEntities())
            {
                labTechnician = db.LabTechnicians.SingleOrDefault(l => l.UserId == id);
                List<Test> dbTests = new List<Test>();
                dbTests = db.Tests.Where(t => t.LabTechnicianId == labTechnician.Id && t.Status == 3
                && (t.PatientId.ToString().Contains(param.sSearch)
                || t.Patient.NationalNumber.Contains(param.sSearch)
                || t.Patient.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<LabTechnicianTestListModel> Tests = new List<LabTechnicianTestListModel>();
                dbTests.ForEach(r => {
                    Tests.Add(new LabTechnicianTestListModel()
                    {
                        Id = r.Id,
                        Description = r.Description,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        DoctorName = r.Doctor.FullName,
                        StatusString = ((TestStatusType)r.Status).ToString(),
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm")
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
        public ActionResult LabTechnicianTestsHistory()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetLabTechnicianHistoryData(DataTablesParam param)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            LabTechnician labTechnician = new LabTechnician();
            using (var db = new MalaffiEntities())
            {
                labTechnician = db.LabTechnicians.SingleOrDefault(l => l.UserId == id);
                List<Test> dbTests = new List<Test>();
                dbTests = db.Tests.Where(t => t.LabTechnicianId == labTechnician.Id && t.Status == 4
                && (t.PatientId.ToString().Contains(param.sSearch)
                || t.Patient.NationalNumber.Contains(param.sSearch)
                || t.Patient.FullName.Contains(param.sSearch)
                || param.sSearch == null)).ToList();
                List<LabTechnicianTestListModel> Tests = new List<LabTechnicianTestListModel>();
                dbTests.ForEach(r => {
                    Tests.Add(new LabTechnicianTestListModel()
                    {
                        Id = r.Id,
                        Description = r.Description,
                        PatientId = r.PatientId,
                        PatientName = r.Patient.FullName,
                        DoctorName = r.Doctor.FullName,
                        StatusString = ((TestStatusType)r.Status).ToString(),
                        CreationDate = r.CreationDate.ToString("dd-MM-yyyy hh:mm")
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

        public ActionResult Upload(int id)
        {
            UploadTestModel model = new UploadTestModel();
            model.Id = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(UploadTestModel model)
        {
            HttpPostedFileBase file = Request.Files[0];
            AttachmentTypes attachmentType = AttachmentTypes.TestFile;
            if (file != null && attachmentType > 0)
            {
                var fileUrl = GenerateAbsoluteUrl(attachmentType, "{0}");
                AttachmentManager.InsertAttachment(file.FileName, file.InputStream, file.ContentType, attachmentType, fileUrl, model.Id.ToString(), file.ContentLength);
                using (var db = new MalaffiEntities())
                {
                    var test = db.Tests.SingleOrDefault(t => t.Id == model.Id);
                    test.Status = 4;
                    db.SaveChanges();
                }
                return RedirectToAction("LabTechnicianMyTests");
            }
            return RedirectToAction("Upload");
        }

        public string GenerateAbsoluteUrl(AttachmentTypes attachmentType, string fileName)
        {
            return VirtualPathUtility.ToAbsolute("~/Documents/" + attachmentType + "/" + fileName);
        }

        private string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            string newUrl = serverUrl;
            //Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            //newUrl = (forceHttps ? "https" : originalUri.Scheme) +
            //    "://" + originalUri.Authority + newUrl;
            return newUrl;
        }

        public string GetAttachmentURLByReferanceId(AttachmentTypes attachmentType, string referanceId)
        {
            var res = AttachmentManager.GetAttachmentListURLByReferanceId(attachmentType, referanceId);
            return res.Count > 0 ? res[0] : null;
        }

        [Authorize]
        public ActionResult Activate(int id)
        {
            using (var db = new MalaffiEntities())
            {
                var test = db.Tests.SingleOrDefault(t => t.Id == id);
                test.Status = 1;
                db.SaveChanges();
            }
            return RedirectToAction("DoctorTests");
        }

        [Authorize]
        public ActionResult Deactivate(int id)
        {
            using (var db = new MalaffiEntities())
            {
                var test = db.Tests.SingleOrDefault(t => t.Id == id);
                test.Status = 2;
                db.SaveChanges();
            }
            return RedirectToAction("DoctorTests");
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            using (var db = new MalaffiEntities())
            {
                var test = db.Tests.SingleOrDefault(t => t.Id == id);
                test.IsDeleted = true;
                db.SaveChanges();
            }
            return RedirectToAction("DoctorTests");
        }

        [Authorize]
        public ActionResult Assign(int id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var labTechnicianId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            LabTechnician labTechnician = new LabTechnician();
            using (var db = new MalaffiEntities())
            {
                labTechnician = db.LabTechnicians.SingleOrDefault(l => l.UserId == labTechnicianId);
                var test = db.Tests.SingleOrDefault(t => t.Id == id);
                test.LabTechnicianId = labTechnician.Id;
                test.Status = 3;
                db.SaveChanges();
            }
            return RedirectToAction("LabTechnicianTests");
        }
    }
}