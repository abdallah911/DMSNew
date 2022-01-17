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
    public class ProposalClassRelationsController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();

        // GET: ProposalClassRelations
        public ActionResult Index()
        {
            var proposalClassRelations = db.ProposalClassRelations.Include(p => p.ProposalClass);
            return View(proposalClassRelations.ToList());
        }

        // GET: ProposalClassRelations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalClassRelation proposalClassRelation = db.ProposalClassRelations.Find(id);
            if (proposalClassRelation == null)
            {
                return HttpNotFound();
            }
            return View(proposalClassRelation);
        }

        // GET: ProposalClassRelations/Create
        public ActionResult Create()
        {
            ViewBag.ProposalClassId = new SelectList(db.ProposalClasses, "Id", "ClassCode");
            return View();
        }

        // POST: ProposalClassRelations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,ProposalClassId")] ProposalClassRelation proposalClassRelation)
        {
            if (ModelState.IsValid)
            {
                db.ProposalClassRelations.Add(proposalClassRelation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProposalClassId = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClassRelation.ProposalClassId);
            return View(proposalClassRelation);
        }

        // GET: ProposalClassRelations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalClassRelation proposalClassRelation = db.ProposalClassRelations.Find(id);
            if (proposalClassRelation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProposalClassId = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClassRelation.ProposalClassId);
            return View(proposalClassRelation);
        }

        // POST: ProposalClassRelations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,ProposalClassId")] ProposalClassRelation proposalClassRelation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proposalClassRelation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProposalClassId = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalClassRelation.ProposalClassId);
            return View(proposalClassRelation);
        }

        // GET: ProposalClassRelations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalClassRelation proposalClassRelation = db.ProposalClassRelations.Find(id);
            if (proposalClassRelation == null)
            {
                return HttpNotFound();
            }
            return View(proposalClassRelation);
        }

        // POST: ProposalClassRelations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProposalClassRelation proposalClassRelation = db.ProposalClassRelations.Find(id);
            db.ProposalClassRelations.Remove(proposalClassRelation);
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
