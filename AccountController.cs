using Malaffi.Models;
using Malaffi.Models.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Malaffi.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            List<SelectListItem> DoctorMedicaSpecializationTypes = new List<SelectListItem>();
            List<SelectListItem> LabTechnicianMedicaSpecializationTypes = new List<SelectListItem>();
            using (var db = new MalaffiEntities())
            {
                db.MedicaSpecializationTypes.ToList().ForEach(c =>
                {
                    if (c.Category == 1)
                    {
                        DoctorMedicaSpecializationTypes.Add(new SelectListItem() { Text = c.Description, Value = c.Id.ToString() });
                    }
                    if (c.Category == 2)
                    {
                        LabTechnicianMedicaSpecializationTypes.Add(new SelectListItem() { Text = c.Description, Value = c.Id.ToString() });
                    }
                });
                ViewBag.DoctorMedicaSpecializationTypes = new SelectList(DoctorMedicaSpecializationTypes, "Value", "Text"); ;
                ViewBag.LabTechnicianMedicaSpecializationTypes = new SelectList(LabTechnicianMedicaSpecializationTypes, "Value", "Text"); ;
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginAndRegisterModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Login.Email);
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Id));

            var result = await SignInManager.PasswordSignInAsync(model.Login.Email, model.Login.Password, model.Login.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    using (var db = new MalaffiEntities())
                    {
                        var RolesForUser = UserManager.GetRoles(user.Id);
                        if(RolesForUser[0] == "Doctor")
                        {

                            claims.Add(new Claim(ClaimTypes.Name, user.Id));
                        }
                        else if (RolesForUser[0] == "LabTechnician")
                        {
                            claims.Add(new Claim(ClaimTypes.Name, user.Id));
                        }
                        else if (RolesForUser[0] == "Patient")
                        {
                            claims.Add(new Claim(ClaimTypes.Name, user.Id));
                        }
                        else if (RolesForUser[0] == "Pharmacist")
                        {
                            claims.Add(new Claim(ClaimTypes.Name, user.Id));
                        }
                    }
                    var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(id);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.Login.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(LoginAndRegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Register.Email, Email = model.Register.Email };
            var result = await UserManager.CreateAsync(user, model.Register.Password);

            if (result.Succeeded)
            {
                using (var db = new MalaffiEntities())
                {
                    if (model.RegisterType == 2) //Doctor
                    {
                        db.Doctors.Add(new Doctor()
                        {
                            UserId = user.Id,
                            FullName = model.Register.FullName,
                            AssociationId = model.Register.AssociationId,
                            MedicaSpecializationId = model.Register.MedicaSpecialization
                        });
                        db.SaveChanges();
                        UserManager.AddToRole(user.Id, "Doctor");
                    }
                    else if (model.RegisterType == 3) //LabTechnician
                    {
                        db.LabTechnicians.Add(new LabTechnician()
                        {
                            UserId = user.Id,
                            FullName = model.Register.FullName,
                            CommercialRegistrationNo = model.Register.CommercialRegistrationNo,
                            MedicaSpecializationId = model.Register.MedicaSpecialization,
                            Address = model.Register.Address
                        });
                        db.SaveChanges();
                        UserManager.AddToRole(user.Id, "LabTechnician");
                    }
                    else if (model.RegisterType == 5) //Patient
                    {
                        db.Patients.Add(new Patient()
                        {
                            UserId = user.Id,
                            FullName = model.Register.FullName,
                            DBO = DateTime.Now,
                            Gender = model.GenderType == null ? 1 : model.GenderType.Value,
                            NationalNumber = model.Register.NationalNumber
                        });
                        db.SaveChanges();
                        UserManager.AddToRole(user.Id, "Patient");
                    }
                    else if (model.RegisterType == 4) //Pharmacist
                    {
                        db.Pharmacists.Add(new Pharmacist()
                        {
                            UserId = user.Id,
                            FullName = model.Register.FullName,
                            CommercialRegistrationNo = model.Register.CommercialRegistrationNo,
                            Address = model.Register.Address
                        });
                        db.SaveChanges();
                        UserManager.AddToRole(user.Id, "Pharmacist");
                    }
                }

                await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            AddErrors(result);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}