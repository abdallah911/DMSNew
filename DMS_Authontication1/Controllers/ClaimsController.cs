using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Globalization;
using System.Threading;
using DMS_Authontication1.ViewModel.HR;


namespace DMS_Authontication1.Controllers
{
    public class ClaimsController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();
        ApplicationDbContext myEntities = new ApplicationDbContext();
        DBApproval dbData = new DBApproval();
        DBData dbData2 = new DBData();

        [HttpGet]
        public ActionResult UploadClaimPhoto(string id = "")
        {
            // You can return a view or JSON, depending on what you want
            ViewBag.stat = "Ok";
            if (id != "")
            {
                Int64 ClaimId = Int64.Parse(id);

                     var data = (from h in db.HospitalClaims
                                 join sp in db.Specialities1 on h.SpecialistId equals sp.SPEC_ID
                                 where h.IdPrimary == ClaimId
                                 select new HospitalClaimPhotoesViewModel
                                 {
                                     CardId = h.CARD_ID,
                                     ClaimNumber = h.IdPrimary,
                                     Speciality = sp.SPEC_ANAME
                                 }).FirstOrDefault();
                return View(data);
            }
            ViewBag.stat = "NoData";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HospitalClaimPhotoesViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.ClaimPhotoes.Where(c => c.ClaimNumber == model.ClaimNumber).FirstOrDefault() != null)
                    {
                        ViewBag.error = "yes";
                        return View(model);
                    }
                    
                    var entity = new ClaimPhoto();
                    
                    entity.ClaimNumber = model.ClaimNumber;
                    entity.CardId = model.CardId;
                    entity.Speciality = model.Speciality;
                    entity.IsDispense = false;
                    entity.IsDespenseLab = false;
                    entity.IsDespenseRay = false;
                    entity.IsDespensePharm = false;
                    entity.IsDeleted = false;
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
                    //return RedirectToAction("UploadClaimPhoto");

                    return RedirectToAction("UploadClaimPhoto", new { id = model.ClaimNumber });

                }
                catch (Exception ex)
                {

                    ViewBag.error = "yes";
                    return View(model);
                }

            }
            ViewBag.error = "yes";
            return View(model);
        }
        [HttpPost]
        public JsonResult getData(Int64 claimId)
        {
            var result = (from h in db.HospitalClaims
                          join sp in db.Specialities1 on h.SpecialistId equals sp.SPEC_ID
                          where h.IdPrimary == claimId
                          select new
                          {
                              CardId = h.CARD_ID,
                              ClaimNumber = h.IdPrimary,
                              Speciality = sp.SPEC_ANAME
                          }).FirstOrDefault();

            if (result == null)
            {
                return Json(new { success = false, message = "لا توجد بيانات" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadClaimPhoto(HospitalClaimPhotoesViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.ClaimPhotoes.Where(c => c.ClaimNumber == model.ClaimNumber).FirstOrDefault() != null)
                    {
                        ViewBag.error = "Claim added befor for this card";
                        return View(model);
                    }

                    var entity = new ClaimPhoto();
                    entity.ClaimNumber = model.ClaimNumber;
                    entity.CardId = model.CardId;
                    entity.Speciality = model.Speciality;
                    entity.IsDispense = false;
                    entity.IsDespenseLab = false;
                    entity.IsDespenseRay = false;
                    entity.IsDespensePharm = false;
                    entity.IsDeleted = false;
                    entity.CreatedBy = User.Identity.Name;
                    entity.CreatedDate = DateTime.Now;
                    if (model.ImageFile != null)
                    {
                        string folderPath = Server.MapPath("~/Content/Claims/");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string extension = Path.GetExtension(model.ImageFile.FileName);
                        string fileName = "Claim " + model.ClaimNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        model.ImageFile.SaveAs(Server.MapPath("/Content/Claims/" + fileName /*ImageFile.FileName*/));
                        entity.Url = "~/Content/Claims/" + fileName;
                    }
                  
                    db.ClaimPhotoes.Add(entity);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.error = "Saved successfully";
                    return View();
                   // return RedirectToAction("UploadClaimPhoto", "Claims");

                }
                catch (Exception ex)
                {

                    ViewBag.error = "There was a problem saving.";
                    return View(model);
                }

            }
            ViewBag.error = "Please Complete the data";         
            return View(model);
        }
    }
}