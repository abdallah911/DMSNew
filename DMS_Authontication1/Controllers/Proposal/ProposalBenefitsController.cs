using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel.Proposal;

namespace DMS_Authontication1.Controllers.Proposal
{
    public class ProposalBenefitsController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();

        // GET: ProposalBenefits
        public ActionResult Index(int Id)
        {
            Session["ProposalClassId"] = Id; 
            var benefitsUse = from P in db.ProposalBenefits.Where(n => n.ProposalClassId == Id)
                              join pt in   db.ProposalBenefitsTypes on  P.TypeBenefitsId equals pt.Id 
                            
                              select new ProposalBenefitVM { Id=P.Id ,Name=pt.BenefitsName,Match=P.Match};
            
           var query = from pt in db.ProposalBenefitsTypes
                       select new ProposalBenefitVM
            {
                Id = pt.Id,
                Name = pt. BenefitsName,
                Match = null
            };

            var benefitsUnUse = query.ToList();
            //var query = from pt in db.ProposalBenefitsTypes
            //            join pet in db.ProposalBenefits.Where(n=>n.ProposalClassId == Id) on pt.Id equals pet.TypeBenefitsId  into gj
            //            from subpet in gj.DefaultIfEmpty()


            //            select new ProposalBenefitVM
            //            {
            //                Id = pt.Id,
            //                Name = pt.BenefitsName,
            //                Match = null
            //            };
            //var benefitsUnUse = from pt in db.ProposalBenefitsTypes
            //                    join  P in db.ProposalBenefits.Where(n => n.ProposalClassId == Id) on  pt.Id equals P.TypeBenefitsId into temp
            //                    from P in temp.DefaultIfEmpty()
            //                    where P== temp.DefaultIfEmpty()
            //                    select new ProposalBenefitVM
            //                    {
            //                      Id=  pt.Id,
            //                        Name = pt.BenefitsName,
            //                        Match =null
            //                    };

            //foreach (var item in query)
            //{
            //    item.Name
            //}

            foreach (var item in benefitsUse)
            {
                var benfit = benefitsUnUse.Where(n => n.Name == item.Name).FirstOrDefault();
                if (benfit !=null)
                { benefitsUnUse.Remove(benfit); }

            }
            var proposalClass = db.ProposalClasses.Where(n => n.Id == Id).FirstOrDefault();
            ViewBag.ClassType = db.ProposalBasicDatas.Where(n => n.Id == proposalClass.ProposalId && n.NumProposal == proposalClass.NumProposal).FirstOrDefault().TypeProposal;
            ViewBag.proclass = proposalClass;
            var proposalBenefits = new ProposalBenefitsVM { BenefitsUse = benefitsUse.ToList(),BenefitsUnUse= benefitsUnUse.ToList() };
            return View(proposalBenefits);
        }

        // GET: ProposalBenefits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalBenefit proposalBenefit = db.ProposalBenefits.Find(id);
            if (proposalBenefit == null)
            {
                return HttpNotFound();
            }
            return View(proposalBenefit);
        }

        // GET: ProposalBenefits/Create
        public ActionResult Create()
        {
            ViewBag.ProposalClassId = new SelectList(db.ProposalClasses, "Id", "ClassCode");
            return View();
        }

        // POST: ProposalBenefits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProposalClassId,TypeBenefitsId,Match")] ProposalBenefit proposalBenefit)
        {
            if (ModelState.IsValid)
            {
                var id =  db.ProposalBenefits.ToList().Count>0? db.ProposalBenefits.Max(n => n.Id) + 1:1;
                proposalBenefit.Id = id;
                db.ProposalBenefits.Add(proposalBenefit);
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = (int)(Session["ProposalClassId"]) });
              //  return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { Id = (int)(Session["ProposalClassId"]) });

        }

        // GET: ProposalBenefits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalBenefit proposalBenefit = db.ProposalBenefits.Find(id);
            if (proposalBenefit == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProposalClassId = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalBenefit.ProposalClassId);
            return View(proposalBenefit);
        }

        // POST: ProposalBenefits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProposalClassId,TypeBenefitsId,Match")] ProposalBenefit proposalBenefit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proposalBenefit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProposalClassId = new SelectList(db.ProposalClasses, "Id", "ClassCode", proposalBenefit.ProposalClassId);
            return View(proposalBenefit);
        }

        // GET: ProposalBenefits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalBenefit proposalBenefit = db.ProposalBenefits.Find(id);
            if (proposalBenefit == null)
            {
                return HttpNotFound();
            }
            return View(proposalBenefit);
        }

        // POST: ProposalBenefits/Delete/5
       
       
        public ActionResult DeleteConfirmed(int id,int ClassId )
        {
            ProposalBenefit proposalBenefit = db.ProposalBenefits.Find(id);
            db.ProposalBenefits.Remove(proposalBenefit);
            db.SaveChanges();
            return RedirectToAction("Index", new { Id = ClassId });
            //return RedirectToAction("Index");
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
