using DMS_Authontication1.Models;
using DMS_TEST.ViewModel;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private DMS_TESTEntities db;

        public RoleController()
        {
            db = new DMS_TESTEntities();

        }

        public RoleController(ApplicationRoleManager roleManager)
        {
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

        // GET: Role
        public ActionResult Index()
        {
            List<RoleViewModel> list = new List<RoleViewModel>();
            foreach (var role in RoleManager.Roles)
                list.Add(new RoleViewModel(role));
            return View(list);
        }

        //Get:Create
        public ActionResult Create()
        {
            return View();
        }
        //Post:Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel model)
        {
            var role = new ApplicationRole() { Name = model.Name };
            await RoleManager.CreateAsync(role);
            foreach (ERPModulesPage page in db.ERPModulesPages)
            {
                ERPRolesModulesPage Newmodel = new ERPRolesModulesPage();
                Newmodel.PageId = page.Id;
                Newmodel.RoleId = role.Id;
                Newmodel.CreatedBy = User.Identity.Name;
                Newmodel.CreatedDate = DateTime.Now;
                Newmodel.FullControl = false;
                Newmodel.EditPermission = false;
                Newmodel.AddPermission = false;
                Newmodel.ActivationControl = false;
                Newmodel.Preview = false;
                db.ERPRolesModulesPages.Add(Newmodel);
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult RolePermision(string RoleId)
        {
            ViewBag.roleName = RoleManager.Roles.Where(x => x.Id == RoleId).FirstOrDefault().Name;
            return View();
        }
        public JsonResult PagesPermision(string RoleId)
        {

            var _ERPRolesModulesPages = db.ERPRolesModulesPages.Where(x => x.RoleId == RoleId)
            .Join(db.ERPModulesPages, rmp => rmp.PageId, mp => mp.Id, (rmp, mp) => new { rmp, mp })
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
            return new JsonResult { Data = _ERPRolesModulesPages, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }
        public JsonResult UpdateRolePagesPermision(ModulesPagesViewModel modulesPages)
        {

            ERPRolesModulesPage model = db.ERPRolesModulesPages.Where(x => x.RoleId == modulesPages.RoleId && x.PageId == modulesPages.PageId).FirstOrDefault();
            if (model == null)
            {
                return new JsonResult { Data = "Page is not found", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            model.FullControl = modulesPages.FullControl;
            model.EditPermission = modulesPages.EditPermission;
            model.AddPermission = modulesPages.AddPermission;
            model.ActivationControl = modulesPages.ActivationControl;
            model.Preview = modulesPages.Preview;
            db.Entry(model).State = EntityState.Modified;
            int result = db.SaveChanges();
            return new JsonResult { Data = result == 1 ? "Updated Successfully" : "Updated falied", JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }
        //Get:Edit
        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            return View(new RoleViewModel(role));
        }
        //Post:Edit
        [HttpPost]
        public async Task<ActionResult> Edit(RoleViewModel model)
        {
            var role = new ApplicationRole() { Id = model.Id, Name = model.Name };
            if (RoleManager.Roles.Where(x => x.Name == role.Name).Count() == 0)
            {
                await RoleManager.UpdateAsync(role);
            }
            return RedirectToAction("Index");
        }

        //Get:Details
        public async Task<ActionResult> Details(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            return View(new RoleViewModel(role));
        }
        //Post:Details
        [HttpPost]
        public async Task<ActionResult> Details(RoleViewModel model)
        {
            var role = new ApplicationRole() { Id = model.Id, Name = model.Name };
            await RoleManager.UpdateAsync(role);

            return RedirectToAction("Index");
        }
        //Get:Delete
        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            return View(new RoleViewModel(role));
        }
        //Post:Delete
        [HttpPost, ActionName("Delete")]

        public async Task<ActionResult> DeleteConformied(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            await RoleManager.DeleteAsync(role);

            return RedirectToAction("Index");
        }

    }
}