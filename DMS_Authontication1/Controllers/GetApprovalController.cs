using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class GetApprovalController : Controller
    {
        DMS_TESTEntities db;
        public GetApprovalController()
        {
            db = new DMS_TESTEntities();
        }
        public JsonResult GetReasons(string searchterm, string CardId)
        {
            var datalist = db.AcceptionReasons.ToList();

            if (searchterm != null)
            {
                //bool IsNoPayNoOverEdit = id.Split('-')[0].StartsWith("500") ? true : User.Identity.Name == "GodaKotb" ? true : false;

                if (!CardId.Split('-')[0].StartsWith("500") && !(User.Identity.Name == "GodaKotb"))
                {
                    datalist = db.AcceptionReasons.Where(x => x.Name.Contains(searchterm)&&x.Id!=1).ToList();
                }
                else
                {
                    datalist = db.AcceptionReasons.Where(x => x.Name.Contains(searchterm)).ToList();
                }
            }
            var modifieddate = datalist.Select(x => new
            {
                id = x.Id,
                text = x.Name
            });
            return Json(modifieddate, JsonRequestBehavior.AllowGet);

        }
        // GET: GetApproval
        [HttpGet]
        public ActionResult Index()
        {
            try
            {

                return View();
            }
            catch (Exception ex)
            {
                return Redirect("/Shared/Error");
            }



        }
        // GET: GetApproval
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                AcceptionViewModel Model = new AcceptionViewModel()
                {
                    GetProvidersList = db.Providers.ToList(),
                    GetReasonsList = db.AcceptionReasons.ToList(),
                    // GetEmployeeList = db.COMP_EMPLOYEES.ToList()
                };
                return View(Model);
            }
            catch (Exception ex)
            {
                return Redirect("/Shared/Error");
            }



        }
        public JsonResult Save(AcceptionViewModel data)
        {
            DateTime datenow = DateTime.Now.Date;
            int CompEmpId = db.Comp_Employees.Where(x => x.CARD_ID == data.CardId && x.INS_START_DATE <= datenow && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).Select(x => x.Id).FirstOrDefault();
            var ob = db.Acceptions.Where(x => x.CompEmployeesId == CompEmpId && x.ApprovalType == "Vip").FirstOrDefault();
            if (ob != null)
            {
                return Json("Card is already existed (Vip) ");

            }
            else
            {
                data.CreatedBy = User.Identity.Name;
                data.CreatedDate = DateTime.Now;

                var Object = new Acception
                {
                    CompEmployeesId = CompEmpId,
                    ProvidersId = data.ProvidersId,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    ApprovalType = data.ApprovalType,
                    AcceptionFlag = true
                };
                db.Acceptions.Add(Object);

                int result = db.SaveChanges();


                if (data.ApprovalType == "Normal")
                {


                    int AcceptionId = Object.Id;

                    CardAcceptionReason Objectt = new CardAcceptionReason();
                    for (int i = 0; i < data.Array.Length; i++)
                    {
                        var id = int.Parse(data.Array[i]);

                        Objectt.AcceptionId = AcceptionId;
                        Objectt.AcceptionReasonsId = id;


                        db.CardAcceptionReasons.Add(Objectt);
                        db.SaveChanges();

                    }

                }
                return Json(Object.Id);

            }
        }
        public JsonResult ApprovalList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            int lgSearch;
            int.TryParse(sSearch, out lgSearch);

            var result = new
            {
                sEcho = sEcho,
                aaData = db.Acceptions.Where(x => x.AcceptionFlag == true).AsEnumerable()
                .Where(r => sSearch != "" ? r.CreatedBy.Contains(sSearch) || r.Id == lgSearch : true).OrderByDescending(m => m.Id)
                .Select(l => new
                {
                    Id = l.Id,
                    CardId = db.Comp_Employees.Where(c => c.Id == l.CompEmployeesId).FirstOrDefault().CARD_ID,
                    CreatedDate = l.CreatedDate,
                    CreatedBy = l.CreatedBy
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.Acceptions.Where(x => x.AcceptionFlag == true).AsEnumerable()
               .Where(r => sSearch != "" ? r.CreatedBy.Contains(sSearch) || r.Id == lgSearch : true).Count(),
                iTotalDisplayRecords = db.Acceptions.Where(x => x.AcceptionFlag == true).AsEnumerable()
               .Where(r => sSearch != "" ? r.CreatedBy.Contains(sSearch) || r.Id == lgSearch : true).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult ApprovalDelete(int id)
        {
            try
            {
                var model = db.Acceptions.Where(i => i.Id == id).FirstOrDefault();
                if (model != null)
                {
                    if (model.ApprovalType == "Normal")
                    {
                        var CardAcceptions = db.CardAcceptionReasons.Where(c => c.AcceptionId == id).ToList();
                        db.CardAcceptionReasons.RemoveRange(CardAcceptions);
                        db.SaveChanges();
                    }
                    db.Acceptions.Remove(model);
                    db.SaveChanges();
                    return Json(new { ok = true, message = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        ok = false,
                        message = "No Data"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}