using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using DMS_TEST.ViewModel;
using DMS_Authontication1.ViewModel;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Net;
using System.Data.Entity;
using Newtonsoft.Json;
using DMS_Authontication1;
using DMS_Authontication1.ViewModel.PharmacyAdmin;
using Microsoft.AspNet.Identity;
using System.Data.Entity.SqlServer;
using System.Text;
using System.Security.Cryptography;

namespace DMS_TEST.Controllers
{

    public class PharmacyController : Controller
    {
        // GET: Pharmacy
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public PharmacyController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();
        }
        string Id;

        #region Pharmacy
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult Pharmacy()
        {
            NotificationHub objNotifHub = new NotificationHub();
            objNotifHub.SendMessages();
            Session.Clear();
            ViewBag.ddlSpeciality = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
            if (User.IsInRole("Admin"))
            {
                var context = new ApplicationDbContext();
                ViewBag.ddlUsers = new SelectList(context.Users.Where(x => x.Type == "1" || x.Type == "2" || x.Type == "Admin").ToList(), "UserName", "UserName", User.Identity.Name);
            }
            return View();
        }
        [Authorize(Roles = "Pharmacy")]
        public ActionResult PharmacyPage()
        {
            NotificationHub objNotifHub = new NotificationHub();
            objNotifHub.SendMessages();
            Session.Clear();
            ViewBag.ddlSpeciality = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
            if (User.IsInRole("Admin"))
            {
                var context = new ApplicationDbContext();
                ViewBag.ddlUsers = new SelectList(context.Users.Where(x => x.Type == "1" || x.Type == "2" || x.Type == "Admin").ToList(), "UserName", "UserName", User.Identity.Name);
            }
            return View();
        }
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult CardCode()
        {
            return View();
        }

        public JsonResult GetCardCode(string id, string calimNumber)
        {

            var model = db.CardCodes.Where(x => x.Code == calimNumber && x.CardId == id && x.IsActive && !x.IsUsed).FirstOrDefault();
            if (model != null)
            {
                return new JsonResult { Data = "0", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                return new JsonResult { Data = "1", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        public JsonResult HaveClaim(string id)
        {

            var model = db.CardCodes.Where(x => x.CardId == id && x.IsActive && !x.IsUsed).FirstOrDefault();
            if (model != null)
            {
                return new JsonResult { Data = "0", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                return new JsonResult { Data = "1", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        public JsonResult AddCardCode(string id)
        {
            DateTime datenow = DateTime.Now.Date;
            var IsActive = db.Comp_Employees.Where(x => x.CARD_ID == id && x.INS_START_DATE <= datenow
            && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            if (IsActive == null)
            {
                return new JsonResult { Data = "لا يمكن اضافة كود لهذا الكارت", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            Random generator = new Random();
            string cardCode = generator.Next(0, 1000000).ToString("D6");
            var model = db.CardCodes.Where(x => x.Code == cardCode && x.CardId == id).FirstOrDefault();
            if (model != null)
            {
                model.IsActive = true;
                model.IsUsed = false;
                db.Entry(model).State = EntityState.Modified;
            }
            else
            {
                var Employeecode = new CardCode
                {
                    Code = cardCode,
                    CardId = id,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    IsActive = true,
                    IsUsed = false,
                };
                db.CardCodes.Add(Employeecode);
            }
            db.SaveChanges();
            return new JsonResult { Data = cardCode, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            //return Json(cardCode);
        }
        public JsonResult AddCardPharmacy(string id)
        {
            var carduse = db.CardUseds.Where(c => c.CardId == id).FirstOrDefault();
            if (carduse != null)
            {
                db.CardUseds.Remove(carduse);
                db.SaveChanges();
            }
            CardUsed cardUsed = new CardUsed
            {
                CardId = id,
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.Name
            };
            db.CardUseds.Add(cardUsed);
            db.SaveChanges();
            var company = id.Split('-')[0];
            var userID = User.Identity.GetUserId();
            var isPermission = db.UserCompanyPermissions.Where(u => u.UserId == userID && u.IsActive != false).Select(c => c.CompId).ToList();
            if (isPermission == null || isPermission.Count == 0)
            {
                var emp = db.fn_searchCompEmployees(id).ToList();
                return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                if (isPermission.Contains(company))
                {
                    var emp = db.fn_searchCompEmployees(id).ToList();
                    return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = "null", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        public JsonResult AddCard(string id)
        {
            //var carduse = db.CardUseds.Where(c => c.CardId == id).FirstOrDefault();
            //if (carduse != null)
            //{
            //    db.CardUseds.Remove(carduse);
            //    db.SaveChanges();
            //}
            //CardUsed cardUsed = new CardUsed
            //{
            //    CardId = id,
            //    CreatedDate = DateTime.Now,
            //    CreatedBy = User.Identity.Name
            //};
            //db.CardUseds.Add(cardUsed);
            //db.SaveChanges();
            var company = id.Split('-')[0];
            var userID = User.Identity.GetUserId();
            var isPermission = db.UserCompanyPermissions.Where(u => u.UserId == userID && u.IsActive != false).Select(c => c.CompId).ToList();
            if (isPermission == null || isPermission.Count == 0)
            {
                var emp = db.fn_searchCompEmployees(id).ToList();
                return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                if (isPermission.Contains(company))
                {
                    var emp = db.fn_searchCompEmployees(id).ToList();
                    return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = "null", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        public JsonResult AddCardForAddDoctor(string id)
        {
            var company = id.Split('-')[0];
            var userID = User.Identity.GetUserId();
            var isPermission = db.UserCompanyPermissions.Where(u => u.UserId == userID && u.IsActive != false).Select(c => c.CompId).ToList();
            if (isPermission == null || isPermission.Count == 0)
            {
                var emp = db.fn_searchCompEmployees(id).ToList();
                if (emp.Count() > 0)
                {
                    var diffOfDates = emp.ElementAt(0).INS_END_DATE.Value - DateTime.Now;
                    if (diffOfDates.Days >= 28)
                    {
                        return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                        if (empnext.Count() > 0)
                            return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        else
                            return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                else
                {
                    var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                    return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                if (isPermission.Contains(company))
                {
                    var emp = db.fn_searchCompEmployees(id).ToList();
                    if (emp.Count() > 0)
                    {
                        var diffOfDates = emp.ElementAt(0).INS_END_DATE.Value - emp.ElementAt(0).INS_START_DATE.Value;
                        if (diffOfDates.Days >= 28)
                        {
                            return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                        else
                        {
                            var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                            if (empnext.Count() > 0)
                                return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                            else
                                return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                    }
                    else
                    {
                        var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                        return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                return new JsonResult { Data = "null", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        public JsonResult AddCardForAddChronic(string id)
        {
            try
            {
                var company = id.Split('-')[0];
                var userID = User.Identity.GetUserId();
                var isPermission = db.UserCompanyPermissions.Where(u => u.UserId == userID && u.IsActive != false).Select(c => c.CompId).ToList();
                if (isPermission == null || isPermission.Count == 0)
                {
                    var emp = db.fn_searchCompEmployees(id).ToList();
                    if (emp.Count() > 0)
                    {
                        System.IO.File.AppendAllText(Server.MapPath("~/Registerlog.txt"), DateTime.Now.ToString() + ": " +
                        System.Environment.NewLine + emp.FirstOrDefault() + System.Environment.NewLine);
                        return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                        System.IO.File.AppendAllText(Server.MapPath("~/Registerlog.txt"), DateTime.Now.ToString() + ": " +
                        System.Environment.NewLine + empnext.FirstOrDefault() + System.Environment.NewLine);
                        return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                else
                {
                    if (isPermission.Contains(company))
                    {
                        var emp = db.fn_searchCompEmployees(id).ToList();
                        if (emp.Count() > 0)
                        {
                            System.IO.File.AppendAllText(Server.MapPath("~/Registerlog.txt"), DateTime.Now.ToString() + ": " +
                        System.Environment.NewLine + emp.FirstOrDefault() + System.Environment.NewLine);
                            return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                        else
                        {
                            var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                            System.IO.File.AppendAllText(Server.MapPath("~/Registerlog.txt"), DateTime.Now.ToString() + ": " +
                        System.Environment.NewLine + empnext.FirstOrDefault() + System.Environment.NewLine);
                            return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                    }
                    return new JsonResult { Data = "null", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Server.MapPath("~/Registerlog.txt"), DateTime.Now.ToString() + ": " +
                        System.Environment.NewLine + ex + System.Environment.NewLine);
                return new JsonResult { Data = ex.InnerException, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        public JsonResult AddCardForTest(string id)
        {
            try
            {


                var company = id.Split('-')[0];
                var userID = User.Identity.GetUserId();
                var isPermission = db.UserCompanyPermissions.Where(u => u.UserId == userID && u.IsActive != false).Select(c => c.CompId).ToList();
                if (isPermission == null || isPermission.Count == 0)
                {
                    var emp = db.fn_searchCompEmployees(id).ToList();
                    if (emp.Count() > 0)
                    {
                        return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                        return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                else
                {
                    if (isPermission.Contains(company))
                    {
                        var emp = db.fn_searchCompEmployees(id).ToList();
                        if (emp.Count() > 0)
                        {
                            return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                        else
                        {
                            var empnext = db.fn_searchCompEmployeesForNextContract(id).ToList();
                            return new JsonResult { Data = empnext, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                    }
                    return new JsonResult { Data = "null", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.InnerException, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        public JsonResult GetSecondContract(string id)
        {
            int check;
            var employee = db.Comp_Employees.Where(c => c.CARD_ID == id).OrderByDescending(c => c.CONTRACT_NO).FirstOrDefault();
            if (employee == null)
            {
                check = 0;
                return new JsonResult { Data = new { check = check, Employee = 0 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                check = 1;
                return new JsonResult { Data = new { check = check, Employee = employee }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        [HttpPost]
        public JsonResult GetCompActivation(string id, string CardId)
        {
            int CompId = Convert.ToInt32(id);
            try
            {
                Contract_Comp contractComp = db.Contract_Comp.Where(x => x.C_COMP_ID == CompId).FirstOrDefault();
                string emp = "";
                if (contractComp != null)
                {
                    emp = contractComp.ACTIVE;
                }
                else
                {
                    return Json(new { ok = false, data = "Expire", message = "Company is not existed" }, JsonRequestBehavior.AllowGet);
                }
                var CurrentDate = DateTime.Now.Date;
                var empCardTerminationFlag = db.Comp_Employees.Where(x => x.CARD_ID == CardId && x.INS_START_DATE <= CurrentDate && x.INS_END_DATE >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

                if (empCardTerminationFlag != null)
                {
                    if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "N")
                    {
                        return Json(new { ok = true, data = "Yes", message = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && ((empCardTerminationFlag.TERMINATE_DATE > DateTime.Now) || empCardTerminationFlag.TERMINATE_DATE == null))
                    {
                        return Json(new { ok = true, data = "Yes", message = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "H"/* && ((empCardTerminationFlag.TERMINATE_DATE >= DateTime.Now) || empCardTerminationFlag.TERMINATE_DATE == null)*/)
                    {
                        return Json(new { ok = false, data = "Hold", message = "تم استهلاك النسبه المقررة للحد الاقصي للتغطية برجاء الرجوع الي ادارة الموارد البشريه الخاصه بسياداتكم " }, JsonRequestBehavior.AllowGet);
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE < DateTime.Now)
                    {
                        return Json(new { ok = false, data = "Expire", message = "كارت مغلق Expired Card" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { ok = false, data = "Expire", message = "Expired Card" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var CompTerminationFlag = db.Contract_Data.Where(x => x.C_COMP_ID == CompId && x.DATE_FROM <= DateTime.Now && x.DATE_TO >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (CompTerminationFlag != null)
                    {
                        return Json(new { ok = false, data = "Expire", message = "Card is not existed" }, JsonRequestBehavior.AllowGet);

                    }
                }
                return Json(new { ok = false, data = "Expire", message = "Expired Company" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //Medicines
        public ActionResult GetAllDiagnose_In_Speciality(string ID)
        {
            int id = Convert.ToInt32(ID);
            db = new DMS_TESTEntities();
            var x = from u in db.Diagnosis
                    where u.SPEC_ID == id
                    select new
                    {
                        u.DIAG_CODE,
                        u.DIAG_ANAME
                    };
            return Json("x", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getDiag(string id)
        {
            int SpecialId = Convert.ToInt32(id);
            var Diagnoises = db.Diagnosis.Where(x => x.SPEC_ID == SpecialId && x.ACTIVE == "1")
                .Select(l => new
                {
                    Code = l.DIAG_CODE,
                    Name = l.DIAG_ANAME

                })
            .ToList();
            return Json(Diagnoises, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult HaveApproval(string id)
        {
            try
            {
                DateTime datenow = DateTime.Now.Date;
                //Default is pharmacy=3
                int EmpId = db.Comp_Employees.Where(x => x.CARD_ID == id && x.INS_START_DATE <= datenow && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault().Id;
                var accption = db.Acceptions.Where(x => x.CompEmployeesId == EmpId && x.AcceptionFlag == true && x.ProvidersId == 3).OrderByDescending(d => d.Id).FirstOrDefault();
                if (accption == null)
                {
                    return Json(new { ok = false, message = "No" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { ok = true, message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult HaveChronic(string id)
        {
            try
            {
                DateTime datenow = DateTime.Now.Date;
                var Rosita = db.Roshitas.Where(r => r.CardId == id && r.Manager == "Doctor_Chronic").Where(x => x.RoshetaType == "11603" || x.RoshetaType == "11602").OrderByDescending(c => c.CreatedDate).FirstOrDefault();
                if (Rosita != null)
                {
                    var data = db.RoshitaDetails.Where(x => x.RoshitaID == Rosita.Id && x.IsDealed == false && x.TotalUnits != 0)
                       .Join(db.Med_Medicine, d => d.MedicienCode, m => m.MED_CODE, (d, m) => new { d, m })
                       .Join(db.MedicineDatas, med => med.m.MED_CODE, md => md.M_CODE, (med, md) => new { med, md })
                       .Where(l => l.med.m.CARD_NO == id && l.med.m.ACTIVE != "N" && l.md.ACTIVE != "N" && (l.med.m.StartDate <= datenow || l.med.m.StartDate == null))
                       .Distinct().ToList();
                    if (data.Count == 0)
                        return Json(new { ok = false, message = "No" }, JsonRequestBehavior.AllowGet);
                    return Json(new { ok = true, message = "Ok" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { ok = false, message = "No" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult HaveDoctor(string id)
        {
            try
            {

                List<Roshita> roshitaDoctor = new List<Roshita>();
                var date = DateTime.Now.AddDays(-14);
                List<DoctorContainerViewModel> data = new List<DoctorContainerViewModel>();
                roshitaDoctor = db.Roshitas.Where(r => r.CardId == id && r.Manager == "Doctor_Daily" && r.CreatedDate >= date).OrderByDescending(x => x.Id).ToList();

                foreach (var item in roshitaDoctor)
                {
                    data.AddRange(db.RoshitaDetails
                .Join(db.MedicineDatas,
                      d => d.MedicienCode, m => m.M_CODE,
                      (d, m) => new { d, m })
                .Where(l => l.d.RoshitaID == item.Id && l.d.IsDealed == false)
                .Select(l => new DoctorContainerViewModel
                {
                    Id = l.d.Id,
                    MedicienCode = l.d.MedicienCode,
                    IsDealed = l.d.IsDealed
                })
                .ToList());
                }
                if (data.Count() > 0)
                {
                    return Json(new { ok = true, message = "Ok" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { ok = false, message = "No" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetCompName(string id)
        {
            try
            {
                var emp = db.Comp_Employees.Where(x => x.CARD_ID == id).FirstOrDefault();
                int s = Convert.ToInt32(TempData["ID"]);
                var state = db.Contract_Comp.Where(x => x.C_COMP_ID == emp.C_COMP_ID).FirstOrDefault();
                return Json(new { ok = true, data = state, message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetLimit(string id)
        {
            try
            {
                var emp = db.Comp_Employees.Where(x => x.CARD_ID == id).FirstOrDefault();
                var limit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                //string Last21="21-" + (DateTime.Now.Day > 21 ? DateTime.Now.Month.ToString() : (DateTime.Now.Month-1==0?"12-": (DateTime.Now.Month-1).ToString()))+ (DateTime.Now.Month-1 == 0&&DateTime.Now.Day<21 ? (DateTime.Now.Year-1).ToString() : (DateTime.Now.Year).ToString()
                string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                List<Roshita> AcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= Last21Time && (r.Manager.Contains("Monthly") || r.Manager.Contains("Pharmacy_Chronic"))).ToList();
                double AcumlatorAmount = 0;
                foreach (var item in AcumlatorList)
                {
                    AcumlatorAmount += item.CompanyPayment;
                }
                if (Convert.ToInt32(limit.INSURANCE_MONTH) > 0)
                {
                    limit.INSURANCE_MONTH -= AcumlatorAmount;
                }
                return Json(new { ok = true, limit = limit, message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CellingAmount(string id, string ServiceCode)
        {
            double PersonNoPay = 0;
            string Message = "";
            ServiceCode = ServiceCode == "11604" ? "11601" : ServiceCode;
            int _IntServiceCode = Convert.ToInt32(ServiceCode);
            string _CompId = id.Split('-')[0];
            string MainService = ServiceCode.Substring(0, 3);
            var CurrentDate = DateTime.Now.Date;
            Comp_Employees emp = new Comp_Employees();
            emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= CurrentDate && c.INS_END_DATE >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

            if (ServiceCode == "11602")
            {

                string compardatestr = "20/" + ((DateTime.Now.Day <= 20) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(+1).ToString("MM/yyyy")).ToString();
                //string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                //string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                DateTime compardate = DateTime.ParseExact(compardatestr, "dd/MM/yyyy", null);

                if ((emp.INS_END_DATE < compardate) && !(emp.CARD_ID.Split('-')[0].Contains("500")))
                {
                    var nextEmployeecontract = db.Comp_Employees.Where(c => c.CARD_ID == id).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (nextEmployeecontract == null)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.CONTRACT_NO <= emp.CONTRACT_NO)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.TERMINATE_DATE != null)
                    {
                        if (nextEmployeecontract.TERMINATE_DATE.Value.Month <= (DateTime.Now.Month + 1))
                            return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });
                        else
                            emp = nextEmployeecontract;
                    }
                    emp = nextEmployeecontract;
                }
            }

            //provider service permision 
            var provider = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (provider != null)
            {
                var permission = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == _IntServiceCode
                && (x.ProviderName == provider.Provider || x.ProviderName == "ALL")
                && (x.CompId == _CompId || x.CompId == "ALL" || x.CardId == id)).ToList().OrderByDescending(x => x.Id);
                var _permision = permission.Where(x => x.CardId == id || x.ClassCode == emp.CLASS_CODE).FirstOrDefault();
                if (_permision == null)
                {
                    _permision = permission.Where(x => x.CompId == "ALL" || x.CompId == _CompId).FirstOrDefault();
                }
                //ProviderServicesPermission _permision2 = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == _IntServiceCode && 
                //(x.ProviderName == provider.Provider || x.ProviderName == "ALL") && (x.CardId == id || x.CompId == "ALL" || ((x.ClassCode == "" || x.ClassCode == null) ? x.CompId == _CompId : (x.CompId == _CompId && x.ClassCode == emp.CLASS_CODE)))).OrderByDescending(x => x.Id).FirstOrDefault();
                if (_permision != null && _permision.IsActive == false)
                {
                    if (_permision.CardId == "All" || _permision.CompId == "All" || _permision.CardId == id)
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });
                    }
                    else if (_permision.ClassCode == emp.CLASS_CODE)
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });
                    }
                    else if ((_permision.CardId == "" || _permision.CardId == null) && (_permision.ClassCode == null || _permision.ClassCode == "") && (_permision.CompId == _CompId || _permision.CompId == "ALL"))
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });
                    }
                }
            }
            if (emp != null)
            {
                double CompContractClassMAX_AMOUNT = 0;
                double MaxServiceAmount = 0;
                double CeilingPert;
                double MaxSubServiceAmount;
                bool type = false;
                string isfamily = "";
                string ispool = "";
                //bool hasException = false;
                var remainingconsumption = db.RemainConsumptions.Where(x => x.CARD_ID == id && x.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                if (remainingconsumption != null)
                {
                    if (remainingconsumption.REMAINING >= 0)
                    {
                        CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value;
                        type = true;
                    }
                    else
                    {
                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (accption == null)
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                        if (reasons != null)
                        {
                            CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value;
                            type = true;
                        }
                        else
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                    }
                }
                var CompContractClassEmp = db.CompContractClassEmps.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.CARD_ID == id).FirstOrDefault();
                if (CompContractClassEmp == null)
                {
                    var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(classLimit.MAX_AMOUNT * 0.85);
                    isfamily = classLimit.FOR_FAMILY;
                }
                else
                {
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(CompContractClassEmp.MAX_AMOUNT * 0.85);
                    isfamily = CompContractClassEmp.FOR_FAMILY;
                }
                type = false;

                var DataService1 = new Comp_Customized_D_D();
                var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode && c.CARD_ID == id).FirstOrDefault();
                if (DataService != null)
                {
                    ispool = DataService.POLL_CONSUMPTION;
                    var max_serv = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CARD_ID == id).FirstOrDefault();
                    MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                    //ispool = max_serv.POLL_CONSUMPTION;
                }
                else if (DataService == null)
                {
                    DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (DataService1 != null)
                    {
                        ispool = DataService1.POLL_CONSUMPTION;
                        var max_serv = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService).FirstOrDefault();
                        MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                        //ispool = max_serv.POLL_CONSUMPTION;

                    }

                }
                //Ceiling pert
                if (DataService != null)
                {
                    CeilingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService.CEILING_AMT);

                }
                else if (DataService1 != null)
                {
                    CeilingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService1.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService1.CEILING_AMT);
                }
                else
                {
                    Message = "هذه الخدمه غير مغطاه برجاء الرجوع للاداره الطبيه";
                    CeilingPert = 100;
                    MaxSubServiceAmount = 0;
                    return Json(new { Validation = false, Message = Message, Limit = 0, CeilingPert = 0 });

                }
                double Available = 0;
                double Limit = 0;
                List<Roshita> AcumlatorList = new List<Roshita>();
                var EmpCode = id.Split('-')[2];
                var CompCode = id.Split('-')[0];
                if (isfamily == "Y")
                {
                    AcumlatorList = db.Roshitas.Where(r => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", r.CardId) > 0
                    && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }
                //else if (ispool == "Y")
                //{
                //    AcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                //     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                //}
                else
                {
                    AcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }
                if (ServiceCode == "11602")
                {
                    PersonNoPay = (from roshita in db.Roshitas
                                   join details in db.RoshitaDetails
                                         on roshita.Id equals details.RoshitaID
                                   where roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                   && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= emp.INS_START_DATE && roshita.CreatedDate < emp.INS_END_DATE
                                   select new
                                   {
                                       Amount = details.Amount,
                                   }).ToList().Sum(r => r.Amount);
                }
                List<Roshita> copyacumlator = new List<Roshita>();
                copyacumlator.AddRange(AcumlatorList);
                for (int i = 0; i < copyacumlator.Count(); i++)
                {
                    var item = copyacumlator[i];
                    var chickpermision = (from roshitaacception in db.RoshitaAcceptions
                                          join cardaception in db.CardAcceptionReasons
                                                on roshitaacception.AcceptionId equals cardaception.AcceptionId
                                          where roshitaacception.RoshitaId == item.Id && (cardaception.AcceptionReasonsId == 1 || cardaception.AcceptionReasonsId == 2)
                                          select new
                                          {
                                              id = cardaception.AcceptionReasonsId,
                                          }).ToList();
                    if (chickpermision.Count() > 0)
                    {
                        AcumlatorList.Remove(item);
                    }
                }

                //Main consumption
                double AcumlatorAmount = 0;
                foreach (var item in AcumlatorList)
                {
                    AcumlatorAmount += item.CompanyPayment;
                }
                AcumlatorAmount -= PersonNoPay;
                Available = Convert.ToDouble(CompContractClassMAX_AMOUNT) - AcumlatorAmount;
                double annualLimit = Available;
                //Service consumption
                List<Roshita> AcumlatorServiceList = AcumlatorList.Where(r => r.RoshetaType.Contains(MainService)).ToList();
                double AcumlatorServiceAmount = 0;
                foreach (var item in AcumlatorServiceList)
                {
                    AcumlatorServiceAmount += item.CompanyPayment;
                }
                AcumlatorServiceAmount -= PersonNoPay;
                double ServiceAvailable = (MaxServiceAmount - AcumlatorServiceAmount) < 0 ? 0 : MaxServiceAmount - AcumlatorServiceAmount;
                //SubService consumption
                List<Roshita> AcumlatorSubServiceList = AcumlatorServiceList.Where(r => r.RoshetaType == ServiceCode).ToList();
                double AcumlatorSubServiceAmount = 0;
                foreach (var item in AcumlatorSubServiceList)
                {
                    AcumlatorSubServiceAmount += item.CompanyPayment;
                }
                AcumlatorSubServiceAmount -= PersonNoPay;
                double SubServiceAvailable = (MaxSubServiceAmount - AcumlatorSubServiceAmount) < 0 ? 0 : MaxSubServiceAmount - AcumlatorSubServiceAmount;
                //limit
                Limit = (Available >= ServiceAvailable) ? ServiceAvailable : Available;
                Limit = (Limit >= SubServiceAvailable) ? SubServiceAvailable : Limit;
                //}
                //polling
                if (remainingconsumption != null)
                {
                    Limit = (double)(remainingconsumption.REMAINING.Value < Limit ? remainingconsumption.REMAINING : Limit);
                }
                if (ispool == "Y")
                {
                    Limit = (double)(db.CONSUMPTION_POOL.Where(r => r.COMP_ID == emp.C_COMP_ID).FirstOrDefault().REMAINING);

                }
                bool Validation = Limit > 0 ? true : false;
                Message = Validation ? "Ok" : "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد";
                //Message = Validation ? "Ok" : "Exceeded his annual contract limit";
                //Co-insurance
                double nopaylast21day = 0;
                Co_Insurance_01 CoInsurancelimit2 = new Co_Insurance_01();
                COMP_CUSTOMIZED_D_D_MED_EMP CustemizedMedEmp = new COMP_CUSTOMIZED_D_D_MED_EMP();
                if (isfamily == "Y" || ispool == "Y")
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", c.CARD_ID) > 0 && c.C_COMP_ID == emp.C_COMP_ID
                     && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                     && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }
                else
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                     && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                     && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }
                if (CustemizedMedEmp != null)
                {
                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                    string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);

                    nopaylast21day = (from roshita in db.Roshitas
                                      join details in db.RoshitaDetails
                                            on roshita.Id equals details.RoshitaID
                                      where roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                      && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                      select new
                                      {
                                          Amount = details.Amount,
                                      }).ToList().Sum(r => r.Amount);
                    //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
                    List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                    List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                    List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic")).ToList();
                    //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
                    bool LimitDailyPreceptionCount = false;
                    bool LimitMonthlyPreceptionCount = false;
                    LimitDailyPreceptionCount = (CustemizedMedEmp.DAY_NO_ROSHTA_MON == null || (CustemizedMedEmp.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                    LimitMonthlyPreceptionCount = (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                    LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                    LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                    Double LimitDailyMonthlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                    Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                    Double LimitDailyYearlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                    Double LimitMonthlyYearlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                    if (ServiceCode == "11601" || ServiceCode == "11604")
                    {
                        //if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
                        //    Limit = LimitDailyMonthlyPreceptionAmount;
                        //if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
                        //    Limit = LimitDailyYearlyPreceptionAmount;

                        if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
                            Limit = LimitDailyMonthlyPreceptionAmount;
                        if (LimitDailyMonthlyPreceptionAmount < 0)
                            Limit = .001;
                        if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
                            Limit = LimitDailyYearlyPreceptionAmount;
                        if (LimitDailyYearlyPreceptionAmount < 0)
                            Limit = .001;
                    }
                    if (ServiceCode == "11602" || ServiceCode == "11603")
                    {
                        int nopay = 0;
                        int noover = 0;
                        if (ServiceCode == "11602")
                        {
                            var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                            if (medcard != null)
                            {
                                noover = medcard.NO_OVER.Value;
                                nopay = medcard.NO_PAY.Value;
                            }
                        }
                        if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyMonthlyPreceptionAmount;
                        if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyYearlyPreceptionAmount;
                    }
                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                    if (CoInsurancelimit2.INSURANCE_DAY < 0)
                    {
                        CoInsurancelimit2.INSURANCE_DAY = .001;
                    }
                    if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                    {
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    }
                    if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    //approval ceiling
                    if (Validation == false)
                    {
                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (accption == null)
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0, AnnualLimit = annualLimit });
                        }
                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                        if (reasons != null)
                        {
                            return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });
                        }

                    }
                    //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                    return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });

                }
                else
                {
                    var CustemizedMed = db.COMP_CUSTOMIZED_D_D_MED.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (CustemizedMed == null)
                    {
                        return Json(new { Validation = false, Message = "يرجي مراجعه الادارة الطبيه", Limit = 0, CeilingPert = 0, AnnualLimit = annualLimit });
                    }
                    else
                    {

                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        nopaylast21day = (from roshita in db.Roshitas
                                          join details in db.RoshitaDetails
                                                on roshita.Id equals details.RoshitaID
                                          where roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                          && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                          select new
                                          {
                                              Amount = details.Amount,
                                          }).ToList().Sum(r => r.Amount);
                        //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                        List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
                        List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                        List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                        List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic")).ToList();
                        //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
                        bool LimitDailyPreceptionCount = false;
                        bool LimitMonthlyPreceptionCount = false;
                        LimitDailyPreceptionCount = (CustemizedMed.DAY_NO_ROSHTA_MON == null || (CustemizedMed.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                        LimitMonthlyPreceptionCount = (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                        LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMed.DAY_NO_ROSHTA_YEAR == null || (CustemizedMed.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                        LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                        Double LimitDailyMonthlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                        Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMed.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                        Double LimitDailyYearlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                        Double LimitMonthlyYearlyPreceptionAmount = CustemizedMed.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                        if (ServiceCode == "11601" || ServiceCode == "11604")
                        {
                            if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
                                Limit = LimitDailyMonthlyPreceptionAmount;
                            if (LimitDailyMonthlyPreceptionAmount < 0)
                                Limit = .001;
                            if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
                                Limit = LimitDailyYearlyPreceptionAmount;
                            if (LimitDailyYearlyPreceptionAmount < 0)
                                Limit = .001;
                        }
                        if (ServiceCode == "11602" || ServiceCode == "11603")
                        {
                            int nopay = 0;
                            int noover = 0;
                            if (ServiceCode == "11602")
                            {
                                var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                                if (medcard != null)
                                {
                                    noover = medcard.NO_OVER.Value;
                                    nopay = medcard.NO_PAY.Value;
                                }
                            }
                            if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyMonthlyPreceptionAmount;
                            if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyYearlyPreceptionAmount;
                        }
                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                        if (CoInsurancelimit2.INSURANCE_DAY < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = .001;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        }

                        if (CoInsurancelimit2.INSURANCE_DAY == null)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = 0;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH == null)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = 0;
                        }
                        if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        //approval ceiling
                        if (Validation == false)
                        {
                            var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                            if (accption == null)
                            {
                                return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0, AnnualLimit = annualLimit });
                            }
                            var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                            if (reasons != null)
                            {
                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });
                            }

                        }
                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });

                    }
                }

            }
            else
            {
                return Json(new { Validation = false, Message = "Employee contract issue ,you can call operation department", Limit = 0, CeilingPert = 0 });

            }

        }

        public JsonResult CellingAmountEditPage(string id, string ServiceCode, Int64 RoshitaId)
        {
            double PersonNoPay = 0;
            string Message = "";
            ServiceCode = ServiceCode == "11604" ? "11601" : ServiceCode;
            string MainService = ServiceCode.Substring(0, 3);
            Comp_Employees emp = new Comp_Employees();
            emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

            if (ServiceCode == "11602")
            {

                string compardatestr = "20/" + ((DateTime.Now.Day <= 20) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(+1).ToString("MM/yyyy")).ToString();
                //string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                DateTime compardate = DateTime.ParseExact(compardatestr, "dd/MM/yyyy", null);

                if ((emp.INS_END_DATE < compardate) && !(emp.CARD_ID.Split('-')[0].Contains("500")))
                {
                    var nextEmployeecontract = db.Comp_Employees.Where(c => c.CARD_ID == id).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (nextEmployeecontract == null)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.CONTRACT_NO <= emp.CONTRACT_NO)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    emp = nextEmployeecontract;
                }
            }
            //
            if (emp != null)
            {
                double CompContractClassMAX_AMOUNT = 0;
                double MaxServiceAmount = 0;
                double CeilingPert;
                double MaxSubServiceAmount;
                bool type = false;
                double RoshitaNoPayEdit = 0;
                string isfamily = "";
                string ispool = "";
                if (ServiceCode == "11602")
                {
                    RoshitaNoPayEdit = (from roshita in db.Roshitas
                                        join details in db.RoshitaDetails
                                              on roshita.Id equals details.RoshitaID
                                        where roshita.Id == RoshitaId && roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                        && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= emp.INS_START_DATE && roshita.CreatedDate < emp.INS_END_DATE
                                        select new
                                        {
                                            Amount = details.Amount,
                                        }).ToList().Sum(r => r.Amount);


                    PersonNoPay = (from roshita in db.Roshitas
                                   join details in db.RoshitaDetails
                                         on roshita.Id equals details.RoshitaID
                                   where roshita.Id != RoshitaId && roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                   && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= emp.INS_START_DATE && roshita.CreatedDate < emp.INS_END_DATE
                                   select new
                                   {
                                       Amount = details.Amount,
                                   }).ToList().Sum(r => r.Amount);
                }
                var remainingconsumption = db.RemainConsumptions.Where(x => x.CARD_ID == id && x.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                var remainingPool = db.CONSUMPTION_POOL.Where(r => r.COMP_ID == emp.C_COMP_ID).FirstOrDefault();
                if (remainingconsumption != null)
                {
                    if (remainingconsumption.REMAINING >= 0)
                    {
                        var rosita = db.Roshitas.Where(r => r.Id == RoshitaId).FirstOrDefault();
                        CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit;
                        type = true;
                        remainingconsumption.REMAINING = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit;
                    }
                    else
                    {
                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (accption == null)
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                        if (reasons != null)
                        {
                            var rosita = db.Roshitas.Where(r => r.Id == RoshitaId).FirstOrDefault();
                            CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit; ;
                            type = true;
                            remainingconsumption.REMAINING = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit; ;

                        }
                        else
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                    }
                }
                //if (remainingPool != null)
                //{
                //    var rosita = db.Roshitas.Where(r => r.Id == RoshitaId).FirstOrDefault();
                //    remainingPool.REMAINING = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit;
                //}
                //else
                //{
                var CompContractClassEmp = db.CompContractClassEmps.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.CARD_ID == id).FirstOrDefault();
                if (CompContractClassEmp == null)
                {
                    var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(classLimit.MAX_AMOUNT * 0.85);
                    isfamily = classLimit.FOR_FAMILY;
                }
                else
                {
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(CompContractClassEmp.MAX_AMOUNT * 0.85);
                    isfamily = CompContractClassEmp.FOR_FAMILY;
                }
                type = false;

                var DataService1 = new Comp_Customized_D_D();
                var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode && c.CARD_ID == id).FirstOrDefault();
                if (DataService != null)
                {
                    ispool = DataService.POLL_CONSUMPTION;
                    var max_serv = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CARD_ID == id).FirstOrDefault();
                    MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                    //ispool = max_serv.POLL_CONSUMPTION;
                }
                else if (DataService == null)
                {
                    DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (DataService1 != null)
                    {
                        ispool = DataService1.POLL_CONSUMPTION;
                        var max_serv = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService).FirstOrDefault();
                        MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                        //ispool = max_serv.POLL_CONSUMPTION;

                    }

                }
                //Ceiling pert
                if (DataService != null)
                {
                    CeilingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService.CEILING_AMT);

                }
                else if (DataService1 != null)
                {
                    CeilingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService1.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService1.CEILING_AMT);
                }
                else
                {
                    Message = "هذه الخدمه غير مغطاه برجاء الرجوع للاداره الطبيه";
                    CeilingPert = 100;
                    MaxSubServiceAmount = 0;
                    return Json(new { Validation = false, Message = Message, Limit = 0, CeilingPert = 0 });

                }

                double Available = 0;
                double Limit = 0;
                List<Roshita> AcumlatorList = new List<Roshita>();
                var EmpCode = id.Split('-')[2];
                var CompCode = id.Split('-')[0];
                if (isfamily == "Y")
                {
                    AcumlatorList = db.Roshitas.Where(r => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", r.CardId) > 0 && r.Id != RoshitaId
                    && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }
                else
                {
                    AcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                    && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }
                //List<Roshita> AcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                //     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();

                List<Roshita> copyacumlator = new List<Roshita>();
                copyacumlator.AddRange(AcumlatorList);
                for (int i = 0; i < copyacumlator.Count(); i++)
                {
                    var item = copyacumlator[i];
                    var chickpermision = (from roshitaacception in db.RoshitaAcceptions
                                          join cardaception in db.CardAcceptionReasons
                                                on roshitaacception.AcceptionId equals cardaception.AcceptionId
                                          where roshitaacception.RoshitaId == item.Id && (cardaception.AcceptionReasonsId == 1 || cardaception.AcceptionReasonsId == 2)
                                          select new
                                          {
                                              id = cardaception.AcceptionReasonsId,
                                          }).ToList();
                    if (chickpermision.Count() > 0)
                    {
                        AcumlatorList.Remove(item);
                    }
                }
                //if (type == true)
                //{
                //    Available = CompContractClassMAX_AMOUNT;

                //}
                //else
                //{
                //Main consumption
                double AcumlatorAmount = 0;
                foreach (var item in AcumlatorList)
                {
                    AcumlatorAmount += item.CompanyPayment;
                }
                AcumlatorAmount -= PersonNoPay;
                Available = Convert.ToDouble(CompContractClassMAX_AMOUNT) - AcumlatorAmount;
                //}
                //if (remainingconsumption != null && remainingconsumption.REMAINING != null &&
                //    (remainingconsumption.REMAINING == Available))
                //{
                //    Limit = Available;
                //}
                //else
                //{
                //Service Concamution
                List<Roshita> AcumlatorServiceList = AcumlatorList.Where(r => r.RoshetaType.Contains(MainService)).ToList();
                double AcumlatorServiceAmount = 0;
                foreach (var item in AcumlatorServiceList)
                {
                    AcumlatorServiceAmount += item.CompanyPayment;
                }
                AcumlatorServiceAmount -= PersonNoPay;
                double ServiceAvailable = (MaxServiceAmount - AcumlatorServiceAmount) < 0 ? 0 : MaxServiceAmount - AcumlatorServiceAmount;
                //SubService Concamution
                List<Roshita> AcumlatorSubServiceList = AcumlatorServiceList.Where(r => r.RoshetaType == ServiceCode).ToList();
                double AcumlatorSubServiceAmount = 0;
                foreach (var item in AcumlatorSubServiceList)
                {
                    AcumlatorSubServiceAmount += item.CompanyPayment;
                }
                AcumlatorSubServiceAmount -= PersonNoPay;
                double SubServiceAvailable = (MaxSubServiceAmount - AcumlatorSubServiceAmount) < 0 ? 0 : MaxSubServiceAmount - AcumlatorSubServiceAmount;
                //limit
                Limit = (Available >= ServiceAvailable) ? ServiceAvailable : Available;
                Limit = (Limit >= SubServiceAvailable) ? SubServiceAvailable : Limit;
                //}
                //polling
                if (remainingconsumption != null)
                {
                    Limit = (double)(remainingconsumption.REMAINING.Value < Limit ? remainingconsumption.REMAINING : Limit);
                }

                if (ispool == "Y")
                {
                    Limit = remainingPool.REMAINING.Value;

                }

                bool Validation = Limit > 0 ? true : false;
                Message = Validation ? "Ok" : "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد";

                //approval ceiling
                if (Validation == false)
                {
                    var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                    if (accption == null)
                    {
                        return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                    }
                    var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                    if (reasons != null)
                    {
                        return Json(new { Validation = true, Message = "Has Approval", Limit = 0.001, CeilingPert = CeilingPert });
                    }

                }
                //Co-insurance
                double nopaylast21day = 0;
                Co_Insurance_01 CoInsurancelimit2 = new Co_Insurance_01();
                bool LimitDailyPreceptionCount = false;
                bool LimitMonthlyPreceptionCount = false;
                COMP_CUSTOMIZED_D_D_MED_EMP CustemizedMedEmp = new COMP_CUSTOMIZED_D_D_MED_EMP();
                if (isfamily == "Y" || ispool == "Y")
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", c.CARD_ID) > 0 && c.C_COMP_ID == emp.C_COMP_ID
                     && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                     && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }
                else
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                  && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }

                //var CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                //  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                //  && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                if (CustemizedMedEmp != null)
                {
                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                    string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                    nopaylast21day = (from roshita in db.Roshitas
                                      join details in db.RoshitaDetails
                                            on roshita.Id equals details.RoshitaID
                                      where roshita.CardId == id && roshita.Id != RoshitaId && roshita.Manager == "Pharmacy_Chronic"
                                      && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                      select new
                                      {
                                          Amount = details.Amount,
                                      }).ToList().Sum(r => r.Amount);
                    List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    List<Roshita> YearlyDailyAcumlatorList = MainAcumlatorList.Where(x => (x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor") && x.CompanyPayment > 0).ToList();
                    List<Roshita> YearlyMonthlyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                    List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                    List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.Id != RoshitaId && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic")).ToList();
                    //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();

                    LimitDailyPreceptionCount = (CustemizedMedEmp.DAY_NO_ROSHTA_MON == null || (CustemizedMedEmp.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                    LimitMonthlyPreceptionCount = (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                    LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                    LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                    Double LimitDailyMonthlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                    Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                    Double LimitDailyYearlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                    Double LimitMonthlyYearlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                    if (ServiceCode == "11601" || ServiceCode == "11604")
                    {
                        if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
                            Limit = LimitDailyMonthlyPreceptionAmount;
                        if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
                            Limit = LimitDailyYearlyPreceptionAmount;
                    }
                    if (ServiceCode == "11602" || ServiceCode == "11603")
                    {
                        int nopay = 0;
                        int noover = 0;
                        if (ServiceCode == "11602")
                        {
                            var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                            if (medcard != null)
                            {
                                noover = medcard.NO_OVER.Value;
                                nopay = medcard.NO_PAY.Value;
                            }
                        }
                        if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyMonthlyPreceptionAmount;
                        if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyYearlyPreceptionAmount;
                    }

                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                    if (CoInsurancelimit2.INSURANCE_DAY < 0)
                    {
                        CoInsurancelimit2.INSURANCE_DAY = .001;
                    }
                    if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                    {
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    }
                    if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                }
                else
                {
                    var CustemizedMed = db.COMP_CUSTOMIZED_D_D_MED.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (CustemizedMed == null)
                    {
                        return Json(new { Validation = false, Message = "يرجي مراجعه الادارة الطبيه", Limit = 0, CeilingPert = 0 });
                    }
                    else
                    {

                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();

                        nopaylast21day = (from roshita in db.Roshitas
                                          join details in db.RoshitaDetails
                                                on roshita.Id equals details.RoshitaID
                                          where roshita.CardId == id && roshita.Id != RoshitaId && roshita.Manager == "Pharmacy_Chronic"
                                          && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                          select new
                                          {
                                              Amount = details.Amount,
                                          }).ToList().Sum(r => r.Amount);
                        List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => (x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor") && x.CompanyPayment > 0).ToList();
                        List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                        List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                        List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.Id != RoshitaId && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic")).ToList();
                        //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();

                        LimitDailyPreceptionCount = (CustemizedMed.DAY_NO_ROSHTA_MON == null || (CustemizedMed.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                        LimitMonthlyPreceptionCount = (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                        LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMed.DAY_NO_ROSHTA_YEAR == null || (CustemizedMed.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                        LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                        Double LimitDailyMonthlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                        Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMed.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                        Double LimitDailyYearlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                        Double LimitMonthlyYearlyPreceptionAmount = CustemizedMed.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                        if (ServiceCode == "11601" || ServiceCode == "11604")
                        {
                            if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
                                Limit = LimitDailyMonthlyPreceptionAmount;
                            if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
                                Limit = LimitDailyYearlyPreceptionAmount;
                        }
                        if (ServiceCode == "11602" || ServiceCode == "11603")
                        {
                            int nopay = 0;
                            int noover = 0;
                            if (ServiceCode == "11602")
                            {
                                var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                                if (medcard != null)
                                {
                                    noover = medcard.NO_OVER.Value;
                                    nopay = medcard.NO_PAY.Value;
                                }
                            }
                            if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyMonthlyPreceptionAmount;
                            if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyYearlyPreceptionAmount;
                        }
                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                        if (CoInsurancelimit2.INSURANCE_DAY < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = .001;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        }

                        if (CoInsurancelimit2.INSURANCE_DAY == null)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = 0;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH == null)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = 0;
                        }
                        if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                            CoInsurancelimit2.INSURANCE_MONTH = .001;

                    }
                }

                return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, IsFamily = isfamily, IsPool = ispool });
            }
            return Json(new { Validation = false, Message = "Employee contract issue ,you can call operation department", Limit = 0, CeilingPert = 0 });
        }

        public JsonResult CellingAmount2(string id, string ServiceCode)
        {
            string Message = "";
            ServiceCode = ServiceCode == "11604" ? "11601" : ServiceCode;
            int _IntServiceCode = Convert.ToInt32(ServiceCode);
            string _CompId = id.Split('-')[0];
            string MainService = ServiceCode.Substring(0, 3);
            var CurrentDate = DateTime.Now.Date;
            Comp_Employees emp = new Comp_Employees();
            emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= CurrentDate && c.INS_END_DATE >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

            if (ServiceCode == "11602")
            {

                //string compardatestr = "20/" + ((DateTime.Now.Day <= 20) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(+1).ToString("MM/yyyy")).ToString();                string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                DateTime compardate = DateTime.ParseExact(compardatestr, "dd/MM/yyyy", null);

                if ((emp.INS_END_DATE < compardate) && !(emp.CARD_ID.Split('-')[0].Contains("500")))
                {
                    var nextEmployeecontract = db.Comp_Employees.Where(c => c.CARD_ID == id).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (nextEmployeecontract == null)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.CONTRACT_NO <= emp.CONTRACT_NO)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    emp = nextEmployeecontract;
                }
            }
            //provider service permision 10573-1-1-1
            var provider = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (provider != null)
            {
                var permission = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == _IntServiceCode
                && (x.ProviderName == provider.Provider || x.ProviderName == "ALL")
                && (x.CompId == _CompId || x.CompId == "ALL" || x.CardId == id)).ToList().OrderByDescending(x => x.Id);
                var _permision = permission.Where(x => x.CardId == id || x.ClassCode == emp.CLASS_CODE).FirstOrDefault();
                if (_permision == null)
                {
                    _permision = permission.Where(x => x.CompId == "ALL" || x.CompId == _CompId).FirstOrDefault();
                }
                //ProviderServicesPermission _permision2 = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == _IntServiceCode && 
                //(x.ProviderName == provider.Provider || x.ProviderName == "ALL") && (x.CardId == id || x.CompId == "ALL" || ((x.ClassCode == "" || x.ClassCode == null) ? x.CompId == _CompId : (x.CompId == _CompId && x.ClassCode == emp.CLASS_CODE)))).OrderByDescending(x => x.Id).FirstOrDefault();
                if (_permision != null && _permision.IsActive == false)
                {
                    if (_permision.CardId == "All" || _permision.CompId == "All" || _permision.CardId == id)
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });

                    }
                    else if (_permision.ClassCode == emp.CLASS_CODE)
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });

                    }
                    else if ((_permision.CardId == "" || _permision.CardId == null) && (_permision.ClassCode == null || _permision.ClassCode == "") && (_permision.CompId == _CompId || _permision.CompId == "ALL"))
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });

                    }
                }
            }
            //
            if (emp != null)
            {
                double CompContractClassMAX_AMOUNT = 0;
                double MaxServiceAmount = 0;
                double CeilingPert;
                double MaxSubServiceAmount;
                var CompContractClassEmp = db.CompContractClassEmps.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.CARD_ID == id).FirstOrDefault();
                if (CompContractClassEmp == null)
                {
                    var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(classLimit.MAX_AMOUNT * 0.85);
                }
                else
                {
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(CompContractClassEmp.MAX_AMOUNT * 0.85);
                }
                //var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();

                var DataService1 = new Comp_Customized_D_D();
                var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode && c.CARD_ID == id).FirstOrDefault();
                if (DataService != null)
                {
                    var max_serv = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CARD_ID == id).FirstOrDefault();
                    MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);

                }
                else if (DataService == null)
                {
                    DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (DataService1 != null)
                    {
                        var max_serv = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService).FirstOrDefault();
                        MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);

                    }

                }
                //Ceiling pert
                if (DataService != null)
                {
                    CeilingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService.CEILING_AMT);

                }
                else if (DataService1 != null)
                {
                    CeilingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService1.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService1.CEILING_AMT);
                }
                else
                {
                    Message = "هذه الخدمه غير مغطاه برجاء الرجوع للاداره الطبيه";
                    CeilingPert = 100;
                    MaxSubServiceAmount = 0;
                    return Json(new { Validation = false, Message = Message, Limit = 0, CeilingPert = 0 });

                }
                //Main consumption
                List<Roshita> AcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                double AcumlatorAmount = 0;
                foreach (var item in AcumlatorList)
                {
                    AcumlatorAmount += item.CompanyPayment;
                }
                double Available = Convert.ToDouble(CompContractClassMAX_AMOUNT) - AcumlatorAmount;
                //Service consumption
                List<Roshita> AcumlatorServiceList = db.Roshitas.Where(r => r.CardId == id && r.RoshetaType.Contains(MainService) && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                double AcumlatorServiceAmount = 0;
                foreach (var item in AcumlatorServiceList)
                {
                    AcumlatorServiceAmount += item.CompanyPayment;
                }
                double ServiceAvailable = (MaxServiceAmount - AcumlatorServiceAmount) < 0 ? 0 : MaxServiceAmount - AcumlatorServiceAmount;
                //SubService consumption
                List<Roshita> AcumlatorSubServiceList = db.Roshitas.Where(r => r.CardId == id && r.RoshetaType == ServiceCode && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                double AcumlatorSubServiceAmount = 0;
                foreach (var item in AcumlatorSubServiceList)
                {
                    AcumlatorSubServiceAmount += item.CompanyPayment;
                }
                double SubServiceAvailable = (MaxSubServiceAmount - AcumlatorSubServiceAmount) < 0 ? 0 : MaxSubServiceAmount - AcumlatorSubServiceAmount;
                //limit
                double Limit = (Available >= ServiceAvailable) ? ServiceAvailable : Available;
                Limit = (Limit >= SubServiceAvailable) ? SubServiceAvailable : Limit;
                //polling
                bool Validation = Limit > 0 ? true : false;
                Message = Validation ? "Ok" : "Exceeded his annual contract limit";
                //Co-insurance
                Co_Insurance_01 CoInsurancelimit2 = new Co_Insurance_01();
                var CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                  && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                if (CustemizedMedEmp != null)
                {
                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                    string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                    List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    List<Roshita> YearlyDailyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Daily").ToList();
                    List<Roshita> YearlyMonthlyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                    List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                    List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
                    bool LimitDailyPreceptionCount = false;
                    bool LimitMonthlyPreceptionCount = false;
                    LimitDailyPreceptionCount = (CustemizedMedEmp.DAY_NO_ROSHTA_MON == null || (CustemizedMedEmp.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                    LimitMonthlyPreceptionCount = (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                    LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                    LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                    Double LimitDailyMonthlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                    Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Monthly Amount

                    Double LimitDailyYearlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Daily Amount
                    Double LimitMonthlyYearlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Monthly Amount

                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));

                    if (CoInsurancelimit2.INSURANCE_DAY < 0)
                    {
                        CoInsurancelimit2.INSURANCE_DAY = .001;
                    }
                    if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                    {
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    }
                    //approval ceiling
                    if (Validation == false)
                    {
                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (accption == null)
                        {
                            return Json(new { Validation = false, Message = "Exceeded his annual contract limit", Limit = 0, CeilingPert = 0 });
                        }
                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                        if (reasons != null)
                        {
                            return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert });
                        }

                    }
                    //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                    return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount });

                }
                else
                {
                    var CustemizedMed = db.COMP_CUSTOMIZED_D_D_MED.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (CustemizedMed == null)
                    {
                        return Json(new { Validation = false, Message = "يرجي مراجعه الادارة الطبيه", Limit = 0, CeilingPert = 0 });
                    }
                    else
                    {

                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                        List<Roshita> YearlyDailyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Daily").ToList();
                        List<Roshita> YearlyMonthlyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                        List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                        List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
                        bool LimitDailyPreceptionCount = false;
                        bool LimitMonthlyPreceptionCount = false;
                        LimitDailyPreceptionCount = (CustemizedMed.DAY_NO_ROSHTA_MON == null || (CustemizedMed.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                        LimitMonthlyPreceptionCount = (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                        LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMed.DAY_NO_ROSHTA_YEAR == null || (CustemizedMed.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                        LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                        Double LimitDailyMonthlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                        Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMed.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Monthly Amount

                        Double LimitDailyYearlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Daily Amount
                        Double LimitMonthlyYearlyPreceptionAmount = CustemizedMed.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Monthly Amount

                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                        if (CoInsurancelimit2.INSURANCE_DAY < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = .001;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        }

                        if (CoInsurancelimit2.INSURANCE_DAY == null)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = 0;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH == null)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = 0;
                        }
                        //approval ceiling
                        if (Validation == false)
                        {
                            var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                            if (accption == null)
                            {
                                return Json(new { Validation = false, Message = "Exceeded his annual contract limit", Limit = 0, CeilingPert = 0 });
                            }
                            var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                            if (reasons != null)
                            {
                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert });
                            }

                        }
                        //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount });

                    }
                }

            }
            return Json(new { Validation = false, Message = "Employee contract issue ,you can call operation department", Limit = 0, CeilingPert = 0 });
        }

        public JsonResult CellingAmountEditPage2(string id, string ServiceCode, Int64 RoshitaId)
        {
            string Message = "";
            ServiceCode = ServiceCode == "11604" ? "11601" : ServiceCode;
            string MainService = ServiceCode.Substring(0, 3);
            Comp_Employees emp = new Comp_Employees();
            emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

            if (ServiceCode == "11602")
            {

                //string compardatestr = "20/" + ((DateTime.Now.Day <= 20) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(+1).ToString("MM/yyyy")).ToString();
                string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                DateTime compardate = DateTime.ParseExact(compardatestr, "dd/MM/yyyy", null);

                if ((emp.INS_END_DATE < compardate) && !(emp.CARD_ID.Split('-')[0].Contains("500")))
                {
                    var nextEmployeecontract = db.Comp_Employees.Where(c => c.CARD_ID == id).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (nextEmployeecontract == null)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.CONTRACT_NO <= emp.CONTRACT_NO)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    emp = nextEmployeecontract;
                }
            }
            //
            if (emp != null)
            {
                double CompContractClassMAX_AMOUNT = 0;
                double MaxServiceAmount = 0;
                double CeilingPert;
                double MaxSubServiceAmount;
                var CompContractClassEmp = db.CompContractClassEmps.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && emp.CARD_ID == id).FirstOrDefault();
                if (CompContractClassEmp == null)
                {
                    var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(classLimit.MAX_AMOUNT * 0.85);
                }
                else
                {
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(CompContractClassEmp.MAX_AMOUNT * 0.85);
                }
                //var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();

                var DataService1 = new Comp_Customized_D_D();
                var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode && c.CARD_ID == id).FirstOrDefault();
                if (DataService != null)
                {
                    var max_serv = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CARD_ID == id).FirstOrDefault();
                    MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);

                }
                else if (DataService == null)
                {
                    DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (DataService1 != null)
                    {
                        var max_serv = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService).FirstOrDefault();
                        MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);

                    }

                }
                //Ceiling pert
                if (DataService != null)
                {
                    CeilingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService.CEILING_AMT);

                }
                else if (DataService1 != null)
                {
                    CeilingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService1.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService1.CEILING_AMT);
                }
                else
                {
                    Message = "هذه الخدمه غير مغطاه برجاء الرجوع للاداره الطبيه";
                    CeilingPert = 100;
                    MaxSubServiceAmount = 0;
                    return Json(new { Validation = false, Message = Message, Limit = 0, CeilingPert = 0 });

                }
                //Main Concamution
                List<Roshita> AcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                double AcumlatorAmount = 0;
                foreach (var item in AcumlatorList)
                {
                    AcumlatorAmount += item.CompanyPayment;
                }
                double Available = Convert.ToDouble(CompContractClassMAX_AMOUNT) - AcumlatorAmount;
                //Service Concamution
                List<Roshita> AcumlatorServiceList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && r.RoshetaType.Contains(MainService) && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                double AcumlatorServiceAmount = 0;
                foreach (var item in AcumlatorServiceList)
                {
                    AcumlatorServiceAmount += item.CompanyPayment;
                }
                double ServiceAvailable = (MaxServiceAmount - AcumlatorServiceAmount) < 0 ? 0 : MaxServiceAmount - AcumlatorServiceAmount;
                //SubService Concamution
                List<Roshita> AcumlatorSubServiceList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && r.RoshetaType == ServiceCode && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                double AcumlatorSubServiceAmount = 0;
                foreach (var item in AcumlatorSubServiceList)
                {
                    AcumlatorSubServiceAmount += item.CompanyPayment;
                }
                double SubServiceAvailable = (MaxSubServiceAmount - AcumlatorSubServiceAmount) < 0 ? 0 : MaxSubServiceAmount - AcumlatorSubServiceAmount;
                //limit
                double Limit = (Available >= ServiceAvailable) ? ServiceAvailable : Available;
                Limit = (Limit >= SubServiceAvailable) ? SubServiceAvailable : Limit;
                //polling
                bool Validation = Limit > 0 ? true : false;
                Message = Validation ? "Ok" : "Exceeded his annual contract limit";

                //approval ceiling
                if (Validation == false)
                {
                    var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                    if (accption == null)
                    {
                        return Json(new { Validation = false, Message = "Exceeded his annual contract limit", Limit = 0, CeilingPert = 0 });
                    }
                    var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                    if (reasons != null)
                    {
                        return Json(new { Validation = true, Message = "Has Approval", Limit = 0.001, CeilingPert = CeilingPert });
                    }

                }
                //Co-insurance

                Co_Insurance_01 CoInsurancelimit2 = new Co_Insurance_01();
                bool LimitDailyPreceptionCount = false;
                bool LimitMonthlyPreceptionCount = false;
                var CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                  && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                if (CustemizedMedEmp != null)
                {
                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                    string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                    List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    List<Roshita> YearlyDailyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Daily").ToList();
                    List<Roshita> YearlyMonthlyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                    List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                    List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();

                    LimitDailyPreceptionCount = (CustemizedMedEmp.DAY_NO_ROSHTA_MON == null || (CustemizedMedEmp.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                    LimitMonthlyPreceptionCount = (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                    LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                    LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                    Double LimitDailyMonthlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                    Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Monthly Amount

                    Double LimitDailyYearlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Daily Amount
                    Double LimitMonthlyYearlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Monthly Amount

                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));

                    if (CoInsurancelimit2.INSURANCE_DAY < 0)
                    {
                        CoInsurancelimit2.INSURANCE_DAY = .001;
                    }
                    if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                    {
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    }

                }
                else
                {
                    var CustemizedMed = db.COMP_CUSTOMIZED_D_D_MED.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (CustemizedMed == null)
                    {
                        return Json(new { Validation = false, Message = "يرجي مراجعه الادارة الطبيه", Limit = 0, CeilingPert = 0 });
                    }
                    else
                    {

                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                        List<Roshita> YearlyDailyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Daily").ToList();
                        List<Roshita> YearlyMonthlyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                        List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                        List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();

                        LimitDailyPreceptionCount = (CustemizedMed.DAY_NO_ROSHTA_MON == null || (CustemizedMed.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                        LimitMonthlyPreceptionCount = (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                        LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMed.DAY_NO_ROSHTA_YEAR == null || (CustemizedMed.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                        LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                        Double LimitDailyMonthlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                        Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMed.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Monthly Amount

                        Double LimitDailyYearlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Daily Amount
                        Double LimitMonthlyYearlyPreceptionAmount = CustemizedMed.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Monthly Amount

                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                        if (CoInsurancelimit2.INSURANCE_DAY < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = .001;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        }

                        if (CoInsurancelimit2.INSURANCE_DAY == null)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = 0;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH == null)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = 0;
                        }

                    }
                }

                return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount });
            }
            return Json(new { Validation = false, Limit = 0, CeilingPert = 0 });
        }

        public JsonResult GetList(int sEcho = 1, int iDisplayStart = 0, int iDisplayLength = 24, string sSearch = "")
        {
            if (sSearch != null)
            {
                sSearch = sSearch.ToLower();
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Join(db.MedicineGroups, d => d.MED_GROUP, g => g.GroupId, (d, g) => new { d, g })
                    .Where(r => r.d.TRADE_NAME.ToLower().StartsWith(sSearch) || r.d.M_CODE.Contains(sSearch)
                    || r.d.DOSAGE_FORM.ToLower().Contains(sSearch)).OrderBy(m => m.d.TRADE_NAME.ToLower().StartsWith(sSearch))
                    .Select(l => new MedicienDataViewModal
                    {
                        M_CODE = l.d.M_CODE,
                        TRADE_NAME = l.d.TRADE_NAME,
                        DOSAGE_FORM = l.d.DOSAGE_FORM,
                        PACK_PRICE = l.d.PACK_PRICE,
                        PACK_SIZE = l.d.PACK_SIZE,
                        UNIT_NO = l.d.UNIT_NO,
                        UNIT_PRICE = l.d.UNIT_PRICE,
                        Group_Type = l.g.GroupType,
                        IsCovered = l.d.IsCovered
                    }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Count(),
                    iTotalDisplayRecords = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Count()
                };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Join(db.MedicineGroups, d => d.MED_GROUP, g => g.GroupId, (d, g) => new { d, g })
                    .OrderBy(m => m.d.M_CODE).AsEnumerable().
                    Select(l => new MedicienDataViewModal
                    {
                        M_CODE = l.d.M_CODE,
                        TRADE_NAME = l.d.TRADE_NAME,
                        DOSAGE_FORM = l.d.DOSAGE_FORM,
                        PACK_PRICE = l.d.PACK_PRICE,
                        PACK_SIZE = l.d.PACK_SIZE,
                        UNIT_NO = l.d.UNIT_NO,
                        UNIT_PRICE = l.d.UNIT_PRICE,
                        Group_Type = l.g.GroupType,
                        IsCovered = l.d.IsCovered
                    }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),
                    iTotalRecords = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Count(),
                    iTotalDisplayRecords = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Count()
                };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetMedicineByCode(string code)
        {

            var Data = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Join(db.MedicineGroups, d => d.MED_GROUP, g => g.GroupId, (d, g) => new { d, g })
            .OrderBy(m => m.d.M_CODE).
            Select(l => new MedicienDataViewModal
            {
                M_CODE = l.d.M_CODE,
                TRADE_NAME = l.d.TRADE_NAME,
                DOSAGE_FORM = l.d.DOSAGE_FORM,
                PACK_PRICE = l.d.PACK_PRICE,
                PACK_SIZE = l.d.PACK_SIZE,
                UNIT_NO = l.d.UNIT_NO,
                UNIT_PRICE = l.d.UNIT_PRICE,
                Group_Type = l.g.GroupType,
                IsCovered = l.d.IsCovered,
                M_TYPE = l.d.M_TYPE,
            }).FirstOrDefault(x => x.M_CODE == code);

            return new JsonResult { Data = Data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult CheckDaily(string id, string code)
        {
            MedicineData CurentMedicine2 = db.MedicineDatas.Where(x => x.M_CODE == code).FirstOrDefault();
            string message = "";
            int check;
            var createdDate = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Doctor_Chronic") && !r.Manager.Contains("Stop"))
                .Join(db.RoshitaDetails, x => x.Id, d => d.RoshitaID, (x, d) => new { x, d })
              .Where(z => z.d.MedicienCode == code && z.d.PaymentGroup != "Cash" && z.d.IsDealed == true)
              .OrderByDescending(v => v.x.CreatedDate)
              .Select(l => new
              {
                  id = l.x.Id,
                  Createdate = l.x.CreatedDate,
                  TotalDuration = l.d.TotalDuration
              }).FirstOrDefault();
            //string Current = DateTime.Now.ToString("dd");

            if (createdDate == null)
            {
                check = 0; //vaild to despense
            }
            else
            {
                if (DateTime.Now.Date >= createdDate.Createdate.Value.AddDays(createdDate.TotalDuration).Date)
                {
                    check = 0;
                }
                else
                {
                    check = 1;
                    message = "لقد تم صرف هذا الدواء من قبل ومازال فى فتره الاستخدام";
                    return new JsonResult { Data = new { check = check, messa = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }
            var date = DateTime.Now.AddDays(-14);
            var createdDate2 = db.Roshitas.Where(r => r.CardId == id && r.Manager.Contains("Doctor_Daily") && r.CreatedDate >= date && !r.Manager.Contains("Stop"))
                .Join(db.RoshitaDetails, x => x.Id, d => d.RoshitaID, (x, d) => new { x, d })
              .Where(z => z.d.MedicienCode == code && z.d.PaymentGroup != "Cash" && z.d.IsDealed == false)
              .OrderByDescending(v => v.x.CreatedDate)
              .Select(l => new
              {
                  id = l.x.Id,
                  Createdate = l.x.CreatedDate,
                  TotalDuration = l.d.TotalDuration
              }).FirstOrDefault();
            if (createdDate2 != null)
            {
                check = 1;
                message = "هذا الدواء مسجل فالادوية اليوميه";
                return new JsonResult { Data = new { check = check, messa = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            //string CurentGroup = db.MedicineDatas.Where(x => x.M_CODE == code).FirstOrDefault().MED_GROUP;
            //MedicineData CurentMedicine = db.MedicineDatas.Where(x => x.M_CODE == code).FirstOrDefault();
            var ChronicMedicine = db.Med_Medicine.Where(z => z.CARD_NO == id && z.ACTIVE == "Y" && (z.MONTH_DATE_STOP == null || z.MONTH_DATE_STOP > DateTime.Now))
                .Join(db.MedicineDatas, r => r.MED_CODE, m => m.M_CODE, (x, d) => new { x, d })
              // .Join(db.MedicineGroups, r => r.d.MED_GROUP, m => m.GroupId, (x, d) => new { x, d })
              .Where(z => z.d.MED_GROUP == CurentMedicine2.MED_GROUP && z.x.DOSAGE_FORM == CurentMedicine2.DOSAGE_FORM).FirstOrDefault();
            //   var ChronicMedicine = db.Roshitas.Where(z => z.CardId == id && z.Manager == "Doctor_Chronic")
            //  .Join(db.RoshitaDetails, x => x.Id, d => d.RoshitaID, (x, d) => new { x, d })
            //  .Join(db.MedicineDatas, r => r.d.MedicienCode, m => m.M_CODE, (x, d) => new { x, d })
            //  .Join(db.MedicineGroups, r => r.d.MED_GROUP, m => m.GroupId, (x, d) => new { x, d })
            //.Where(z => z.d.GroupId == CurentGroup).FirstOrDefault();
            if (ChronicMedicine != null)
            {
                check = 1;
                message = "هذا الدواء او الدواء البديل يوجد فى الادويه المزمنه للمريض";
                return new JsonResult { Data = new { check = check, messa = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            //var createdDate2 = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Doctor_Chronic") && !r.Manager.Contains("Stop"))
            //  .Join(db.RoshitaDetails, x => x.Id, d => d.RoshitaID, (x, d) => new { x, d })
            //.Where(z => z.d.PaymentGroup != "Cash" && z.d.IsDealed == true)
            //.Join(db.MedicineDatas, r => r.d.MedicienCode, m => m.M_CODE, (c, g) => new { c, g })
            //.Where(z => z.g.MED_GROUP == CurentMedicine2.MED_GROUP && z.g.DOSAGE_FORM == CurentMedicine2.DOSAGE_FORM)
            //.OrderByDescending(v => v.c.x.CreatedDate)
            //.Select(l => new
            //{
            //    id = l.c.x.Id,
            //    Createdate = l.c.x.CreatedDate,
            //    TotalDuration = l.c.d.TotalDuration
            //}).FirstOrDefault();

            //if (createdDate2 == null)
            //{
            //    check = 0; //vaild to despense

            //}
            //else
            //{
            //    if (DateTime.Now.Date >= createdDate2.Createdate.Value.AddDays(createdDate2.TotalDuration).Date)
            //    {
            //        check = 0;

            //    }
            //    else
            //    {
            //        check = 1;
            //        message = "لقد تم صرف  الدواء البديل من قبل ومازال فى فتره الاستخدام";
            //        return new JsonResult { Data = new { check = check, messa = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            //    }

            //}

            return new JsonResult { Data = new { check = check, messa = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckVip(string id)
        {
            int IsVip = 0;
            DateTime datenow = DateTime.Now.Date;
            var empCardTerminationFlag = db.Comp_Employees.Where(x => x.CARD_ID == id && x.INS_START_DATE <= datenow
            && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            if (empCardTerminationFlag.USR_TYP == "V")
            {
                IsVip = 1; //Accept pending
                return new JsonResult { Data = new { IsVip = IsVip }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                IsVip = 0;
                return new JsonResult { Data = new { IsVip = IsVip }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        public JsonResult Medicines()
        {
            var med = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Join(db.MedicineGroups, d => d.MED_GROUP, g => g.GroupId, (d, g) => new { d, g }).OrderBy(m => m.d.M_CODE).Take(11000)
                .Select(l => new MedicienDataViewModal
                {
                    M_CODE = l.d.M_CODE,
                    TRADE_NAME = l.d.TRADE_NAME,
                    DOSAGE_FORM = l.d.DOSAGE_FORM,
                    PACK_PRICE = l.d.PACK_PRICE,
                    PACK_SIZE = l.d.PACK_SIZE,
                    UNIT_NO = l.d.UNIT_NO,
                    UNIT_PRICE = l.d.UNIT_PRICE,
                    Group_Type = l.g.GroupType,
                    IsCovered = l.d.IsCovered,
                    MED_GROUP = l.d.MED_GROUP
                }).ToList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = serializer.Serialize(med),
                ContentType = "application/json"
            };
            return new JsonResult { Data = med, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        public JsonResult Medicines2()
        {
            var med = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Join(db.MedicineGroups, d => d.MED_GROUP, g => g.GroupId, (d, g) => new { d, g }).OrderBy(m => m.d.M_CODE).Skip(11000)//.Take(10000)
                .Select(l => new MedicienDataViewModal
                {
                    M_CODE = l.d.M_CODE,
                    TRADE_NAME = l.d.TRADE_NAME,
                    DOSAGE_FORM = l.d.DOSAGE_FORM,
                    PACK_PRICE = l.d.PACK_PRICE,
                    PACK_SIZE = l.d.PACK_SIZE,
                    UNIT_NO = l.d.UNIT_NO,
                    UNIT_PRICE = l.d.UNIT_PRICE,
                    Group_Type = l.g.GroupType,
                    IsCovered = l.d.IsCovered,
                    MED_GROUP = l.d.MED_GROUP
                }).ToList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = serializer.Serialize(med),
                ContentType = "application/json"
            };
            return new JsonResult { Data = med, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

        }
        public JsonResult MedicinesDiagnoises(List<Diagnose> Diagnoises)
        {
            List<MedicienDataViewModal> MedicinesData = new List<MedicienDataViewModal>();
            foreach (Diagnose item in Diagnoises)
            {
                int id = db.Diagnosis.Where(m => m.DIAG_CODE == item.DIAG_CODE).FirstOrDefault().Id;
                var Medicines = db.MedicinesDiagnosis.Where(x => x.DiagnoiseId == id).ToList();
                foreach (MedicinesDiagnosi Medicine in Medicines)
                {
                    // Medicine.MedicineId=db.MedicineDatas.Where(m=>m.M_CODE==Medicine.MedicineId)
                    MedicienDataViewModal Data = db.MedicineDatas.Where(x => x.Id == Medicine.MedicineId)
                        .Join(db.MedicineGroups, d => d.MED_GROUP, g => g.GroupId, (d, g) => new { d, g }).OrderBy(m => m.d.M_CODE)
                        .AsEnumerable().Select(l => new MedicienDataViewModal
                        {
                            M_CODE = l.d.M_CODE,
                            TRADE_NAME = l.d.TRADE_NAME,
                            DOSAGE_FORM = l.d.DOSAGE_FORM,
                            PACK_PRICE = l.d.PACK_PRICE,
                            PACK_SIZE = Convert.ToInt32(l.d.PACK_SIZE),
                            UNIT_NO = Convert.ToInt32(l.d.UNIT_NO),
                            UNIT_PRICE = l.d.UNIT_PRICE,
                            Group_Type = l.g.GroupType,
                            IsCovered = Convert.ToBoolean(l.d.IsCovered)
                        }).FirstOrDefault();
                    if (Data != null)
                    {
                        bool IsExsit = false;
                        foreach (var item2 in MedicinesData)
                        {
                            if (item2.M_CODE == Data.M_CODE)
                            {
                                IsExsit = true;
                            }
                        }
                        if (IsExsit == false)
                        {
                            MedicinesData.Add(Data);
                        }
                    }
                }
            }
            return new JsonResult { Data = MedicinesData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult MedicinesGroupValiadtion(List<MedicineData> medicineGroups)
        {
            string currentMedicine = medicineGroups.FirstOrDefault().TRADE_NAME;
            MedicineData currentMedicineData = db.MedicineDatas.Where(x => x.M_CODE == currentMedicine).FirstOrDefault();
            List<MedicineData> Groups = new List<MedicineData>();

            bool Samegroup = false;
            foreach (MedicineData item in medicineGroups)
            {
                MedicineData Group = new MedicineData();
                if (item.M_CODE != null)//not include first medicine
                {
                    Group = db.MedicineDatas.Where(x => x.M_CODE == item.M_CODE).FirstOrDefault();
                    Groups.Add(Group);
                }
            }
            foreach (MedicineData item in Groups)
            {
                if (currentMedicineData.MED_GROUP == item.MED_GROUP && currentMedicineData.DOSAGE_FORM == item.DOSAGE_FORM)
                {
                    Samegroup = true;
                }
            }
            return new JsonResult { Data = Samegroup, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //MedicinesGroupValiadtionAll
        public JsonResult MedicinesGroupValiadtionAll(string CardId, string MedicineCode)
        {
            DateTime MonthlyDate = DateTime.UtcNow.Date.AddDays(-28);
            DateTime CurrentDate = DateTime.UtcNow.Date;
            var Roshitas = db.Roshitas.Where(r => r.CardId == CardId)
             .Join(db.RoshitaDetails, x => x.Id, y => y.RoshitaID, (x, y) => new { x, y })
             .Where(l => l.y.MedicienCode == MedicineCode && l.x.CreatedDate >= MonthlyDate && (l.x.Manager == "Monthly" || l.x.Manager == "Pharmacy_Chronic")).ToList();
            List<string> RoshitaCompare = new List<string>();
            foreach (var item in Roshitas.Where(w => w.x.Manager == "Monthly"))
            {
                DateTime DateCompare = item.x.CreatedDate.Value.AddDays(item.y.TotalDuration).Date;
                if (CurrentDate <= DateCompare)
                {
                    RoshitaCompare.Add(item.y.MedicienCode);
                }
            }
            RoshitaCompare.AddRange(Roshitas.Where(w => w.x.Manager == "Pharmacy_Chronic").Select(x => x.y.MedicienCode).ToList());

            //DAily
            DateTime DailyDate = DateTime.UtcNow.Date.AddDays(-5);
            var RoshitasDailies = db.Roshitas.Where(r => r.CardId == CardId)
             .Join(db.RoshitaDetails, x => x.Id, y => y.RoshitaID, (x, y) => new { x, y })
             .Where(l => l.x.CreatedDate >= DailyDate && l.x.Manager == "Daily").Select(x => x.y.MedicienCode).ToList();
            RoshitaCompare.AddRange(RoshitasDailies);


            MedicineData currentMedicineData = db.MedicineDatas.Where(x => x.M_CODE == MedicineCode).FirstOrDefault();
            List<MedicineData> Groups = new List<MedicineData>();
            bool Samegroup = false;
            foreach (var item in RoshitaCompare)
            {
                MedicineData Group = new MedicineData();
                if (item != null)//not include first medicine
                {
                    Group = db.MedicineDatas.Where(x => x.M_CODE == item).FirstOrDefault();
                    Groups.Add(Group);
                }
            }
            foreach (MedicineData item in Groups)
            {
                if (currentMedicineData.MED_GROUP == item.MED_GROUP && currentMedicineData.DOSAGE_FORM == item.DOSAGE_FORM)
                {
                    Samegroup = true;
                }
            }
            return new JsonResult { Data = Samegroup, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }


        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public JsonResult SavePrescription(PrescriptionViewModel data)
        {
            var username = User.Identity.Name;
            var carduse = db.CardUseds.Where(c => c.CardId == data.CardId && c.CreatedBy == username).FirstOrDefault();
            if (carduse == null)
            {
                return Json("Failed");
            }
            var companyId = data.CardId.Split('-')[0];
            CardCode modelcode = new CardCode();
            if (companyId == "888" && data.ClaimNumber != 10)
            {
                var claimchick = data.ClaimNumber.ToString();
                modelcode = db.CardCodes.Where(x => x.Code == claimchick && x.CardId == data.CardId && x.IsActive && !x.IsUsed).FirstOrDefault();
                if (modelcode == null)
                {
                    return Json("Failed");
                }
                modelcode.IsActive = false;
                modelcode.IsUsed = true;
                db.Entry(modelcode).State = EntityState.Modified;
            }
            //Roshita
            Roshita roshita = new Roshita()
            {
                CardId = data.CardId,
                RoshetaType = data.RoshetaType,
                CompanyPercent = data.CompanyPercent,
                Limit = data.Limit,
                Speciality = data.Speciality,
                Diagnose1 = data.Diagnose1,
                Diagnose2 = data.Diagnose2,
                TotalValue = Math.Round(data.TotalValue.Value, 2),
                PersonPayment = Math.Round(data.PersonPayment, 2),
                CompanyPayment = Math.Round(data.CompanyPayment, 2),
                OverInsurance = Math.Round(data.OverInsurance.Value, 2),
                Cash = Math.Round(data.Cash.Value, 2),
                PhoneNumber = data.PhoneNumber,
                ClaimNumber = data.ClaimNumber,
                CreatedBy = data.CreatedBy == null ? User.Identity.Name : data.CreatedBy,
                CreatedDate = DateTime.Now,
                Manager = data.RoshetaType == "11601" ? "Daily" : data.RoshetaType == "11603" ? "Monthly" : data.RoshetaType,
                IsSync = null,
                SyncDate = null,
                SyncBy = null,
                IsFamily = data.IsFamily,
                IsPool = data.IsPool,
            };

            db.Roshitas.Add(roshita);
            db.CardUseds.Remove(carduse);
            db.SaveChanges();
            // RoshitaDetails
            bool oneNotification = false;
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                Medicien.RoshitaID = roshita.Id;
                if (Medicien.PaymentGroup == "Pending" || Medicien.PaymentGroup == "PendingChronic")
                {
                    if (oneNotification == false)
                    {
                        string CardId = db.Roshitas.Where(x => x.Id == Medicien.RoshitaID).FirstOrDefault().CardId;
                        NotificationHub objNotifHub = new NotificationHub();
                        Notification notification = new Notification();
                        notification.SentTo = CardId.Split('-')[0] == "888" ? "AdminHelth" : "Admin";
                        notification.CreatedBy = User.Identity.Name;
                        notification.CreatedDate = DateTime.Now;
                        notification.Type = 1;//pending
                        notification.TypeNmae = "Medicine";//pending
                        notification.Details = CardId;
                        notification.RoshitaId = roshita.Id;
                        notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                        notification.Title = "Pending";
                        db.Notifications.Add(notification);
                        objNotifHub.SendMessages();
                        oneNotification = true;
                    }
                    Medicien.IsDealed = false;
                }
                else
                {
                    Medicien.IsDealed = true;
                }
                db.RoshitaDetails.Add(Medicien);
            }
            //SaveDiagnoises
            foreach (Diagnose item in data.diagnose)
            {
                PrescriptionRoshitaDignosi dignosi = new PrescriptionRoshitaDignosi();
                dignosi.RositaId = roshita.Id;
                dignosi.DiagnoiseName = item.DIAG_ANAME;
                db.PrescriptionRoshitaDignosis.Add(dignosi);
            }
            //SaveDealApproval
            if (data.hasApproval)
            {
                int EmpId = db.Comp_Employees.Where(c => c.CARD_ID == roshita.CardId && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault().Id;
                var accptionlist = db.Acceptions.Where(x => x.CompEmployeesId == EmpId && x.AcceptionFlag == true).ToList();
                foreach (var item in accptionlist)
                {
                    if (item.ApprovalType != "Vip")
                    {
                        item.AcceptionFlag = false;
                        item.UpdatedDate = DateTime.Now;
                        item.UpdatedBy = User.Identity.Name;
                        db.Entry(item).State = EntityState.Modified;
                    }
                }
                RoshitaAcception roshitaAcception = new RoshitaAcception();
                roshitaAcception.AcceptionId = accptionlist.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                roshitaAcception.RoshitaId = roshita.Id;
                db.RoshitaAcceptions.Add(roshitaAcception);
            }
            try
            {
                if (roshita.CompanyPayment > 0)
                {
                    var EmpCode = roshita.CardId.Split('-')[2];
                    var CompCodeCard = roshita.CardId.Split('-')[0];
                    if (data.IsFamily == "Y" && data.IsPool != "Y")
                    {
                        var remaining = db.RemainConsumptions.Where(r => SqlFunctions.PatIndex(CompCodeCard + "-%-" + EmpCode + "-%", r.CARD_ID) > 0)
                        .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = remaining.REMAINING - roshita.CompanyPayment;
                            remaining.NET = remaining.NET + roshita.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }
                    if (data.IsPool == "Y")
                    {
                        var CompCode = int.Parse(roshita.CardId.Split('-')[0]);
                        var remaining = db.CONSUMPTION_POOL.Where(r => r.COMP_ID == CompCode)
                            .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = remaining.REMAINING - roshita.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }
                    if (data.IsFamily != "Y" && data.IsPool != "Y")
                    {
                        var remaining = db.RemainConsumptions.Where(r => r.CARD_ID == roshita.CardId)
                            .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = remaining.REMAINING - roshita.CompanyPayment;
                            remaining.NET = remaining.NET + roshita.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }

                }
                int result = db.SaveChanges();
                var model = db.CardsSms.Where(c => c.CardId == roshita.CardId).FirstOrDefault();
                if (model != null)
                {
                    try
                    {
                        PostSMSData("Your medication card has been dispensed . If it is not used, please call 0226390390 ", model.Phone);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);

            }
            catch (DbEntityValidationException e)
            {
                db.Roshitas.Remove(roshita);
                db.SaveChanges();
                return Json("Failed to Save Prescription");
                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //            ve.PropertyName, ve.ErrorMessage);
                //    }
                //}
                //throw;
            }

        }

        public string PostSMSData(string Message, string PhoneNumber)
        {
            string requestXml =
                "<SubmitSMSRequest xmlns='http://www.edafa.com/web2sms/sms/model/'>" +
                "<AccountId>200001555</AccountId>" +
                "<Password>Vodafone.1</Password>" +
                "<SecureHash>" + SecretHashMethod(Message, PhoneNumber) + "</SecureHash>" +
                "<SMSList>" +
                "<SenderName>DIAMOND MED</SenderName>" +
                "<ReceiverMSISDN>" + PhoneNumber + "</ReceiverMSISDN>" +
                "<SMSText>" + Message + "</SMSText>" +
                "</SMSList>" +
                "</SubmitSMSRequest>";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://e3len.vodafone.com.eg/web2sms/sms/submit/");
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.ContentType = "application/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }
            return null;
        }
        private string SecretHashMethod(string Message, string PhoneNumber)
        {
            string secret = "B88551A75DC04D78BB92ABAD298BB19F";
            StringBuilder SecretHash = new StringBuilder();

            //var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes("AccountId=200001555&Password=Vodafone.1&SenderName=DIAMOND MED&ReceiverMSISDN=" + PhoneNumber + "&SMSText=" + Message);
            //byte[] messageBytes = encoding.GetBytes("AccountId=200001555&Password=Vodafone.1&SenderName=DIAMOND MED&ReceiverMSISDN=01028599477&SMSText=Hello World");
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                foreach (Byte b in hashmessage)
                    SecretHash.Append(b.ToString("x2"));
                return SecretHash.ToString().ToUpper();
            }
        }
        public JsonResult AgeAndGender(string id)
        {
            this.Id = id;
            return Json(new { ok = this.Id }, JsonRequestBehavior.AllowGet);
        }
        //DisableCard
        public JsonResult DisableCard(string CardId)
        {
            int res;
            var found = db.CardUseds.Where(c => c.CardId == CardId).FirstOrDefault();
            if (found != null)
            {
                if (found.CreatedBy == User.Identity.Name)
                {
                    return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
                return new JsonResult { Data = "False", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            CardUsed cardUsed = new CardUsed
            {
                CardId = CardId,
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.Name
            };
            db.CardUseds.Add(cardUsed);
            res = db.SaveChanges();
            if (res > 0)
            {
                return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = "False", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Delete DisableCard
        public JsonResult DeleteDisableCard(string CardId)
        {
            var found = db.CardUseds.Where(c => c.CardId == CardId).FirstOrDefault();
            if (found != null)
            {
                db.CardUseds.Remove(found);
                db.SaveChanges();
                return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = "False", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Gender Validation
        public JsonResult GenderValidation(string CardId, string MedicineCode, string Id)
        {
            var EmpGender = db.Comp_Employees.Where(x => x.CARD_ID == CardId).FirstOrDefault().GENDER;
            var MedicineGender = db.MedicineDatas.Where(x => x.M_CODE == MedicineCode).FirstOrDefault().DiagnoiseGender;
            if (Id == "1")
            {
                return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            if (EmpGender == MedicineGender || EmpGender == 0 || EmpGender == null)
            {
                return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            if (MedicineGender == 3)
            {
                return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = "False", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //Age Validation
        public JsonResult AgeValidation(string CardId, string MedicineCode, string Id)
        {
            var EmpBithdate = db.Comp_Employees.Where(x => x.CARD_ID == CardId).FirstOrDefault().BIRTH_DATE;
            var today = DateTime.Today;
            var age = today.Year - (EmpBithdate == null ? today.Year : EmpBithdate.Value.Year);
            string Adaltation = "Child";
            if (Id != "1")
            {
                if (age > 6)
                {
                    Adaltation = "Adult";
                }
                var MedicineAge = db.MedicineDatas.Where(x => x.M_CODE == MedicineCode).FirstOrDefault().DiagnoiseAge;

                if (Adaltation == MedicineAge || MedicineAge == "All" || age == 0)
                {
                    return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                return new JsonResult { Data = "True", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = "False", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //Medicine Is Covered
        public JsonResult ISCovered(string MedicineCode)
        {
            var MedicineCover = db.MedicineDatas.Where(x => x.M_CODE == MedicineCode).FirstOrDefault().IsCovered;
            return new JsonResult { Data = MedicineCover, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //MedicinesDurationValiadtion
        public JsonResult MedicinesDurationValiadtion(string CardId, string MedicineCode)
        {
            DateTime MonthlyDate = DateTime.UtcNow.Date.AddDays(-28);
            DateTime CurrentDate = DateTime.UtcNow.Date;
            var Roshitas = db.Roshitas.Where(r => r.CardId == CardId)
             .Join(db.RoshitaDetails, x => x.Id, y => y.RoshitaID, (x, y) => new { x, y })
             .Where(l => l.y.MedicienCode == MedicineCode && l.x.CreatedDate >= MonthlyDate && (l.x.Manager == "Monthly" || l.x.Manager == "Pharmacy_Chronic")).ToList();
            List<Roshita> RoshitaCompare = new List<Roshita>();
            foreach (var item in Roshitas.Where(w => w.x.Manager == "Monthly"))
            {
                DateTime DateCompare = item.x.CreatedDate.Value.AddDays(item.y.TotalDuration).Date;
                if (CurrentDate <= DateCompare)
                {
                    RoshitaCompare.Add(item.x);
                }
            }
            //DAily
            DateTime DailyDate = DateTime.UtcNow.Date.AddDays(-5);
            var RoshitasDailies = db.Roshitas.Where(r => r.CardId == CardId)
             .Join(db.RoshitaDetails, x => x.Id, y => y.RoshitaID, (x, y) => new { x, y })
             .Where(l => l.y.MedicienCode == MedicineCode && l.x.CreatedDate >= DailyDate && l.x.Manager == "Daily").ToList();
            if (Roshitas.Where(w => w.x.Manager == "Pharmacy_Chronic").Count() == 0 && RoshitaCompare.Count == 0 && RoshitasDailies.Count == 0)
            {
                //add medicine
                return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        //GetLastApproval
        public JsonResult GetLastApproval(string CardId, int Type = 3)
        {
            DateTime datenow = DateTime.Now.Date;
            //Default is pharmacy=3
            int EmpId = db.Comp_Employees.Where(x => x.CARD_ID == CardId && x.INS_START_DATE <= datenow && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault().Id;
            var accption = db.Acceptions.Where(x => x.CompEmployeesId == EmpId && x.AcceptionFlag == true && x.ProvidersId == Type).OrderByDescending(d => d.Id).FirstOrDefault();
            if (accption == null)
            {
                return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id)
                .Select(x => x.AcceptionReason.Name)
                .ToList();
            return new JsonResult { Data = reasons, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //GetRoshitaApproval
        public JsonResult GetRoshitaApproval(int RoshitaId, int Type = 3)
        {
            //Default is pharmacy=3
            var RoshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == RoshitaId).FirstOrDefault();
            if (RoshitaAcception == null)
            {
                return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var accption = db.Acceptions.Where(x => x.Id == RoshitaAcception.AcceptionId && x.ProvidersId == Type).OrderByDescending(d => d.Id).FirstOrDefault();
            if (accption == null)
            {
                return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id)
                .Select(x => x.AcceptionReason.Name)
                .ToList();
            return new JsonResult { Data = reasons, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult CheckType(string CardId)
        {
            DateTime datenow = DateTime.Now.Date;
            int EmpId = db.Comp_Employees.Where(c => c.CARD_ID == CardId && c.INS_START_DATE <= datenow && c.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault().Id;
            var accptionType = db.Acceptions.Where(x => x.CompEmployeesId == EmpId).OrderByDescending(d => d.Id).FirstOrDefault();
            if (accptionType != null)
            {
                if (accptionType.ApprovalType == "Vip")
                {
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        #endregion
        //--------------------------------------------------------------------
        #region pending
        public ActionResult Pending(int NotificationId)
        {
            var model = db.Notifications.Where(n => n.Id == NotificationId && n.IsDeleted == false && n.IsRead == false)
                .Include(x => x.Roshita).Include(r => r.Roshita.RoshitaDetails).Include(rd => rd.Roshita.PrescriptionRoshitaDignosis)
                .FirstOrDefault();
            //model.Roshita.RoshitaDetails = model.Roshita.RoshitaDetails.Where(x => x.PaymentGroup == "Pending" || x.PaymentGroup == "PendingChronic" || x.PaymentGroup == "Accepted"
            //|| x.PaymentGroup == "Rejected").ToList();
            return View(model);
        }
        public ActionResult Pending2(string Id)
        {
            return View();
        }
        public JsonResult ApprovedMedicine(string id, int TxtSearch)
        {
            var emp = db.Roshitas.Where(x => x.CardId == id)
                 .Join(db.RoshitaDetails, r => r.Id, d => d.RoshitaID, (r, d) => new { r, d })
                 .Where(b => b.r.CreatedBy == User.Identity.Name)
                 .Where(c => c.d.RoshitaID == TxtSearch)
                 .Where(c => c.d.PaymentGroup == "Accepted" || c.d.PaymentGroup == "Rejected" || c.d.PaymentGroup == "Pending")
                 .Select(l => new Roshita_RoshitaDetails
                 {
                     Id = l.r.Id,
                     DId = l.d.Id,
                     MedicienName = l.d.MedicienName,
                     Amount = l.d.Amount,
                     CreatedDate = l.r.CreatedDate,
                     PaymentGroup = l.d.PaymentGroup,
                     IsDealed = Convert.ToBoolean(l.d.IsDealed)
                 }).ToList();

            return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ChangeStatus(int MedicineId, string status)
        {
            var emp = db.RoshitaDetails.Where(x => x.Id == MedicineId).FirstOrDefault();
            bool Flag;
            int result = -2;
            if (emp.IsDealed == false)
            {
                if (status == "true")
                {
                    Flag = true;
                    var Roshta = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault();
                    double OldCompanyPayment = Roshta.CompanyPayment;
                    if (emp.PaymentGroup == "Accepted")
                    {
                        Roshta.TotalValue += emp.Amount;
                        double TotalValue = Roshta.TotalValue.Value - (Roshta.Cash.Value - (Roshta.OverInsurance.Value + Roshta.PersonPayment));// - Roshta.Cash.Value;//actally cash 
                        double Limit = Roshta.Limit;

                        double CompanyPercent = Convert.ToDouble(Roshta.CompanyPercent) / 100;
                        double PersonPercent = Math.Round(1 - CompanyPercent, 2);
                        double CompanyPayment = 0;
                        if (Limit != 0)
                        {
                            CompanyPayment = TotalValue * CompanyPercent;
                            if (Limit * CompanyPercent <= CompanyPayment)
                            {
                                Roshta.Cash = Roshta.Cash - Roshta.OverInsurance - Roshta.PersonPayment;
                                Roshta.PersonPayment = Limit * PersonPercent;
                                Limit = Limit * CompanyPercent;
                                Roshta.CompanyPayment = Limit;
                                Roshta.OverInsurance = TotalValue - Limit - Roshta.PersonPayment;
                                Roshta.Cash = Roshta.Cash + Roshta.PersonPayment + Roshta.OverInsurance;//total cash

                            }
                            else
                            {
                                Roshta.CompanyPayment = CompanyPayment;
                                Roshta.Cash -= Roshta.PersonPayment;
                                Roshta.PersonPayment = TotalValue * PersonPercent;
                                Roshta.Cash += Roshta.PersonPayment;

                            }

                        }
                        else
                        {
                            Roshta.CompanyPayment = TotalValue * CompanyPercent;
                            Roshta.Cash -= Roshta.PersonPayment;
                            Roshta.PersonPayment = TotalValue * PersonPercent;
                            Roshta.Cash += Roshta.PersonPayment;
                        }
                        if ((Roshta.CompanyPayment - OldCompanyPayment) > 0)
                        {
                            var remaining = db.RemainConsumptions.Where(r => r.CARD_ID == Roshta.CardId)
                                .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                            if (remaining != null)
                            {
                                remaining.REMAINING = remaining.REMAINING - (Roshta.CompanyPayment - OldCompanyPayment);
                                remaining.NET = remaining.NET + (Roshta.CompanyPayment - OldCompanyPayment);
                                db.Entry(remaining).State = EntityState.Modified;
                            }
                        }
                    }
                    else if (emp.PaymentGroup == "Rejected")
                    {
                        Roshta.TotalValue += emp.Amount;
                        Roshta.Cash += emp.Amount;
                    }
                    db.Entry(Roshta).State = EntityState.Modified;

                }
                else
                {
                    Flag = false;
                }

                emp.IsDealed = Flag;
                db.Entry(emp).State = EntityState.Modified;
                if (status != "N")
                {

                    //string CardId = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault().CardId;
                    NotificationHub objNotifHub = new NotificationHub();
                    Notification notification = db.Notifications.Where(x => x.RoshitaId == emp.RoshitaID).OrderByDescending(x => x.Id).FirstOrDefault();
                    notification.IsRead = true;
                    db.Entry(notification).State = EntityState.Modified;

                    objNotifHub.SendMessages();
                }
                result = db.SaveChanges();
            }
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult UpdatePrescriptionPending(PrescriptionViewModel data)
        {
            try
            {
                //Roshita
                var roshita = db.Roshitas.Where(x => x.Id == data.Id).Include(d => d.RoshitaDetails)
                    .Include(r => r.PrescriptionRoshitaDignosis).FirstOrDefault();
                double OldCompanyPayment = roshita.CompanyPayment;
                roshita.TotalValue = data.TotalValue;
                roshita.OverInsurance = data.OverInsurance;
                roshita.PersonPayment = data.PersonPayment;
                roshita.CompanyPayment = data.CompanyPayment;
                roshita.Cash = data.Cash;

                foreach (var item in data.roshitaDetail)
                {
                    if (item.MedicineNoPay == "true" && (item.PaymentGroup == "Rejected" || item.PaymentGroup == "Accepted"))
                    {
                        roshita.RoshitaDetails.Where(x => x.Id == item.Id).FirstOrDefault().IsDealed = true;
                    }
                }
                db.Entry(roshita).State = EntityState.Modified;
                NotificationHub objNotifHub = new NotificationHub();
                Notification notification = db.Notifications.Where(x => x.RoshitaId == data.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                notification.IsRead = true;
                db.Entry(notification).State = EntityState.Modified;

                objNotifHub.SendMessages();

                if ((roshita.CompanyPayment - OldCompanyPayment) > 0)
                {
                    var remaining = db.RemainConsumptions.Where(r => r.CARD_ID == roshita.CardId)
                        .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (remaining != null)
                    {
                        remaining.REMAINING = remaining.REMAINING - (roshita.CompanyPayment - OldCompanyPayment);
                        remaining.NET = remaining.NET + (roshita.CompanyPayment - OldCompanyPayment);
                        db.Entry(remaining).State = EntityState.Modified;
                    }
                }
                int result = db.SaveChanges();
                var model = db.CardsSms.Where(c => c.CardId == roshita.CardId).FirstOrDefault();
                if (model != null)
                {
                    try
                    {
                        PostSMSData("Your medication card has been dispensed . If it is not used, please call 0226390390", model.Phone);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);

            }
            catch (DbEntityValidationException e)
            {
                return Json("Failed to Save Prescription");
            }

        }

        public JsonResult ChangeStatus2(int ApprovalId, string status)
        {
            var emp = db.RoshitaDetails.Where(x => x.Id == ApprovalId).FirstOrDefault();
            bool Flag;
            int result = -2;
            if (emp.IsDealed == false)
            {
                if (status == "true")
                {
                    Flag = true;
                    var Roshta = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault();
                    if (emp.PaymentGroup == "Accepted")
                    {
                        Roshta.TotalValue += emp.Amount;
                        double TotalValue = Roshta.TotalValue.Value - (Roshta.Cash.Value - (Roshta.OverInsurance.Value + Roshta.PersonPayment));// - Roshta.Cash.Value;//actally cash 
                        double Limit = Roshta.Limit;

                        double CompanyPercent = Convert.ToDouble(Roshta.CompanyPercent) / 100;
                        double PersonPercent = Math.Round(1 - CompanyPercent, 2);
                        double CompanyPayment = 0;
                        if (Limit != 0)
                        {
                            CompanyPayment = TotalValue * CompanyPercent;
                            if (Limit * CompanyPercent <= CompanyPayment)
                            {
                                Roshta.Cash = Roshta.Cash - Roshta.OverInsurance - Roshta.PersonPayment;
                                Roshta.PersonPayment = Limit * PersonPercent;
                                Limit = Limit * CompanyPercent;
                                Roshta.CompanyPayment = Limit;
                                Roshta.OverInsurance = TotalValue - Limit - Roshta.PersonPayment;
                                Roshta.Cash = Roshta.Cash + Roshta.PersonPayment + Roshta.OverInsurance;//total cash

                            }
                            else
                            {
                                Roshta.CompanyPayment = CompanyPayment;
                                Roshta.Cash -= Roshta.PersonPayment;
                                Roshta.PersonPayment = TotalValue * PersonPercent;
                                Roshta.Cash += Roshta.PersonPayment;

                            }

                        }
                        else
                        {
                            Roshta.CompanyPayment = TotalValue * CompanyPercent;
                            Roshta.Cash -= Roshta.PersonPayment;
                            Roshta.PersonPayment = TotalValue * PersonPercent;
                            Roshta.Cash += Roshta.PersonPayment;
                        }

                    }
                    else if (emp.PaymentGroup == "Rejected")
                    {
                        Roshta.TotalValue += emp.Amount;
                        Roshta.Cash += emp.Amount;
                    }
                    db.Entry(Roshta).State = EntityState.Modified;

                }
                else
                {
                    Flag = false;
                }

                emp.IsDealed = Flag;
                db.Entry(emp).State = EntityState.Modified;
                if (status != "N")
                {

                    string CardId = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault().CardId;
                    NotificationHub objNotifHub = new NotificationHub();
                    Notification notification = db.Notifications.Where(x => x.Details == CardId).OrderByDescending(x => x.Id).FirstOrDefault();
                    notification.IsRead = true;
                    db.Entry(notification).State = EntityState.Modified;

                    objNotifHub.SendMessages();
                }
                result = db.SaveChanges();
            }
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult Approvals(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var approvals = db.Roshitas.Where(x => x.CardId == id && x.CreatedBy == User.Identity.Name && (x.Manager == "Daily" || x.Manager == "Monthly" || x.Manager == "Lab" || x.Manager == "Ray"))
              .Join(db.RoshitaDetails, r => r.Id, d => d.RoshitaID, (r, d) => new { r, d }).Where(x => x.d.IsDealed == false).Where(x => x.d.PaymentGroup == "Pending" || x.d.PaymentGroup == "PendingChronic" || x.d.PaymentGroup == "Accepted" || x.d.PaymentGroup == "Rejected")
             .Select(l => new
             {
                 l.r.Id,
                 l.r.CreatedDate

             }).ToList().Distinct();
            return new JsonResult { Data = approvals, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult PendingReport(string id)
        {
            var obj = JsonConvert.DeserializeObject<List<PentingPrintViewModel>>(id);
            var Did = obj.First();
            var RoshitaDetails = db.RoshitaDetails.Where(r => r.Id == Did.Id).FirstOrDefault();
            var data = db.Roshitas.Where(r => r.Id == RoshitaDetails.RoshitaID).FirstOrDefault();
            var patient = db.Comp_Employees.Where(b => b.CARD_ID == data.CardId).FirstOrDefault();
            var Company = db.Contract_Comp.Where(c => c.C_COMP_ID == patient.C_COMP_ID).FirstOrDefault();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Pending.rpt"));
            var y = new List<RTCPendingDatasource>();
            foreach (var item in obj)
            {
                var x = db.RoshitaDetails.Where(d => d.Id == item.Id).FirstOrDefault();
                var z = new RTCPendingDatasource
                {
                    MedicienName = x.MedicienName,
                    Amount = x.Amount
                };
                y.Add(z);
            }
            rd.SetDataSource(y);
            if (patient.EMP_ENAME == null)
            {
                rd.SetParameterValue("PatientName", "Unnamed");
            }
            else
            {
                rd.SetParameterValue("PatientName", patient.EMP_ENAME);
            }
            rd.SetParameterValue("Pharmacy", User.Identity.Name);
            rd.SetParameterValue("Approval", Convert.ToDateTime(data.CreatedDate).ToString("ddMMyyyy") + RoshitaDetails.RoshitaID.ToString());
            rd.SetParameterValue("PhoneNumber", data.PhoneNumber);
            rd.SetParameterValue("CompanyName", Company.C_ENAME);
            rd.SetParameterValue("CardId", data.CardId);
            rd.SetParameterValue("TotalValue", Did.Amount);
            rd.SetParameterValue("Cash", Did.Cash);
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
                return File(stream, "application/pfd", ApprovalDate.ToString("ddMMyyyy") + data.Id.ToString() + ".pdf");
            }
            catch
            {
                throw;
            }
        }

        #endregion
        //--------------------------------------------------------------------------------------------------------
        #region  Control Panel Pharmacy
        //Main Roshta
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            if (User.IsInRole("Admin"))
            {
                ViewBag.ddlUsers = new SelectList(context.Users.ToList(), "UserName", "UserName", User.Identity.Name);
            }
            else if (User.IsInRole("Pharmacy_Admin"))
            {
                var CurrentUser = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                ViewBag.ddlUsers = new SelectList(context.Users.Where(x => x.Provider == CurrentUser.Provider).ToList(), "UserName", "UserName", User.Identity.Name);
            }
            return View();
        }
        //Main Roshta
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult IndexClaim()
        {
            var context = new ApplicationDbContext();

            if (User.IsInRole("Admin"))
            {
                ViewBag.ddlUsers = new SelectList(context.Users.ToList(), "UserName", "UserName", User.Identity.Name);
            }
            else if (User.IsInRole("Pharmacy_Admin"))
            {
                var CurrentUser = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                ViewBag.ddlUsers = new SelectList(context.Users.Where(x => x.Provider == CurrentUser.Provider).ToList(), "UserName", "UserName", User.Identity.Name);
            }
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult DeletedStopRoshita()
        {
            var context = new ApplicationDbContext();
            ViewBag.ddlUsers = new SelectList(context.Users.ToList(), "UserName", "UserName", User.Identity.Name);
            return View();
        }

        public JsonResult StopEditPreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "", string Company = "", string Provider = "", string From = "", string To = "", string CardId = "", string Branch = "", string ApprovalNo = "", string Type = "")
        {

            if (User.IsInRole("Admin"))
            {
                if (To != "")
                {
                    DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                    To = T.ToString();
                }
                var Adminresult = new
                {
                    sEcho = sEcho,
                    aaData = db.fn_StopEditAdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).OrderByDescending(m => m.Id)
               .Select(l => new
               {
                   Id = l.Id,
                   Oracle_Id = l.Oracle_Id,
                   CardId = l.CardId,
                   CompanyPercent = l.CompanyPercent,
                   TotalValue = l.TotalValue,
                   Manager = l.Manager,
                   UpdatedDate = l.UpdatedDate,
                   UpdatedBy = l.UpdatedBy
               }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.fn_StopEditAdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count(),
                    iTotalDisplayRecords = db.fn_StopEditAdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count()
                };
                return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && x.Manager.Contains("Stop")).AsEnumerable()
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
                    CreatedDate = l.UpdatedDate,
                    CreatedBy = l.UpdatedBy
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Count(),
                iTotalDisplayRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };





        }

        public JsonResult PreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "", string Company = "", string Provider = "", string From = "", string To = "", string CardId = "", string Branch = "", string ApprovalNo = "", string Type = "")
        {
            //long lgSearch;
            //long.TryParse(sSearch, out lgSearch);
            if (!User.IsInRole("Admin") && From != "" && To != "")
            {
                //sSearch = sSearch.ToUpper();
                //DateTime LastFiveDays = DateTime.Now.AddDays(-7);
                if (To != "")
                {
                    DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                    To = T.ToString();
                }
                if (User.IsInRole("Pharmacy"))
                {
                    Branch = User.Identity.Name;
                }
                else if (User.IsInRole("Pharmacy_Admin") && Branch == "")
                {
                    var context = new ApplicationDbContext();
                    Provider = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Provider;
                }
                var PharmacyResult = new
                {
                    sEcho = sEcho,
                    aaData = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).OrderByDescending(m => m.Id)
               .Select(l => new
               {
                   Id = l.Id,
                   Oracle_Id = l.Oracle_Id,
                   CardId = l.CardId,
                   CompanyPercent = l.CompanyPercent,
                   TotalValue = l.TotalValue,
                   Manager = l.Manager,
                   CreatedDate = l.CreatedDate,
                   CreatedBy = l.CreatedBy
               }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count(),
                    iTotalDisplayRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count()
                };
                return new JsonResult { Data = PharmacyResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            if (User.IsInRole("Admin"))
            {
                //sSearch = sSearch.ToUpper();
                //DateTime LastFiveDays = DateTime.Now.AddDays(-7);
                if (To != "")
                {
                    DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                    To = T.ToString();
                }
                var Adminresult = new
                {
                    sEcho = sEcho,
                    aaData = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).OrderByDescending(m => m.Id)
               .Select(l => new
               {
                   Id = l.Id,
                   Oracle_Id = l.Oracle_Id,
                   CardId = l.CardId,
                   CompanyPercent = l.CompanyPercent,
                   TotalValue = l.TotalValue,
                   Manager = l.Manager,
                   CreatedDate = l.CreatedDate,
                   CreatedBy = l.CreatedBy
               }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count(),
                    iTotalDisplayRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count()
                };
                return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Lab") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")).AsEnumerable()
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





        }
        public JsonResult PreseptionAdminCount(string Company = "", string Provider = "", string From = "", string To = "", string CardId = "", string Branch = "", string ApprovalNo = "", string Type = "")
        {


            if (To != "")
            {
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                To = T.ToString();
            }
            var Adminresult = db.fn_AdminClamsCounts(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).FirstOrDefault();

            return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult Details(long id)
        {
            ViewBag.ddlSpeciality = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
            PharmacyAdminVm pharmacyAdminVm = new PharmacyAdminVm();
            pharmacyAdminVm.RoshDetsils = db.RoshitaDetails
                 .Join(db.MedicineDatas, d => d.MedicienCode, m => m.M_CODE, (d, m) => new { d, m })
                 .Where(l => l.d.RoshitaID == id && l.d.IsDealed == true)
                 .Select(l => new DoctorContainerViewModel
                 {
                     Id = l.d.Id,
                     MedicienCode = l.d.MedicienCode,
                     MedicienName = l.d.MedicienName,
                     Dose = l.d.Dose,
                     Duration = l.d.Duration,
                     TotalDuration = l.d.TotalDuration,
                     TotalUnits = l.d.TotalUnits,
                     Amount = l.d.Amount,
                     DOSAGE_FORM = l.m.DOSAGE_FORM,
                     UNIT_NO = l.m.UNIT_NO,
                     PACK_PRICE = l.m.PACK_PRICE,
                     PACK_SIZE = l.m.PACK_SIZE,
                     UNIT_PRICE = l.m.UNIT_PRICE
                 })
                 .ToList();
            pharmacyAdminVm.RDignosis = db.PrescriptionRoshitaDignosis.Where(r => r.RositaId == id).ToList();
            pharmacyAdminVm.OldRDignosis = db.RoshitaDiagnosisAdmins.Where(r => r.RoshitaId == id).ToList();
            pharmacyAdminVm.spec = db.Roshitas.Where(r => r.Id == id).First().Speciality;
            //ViewBag.RoshitaId = id;
            return View(pharmacyAdminVm);
        }

        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        [HttpPost]
        public JsonResult PharmacyDelete(long id)
        {
            try
            {
                Roshita roshta = db.Roshitas.Where(c => c.Id == id).FirstOrDefault();
                roshta.UpdatedBy = User.Identity.Name;
                roshta.UpdatedDate = DateTime.Now;
                if (roshta.Manager == "Daily")
                {
                    //string CardId = roshta.CardId;
                    NotificationHub objNotifHub = new NotificationHub();
                    Notification notification = db.Notifications.AsEnumerable().Where(x => x.RoshitaId == roshta.Id && x.CreatedDate.ToShortDateString() == roshta.CreatedDate.Value.ToShortDateString()).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (notification != null)
                    {
                        notification.IsRead = true;
                        db.Entry(notification).State = EntityState.Modified;
                        db.SaveChanges();
                        objNotifHub.SendMessages();
                    }
                    roshta.Manager = "Daily_Stop";
                }
                else if (roshta.Manager == "Monthly")
                {
                    //string CardId = roshta.CardId;
                    NotificationHub objNotifHub = new NotificationHub();
                    Notification notification = db.Notifications.AsEnumerable().Where(x => x.RoshitaId == roshta.Id && x.CreatedDate.ToShortDateString() == roshta.CreatedDate.Value.ToShortDateString()).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (notification != null)
                    {
                        notification.IsRead = true;
                        db.Entry(notification).State = EntityState.Modified;
                        db.SaveChanges();
                        objNotifHub.SendMessages();
                    }
                    roshta.Manager = "Monthly_Stop";
                }
                else if (roshta.Manager == "Pharmacy_Chronic")
                {
                    roshta.Manager = "Pharmacy_Chronic_Stop";
                    string CardId = db.Roshitas.Where(x => x.Id == id).FirstOrDefault().CardId;
                    var roshitaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == id).ToList();
                    long DoctorChronicRositaId = db.Roshitas.Where(x => x.CardId == CardId && x.Manager == "Doctor_Chronic").OrderByDescending(x => x.CreatedDate).FirstOrDefault().Id;
                    var DoctrorchronicRositaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == DoctorChronicRositaId).ToList();
                    //update rostita with Doctor_chronic
                    //update med_medicine
                    Med_Medicine medMedicine = new Med_Medicine();

                    foreach (RoshitaDetail item in roshitaDetails)
                    {
                        medMedicine = db.Med_Medicine.Where(x => x.CARD_NO == CardId && x.MED_CODE == item.MedicienCode).FirstOrDefault();
                        if (medMedicine != null)
                        {
                            medMedicine.EXCESS += (item.Dose * item.Duration) - (medMedicine.NO_OF_UINT * (medMedicine.PACK_SIZE / medMedicine.UNIT_NO));
                            if (medMedicine.EXCESS >= (medMedicine.PACK_SIZE / medMedicine.UNIT_NO) || medMedicine.EXCESS < 0)
                            {
                                medMedicine.EXCESS = 0;
                            }
                            medMedicine.NO_OF_UINT = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(((item.Dose * item.Duration) - Convert.ToDouble(medMedicine.EXCESS)) / (Convert.ToDouble(medMedicine.PACK_SIZE) / Convert.ToDouble(medMedicine.UNIT_NO)))));
                            medMedicine.TOTAL_AMT = medMedicine.NO_OF_UINT * medMedicine.UNIT_PRICE;
                            medMedicine.SyncBy = "Updated";
                            db.Entry(medMedicine).State = EntityState.Modified;
                            foreach (RoshitaDetail item2 in DoctrorchronicRositaDetails)
                            {
                                if (item.MedicienCode == item2.MedicienCode)
                                {
                                    item2.TotalUnits = medMedicine.NO_OF_UINT;
                                    item2.Amount = medMedicine.TOTAL_AMT.Value;
                                    item2.IsDealed = false;
                                    db.Entry(item2).State = EntityState.Modified;
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
                else if (roshta.Manager == "Pharmacy_Doctor")
                {
                    roshta.Manager = "Pharmacy_Doctor_Stop";
                    var roshitawithdetails = db.Roshitas.Include(x=>x.RoshitaDetails).Where(x => x.Id == id).FirstOrDefault();
                    //var roshitaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == id).ToList();
                    var DoctorRosita = db.Roshitas.Include(x => x.RoshitaDetails).Where(x => x.CardId == roshitawithdetails.CardId && (x.Manager == "Doctor_Daily" /*|| x.Manager == "Doctor_Chronic"*/))
                        .OrderByDescending(x => x.CreatedDate).ToList();
                    long DoctorRositaId=0;
                    foreach (var item in DoctorRosita)
                    {
                        var details = item.RoshitaDetails.Where(x => x.MedicienCode == roshitawithdetails.RoshitaDetails.ElementAt(0).MedicienCode).FirstOrDefault();
                        if(details!=null)
                        {
                            DoctorRositaId = details.RoshitaID;
                            break;
                        }
                    }
                    var DoctrorchronicRositaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == DoctorRositaId).ToList();
                    foreach (RoshitaDetail item in roshitawithdetails.RoshitaDetails)
                    {
                        foreach (RoshitaDetail item2 in DoctrorchronicRositaDetails)
                        {
                            if (item.MedicienCode == item2.MedicienCode)
                            {
                                item2.IsDealed = false;
                                db.Entry(item2).State = EntityState.Modified;
                            }
                        }

                    }
                    db.SaveChanges();
                }
                roshta.SyncBy = "Update";
                db.Entry(roshta).State = EntityState.Modified;
                if (roshta.CompanyPayment > 0)
                {

                    var EmpCode = roshta.CardId.Split('-')[2];
                    var CompCodeCard = roshta.CardId.Split('-')[0];
                    if (roshta.IsFamily == "Y" && roshta.IsPool != "Y")
                    {
                        var remaining = db.RemainConsumptions.Where(r => SqlFunctions.PatIndex(CompCodeCard + "-%-" + EmpCode + "-%", r.CARD_ID) > 0)
                        .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = remaining.REMAINING + roshta.CompanyPayment;
                            remaining.NET = remaining.NET - roshta.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }
                    if (roshta.IsPool == "Y")
                    {
                        var CompCode = int.Parse(roshta.CardId.Split('-')[0]);
                        var remaining = db.CONSUMPTION_POOL.Where(r => r.COMP_ID == CompCode)
                            .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = remaining.REMAINING + roshta.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }
                    if (roshta.IsFamily != "Y" && roshta.IsPool != "Y")
                    {
                        var remaining = db.RemainConsumptions.Where(r => r.CARD_ID == roshta.CardId)
                       .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = remaining.REMAINING + roshta.CompanyPayment;
                            remaining.NET = remaining.NET - roshta.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }


                }
                db.SaveChanges();
                return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]

        public ActionResult Edit(long id)
        {
            var roshita = db.Roshitas.Where(r => r.Id == id && !r.Manager.Contains("Stop")).FirstOrDefault();
            if (roshita != null)
            {
                List<DoctorContainerViewModel> data = db.RoshitaDetails
                     .Join(db.MedicineDatas, d => d.MedicienCode, m => m.M_CODE, (d, m) => new { d, m })
                     .Where(l => l.d.RoshitaID == id)
                     .Select(l => new DoctorContainerViewModel
                     {
                         Id = l.d.Id,
                         MedicienCode = l.d.MedicienCode,
                         MedicienName = l.d.MedicienName,
                         Dose = l.d.Dose,
                         Duration = l.d.Duration,
                         TotalDuration = l.d.TotalDuration,
                         TotalUnits = l.d.TotalUnits,
                         Amount = l.d.Amount,
                         PaymentGroup = l.d.PaymentGroup,
                         DOSAGE_FORM = l.m.DOSAGE_FORM,
                         UNIT_NO = l.m.UNIT_NO,
                         PACK_PRICE = l.m.PACK_PRICE,
                         PACK_SIZE = l.m.PACK_SIZE,
                         UNIT_PRICE = l.m.UNIT_PRICE,
                         MedicineNoPay = l.d.MedicineNoPay,
                         IsDealed = l.d.IsDealed
                     })
                     .ToList();
                return View(data);
            }
            else
            {
                return HttpNotFound();
            }
        }
        public JsonResult Manger(long id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Rosita = db.Roshitas.Where(r => r.Id == id)
                .Select(l => new
                {
                    Manager = l.Manager,
                    CardId = l.CardId,
                    RoshetaType = l.RoshetaType
                })
                .FirstOrDefault();


            return Json(Rosita, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePrescription(PrescriptionViewModel data)
        {
            //Roshita
            var roshita = db.Roshitas.Where(x => x.Id == data.Id)
                .Include(r => r.PrescriptionRoshitaDignosis).FirstOrDefault();

            Roshita roshita1 = new Roshita();
            roshita1.Manager = roshita.Manager;
            roshita1.CardId = roshita.CardId;
            roshita1.Speciality = roshita.Speciality;
            roshita1.Diagnose1 = roshita.Diagnose1;
            roshita1.Diagnose2 = roshita.Diagnose2;
            roshita1.diagnose3 = roshita.diagnose3;
            roshita1.RoshetaType = roshita.RoshetaType;
            roshita1.CompanyPercent = roshita.CompanyPercent;
            roshita1.Limit = roshita.Limit;
            roshita1.PhoneNumber = roshita.PhoneNumber;
            roshita1.ClaimNumber = roshita.ClaimNumber;
            roshita1.CreatedBy = roshita.CreatedBy;
            roshita1.CreatedDate = roshita.CreatedDate;
            roshita1.UpdatedBy = User.Identity.Name;
            roshita1.UpdatedDate = DateTime.Now;

            roshita1.OverInsurance = data.OverInsurance;
            roshita1.PersonPayment = data.PersonPayment;
            roshita1.CompanyPayment = data.CompanyPayment;
            roshita1.TotalValue = data.TotalValue;
            roshita1.Cash = data.Cash;
            roshita1.PatchId = data.PatchId;
            roshita1.IsFamily = data.IsFamily;
            roshita1.IsPool = data.IsPool;

            roshita.Manager = "Stop-ED";
            roshita.SyncBy = "Update";
            roshita.UpdatedBy = User.Identity.Name;
            roshita.UpdatedDate = DateTime.Now;


            db.Entry(roshita).State = EntityState.Modified;
            //if (ModelState.IsValid)
            //{
            //    //db.Entry(roshita).State = EntityState.Modified;
            //    //db.Roshitas.Add(roshita1);
            //    RoshitaAcception roshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == data.Id).FirstOrDefault();
            //    if (roshitaAcception != null)
            //    {
            //        db.RoshitaAcceptions.Remove(roshitaAcception);

            //    }
            //    // db.SaveChanges();
            //}
            // RoshitaDetails
            List<RoshitaDetail> List_R_Details = db.RoshitaDetails.Where(x => x.RoshitaID == data.Id).ToList();
            bool oneNotification = (List_R_Details.Where(x => x.RoshitaID == data.Id && (x.PaymentGroup == "Pending" || x.PaymentGroup == "PendingChronic")).ToList().Count == 0) ? false : true;
            if (oneNotification)
            {
                var noteficationdelete = db.Notifications.Where(n => n.RoshitaId == roshita.Id).FirstOrDefault();
                noteficationdelete.IsDeleted = true;
                noteficationdelete.IsRead = true;
                db.Entry(noteficationdelete).State = EntityState.Modified;
                oneNotification = false;
            }
            if (roshita1.Manager == "Pharmacy_Chronic")
            {
                foreach (RoshitaDetail OldMedicien in List_R_Details)
                {
                    bool IsExist = false;
                    foreach (RoshitaDetail NewMedicien in data.roshitaDetail)
                    {
                        if (OldMedicien.MedicienCode == NewMedicien.MedicienCode)
                        {
                            IsExist = true;
                            break;
                        }
                    }
                    if (!IsExist)//==false
                    {
                        //removed from list
                        Med_Medicine medMedicine = db.Med_Medicine.Where(x => x.CARD_NO == roshita.CardId && x.MED_CODE == OldMedicien.MedicienCode).FirstOrDefault();
                        if (medMedicine != null)
                        {
                            medMedicine.EXCESS += (OldMedicien.Dose * OldMedicien.Duration) - (medMedicine.NO_OF_UINT * (medMedicine.PACK_SIZE / medMedicine.UNIT_NO));
                            if (medMedicine.EXCESS >= (medMedicine.PACK_SIZE / medMedicine.UNIT_NO) || medMedicine.EXCESS < 0)
                            {
                                medMedicine.EXCESS = 0;
                            }
                            medMedicine.NO_OF_UINT = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(((OldMedicien.Dose * OldMedicien.Duration) - Convert.ToDouble(medMedicine.EXCESS)) / (Convert.ToDouble(medMedicine.PACK_SIZE) / Convert.ToDouble(medMedicine.UNIT_NO)))));
                            medMedicine.TOTAL_AMT = medMedicine.NO_OF_UINT * medMedicine.UNIT_PRICE;
                            medMedicine.SyncBy = "Updated";
                            db.Entry(medMedicine).State = EntityState.Modified;
                            long DoctorChronicRositaId = db.Roshitas.Where(x => x.CardId == roshita.CardId && x.Manager == "Doctor_Chronic").OrderByDescending(x => x.CreatedDate).FirstOrDefault().Id;
                            RoshitaDetail DoctrorchronicRositaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == DoctorChronicRositaId && x.MedicienCode == OldMedicien.MedicienCode).FirstOrDefault();
                            DoctrorchronicRositaDetails.TotalUnits = medMedicine.NO_OF_UINT;
                            DoctrorchronicRositaDetails.Amount = medMedicine.TOTAL_AMT.Value;
                            DoctrorchronicRositaDetails.IsDealed = false;
                            DoctrorchronicRositaDetails.MedicineNoPay = OldMedicien.MedicineNoPay;
                            db.Entry(DoctrorchronicRositaDetails).State = EntityState.Modified;


                        }
                    }
                }

            }

            roshita1.RoshitaDetails = new List<RoshitaDetail>();
            var oldpending = db.RoshitaDetails.Where(r => r.RoshitaID == data.Id && (r.PaymentGroup == "Pending" || r.PaymentGroup == "PendingChronic" || r.PaymentGroup == "Accepted"
            || r.PaymentGroup == "Rejected") && r.IsDealed == false).ToList();
            foreach (var old in oldpending)
            {
                RoshitaDetail oldMedicien = new RoshitaDetail
                {
                    MedicienCode = old.MedicienCode,
                    MedicienName = old.MedicienName,
                    Dose = old.Dose,
                    Duration = old.Duration,
                    TotalDuration = old.TotalDuration,
                    TotalUnits = old.TotalUnits,
                    Amount = old.Amount,
                    IsDealed = old.IsDealed,
                    PaymentGroup = old.PaymentGroup,
                    MedicineNoPay = old.MedicineNoPay
                };
                roshita1.RoshitaDetails.Add(oldMedicien);
                if (oldMedicien.PaymentGroup == "Pending" || oldMedicien.PaymentGroup == "PendingChronic")
                {
                    if (oneNotification == false)
                    {
                        //NotificationHub objNotifHub = new NotificationHub();

                        Notification notification = new Notification();
                        notification.SentTo = roshita1.CardId.Split('-')[0] == "888" ? "AdminHelth" : "Admin";
                        notification.CreatedBy = User.Identity.Name;
                        notification.CreatedDate = DateTime.Now;
                        notification.Type = 1;//pending
                        notification.TypeNmae = "Medicine";//pending
                        notification.Details = roshita1.CardId;
                        //notification.RoshitaId = roshita1.Id;
                        notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                        notification.Title = "Pending";
                        roshita1.Notifications.Add(notification);
                        oneNotification = true;
                    }
                }
                old.PaymentGroup = old.PaymentGroup + "-Stop";
                db.Entry(old).State = EntityState.Modified;

            }

            roshita1.PrescriptionRoshitaDignosis = new List<PrescriptionRoshitaDignosi>();
            foreach (var item in roshita.PrescriptionRoshitaDignosis)
            {
                roshita1.PrescriptionRoshitaDignosis.Add(new PrescriptionRoshitaDignosi
                {
                    DiagnoiseName = item.DiagnoiseName
                });
            }

            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                Medicien.RoshitaID = 0;
                Medicien.IsSync = null;
                Medicien.SyncBy = null;
                Medicien.SyncDate = null;
                if (Medicien.PaymentGroup == "Pending" || Medicien.PaymentGroup == "PendingChronic")
                {
                    if (oneNotification == false)
                    {
                        //NotificationHub objNotifHub = new NotificationHub();

                        Notification notification = new Notification();
                        notification.SentTo = roshita1.CardId.Split('-')[0] == "888" ? "AdminHelth" : "Admin";
                        notification.CreatedBy = User.Identity.Name;
                        notification.CreatedDate = DateTime.Now;
                        notification.Type = 1;//pending
                        notification.TypeNmae = "Medicine";//pending
                        notification.Details = roshita1.CardId;
                        //notification.RoshitaId = roshita1.Id;
                        notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                        notification.Title = "Pending";
                        roshita1.Notifications.Add(notification);
                        //db.Notifications.Add(notification);
                        //objNotifHub.SendMessages();
                        oneNotification = true;
                    }
                    Medicien.IsDealed = false;
                }
                else
                {
                    Medicien.IsDealed = true;
                }
                roshita1.RoshitaDetails.Add(Medicien);
            }
            try
            {
                db.Roshitas.Add(roshita1);
                if (roshita1.CompanyPayment > 0)
                {
                    var EmpCode = roshita1.CardId.Split('-')[2];
                    var CompCodeCard = roshita1.CardId.Split('-')[0];
                    if (data.IsFamily == "Y" && data.IsPool != "Y")
                    {
                        var remaining = db.RemainConsumptions.Where(r => SqlFunctions.PatIndex(CompCodeCard + "-%-" + EmpCode + "-%", r.CARD_ID) > 0)
                        .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = (remaining.REMAINING + roshita.CompanyPayment) - roshita1.CompanyPayment;
                            remaining.NET = (remaining.NET - roshita.CompanyPayment) + roshita1.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }
                    if (data.IsPool == "Y")
                    {
                        var CompCode = int.Parse(roshita.CardId.Split('-')[0]);
                        var remaining = db.CONSUMPTION_POOL.Where(r => r.COMP_ID == CompCode)
                            .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = remaining.REMAINING - roshita1.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }
                    if (data.IsFamily != "Y" && data.IsPool != "Y")
                    {
                        var remaining = db.RemainConsumptions.Where(r => r.CARD_ID == roshita1.CardId)
                        .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (remaining != null)
                        {
                            remaining.REMAINING = (remaining.REMAINING + roshita.CompanyPayment) - roshita1.CompanyPayment;
                            remaining.NET = (remaining.NET - roshita.CompanyPayment) + roshita1.CompanyPayment;
                            db.Entry(remaining).State = EntityState.Modified;
                        }
                    }
                }
                int result = db.SaveChanges();
                if (oneNotification)
                {
                    var noteficationdelete = db.Notifications.Where(n => n.RoshitaId == roshita.Id).FirstOrDefault();
                    if (noteficationdelete != null)
                    {
                        noteficationdelete.IsDeleted = true;
                        noteficationdelete.IsRead = true;
                        db.Entry(noteficationdelete).State = EntityState.Modified;
                        int result2 = db.SaveChanges();
                    }
                }
                else
                {
                    var noteficationdelete = db.Notifications.Where(n => n.RoshitaId == roshita.Id).FirstOrDefault();
                    if (noteficationdelete != null)
                    {
                        noteficationdelete.RoshitaId = roshita1.Id;
                        db.Entry(noteficationdelete).State = EntityState.Modified;
                        int result3 = db.SaveChanges();
                    }
                }
                RoshitaAcception roshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == data.Id).FirstOrDefault();
                if (roshitaAcception != null)
                {

                    db.RoshitaAcceptions.Add(new RoshitaAcception
                    {
                        RoshitaId = roshita1.Id,
                        AcceptionId = roshitaAcception.AcceptionId,
                    });
                    db.SaveChanges();
                }
                NotificationHub objNotifHub = new NotificationHub();
                objNotifHub.SendMessages();
                var model = db.CardsSms.Where(c => c.CardId == roshita.CardId).FirstOrDefault();
                if (model != null)
                {
                    try
                    {
                        PostSMSData("Your medication card has been dispensed . If it is not used, please call 0226390390", model.Phone);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita1.Id);

            }
            catch (DbEntityValidationException e)
            {
                return Json("Failed to Save Prescription");
            }

        }

        public JsonResult UpdatePrescription2(PrescriptionViewModel data)
        {
            //Roshita
            var roshita = db.Roshitas.Where(x => x.Id == data.Id).FirstOrDefault();
            roshita.OverInsurance = data.OverInsurance;
            roshita.PersonPayment = data.PersonPayment;
            roshita.CompanyPayment = data.CompanyPayment;
            roshita.TotalValue = data.TotalValue;
            roshita.Cash = data.Cash;
            roshita.SyncBy = "Update";
            roshita.PatchId = data.PatchId;
            if (ModelState.IsValid)
            {
                db.Entry(roshita).State = EntityState.Modified;
                RoshitaAcception roshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == data.Id).FirstOrDefault();
                if (roshitaAcception != null)
                {
                    db.RoshitaAcceptions.Remove(roshitaAcception);

                }
                // db.SaveChanges();
            }
            // RoshitaDetails
            List<RoshitaDetail> List_R_Details = db.RoshitaDetails.Where(x => x.RoshitaID == data.Id).ToList();
            if (roshita.Manager == "Pharmacy_Chronic")
            {
                foreach (RoshitaDetail OldMedicien in List_R_Details)
                {
                    bool IsExist = false;
                    foreach (RoshitaDetail NewMedicien in data.roshitaDetail)
                    {
                        if (OldMedicien.MedicienCode == NewMedicien.MedicienCode)
                        {
                            IsExist = true;
                            break;
                        }
                    }
                    if (!IsExist)//==false
                    {
                        //removed from list
                        Med_Medicine medMedicine = db.Med_Medicine.Where(x => x.CARD_NO == roshita.CardId && x.MED_CODE == OldMedicien.MedicienCode).FirstOrDefault();
                        if (medMedicine != null)
                        {
                            medMedicine.EXCESS += (OldMedicien.Dose * OldMedicien.Duration) - (medMedicine.NO_OF_UINT * (medMedicine.PACK_SIZE / medMedicine.UNIT_NO));
                            if (medMedicine.EXCESS >= (medMedicine.PACK_SIZE / medMedicine.UNIT_NO) || medMedicine.EXCESS < 0)
                            {
                                medMedicine.EXCESS = 0;
                            }
                            medMedicine.NO_OF_UINT = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(((OldMedicien.Dose * OldMedicien.Duration) - Convert.ToDouble(medMedicine.EXCESS)) / (Convert.ToDouble(medMedicine.PACK_SIZE) / Convert.ToDouble(medMedicine.UNIT_NO)))));
                            medMedicine.TOTAL_AMT = medMedicine.NO_OF_UINT * medMedicine.UNIT_PRICE;
                            medMedicine.SyncBy = "Updated";
                            db.Entry(medMedicine).State = EntityState.Modified;
                            long DoctorChronicRositaId = db.Roshitas.Where(x => x.CardId == roshita.CardId && x.Manager == "Doctor_Chronic").OrderByDescending(x => x.CreatedDate).FirstOrDefault().Id;
                            RoshitaDetail DoctrorchronicRositaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == DoctorChronicRositaId && x.MedicienCode == OldMedicien.MedicienCode).FirstOrDefault();
                            DoctrorchronicRositaDetails.TotalUnits = medMedicine.NO_OF_UINT;
                            DoctrorchronicRositaDetails.Amount = medMedicine.TOTAL_AMT.Value;
                            DoctrorchronicRositaDetails.IsDealed = false;
                            db.Entry(DoctrorchronicRositaDetails).State = EntityState.Modified;


                        }
                    }
                }

            }
            var mediciens = List_R_Details.Where(x => x.RoshitaID == data.Id && x.IsDealed == true).ToList();
            db.RoshitaDetails.RemoveRange(mediciens);
            //db.SaveChanges();
            bool oneNotification = (List_R_Details.Where(x => x.RoshitaID == data.Id && (x.PaymentGroup == "Pending" || x.PaymentGroup == "PendingChronic")).ToList().Count == 0) ? false : true; ;
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                if (Medicien.PaymentGroup == "Pending" || Medicien.PaymentGroup == "PendingChronic" || Medicien.PaymentGroup == "Cash")
                {
                    RoshitaDetail roshitaDetail = List_R_Details.Where(x => x.RoshitaID == data.Id && x.MedicienCode == Medicien.MedicienCode && x.IsDealed == false).FirstOrDefault();
                    if (roshitaDetail != null)
                    {
                        if (Medicien.PaymentGroup == "Pending" || Medicien.PaymentGroup == "PendingChronic")
                        {
                            Medicien.PaymentGroup = roshitaDetail.PaymentGroup;
                        }
                        //added before and insert pending or cash
                        oneNotification = true;
                        db.RoshitaDetails.Remove(roshitaDetail);
                        //db.SaveChanges();
                    }

                    if (oneNotification == false && (Medicien.PaymentGroup == "Pending" || Medicien.PaymentGroup == "PendingChronic"))
                    {
                        //if new pending and didn't have notification
                        string CardId = db.Roshitas.Where(x => x.Id == Medicien.RoshitaID).FirstOrDefault().CardId;
                        var NotificationList = db.Notifications.Where(x => x.Details == CardId && x.DetailsURL == "/DoctorMedicinesLabsRaysApproval/index" && x.IsRead == false).ToList();
                        if (NotificationList.Count == 0)
                        {
                            NotificationHub objNotifHub = new NotificationHub();
                            Notification notification = new Notification();
                            notification.SentTo = "Admin";
                            notification.CreatedBy = User.Identity.Name;
                            notification.CreatedDate = DateTime.Now;
                            notification.Type = 1;//pending
                            notification.TypeNmae = "Medicine";//pending
                            notification.Details = CardId;
                            notification.RoshitaId = roshita.Id;
                            notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                            notification.Title = "Pending";
                            db.Notifications.Add(notification);
                            objNotifHub.SendMessages();
                        }
                        oneNotification = true;
                    }
                    Medicien.IsDealed = (Medicien.PaymentGroup == "Cash") ? true : false;
                }
                else
                {
                    Medicien.IsDealed = true;
                }


                db.RoshitaDetails.Add(Medicien);
            }
            try
            {
                int result = db.SaveChanges();
                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);

            }
            catch (DbEntityValidationException e)
            {
                //db.Roshitas.Remove(roshita);
                return Json("Failed to Save Prescription");
                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //            ve.PropertyName, ve.ErrorMessage);
                //    }
                //}
                //throw;
            }

        }

        public ActionResult PrintDeleteEditRoshita(string id)
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
                int? NoOver = 0, NoPay = 0;
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

                    var RoshitaNoOverNoPays = db.RoshitaNoOverNoPays.Where(m => m.RositaId == data.Id).FirstOrDefault();
                    if (RoshitaNoOverNoPays != null)
                    {
                        NoOver = RoshitaNoOverNoPays.NoOver == null ? 0 : RoshitaNoOverNoPays.NoOver;
                        NoPay = RoshitaNoOverNoPays.NoPay == null ? 0 : RoshitaNoOverNoPays.NoPay;
                    }
                    else
                    {
                        med_card = db.Med_Card.Where(m => m.CARD_NO == data.CardId).FirstOrDefault();
                        NoOver = med_card.NO_OVER == null ? 0 : med_card.NO_OVER;
                        NoPay = med_card.NO_PAY == null ? 0 : med_card.NO_PAY;
                    }
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

                rd.SetParameterValue("pay", NoPay);
                rd.SetParameterValue("over", NoOver);
                rd.SetParameterValue("perc", CellingPert);
                rd.SetParameterValue("Type", data.Manager + "(" + data.RoshetaType + ")");
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
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                // throw ex;
                return View("~/Views/Shared/Error.cshtml");

            }
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
                int? NoOver = 0, NoPay = 0;
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
                ////500142+500103+500125+10560
                if (data.ClaimNumber == null && (data.RoshetaType == "11601" || data.RoshetaType == "11603") &&
                    (data.CardId.Contains("500142") || data.CardId.Contains("500103") || data.CardId.Contains("500125") || data.CardId.Contains("10560")))
                {
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RoshitaReport2.rpt"));
                }
                else
                {
                    if (data.Manager == "Daily")
                    {
                        rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RoshitaReportAfterBefore.rpt"));
                    }
                    else
                    {
                        rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RoshitaReport.rpt"));
                    }
                }
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
                    MedicineNoPay = d.r.MedicineNoPay,
                    RealAmount = Convert.ToDouble(d.m.UNIT_PRICE) * Convert.ToDouble(d.r.TotalUnits),
                }).ToList();
                if (y.Count() == 0)
                {
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RoshitaReport.rpt"));

                }
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
            catch (Exception ex)
            {
                ViewBag.ErrorM = ex;
                // throw ex;
                return View("~/Views/Shared/Error.cshtml");

            }
        }

        public ActionResult PrintClams(string From, string To, string Branch = "")
        {
            try
            {
                if (User.IsInRole("Pharmacy"))
                {
                    Branch = User.Identity.Name;
                }
                else if (User.IsInRole("Pharmacy_Admin") && Branch == "")
                {
                    var context = new ApplicationDbContext();
                    Branch = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Provider;
                }

                DateTime F = Convert.ToDateTime(From);
                if (F < Convert.ToDateTime("12/01/2020 12:00:00 AM"))
                    From = "12/01/2020";
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);//to get all pervious day
                if (T < Convert.ToDateTime("01/12/2020 12:00:00 AM"))
                    To = "12/01/2020";
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ClamsReport.rpt"));
                ApplicationDbContext users = new ApplicationDbContext();
                var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                int ProviderId = Convert.ToInt32(CurrentUser.Provider);
                var sericeProviderDiscounts = db.Ser_Prov_Disc.Where(x => x.PROV_ID == ProviderId).FirstOrDefault();

                List<RoshitaCompEmolyessReportViewModel> Data = db.fn_ClaimsReport(From, To, Branch)//.AsEnumerable()
                    .Select(d => new RoshitaCompEmolyessReportViewModel
                    {
                        //Id = d.Oracle_Id==null?0: d.Oracle_Id.Value,
                        Id = d.Id.Value,
                        //Id = 0,
                        CardId = d.CardId,
                        EMP_ENAME = d.EMP_ENAME,
                        Manager = d.Manager,
                        TotalValue = d.TotalValue != null ? d.TotalValue.Value : 0,
                        TotalLocal = d.TotalLocal != null ? d.TotalLocal.Value : 0,// Convert.ToDouble(d.TotalLocal),
                        TotalLocalDiscount = d.TotalLocalDiscount != null ? d.TotalLocalDiscount.Value : 0,// Convert.ToDouble(d.TotalLocalDiscount),
                        TotalLocalDevlopment = d.TotalLocalDevlopment != null ? d.TotalLocalDevlopment.Value : 0,// Convert.ToDouble(d.TotalLocalDevlopment),
                        TotalImport = d.TotalImport != null ? d.TotalImport.Value : 0,// Convert.ToDouble(d.TotalImport),
                        TotalImportDiscount = d.TotalImportDiscount != null ? d.TotalImportDiscount.Value : 0,// Convert.ToDouble(d.TotalImport),
                        TotalImportDevelopment = d.TotalImportDevelopment != null ? d.TotalImportDevelopment.Value : 0,// Convert.ToDouble(d.TotalImport),
                        CompanyPayment = d.CompanyPayment,// Convert.ToDouble(d.CompanyPayment),
                        OverInsurance = d.OverInsurance != null ? d.OverInsurance.Value : 0,// Convert.ToDouble(d.OverInsurance),
                        Cash = d.Cash == null ? d.Cash.Value : 0,// Convert.ToDouble(d.Cash),
                        PersonPayment = d.PersonPayment,// Convert.ToDouble(d.PersonPayment),
                    }).OrderBy(x => x.Id).ToList();
                rd.SetDataSource(Data);

                // string Branch = CurrentUser != null ? CurrentUser.Provider : "";
                rd.SetParameterValue("From", From);
                rd.SetParameterValue("To", To);
                rd.SetParameterValue("Provider", CurrentUser.Provider);
                rd.SetParameterValue("Branch", Branch);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", F.ToString("ddMMyyyy") + "Clams.pdf");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");
                // throw ex.Message("There are no calmes");
            }
        }

        public ActionResult PrintNewXlxClams(string From, string To, string Branch, string Provider
            , string Company, string ApprovalNo, string CardId, string ddlType)
        {
            try
            {
                DateTime F = Convert.ToDateTime(From);
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);//to get all day
                ReportDocument rd = new ReportDocument();


                if (ddlType == "Pharmacy_Data")
                {
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ChronicDataReport.rpt"));
                    rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

                    rd.SetParameterValue("@comp", Company);
                    rd.SetParameterValue("@from", From);
                    rd.SetParameterValue("@to", To);
                }
                else
                {
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AllClaimsReport.rpt"));
                    rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");
                    rd.SetParameterValue("@from", F);
                    rd.SetParameterValue("@to", T);
                    rd.SetParameterValue("@provider", Provider);
                    rd.SetParameterValue("@Company", Company);
                    rd.SetParameterValue("@Branch", Branch);
                    rd.SetParameterValue("@ApprovalNo", ApprovalNo);
                    rd.SetParameterValue("@CardId", CardId);
                    rd.SetParameterValue("@TYPE", ddlType);

                }

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/xls", F.ToString("ddMMyyyy") + "Clams.xls");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");

                throw ex;
            }
        }
        public ActionResult PrintXlxClams(string From, string To)
        {
            try
            {
                DateTime F = Convert.ToDateTime(From);
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);//to get all day
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ClamsReport.rpt"));

                ApplicationDbContext users = new ApplicationDbContext();
                var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                int ProviderId = Convert.ToInt32(CurrentUser.Provider);
                var sericeProviderDiscounts = db.Ser_Prov_Disc.Where(x => x.PROV_ID == ProviderId).FirstOrDefault();


                List<RoshitaCompEmolyessReportViewModel> Data = db.fn_ClaimsReport(From, To, User.Identity.Name)//.AsEnumerable()
                .Select(d => new RoshitaCompEmolyessReportViewModel
                {
                    Id = d.Id.Value,
                    //Id = d.Oracle_Id == null ? 0 : d.Oracle_Id.Value,
                    CardId = d.CardId,
                    EMP_ENAME = d.EMP_ENAME,
                    Manager = d.Manager,
                    TotalValue = d.TotalValue.Value,
                    TotalLocal = d.TotalLocal.Value,// Convert.ToDouble(d.TotalLocal),
                    TotalLocalDiscount = d.TotalLocalDiscount.Value,// Convert.ToDouble(d.TotalLocalDiscount),
                    TotalLocalDevlopment = d.TotalLocalDevlopment.Value,// Convert.ToDouble(d.TotalLocalDevlopment),
                    TotalImport = d.TotalImport.Value,// Convert.ToDouble(d.TotalImport),
                    TotalImportDiscount = d.TotalImportDiscount.Value,// Convert.ToDouble(d.TotalImport),
                    TotalImportDevelopment = d.TotalImportDevelopment.Value,// Convert.ToDouble(d.TotalImport),
                    CompanyPayment = d.CompanyPayment,// Convert.ToDouble(d.CompanyPayment),
                    OverInsurance = d.OverInsurance.Value,// Convert.ToDouble(d.OverInsurance),
                    Cash = d.Cash.Value,// Convert.ToDouble(d.Cash),
                    PersonPayment = d.PersonPayment,// Convert.ToDouble(d.PersonPayment),
                }).OrderBy(x => x.Id).ToList();
                rd.SetDataSource(Data);

                string Branch = CurrentUser != null ? CurrentUser.Provider : "";
                rd.SetParameterValue("From", From);
                rd.SetParameterValue("To", To);
                rd.SetParameterValue("Provider", User.Identity.Name);
                rd.SetParameterValue("Branch", Branch);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/xls", F.ToString("ddMMyyyy") + "Clams.xls");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");

                throw ex;
            }
        }
        public ActionResult GetFile(string fileName)
        {
            try
            {
                string path = "";
                string Name = "";
                if (fileName == "Pharmacy")
                {
                    path = Path.Combine(Server.MapPath("~/assets/ManualFiles/PharmacyManual.pdf"));
                    Name = "PharmacyManual.pdf";
                }
                else if (fileName == "Lab")
                {
                    path = Path.Combine(Server.MapPath("~/assets/ManualFiles/LabManual.pdf"));
                    Name = "LabManual.pdf";
                }
                else if (fileName == "Ray")
                {
                    path = Path.Combine(Server.MapPath("~/assets/ManualFiles/RayManual.pdf"));
                    Name = "RayManual.pdf";
                }
                var htmlCode = System.IO.File.ReadAllBytes(path);
                FileResult fileResult = new FileContentResult(htmlCode, "application/pdf")
                {
                    FileDownloadName = Name
                };
                return fileResult;
            }

            catch (Exception ex)
            {
                return null;
                //return Json("EROOOOOOOOOR");
            }
        }

        public ActionResult PrintPendingDetails(int Id)
        {
            try
            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RoshitaDetails.rpt"));
                rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

                rd.SetParameterValue("@idd", Id);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", DateTime.Now.ToString("ddMMyyyy") + "pendingDetails.pdf");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");

                throw ex;
            }
        }

        public JsonResult SaveDiagnoisesAdmin(string[] DiagnosisList, string Speciality, int Roshitaid)
        {
            var model = db.RoshitaDiagnosisAdmins.Where(r => r.RoshitaId == Roshitaid).ToList();
            if (model.Count > 0)
            {
                db.RoshitaDiagnosisAdmins.RemoveRange(model);
                db.SaveChanges();
            }
            List<RoshitaDiagnosisAdmin> Diagnosis = new List<RoshitaDiagnosisAdmin>();
            foreach (var item in DiagnosisList)
            {
                RoshitaDiagnosisAdmin dignosi = new RoshitaDiagnosisAdmin
                {
                    RoshitaId = Roshitaid,
                    DiagnoiseName = item,
                    SpecialistName = Speciality,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };
                Diagnosis.Add(dignosi);
            }
            db.RoshitaDiagnosisAdmins.AddRange(Diagnosis);
            db.SaveChanges();
            return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion
        //---------------------------------------------------------------------
        #region CompaniesChronicDelivery
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult CompaniesChronicDelivery()
        {

            var CurrentUser = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //int userProvider = Convert.ToInt32(CurrentUser.Provider);
            var medCards = db.Med_Card.Where(m => (m.PROVIDER_CODE.Contains(CurrentUser.Provider) || m.PROVIDER_CODE.Contains("1268")) && m.LOOK_01 == 0)
                .Join(db.Contract_Comp, m => m.C_COMP_ID, c => c.C_COMP_ID, (m, c) => new { m, c }).Select(
                l => new
                {
                    value = l.m.C_COMP_ID,
                    text = l.m.C_COMP_ID + "||" + l.c.C_ENAME,
                }).Distinct().ToList();
            ViewBag.ddlcompanies = new SelectList(medCards, "value", "text");

            return View();
        }

        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult CompaniesChronicDeliveryNew()
        {

            var CurrentUser = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //int userProvider = Convert.ToInt32(CurrentUser.Provider);
            var medCards = db.Med_Card.Where(m => (m.PROVIDER_CODE.Contains(CurrentUser.Provider) || m.PROVIDER_CODE.Contains("1268")) && m.LOOK_01 == 0)
                .Join(db.Contract_Comp, m => m.C_COMP_ID, c => c.C_COMP_ID, (m, c) => new { m, c }).Select(
                l => new
                {
                    value = l.m.C_COMP_ID,
                    text = l.m.C_COMP_ID + "||" + l.c.C_ENAME,
                }).Distinct().ToList();
            ViewBag.ddlcompanies = new SelectList(medCards, "value", "text");

            return View();
        }

        public JsonResult CompanyGroupsList(int CompId)
        {
            var CurrentUser = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //int userProvider = Convert.ToInt32(CurrentUser.Provider);
            var medCards = db.Med_Card.Where(m => m.PROVIDER_CODE.Contains(CurrentUser.Provider) && m.LOOK_01 == 0 && m.C_COMP_ID == CompId)
                .Select(
                l => new
                {
                    value = l.GROUP_ID,
                    text = l.GROUP_ID + "||" + l.GROUP_NAME,
                }).Distinct().ToList();
            return Json(new SelectList(medCards, "value", "text"));
        }

        public JsonResult CompaniesChronicMedicinesList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "", string Date = "", int CompId = 0, int GroupId = 0, string CardId = "")
        {
            DateTime dateTime = Convert.ToDateTime(Date);
            var CurrentUser = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //int userProvider = Convert.ToInt32(CurrentUser.Provider);
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Med_Card.Where(x => (x.PROVIDER_CODE.Contains(CurrentUser.Provider) || x.PROVIDER_CODE.Contains("1268")) && x.LOOK_01 == 0 && x.C_COMP_ID == CompId && (GroupId != 0 ? x.GROUP_ID == GroupId : true) && (CardId != "" ? x.CARD_NO == CardId : true))
                .Join(db.Med_Medicine, MC => MC.CARD_NO, MM => MM.CARD_NO, (MC, MM) => new { MC, MM })
                .Where(x => x.MM.ACTIVE == "Y" && (x.MM.MONTH_DATE_STOP == null || x.MM.MONTH_DATE_STOP >= dateTime) && x.MM.EXCESS == 0)
                .Join(db.Comp_Employees, M => M.MM.CARD_NO, E => E.CARD_ID, (M, E) => new { M, E })
                .Where(x => x.E.INS_START_DATE <= DateTime.Now && x.E.INS_END_DATE >= DateTime.Now && (x.E.TERMINATE_FLAG == "N" || (x.E.TERMINATE_FLAG == "Y" && x.E.TERMINATE_DATE > DateTime.Now ? true : false))).OrderByDescending(x => x.E.CONTRACT_NO)
                .Select(l => new
                {
                    CardId = l.M.MC.CARD_NO,
                    CompanyGroup = l.M.MC.GROUP_NAME,
                    MedicienCode = l.M.MM.MED_CODE,
                    Name = l.M.MM.MED_NAME,
                    Dosage = l.M.MM.DOSAGE_FORM,
                    PackageSize = l.M.MM.PACK_SIZE,
                    PackagePrice = l.M.MM.PACK_PRICE,
                    UnitNumber = l.M.MM.UNIT_NO,
                    UnitPrice = l.M.MM.UNIT_PRICE,
                    Dose = l.M.MM.DOSE,
                    DoseDuration = l.M.MM.DOS_DUR,
                    MedicineDuration = l.M.MM.MED_DURATION,
                    TotalUnits = l.M.MM.NO_OF_UINT,
                    Amount = l.M.MM.TOTAL_AMT,
                    Month = l.M.MM.MONTH_DATE_STOP,
                    Act = l.M.MM.ACT_MONTH
                }).Distinct().OrderByDescending(m => m.CardId).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.Med_Card.Where(x => (x.PROVIDER_CODE.Contains(CurrentUser.Provider) || x.PROVIDER_CODE.Contains("1268")) && x.LOOK_01 == 0 && x.C_COMP_ID == CompId && (GroupId != 0 ? x.GROUP_ID == GroupId : true) && (CardId != "" ? x.CARD_NO == CardId : true))
                .Join(db.Med_Medicine, MC => MC.CARD_NO, MM => MM.CARD_NO, (MC, MM) => new { MC, MM }).Where(x => x.MM.ACTIVE == "Y" && x.MM.EXCESS == 0 && (x.MM.MONTH_DATE_STOP == null || x.MM.MONTH_DATE_STOP >= dateTime)).Join(db.Comp_Employees, M => M.MM.CARD_NO, E => E.CARD_ID, (M, E) => new { M, E }).Where(x => x.E.INS_START_DATE <= DateTime.Now && x.E.INS_END_DATE >= DateTime.Now && (x.E.TERMINATE_FLAG == "N" || (x.E.TERMINATE_FLAG == "Y" && x.E.TERMINATE_DATE > DateTime.Now ? true : false))).OrderByDescending(x => x.E.CONTRACT_NO).Count(),
                iTotalDisplayRecords = db.Med_Card.Where(x => (x.PROVIDER_CODE.Contains(CurrentUser.Provider) || x.PROVIDER_CODE.Contains("1268")) && x.LOOK_01 == 0 && x.C_COMP_ID == CompId && (GroupId != 0 ? x.GROUP_ID == GroupId : true) && (CardId != "" ? x.CARD_NO == CardId : true))
                .Join(db.Med_Medicine, MC => MC.CARD_NO, MM => MM.CARD_NO, (MC, MM) => new { MC, MM }).Where(x => x.MM.ACTIVE == "Y" && x.MM.EXCESS == 0 && (x.MM.MONTH_DATE_STOP == null || x.MM.MONTH_DATE_STOP >= dateTime)).Join(db.Comp_Employees, M => M.MM.CARD_NO, E => E.CARD_ID, (M, E) => new { M, E }).Where(x => x.E.INS_START_DATE <= DateTime.Now && x.E.INS_END_DATE >= DateTime.Now && (x.E.TERMINATE_FLAG == "N" || (x.E.TERMINATE_FLAG == "Y" && x.E.TERMINATE_DATE > DateTime.Now ? true : false))).OrderByDescending(x => x.E.CONTRACT_NO).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult CompaniesChronicMedicinesListCount(string Date = "", int CompId = 0, int GroupId = 0, string CardId = "")
        {
            var CurrentUser = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //int userProvider = Convert.ToInt32(CurrentUser.Provider);
            DateTime dateTime = Convert.ToDateTime(Date);

            var Cards = db.Med_Card.Where(x => (x.PROVIDER_CODE.Contains(CurrentUser.Provider) || x.PROVIDER_CODE.Contains("1268")) && x.LOOK_01 == 0 && x.C_COMP_ID == CompId && (GroupId != 0 ? x.GROUP_ID == GroupId : true) && (CardId != "" ? x.CARD_NO == CardId : true))
                .Join(db.Med_Medicine, MC => MC.CARD_NO, MM => MM.CARD_NO, (MC, MM) => new { MC, MM }).Where(x => x.MM.ACTIVE == "Y" && x.MM.EXCESS == 0 && (x.MM.MONTH_DATE_STOP == null || x.MM.MONTH_DATE_STOP >= dateTime)).Join(db.Comp_Employees, M => M.MM.CARD_NO, E => E.CARD_ID, (M, E) => new { M, E }).Where(x => x.E.INS_START_DATE <= DateTime.Now && x.E.INS_END_DATE >= DateTime.Now && (x.E.TERMINATE_FLAG == "N" || (x.E.TERMINATE_FLAG == "Y" && x.E.TERMINATE_DATE > DateTime.Now ? true : false))).OrderByDescending(x => x.E.CONTRACT_NO)
                .Select(l => new
                {
                    CardId = l.M.MC.CARD_NO,
                    Amount = l.M.MM.TOTAL_AMT

                }).ToList();
            var Adminresult = new
            {
                TotalCounts = Cards.Count(),
                TotalCards = Cards.Select(x => x.CardId).Distinct().Count(),
                TotalValue = Cards.Sum(x => x.Amount)

            };

            return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        public ActionResult CompaniesChronicDeliveryReport(string Date = "", int CompId = 0, int GroupId = 0, string CardId = "")
        {
            //var date = DateTime.Now;
            //ReportDocument rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ChronicDataReport.rpt"));
            //rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            //rd.SetParameterValue("@comp", CompId);

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
            //stream.Seek(0, SeekOrigin.Begin);
            //rd.Close();
            //rd.Dispose();
            //GC.Collect();
            //return File(stream, "application/xls", date.ToString("ddMMyyyy") + "CompanyChronicDelivery.xls");

            DateTime DispenseDate = Convert.ToDateTime(Date);

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CompanyChronicDelivery.rpt"));
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            int ProviderId = Convert.ToInt32(CurrentUser.Provider);
            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");
            rd.SetParameterValue("@ProviderId", ProviderId);
            rd.SetParameterValue("@ProviderName", CurrentUser.UserName);
            rd.SetParameterValue("@CompId", CompId);
            rd.SetParameterValue("@CardId", CardId);
            rd.SetParameterValue("@DispenseDate", DispenseDate);
            rd.SetParameterValue("@GroupId", GroupId);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            rd.Close();
            rd.Dispose();
            GC.Collect();
            return File(stream, "application/pdf", Date + "CompanyChronicDelivery.pdf");
        }
        #endregion

        #region GeneralFunctions

        [HttpPost]
        public JsonResult CkeckCompanyClosedorOpen(string id, string CardId)
        {
            int CompId = Convert.ToInt32(id);
            try
            {
                var model = db.APPROVAL_BAD.Where(x => x.COMP_ID == CompId).FirstOrDefault();
                if (model != null)
                {
                    if (model.FLAG == "Y")
                    {
                        return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
                    }
                    else if (model.FLAG == "N")
                    {
                        return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public class RTCPendingDatasource
        {
            public string MedicienCode { get; set; }
            public string MedicienName { get; set; }
            public int Dose { get; set; }
            public int Duration { get; set; }
            public int TotalDuration { get; set; }
            public int TotalUnits { get; set; }
            public double Amount { get; set; }
            public bool IsDealed { get; set; }
            public string PaymentGroup { get; set; }
        }
        #endregion

        #region AirPort
        public JsonResult CellingAmountAirPort(string id, string ServiceCode)
        {
            double PersonNoPay = 0;
            string Message = "";
            string StaticServiceCode = ServiceCode;
            ServiceCode = ServiceCode == "11604" || ServiceCode == "11601" ? "11603" : ServiceCode;
            int _IntServiceCode = Convert.ToInt32(ServiceCode);
            string _CompId = id.Split('-')[0];
            string MainService = ServiceCode.Substring(0, 3);
            var CurrentDate = DateTime.Now.Date;
            Comp_Employees emp = new Comp_Employees();
            emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= CurrentDate && c.INS_END_DATE >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

            if (ServiceCode == "11602")
            {

                //string compardatestr = "20/" + ((DateTime.Now.Day <= 20) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(+1).ToString("MM/yyyy")).ToString(); 
                string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                //string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                DateTime compardate = DateTime.ParseExact(compardatestr, "dd/MM/yyyy", null);

                if ((emp.INS_END_DATE < compardate) && !(emp.CARD_ID.Split('-')[0].Contains("500")))
                {
                    var nextEmployeecontract = db.Comp_Employees.Where(c => c.CARD_ID == id).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (nextEmployeecontract == null)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.CONTRACT_NO <= emp.CONTRACT_NO)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.TERMINATE_DATE != null)
                    {
                        if (nextEmployeecontract.TERMINATE_DATE.Value.Month <= (DateTime.Now.Month + 1))
                            return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });
                        else
                            emp = nextEmployeecontract;
                    }
                    emp = nextEmployeecontract;
                }
            }

            //provider service permision 
            var provider = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (provider != null)
            {
                var permission = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == _IntServiceCode
                && (x.ProviderName == provider.Provider || x.ProviderName == "ALL")
                && (x.CompId == _CompId || x.CompId == "ALL" || x.CardId == id)).ToList().OrderByDescending(x => x.Id);
                var _permision = permission.Where(x => x.CardId == id || x.ClassCode == emp.CLASS_CODE).FirstOrDefault();
                if (_permision == null)
                {
                    _permision = permission.Where(x => x.CompId == "ALL" || x.CompId == _CompId).FirstOrDefault();
                }
                //ProviderServicesPermission _permision2 = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == _IntServiceCode && 
                //(x.ProviderName == provider.Provider || x.ProviderName == "ALL") && (x.CardId == id || x.CompId == "ALL" || ((x.ClassCode == "" || x.ClassCode == null) ? x.CompId == _CompId : (x.CompId == _CompId && x.ClassCode == emp.CLASS_CODE)))).OrderByDescending(x => x.Id).FirstOrDefault();
                if (_permision != null && _permision.IsActive == false)
                {
                    if (_permision.CardId == "All" || _permision.CompId == "All" || _permision.CardId == id)
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });
                    }
                    else if (_permision.ClassCode == emp.CLASS_CODE)
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });
                    }
                    else if ((_permision.CardId == "" || _permision.CardId == null) && (_permision.ClassCode == null || _permision.ClassCode == "") && (_permision.CompId == _CompId || _permision.CompId == "ALL"))
                    {
                        return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });
                    }
                }
            }
            if (emp != null)
            {
                double CompContractClassMAX_AMOUNT = 0;
                double MaxServiceAmount = 0;
                double CeilingPert;
                double MaxSubServiceAmount;
                bool type = false;
                string isfamily = "";
                string ispool = "";
                //bool hasException = false;
                var remainingconsumption = db.RemainConsumptions.Where(x => x.CARD_ID == id && x.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                if (remainingconsumption != null)
                {
                    if (remainingconsumption.REMAINING >= 0)
                    {
                        CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value;
                        type = true;
                    }
                    else
                    {
                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (accption == null)
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                        if (reasons != null)
                        {
                            CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value;
                            type = true;
                        }
                        else
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                    }
                }
                var CompContractClassEmp = db.CompContractClassEmps.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.CARD_ID == id).FirstOrDefault();
                if (CompContractClassEmp == null)
                {
                    var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(classLimit.MAX_AMOUNT * 0.85);
                    isfamily = classLimit.FOR_FAMILY;
                }
                else
                {
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(CompContractClassEmp.MAX_AMOUNT * 0.85);
                    isfamily = CompContractClassEmp.FOR_FAMILY;
                }
                type = false;

                var DataService1 = new Comp_Customized_D_D();
                var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode && c.CARD_ID == id).FirstOrDefault();
                if (DataService != null)
                {
                    ispool = DataService.POLL_CONSUMPTION;
                    var max_serv = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CARD_ID == id).FirstOrDefault();
                    MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                    //ispool = max_serv.POLL_CONSUMPTION;
                }
                else if (DataService == null)
                {
                    DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (DataService1 != null)
                    {
                        ispool = DataService1.POLL_CONSUMPTION;
                        var max_serv = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService).FirstOrDefault();
                        MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                        //ispool = max_serv.POLL_CONSUMPTION;

                    }

                }
                //Ceiling pert
                if (DataService != null)
                {
                    CeilingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService.CEILING_AMT);

                }
                else if (DataService1 != null)
                {
                    CeilingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService1.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService1.CEILING_AMT);
                }
                else
                {
                    Message = "هذه الخدمه غير مغطاه برجاء الرجوع للاداره الطبيه";
                    CeilingPert = 100;
                    MaxSubServiceAmount = 0;
                    return Json(new { Validation = false, Message = Message, Limit = 0, CeilingPert = 0 });

                }
                double Available = 0;
                double Limit = 0;
                List<Roshita> AcumlatorList = new List<Roshita>();
                var EmpCode = id.Split('-')[2];
                var CompCode = id.Split('-')[0];
                if (isfamily == "Y")
                {
                    AcumlatorList = db.Roshitas.Where(r => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", r.CardId) > 0
                    && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }
                //else if (ispool == "Y")
                //{
                //    AcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                //     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                //}
                else
                {
                    AcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }
                if (ServiceCode == "11602")
                {
                    PersonNoPay = (from roshita in db.Roshitas
                                   join details in db.RoshitaDetails
                                         on roshita.Id equals details.RoshitaID
                                   where roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                   && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= emp.INS_START_DATE && roshita.CreatedDate < emp.INS_END_DATE
                                   select new
                                   {
                                       Amount = details.Amount,
                                   }).ToList().Sum(r => r.Amount);
                }
                List<Roshita> copyacumlator = new List<Roshita>();
                copyacumlator.AddRange(AcumlatorList);
                for (int i = 0; i < copyacumlator.Count(); i++)
                {
                    var item = copyacumlator[i];
                    var chickpermision = (from roshitaacception in db.RoshitaAcceptions
                                          join cardaception in db.CardAcceptionReasons
                                                on roshitaacception.AcceptionId equals cardaception.AcceptionId
                                          where roshitaacception.RoshitaId == item.Id && (cardaception.AcceptionReasonsId == 1 || cardaception.AcceptionReasonsId == 2)
                                          select new
                                          {
                                              id = cardaception.AcceptionReasonsId,
                                          }).ToList();
                    if (chickpermision.Count() > 0)
                    {
                        AcumlatorList.Remove(item);
                    }
                }

                //Main consumption
                double AcumlatorAmount = 0;
                foreach (var item in AcumlatorList)
                {
                    AcumlatorAmount += item.CompanyPayment;
                }
                AcumlatorAmount -= PersonNoPay;
                Available = Convert.ToDouble(CompContractClassMAX_AMOUNT) - AcumlatorAmount;
                double annualLimit = Available;
                //Service consumption
                List<Roshita> AcumlatorServiceList = AcumlatorList.Where(r => r.RoshetaType.Contains(MainService)).ToList();
                double AcumlatorServiceAmount = 0;
                foreach (var item in AcumlatorServiceList)
                {
                    AcumlatorServiceAmount += item.CompanyPayment;
                }
                AcumlatorServiceAmount -= PersonNoPay;
                double ServiceAvailable = (MaxServiceAmount - AcumlatorServiceAmount) < 0 ? 0 : MaxServiceAmount - AcumlatorServiceAmount;
                //SubService consumption
                //List<Roshita> AcumlatorSubServiceList = AcumlatorServiceList.Where(r => r.RoshetaType == ServiceCode).ToList();
                //double AcumlatorSubServiceAmount = 0;
                //foreach (var item in AcumlatorSubServiceList)
                //{
                //    AcumlatorSubServiceAmount += item.CompanyPayment;
                //}
                //AcumlatorSubServiceAmount -= PersonNoPay;
                //double SubServiceAvailable = (MaxSubServiceAmount - AcumlatorSubServiceAmount) < 0 ? 0 : MaxSubServiceAmount - AcumlatorSubServiceAmount;
                //limit
                Limit = (Available >= ServiceAvailable) ? ServiceAvailable : Available;
                //Limit = (Limit >= SubServiceAvailable) ? SubServiceAvailable : Limit;
                //}
                //polling
                if (remainingconsumption != null)
                {
                    Limit = (double)(remainingconsumption.REMAINING.Value < Limit ? remainingconsumption.REMAINING : Limit);
                }
                if (ispool == "Y")
                {
                    Limit = (double)(db.CONSUMPTION_POOL.Where(r => r.COMP_ID == emp.C_COMP_ID).FirstOrDefault().REMAINING);

                }
                bool Validation = Limit > 0 ? true : false;
                Message = Validation ? "Ok" : "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد";
                //Message = Validation ? "Ok" : "Exceeded his annual contract limit";
                //Co-insurance
                double nopaylast21day = 0;
                Co_Insurance_01 CoInsurancelimit2 = new Co_Insurance_01();
                COMP_CUSTOMIZED_D_D_MED_EMP CustemizedMedEmp = new COMP_CUSTOMIZED_D_D_MED_EMP();
                if (isfamily == "Y" || ispool == "Y")
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", c.CARD_ID) > 0 && c.C_COMP_ID == emp.C_COMP_ID
                     && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                     && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }
                else
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                     && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                     && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }
                if (CustemizedMedEmp != null)
                {
                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                    string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);

                    nopaylast21day = (from roshita in db.Roshitas
                                      join details in db.RoshitaDetails
                                            on roshita.Id equals details.RoshitaID
                                      where roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                      && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                      select new
                                      {
                                          Amount = details.Amount,
                                      }).ToList().Sum(r => r.Amount);
                    //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    //List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor" || x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                    List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
                    List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor")).ToList();
                    //List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                    //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
                    //bool LimitDailyPreceptionCount = false;
                    bool LimitMonthlyPreceptionCount = false;
                    //LimitDailyPreceptionCount = (CustemizedMedEmp.DAY_NO_ROSHTA_MON == null || (CustemizedMedEmp.DAY_NO_ROSHTA_MON - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                    LimitMonthlyPreceptionCount = (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                                                                                                                                                                                                  //LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                    LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                    //Double LimitDailyMonthlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                    Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                    //Double LimitDailyYearlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                    Double LimitMonthlyYearlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                    if (StaticServiceCode == "11601" || StaticServiceCode == "11604")
                    {
                        //if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
                        //    Limit = LimitDailyMonthlyPreceptionAmount;
                        //if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
                        //    Limit = LimitDailyYearlyPreceptionAmount;

                        if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount)
                            Limit = LimitMonthlyMonthlyPreceptionAmount;
                        if (LimitMonthlyMonthlyPreceptionAmount < 0)
                            Limit = .001;
                        if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount)
                            Limit = LimitMonthlyYearlyPreceptionAmount;
                        if (LimitMonthlyYearlyPreceptionAmount < 0)
                            Limit = .001;
                    }
                    if (StaticServiceCode == "11602" || StaticServiceCode == "11603")
                    {
                        int nopay = 0;
                        int noover = 0;
                        if (StaticServiceCode == "11602")
                        {
                            var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                            if (medcard != null)
                            {
                                noover = medcard.NO_OVER.Value;
                                nopay = medcard.NO_PAY.Value;
                            }
                        }
                        if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyMonthlyPreceptionAmount;
                        if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyYearlyPreceptionAmount;
                    }
                    CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                    if (CoInsurancelimit2.INSURANCE_DAY < 0)
                    {
                        CoInsurancelimit2.INSURANCE_DAY = .001;
                    }
                    if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                    {
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    }
                    if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    //approval ceiling
                    if (Validation == false)
                    {
                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (accption == null)
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0, AnnualLimit = annualLimit });
                        }
                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                        if (reasons != null)
                        {
                            return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitMonthlyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });
                        }

                    }
                    //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                    return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitMonthlyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });

                }
                else
                {
                    var CustemizedMed = db.COMP_CUSTOMIZED_D_D_MED.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (CustemizedMed == null)
                    {
                        return Json(new { Validation = false, Message = "يرجي مراجعه الادارة الطبيه", Limit = 0, CeilingPert = 0, AnnualLimit = annualLimit });
                    }
                    else
                    {

                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        nopaylast21day = (from roshita in db.Roshitas
                                          join details in db.RoshitaDetails
                                                on roshita.Id equals details.RoshitaID
                                          where roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                          && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                          select new
                                          {
                                              Amount = details.Amount,
                                          }).ToList().Sum(r => r.Amount);
                        //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                        //List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
                        List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
                        //List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                        List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor")).ToList();
                        //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
                        //bool LimitDailyPreceptionCount = false;
                        bool LimitMonthlyPreceptionCount = false;
                        //LimitDailyPreceptionCount = (CustemizedMed.DAY_NO_ROSHTA_MON == null || (CustemizedMed.DAY_NO_ROSHTA_MON - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                        LimitMonthlyPreceptionCount = (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                                                                                                                                                                                                //LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMed.DAY_NO_ROSHTA_YEAR == null || (CustemizedMed.DAY_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                        LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                        //Double LimitDailyMonthlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                        Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMed.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                        //Double LimitDailyYearlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                        Double LimitMonthlyYearlyPreceptionAmount = CustemizedMed.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                        if (StaticServiceCode == "11601" || StaticServiceCode == "11604")
                        {
                            if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount)
                                Limit = LimitMonthlyMonthlyPreceptionAmount;
                            if (LimitMonthlyMonthlyPreceptionAmount < 0)
                                Limit = .001;
                            if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount)
                                Limit = LimitMonthlyYearlyPreceptionAmount;
                            if (LimitMonthlyYearlyPreceptionAmount < 0)
                                Limit = .001;
                        }
                        if (StaticServiceCode == "11602" || StaticServiceCode == "11603")
                        {
                            int nopay = 0;
                            int noover = 0;
                            if (StaticServiceCode == "11602")
                            {
                                var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                                if (medcard != null)
                                {
                                    noover = medcard.NO_OVER.Value;
                                    nopay = medcard.NO_PAY.Value;
                                }
                            }
                            if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyMonthlyPreceptionAmount;
                            if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyYearlyPreceptionAmount;
                        }
                        CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.DAY_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                        if (CoInsurancelimit2.INSURANCE_DAY < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = .001;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        }

                        if (CoInsurancelimit2.INSURANCE_DAY == null)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = 0;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH == null)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = 0;
                        }
                        if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        //approval ceiling
                        if (Validation == false)
                        {
                            var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                            if (accption == null)
                            {
                                return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0, AnnualLimit = annualLimit });
                            }
                            var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                            if (reasons != null)
                            {
                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitMonthlyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });
                            }

                        }
                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitMonthlyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, IsFamily = isfamily, IsPool = ispool, AnnualLimit = annualLimit });

                    }
                }

            }
            else
            {
                return Json(new { Validation = false, Message = "Employee contract issue ,you can call operation department", Limit = 0, CeilingPert = 0 });

            }

        }

        public JsonResult CellingAmountEditAirPort(string id, string ServiceCode, Int64 RoshitaId)
        {
            double PersonNoPay = 0;
            string Message = "";
            string StaticServiceCode = ServiceCode;
            ServiceCode = ServiceCode == "11604" || ServiceCode == "11601" ? "11603" : ServiceCode;
            string MainService = ServiceCode.Substring(0, 3);
            Comp_Employees emp = new Comp_Employees();
            emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

            if (ServiceCode == "11602")
            {

                //string compardatestr = "20/" + ((DateTime.Now.Day <= 20) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(+1).ToString("MM/yyyy")).ToString();
                string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
                DateTime compardate = DateTime.ParseExact(compardatestr, "dd/MM/yyyy", null);

                if ((emp.INS_END_DATE < compardate) && !(emp.CARD_ID.Split('-')[0].Contains("500")))
                {
                    var nextEmployeecontract = db.Comp_Employees.Where(c => c.CARD_ID == id).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (nextEmployeecontract == null)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    if (nextEmployeecontract.CONTRACT_NO <= emp.CONTRACT_NO)
                    {
                        return Json(new { Validation = false, Message = "تم انتهاء مدةالتعاقد لهذه الشركة ", Limit = 0, CeilingPert = 0 });

                    }
                    emp = nextEmployeecontract;
                }
            }
            //
            if (emp != null)
            {
                double CompContractClassMAX_AMOUNT = 0;
                double MaxServiceAmount = 0;
                double CeilingPert;
                double MaxSubServiceAmount;
                bool type = false;
                double RoshitaNoPayEdit = 0;
                string isfamily = "";
                string ispool = "";
                if (ServiceCode == "11602")
                {
                    RoshitaNoPayEdit = (from roshita in db.Roshitas
                                        join details in db.RoshitaDetails
                                              on roshita.Id equals details.RoshitaID
                                        where roshita.Id == RoshitaId && roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                        && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= emp.INS_START_DATE && roshita.CreatedDate < emp.INS_END_DATE
                                        select new
                                        {
                                            Amount = details.Amount,
                                        }).ToList().Sum(r => r.Amount);


                    PersonNoPay = (from roshita in db.Roshitas
                                   join details in db.RoshitaDetails
                                         on roshita.Id equals details.RoshitaID
                                   where roshita.Id != RoshitaId && roshita.CardId == id && roshita.Manager == "Pharmacy_Chronic"
                                   && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= emp.INS_START_DATE && roshita.CreatedDate < emp.INS_END_DATE
                                   select new
                                   {
                                       Amount = details.Amount,
                                   }).ToList().Sum(r => r.Amount);
                }
                var remainingconsumption = db.RemainConsumptions.Where(x => x.CARD_ID == id && x.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                var remainingPool = db.CONSUMPTION_POOL.Where(r => r.COMP_ID == emp.C_COMP_ID).FirstOrDefault();
                if (remainingconsumption != null)
                {
                    if (remainingconsumption.REMAINING >= 0)
                    {
                        var rosita = db.Roshitas.Where(r => r.Id == RoshitaId).FirstOrDefault();
                        CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit;
                        type = true;
                        remainingconsumption.REMAINING = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit;
                    }
                    else
                    {
                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (accption == null)
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                        if (reasons != null)
                        {
                            var rosita = db.Roshitas.Where(r => r.Id == RoshitaId).FirstOrDefault();
                            CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit; ;
                            type = true;
                            remainingconsumption.REMAINING = remainingconsumption.REMAINING.Value + rosita.CompanyPayment - RoshitaNoPayEdit; ;

                        }
                        else
                        {
                            return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                        }
                    }
                }
                var CompContractClassEmp = db.CompContractClassEmps.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.CARD_ID == id).FirstOrDefault();
                if (CompContractClassEmp == null)
                {
                    var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(classLimit.MAX_AMOUNT * 0.85);
                    isfamily = classLimit.FOR_FAMILY;
                }
                else
                {
                    CompContractClassMAX_AMOUNT = Convert.ToDouble(CompContractClassEmp.MAX_AMOUNT * 0.85);
                    isfamily = CompContractClassEmp.FOR_FAMILY;
                }
                type = false;

                var DataService1 = new Comp_Customized_D_D();
                var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode && c.CARD_ID == id).FirstOrDefault();
                if (DataService != null)
                {
                    ispool = DataService.POLL_CONSUMPTION;
                    var max_serv = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CARD_ID == id).FirstOrDefault();
                    MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                    //ispool = max_serv.POLL_CONSUMPTION;
                }
                else if (DataService == null)
                {
                    DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (DataService1 != null)
                    {
                        ispool = DataService1.POLL_CONSUMPTION;
                        var max_serv = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService).FirstOrDefault();
                        MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);
                        //ispool = max_serv.POLL_CONSUMPTION;

                    }

                }
                //Ceiling pert
                if (DataService != null)
                {
                    CeilingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService.CEILING_AMT);

                }
                else if (DataService1 != null)
                {
                    CeilingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
                    MaxSubServiceAmount = (DataService1.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService1.CEILING_AMT);
                }
                else
                {
                    Message = "هذه الخدمه غير مغطاه برجاء الرجوع للاداره الطبيه";
                    CeilingPert = 100;
                    MaxSubServiceAmount = 0;
                    return Json(new { Validation = false, Message = Message, Limit = 0, CeilingPert = 0 });

                }

                double Available = 0;
                double Limit = 0;
                List<Roshita> AcumlatorList = new List<Roshita>();
                var EmpCode = id.Split('-')[2];
                var CompCode = id.Split('-')[0];
                if (isfamily == "Y")
                {
                    AcumlatorList = db.Roshitas.Where(r => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", r.CardId) > 0 && r.Id != RoshitaId
                    && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }
                else
                {
                    AcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
                    && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
                }

                List<Roshita> copyacumlator = new List<Roshita>();
                copyacumlator.AddRange(AcumlatorList);
                for (int i = 0; i < copyacumlator.Count(); i++)
                {
                    var item = copyacumlator[i];
                    var chickpermision = (from roshitaacception in db.RoshitaAcceptions
                                          join cardaception in db.CardAcceptionReasons
                                                on roshitaacception.AcceptionId equals cardaception.AcceptionId
                                          where roshitaacception.RoshitaId == item.Id && (cardaception.AcceptionReasonsId == 1 || cardaception.AcceptionReasonsId == 2)
                                          select new
                                          {
                                              id = cardaception.AcceptionReasonsId,
                                          }).ToList();
                    if (chickpermision.Count() > 0)
                    {
                        AcumlatorList.Remove(item);
                    }
                }

                //Main consumption
                double AcumlatorAmount = 0;
                foreach (var item in AcumlatorList)
                {
                    AcumlatorAmount += item.CompanyPayment;
                }
                AcumlatorAmount -= PersonNoPay;
                Available = Convert.ToDouble(CompContractClassMAX_AMOUNT) - AcumlatorAmount;

                //Service Concamution
                List<Roshita> AcumlatorServiceList = AcumlatorList.Where(r => r.RoshetaType.Contains(MainService)).ToList();
                double AcumlatorServiceAmount = 0;
                foreach (var item in AcumlatorServiceList)
                {
                    AcumlatorServiceAmount += item.CompanyPayment;
                }
                AcumlatorServiceAmount -= PersonNoPay;
                double ServiceAvailable = (MaxServiceAmount - AcumlatorServiceAmount) < 0 ? 0 : MaxServiceAmount - AcumlatorServiceAmount;
                //limit
                Limit = (Available >= ServiceAvailable) ? ServiceAvailable : Available;

                //polling
                if (remainingconsumption != null)
                {
                    Limit = (double)(remainingconsumption.REMAINING.Value < Limit ? remainingconsumption.REMAINING : Limit);
                }

                if (ispool == "Y")
                {
                    Limit = remainingPool.REMAINING.Value;

                }

                bool Validation = Limit > 0 ? true : false;
                Message = Validation ? "Ok" : "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد";

                //approval ceiling
                if (Validation == false)
                {
                    var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                    if (accption == null)
                    {
                        return Json(new { Validation = false, Message = "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد", Limit = 0, CeilingPert = 0 });
                    }
                    var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                    if (reasons != null)
                    {
                        return Json(new { Validation = true, Message = "Has Approval", Limit = 0.001, CeilingPert = CeilingPert });
                    }

                }
                //Co-insurance
                double nopaylast21day = 0;
                Co_Insurance_01 CoInsurancelimit2 = new Co_Insurance_01();
                //bool LimitDailyPreceptionCount = false;
                bool LimitMonthlyPreceptionCount = false;
                COMP_CUSTOMIZED_D_D_MED_EMP CustemizedMedEmp = new COMP_CUSTOMIZED_D_D_MED_EMP();
                if (isfamily == "Y" || ispool == "Y")
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => SqlFunctions.PatIndex(CompCode + "-%-" + EmpCode + "-%", c.CARD_ID) > 0 && c.C_COMP_ID == emp.C_COMP_ID
                     && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                     && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }
                else
                {
                    CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                  && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                }

                if (CustemizedMedEmp != null)
                {
                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                    string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                    nopaylast21day = (from roshita in db.Roshitas
                                      join details in db.RoshitaDetails
                                            on roshita.Id equals details.RoshitaID
                                      where roshita.CardId == id && roshita.Id != RoshitaId && roshita.Manager == "Pharmacy_Chronic"
                                      && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                      select new
                                      {
                                          Amount = details.Amount,
                                      }).ToList().Sum(r => r.Amount);
                    //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    //List<Roshita> YearlyDailyAcumlatorList = MainAcumlatorList.Where(x => (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor") && x.CompanyPayment > 0).ToList();
                    List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
                    //List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                    List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.Id != RoshitaId && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor")).ToList();
                    //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();

                    //LimitDailyPreceptionCount = (CustemizedMedEmp.DAY_NO_ROSHTA_MON == null || (CustemizedMedEmp.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                    LimitMonthlyPreceptionCount = (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                                                                                                                                                                                                  //LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                    LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                    //Double LimitDailyMonthlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                    Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                    //Double LimitDailyYearlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                    Double LimitMonthlyYearlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                    if (StaticServiceCode == "11601" || StaticServiceCode == "11604")
                    {
                        if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount)
                            Limit = LimitMonthlyMonthlyPreceptionAmount;
                        if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount)
                            Limit = LimitMonthlyYearlyPreceptionAmount;
                    }
                    if (StaticServiceCode == "11602" || StaticServiceCode == "11603")
                    {
                        int nopay = 0;
                        int noover = 0;
                        if (StaticServiceCode == "11602")
                        {
                            var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                            if (medcard != null)
                            {
                                noover = medcard.NO_OVER.Value;
                                nopay = medcard.NO_PAY.Value;
                            }
                        }
                        if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyMonthlyPreceptionAmount;
                        if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                            Limit = LimitMonthlyYearlyPreceptionAmount;
                    }

                    CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                    if (CoInsurancelimit2.INSURANCE_DAY < 0)
                    {
                        CoInsurancelimit2.INSURANCE_DAY = .001;
                    }
                    if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                    {
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                    }
                    if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                        CoInsurancelimit2.INSURANCE_MONTH = .001;
                }
                else
                {
                    var CustemizedMed = db.COMP_CUSTOMIZED_D_D_MED.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService && c.SER_SERV == ServiceCode).FirstOrDefault();
                    if (CustemizedMed == null)
                    {
                        return Json(new { Validation = false, Message = "يرجي مراجعه الادارة الطبيه", Limit = 0, CeilingPert = 0 });
                    }
                    else
                    {

                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();

                        nopaylast21day = (from roshita in db.Roshitas
                                          join details in db.RoshitaDetails
                                                on roshita.Id equals details.RoshitaID
                                          where roshita.CardId == id && roshita.Id != RoshitaId && roshita.Manager == "Pharmacy_Chronic"
                                          && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
                                          select new
                                          {
                                              Amount = details.Amount,
                                          }).ToList().Sum(r => r.Amount);
                        //List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => (x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor") && x.CompanyPayment > 0).ToList();
                        List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
                        //List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                        List<Roshita> MonthlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.CardId == id && x.Id != RoshitaId && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic" || x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor")).ToList();
                        //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();

                        //LimitDailyPreceptionCount = (CustemizedMed.DAY_NO_ROSHTA_MON == null || (CustemizedMed.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                        LimitMonthlyPreceptionCount = (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                                                                                                                                                                                                //LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMed.DAY_NO_ROSHTA_YEAR == null || (CustemizedMed.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                        LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                        //Double LimitDailyMonthlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
                        Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMed.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

                        //Double LimitDailyYearlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
                        Double LimitMonthlyYearlyPreceptionAmount = CustemizedMed.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
                        if (StaticServiceCode == "11601" || StaticServiceCode == "11604")
                        {
                            if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount)
                                Limit = LimitMonthlyMonthlyPreceptionAmount;
                            if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount)
                                Limit = LimitMonthlyYearlyPreceptionAmount;
                        }
                        if (StaticServiceCode == "11602" || StaticServiceCode == "11603")
                        {
                            int nopay = 0;
                            int noover = 0;
                            if (StaticServiceCode == "11602")
                            {
                                var medcard = db.Med_Card.Where(c => c.CARD_NO == id).FirstOrDefault();
                                if (medcard != null)
                                {
                                    noover = medcard.NO_OVER.Value;
                                    nopay = medcard.NO_PAY.Value;
                                }
                            }
                            if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyMonthlyPreceptionAmount;
                            if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount && nopay != 1 && noover != 1)
                                Limit = LimitMonthlyYearlyPreceptionAmount;
                        }
                        CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_DAY = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.DAY_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

                        if (CoInsurancelimit2.INSURANCE_DAY < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = .001;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH < 0)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = .001;
                        }

                        if (CoInsurancelimit2.INSURANCE_DAY == null)
                        {
                            CoInsurancelimit2.INSURANCE_DAY = 0;
                        }
                        if (CoInsurancelimit2.INSURANCE_MONTH == null)
                        {
                            CoInsurancelimit2.INSURANCE_MONTH = 0;
                        }
                        if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
                            CoInsurancelimit2.INSURANCE_MONTH = .001;

                    }
                }

                return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitMonthlyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, IsFamily = isfamily, IsPool = ispool });
            }
            return Json(new { Validation = false, Message = "Employee contract issue ,you can call operation department", Limit = 0, CeilingPert = 0 });
        }


        #endregion
    }
}