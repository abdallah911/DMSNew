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
                 .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.ServiceCode == ServiceCode || r.ProviderName.Contains(sSearch) || r.CompId.Contains(sSearch) || r.ClassCode.Contains(sSearch) || r.CreatedBy.Contains(sSearch) : true)
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

            var ProviderUsers = db.Serv_Providers1.Where(x => x.PR_CODE == intSearch || x.PR_ENAME.Contains(search) || x.PR_ANAME.Contains(search))
                .Select(c => new
                {
                    id = c.PR_CODE,
                    text = c.PR_CODE + " || " + c.PR_ENAME
                }).ToList();
            return new JsonResult { Data = ProviderUsers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetProviderUsersName(string search, int page)
        {
            //int intSearch;
            //int.TryParse(search, out intSearch);

            var ProviderUsers = UserDB.Users.Where(r => r.UserName.Contains(search) || r.FName.Contains(search) || r.Email.Contains(search) || r.Type.Contains(search)
                    || r.LName.Contains(search) || r.Provider.Contains(search))
                .Select(c => new
                {
                    id = c.Id,
                    text = c.UserName + " || " + c.Email
                }).ToList();
            return new JsonResult { Data = ProviderUsers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetActiveEmployess(string search, int page)
        {
            long lgSearch;
            long.TryParse(search, out lgSearch);
            //db.fn_GetEmployessForCompany(Provider, maxcontract, "N", search)
            var Employees = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now && x.TERMINATE_FLAG == "N")
                /*.AsEnumerable()*/.Where(x => x.CARD_ID == search/* || x.EMP_ANAME.Contains(search) || x.EMP_ENAME.Contains(search)*/)
                .Select(c => new
                {
                    id = c.CARD_ID,
                    text = c.CARD_ID + " || " + c.EMP_ENAME
                }).ToList();

            return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetActiveCompanies(string search, int page)
        {
            long lgSearch;
            long.TryParse(search, out lgSearch);
            var Companies = db.Contract_Comp.Where(x => x.ACTIVE == "Y" && x.C_COMP_ID == lgSearch)
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
            ViewBag.Validation = true;
            return View();
        }

        // POST: ProviderServicesPermissions/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProviderName,ServiceCode,IsActive,CompId,ClassCode,CardId,IsDeleted,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] ProviderServicesPermissionsViewModal model)
        {
            if (model.UserId == null)
            {
                ProviderBlock providerBlock = new ProviderBlock
                {
                    Id = model.Id,
                    UserId = model.UserId,
                    CompId = int.Parse(model.CompId),
                    IsActive = model.IsActive,
                    ServiceCode = model.ServiceCode.ToString(),
                };
                db.ProviderBlocks.Add(providerBlock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Validation = true;
            if (ModelState.IsValid)
            {
                //List<ProviderServicesPermission> permission = new List<ProviderServicesPermission>();
                ProviderServicesPermission _permision = new ProviderServicesPermission();
                if (model.CardId != null && model.ClassCode == null)
                {
                    var permission = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == model.ServiceCode
                                    && x.CardId == model.CardId).ToList().OrderByDescending(x => x.Id);
                    _permision = permission.FirstOrDefault();
                }
                else if (model.CardId != null && model.ClassCode != null)
                {
                    var permission = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == model.ServiceCode
                                    && x.CardId == model.CardId && x.ClassCode == model.ClassCode).ToList().OrderByDescending(x => x.Id);
                    _permision = permission.FirstOrDefault();
                }
                else if (model.CompId != null && model.ClassCode == null)
                {
                    var permission = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == model.ServiceCode
                                    && (x.CompId == model.CompId || x.CompId == "ALL")).ToList().OrderByDescending(x => x.Id);
                    _permision = permission.FirstOrDefault();
                }
                else if (model.CompId != null && model.ClassCode != null)
                {
                    var permission = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == model.ServiceCode
                                    && (x.CompId == model.CompId || x.CompId == "ALL") && x.ClassCode == model.ClassCode).ToList().OrderByDescending(x => x.Id);
                    _permision = permission.FirstOrDefault();
                }
                if (_permision != null && _permision.IsActive == false)
                {
                    ViewBag.Validation = false;
                }
                if (ViewBag.Validation == false)
                {
                    if ((_permision.CreatedBy == "GodaKotb" || _permision.CreatedBy == "dr.ayman"
                             || _permision.CreatedBy == "ADMIN2" || _permision.CreatedBy == "dr.ahella"
                             || _permision.CreatedBy == "Aya" || _permision.CreatedBy == "Radwa"
                             || _permision.CreatedBy == "dr.nancy.mo" || _permision.CreatedBy == "m.abdeen"
                             || _permision.UpdatedBy == "GodaKotb" || _permision.UpdatedBy == "dr.ayman"
                             || _permision.UpdatedBy == "ADMIN2" || _permision.UpdatedBy == "dr.ahella"
                             || _permision.UpdatedBy == "Aya" || _permision.UpdatedBy == "Radwa"
                             || _permision.UpdatedBy == "dr.nancy.mo" || _permision.UpdatedBy == "m.abdeen")
                             && (User.Identity.Name != "GodaKotb" && User.Identity.Name != "dr.ayman" &&
                                User.Identity.Name != "dr.ahella" && User.Identity.Name != "Aya" &&
                                User.Identity.Name != "Radwa" && User.Identity.Name != "dr.nancy.mo" &&
                                User.Identity.Name != "ADMIN2" && User.Identity.Name != "M.abdeen"))
                    {
                        ViewBag.Validation = false;
                        return View();
                    }
                }

                ProviderServicesPermission providerServicesPermission = new ProviderServicesPermission
                {
                    Id = model.Id,
                    CardId = model.CardId,
                    ClassCode = model.ClassCode,
                    CompId = model.CompId,
                    IsActive = model.IsActive,
                    IsDeleted = model.IsDeleted,
                    ProviderName = model.ProviderName,
                    ServiceCode = model.ServiceCode,
                    UpdatedBy = model.UpdatedBy,
                    UpdatedDate = model.UpdatedDate
                };
                providerServicesPermission.CreatedBy = User.Identity.Name;
                providerServicesPermission.CreatedDate = DateTime.Now;

                db.ProviderServicesPermissions.Add(providerServicesPermission);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
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
            if ((providerServicesPermission.CreatedBy == "GodaKotb" || providerServicesPermission.CreatedBy == "dr.ayman"
                || providerServicesPermission.CreatedBy == "ADMIN2" || providerServicesPermission.CreatedBy == "dr.ahella"
                || providerServicesPermission.CreatedBy == "Aya" || providerServicesPermission.CreatedBy == "Radwa"
                || providerServicesPermission.CreatedBy == "dr.nancy.mo" || providerServicesPermission.CreatedBy == "m.abdeen"
                || providerServicesPermission.UpdatedBy == "GodaKotb" || providerServicesPermission.UpdatedBy == "dr.ayman"
                || providerServicesPermission.UpdatedBy == "ADMIN2" || providerServicesPermission.UpdatedBy == "dr.ahella"
                || providerServicesPermission.UpdatedBy == "Aya" || providerServicesPermission.UpdatedBy == "Radwa"
                || providerServicesPermission.UpdatedBy == "dr.nancy.mo" || providerServicesPermission.UpdatedBy == "m.abdeen")
                && (User.Identity.Name != "GodaKotb" && User.Identity.Name != "dr.ayman" &&
                    User.Identity.Name != "dr.ahella" && User.Identity.Name != "Aya" &&
                    User.Identity.Name != "Radwa" && User.Identity.Name != "dr.nancy.mo" &&
                    User.Identity.Name != "ADMIN2" && User.Identity.Name != "M.abdeen"))
            {
                //ViewBag.IsAuthenticate = false;
                return View();
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
