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

        public bool IsRealImage(HttpPostedFileBase file)
        {
            try
            {
                using (var img = System.Drawing.Image.FromStream(file.InputStream))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadClaimPhoto(HospitalClaimPhotoesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.error = "Please complete the data.";
                ViewBag.error = "من فضلك ارفع الصورة";
                return View(model);
            }

            try
            {
                var existingEntity = db.ClaimPhotoes.FirstOrDefault(c => c.ClaimNumber == model.ClaimNumber);
                string folderPath = @"C:\Domains\DMS_Providers\Content\Claims\";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = null;
                string fullPath = null;

                if (model.ImageFile != null)
                {
                    if (!IsRealImage(model.ImageFile))
                    {
                        //ViewBag.error = "Invalid image format.";
                        ViewBag.error = "يوجد مشكلة في الصورة";
                        return View(model);
                    }

                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    fileName = "Claim_" + model.ClaimNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    fullPath = Path.Combine(folderPath, fileName);

                    model.ImageFile.SaveAs(fullPath);
                }

                if (existingEntity != null)
                {
                   
                    //if (model.ImageFile != null && !string.IsNullOrEmpty(existingEntity.Url))
                    //{
                    //    string oldImagePath = Path.Combine(folderPath, Path.GetFileName(existingEntity.Url.Replace("~", "")));
                    //    if (System.IO.File.Exists(oldImagePath))
                    //    {
                    //        System.IO.File.Delete(oldImagePath);
                    //    }
                    //}

                    existingEntity.CardId = model.CardId;
                    existingEntity.Speciality = model.Speciality;
                    existingEntity.IsDispense = false;
                    existingEntity.IsDespenseLab = false;
                    existingEntity.IsDespenseRay = false;
                    existingEntity.IsDespensePharm = false;
                    existingEntity.IsDeleted = false;
                    existingEntity.CreatedBy = User.Identity.Name;
                    existingEntity.CreatedDate = DateTime.Now;

                    if (fileName != null)
                        existingEntity.Url = "~/Content/Claims/" + fileName;

                    db.SaveChanges();
                    ViewBag.error = "تم التعديل بنجاح يمكنك الان الحصول على الخدمة لدى كافة مقدمي الخدمة";
                    return View();
                }
                else
                {
                    var newEntity = new ClaimPhoto
                    {
                        ClaimNumber = model.ClaimNumber,
                        CardId = model.CardId,
                        Speciality = model.Speciality,
                        IsDispense = false,
                        IsDespenseLab = false,
                        IsDespenseRay = false,
                        IsDespensePharm = false,
                        IsDeleted = false,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        Url = fileName != null ? "~/Content/Claims/" + fileName : null
                    };

                    db.ClaimPhotoes.Add(newEntity);
                    db.SaveChanges();
                    //ViewBag.error = "Saved successfully.";
                    ViewBag.error = "تم الحفظ بنجاح يمكنك الان الحصول على الخدمة لدى كافة مقدمي الخدمة";

                    return View();
                }
            }
            catch (Exception ex)
            {
                //ViewBag.error = "There was a problem saving. " + ex.Message;
                ViewBag.error = @"حدثت مشكلة اثناء الحفظ \n" + ex.Message;

                return View(model);
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UploadClaimPhoto(HospitalClaimPhotoesViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (db.ClaimPhotoes.Where(c => c.ClaimNumber == model.ClaimNumber).FirstOrDefault() != null)
        //            {
        //                ViewBag.error = "Claim added befor for this card";
        //                return View(model);
        //            }

        //            var entity = new ClaimPhoto();
        //            entity.ClaimNumber = model.ClaimNumber;
        //            entity.CardId = model.CardId;
        //            entity.Speciality = model.Speciality;
        //            entity.IsDispense = false;
        //            entity.IsDespenseLab = false;
        //            entity.IsDespenseRay = false;
        //            entity.IsDespensePharm = false;
        //            entity.IsDeleted = false;
        //            entity.CreatedBy = User.Identity.Name;
        //            entity.CreatedDate = DateTime.Now;
        //            if (model.ImageFile != null)
        //            {
        //                //string folderPath = Server.MapPath("~/Content/Claims/");
        //                //string folderPath = Server.MapPath(@"C:\Domains\DMS_Providers\Content\Claims\");
        //                string folderPath = @"C:\Domains\DMS_Providers\Content\Claims\";
        //                if (!Directory.Exists(folderPath))
        //                {
        //                    Directory.CreateDirectory(folderPath);
        //                }

        //                string extension = Path.GetExtension(model.ImageFile.FileName);
        //                string fileName = "Claim " + model.ClaimNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;                        
        //                string fullPath = Path.Combine(folderPath, fileName);
        //                model.ImageFile.SaveAs(fullPath);
        //                entity.Url = "~/Content/Claims/" + fileName;

        //                //model.ImageFile.SaveAs(Server.MapPath(@"C:\Domains\DMS_Providers\Content\Claims\" + fileName /*ImageFile.FileName*/));
        //                //entity.Url = @"C:\Domains\DMS_Providers\Content\Claims\" + fileName;
        //                //entity.Url = fullPath;
        //            }

        //            db.ClaimPhotoes.Add(entity);
        //            db.SaveChanges();
        //            //return RedirectToAction("Index");
        //            ViewBag.error = "Saved successfully";
        //            return View();
        //           // return RedirectToAction("UploadClaimPhoto", "Claims");

        //        }
        //        catch (Exception ex)
        //        {

        //            ViewBag.error = "There was a problem saving.";
        //            return View(model);
        //        }

        //    }
        //    ViewBag.error = "Please Complete the data";         
        //    return View(model);
        //}
        public ActionResult UploadFamilyPhoto()
        {
            return View();
        }
        public ActionResult ChronicDelivery(string cardId, string Name)
        {
            ViewBag.cardId = cardId;
            ViewBag.Name = Name;

            var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.DELIVERY == 1 && p.TERMINATE_FLAG != "Y")
                    .Select(
                          s => new
                          {
                              Code = s.PR_CODE,
                              Name = s.PR_ENAME
                          }).ToList();
            SelectList providers = new SelectList(providerList, "Code", "Name");
            ViewBag.provider = providers;
            return View();
        }
    }
}