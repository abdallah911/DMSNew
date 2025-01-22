using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using DMS_TEST.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR,HR_Admin")]
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class ReportsController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext myEntities = new ApplicationDbContext();
        private ApplicationUserManager _userManager;


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ReportsController()
        {
            db = new DMS_TESTEntities();
        }

        #region Reports View
        public ActionResult Index()
        {
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
                    var companyname = (from comp in compines
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
                if (User.Identity.Name == "sofico-hr")
                {
                    return View("~/Views/Reports/ConsumptionHR.cshtml");
                }

                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;

                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }
        public ActionResult Inquiries()
        {
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
                    var companyname = (from comp in compines
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
                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }
        public ActionResult StaffConsumption()
        {
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
                    var companyname = (from comp in compines
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
                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }

        public ActionResult Consumption()
        {
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
                    var companyname = (from comp in compines
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
                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }

        public ActionResult ConsumptionPremium()
        {
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
                    var companyname = (from comp in compines
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
                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;

                return View();

            }
        }


        public ActionResult ConsumptionHR()
        {
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
                    var companyname = (from comp in compines
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

                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                return View();
            }
        }
        public ActionResult ConsumptionHR2()
        {
            bool IsIos = false;
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
                    IsIos = true;
                }
                else
                {
                    var companyname = (from comp in compines
                                       join contCo in db.Contract_Comp
                                       on int.Parse(comp) equals contCo.C_COMP_ID
                                       select new
                                       {
                                           Code = contCo.C_COMP_ID,
                                           Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
                                       }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                    IsIos = companyname.Where(x => x.Name.Contains("500")).FirstOrDefault() != null ? true : false;
                }
                ViewBag.IsIos = IsIos;
                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    ViewBag.IsIos = compa.Contains("500") ? true : false;
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }

        public ActionResult HrClaims()
        {
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
                    var companyname = (from comp in compines
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

                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/HrClaimsPremium.cshtml");
            }
        }




        public JsonResult PreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, int ClaimType, long? ApprovalNo,
            string sSearch = "", string CompanyNumber = "", string From = "", string To = "", string CardId = "", string Type = "")
        {
            //all
            if (ClaimType == 0)
            {
                DateTime dateFrom = Convert.ToDateTime(From);
                DateTime dateTo = Convert.ToDateTime(To);
                var result1 = new
                {
                    sEcho = sEcho,
                    aaData = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .Select(l => new
                   {
                       Id = l.Id,
                       Oracle_Id = ((l.Oracle_Id == null || l.Oracle_Id == 0) ? Convert.ToString("2" + l.CreatedDate.Value.ToString("ddMMyy") + l.Id) : Convert.ToString(l.Oracle_Id)),
                       CardId = l.CardId,
                       CompanyPercent = l.CompanyPercent,
                       TotalValue = l.TotalValue,
                       Manager = l.Manager,
                       CreatedDate = l.CreatedDate,
                       CreatedBy = l.CreatedBy
                   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count(),
                    iTotalDisplayRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count()
                };
                return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            //pharmacy
            else if (ClaimType == 1)
            {
                DateTime dateFrom = Convert.ToDateTime(From);
                DateTime dateTo = Convert.ToDateTime(To);
                var result1 = new
                {
                    sEcho = sEcho,
                    aaData = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true)
                   && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   !r.Manager.Contains("Lab") && !r.Manager.Contains("Ray") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .Select(l => new
                   {
                       Id = l.Id,
                       Oracle_Id = ((l.Oracle_Id == null || l.Oracle_Id == 0) ? Convert.ToString("2" + l.CreatedDate.Value.ToString("ddMMyy") + l.Id) : Convert.ToString(l.Oracle_Id)),
                       CardId = l.CardId,
                       CompanyPercent = l.CompanyPercent,
                       TotalValue = l.TotalValue,
                       Manager = l.Manager,
                       CreatedDate = l.CreatedDate,
                       CreatedBy = l.CreatedBy
                   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   !r.Manager.Contains("Lab") && !r.Manager.Contains("Ray") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count(),
                    iTotalDisplayRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   !r.Manager.Contains("Lab") && !r.Manager.Contains("Ray") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count()
                };
                return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            //lab 
            else if (ClaimType == 2)
            {
                DateTime dateFrom = Convert.ToDateTime(From);
                DateTime dateTo = Convert.ToDateTime(To);
                var result1 = new
                {
                    sEcho = sEcho,
                    aaData = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop") && r.Manager.Contains("Lab")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .Select(l => new
                   {
                       Id = l.Id,
                       Oracle_Id = ((l.Oracle_Id == null || l.Oracle_Id == 0) ? Convert.ToString("2" + l.CreatedDate.Value.ToString("ddMMyy") + l.Id) : Convert.ToString(l.Oracle_Id)),
                       CardId = l.CardId,
                       CompanyPercent = l.CompanyPercent,
                       TotalValue = l.TotalValue,
                       Manager = l.Manager,
                       CreatedDate = l.CreatedDate,
                       CreatedBy = l.CreatedBy
                   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop") && r.Manager.Contains("Lab")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count(),
                    iTotalDisplayRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop") && r.Manager.Contains("Lab")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count()
                };
                return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            //Ray 
            else if (ClaimType == 3)
            {
                DateTime dateFrom = Convert.ToDateTime(From);
                DateTime dateTo = Convert.ToDateTime(To);
                var result1 = new
                {
                    sEcho = sEcho,
                    aaData = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop") && r.Manager.Contains("Ray")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .Select(l => new
                   {
                       Id = l.Id,
                       Oracle_Id = ((l.Oracle_Id == null || l.Oracle_Id == 0) ? Convert.ToString("2" + l.CreatedDate.Value.ToString("ddMMyy") + l.Id) : Convert.ToString(l.Oracle_Id)),
                       CardId = l.CardId,
                       CompanyPercent = l.CompanyPercent,
                       TotalValue = l.TotalValue,
                       Manager = l.Manager,
                       CreatedDate = l.CreatedDate,
                       CreatedBy = l.CreatedBy
                   }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop") && r.Manager.Contains("Ray")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count(),
                    iTotalDisplayRecords = db.Roshitas.OrderByDescending(m => m.CreatedDate)
                   .Where(r => (sSearch != "" ? (r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch)
                   ) : true)
                   && (CardId != "" ? r.CardId.Contains(CardId) : true) && (ApprovalNo != null ? r.Oracle_Id == ApprovalNo : true) && (Type != "" ? r.Manager.Contains(Type) : true)
                   && (CompanyNumber != "" ? r.CardId.Contains(CompanyNumber) : true) && !r.Manager.Contains("Stop") && r.Manager.Contains("Ray")
                   && !r.Manager.Contains("Doctor_Daily") && !r.Manager.Contains("Doctor_Chronic") &&
                   (DbFunctions.TruncateTime(r.CreatedDate) >= dateFrom && DbFunctions.TruncateTime(r.CreatedDate) <= dateTo))
                   .AsEnumerable()
                   .ToList().Count()
                };
                return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            #region defult
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Roshitas.Where(x => !x.Manager.Contains("Lab") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date).OrderByDescending(m => m.Id)
                .Select(l => new Roshita
                {
                    Oracle_Id = (l.Oracle_Id == null || l.Oracle_Id == 0) ? Convert.ToInt64("2" + l.CreatedDate.Value.ToString("ddMMyy") + l.Id) : l.Oracle_Id,
                    Id = l.Id,
                    // Oracle_Id = l.Oracle_Id,
                    CardId = l.CardId,
                    CompanyPercent = l.CompanyPercent,
                    TotalValue = Math.Round(l.TotalValue.Value, 3),
                    Manager = l.Manager,
                    CreatedDate = l.CreatedDate,
                    CreatedBy = l.CreatedBy
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Lab") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Count(),
                iTotalDisplayRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Lab") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            #endregion



        }
        public ActionResult ControlPenelReport(string id)
        {
            try
            {
                var data = new Roshita();
                int Approval;//= Convert.ToInt32(id.Substring(7));
                long Id = Convert.ToInt64(id);

                if (id.StartsWith("2") && id.Length >= 14)
                {
                    Approval = Convert.ToInt32(id.Substring(7));
                    data = db.Roshitas.Where(r => r.Id == Approval).FirstOrDefault();
                    if (data == null)
                    {
                        data = db.Roshitas.Where(r => r.Oracle_Id == Id).FirstOrDefault();
                        Approval = Convert.ToInt32(data.Id);
                    }

                }
                else
                {
                    //long Id = Convert.ToInt64(id);
                    data = db.Roshitas.Where(r => r.Oracle_Id == Id).FirstOrDefault();
                    Approval = Convert.ToInt32(data.Id);

                }
                if (data.Manager.Contains("Lab"))
                {
                    var patient = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId /*&& c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now*/).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    var Company = db.Contract_Comp.Where(c => c.C_COMP_ID == patient.C_COMP_ID).FirstOrDefault();
                    List<PrescriptionRoshitaDignosi> diagnoises = db.PrescriptionRoshitaDignosis.Where(x => x.RositaId == data.Id).ToList();
                    string diagnoisesString = data.Speciality == "Empty" ? " " : data.Speciality + '-';
                    diagnoisesString += String.Join(",", diagnoises.Select(p => p.DiagnoiseName).ToArray());
                    string accptionlistString = "";
                    RoshitaAcception _RoshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == Approval).FirstOrDefault();
                    if (_RoshitaAcception != null)
                    {
                        var acception = db.Acceptions.Where(x => x.Id == _RoshitaAcception.AcceptionId).FirstOrDefault();
                        if (acception != null && acception.ApprovalType == "Vip")
                        {
                            accptionlistString = "Vip";
                        }
                        else
                        {
                            //var accptionlist = db.Acceptions.Where(x => x.CompEmployeesId == patient.Id && x.AcceptionFlag == false && x.UpdatedDate == DateTime.Today).OrderByDescending(d => d.Id).FirstOrDefault();
                            List<CardAcceptionReason> reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == acception.Id).ToList();
                            accptionlistString = String.Join(",", reasons.Select(p => p.AcceptionReason.Name.Trim()).ToArray());

                        }
                    }
                    else
                    {
                        accptionlistString = "No Exeption";
                    }
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "labs.rpt"));
                    var y = db.RoshitaDetails.Where(r => r.RoshitaID == data.Id && r.IsDealed == true)
                     .Select(d => new
                     {
                         MedicienName = d.MedicienName,
                         Amount = d.Amount,
                         PaymentGroup = d.PaymentGroup
                     }).ToList();
                    rd.SetDataSource(y);
                    if (string.IsNullOrEmpty(patient.EMP_ENAME) || patient.EMP_ENAME == "NULL")
                    {
                        if (string.IsNullOrEmpty(patient.EMP_ANAME) || patient.EMP_ANAME == "NULL")
                        {
                            rd.SetParameterValue("PatientName", "Unnamed");
                        }
                        else
                        {
                            rd.SetParameterValue("PatientName", patient.EMP_ANAME);
                        }
                    }
                    else
                    {
                        rd.SetParameterValue("PatientName", patient.EMP_ENAME);
                    }
                    data.RoshetaType = "Lab";

                rd.SetParameterValue("CompType", patient.COMP_ID);
                    rd.SetParameterValue("Type", data.RoshetaType);
                    rd.SetParameterValue("Pharmacy", data.CreatedBy);
                    rd.SetParameterValue("Approval", id);
                    rd.SetParameterValue("PhoneNumber", data.PhoneNumber);
                    rd.SetParameterValue("CompanyName", Company.C_ENAME);
                    rd.SetParameterValue("CardId", data.CardId);
                    if (data.Diagnose1 != null && data.Diagnose1 != "Empty")
                        rd.SetParameterValue("Notes", data.Diagnose1);
                    else rd.SetParameterValue("Notes", "");
                    rd.SetParameterValue("Diagnosis", diagnoisesString);
                    rd.SetParameterValue("Permission", accptionlistString);
                    rd.SetParameterValue("TotalValue", data.TotalValue);
                    rd.SetParameterValue("OverInsurance", data.OverInsurance);
                    rd.SetParameterValue("PersonPayment", data.PersonPayment);
                    rd.SetParameterValue("CompanyPayment", data.CompanyPayment);
                    rd.SetParameterValue("Cash", data.Cash);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        DateTime ApprovalDate = Convert.ToDateTime(data.CreatedDate);
                        return File(stream, "application/pfd", ApprovalDate.ToString("ddMMyyyy") + Approval.ToString() + "Lab" + ".pdf");
                    }
                    catch
                    {
                        throw;
                    }
                }
                else if (data.Manager.Contains("Ray"))
                {
                    var patient = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId /*&& c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now*/).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    var Company = db.Contract_Comp.Where(c => c.C_COMP_ID == patient.C_COMP_ID).FirstOrDefault();
                    List<PrescriptionRoshitaDignosi> diagnoises = db.PrescriptionRoshitaDignosis.Where(x => x.RositaId == data.Id).ToList();
                    string diagnoisesString = data.Speciality == "Empty" ? " " : data.Speciality + '-';
                    diagnoisesString += String.Join(",", diagnoises.Select(p => p.DiagnoiseName).ToArray());
                    string accptionlistString = "";
                    RoshitaAcception _RoshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == Approval).FirstOrDefault();
                    if (_RoshitaAcception != null)
                    {
                        var acception = db.Acceptions.Where(x => x.Id == _RoshitaAcception.AcceptionId).FirstOrDefault();
                        if (acception != null && acception.ApprovalType == "Vip")
                        {
                            accptionlistString = "Vip";
                        }
                        else
                        {
                            //var accptionlist = db.Acceptions.Where(x => x.CompEmployeesId == patient.Id && x.AcceptionFlag == false && x.UpdatedDate == DateTime.Today).OrderByDescending(d => d.Id).FirstOrDefault();
                            List<CardAcceptionReason> reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == acception.Id).ToList();
                            accptionlistString = String.Join(",", reasons.Select(p => p.AcceptionReason.Name.Trim()).ToArray());

                        }
                    }
                    else
                    {
                        accptionlistString = "No Exeption";
                    }
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Rays.rpt"));
                    // rd.Subreports[0].SetDataSource(db.RoshitaDetails.Where(r=>r.RoshitaID==Approval).ToList());
                    var y = db.RoshitaDetails.Where(r => r.RoshitaID == data.Id && r.IsDealed == true)
                     .Select(d => new
                     {
                         MedicienName = d.MedicienName,
                         Amount = d.Amount,
                         PaymentGroup = d.PaymentGroup
                     }).ToList();
                    rd.SetDataSource(y);
                    if (string.IsNullOrEmpty(patient.EMP_ENAME) || patient.EMP_ENAME == "NULL")
                    {
                        if (string.IsNullOrEmpty(patient.EMP_ANAME) || patient.EMP_ANAME == "NULL")
                        {
                            rd.SetParameterValue("PatientName", "Unnamed");
                        }
                        else
                        {
                            rd.SetParameterValue("PatientName", patient.EMP_ANAME);
                        }
                    }
                    else
                    {
                        rd.SetParameterValue("PatientName", patient.EMP_ENAME);
                    }
                    data.RoshetaType = "Ray";
                rd.SetParameterValue("CompType", patient.COMP_ID);
                    rd.SetParameterValue("Type", data.RoshetaType);
                    rd.SetParameterValue("Pharmacy", data.CreatedBy);
                    rd.SetParameterValue("Approval", id);
                    rd.SetParameterValue("PhoneNumber", data.PhoneNumber);
                    rd.SetParameterValue("CompanyName", Company.C_ENAME);
                    rd.SetParameterValue("CardId", data.CardId);
                    if (data.Diagnose1 != null && data.Diagnose1 != "Empty")
                        rd.SetParameterValue("Notes", data.Diagnose1);
                    else rd.SetParameterValue("Notes", "");
                    rd.SetParameterValue("Diagnosis", diagnoisesString);
                    rd.SetParameterValue("Permission", accptionlistString);
                    rd.SetParameterValue("TotalValue", data.TotalValue);
                    rd.SetParameterValue("OverInsurance", data.OverInsurance);
                    rd.SetParameterValue("PersonPayment", data.PersonPayment);
                    rd.SetParameterValue("CompanyPayment", data.CompanyPayment);
                    rd.SetParameterValue("Cash", data.Cash);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        DateTime ApprovalDate = Convert.ToDateTime(data.CreatedDate);
                        return File(stream, "application/pfd", Approval.ToString() + "Ray" + ".pdf");
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {


                    // based on type
                    DateTime datecompare = new DateTime(data.CreatedDate.Value.Year, data.CreatedDate.Value.Month,
                        data.CreatedDate.Value.Day);
                    var patient = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId && c.INS_START_DATE <= datecompare
                    && c.INS_END_DATE >= datecompare).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    var Company = db.Contract_Comp.Where(c => c.C_COMP_ID == patient.C_COMP_ID).FirstOrDefault();
                    List<PrescriptionRoshitaDignosi> diagnoises = db.PrescriptionRoshitaDignosis.Where(x => x.RositaId == data.Id).ToList();
                    string diagnoisesString = data.Speciality == "Empty" ? " " : data.Speciality + '-';
                    diagnoisesString += String.Join(",", diagnoises.Select(p => p.DiagnoiseName).ToArray());
                    string accptionlistString = "";
                    var DataService1 = new Comp_Customized_D_D();
                    var med_card = new Med_Card();
                    int? NoOver = 0, NoPay = 0, AcceptionId = 0;
                    double CellingPert;
                    var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == patient.C_COMP_ID && c.CONTRACT_NO == patient.CONTRACT_NO && c.SER_SERV == data.RoshetaType && c.CARD_ID == patient.CARD_ID).FirstOrDefault();
                    if (DataService == null)
                    {
                        DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == patient.C_COMP_ID && c.CLASS_CODE == patient.CLASS_CODE && c.CONTRACT_NO == patient.CONTRACT_NO && c.SER_SERV == data.RoshetaType).FirstOrDefault();
                    }
                    RoshitaAcception _RoshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == Approval).FirstOrDefault();
                    if (_RoshitaAcception != null)
                    {
                        var acception = db.Acceptions.Where(x => x.Id == _RoshitaAcception.AcceptionId).FirstOrDefault();
                        AcceptionId = _RoshitaAcception.AcceptionId;
                        if (acception != null && acception.ApprovalType == "Vip")
                        {
                            accptionlistString = "Vip";
                        }
                        else
                        {
                            //var accptionlist = db.Acceptions.Where(x => x.CompEmployeesId == patient.Id && x.AcceptionFlag == false && x.UpdatedDate == DateTime.Today).OrderByDescending(d => d.Id).FirstOrDefault();
                            List<CardAcceptionReason> reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == acception.Id).ToList();
                            accptionlistString = String.Join(",", reasons.Select(p => p.AcceptionReason.Name.Trim()).ToArray());

                        }
                    }
                    else
                    {
                        accptionlistString = "No Exeption";
                    }


                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RoshitaReport.rpt"));

                    var y = db.RoshitaDetails.Where(r => r.RoshitaID == data.Id && r.IsDealed == true)
                       .Join(db.MedicineDatas, r => r.MedicienCode, m => m.M_CODE, (r, m) => new { r, m })
                       .AsEnumerable()
                    .Select(d => new RoshitaDetailsMedicineDataReportViewModel
                    {
                        MedicienName = d.r.MedicienName,//name
                        LIC_TYPE = d.m.LIC_TYPE,//form
                        UNIT_NO = Convert.ToInt32(d.m.PACK_SIZE.Value),//size
                        TotalUnits = Convert.ToInt32(d.r.TotalUnits),//count
                        Amount = d.r.Amount,//
                        PaymentGroup = d.r.PaymentGroup,//type
                        MedicineNoPay = d.r.MedicineNoPay
                    }).ToList();
                    rd.SetDataSource(y);
                    if (string.IsNullOrEmpty(patient.EMP_ENAME) || patient.EMP_ENAME == "NULL")
                    {
                        if (string.IsNullOrEmpty(patient.EMP_ANAME) || patient.EMP_ANAME == "NULL")
                        {
                            rd.SetParameterValue("PatientName", "Unnamed");
                        }
                        else
                        {
                            rd.SetParameterValue("PatientName", patient.EMP_ANAME);
                        }
                    }
                    else
                    {
                        rd.SetParameterValue("PatientName", patient.EMP_ENAME);
                    }
                    if (data.RoshetaType == "11601")
                    {
                        if (data.Manager == "Pharmacy_Doctor")
                        {
                            data.RoshetaType = "Pharmacy_Doctor";
                        }
                        else
                        {
                            data.RoshetaType = "Daily";
                        }


                    }
                    if (data.RoshetaType == "11603")
                    {
                        data.RoshetaType = "Monthly";
                    }
                    if (data.RoshetaType == "11602")
                    {
                        data.RoshetaType = "Pharmacy_Chronic";

                        med_card = db.Med_Card.Where(m => m.CARD_NO == data.CardId).FirstOrDefault();
                        var RoshitaNoOverNoPays = db.RoshitaNoOverNoPays.Where(m => m.RositaId == data.Id).FirstOrDefault();
                        if (RoshitaNoOverNoPays != null)
                        {
                            NoOver = RoshitaNoOverNoPays.NoOver == null ? 0 : RoshitaNoOverNoPays.NoOver;
                            NoPay = RoshitaNoOverNoPays.NoPay == null ? 0 : RoshitaNoOverNoPays.NoPay;
                        }
                        else
                        {
                            NoOver = med_card.NO_OVER == null ? 0 : med_card.NO_OVER;
                            NoPay = med_card.NO_PAY == null ? 0 : med_card.NO_PAY;
                        }

                        diagnoisesString = data.Speciality == "Empty" ? " " : data.Speciality + '-';
                        diagnoisesString += String.Join(",", med_card.TASHKHES_01);

                    }

                    if (DataService != null)
                    {
                        CellingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;

                    }
                    else if (DataService1 != null)
                    {
                        CellingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
                    }
                    else
                    {
                        CellingPert = 100;
                    }
                    rd.SetParameterValue("hasApprovalCode", AcceptionId);

                    rd.SetParameterValue("CompType", patient.COMP_ID);
                    rd.SetParameterValue("pay", NoPay);
                    rd.SetParameterValue("over", NoOver);
                    rd.SetParameterValue("perc", CellingPert);
                    rd.SetParameterValue("Type", data.RoshetaType);
                    rd.SetParameterValue("Pharmacy", data.CreatedBy);
                    //rd.SetParameterValue("Approval", Convert.ToDateTime(data.CreatedDate).ToString("ddMMyyyy") + Approval.ToString());
                    rd.SetParameterValue("Approval", id);
                    if (data.PhoneNumber != null && data.PhoneNumber != "Now")
                        rd.SetParameterValue("PhoneNumber", data.PhoneNumber);
                    else rd.SetParameterValue("PhoneNumber", "");
                    rd.SetParameterValue("CompanyName", Company.C_ENAME);
                    rd.SetParameterValue("CardId", data.CardId);
                    if (data.Diagnose1 != null && data.Diagnose1 != "Empty")
                        rd.SetParameterValue("Notes", data.Diagnose1);
                    else rd.SetParameterValue("Notes", "");
                    rd.SetParameterValue("Diagnosis", diagnoisesString);
                    rd.SetParameterValue("Permission", accptionlistString);
                    rd.SetParameterValue("TotalValue", data.TotalValue);
                    rd.SetParameterValue("OverInsurance", data.OverInsurance);
                    rd.SetParameterValue("PersonPayment", data.PersonPayment);
                    rd.SetParameterValue("CompanyPayment", data.CompanyPayment);
                    rd.SetParameterValue("Cash", data.Cash);
                    rd.SetParameterValue("CreatedDate", data.CreatedDate.Value);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    DateTime ApprovalDate = Convert.ToDateTime(data.CreatedDate);
                    return File(stream, "application/pfd", id.ToString() + ".pdf");

                }
            }
            catch (Exception ex)
            {
                // throw ex;
                return View("~/Views/Shared/Error.cshtml");

            }
        }
        #endregion


        #region Print Reports
        /// <summary>
        /// For print MedicalServices report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="Claimfrom"></param>
        /// <param name="Claimto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServNum"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintMedicalServices(string Regfrom, string Regto, string Serfrom, string Serto
            , string Claimfrom, string Claimto, string CompNum, string ServNum, string ButtType)
        {
            Int64 ClaimStart, ClaimEnd, CompNumber, ServNum1, ServNum2;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            ClaimStart = Claimfrom == string.Empty ? 0 : Convert.ToInt64(Claimfrom);
            ClaimEnd = Claimto == string.Empty ? 999999999999999999 : Convert.ToInt64(Claimto);
            CompNumber = Convert.ToInt64(CompNum);

            ServNum1 = ServNum == string.Empty ? 0 : Convert.ToInt64(ServNum);
            ServNum2 = ServNum == string.Empty ? 9999999999999 : Convert.ToInt64(ServNum);

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "MedicalServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEob.rpt"));
            }
            else if (ButtType == "OtherServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEobService.rpt"));
            }
            else if (ButtType == "SerOutMediAuth")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEobServiceOut.rpt"));

            }


            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("seda1", SerDateFrom);
            rd.SetParameterValue("seda2", SerDateTo);
            rd.SetParameterValue("crda1", RegDateFrom);
            rd.SetParameterValue("crda2", RegDateTo);
            rd.SetParameterValue("clno1", ClaimStart);
            rd.SetParameterValue("clno2", ClaimEnd);
            rd.SetParameterValue("comp", CompNumber);
            rd.SetParameterValue("prv1", ServNum1);
            rd.SetParameterValue("prv2", ServNum2);


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();

                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "MedicalServices.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Print Hr Claim Xsl
        /* window.open('/Reports/PrintXlxConsumption?From=' + $("#From").val() +
             '&&To=' + $("#To").val() + '&&ApprovalNo=' + $("#PharmacyApprovalNo").val() +
             '&&CardId=' + $("#PharmacyApprovalNo").val() + '&&TypePrint=' + $("#ddlType").val() +
             '&&CompanyNumber=' + $("#CopmanyNumber").val());*/
        public ActionResult PrintHrClaimXsl(string From, string To, string ApprovalNo, string CardId,
                                            string TypePrint, string CompanyNumber)
        {
            Int64 ClaimStart, ClaimEnd, CompNumber;
            DateTime dat1, dat2;

            ClaimStart = ApprovalNo == string.Empty ? 0 : Convert.ToInt64(ApprovalNo);
            ClaimEnd = ApprovalNo == string.Empty ? 999999999999999999 : Convert.ToInt64(ApprovalNo);
            CompNumber = Convert.ToInt64(CompanyNumber);

            dat1 = From == string.Empty ? new DateTime(2018, 1, 1) : (Convert.ToDateTime(From)).Date;
            dat2 = To == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(To)).Date;

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "HrClaimsReport.rpt"));


            //  rd.SetDatabaseLogon("APP", "12369");

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@from", dat1);
            rd.SetParameterValue("@to", dat2);
            rd.SetParameterValue("@ClaimStart", ClaimStart);
            rd.SetParameterValue("@ClaimEnd", ClaimEnd);
            rd.SetParameterValue("@typ", TypePrint);
            rd.SetParameterValue("@CardId", CardId);
            rd.SetParameterValue("@cmp", CompanyNumber);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/xls", "AllClaims" + DateTime.Now.ToString("ddMMyyyy") + ".xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// For Print MedicalServices Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="Claimfrom"></param>
        /// <param name="Claimto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServNum"></param>
        /// <returns>Report as EXCEL</returns>
        public ActionResult PrintXlxMedicalServices(string Regfrom, string Regto, string Serfrom, string Serto
            , string Claimfrom, string Claimto, string CompNum, string ServNum, string ButtType)
        {
            Int64 ClaimStart, ClaimEnd, CompNumber, ServNum1, ServNum2;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            ClaimStart = Claimfrom == string.Empty ? 0 : Convert.ToInt64(Claimfrom);
            ClaimEnd = Claimto == string.Empty ? 999999999999999999 : Convert.ToInt64(Claimto);
            //CompNumber = Convert.ToInt64(CompNum);

            ServNum1 = ServNum == string.Empty ? 0 : Convert.ToInt64(ServNum);
            ServNum2 = ServNum == string.Empty ? 9999999999999 : Convert.ToInt64(ServNum);

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "MedicalServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEob.rpt"));
            }
            else if (ButtType == "OtherServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEobService.rpt"));
            }
            else if (ButtType == "SerOutMediAuth")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEobServiceOut.rpt"));
            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("seda1", SerDateFrom);
            rd.SetParameterValue("seda2", SerDateTo);
            rd.SetParameterValue("crda1", RegDateFrom);
            rd.SetParameterValue("crda2", RegDateTo);
            rd.SetParameterValue("clno1", ClaimStart);
            rd.SetParameterValue("clno2", ClaimEnd);
            rd.SetParameterValue("comp", CompNum);
            rd.SetParameterValue("prv1", ServNum1);
            rd.SetParameterValue("prv2", ServNum2);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "MedicalServices.xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// For print Staff Consumption report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="CardNum"></param>
        /// <param name="ButtType"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintEmployees(string Regfrom, string Regto
            , string CompNum, string CardNum, string ButtType)
        {
            Int32 CompNumber;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "Active")
            {
                if (CompNumber == 10362 || CompNumber == 500144)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpInternalCode.rpt"));
                else
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmp.rpt"));
            }
            else if (ButtType == "Closed")
            {
                if (CompNumber == 10362 || CompNumber == 500144)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpYInternalCode.rpt"));
                else
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpY.rpt"));
            }


            rd.SetDatabaseLogon("APP", "12369");

            rd.SetDatabaseLogon("APP", "12369");
            rd.SetParameterValue("comp", CompNumber);
            rd.SetParameterValue("rel1", 0);
            rd.SetParameterValue("rel2", 99999999);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "Employees.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// For Print Staff Consumption Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="CardNum"></param>
        /// <param name="ButtType"></param>
        /// <returns>Report as EXCEL</returns>
        public ActionResult PrintXlxEmployees(string Regfrom, string Regto
            , string CompNum, string CardNum, string ButtType)
        {

            Int32 CompNumber;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "Active")
            {
                if (CompNumber == 10362 || CompNumber == 500144)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpInternalCode.rpt"));
                else
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmp.rpt"));
            }
            else if (ButtType == "Closed")
            {
                if (CompNumber == 10362 || CompNumber == 500144)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpYInternalCode.rpt"));
                else
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpY.rpt"));
            }


            rd.SetDatabaseLogon("APP", "12369");

            rd.SetDatabaseLogon("APP", "12369");
            rd.SetParameterValue("comp", CompNumber);
            rd.SetParameterValue("rel1", 0);
            rd.SetParameterValue("rel2", 99999999);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                // 
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Employees.xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// For print Staff Consumption report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServiceType"></param>
        /// <param name="CardNum"></param>
        /// <param name="Family"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintEmployeesConsumption(string Regfrom, string Regto
            , string CompNum, string ServiceType, string CardNum, string Family)
        {
            Int32 CompNumber, grp1, grp2;
            string card;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);
            card = CardNum;

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(ServiceType))
            {
                case 1:
                    grp1 = 1016;
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHr.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 2:
                    grp1 = 1014;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 3:
                    grp1 = 1009;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 4:
                    grp1 = 116;
                    grp2 = 1013;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp2);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 5:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrPrvIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                default:
                    return View();

            }

        }


        /// <summary>
        /// For Print Staff Consumption Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServiceType"></param>
        /// <param name="CardNum"></param>
        /// <param name="Family"></param>
        /// <returns>Report as Excel </returns>
        public ActionResult PrintXlxEmployeesConsumption(string Regfrom, string Regto
            , string CompNum, string ServiceType, string CardNum, string Family)
        {

            Int32 CompNumber, grp1, grp2;
            string card;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);
            card = CardNum;

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();
            switch (Convert.ToInt32(ServiceType))
            {
                case 1:
                    grp1 = 1016;
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHr.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 2:
                    grp1 = 1014;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 3:
                    grp1 = 1009;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 4:
                    grp1 = 116;
                    grp2 = 1013;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp2);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 5:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrPrvIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Close();
                        rd.Dispose();
                        GC.Collect();
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                default:
                    return View();


            }
        }



        /// <summary>
        /// For print Consumption report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="CompanyFrom"></param>
        /// <param name="CompanyTo"></param>
        /// <param name="ClassFrom"></param>
        /// <param name="ClassTo"></param>
        /// <param name="CardFrom"></param>
        /// <param name="CardTo"></param>
        /// <param name="ContractNumber"></param>
        /// <param name="RepotType"></param>
        /// <param name="larg"></param>
        /// <param name="smal"></param>
        /// <param name="Percent"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintConsumption(string Regfrom, string Regto, string Serfrom, string Serto
            , string CompanyFrom, string CompanyTo, string ClassFrom, string ClassTo, string CardFrom
            , string CardTo, string ContractNumber, string RepotType, string larg, string smal, string Percent)
        {
            Int64 CompanyStart, CompanyEnd,
                ContrNumber, per, lrg, sml, prv1 = 0, prv2 = 999999999999999999;
            string CardStart, CardEnd, ClassStart, ClassEnd;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            CompanyStart = CompanyFrom == string.Empty ? 0 : Convert.ToInt64(CompanyFrom);
            CompanyEnd = CompanyTo == string.Empty ? 999999999 : Convert.ToInt64(CompanyTo);
            ClassStart = ClassFrom == string.Empty ? " " : ClassFrom;
            ClassEnd = ClassTo == string.Empty ? "zzzzz" : ClassTo;
            CardStart = CardFrom == string.Empty ? " " : CardFrom;
            CardEnd = CardTo == string.Empty ? "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz" : CardTo;
            ContrNumber = ContractNumber == string.Empty ? 0 : Convert.ToInt64(ContractNumber);



            per = Percent == string.Empty ? 0 : Convert.ToInt64(Percent);
            lrg = (larg == string.Empty || larg == "0") ? 999999999999999999 : Convert.ToInt64(larg);
            sml = smal == string.Empty ? 0 : Convert.ToInt64(smal);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;


            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 1:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub1InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub1.rpt"));

                    break;

                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2.rpt"));

                    break;

                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub4.rpt"));

                    break;

                case 4:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub7InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub7.rpt"));

                    break;

                case 5:

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub10.rpt"));

                    break;

                case 6:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub11.rpt"));

                    break;

                case 7:

                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub12InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub12.rpt"));

                    break;

                case 8:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub13.rpt"));

                    break;

                case 9:

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub14New.rpt"));

                    break;

                case 10:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "IndemnityCheckInternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "IndemnityCheck.rpt"));

                    break;


                case 11:

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2New.rpt"));

                    break;

                default:
                    return View();


            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("crda1", RegDateFrom);
            rd.SetParameterValue("crda2", RegDateTo);
            rd.SetParameterValue("srda1", SerDateFrom);
            rd.SetParameterValue("srda2", SerDateTo);
            rd.SetParameterValue("comp1", CompanyStart);
            rd.SetParameterValue("comp2", CompanyEnd);
            rd.SetParameterValue("crd1", CardStart);
            rd.SetParameterValue("crd2", CardEnd);
            rd.SetParameterValue("cls1", ClassStart);
            rd.SetParameterValue("cls2", ClassEnd);
            rd.SetParameterValue("prv1", prv1);
            rd.SetParameterValue("prv2", prv2);
            rd.SetParameterValue("lrg", lrg);
            rd.SetParameterValue("sml", sml);
            rd.SetParameterValue("UserName", "");

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "Consumption.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// For Print Consumption Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param> 
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="CompanyFrom"></param>
        /// <param name="CompanyTo"></param>
        /// <param name="ClassFrom"></param>
        /// <param name="ClassTo"></param>
        /// <param name="CardFrom"></param>
        /// <param name="CardTo"></param>
        /// <param name="ContractNumber"></param>
        /// <param name="RepotType"></param>
        /// <param name="larg"></param>
        /// <param name="smal"></param>
        /// <param name="Percent"></param>
        /// <returns>Report as EXCEL</returns>
        public ActionResult PrintXlxConsumption(string Regfrom, string Regto, string Serfrom, string Serto
            , string CompanyFrom, string CompanyTo, string ClassFrom, string ClassTo, string CardFrom
            , string CardTo, string ContractNumber, string RepotType, string larg, string smal, string Percent)
        {
            Int64 CompanyStart, CompanyEnd,
                ContrNumber, per, lrg, sml, prv1 = 0, prv2 = 999999999999999999;
            string CardStart, CardEnd, ClassStart, ClassEnd;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            CompanyStart = CompanyFrom == string.Empty ? 0 : Convert.ToInt64(CompanyFrom);
            CompanyEnd = CompanyTo == string.Empty ? 999999999 : Convert.ToInt64(CompanyTo);
            ClassStart = ClassFrom == string.Empty ? "0" : ClassFrom;
            ClassEnd = ClassTo == string.Empty ? "zzzzz" : ClassTo;
            CardStart = CardFrom == string.Empty ? "0" : CardFrom;
            CardEnd = CardTo == string.Empty ? "9999999999999999999999" : CardTo;
            ContrNumber = ContractNumber == string.Empty ? 0 : Convert.ToInt64(ContractNumber);



            per = Percent == string.Empty ? 0 : Convert.ToInt64(Percent);
            lrg = larg == string.Empty ? 999999999999999999 : Convert.ToInt64(larg);
            sml = smal == string.Empty ? 0 : Convert.ToInt64(smal);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;


            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 1:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub1InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub1.rpt"));

                    break;

                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2.rpt"));

                    break;

                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub4.rpt"));

                    break;

                case 4:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub7InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub7.rpt"));

                    break;
                case 5:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub10.rpt"));

                    break;
                case 6:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub11.rpt"));

                    break;

                case 7:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub12InternalCode.rpt"));
                    else

                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub12.rpt"));

                    break;

                case 8:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub13.rpt"));

                    break;
                case 9:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub14New.rpt"));

                    break;
                case 10:

                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "IndemnityCheckInternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "IndemnityCheck.rpt"));


                    break;
                case 11:

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2New.rpt"));

                    break;


                default:
                    return View();
            }
            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("crda1", RegDateFrom);
            rd.SetParameterValue("crda2", RegDateTo);
            rd.SetParameterValue("srda1", SerDateFrom);
            rd.SetParameterValue("srda2", SerDateTo);
            rd.SetParameterValue("comp1", CompanyStart);
            rd.SetParameterValue("comp2", CompanyEnd);
            rd.SetParameterValue("crd1", CardStart);
            rd.SetParameterValue("crd2", CardEnd);
            rd.SetParameterValue("cls1", ClassStart);
            rd.SetParameterValue("cls2", ClassEnd);
            rd.SetParameterValue("cls1", ClassStart);
            rd.SetParameterValue("cls2", ClassEnd);
            rd.SetParameterValue("prv1", prv1);
            rd.SetParameterValue("prv2", prv2);
            rd.SetParameterValue("lrg", lrg);
            rd.SetParameterValue("sml", sml);
            rd.SetParameterValue("UserName", "");

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}