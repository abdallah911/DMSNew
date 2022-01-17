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
    public class ProposalBasicDatasController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();

        // GET: ProposalBasicDatas
        public ActionResult Index()
        {
            Session["pro_id"] = null;
            Session["Pro_num"] = null;
            Session["Pro_Type"] = null;


            return View(db.ProposalBasicDatas.ToList());
        }
        public JsonResult PreseptionAdminCount(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "", string CompanyName = "", string CopmanyId = "", string From = "", string To = "", string ddlType = "", string ProposalId = "")
        {

            bool falag = ddlType == "" ? true : ddlType == "false" ? false : true;


            DateTime DateTo = DateTime.TryParse(To, out DateTo) ? DateTo : DateTime.Now;
            DateTime DateFrom = DateTime.TryParse(From, out DateFrom) ? DateFrom : DateTo.AddMonths(-1);
            var count = db.ProposalBasicDatas.Where(x => x.CreatedDate >= DateFrom.Date && x.CreatedDate <= DateTo.Date && x.CompId.ToString().Contains(CopmanyId) && x.Id.ToString().Contains(ProposalId)  && ddlType == "" ? true : x.TypeProposal == falag);
        var listDClasses = db.ProposalBasicDatas.Where(x => x.CreatedDate >= DateFrom.Date && x.CreatedDate <= DateTo.Date && x.CompId.ToString().Contains(CopmanyId) && x.Id.ToString().Contains(ProposalId) && ddlType == "" ? true : x.TypeProposal == falag).Join(// outer sequence 
                      db.ProposalClasses.ToList(),  // inner sequence 
                      ProposalBasicDatas => ProposalBasicDatas.Id+ ProposalBasicDatas.NumProposal,    // outerKeySelector
                      ProposalClasses => ProposalClasses.ProposalId+ ProposalClasses.NumProposal,  // innerKeySelector
                      (ProposalBasicDatas, ProposalClasses) => new  // result selector
                      {
                          TotalPremium =  (ProposalClasses.NumMale+ ProposalClasses.NumFemael)* ProposalClasses.Premium,
                          NumChronic = ProposalClasses.NumChronic,
                          TotalChronic = ProposalClasses.TotalChronic
                      }).ToList();

            var Adminresult = new { 
                TotalPremium= listDClasses.Sum(n=>n.TotalPremium),
                TotalChronic = listDClasses.Sum(n=>n.TotalChronic),
                NumChronic = listDClasses.Sum(n=>n.NumChronic),
                Count = count
            };

            return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        public JsonResult PreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "", string CompanyName = "", string CopmanyId = "", string From ="", string To = "", string ddlType = "", string ProposalId = "")
        {
           
               DateTime  DateTo = DateTime.TryParse(To,out DateTo )? DateTo:DateTime.Now ;
            DateTime DateFrom = DateTime.TryParse(From, out DateFrom) ? DateFrom : DateTo.AddMonths(-1);
           bool falag=  ddlType == "" ? true: ddlType== "false" ? false:true;


           var result = new
            {
                sEcho = sEcho,
                aaData = db.ProposalBasicDatas.AsEnumerable().Where(x => x.CreatedDate.Date >= DateFrom.Date&&x.CreatedDate.Date <= DateTo.Date && x.CompId.ToString().Contains(CopmanyId) && x.Id.ToString().Contains(ProposalId) && ddlType=="" ?true: x.TypeProposal== falag)
                .AsEnumerable()
                .Where(r => sSearch != "" ? r.Id.ToString().Contains(sSearch) || r.CompId.ToString().Contains(sSearch) || r.CompName.ToString().Contains(sSearch) || r.status.Contains(sSearch) : true).OrderByDescending(m => m.Id)
                .Select(l => new ProposalBasicDataVM
                {
                    status = l.status,
                    Id = l.Id,
                   
                    TypeProposal =  (l.TypeProposal==true? "New": "Renewal"),
                    CompName = l.CompName,
                    CompId = l.CompId,
                    ContractNo = l.ContractNo,
                    NumProposal=l.NumProposal,
                    CreatedDate = l.CreatedDate,
                    CreatedBy = l.CreatedBy
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.ProposalBasicDatas.AsEnumerable()
                               .Where(r => sSearch != "" ? r.Id.ToString().Contains(sSearch) || r.CompId.ToString().Contains(sSearch) || r.CompName.ToString().Contains(sSearch) || r.status.Contains(sSearch) : true).Count(),
                iTotalDisplayRecords = db.ProposalBasicDatas.AsEnumerable()
                               .Where(r => sSearch != "" ? r.Id.ToString().Contains(sSearch) || r.CompId.ToString().Contains(sSearch) || r.CompName.ToString().Contains(sSearch) || r.status.Contains(sSearch) : true).Where(x => x.CreatedDate.Date >= DateFrom.Date && x.CreatedDate.Date <= DateTo.Date).Count()
            };
            #region comt
            //if(result.aaData.Count()==0)
            //{
            //     result = new
            //    {
            //        sEcho = sEcho,
            //        aaData = db.ProposalBasicDatas
            //   .AsEnumerable()
            //   .Where(r => sSearch != "" ? r.Id.ToString().Contains(sSearch) || r.CompId.ToString().Contains(sSearch) || r.CompName.ToString().Contains(sSearch) || r.status.Contains(sSearch) : true).OrderByDescending(m => m.Id)
            //   .Select(l => new ProposalBasicDataVM
            //   {
            //       status = l.status,
            //       Id = l.Id,

            //       TypeProposal = (l.TypeProposal == true ? "New" : "Renewal"),
            //       CompName = l.CompName,
            //       CompId = l.CompId,
            //       ContractNo = l.ContractNo,
            //       NumProposal = l.NumProposal,
            //       CreatedDate = l.CreatedDate,
            //       CreatedBy = l.CreatedBy
            //   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

            //        iTotalRecords = db.ProposalBasicDatas.AsEnumerable()
            //                  .Where(r => sSearch != "" ? r.Id.ToString().Contains(sSearch) || r.CompId.ToString().Contains(sSearch) || r.CompName.ToString().Contains(sSearch) || r.status.Contains(sSearch) : true).Count(),
            //        iTotalDisplayRecords = db.ProposalBasicDatas.AsEnumerable()
            //                  .Where(r => sSearch != "" ? r.Id.ToString().Contains(sSearch) || r.CompId.ToString().Contains(sSearch) || r.CompName.ToString().Contains(sSearch) || r.status.Contains(sSearch) : true).Where(x => x.CreatedDate >= DateFrom.Date && x.CreatedDate <= DateTo.Date).Count()
            //    };
            //}
            #endregion

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        // GET: ProposalBasicDatas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalBasicData proposalBasicData = db.ProposalBasicDatas.Find(id);
            if (proposalBasicData == null)
            {
                return HttpNotFound();
            }
            var proposalBasicDatatotal = db.ProposalBasicDatas.Include(x => db.ProposalRenewalSummaries.Where(n => n.ProposalId == x.Id)).Where(vv=>vv.Id==id).FirstOrDefault();
            //.Include(cc => db.ProposalClasses.Include(b => b.ProposalBenefits).Where(n => n.ProposalId == cc.Id && n.NumProposal == cc.NumProposal))
            return View(proposalBasicData);
        }

        // GET: ProposalBasicDatas/Create
        public ActionResult Create()
        {
            ViewBag.CompId = new SelectList(db.Contract_Comp.OrderBy(x=>x.C_ENAME), "C_COMP_ID", "C_ENAME");
            return View();
        }
        public JsonResult GetStateList(int id)
        {
              SelectList StateListlist = new SelectList(db.Contract_Data.Where(x=>x.C_COMP_ID==id).OrderByDescending(x => x.CONTRACT_NO), "CONTRACT_NO", "C_COMP_ID");
            var compname = db.Contract_Comp.Where(x=>x.C_COMP_ID==id).Select(x=>x.C_ENAME).FirstOrDefault();

            return Json( new { StateListlist= StateListlist,Name= compname }, JsonRequestBehavior.AllowGet);

        }
        // POST: ProposalBasicDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TypeProposal,InsuredBefore,ExClient,CompName,CompId,ContractNo,InsuredOtherComp,NumProposal,Conclusion,status,OtherCompName,CreatedBy,ClassType,CreatedDate,UpdateBy,UpdateDate")] ProposalBasicData proposalBasicData)
        {
            if (ModelState.IsValid)
            {//ViewBag.ClassType=new List
                proposalBasicData.NumProposal = 1;
                proposalBasicData.CreatedBy = User.Identity.Name;
                proposalBasicData.CreatedDate = DateTime.Now;
                proposalBasicData.status = "Created";
                db.ProposalBasicDatas.Add(proposalBasicData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            

            return View(proposalBasicData);
        }

        // GET: ProposalBasicDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalBasicData proposalBasicData = db.ProposalBasicDatas.Find(id);
            if (proposalBasicData == null)
            {
                return HttpNotFound();
            }
            return View(proposalBasicData);
        }
        public ActionResult RedirectToServeces(int? Id,int? Num)
        {

            return Redirect($"~/ProposalClasses/Index?Id={  Id}&Num={Num}");
        }
        // POST: ProposalBasicDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TypeProposal,InsuredBefore,ExClient,CompName,CompId,ContractNo,InsuredOtherComp,NumProposal,Conclusion,status,OtherCompName,CreatedBy,CreatedDate,UpdateBy,UpdateDate")] ProposalBasicData proposalBasicData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proposalBasicData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(proposalBasicData);
        }

        // GET: ProposalBasicDatas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProposalBasicData proposalBasicData = db.ProposalBasicDatas.Find(id);
            if (proposalBasicData == null)
            {
                return HttpNotFound();
            }
            return View(proposalBasicData);
        }

        // POST: ProposalBasicDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProposalBasicData proposalBasicData = db.ProposalBasicDatas.Find(id);
            db.ProposalBasicDatas.Remove(proposalBasicData);
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
