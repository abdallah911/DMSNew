using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_TEST.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static DMS_Authontication1.Controllers.ManageController;

namespace DMS_Authontication1.Controllers
{
    [Authorize(Roles ="Admin")]
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class UserController : Controller
    {
        private DMS_TESTEntities tESTEntities;
        private ApplicationDbContext db ;
        private ApplicationUserManager _userManager;
        public UserController()
        {
            db = new ApplicationDbContext();
            tESTEntities = new DMS_TESTEntities();

        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserPermision(string UserId)
        {
            ViewBag.userName = db.Users.Where(x => x.Id == UserId).FirstOrDefault().UserName;
            return View();
        }
        public JsonResult PagesPermision(string UserId)
        {

            var _ERPRolesUsersPages = tESTEntities.ERPUsersModulesPages.Where(x => x.UserId == UserId)
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
            return new JsonResult { Data = _ERPRolesUsersPages, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }
        public JsonResult UpdateUserPagesPermision(ModulesPagesViewModel modulesPages)
        {

            ERPUsersModulesPage model = tESTEntities.ERPUsersModulesPages.Where(x => x.UserId == modulesPages.UserId && x.PageId == modulesPages.PageId).FirstOrDefault();
            if (model == null)
            {
                return new JsonResult { Data = "Page is not found", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            model.FullControl = modulesPages.FullControl;
            model.EditPermission = modulesPages.EditPermission;
            model.AddPermission = modulesPages.AddPermission;
            model.ActivationControl = modulesPages.ActivationControl;
            model.Preview = modulesPages.Preview;
            tESTEntities.Entry(model).State = EntityState.Modified;
            int result = tESTEntities.SaveChanges();
            return new JsonResult { Data = result == 1 ? "Updated Successfully" : "Updated falied", JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }
        // GET: ResetPassword
        public  ActionResult AdminResetPassword(string UserId)
        {
            ViewBag.UserId = UserId;
            if (string.IsNullOrEmpty(UserId))
            {
                ViewBag.Error = "You Should Select User";
                return View();
            }

            return View();
        }


        // POST: ResetPassword
        [HttpPost]
        public async Task<ActionResult> AdminResetPassword(AdminChangeUserPasswordNM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (string.IsNullOrEmpty(model.UserId))
            {
                return HttpNotFound();
            }
            UserManager.UserValidator= new UserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false

            };
            //var user = UserManager.FindById(model.UserId);
            //var password = UserManager.PasswordHasher.HashPassword(model.Password);
            //user.PasswordHash = password;

            //var result = UserManager.Update(user);
            var token = await UserManager.GeneratePasswordResetTokenAsync(model.UserId);
            var result= await UserManager.ResetPasswordAsync(model.UserId, token, model.Password);
            if (result.Succeeded)
            {
                return View("ChangeSuccess");
            }
            else
            {
                return View(model);
            }

        }


        public JsonResult UserList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            if (sSearch != null)
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.Users.OrderBy(m => m.UserName)
                    .Where(r => r.UserName.Contains(sSearch) || r.FName.Contains(sSearch) || r.Email.Contains(sSearch) || r.Type.Contains(sSearch)
                     || r.LName.Contains(sSearch) || r.Provider.Contains(sSearch))
                    .Select(l => new UserViewModel
                    {
                        FName = l.FName,
                        LName = l.LName,
                        Email= l.Email,
                        Provider= l.Provider,
                        Type = l.Type,
                        UserName = l.UserName,
                        Id =l.Id
                        
                    }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Users.Count(),
                    iTotalDisplayRecords = db.Users.Count()
                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.Users.OrderBy(m => m.UserName).AsEnumerable()
                    .Select(l => new UserViewModel
                    {
                        FName = l.FName,
                        LName = l.LName,
                        Email = l.Email,
                        Provider = l.Provider,
                        Type = l.Type,
                        UserName = l.UserName,
                        Id = l.Id

                    }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),
                    iTotalRecords = db.Users.Count(),
                    iTotalDisplayRecords = db.Users.Count()
                };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }


        }
        // GET: ResetPassword
        
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var User = db.Users.Find(id);
            if (User == null)
            {
                return HttpNotFound();
            }
            return View(User);
        }
        // POST: MedicineGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}