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
    public class RaysManagementController : Controller
    {
        //private DMS_TESTEntities db = new DMS_TESTEntities();
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public RaysManagementController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();

        }
        // GET: RaysManagement
        public ActionResult Index()
        {
            return View();
        }

        // GET: RaysManagement/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Serv_Ray serv_Ray = db.Serv_Ray.Find(id);
            if (serv_Ray == null)
            {
                return HttpNotFound();
            }
            return View(serv_Ray);
        }

        // GET: RaysManagement/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.provider = new SelectList(db.Serv_Providers1.Where(x => x.PRV_TYPE == 4).Select(x => new
                {
                    PR_CODE = x.PR_CODE,
                    PR_ENAME = x.PR_CODE + "||" + x.PR_ENAME
                }).ToList(), "PR_CODE", "PR_ENAME");
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
                list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

                ViewBag.GroupType = new SelectList(list, "Value", "Text", "NO");
            }
            ViewBag.Groups = new SelectList(db.Group_Ray.ToList(), "Group_id", "Group_name");

            return View();
        }

        // POST: RaysManagement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LAB_CODE,PR_CODE,PR_ANAME,DMS_CODE,SERV_CODE,SERV_AMOUNT,SERV_ANAME,GRUOP_TYPE,LOOK,GRUOP_ID,GRUOP_NAME,IsSync,SyncDate,SyncBy")] Serv_Ray serv_Ray)
        {
            if (ModelState.IsValid)
            {
                // var maxcode = db.Serv_Ray.Where(x => x.LAB_CODE == serv_Ray.LAB_CODE).ToList();
                var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                long userProvider = Convert.ToInt64(user.Provider);
                if (!User.IsInRole("Admin"))
                {
                    serv_Ray.LAB_CODE = userProvider;
                    serv_Ray.GRUOP_TYPE = "NO";
                    // serv_Ray.SERV_CODE = maxcode.Count == 0 ? 1 : maxcode.Max(x => x.SERV_CODE) + 1;
                    serv_Ray.PR_ANAME = "";
                }
                serv_Ray.GRUOP_NAME = db.Group_Ray.FirstOrDefault(x => x.Group_Id == serv_Ray.GRUOP_ID).Group_Name;

                if (db.Serv_Ray.Where(x => x.LAB_CODE == serv_Ray.LAB_CODE && x.SERV_CODE == serv_Ray.SERV_CODE).ToList().Count != 0)
                {
                    if (User.IsInRole("Admin"))
                    {
                        ViewBag.provider = new SelectList(db.Serv_Providers1.Where(x => x.PRV_TYPE == 4).ToList(), "PR_CODE", "PR_ENAME");
                        List<SelectListItem> list = new List<SelectListItem>();
                        list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
                        list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

                        ViewBag.GroupType = new SelectList(list, "Value", "Text", "NO");
                    }
                    ViewBag.Groups = new SelectList(db.Group_Ray.ToList(), "Group_id", "Group_name");
                    ViewBag.exsit = "exsit";
                    return View();
                }
                Serv_Providers1 serv_Providers1 = db.Serv_Providers1.Where(s => s.PR_CODE == serv_Ray.LAB_CODE && s.PRV_TYPE == 4).FirstOrDefault();
                serv_Ray.PR_ANAME = serv_Providers1.PR_ANAME;
                db.Serv_Ray.Add(serv_Ray);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (User.IsInRole("Admin"))
            {
                ViewBag.provider = new SelectList(db.Serv_Providers1.Where(x => x.PRV_TYPE == 4).ToList(), "PR_CODE", "PR_ENAME");
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
                list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

                ViewBag.GroupType = new SelectList(list, "Value", "Text", "NO");
            }
            ViewBag.Groups = new SelectList(db.Group_Ray.ToList(), "Group_id", "Group_name");
            return View(serv_Ray);
        }

        // GET: RaysManagement/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Serv_Ray serv_Ray = db.Serv_Ray.Find(id);
            ViewBag.Groups = new SelectList(db.Group_Ray.ToList(), "Group_id", "Group_name", serv_Ray.GRUOP_ID);

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
            list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

            ViewBag.GroupType = new SelectList(list, "Value", "Text", serv_Ray.GRUOP_TYPE);

            if (serv_Ray == null)
            {
                return HttpNotFound();
            }
            return View(serv_Ray);
        }

        // POST: RaysManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LAB_CODE,PR_CODE,PR_ANAME,DMS_CODE,SERV_CODE,SERV_AMOUNT,SERV_ANAME,GRUOP_TYPE,LOOK,GRUOP_ID,GRUOP_NAME,IsSync,SyncDate,SyncBy")] Serv_Ray serv_Ray)
        {
            if (ModelState.IsValid)
            {
                Serv_Ray CurrentServ_Ray = db.Serv_Ray.Find(serv_Ray.Id);
                CurrentServ_Ray.GRUOP_ID = serv_Ray.GRUOP_ID;
                CurrentServ_Ray.SERV_AMOUNT = serv_Ray.SERV_AMOUNT;
                CurrentServ_Ray.GRUOP_NAME = db.Group_Ray.FirstOrDefault(x => x.Group_Id == serv_Ray.GRUOP_ID).Group_Name;
                CurrentServ_Ray.GRUOP_TYPE = serv_Ray.GRUOP_TYPE == null ? CurrentServ_Ray.GRUOP_TYPE : serv_Ray.GRUOP_TYPE;

                db.Entry(CurrentServ_Ray).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Groups = new SelectList(db.Group_Ray.ToList(), "Group_id", "Group_name", serv_Ray.GRUOP_ID);

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "YES", Text = "YES" });
            list.Add(new SelectListItem() { Value = "NO", Text = "NO" });

            ViewBag.GroupType = new SelectList(list, "Value", "Text", serv_Ray.GRUOP_TYPE);
            return View(serv_Ray);
        }

        // GET: RaysManagement/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Serv_Ray serv_Ray = db.Serv_Ray.Find(id);
            if (serv_Ray == null)
            {
                return HttpNotFound();
            }
            return View(serv_Ray);
        }

        // POST: RaysManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Serv_Ray serv_Ray = db.Serv_Ray.Find(id);
            db.Serv_Ray.Remove(serv_Ray);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult RayList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            if (User.IsInRole("Rays_Admin"))
            {
                var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                long userProvider = Convert.ToInt64(user.Provider);
                // return View(db.Serv_Ray.Where(x => x.LAB_CODE == userProvider).ToList());
            }
            //return View(db.Serv_Ray.ToList());
            if (sSearch != null)
            {
                if (User.IsInRole("Rays_Admin"))
                {
                    var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    long userProvider = Convert.ToInt64(user.Provider);
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Ray.OrderBy(m => m.Id)
                    .Where(r => (r.SERV_ANAME.Contains(sSearch) || r.GRUOP_NAME.Contains(sSearch) || r.GRUOP_TYPE.Contains(sSearch)) && r.LAB_CODE == userProvider)
                    .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Ray.Count(),
                        iTotalDisplayRecords = db.Serv_Ray.Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Ray.OrderBy(m => m.Id)
                        .Where(r => r.SERV_ANAME.Contains(sSearch) || r.GRUOP_NAME.Contains(sSearch) || r.GRUOP_TYPE.Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Ray.Count(),
                        iTotalDisplayRecords = db.Serv_Ray.Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                if (User.IsInRole("Rays_Admin"))
                {
                    var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    long userProvider = Convert.ToInt64(user.Provider);
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Ray.OrderBy(m => m.Id).Where(r => r.LAB_CODE == userProvider)
                   .Select(l => new Serv_Ray
                   {
                       LAB_CODE = l.LAB_CODE,
                       SERV_CODE = l.SERV_CODE,
                       SERV_ANAME = l.SERV_ANAME,
                       SERV_AMOUNT = l.SERV_AMOUNT,
                       GRUOP_NAME = l.GRUOP_NAME,
                       GRUOP_TYPE = l.GRUOP_TYPE,
                       Id = l.Id

                   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Ray.Count(),
                        iTotalDisplayRecords = db.Serv_Ray.Count()
                    };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.Serv_Ray.OrderBy(m => m.Id)
                   .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.Serv_Ray.Count(),
                        iTotalDisplayRecords = db.Serv_Ray.Count()
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
