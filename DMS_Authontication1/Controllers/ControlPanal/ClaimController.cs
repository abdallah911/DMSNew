using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;

namespace DMS_Authontication1.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Pharmacy,Pharmacy_Admin,Lab,Lab_Admin,Rays,Rays_Admin")]
    public class ClaimController : Controller
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
            return RedirectToAction("Diagnoisis/" + viewmodel.Id);
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
            return View();
        }

        // GET: Specialities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Specialities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClaimPhotoesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.ClaimPhotoes.Where(c => c.ClaimNumber == model.ClaimNumber).FirstOrDefault() != null)
                {
                    ViewBag.error = "yes";
                    return View(model);
                }
                var entity = new ClaimPhoto();
                entity.ClaimNumber = model.ClaimNumber;
                entity.CardId = model.CardId;
                entity.IsDispense = false;
                entity.IsDespenseLab = false;
                entity.IsDespenseRay = false;
                entity.IsDespensePharm = false;
                entity.CreatedBy = User.Identity.Name;
                entity.CreatedDate = DateTime.Now;
                if (model.ImageFile != null)
                {
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    string fileName = "Claim " + model.ClaimNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    model.ImageFile.SaveAs(Server.MapPath("/Content/Claims/" + fileName /*ImageFile.FileName*/));
                    entity.Url = "~/Content/Claims/" + fileName;
                }
                db.ClaimPhotoes.Add(entity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.error = "yes";
            return View(model);
        }

        public JsonResult ClaimList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            if (sSearch != null)
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.ClaimPhotoes.OrderBy(m => m.Id)
                    .Where(r => r.CardId.Contains(sSearch))
                    .Select(l => new ClaimPhotoesViewModel
                    {
                        Id = l.Id,
                        CardId = l.CardId,
                        ClaimNumber = l.ClaimNumber,
                        IsDispense = l.IsDispense,
                        Url = l.Url,

                    }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.ClaimPhotoes.Count(),
                    iTotalDisplayRecords = db.ClaimPhotoes.Count()
                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.ClaimPhotoes.OrderBy(m => m.Id).AsEnumerable()
                   .Select(l => new ClaimPhotoesViewModel
                   {
                       Id = l.Id,
                       CardId = l.CardId,
                       ClaimNumber = l.ClaimNumber,
                       IsDispense = l.IsDispense,
                       Url = l.Url,
                   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),
                    iTotalRecords = db.ClaimPhotoes.Count(),
                    iTotalDisplayRecords = db.ClaimPhotoes.Count()
                };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }


        }

        [HttpPost]
        public JsonResult ClaimDelete(int id)
        {
            try
            {
                var claim = db.ClaimPhotoes.Find(id);
                db.ClaimPhotoes.Remove(claim);
                db.SaveChanges();
                return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Download(int id)
        {

            var fileName = db.ClaimPhotoes.Where(x => x.Id == id).Select(x => x.Url).FirstOrDefault();
            // Set the path to your image file
            var filePath =Server.MapPath(fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("Image not found");
            }

            // Read the file into a byte array
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Return the file as a response
            return File(fileBytes, "image/jpeg", fileName);
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
