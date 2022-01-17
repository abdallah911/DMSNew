using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;

namespace DMS_Authontication1.Controllers.Proposal
{
    public class ProposalClassesController : Controller
    {
        //public int pro_id { get; set; }
        //public int Pro_num { get; set; }
        //public string  Pro_Type { get; set; }
        private DMS_TESTEntities db = new DMS_TESTEntities();

        // GET: ProposalClasses
        public ActionResult Index(int? Id, int? Num,string Type)
        {
            Session["pro_id"] =  Id; ;
            Session["Pro_num"] = Num;
            Session["Pro_Type"] = Type;
          var  pro_id = Session["pro_id"];
           var Pro_num = Session["Pro_num"];
            var Pro_Type = Session["Pro_Type"] ;
           
            if (Type.Contains("أعمار"))
                ViewBag.Age = "Age";
            if (Type.Contains("النوع"))
                ViewBag.Gender = "Gender";
            if (Type.Contains("الفرع"))
                ViewBag.Branch = "Branch";
            if (Type.Contains("علاقات"))
                ViewBag.Relations = "Relations";
            var proposalClasses = db.ProposalClasses.Where(n=>n.ProposalId==(int)pro_id&& n.NumProposal==(int)Pro_num ).ToList();
            
            
            return View(proposalClasses);
        }

        // GET: ProposalClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalClass proposalClass = db.ProposalClasses.Find(id);
            if (proposalClass == null)
            {
                return HttpNotFound();
            }
            return View(proposalClass);
        }
        [HttpGet]
        // GET: ProposalClasses/Create
        public ActionResult Create()
        {
            string Pro_Type = Session["Pro_Type"].ToString();
            if (Pro_Type.Contains("أعمار"))
                ViewBag.Age = "Age"; 
            if (Pro_Type.Contains("النوع"))
                ViewBag.Gender = "Gender";
            if (Pro_Type.Contains("الفرع"))
                ViewBag.Branch = "Branch"; 
            if (Pro_Type.Contains("علاقات"))
                ViewBag.Relations = "Relations";

            var compclasses = (from comcont in db.CompContractClasses
                               
                               join insclass in db.Insurance_Class
                               on comcont.CLASS_CODE equals insclass.CLASS_CODE
                               select new
                               {
                                   ClassCode = comcont.CLASS_CODE,
                                   ClassString = comcont.CLASS_CODE + " | " + insclass.CLASS_ANAME
                               }).ToList().Distinct();

            SelectList compclasseslist = new SelectList(compclasses, "ClassCode", "ClassString");
            ViewBag.ClassCode = compclasseslist;
            return View();
        }

        // POST: ProposalClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProposalId,NumProposal,ClassCode,NumMale,NumFemael,NumChronic,TotalChronic,Premium,Max,AgeFrom,AgeFromType,AgeTo,AgeToType,Gender,Branch")] ProposalClass proposalClass)
        {
            if (ModelState.IsValid)
            {
                proposalClass.ProposalId = (int)Session["pro_id"] ;
                proposalClass.NumProposal=(int)Session["Pro_num"];
                var id = db.ProposalClasses.Max(n => n.Id) + 1;
                proposalClass.Id = id;
                db.ProposalClasses.Add(proposalClass);
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = (int)(Session["pro_id"]), Num = (int)(Session["Pro_num"]), Type= Session["Pro_Type"].ToString() });

               // return RedirectToAction("Index");
            }

            ViewBag.ProposalId = new SelectList(db.ProposalBasicDatas, "Id", "CompName", proposalClass.ProposalId);
            ViewBag.Id = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClass.Id);
            ViewBag.Id = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClass.Id);
            return View(proposalClass);
        }

        // GET: ProposalClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalClass proposalClass = db.ProposalClasses.Find(id);
            if (proposalClass == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProposalId = new SelectList(db.ProposalBasicDatas, "Id", "CompName", proposalClass.ProposalId);
            ViewBag.Id = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClass.Id);
            ViewBag.Id = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClass.Id);
            return View(proposalClass);
        }

        // POST: ProposalClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProposalId,NumProposal,ClassCode,NumMale,NumFemael,NumChronic,TotalChronic,Premium,Max,AgeFrom,AgeFromType,AgeTo,AgeToType,Gender,Branch")] ProposalClass proposalClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proposalClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = (int)(Session["pro_id"]), Num = (int)(Session["Pro_num"]), Type = Session["Pro_Type"].ToString() });

                //return RedirectToAction("Index");
            }
            ViewBag.ProposalId = new SelectList(db.ProposalBasicDatas, "Id", "CompName", proposalClass.ProposalId);
            ViewBag.Id = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClass.Id);
            ViewBag.Id = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClass.Id);
            return View(proposalClass);
        }

        // GET: ProposalClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalClass proposalClass = db.ProposalClasses.Find(id);
            if (proposalClass == null)
            {
                return HttpNotFound();
            }
            return View(proposalClass);
        }

        // POST: ProposalClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProposalClass proposalClass = db.ProposalClasses.Find(id);
            db.ProposalClasses.Remove(proposalClass);
            db.SaveChanges();
            return RedirectToAction("Index", new { Id = (int)(Session["pro_id"]), Num = (int)(Session["Pro_num"]), Type = Session["Pro_Type"].ToString() });

            //return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
