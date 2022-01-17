using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR")]
    public class IndemnitiesController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();
        //[Authorize(Roles = "Admin")]

        // GET: Indemnities
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Indemnities.OrderByDescending(x => x.BatchID).ToList());

            }
            return View(db.Indemnities.Where(x => x.CreatedBy == User.Identity.Name).OrderByDescending(x => x.BatchID).ToList());
        }

        // GET: Indemnities/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indemnity indemnity = db.Indemnities.Find(id);
            if (indemnity == null)
            {
                return HttpNotFound();
            }
            return View(indemnity);
        }

        // GET: Indemnities/Create
        public ActionResult Create()
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.UserCompId = CurrentUser.Provider;
            var services = db.Services.ToList();
            SelectList ServicesList = new SelectList(services, "ID", "NameAr");
            ViewBag.Services = ServicesList;
            return View();
        }

        // POST: Indemnities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BatchID,CardID,ServiceCardID,EmployeeName,CompanyName,NationalID,BankName,BankBranch,BankAccount,Type,ServiceID,ServiceDate,Total,CreatedDate,CreatedBy,UpdateDate,UpdateBy,DeletedBy,DeletedDate,IsDeleted,Status,AttachPDF")] Indemnity indemnity)
        {
            var services = db.Services.ToList();
            SelectList ServicesList = new SelectList(services, "ID", "NameAr");
            ViewBag.Services = ServicesList;
            if (ModelState.IsValid)
            {
                db.Indemnities.Add(indemnity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(indemnity);
        }

        // GET: Indemnities/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indemnity indemnity = db.Indemnities.Find(id);
            if (indemnity == null)
            {
                return HttpNotFound();
            }
            return View(indemnity);
        }

        // POST: Indemnities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BatchID,CardID,EmployeeName,CompanyName,NationalID,BankName,BankBranch,BankAccount,Type,ServiceID,ServiceDate,Total,CreatedDate,CreatedBy,UpdateDate,UpdateBy,DeletedBy,DeletedDate,IsDeleted,Status,AttachPDF")] Indemnity indemnity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indemnity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indemnity);
        }

        // GET: Indemnities/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indemnity indemnity = db.Indemnities.Find(id);
            if (indemnity == null)
            {
                return HttpNotFound();
            }
            return View(indemnity);
        }

        // POST: Indemnities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            List<IndemnityCardsService> indemnitycardsService = db.IndemnityCardsServices.Where(x=>x.IndemnityId==id).ToList();
            foreach(var item in indemnitycardsService)
            {
                List<IndemnityService> indemnityService = db.IndemnityServices.Where(x => x.IndemnityCardsServicesId == item.Id).ToList();

                db.IndemnityServices.RemoveRange(indemnityService);
            }
            db.IndemnityCardsServices.RemoveRange(indemnitycardsService);
            Indemnity indemnity = db.Indemnities.Find(id);
            db.Indemnities.Remove(indemnity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult SaveIndemnity(Indemnity data)
        {
            data.CreatedBy = User.Identity.Name;
            data.CreatedDate = DateTime.Now;
            foreach (var item in data.IndemnityCardsServices)
            {
                if (item.AttachPDF != null)
                {
                    string[] attaches = item.AttachPDF.Split(',');
                    string newAttachment = "";
                    for (int i = 0; i < attaches.Length - 1; i++)
                    {
                        string[] singleattach = attaches[i].Split('.');
                        string attchName = singleattach[0] + DateTime.Now.ToString("yyMMddHH") + "." + singleattach[1];
                        newAttachment += attchName;
                        if (i < attaches.Length - 1)
                        {
                            newAttachment += ",";
                        }
                    }
                    item.AttachPDF = newAttachment;
                }
            }
            db.Indemnities.Add(data);

            int result = db.SaveChanges();
            return Json(data.BatchID);
        }
        public JsonResult SaveAttaches(List<HttpPostedFileBase> Files)
        {
            //HttpPostedFileBase File = Files[0];
            if (Files != null)
            {
                foreach (HttpPostedFileBase File in Files)
                {
                    var fileName = Path.GetFileName(File.FileName);
                    var extention = Path.GetExtension(File.FileName);
                    var filenamewithoutextension = Path.GetFileNameWithoutExtension(File.FileName);
                    fileName = filenamewithoutextension + DateTime.Now.ToString("yyMMddHH") + extention;
                    //var filenamewithoutextension = Path.GetFileNameWithoutExtension(ImageFile.FileName);

                    File.SaveAs(Server.MapPath("/Content/IndemnitiesAttachments/" + fileName /*ImageFile.FileName*/));
                }
                //  Session["FileName"] = "/Content/IndemnitiesAttaches/" + fileName;
            }

            return new JsonResult { Data = "r", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult Download(long id)
        {
            string files = db.IndemnityCardsServices.Where(x => x.Id == id).FirstOrDefault().AttachPDF;
            string[] filesArray = files.Split(',');
            List<string> filesList = new List<string>();
            for (int i = 0; i < filesArray.Length - 1; i++)
            {
                filesArray[i] = /*"~/Content/IndemnitiesAttachments/" +*/ filesArray[i];
                filesList.Add(Server.MapPath(filesArray[i]));
            }
            // Server.MapPath("/Content/IndemnitiesAttachments/"
            //string file = @"c:\someFolder\foo.xlsx";
            //string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //return File(file, contentType, Path.GetFileName(file));

            var archive = Server.MapPath("~/Content/IndemnitiesAttachments/archive.zip");
            var temp = Server.MapPath("~/Content/IndemnitiesAttachments/temp");

            // clear any existing archive
            if (System.IO.File.Exists(archive))
            {
                System.IO.File.Delete(archive);
            }
            // empty the temp folder
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

            // copy the selected files to the temp folder
            filesList.ForEach(f => System.IO.File.Copy(f, Path.Combine(temp, Path.GetFileName(f))));

            // create a new archive
            ZipFile.CreateFromDirectory(temp, archive);

            return File(archive, "application/zip", "archive.zip");
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
