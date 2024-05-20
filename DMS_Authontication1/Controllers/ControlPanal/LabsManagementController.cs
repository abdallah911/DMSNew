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
    public class LabsManagementController : Controller
    {
        //private DMS_TESTEntities db = new DMS_TESTEntities();

        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public LabsManagementController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();

        }
        // GET: LabsManagement
        public ActionResult Index()
        {
            return View();
        }

        // GET: LabsManagement/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Serv_Lab serv_Lab = db.Serv_Lab.Find(id);
            if (serv_Lab == null)
            {
                return HttpNotFound();
            }
            return View(serv_Lab);
        }

        // GET: LabsManagement/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.provider = new SelectList(db.Serv_Providers1.Where(x => x.PRV_TYPE == 3).Select(x => new
                {
                    PR_CODE = x.PR_CODE,
                    PR_ENAME = x.PR_CODE + "||" + x.PR_ENAME
                }).ToList(), "PR_CODE", "PR_ENAME");
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
                list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

                ViewBag.GroupType = new SelectList(list, "Value", "Text", "NO");
            }
            ViewBag.Groups = new SelectList(db.Group_Lab.ToList(), "Group_id", "Group_name");
            return View();
        }

        // POST: LabsManagement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LAB_CODE,PR_CODE,PR_ANAME,DMS_CODE,SERV_CODE,SERV_AMOUNT,SERV_ANAME,GRUOP_TYPE,LOOK,GRUOP_ID,GRUOP_NAME,IsSync,SyncDate,SyncBy")] Serv_Lab serv_Lab)
        {
            //serv_Lab.SERV_CODE = null;

            if (ModelState.IsValid)
            {
                var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

                long userProvider = Convert.ToInt64(user.Provider);
                if (!User.IsInRole("Admin"))
                {
                    serv_Lab.LAB_CODE = userProvider;
                    // var maxcode = db.Serv_Lab.Where(x => x.LAB_CODE == serv_Lab.LAB_CODE).ToList();
                    serv_Lab.GRUOP_TYPE = "NO";
                    // serv_Lab.SERV_CODE = maxcode.Count==0?1: maxcode.Max(x=>x.SERV_CODE)+1;
                    serv_Lab.PR_ANAME = "";
                    string d = serv_Lab.DMS_CODE.ToString();
                }
                serv_Lab.GRUOP_NAME = db.Group_Lab.FirstOrDefault(x => x.Group_Id == serv_Lab.GRUOP_ID).Group_Name;
                if (db.Serv_Lab.Where(x => x.LAB_CODE == serv_Lab.LAB_CODE && x.SERV_CODE == serv_Lab.SERV_CODE).ToList().Count != 0)
                {
                    if (User.IsInRole("Admin"))
                    {
                        ViewBag.provider = new SelectList(db.Serv_Providers1.Where(x => x.PRV_TYPE == 3).ToList(), "PR_CODE", "PR_ENAME");
                        List<SelectListItem> list = new List<SelectListItem>();
                        list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
                        list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

                        ViewBag.GroupType = new SelectList(list, "Value", "Text", "NO");
                    }
                    ViewBag.Groups = new SelectList(db.Group_Lab.ToList(), "Group_id", "Group_name");
                    ViewBag.exsit = "exsit";
                    return View();
                }
                Serv_Providers1 serv_Providers1 = db.Serv_Providers1.Where(s => s.PR_CODE == serv_Lab.LAB_CODE && s.PRV_TYPE == 3).FirstOrDefault();
                serv_Lab.PR_ANAME = serv_Providers1.PR_ANAME;
                db.Serv_Lab.Add(serv_Lab);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (User.IsInRole("Admin"))
            {
                ViewBag.provider = new SelectList(db.Serv_Providers1.Where(x => x.PRV_TYPE == 3).ToList(), "PR_CODE", "PR_ENAME");
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
                list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

                ViewBag.GroupType = new SelectList(list, "Value", "Text", "NO");
            }
            ViewBag.Groups = new SelectList(db.Group_Lab.ToList(), "Group_id", "Group_name");

            return View(serv_Lab);
        }

        // GET: LabsManagement/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Serv_Lab serv_Lab = db.Serv_Lab.Find(id);
            ViewBag.Groups = new SelectList(db.Group_Lab.ToList(), "Group_id", "Group_name", serv_Lab.GRUOP_ID);

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
            list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

            ViewBag.GroupType = new SelectList(list, "Value", "Text", serv_Lab.GRUOP_TYPE);
            if (serv_Lab == null)
            {
                return HttpNotFound();
            }
            return View(serv_Lab);
        }

        // POST: LabsManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SERV_AMOUNT,SERV_ANAME,GRUOP_TYPE,LOOK,GRUOP_ID")] Serv_Lab serv_Lab)
        {
            if (ModelState.IsValid)
            {
                Serv_Lab CurrentServ_Lab = db.Serv_Lab.Find(serv_Lab.Id);
                CurrentServ_Lab.GRUOP_ID = serv_Lab.GRUOP_ID;
                CurrentServ_Lab.SERV_AMOUNT = serv_Lab.SERV_AMOUNT;
                CurrentServ_Lab.GRUOP_NAME = db.Group_Lab.FirstOrDefault(x => x.Group_Id == serv_Lab.GRUOP_ID).Group_Name;
                CurrentServ_Lab.GRUOP_TYPE = serv_Lab.GRUOP_TYPE == null ? CurrentServ_Lab.GRUOP_TYPE : serv_Lab.GRUOP_TYPE;
                db.Entry(CurrentServ_Lab).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Groups = new SelectList(db.Group_Lab.ToList(), "Group_id", "Group_name", serv_Lab.GRUOP_ID);

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
            list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

            ViewBag.GroupType = new SelectList(list, "Value", "Text", serv_Lab.GRUOP_TYPE);
            return View(serv_Lab);
        }

        // GET: LabsManagement/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Serv_Lab serv_Lab = db.Serv_Lab.Find(id);
            if (serv_Lab == null)
            {
                return HttpNotFound();
            }
            return View(serv_Lab);
        }

        // POST: LabsManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Serv_Lab serv_Lab = db.Serv_Lab.Find(id);
            db.Serv_Lab.Remove(serv_Lab);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult LabList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            if (User.IsInRole("Lab_Admin"))
            {
                var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                long userProvider = Convert.ToInt64(user.Provider);
                // return View(db.Serv_Lab.Where(x => x.LAB_CODE == userProvider).ToList());
            }
            //return View(db.Serv_Lab.ToList());
            if (sSearch != null)
            {
                if (User.IsInRole("Lab_Admin"))
                {
                    var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    long userProvider = Convert.ToInt64(user.Provider);
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Lab.OrderBy(m => m.Id)
                    .Where(r => (r.SERV_ANAME.Contains(sSearch) || r.GRUOP_NAME.Contains(sSearch) || r.GRUOP_TYPE.Contains(sSearch)) && r.LAB_CODE == userProvider)
                    .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Lab.Count(),
                        iTotalDisplayRecords = db.Serv_Lab.Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Lab.OrderBy(m => m.Id)
                        .Where(r => r.SERV_ANAME.Contains(sSearch) || r.GRUOP_NAME.Contains(sSearch) || r.GRUOP_TYPE.Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Lab.Count(),
                        iTotalDisplayRecords = db.Serv_Lab.Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                if (User.IsInRole("Lab_Admin"))
                {
                    var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    long userProvider = Convert.ToInt64(user.Provider);
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Lab.OrderBy(m => m.Id).Where(r => r.LAB_CODE == userProvider)
                   .Select(l => new Serv_Lab
                   {
                       LAB_CODE = l.LAB_CODE,
                       SERV_CODE = l.SERV_CODE,
                       SERV_ANAME = l.SERV_ANAME,
                       SERV_AMOUNT = l.SERV_AMOUNT,
                       GRUOP_NAME = l.GRUOP_NAME,
                       GRUOP_TYPE = l.GRUOP_TYPE,
                       Id = l.Id

                   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Lab.Count(),
                        iTotalDisplayRecords = db.Serv_Lab.Count()
                    };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Lab.OrderBy(m => m.Id)
                   .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Lab.Count(),
                        iTotalDisplayRecords = db.Serv_Lab.Count()
                    };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }


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
