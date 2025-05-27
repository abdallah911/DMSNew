using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DMS_Authontication1.Models;
using System.Collections.Generic;
using DMS_TEST.ViewModel;
using Newtonsoft.Json;
using DMS_Authontication1.ViewModel.EmployeeVM;

namespace DMS_Authontication1.Controllers
{
    // [Authorize]
    public class AccountController : Controller
    {
        #region Property
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db;
        private DMS_TESTEntities tESTEntities;

        #endregion

        #region Constractor
        public AccountController()
        {
            db = new ApplicationDbContext();
            tESTEntities = new DMS_TESTEntities();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
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

        #endregion

        #region Actions

        //
        // GET: /Account/Login
        //[AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            
            if (user != null)
                return RedirectToAction("Main", "ControlPanel");
            
            var dayy = DateTime.Now.Date;

            //var evnt = tESTEntities.CompanyEvents.Where(e => DateTime.Now >= e.DateFrom && DateTime.Now <= e.DateTo
            //         && e.Active == true).ToList();
            var evnt = tESTEntities.CompanyEvents
                                   .Where(e => System.Data.Entity.DbFunctions.TruncateTime(e.DateFrom) <= dayy
                                             && System.Data.Entity.DbFunctions.TruncateTime(e.DateTo) >= dayy
                                             && e.Active == true)
                                   .ToList();


            if (evnt.Count > 0)
                ViewBag.evnt = 1;
            else
                ViewBag.evnt = 0;

            ViewBag.ReturnUrl = returnUrl;

            return View();

        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    ModelState.AddModelError("", "You need to confirm your email.");
                    return View(model);
                }
                else
                {
                    //var servProviderNew = tESTEntities.SERV_PROVIDERS_NEW.Where(x => x.MAIN_CODE == user.TypeId).FirstOrDefault();
                    //if (servProviderNew != null)
                    //{
                    //    if (servProviderNew.TERMINATE_FLAG == "Y")
                    //    {
                    //        ModelState.AddModelError("", "You'r account locked.");
                    //        return View(model);
                    //    }
                    //}
                    var _ERPRolesUsersPages = tESTEntities.ERPUsersModulesPages.Where(x => x.UserId == user.Id)
                         .Join(tESTEntities.ERPModulesPages, rmp => rmp.PageId, mp => mp.Id, (rmp, mp) => new { rmp, mp })
                         .Select(l => new ModulesPagesViewModel
                         {
                             ModuleName = l.mp.ERPModule.Name,
                             PageName = l.mp.Name,
                             FullControl = l.rmp.FullControl,
                             Preview = l.rmp.Preview,
                             AddPermission = l.rmp.AddPermission,
                             EditPermission = l.rmp.EditPermission,
                             ActivationControl = l.rmp.ActivationControl,
                             PageId = l.rmp.PageId
                         }).OrderBy(x => x.ModuleName).ToList();
                    string seralize = JsonConvert.SerializeObject(_ERPRolesUsersPages);
                    var claims = await UserManager.GetClaimsAsync(user.Id);
                    if (claims.Count() != 0)
                    {
                        for (int i = 0; i < claims.Count; i++)
                        {
                            await UserManager.RemoveClaimAsync(user.Id, claims[i]);
                        }
                        //foreach (var item in claims)
                        //{
                        //    await UserManager.RemoveClaimAsync(user.Id, item);

                        //}
                    }
                    //var claim = ((ClaimsIdentity)User.Identity);
                    //if (claim != null)
                    //{
                    //    foreach (var item in claim.Claims)
                    //    {
                    //        await UserManager.RemoveClaimAsync(user.Id, item);

                    //    }

                    //}
                    await UserManager.AddClaimAsync(user.Id, new Claim("SomeClaimType", seralize));
                    var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

                    switch (result)
                    {
                        case SignInStatus.Success:
                            var userRole = user.Roles.Select(x => x.RoleId).FirstOrDefault();
                            string role = RoleManager.Roles.Where(x => x.Id == userRole).FirstOrDefault().Name;

                            switch (role)
                            {
                                case "Pharmacy":
                                    return Redirect("/Pharmacy/Pharmacy");
                                case "Lab":
                                    return RedirectToLocal("/Labs/Lab");
                                case "Rays":
                                    return RedirectToLocal("/Rays/Ray");
                                case "Doctor":
                                    return RedirectToLocal("/DoctorApprovals");
                                case "Admin":                                   
                                    return RedirectToLocal("/ControlPanel/Main");
                                case "AdminHelth":
                                    return RedirectToLocal("/ControlPanel/Main");
                                case "HR":
                                    return RedirectToLocal("/EmployeeRequest/Index");
                                //return RedirectToLocal("/Dashboard/Dashboard");
                                case "HR_Admin":
                                    if (model.UserName.ToLower() == "dr.mohsen" || model.UserName.ToLower() == "it.admin" || model.UserName.ToLower() == "super-admin" || model.UserName.ToLower() == "matarat5-admin")  
                                    {
                                        //return RedirectToLocal("/Reports/Index");
                                        return RedirectToLocal("/Dashboard/Index");
                                    }
                                    return RedirectToLocal("/Reports/Index");
                                case "Hospital":
                                    return RedirectToLocal("/Hospital/Index");
                                case "AfterSale":
                                    return RedirectToLocal("/AfterSales/Index");
                                case "User":
                                    var usr = db.Users.Where(u => u.UserName == model.UserName).FirstOrDefault().Id;
                                    //var usr = User.Identity.GetUserId();
                                    var cardId = tESTEntities.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                                    var status = ChicActiveCard(cardId.Split('-')[0], cardId);
                                    if (status.data != "Y" && status.message != "ok")
                                    {
                                        ModelState.AddModelError("", "You Can't login With This user As " + status.message);
                                        return View(model);
                                    }
                                    if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                                        returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

                                    if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
                                    {
                                        ViewBag.ReturnURL = returnUrl;
                                        return RedirectToLocal(returnUrl);
                                    }
                                    return RedirectToLocal("/Employee/Index");

                            }
                            return RedirectToLocal(returnUrl);
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
            }
            else
            {
                DateTime datenow = DateTime.Now.Date;
                var employee = tESTEntities.Comp_Employees.Where(x => x.CARD_ID == model.UserName && x.INS_START_DATE <= datenow
                            && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                if (employee != null)
                {
                    var status = ChicActiveCard(employee.CARD_ID.Split('-')[0], employee.CARD_ID);
                    if (status.data != "Y" && status.message != "ok")
                    {
                        ModelState.AddModelError("", "You Can't Register With This Card As " + status.message);
                        return View(model);
                    }
                    var res = new ConfirmRegisterEmployeeVM
                    {
                        FullName = employee.EMP_ENAME,
                        CardId = employee.CARD_ID,
                        UserName = employee.CARD_ID,
                    };
                    return RedirectToAction("ConfirmRegister", "Employee", res);
                }
                else
                {
                    var usercard = tESTEntities.UsersInternalCodes.Where(u => u.InternalCode == model.UserName).FirstOrDefault();
                    if (usercard != null)
                    {
                        var status = ChicActiveCard(usercard.CardId.Split('-')[0], usercard.CardId);
                        if (status.data != "Y" && status.message != "ok")
                        {
                            ModelState.AddModelError("", "You Can't Register With This Card As " + status.message);
                            return View(model);
                        }
                        var res = new ConfirmRegisterEmployeeVM
                        {
                            FullName = usercard.EnglishName,
                            CardId = usercard.CardId,
                            UserName = usercard.InternalCode,
                            Email = usercard.Email,

                        };
                        return RedirectToAction("ConfirmRegister", "Employee", res);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalied UserName or Password.");
                        return View(model);
                    }
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> LoginMobileAsync(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                var result2 = new { Message = "Error, User name or password is empty", Code = 0 };
                return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    var result2 = new { Message = "Error,You need to confirm your email.", Code = 0 };
                    return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {

                    var claims = await UserManager.GetClaimsAsync(user.Id);
                    if (claims.Count() != 0)
                    {
                        for (int i = 0; i < claims.Count; i++)
                        {
                            await UserManager.RemoveClaimAsync(user.Id, claims[i]);
                        }
                    }
                    var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

                    switch (result)
                    {
                        case SignInStatus.Success:
                            var userRole = user.Roles.Select(x => x.RoleId).FirstOrDefault();
                            string role = RoleManager.Roles.Where(x => x.Id == userRole).FirstOrDefault().Name;

                            var usr = db.Users.Where(u => u.UserName == model.UserName).FirstOrDefault().Id;
                            //var usr = User.Identity.GetUserId();
                            var cardId = tESTEntities.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                            var status = ChicActiveCard(cardId.Split('-')[0], cardId);
                            if (status.data != "Y" && status.message != "ok")
                            {
                                var result2 = new { Message = "You Can't login With This user As " + status.message, Code = 0 };
                                return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                            }
                            var result4 = new { Message = "Ok", Code = 1 };
                            return new JsonResult { Data = result4, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                        case SignInStatus.Failure:
                        default:
                            var result5 = new { Message = "Invalid login attempt.", Code = 0 };
                            return new JsonResult { Data = result5, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
            }
            else
            {
                DateTime datenow = DateTime.Now.Date;
                var employee = tESTEntities.Comp_Employees.Where(x => x.CARD_ID == model.UserName && x.INS_START_DATE <= datenow
                            && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                if (employee != null)
                {
                    var status = ChicActiveCard(employee.CARD_ID.Split('-')[0], employee.CARD_ID);
                    if (status.data != "Y" && status.message != "ok")
                    {
                        var result2 = new { Message = "You Can't Register With This Card As " + status.message, Code = 0 };
                        return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    var res = new ConfirmRegisterEmployeeVM
                    {
                        FullName = employee.EMP_ENAME,
                        CardId = employee.CARD_ID,
                        UserName = employee.CARD_ID,
                    };
                    var result4 = new { Message = "Ok", Code = 2 };
                    return new JsonResult { Data = result4, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var usercard = tESTEntities.UsersInternalCodes.Where(u => u.InternalCode == model.UserName).FirstOrDefault();
                    if (usercard != null)
                    {
                        var status = ChicActiveCard(usercard.CardId.Split('-')[0], usercard.CardId);
                        if (status.data != "Y" && status.message != "ok")
                        {
                            var result2 = new { Message = "You Can't Register With This Card As " + status.message, Code = 0 };
                            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                        var res = new ConfirmRegisterEmployeeVM
                        {
                            FullName = usercard.EnglishName,
                            CardId = usercard.CardId,
                            UserName = usercard.InternalCode,
                            Email = usercard.Email,

                        };
                        var result4 = new { Message = "Ok", Code = 2 };
                        return new JsonResult { Data = result4, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        var result2 = new { Message = "Invalied UserName or Password.", Code = 0 };
                        return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
            }

            //var result3 = new { Message = "Ok", Code = 1 };
            //return new JsonResult { Data = result3, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult UserProfileInfo()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var userResult = new
            {
                UserName = user.UserName,
                FirstName = user.FName,
                LastName = user.LName,
                Address = user.Address,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Role = "User",
            };
            var result = new { Message = "Ok", Code = 1, Data = userResult };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Account/Register

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Register()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles)
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            var provider = tESTEntities.Serv_Providers1.Where(x => x.PRV_TYPE == 2).ToList();
            SelectList Providerlist = new SelectList(provider, "PR_CODE", "PR_ENAME");
            ViewBag.provider = Providerlist;

            var address = db.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_ENAME", "BS_ENAME");
            ViewBag.address = addresslist;



            return View();
        }

        ////// POST: /Account/Register
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles)
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            var provider = tESTEntities.Serv_Providers1.Where(x => x.PRV_TYPE == 2).ToList();
            SelectList Providerlist = new SelectList(provider, "PR_CODE", "PR_ENAME");
            ViewBag.provider = Providerlist;

            var address = db.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_ENAME", "BS_ENAME");
            ViewBag.address = addresslist;

            model.Address = model.Address + "/" + model.state;
            //model.Address += f 
            if (ModelState.IsValid)
            {
                string typeid;
                if (model.Type == "5")//Doctor
                {
                    var PreTypeId = Convert.ToInt32(db.Users.Where(s => s.Type == "Doctor").Max(x => x.TypeId));
                    typeid = Convert.ToString(PreTypeId + 1);
                    model.Type = "Doctor";
                    model.Provider = "NULL";

                }
                else if (model.Type == "10")//HR Admin
                {
                    var PreTypeId = Convert.ToInt32(db.Users.Where(s => s.Type == "HR_Admin").Max(x => x.TypeId));
                    typeid = Convert.ToString(PreTypeId + 1);
                    model.Type = "HR_Admin";
                    model.Provider = "0";

                }
                else
                {
                    int ProviderCode = Convert.ToInt32(model.Provider);
                    var provider1 = tESTEntities.Serv_Providers1.Where(x => x.PR_CODE == ProviderCode).FirstOrDefault();
                    //typeid = provider.FirstOrDefault().PR_CODE.ToString() + "-" + Convert.ToString(Convert.ToInt32(db.AspNetUsers.Max(x => x.TypeId)) + 1);
                    int max = 0;
                    var counter = db.Users.Where(m => m.Provider == model.Provider).ToList();
                    if (model.Type == "1")
                    {
                        model.Type = "Hospital";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }
                    else if (model.Type == "2")
                    {
                        model.Type = "Pharmacy";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }
                    else if (model.Type == "3")
                    {
                        model.Type = "Labs";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }
                    else if (model.Type == "4")
                    {
                        model.Type = "Rays";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }
                    else if (model.Type == "6")
                    {
                        model.Type = "Physical Therapy";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }
                    else if (model.Type == "7")
                    {
                        model.Type = "Dental Clinic";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }
                    else if (model.Type == "9")//hr
                    {
                        model.Type = "HR";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }

                    else if (model.Type == "8")//After Sale
                    {
                        model.Type = "AfterSale";
                        string c;

                        foreach (var item in counter)
                        {
                            string x;
                            if (item.TypeId != null)
                            {
                                x = item.TypeId.ToString();
                                if (x.Contains('-'))
                                {
                                    c = item.TypeId.Split('-')[1];
                                }
                                else
                                {
                                    c = item.TypeId;
                                }
                                if (Convert.ToInt32(c) >= max)
                                {
                                    max = Convert.ToInt32(c);
                                }
                            }
                        }

                    }
                    if (model.Type == "HR" || model.Type == "AfterSale")
                    {
                        typeid = model.Provider + "-" + Convert.ToString(/*Convert.ToInt32(PreTypeId[1])*/ max + 1);

                    }
                    else
                    {
                        typeid = provider1.PR_CODE.ToString() + "-" + Convert.ToString(/*Convert.ToInt32(PreTypeId[1])*/ max + 1);
                    }
                }
                var user = new ApplicationUser
                {
                    TypeId = typeid,
                    UserName = model.UserName,
                    Email = model.Email,
                    FName = model.FName,
                    LName = model.LName,
                    Type = model.RoleName,
                    Provider = model.Provider,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumder1 = model.PhoneNumber1
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, model.RoleName);
                    string roleId = RoleManager.FindByName(model.RoleName).Id;
                    //User Permission
                    foreach (ERPRolesModulesPage page in tESTEntities.ERPRolesModulesPages.Where(x => x.RoleId == roleId))
                    {
                        ERPUsersModulesPage Newmodel = new ERPUsersModulesPage();
                        Newmodel.PageId = page.PageId;
                        Newmodel.UserId = user.Id;
                        Newmodel.CreatedBy = User.Identity.Name;
                        Newmodel.CreatedDate = DateTime.Now;
                        Newmodel.FullControl = page.FullControl;
                        Newmodel.EditPermission = page.EditPermission;
                        Newmodel.AddPermission = page.AddPermission;
                        Newmodel.ActivationControl = page.ActivationControl;
                        Newmodel.Preview = page.Preview;
                        tESTEntities.ERPUsersModulesPages.Add(Newmodel);
                    }
                    tESTEntities.SaveChanges();
                    //

                    // await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    //Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking here : " + callbackUrl);
                    if (model.AddCompanies != null)
                    {
                        if (model.AddCompanies[0] == "0")
                        {
                            model.AddCompanies[0] = "All";
                        }
                        foreach (var item in model.AddCompanies)
                        {
                            HrAdminCompany hrAdminCompany = new HrAdminCompany
                            {
                                UserId = user.Id,
                                CompId = item
                            };
                            tESTEntities.HrAdminCompanies.Add(hrAdminCompany);
                        }
                        tESTEntities.SaveChanges();

                    }
                    if (model.Type == "Doctor")
                    {
                        ApplicationUser aspNetUser = db.Users.Where(m => m.UserName == model.UserName).FirstOrDefault();
                        return RedirectToAction("Create", "DoctorPersonalDatas", new { id = aspNetUser.Id });// db.AspNetUsers.Find().Id.FirstOrDefault().Id });
                    }
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public JsonResult GetStateList(string id)
        {

            db.Configuration.ProxyCreationEnabled = false;

            var Number = db.BASIC_DATA.Where(m => m.BS_ENAME == id).Select(l => l.BS_CODE).FirstOrDefault();
            var State = db.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == Number.ToString()).ToList();
            SelectList StateListlist = new SelectList(State, "BS_ENAME", "BS_ANAME");
            ViewBag.State = StateListlist;

            return Json(StateListlist, JsonRequestBehavior.AllowGet);

        }

        #region Password rest and confirm

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
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

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public async Task<ActionResult> LogOff()
        {
            var claim = ((ClaimsIdentity)User.Identity);
            var userid = User.Identity.GetUserId();
            if (claim != null && userid != null)
            {
                foreach (var item in claim.Claims)
                {
                    await UserManager.RemoveClaimAsync(userid, item);

                }

            }
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");

        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
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

        #endregion

        #region Helpers

        public JsonResult Provider(string id)
        {
            int Code = Convert.ToInt32(id);
            var provider = tESTEntities.Serv_Providers1.Where(x => x.PRV_TYPE == Code).ToList();
            SelectList Providerlist = new SelectList(provider, "PR_CODE", "PR_ENAME");
            //  return new JsonResult { Data = Providerlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return Json(Providerlist);
        }

        public JsonResult ContractCompanies()
        {
            // int Code = Convert.ToInt32(id);
            var provider = tESTEntities.Contract_Comp.Where(x => x.ACTIVE == "Y").ToList();
            SelectList Providerlist = new SelectList(provider, "C_COMP_ID", "C_ANAME");
            //  return new JsonResult { Data = Providerlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return Json(Providerlist);
        }


        public JsonResult getCompines()
        {
            var CompaniesDb = tESTEntities.Contract_Comp
                .Select(l => new
                {
                    Code = l.C_COMP_ID,
                    Name = l.C_ENAME + " || " + l.C_COMP_ID

                })
            .ToList();
            return Json(CompaniesDb, JsonRequestBehavior.AllowGet);
        }

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

        public ReturnResult ChicActiveCard(string compid, string CardId)
        {
            int CompId = Convert.ToInt32(compid);
            try
            {
                var emp = tESTEntities.Contract_Comp.Where(x => x.C_COMP_ID == CompId).FirstOrDefault().ACTIVE;
                DateTime datenow = DateTime.Now.Date;
                var da = new DateTime(datenow.Year, datenow.Month, datenow.Day);
                var empCardTerminationFlag = tESTEntities.Comp_Employees.Where(x => x.CARD_ID == CardId && x.INS_START_DATE <= da && x.INS_END_DATE >= da).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                if (empCardTerminationFlag != null)
                {
                    if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "N")
                    {
                        return (new ReturnResult { data = "Y", message = "ok" });
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE > da)
                    {
                        return (new ReturnResult { data = "Y", message = "ok" });
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE < da)
                    {
                        return (new ReturnResult { data = "N", message = "Expired Card" });
                    }
                }
                else
                {
                    var CompTerminationFlag = tESTEntities.Contract_Data.Where(x => x.C_COMP_ID == CompId && x.DATE_FROM <= da && x.DATE_TO >= da).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (CompTerminationFlag != null)
                    {
                        return (new ReturnResult { data = "N", message = "Card is not existed" });

                    }
                }
                return (new ReturnResult { data = "N", message = "Expired card" });
            }
            catch (Exception ex)
            {
                return (new ReturnResult { data = "EX", message = ex.Message });
            }
        }
        public class ReturnResult
        {
            public string data { get; set; }
            public string message { get; set; }
        }

        #endregion
    }
}