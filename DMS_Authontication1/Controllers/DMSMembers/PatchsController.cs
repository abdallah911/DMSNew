using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DMS_Authontication1.Controllers.DMSMembers
{
    public class PatchsController : Controller
    {
        private DMS_TESTEntities db;
        // private UsersEntities db1;
        public PatchsController()
        {
            db = new DMS_TESTEntities();

        }

        // GET: Patchs
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public JsonResult Provider(int id)
        {

            if (id == 8)
            {
                var data = db.Comp_Employees.Where(x => x.C_COMP_ID == 20017)
                    .Select(l => new
                    {
                        EmpName = l.EMP_ENAME
                    }).ToList();//Employees
                return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else if (id == 9)
            {
                var data = db.Contract_Comp.ToList();//Company
                return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            int Code = Convert.ToInt32(id);
            var provider = db.Serv_Providers1.Where(x => x.PRV_TYPE == Code).ToList();
            // SelectList Providerlist = new SelectList(provider, "PRV_TYPE", "PR_ENAME");
            return new JsonResult { Data = provider, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //return Json(new { data = "test", JsonRequestBehavior.AllowGet });
        }
        public JsonResult PostId(int? Id)
        {
            var result = db.Roshitas.Where(x => x.PatchId == Id)
                        .Join(db.Comp_Employees, x => x.CardId, y => y.CARD_ID, (x, y) => new { x, y })
                         .Select(l => new
                         {
                             CardId = l.x.CardId,
                             PatchId = l.x.PatchId,
                             RoshitaId = l.x.Id,
                             CreateData = l.x.CreatedDate.ToString(),
                             CreateBy = l.x.CreatedBy,
                             CompanyPayment = l.x.CompanyPayment,
                             Name = l.y.EMP_ENAME
                         }).ToList();




            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //return Json(new { data = "test", JsonRequestBehavior.AllowGet });
        }
        public JsonResult Patches()
        {

            var result = db.Patchs.Select(x => new
            {

                Id = x.Id,
                AmountOfClaimSubmitted = x.AmountOfClaimSubmitted,
                ServiceProvider = x.ServiceProvider,
                ReceivedDate = x.ReceivedDate.ToString(),
                ClaimDate = x.ClaimDate.ToString(),
                ManualCount = x.ManualAmount,
                SystemCount = x.SystemCount,
                ManualAmount = x.ManualAmount,
                SystemAmount = x.SystemAmount,
                ClaimType = x.ClaimType,
                Status = x.Status,
                Groups = x.Groups,
                ServicesOfTheMonth = x.ServicesOfTheMonth,
                AcceptingDate = x.AcceptingDate.ToString(),
               // PatchId = x.PatchId,
                StartFrom = x.StartFrom.ToString(),
                EndAt = x.EndAt.ToString(),
                CreatedBy = x.CreatedBy.ToString(),


            }).ToList();

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //return Json(new { data = "test", JsonRequestBehavior.AllowGet });
        }


        public JsonResult SavePatch(Patch patch)
        {
           // var PatchId = patch.PatchId;

            var Date = db.Roshitas.Where(x => x.PatchId == patch.Id).OrderBy(x => x.CreatedDate).ToList();
            var startdate = Date.First();
            var endate = Date.Last();

            var startt = startdate.CreatedDate;
            var endd = endate.CreatedDate;

            patch.StartFrom = startt;
            patch.EndAt = endd;

            db.Patchs.Add(patch);
            int result = db.SaveChanges();

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //return Json(new { data = "test", JsonRequestBehavior.AllowGet });
        }
        public JsonResult PostDate(DateTime start, DateTime end)
        {

            var result = db.Patchs.Where(x => x.StartFrom >= start && x.EndAt <= end).Select(x => new
            {
                Id = x.Id,
                AmountOfClaimSubmitted = x.AmountOfClaimSubmitted,
                ServiceProvider = x.ServiceProvider,
                ReceivedDate = x.ReceivedDate.ToString(),
                ClaimDate = x.ClaimDate.ToString(),
                ManualCount = x.ManualAmount,
                SystemCount = x.SystemCount,
                ManualAmount = x.ManualAmount,
                SystemAmount = x.SystemAmount,
                ClaimType = x.ClaimType,
                Status = x.Status,
                Groups = x.Groups,
                ServicesOfTheMonth = x.ServicesOfTheMonth,
                AcceptingDate = x.AcceptingDate.ToString(),
              //  PatchId = x.PatchId,
                StartFrom = x.StartFrom.ToString(),
                EndAt = x.EndAt.ToString(),
                CreatedBy = x.CreatedBy.ToString(),

            }).ToList();
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
    }
}