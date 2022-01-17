using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using DMS_TEST.ViewModel;

namespace DMS_Authontication1.Controllers.ControlPanal
{
    [Authorize(Roles = "Admin")]
    public class ProviderServicesPermissionsController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationDbContext UserDB = new ApplicationDbContext();

        // GET: ProviderServicesPermissions
        public ActionResult Index()
        {
            //return View(db.ProviderServicesPermissions.Where(x=>x.IsDeleted==false).ToList().OrderByDescending(x=>x.Id));
            return View();
        }
        // GET: Medicines
        public JsonResult ProviderServicesPermissionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            int ServiceCode;
            int.TryParse(sSearch, out ServiceCode);
            var result = new
            {
                sEcho = sEcho,
                aaData = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false)
                 .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.ServiceCode== ServiceCode || r.ProviderName.Contains(sSearch) || r.CompId.Contains(sSearch) || r.ClassCode.Contains(sSearch) || r.CreatedBy.Contains(sSearch) : true)
                .Select(l => new ProviderServicesPermissionsViewModal
                {
                    Id = l.Id,
                    ProviderName = l.ProviderName,
                    ServiceCode = l.ServiceCode,
                    IsActive = l.IsActive,
                    CompId = l.CompId,
                    ClassCode = l.ClassCode,
                    CardId = l.CardId,
                    CreatedBy = l.CreatedBy
                }).OrderByDescending(m => m.Id).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false)
                 .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.ServiceCode == ServiceCode || r.ProviderName.Contains(sSearch) || r.CompId.Contains(sSearch) || r.ClassCode.Contains(sSearch) || r.CreatedBy.Contains(sSearch) : true)
                 .Count(),
                iTotalDisplayRecords = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false)
                 .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.ServiceCode == ServiceCode || r.ProviderName.Contains(sSearch) || r.CompId.Contains(sSearch) || r.ClassCode.Contains(sSearch) || r.CreatedBy.Contains(sSearch) : true)
               .Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
         


        }

        public JsonResult GetProviderUsers(string search, int page)
        {
            int intSearch;
            int.TryParse(search, out intSearch);

            var ProviderUsers =db.Serv_Providers1.Where(x => x.PR_CODE== intSearch || x.PR_ENAME.Contains(search) || x.PR_ANAME.Contains(search))
                .Select(c => new
                {
                    id = c.PR_CODE,
                    text = c.PR_CODE + " || " + c.PR_ENAME
                }).ToList();
            return new JsonResult { Data = ProviderUsers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetActiveEmployess(string search, int page)
        {
            long lgSearch;
            long.TryParse(search, out lgSearch);
           //db.fn_GetEmployessForCompany(Provider, maxcontract, "N", search)
            var Employees =db.Comp_Employees.Where(x =>x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now && x.TERMINATE_FLAG == "N")
                /*.AsEnumerable()*/.Where(x => x.CARD_ID==search/* || x.EMP_ANAME.Contains(search) || x.EMP_ENAME.Contains(search)*/)
                .Select(c => new
                {
                    id = c.CARD_ID,
                    text = c.CARD_ID +" || "+c.EMP_ENAME
                }).ToList();

            return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetActiveCompanies(string search, int page)
        {
            long lgSearch;
            long.TryParse(search, out lgSearch);
            var Companies =db.Contract_Comp.Where(x => x.ACTIVE == "Y" && x.C_COMP_ID== lgSearch)
                .Select(c => new
                {
                    id = c.C_COMP_ID,
                    text = c.C_COMP_ID + " || " + c.C_ENAME
                }).ToList();
            return new JsonResult { Data = Companies, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        // GET: ProviderServicesPermissions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProviderServicesPermissions/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProviderName,ServiceCode,IsActive,CompId,ClassCode,CardId,IsDeleted,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] ProviderServicesPermission providerServicesPermission)
        {
            if (ModelState.IsValid)
            {
                providerServicesPermission.CreatedBy = User.Identity.Name;
                providerServicesPermission.CreatedDate = DateTime.Now;

                db.ProviderServicesPermissions.Add(providerServicesPermission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(providerServicesPermission);
        }

        // GET: ProviderServicesPermissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderServicesPermission providerServicesPermission = db.ProviderServicesPermissions.Find(id);
            if (providerServicesPermission == null)
            {
                return HttpNotFound();
            }
            return View(providerServicesPermission);
        }

        // POST: ProviderServicesPermissions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProviderName,ServiceCode,IsActive,CompId,ClassCode,CardId,IsDeleted")] ProviderServicesPermission providerServicesPermission)
        {
            if (ModelState.IsValid)
            {
                ProviderServicesPermission _providerServicesPermission = db.ProviderServicesPermissions.Find(providerServicesPermission.Id);
                //_providerServicesPermission.ProviderName = providerServicesPermission.ProviderName;
                //_providerServicesPermission.ServiceCode = providerServicesPermission.ServiceCode;
                //_providerServicesPermission.CompId = providerServicesPermission.CompId;
                //_providerServicesPermission.ClassCode = providerServicesPermission.ClassCode;
                //_providerServicesPermission.CardId = providerServicesPermission.CardId;
                _providerServicesPermission.IsActive = providerServicesPermission.IsActive;
                _providerServicesPermission.IsDeleted = providerServicesPermission.IsDeleted;
                _providerServicesPermission.UpdatedBy = User.Identity.Name;
                _providerServicesPermission.UpdatedDate = DateTime.Now;
                db.Entry(_providerServicesPermission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(providerServicesPermission);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
