using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using System.IO;
using DMS_Authontication1.ViewModel;

namespace DMS_Authontication1.Controllers
{
    public class DoctorPersonalDatasController : Controller
    {
        ApplicationDbContext db;
        public DoctorPersonalDatasController()
        {
             db = new ApplicationDbContext();

        }

        // GET: DoctorPersonalDatas
        public ActionResult Index()
        {
            // var doctorPersonalDatas;= db.DoctorPersonalDatas.Include(d => d.AspNetUser);
            var doctorPersonalDatas= db.DoctorPersonalData.Include(d => d.User);
            return View(doctorPersonalDatas.ToList());
        }

        // GET: DoctorPersonalDatas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorPersonalData doctorPersonalData = db.DoctorPersonalData.Find(id);
            if (doctorPersonalData == null)
            {
                return HttpNotFound();
            }
            return View(doctorPersonalData);
        }
        DMS_TESTEntities dms = new DMS_TESTEntities();
        // GET: DoctorPersonalDatas/Create
        public ActionResult Create(string id)
        {
            ApplicationUser aspNetUser = db.Users.Find(id);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName",aspNetUser.Id);
            ViewBag.TypeId = aspNetUser.TypeId;
           
            ViewBag.Speciality = new SelectList(dms.Specialities1, "SPEC_ENAME", "SPEC_ENAME");
           // ViewBag.CurrentUser = aspNetUser.UserName;
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,StampImageFile,TypeId,WorkType,Speciality,Image,ImageFile,StampImage,BirthData,Religious,Gender,Nationality,SocialStatus,KidsNumbers,MalitaryStatus,DataOfEndMalitary,IdType,IdNumber,IdStartData,IdExpiredData,IdPlace,BloodType")] DoctorPersonalDataViewModel doctorPersonalData, HttpPostedFileBase ImageFile, HttpPostedFileBase StampImageFile)
        {
            ViewBag.Speciality = new SelectList(dms.Specialities1, "SPEC_ENAME", "SPEC_ENAME");
            if (ModelState.IsValid)
            {
                // at Doctor personal  public HttpPostedFileBase ImageFile { get; set; }
                string filename="";
                string stampfilename="";
                //personal image
                if (doctorPersonalData.ImageFile != null)
                {
                     filename = Path.GetFileNameWithoutExtension(doctorPersonalData.ImageFile.FileName);
                    string extension = Path.GetExtension(doctorPersonalData.ImageFile.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    doctorPersonalData.Image = "~/Content/DoctorImages/" + filename;
                    filename = Path.Combine(Server.MapPath("~/Content/DoctorImages/"), filename);
                    doctorPersonalData.ImageFile.SaveAs(filename);
                }
                //stamp image
                if (doctorPersonalData.StampImageFile != null)
                {
                     stampfilename = Path.GetFileNameWithoutExtension(doctorPersonalData.StampImageFile.FileName);
                    string stampextension = Path.GetExtension(doctorPersonalData.StampImageFile.FileName);
                    stampfilename = stampfilename + DateTime.Now.ToString("yymmssfff") + stampextension;
                    doctorPersonalData.StampImage = "~/Content/DoctorStampImages/" + stampfilename;
                    stampfilename = Path.Combine(Server.MapPath("~/Content/DoctorStampImages/"), stampfilename);
                    doctorPersonalData.StampImageFile.SaveAs(stampfilename);
                }
                //save
                //doctorPersonalData.UserId = db.AspNetUsers.FirstOrDefault().Where(a => a.UserName == doctorPersonalData.UserId);
                DoctorPersonalData Data = new DoctorPersonalData()
                {
                    Image = doctorPersonalData.Image,
                    StampImage = doctorPersonalData.StampImage,
                    UserId=doctorPersonalData.UserId,
                    TypeId=doctorPersonalData.TypeId,
                    WorkType=doctorPersonalData.WorkType,
                    Speciality=doctorPersonalData.Speciality,
                    BirthData=doctorPersonalData.BirthData,
                    Religious=doctorPersonalData.Religious,
                    Gender=doctorPersonalData.Gender,
                    Nationality=doctorPersonalData.Nationality,
                    SocialStatus=doctorPersonalData.SocialStatus,
                    KidsNumbers=doctorPersonalData.KidsNumbers,
                    MalitaryStatus=doctorPersonalData.MalitaryStatus,
                    DataOfEndMalitary=doctorPersonalData.DataOfEndMalitary,
                    IdType=doctorPersonalData.IdType,
                    IdNumber=doctorPersonalData.IdNumber,
                    IdStartData=doctorPersonalData.IdStartData,
                    IdExpiredData=doctorPersonalData.IdExpiredData,
                    IdPlace=doctorPersonalData.IdPlace,
                    BloodType=doctorPersonalData.BloodType
                };
                db.DoctorPersonalData.Add(Data);

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

                return RedirectToAction("CreateWorkPlace", "DoctorWorkPlaces", new { id = doctorPersonalData.UserId });
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", doctorPersonalData.UserId);
            ApplicationUser aspNetUser = db.Users.Find(doctorPersonalData.UserId);
            ViewBag.TypeId = aspNetUser.TypeId;
            ViewBag.Speciality = new SelectList(dms.Specialities1, "SPEC_ENAME", "SPEC_ENAME");
            return View(doctorPersonalData);
        }

        // GET: DoctorPersonalDatas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorPersonalData doctorPersonalData = db.DoctorPersonalData.Find(id);
            if (doctorPersonalData == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FName", doctorPersonalData.UserId);
            return View(doctorPersonalData);
        }

        // POST: DoctorPersonalDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,TypeId,WorkType,Speciality,Image,StampImage,BirthData,Religious,Gender,Nationality,SocialStatus,KidsNumbers,MalitaryStatus,DataOfEndMalitary,IdType,IdNumber,IdStartData,IdExpiredData,IdPlace,BloodType")] DoctorPersonalData doctorPersonalData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctorPersonalData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FName", doctorPersonalData.UserId);
            return View(doctorPersonalData);
        }

        // GET: DoctorPersonalDatas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorPersonalData doctorPersonalData = db.DoctorPersonalData.Find(id);
            if (doctorPersonalData == null)
            {
                return HttpNotFound();
            }
            return View(doctorPersonalData);
        }

        // POST: DoctorPersonalDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DoctorPersonalData doctorPersonalData = db.DoctorPersonalData.Find(id);
            db.DoctorPersonalData.Remove(doctorPersonalData);
            db.SaveChanges();
            return RedirectToAction("Index");
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
