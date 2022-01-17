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
        public JsonResult GetReasons(string searchterm)
        {
            var datalist = db.AcceptionReasons.ToList();

            if (searchterm != null)
            {
                datalist = db.AcceptionReasons.Where(x => x.Name.Contains(searchterm)).ToList();
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

            int CompEmpId = db.Comp_Employees.Where(x => x.CARD_ID == data.CardId && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).Select(x => x.Id).FirstOrDefault();
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
    }
}