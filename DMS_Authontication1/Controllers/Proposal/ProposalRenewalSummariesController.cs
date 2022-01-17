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
    public class ProposalRenewalSummariesController : Controller
    {
        public int pro_id { get; set; }
        public int pro_num { get; set; }
       // public int Pro_num { get; set; }
        private DMS_TESTEntities db = new DMS_TESTEntities();

        // GET: ProposalRenewalSummaries
     

        // GET: ProposalRenewalSummaries/Details/5
        public ActionResult Details(int? Id,int? Num)
        {
            Session["pro_id"] = Id; ;
            Session["Pro_num"] = Num;
            pro_id = (int)((Id == null) ? pro_id : Id);
            pro_num = (int)((Num == null) ? pro_num : Num);
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalRenewalSummary proposalRenewalSummary = db.ProposalRenewalSummaries.Where(x=>x.ProposalId== pro_id).FirstOrDefault();
            if (proposalRenewalSummary == null)
            {
                return RedirectToAction("Create",new { Id=Id,Num=Num});
            }
            return View(proposalRenewalSummary);
        }

        // GET: ProposalRenewalSummaries/Create
        public ActionResult Create(int? Id, int? Num)
        {
            Session["pro_id"] = Id; ;
            Session["Pro_num"] = Num;
            pro_id = (int)Session["pro_id"];
            pro_num = (int)Session["Pro_num"];
           // Pro_num = (int)((Num == null) ? Pro_num : Num);
            return View();
        }

        // POST: ProposalRenewalSummaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProposalId,Status,LastLR,RateIncrease,NoSubscribersRate,ExtraBenefits,ExtraCopayment,SpecialConditions,Recommendations,NoYears,OntractCancelationPeriod,Last3YearsLR,last3YearRateIncrease,Last3YearsChangeNoSubscribersStart,Last3YearsChangeNoSubscribersCurrent,PreviousControlMeasures")] ProposalRenewalSummary proposalRenewalSummary)
        {
            if (ModelState.IsValid)
            {
                proposalRenewalSummary.ProposalId = (int)(Session["pro_id"]);
                var id = db.ProposalRenewalSummaries.Max(n=>n.Id)+1;
                proposalRenewalSummary.Id = id;
                db.ProposalRenewalSummaries.Add(proposalRenewalSummary);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = (int)(Session["pro_id"]), Num = (int)(Session["Pro_num"]) });
                //return RedirectToAction("Details");
            }

            return View(proposalRenewalSummary);
        }

        // GET: ProposalRenewalSummaries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalRenewalSummary proposalRenewalSummary = db.ProposalRenewalSummaries.Find(id);
            if (proposalRenewalSummary == null)
            {
                return HttpNotFound();
            }
            return View(proposalRenewalSummary);
        }

        // POST: ProposalRenewalSummaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProposalId,Status,LastLR,RateIncrease,NoSubscribersRate,ExtraBenefits,ExtraCopayment,SpecialConditions,Recommendations,NoYears,OntractCancelationPeriod,Last3YearsLR,last3YearRateIncrease,Last3YearsChangeNoSubscribersStart,Last3YearsChangeNoSubscribersCurrent,PreviousControlMeasures")] ProposalRenewalSummary proposalRenewalSummary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proposalRenewalSummary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(proposalRenewalSummary);
        }

        // GET: ProposalRenewalSummaries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalRenewalSummary proposalRenewalSummary = db.ProposalRenewalSummaries.Find(id);
            if (proposalRenewalSummary == null)
            {
                return HttpNotFound();
            }
            return View(proposalRenewalSummary);
        }

        // POST: ProposalRenewalSummaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProposalRenewalSummary proposalRenewalSummary = db.ProposalRenewalSummaries.Find(id);
            db.ProposalRenewalSummaries.Remove(proposalRenewalSummary);
            db.SaveChanges();
            return RedirectToAction("Details");
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
