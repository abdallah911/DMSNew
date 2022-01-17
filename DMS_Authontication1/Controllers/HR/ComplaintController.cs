using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.HR
{
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class ComplaintController : Controller
    {
        #region Properities

        private DMS_TESTEntities db = new DMS_TESTEntities();
        ApplicationDbContext myEntities = new ApplicationDbContext();

        #endregion


        #region Methods

        [Authorize(Roles = "HR,HR_Admin,User")]
        [HttpGet]
        public ActionResult CreateDMSComplaint()
        {
            CreateDMSComplaintVM DMSComplaint = new CreateDMSComplaintVM();
            if (User.IsInRole("HR_Admin"))
            {
                var userid = User.Identity.GetUserId();
                var compines = db.HrAdminCompanies.Where(x => x.UserId == userid).Select(c => c.CompId).ToList();
                if (compines[0] == "All")
                {
                    var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
                else
                {
                    var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    (from comp in compines
                     join contCo in db.Contract_Comp
                     on int.Parse(comp) equals contCo.C_COMP_ID
                     select new
                     {
                         Code = contCo.C_COMP_ID,
                         Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
                     }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
            }

            else if (User.IsInRole("HR"))
            {
                var HrUserNamre = User.Identity.GetUserName();
                DMSComplaint.CompName = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            }
            else
            {
                var usr = User.Identity.GetUserId();
                DMSComplaint.CompName = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId.Split('-')[0];
            }
            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;

            var departs = db.Departments.ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList department = new SelectList(departs, "Id", "DepartName");
            ViewBag.Department = department;
            DMSComplaint.CreatedDate = DateTime.Now.ToString();
            return View(DMSComplaint);
        }

        [Authorize(Roles = "HR,HR_Admin,User")]
        [HttpGet]
        public ActionResult CreateProviderComplaint()
        {
            ProviderComplaintVM ProviderComplaint = new ProviderComplaintVM();
            if (User.IsInRole("HR_Admin"))
            {
                var userid = User.Identity.GetUserId();
                var compines = db.HrAdminCompanies.Where(x => x.UserId == userid).Select(c => c.CompId).ToList();
                if (compines[0] == "All")
                {
                    var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
                else
                {
                    var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    (from comp in compines
                     join contCo in db.Contract_Comp
                     on int.Parse(comp) equals contCo.C_COMP_ID
                     select new
                     {
                         Code = contCo.C_COMP_ID,
                         Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
                     }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
            }

            else if (User.IsInRole("HR"))
            {
                var HrUserNamre = User.Identity.GetUserName();
                ProviderComplaint.CompName = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            }
            else
            {
                var usr = User.Identity.GetUserId();
                ProviderComplaint.CompName = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId.Split('-')[0];
            }
            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;
            ProviderComplaint.ProblemDate = DateTime.Now.ToString();
            return View(ProviderComplaint);
        }

        #endregion


        #region Helper Methods

        [HttpPost]

        public JsonResult SaveComplaint(string CompName, string ComplaintReason, int department, string ComplaintDescription,
          string CreatedDate, string PhoneNumber, string employeeName = null)
        {
            DMSComplaint requestAddProvider = new DMSComplaint
            {
                CompName = CompName,
                Reason = ComplaintReason,
                DepartnentId = department,
                PhoneNumber = PhoneNumber,
                Description = ComplaintDescription,
                ProblemDate = CreatedDate == null ? DateTime.Now : DateTime.Parse(CreatedDate),
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserName(),
                Status = "Not Viewed Yet",
                SolveProblem = "لم يتم الرد بعد ",
                EmployeeName = employeeName
            };
            db.DMSComplaints.Add(requestAddProvider);
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = requestAddProvider.Id;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        [HttpPost]
        public JsonResult SearchComplaint(long searchId, string CompName)
        {
            var model = db.DMSComplaints.Where(r => r.Id == searchId && r.CompName == CompName).FirstOrDefault();
            if (model == null)
            {
                return new JsonResult { Data = new { modelreturn = 0, msg = "There's no data for that Request Code : " + searchId }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            CreateDMSComplaintVM requestAddProvider = new CreateDMSComplaintVM
            {
                CompName = model.CompName,
                ComplaintReason = model.Reason,
                Department = model.DepartnentId.ToString(),
                PhoneNumber = model.PhoneNumber,
                ComplaintDescription = model.Description,
                CreatedDate = model.ProblemDate.ToString(),
                Status = model.Status,
                SolveProblem = model.SolveProblem,
                EmployeeName = model.EmployeeName
            };

            return new JsonResult
            {
                Data = new { modelreturn = requestAddProvider, msg = "ok" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        [HttpPost]

        public JsonResult EditComplaint(long ReqestID, string CompName, string ComplaintReason, int department, string ComplaintDescription,
          string CreatedDate, string PhoneNumber, string employeeName = null)
        {
            var model = db.DMSComplaints.AsNoTracking().Where(r => r.Id == ReqestID && r.CompName == CompName).FirstOrDefault();

            if (model == null)
            {
                return new JsonResult { Data = new { respcode = 0, msg = "There's no data for that Complaint Code : " + ReqestID }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            DMSComplaint requestAddProvider = new DMSComplaint
            {
                Id = ReqestID,
                CompName = CompName,
                Reason = ComplaintReason,
                DepartnentId = department,
                PhoneNumber = PhoneNumber,
                Description = ComplaintDescription,
                ProblemDate = CreatedDate == null ? DateTime.Now : DateTime.Parse(CreatedDate),
                CreatedDate = model.CreatedDate,
                CreatedBy = model.CreatedBy,
                Status = model.Status,
                SolveProblem = model.SolveProblem,
                EmployeeName = employeeName,
                UpdatedBy = User.Identity.GetUserName(),
                UpdatedDate = DateTime.Now
            };
            db.Entry(requestAddProvider).State = EntityState.Modified;
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = requestAddProvider.Id;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]

        public JsonResult SaveProviderComplaint(string compName, string subject, int providerTypeId
            , int providerId, int branchId, string problem,
             string problemdate, string phoneNumber)
        {
            ProviderComplaint AddComplaint = new ProviderComplaint
            {
                CompName = compName,
                Subject = subject,
                ProviderTypeId = providerTypeId,
                ProviderId = providerId,
                BranchId = branchId,
                ProblemDate = problemdate == null ? DateTime.Now : DateTime.Parse(problemdate),
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserName(),
                Status = "Not Viewed Yet",
                SolveProblem = "لم يتم الرد بعد ",
                Problem = problem,
                PhoneNumber = phoneNumber
            };
            db.ProviderComplaints.Add(AddComplaint);
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = AddComplaint.Id;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        [HttpPost]
        public JsonResult SearchProviderComplaint(long searchId, string CompName)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            var model = db.ProviderComplaints.Where(r => r.Id == searchId && r.CompName == CompName)
                .Include(x=>x.ProviderTypeNew).Include(w=>w.Serv_Providers1)
                .Include(b=>b.SERV_PROVIDERS_NEW).Select(
                s =>new
                {
                    s.CompName,
                    s.BranchId,
                    s.ProviderTypeNew.PrvAName,
                    s.Serv_Providers1.PR_ANAME,
                    s.SERV_PROVIDERS_NEW.ADDRESS1,
                    s.Subject,
                    s.ProblemDate,
                    s.Problem,
                    s.Status,
                    s.PhoneNumber,
                    s.SolveProblem,

                }
                ).FirstOrDefault();
            if (model == null)
            {
                return new JsonResult { Data = new { modelreturn = 0, msg = "There's no data for that Request Code : " + searchId }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            int compid = int.Parse(model.CompName);
            var companyname = db.Contract_Comp.Where(c => c.C_COMP_ID == compid).FirstOrDefault().C_ANAME;
            ProviderComplaintEditVM AddProviderComplaint = new ProviderComplaintEditVM
            {
                CompName = model.CompName,
                Subject = model.Subject,
                ProblemDate = model.ProblemDate == null ? DateTime.Now.ToString() : model.ProblemDate.ToString(),
                Status = model.Status,
                SolveProblem = model.SolveProblem,
                Problem = model.Problem,
                PhoneNumber = model.PhoneNumber,
                ProviderTypeNew = model.PrvAName,
                Serv_Providers1 = model.PR_ANAME,
                SERV_PROVIDERS_NEW = model.ADDRESS1
            };
            return new JsonResult
            {
                Data = new { modelreturn = AddProviderComplaint, msg = "ok" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        [HttpPost]

        public JsonResult EditProviderComplaint(long ReqestID, string subject ,string problem,
             string problemdate, string phoneNumber)
        {
            var model = db.ProviderComplaints.AsNoTracking().Where(r => r.Id == ReqestID ).FirstOrDefault();

            if (model == null)
            {
                return new JsonResult { Data = new { respcode = 0, msg = "There's no data for that Complaint Code : " + ReqestID }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }

            ProviderComplaint AddComplaint = new ProviderComplaint
            {
                Id = ReqestID,
                CompName = model.CompName,
                Subject = subject,
                ProviderTypeId = model.ProviderTypeId,
                ProviderId = model.ProviderId,
                BranchId = model.BranchId,
                ProblemDate = problemdate == null ? DateTime.Now : DateTime.Parse(problemdate),
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserName(),
                Status = model.Status,
                SolveProblem = model.SolveProblem,
                Problem = problem,
                PhoneNumber = phoneNumber,
                UpdatedBy = User.Identity.GetUserName(),
                UpdatedDate = DateTime.Now
            };
            db.Entry(AddComplaint).State = EntityState.Modified;
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = AddComplaint.Id;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetProviderList(string id)
        {

            myEntities.Configuration.ProxyCreationEnabled = false;

            int Number = int.Parse(id);
            var Providers = db.Serv_Providers1.Where(m => m.PRV_TYPE == Number).ToList();
            SelectList ProviderListlist = new SelectList(Providers, "Id", "PR_ANAME");

            return Json(ProviderListlist, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetBranchList(string id)
        {

            myEntities.Configuration.ProxyCreationEnabled = false;

            int Number = int.Parse(id);
            var prCode = db.Serv_Providers1.Where(m => m.Id == Number).FirstOrDefault().PR_CODE;
            var Branch = db.SERV_PROVIDERS_NEW.Where(m => m.PR_CODE == prCode).ToList();
            SelectList BranchListlist = new SelectList(Branch, "Id", "ADDRESS1");

            return Json(BranchListlist, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}