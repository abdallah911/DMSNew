using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using Newtonsoft.Json;

namespace DMS_Authontication1.Controllers
{
    public class DoctorWorkPlacesController : Controller
    {
        private DMS_TESTEntities db1;
        private ApplicationDbContext db = new ApplicationDbContext();

        public DoctorWorkPlacesController()
        {
            db1 = new DMS_TESTEntities();
            db = new ApplicationDbContext();

        }
        public ActionResult CreateWorkPlace(string id)
        {

            ApplicationUser aspNetUser = db.Users.Find(id);
            //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName",aspNetUser.Id);
            ViewBag.CurrentUserId = id;
            ViewBag.CurrentUser = aspNetUser.UserName;
            ViewBag.TypeId = aspNetUser.TypeId;
            // ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName");
            //C_COMP_ID
            ViewBag.WorkPlace = new SelectList(db1.Contract_Comp, "C_ANAME", "C_ANAME");
            //ViewBag.JsonWorkPlace = JsonConvert.SerializeObject(new SelectList(db.CONTRACT_COMP, "C_ANAME", "C_ANAME"));

            var doctorWorkPlace = db.DoctorWorkPlace.Where(d => d.UserId == id);
            return View(doctorWorkPlace.ToList());
        }

        [HttpPost]
        public JsonResult InsertWorkPlace(List<DoctorWorkPlace> doctorWorkPlace)
        {
            using (ApplicationDbContext entities = new ApplicationDbContext())
            {
                //Truncate Table to delete all old records.
                entities.Database.ExecuteSqlCommand("Delete from DoctorWorkPlaces where UserId='"+doctorWorkPlace.FirstOrDefault().UserId+"'");

                //Check for NULL.
                if (doctorWorkPlace == null)
                {
                    doctorWorkPlace = new List<DoctorWorkPlace>();
                }

                //Loop and insert records.
                foreach (DoctorWorkPlace workplace in doctorWorkPlace)
                {
                    entities.DoctorWorkPlace.Add(workplace);
                }
                int insertedRecords = entities.SaveChanges();

                return Json(insertedRecords);
            }
        }
        public class Data
        {
            public string CId { get; set; }
        }
        [HttpPost]
        public JsonResult Search(Data data)
        {
           // if (data !)
            int test = Convert.ToInt32(data.CId);
            var CID=db1.Contract_Comp.Where(c => c.C_COMP_ID == test).FirstOrDefault();
            return Json(CID.C_ANAME);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,TypeId,WorkPlace,Address,PermenantExpenditure")] DoctorWorkPlace doctorWorkPlace)
        {
            if (ModelState.IsValid)
            {
                db.DoctorWorkPlace.Add(doctorWorkPlace);
                db.SaveChanges();
                return RedirectToAction("Create","Doctors_Permissions",new {id=doctorWorkPlace.UserId });
            }
            ApplicationUser aspNetUser = db.Users.Find(doctorWorkPlace.UserId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", doctorWorkPlace.UserId);
            ViewBag.TypeId = aspNetUser.TypeId;
            return View(doctorWorkPlace);
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
