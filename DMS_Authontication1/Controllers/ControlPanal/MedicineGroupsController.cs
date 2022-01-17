using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;

namespace DMS_Authontication1.Controllers.ControlPanal
{
    [Authorize(Roles = "Admin,Doctor")]
    public class MedicineGroupsController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();

        // GET: MedicineGroups
        public ActionResult Index()
        {
            return View(db.MedicineGroups.ToList());
        }

      
        // GET: MedicineGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedicineGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,GroupName,GroupType")] MedicineGroup medicineGroup)
        {
            if (ModelState.IsValid)
            {
                var exsit = db.MedicineGroups.Where(x => x.GroupName == medicineGroup.GroupName).FirstOrDefault();
                if (exsit != null)
                {
                    ViewBag.exsit = "exsit";
                    return View();
                }
                else
                {
                    medicineGroup.GroupId = Convert.ToString(Convert.ToInt32(db.MedicineGroups.Max(x => x.GroupId)) + 1);
                    db.MedicineGroups.Add(medicineGroup);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }

            return View(medicineGroup);
        }

        // GET: MedicineGroups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicineGroup medicineGroup = db.MedicineGroups.Find(id);
            if (medicineGroup == null)
            {
                return HttpNotFound();
            }
            return View(medicineGroup);
        }

        // POST: MedicineGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,GroupName,GroupType")] MedicineGroup medicineGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicineGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(medicineGroup);
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
