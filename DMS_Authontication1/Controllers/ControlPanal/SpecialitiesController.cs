using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;

namespace DMS_Authontication1.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class SpecialitiesController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();

        #region Diagnoisis
        public ActionResult Diagnoisis(int id)
        {
            var viewmodel = new SpecilityDiagnoseViewmodel()
            {
                DiagnosisList = db.Diagnosis.Where(x => x.SPEC_ID == id).ToList()
            };
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult Diagnoisis(SpecilityDiagnoseViewmodel viewmodel)
        {
            if (!ModelState.IsValid)
            {

                viewmodel.DiagnosisList = db.Diagnosis.Where(x => x.SPEC_ID == viewmodel.Id).ToList();

                return View(viewmodel);
            }

            var model = new Diagnosi
            {
                SPEC_ID = viewmodel.Id,
                DIAG_CODE = DateTime.Now.ToString("ddMMyyHHmm"),
                DIAG_ANAME = viewmodel.DIAG_ANAME,
                DIAG_ENAME = viewmodel.DIAG_ENAME,
                ACTIVE = "1",
                CREATED_BY = User.Identity.Name,
                CREATED_DATE = DateTime.Now,
            };
            db.Diagnosis.Add(model);
            db.SaveChanges();
            return RedirectToAction("Diagnoisis/"+viewmodel.Id);
            //  var Isduplicated = db.MedicineTypes.Where(x => x.MedicineTypeName == viewmodel.MedicineTypeName).Select(x => x.MedicineTypeName).FirstOrDefault();
            //if (Isduplicated != viewmodel.MedicineTypeName)
            //{}
            //else
            //{
            //    ViewBag.Error = "done";
            //}
            //return View();
        }
        public JsonResult DiagnoiseActivation(int id)
        {
            Diagnosi _Diagnosi = db.Diagnosis.Find(id);
            if (_Diagnosi != null)
            {
                _Diagnosi.ACTIVE = _Diagnosi.ACTIVE == "1" ? "0" : "1";
                db.Entry(_Diagnosi).State = EntityState.Modified;
            }
            return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        // GET: Specialities
        public ActionResult Index()
        {
            return View(db.Specialities1.ToList());
        }

        // GET: Specialities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Specialities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SPEC_ID,SPEC_ANAME,SPEC_ENAME,special_notes,ACTIVE,NOTES,CREATED_DATE,CREATED_BY,UPDATE_BY,UPDATE_DATE,IsSync,SyncDate,SyncBy")] Specialities1 specialities1)
        {
            if (ModelState.IsValid)
            {
                specialities1.SPEC_ID = db.Specialities1.Max(x => x.SPEC_ID) + 1;
                specialities1.ACTIVE = "1";
                specialities1.CREATED_BY = User.Identity.Name;
                specialities1.CREATED_DATE = DateTime.Now;
                db.Specialities1.Add(specialities1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(specialities1);
        }


        // Post: Specialities/Activation/5
        public JsonResult Activation(int id)
        {
            Specialities1 _specility= db.Specialities1.Where(x=>x.SPEC_ID==id).FirstOrDefault();
            if (_specility != null)
            {
                _specility.ACTIVE=_specility.ACTIVE=="1"?"0":"1";
                db.Entry(_specility).State = EntityState.Modified;
            }
            return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
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
