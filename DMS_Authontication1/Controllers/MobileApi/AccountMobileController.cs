//using DMS_Authontication1.Models;
//using Microsoft.AspNet.Identity.Owin;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http;
//using System.Web.Mvc;

//namespace DMS_Authontication1.Controllers.MobileApi
//{
//    public class AccountMobileController : Controller
//    {
//        #region Property
//        private ApplicationSignInManager _signInManager;
//        private ApplicationUserManager _userManager;
//        private ApplicationRoleManager _roleManager;
//        private ApplicationDbContext db;
//        private DMS_TESTEntities tESTEntities;

//        #endregion

//        #region Constractor
//        public AccountMobileController()
//        {
//            db = new ApplicationDbContext();
//            tESTEntities = new DMS_TESTEntities();
//        }

//        public AccountMobileController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
//        {
//            UserManager = userManager;
//            SignInManager = signInManager;
//            RoleManager = roleManager;
//        }

//        public ApplicationRoleManager RoleManager
//        {
//            get
//            {
//                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
//            }
//            private set
//            {
//                _roleManager = value;
//            }
//        }

//        public ApplicationSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
//            }
//            private set
//            {
//                _signInManager = value;
//            }
//        }

//        public ApplicationUserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            }
//            private set
//            {
//                _userManager = value;
//            }
//        }

//        #endregion

//        [HttpPost]
//        [Route("LogIn")]
//        public IHttpActionResult LogIn(LoginViewModel model)
//        {

//            try
//            {
//                string Message = "";
//                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
//                {
//                    Message = "Error, User name or password is empty";
//                    return BadRequest( Message);
//                }

//                // This doesn't count login failures towards account lockout
//                // To enable password failures to trigger account lockout, change to shouldLockout: true

//                var user = await UserManager.FindByNameAsync(model.UserName);
//                if (user != null)
//                {
//                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
//                    {
//                        ModelState.AddModelError("", "You need to confirm your email.");
//                        return View(model);
//                    }
//                    else
//                    {
//                        var _ERPRolesUsersPages = tESTEntities.ERPUsersModulesPages.Where(x => x.UserId == user.Id)
//                             .Join(tESTEntities.ERPModulesPages, rmp => rmp.PageId, mp => mp.Id, (rmp, mp) => new { rmp, mp })
//                             .Select(l => new ModulesPagesViewModel
//                             {
//                                 ModuleName = l.mp.ERPModule.Name,
//                                 PageName = l.mp.Name,
//                                 FullControl = l.rmp.FullControl,
//                                 Preview = l.rmp.Preview,
//                                 AddPermission = l.rmp.AddPermission,
//                                 EditPermission = l.rmp.EditPermission,
//                                 ActivationControl = l.rmp.ActivationControl,
//                                 PageId = l.rmp.PageId
//                             }).OrderBy(x => x.ModuleName).ToList();
//                        string seralize = JsonConvert.SerializeObject(_ERPRolesUsersPages);
//                        var claims = await UserManager.GetClaimsAsync(user.Id);
//                        if (claims.Count() != 0)
//                        {
//                            for (int i = 0; i < claims.Count; i++)
//                            {
//                                await UserManager.RemoveClaimAsync(user.Id, claims[i]);
//                            }
//                            //foreach (var item in claims)
//                            //{
//                            //    await UserManager.RemoveClaimAsync(user.Id, item);

//                            //}
//                        }
//                        //var claim = ((ClaimsIdentity)User.Identity);
//                        //if (claim != null)
//                        //{
//                        //    foreach (var item in claim.Claims)
//                        //    {
//                        //        await UserManager.RemoveClaimAsync(user.Id, item);

//                        //    }

//                        //}
//                        await UserManager.AddClaimAsync(user.Id, new Claim("SomeClaimType", seralize));
//                        var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

//                        switch (result)
//                        {
//                            case SignInStatus.Success:
//                                var userRole = user.Roles.Select(x => x.RoleId).FirstOrDefault();
//                                string role = RoleManager.Roles.Where(x => x.Id == userRole).FirstOrDefault().Name;

//                                switch (role)
//                                {
//                                    case "Pharmacy":
//                                        return Redirect("/Pharmacy/Pharmacy");
//                                    case "Lab":
//                                        return RedirectToLocal("/Labs/Lab");
//                                    case "Rays":
//                                        return RedirectToLocal("/Rays/Ray");
//                                    case "Doctor":
//                                        return RedirectToLocal("/DoctorApprovals");
//                                    case "Admin":
//                                        return RedirectToLocal("/ControlPanel/Main");
//                                    case "HR":
//                                        return RedirectToLocal("/EmployeeRequest/Index");
//                                    case "HR_Admin":
//                                        return RedirectToLocal("/Reports/Index");
//                                    case "Hospital":
//                                        return RedirectToLocal("/Hospital/Index");
//                                    case "AfterSale":
//                                        return RedirectToLocal("/AfterSales/Index");
//                                    case "User":
//                                        var usr = db.Users.Where(u => u.UserName == model.UserName).FirstOrDefault().Id;
//                                        //var usr = User.Identity.GetUserId();
//                                        var cardId = tESTEntities.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
//                                        var status = ChicActiveCard(cardId.Split('-')[0], cardId);
//                                        if (status.data != "Y" && status.message != "ok")
//                                        {
//                                            ModelState.AddModelError("", "You Can't login With This user As " + status.message);
//                                            return View(model);
//                                        }
//                                        if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
//                                            returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

//                                        if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
//                                        {
//                                            ViewBag.ReturnURL = returnUrl;
//                                            return RedirectToLocal(returnUrl);
//                                        }
//                                        return RedirectToLocal("/Employee/Index");

//                                }
//                                return RedirectToLocal(returnUrl);
//                            case SignInStatus.LockedOut:
//                                return View("Lockout");
//                            case SignInStatus.RequiresVerification:
//                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
//                            case SignInStatus.Failure:
//                            default:
//                                ModelState.AddModelError("", "Invalid login attempt.");
//                                return View(model);
//                        }
//                    }
//                }
//                else
//                {
//                    DateTime datenow = DateTime.Now.Date;
//                    var employee = tESTEntities.Comp_Employees.Where(x => x.CARD_ID == model.UserName && x.INS_START_DATE <= datenow
//                                && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
//                    if (employee != null)
//                    {
//                        var status = ChicActiveCard(employee.CARD_ID.Split('-')[0], employee.CARD_ID);
//                        if (status.data != "Y" && status.message != "ok")
//                        {
//                            ModelState.AddModelError("", "You Can't Register With This Card As " + status.message);
//                            return View(model);
//                        }
//                        var res = new ConfirmRegisterEmployeeVM
//                        {
//                            FullName = employee.EMP_ENAME,
//                            CardId = employee.CARD_ID,
//                            UserName = employee.CARD_ID,
//                        };
//                        return RedirectToAction("ConfirmRegister", "Employee", res);
//                    }
//                    else
//                    {
//                        var usercard = tESTEntities.UsersInternalCodes.Where(u => u.InternalCode == model.UserName).FirstOrDefault();
//                        if (usercard != null)
//                        {
//                            var status = ChicActiveCard(usercard.CardId.Split('-')[0], usercard.CardId);
//                            if (status.data != "Y" && status.message != "ok")
//                            {
//                                ModelState.AddModelError("", "You Can't Register With This Card As " + status.message);
//                                return View(model);
//                            }
//                            var res = new ConfirmRegisterEmployeeVM
//                            {
//                                FullName = usercard.EnglishName,
//                                CardId = usercard.CardId,
//                                UserName = usercard.InternalCode,
//                                Email = usercard.Email,

//                            };
//                            return RedirectToAction("ConfirmRegister", "Employee", res);
//                        }
//                        else
//                        {
//                            ModelState.AddModelError("", "Invalied UserName or Password.");
//                            return View(model);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var message = ex.Message;
//                if (ex.InnerException != null)
//                {
//                    message = message + " Error: " + ex.InnerException.Message;
//                }
//                return BadRequest(message);
//            }
//        }
//    }
//}
