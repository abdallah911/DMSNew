using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_TEST.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static DMS_TEST.Controllers.PharmacyController;

namespace DMS_Authontication1.Controllers
{

    public class LabsController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public LabsController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();

        }
        #region Labs
        [Authorize(Roles = "Admin,Lab,Lab_Admin")]
        public ActionResult Lab()
        {
            NotificationHub objNotifHub = new NotificationHub();
            // Notification objNotif = new Notification();
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

        public JsonResult CheckDaily(string id, string code)
        {
            var createdDate = db.Roshitas.Join(db.RoshitaDetails, x => x.Id, d => d.RoshitaID, (x, d) => new { x, d })
              .Where(z => z.x.CardId == id && z.d.MedicienCode == code && z.x.Manager == "Lab" && z.d.PaymentGroup != "Cash" && z.d.IsDealed == true)
              .OrderByDescending(v => v.x.CreatedDate)
              .Select(l => new
              {
                  id = l.x.Id,
                  Createdate = l.x.CreatedDate,
                  TotalDuration = l.d.TotalDuration
              }).FirstOrDefault();
            //string Current = DateTime.Now.ToString("dd");
            int check;
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
                }

            }

            return new JsonResult { Data = check, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize(Roles = "Admin,Lab,Lab_Admin")]
        public JsonResult GetList(int sEcho = 1, int iDisplayStart = 0, int iDisplayLength = 24, string sSearch = "")
        {
            ApplicationDbContext myEntities = new ApplicationDbContext();
            var labs = new List<Serv_Lab>();
            var user = myEntities.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            long userProvider = Convert.ToInt64(user.Provider);
            long lgSearch;
            long.TryParse(sSearch, out lgSearch);
            // bool UserRole = User.IsInRole("Lab");
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Serv_Lab.Where(x => x.LAB_CODE == userProvider && x.LOOK == 0).Where(r => sSearch != "" ? r.SERV_ANAME.Contains(sSearch) || r.SERV_CODE == lgSearch : true)
                //.Where(x => UserRole ? x.LAB_CODE == userProvider : x.LAB_CODE == 11206009)
                .OrderBy(m => m.SERV_CODE)
          .Select(l => new
          {
              SERV_CODE = l.SERV_CODE,
              SERV_ANAME = l.SERV_ANAME,
          }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.Serv_Lab.Where(x => x.LAB_CODE == userProvider && x.LOOK == 0).Where(r => sSearch != "" ? r.SERV_ANAME.Contains(sSearch) || r.Id == lgSearch : true)
                .Count(),
                iTotalDisplayRecords = db.Serv_Lab.Where(x => x.LAB_CODE == userProvider && x.LOOK == 0).Where(r => sSearch != "" ? r.SERV_ANAME.Contains(sSearch) || r.Id == lgSearch : true)
                .Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetLabByCode(long code)
        {
            var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            long userProvider = Convert.ToInt64(user.Provider);
            var Data = db.Serv_Lab.Where(x => x.LAB_CODE == userProvider && x.LOOK == 0).
            Select(l => new
            {
                SERV_CODE = l.SERV_CODE,
                SERV_ANAME = l.SERV_ANAME,
                GRUOP_TYPE = l.GRUOP_TYPE,
                SERV_AMOUNT = l.SERV_AMOUNT,

            }).FirstOrDefault(x => x.SERV_CODE == code);

            return new JsonResult { Data = Data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetLimit(string id)
        {
            try
            {
                var emp = db.Comp_Employees.Where(x => x.CARD_ID == id).FirstOrDefault();
                var limit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                return Json(new { ok = true, limit = limit, message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Lab,Lab_Admin")]

        public JsonResult Save(Roshita data)
        {
            if (data.CreatedBy == null)
            {
                data.CreatedBy = User.Identity.Name;
            }
            data.CreatedDate = DateTime.Now;
            db.Roshitas.Add(data);
            data.Manager = "Lab";
            data.RoshetaType = "11206";
            //Oracle_Id
            //string NeworacleId = "";
            //Roshita roshita = db.Roshitas.Where(x => x.Manager != "Doctor_Chronic" && x.Oracle_Id.Value.ToString().StartsWith("2") && (x.SyncBy == null || x.SyncBy == "Sql")).OrderByDescending(x => x.Id).FirstOrDefault();
            //if (DateTime.Now.Day == 1 && Convert.ToInt64(roshita.Oracle_Id.ToString().Substring(9)) != 1)
            //{
            //    NeworacleId = "2" + DateTime.Now.ToString("ddMMyyyy") + "1";
            //}
            //else
            //{
            //    long OracleId = roshita.Oracle_Id == null ? 0 : Convert.ToInt64(roshita.Oracle_Id.ToString().Substring(9))+1;
            //    NeworacleId = "2" + DateTime.Now.ToString("ddMMyyyy") + OracleId;

            //}
            //data.Oracle_Id = Convert.ToInt64(NeworacleId);
            int result = db.SaveChanges();
            Session["id"] = data.Id;
            //data.CreatedDate.ToString();
            return Json("2" + data.CreatedDate.Value.ToString("ddMMyy") + data.Id);
        }
        [Authorize(Roles = "Admin,Lab,Lab_Admin")]

        public JsonResult SaveMediciens(List<RoshitaDetail> Medciens)
        {
            bool oneNotification = false;

            foreach (RoshitaDetail Medicien in Medciens)
            {
                Medicien.RoshitaID = Convert.ToInt64(Session["id"]);
                Medicien.Dose = 0;
                Medicien.Duration = 0;
                Medicien.TotalDuration = 7;
                Medicien.TotalUnits = 1;
                if (Medicien.PaymentGroup == "Pending")
                {
                    if (oneNotification == false)
                    {
                        string CardId = db.Roshitas.Where(x => x.Id == Medicien.RoshitaID).FirstOrDefault().CardId;
                        NotificationHub objNotifHub = new NotificationHub();
                        Notification notification = new Notification();
                        notification.SentTo = "Admin";
                        notification.CreatedBy = User.Identity.Name;
                        notification.CreatedDate = DateTime.Now;
                        notification.Type = 1;//pending
                        notification.Details = CardId;
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
            try
            {
                int result = db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return Json("savd");
        }
        [Authorize(Roles = "Admin,Lab,Lab_Admin")]
        public JsonResult SaveDiagnoises(List<Diagnose> Diagnoises)
        {
            foreach (Diagnose item in Diagnoises)
            {
                PrescriptionRoshitaDignosi dignosi = new PrescriptionRoshitaDignosi();
                dignosi.RositaId = Convert.ToInt64(Session["id"]);
                dignosi.DiagnoiseName = item.DIAG_ANAME;
                db.PrescriptionRoshitaDignosis.Add(dignosi);
            }
            db.SaveChanges();
            return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
        //--------------------------------------------------------------------
        #region pending

        public ActionResult Pending(string Id)
        {

            return View();
        }
        public JsonResult ApprovedMedicine(string id, long TxtSearch)
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
                     IsDealed = l.d.IsDealed

                 }).ToList();

            return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ChangeStatus(int ApprovalId, string status)
        {

            var emp = db.RoshitaDetails.Where(x => x.Id == ApprovalId).FirstOrDefault();
            bool Flag;
            if (status == "true")
            {
                Flag = true;
                var Roshta = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault();
                if (emp.PaymentGroup == "Accepted")
                {
                    Roshta.TotalValue += emp.Amount;
                    double TotalValue = Roshta.TotalValue.Value;// - Roshta.Cash.Value;
                    double Limit = Roshta.Limit;

                    double CompanyPercent = Convert.ToDouble(Roshta.CompanyPercent) / 100;
                    double PersonPercent = Math.Round(1 - CompanyPercent, 2);
                    double CompanyPayment = 0;
                    //var person = parseFloat(100 - co);//percentage
                    //                                  //total-cash
                    //var total = parseFloat($('#txtTotalInvoice').val()) - sumCash;
                    //var cash = parseFloat($('#txtCash').val());
                    //var ValueCredit = 0;
                    //if (Limit > AnuualLimit || Limit == 0)
                    //{
                    //    Limit = AnuualLimit;
                    //}
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


                    // Roshta.CompanyPayment += emp.Amount * (Convert.ToDouble(Roshta.CompanyPercent) / 100);
                    // Roshta.PersonPayment += emp.Amount * (Convert.ToDouble((100 - Roshta.CompanyPercent)) / 100);
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
            int result = db.SaveChanges();
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //public JsonResult Approvals(string id)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var approvals = db.Roshitas.Where(x => x.CardId == id && x.CreatedBy == User.Identity.Name && x.Manager == "Lab")
        //     //.Join(db.RoshitaDetails, r => r.Id, d => d.RoshitaID, (r, d) => new { r, d })
        //     //.Where(b => b.r.CreatedBy == User.Identity.Name)
        //     //.Where(c => c.d.PaymentGroup == "Accepted" || c.d.PaymentGroup == "Rejected" || c.d.PaymentGroup == "Pending")
        //     //.Select(l => new Roshita_RoshitaDetails
        //     //{
        //     //    Id = l.r.Id,
        //     //    DId = l.d.Id,
        //     //    MedicienName = l.d.MedicienName,
        //     //    Amount = l.d.Amount,
        //     //    CreatedDate = l.r.CreatedDate,
        //     //    PaymentGroup = l.d.PaymentGroup,
        //     //    IsDealed = l.d.IsDealed
        //     //})
        //     .ToList();
        //    var serializer = new JavaScriptSerializer();

        //    serializer.MaxJsonLength = Int32.MaxValue;

        //    var result = new ContentResult
        //    {
        //        Content = serializer.Serialize(approvals),
        //        ContentType = "application/json"
        //    };
        //    return new JsonResult { Data = approvals, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        #endregion
        //--------------------------------------------------------------------
        #region Control Panel Lab
        //Main Roshta
        [Authorize(Roles = "Admin,Lab,Lab_Admin")]
        public ActionResult Index()
        {
            if (User.IsInRole("Lab"))
            {
                ViewBag.ddlUsers = new SelectList(UserDB.Users.ToList(), "UserName", "UserName", User.Identity.Name);
            }
            else if (User.IsInRole("Lab_Admin"))
            {
                var CurrentUser = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                ViewBag.ddlUsers = new SelectList(UserDB.Users.Where(x => x.Provider == CurrentUser.Provider).ToList(), "UserName", "UserName", User.Identity.Name);
            }
            return View();
        }
        public JsonResult PreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "", string Company = "", string Provider = "", string From = "", string To = "", string CardId = "", string Branch = "", string ApprovalNo = "")
        {
            string Type = "Lab";
            if (!User.IsInRole("Admin") && From != "" && To != "")
            {
                if (To != "")
                {
                    DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                    To = T.ToString();
                }
                if (User.IsInRole("Lab"))
                {
                    Branch = User.Identity.Name;
                }
                else if (User.IsInRole("Lab_Admin") && Branch == "")
                {
                    var context = new ApplicationDbContext();
                    Provider = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Provider;
                }
                var LabResult = new
                {
                    sEcho = sEcho,
                    aaData = db.fn_AdminLabClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).OrderByDescending(m => m.Id)
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

                    iTotalRecords = db.fn_AdminLabClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count(),
                    iTotalDisplayRecords = db.fn_AdminLabClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count()
                };
                return new JsonResult { Data = LabResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


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
                    aaData = db.fn_AdminLabClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).OrderByDescending(m => m.Id)
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

                    iTotalRecords = db.fn_AdminLabClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count(),
                    iTotalDisplayRecords = db.fn_AdminLabClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type).Count()
                };
                return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Stop")).AsEnumerable()
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

                iTotalRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Count(),
                iTotalDisplayRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };





        }
        public JsonResult PreseptionAdminCount(string Company = "", string Provider = "", string From = "", string To = "", string CardId = "", string Branch = "", string ApprovalNo = "")
        {


            if (To != "")
            {
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                To = T.ToString();
            }
            var Adminresult = db.fn_AdminLabClamsCounts(From, To, Company, Provider, Branch, ApprovalNo, CardId, "Lab").FirstOrDefault();

            return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        //public JsonResult PreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        //{
        //    long lgSearch;
        //    long.TryParse(sSearch, out lgSearch);
        //    var result = new
        //    {
        //        sEcho = sEcho,
        //        aaData = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Lab") && !x.Manager.Contains("Daily") && !x.Manager.Contains("Monthly") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")).OrderByDescending(m => m.Id).AsEnumerable()
        //        .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || r.Id == lgSearch : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date)
        //        .Select(l => new Roshita
        //        {
        //            Id = l.Id,
        //            CardId = l.CardId,
        //            CompanyPercent = l.CompanyPercent,
        //            TotalValue = l.TotalValue,
        //            Manager = l.Manager,
        //            CreatedDate = l.CreatedDate,
        //        }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

        //        iTotalRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Lab") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
        //        .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || r.Id == lgSearch : true).Count(),
        //        iTotalDisplayRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Lab") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
        //        .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || r.Id == lgSearch : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date).Count()
        //    };
        //    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        //}

        [Authorize(Roles = "Admin,Lab,Lab_Admin")]
        public ActionResult Details(long id)
        {
            List<DoctorContainerViewModel> data = db.RoshitaDetails.Where(l => l.RoshitaID == id && l.IsDealed == true).AsEnumerable()
                 //  .Join(db.Serv_Lab, d => d.MedicienCode, m => Convert.ToString(m.Id), (d, m) => new { d, m })

                 .Select(l => new DoctorContainerViewModel
                 {
                     Id = l.Id,
                     MedicienCode = l.MedicienCode,
                     MedicienName = l.MedicienName,
                     Amount = l.Amount,
                     PaymentGroup = l.PaymentGroup,

                 })
                   .GroupBy(x => new { x.MedicienCode })
                .Select(x => x.FirstOrDefault())
                 .ToList();
            return View(data);
        }

        [HttpPost]
        public JsonResult LabDelete(long id)
        {
            try
            {
                Roshita roshta = db.Roshitas.Where(c => c.Id == id).FirstOrDefault();
                if (roshta.Manager == "Lab")
                {
                    roshta.Manager = "Lab_Stop";
                }
                db.Entry(roshta).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Lab,Lab_Admin")]

        public ActionResult Edit(long id)
        {
            List<DoctorContainerViewModel> data = db.RoshitaDetails.Where(l => l.RoshitaID == id && l.IsDealed == true)//.AsEnumerable()
                                                                                                                       //.Join(db.Serv_Lab, d => d.MedicienCode, m =>Convert.ToString(m.Id), (d, m) => new { d, m })
                                                                                                                       //.Where(l => l.d.RoshitaID == id && l.d.IsDealed == true)
                 .Select(l => new DoctorContainerViewModel
                 {
                     Id = l.Id,
                     MedicienCode = l.MedicienCode,
                     MedicienName = l.MedicienName,
                     Amount = l.Amount,
                     PaymentGroup = l.PaymentGroup
                 })
                   .GroupBy(x => new { x.MedicienCode })
                .Select(x => x.FirstOrDefault())
                 .ToList();
            return View(data);
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

        public JsonResult Update([Bind(Include = "Id,TotalValue,PersonPayment,CompanyPayment,OverInsurance,Cash")] Roshita data)
        {

            var roshita = db.Roshitas.Where(x => x.Id == data.Id).FirstOrDefault();
            roshita.OverInsurance = data.OverInsurance;
            roshita.PersonPayment = data.PersonPayment;
            roshita.CompanyPayment = data.CompanyPayment;
            roshita.TotalValue = data.TotalValue;
            roshita.Cash = data.Cash;
            roshita.SyncBy = "Update";
            if (ModelState.IsValid)
            {
                db.Entry(roshita).State = EntityState.Modified;
                db.SaveChanges();
            }
            Session["id"] = roshita.Id;
            return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);
        }

        public JsonResult UpdateMediciens(List<RoshitaDetail> Medciens)
        {
            long id = Convert.ToInt64(Session["id"]);

            List<RoshitaDetail> List_R_Details = db.RoshitaDetails.Where(x => x.RoshitaID == id).ToList();
            var mediciens = List_R_Details.Where(x => x.RoshitaID == id && x.IsDealed == true).ToList();
            db.RoshitaDetails.RemoveRange(mediciens);
            //db.SaveChanges();

            bool oneNotification = (List_R_Details.Where(x => x.RoshitaID == id && x.PaymentGroup == "Pending").ToList().Count == 0) ? false : true; ;
            foreach (RoshitaDetail Medicien in Medciens)
            {
                Medicien.RoshitaID = Convert.ToInt64(Session["id"]);
                //Medicien.IsDealed = true;
                Medicien.Dose = 0;
                Medicien.Duration = 0;
                Medicien.TotalDuration = 7;
                Medicien.TotalUnits = 1;

                if (Medicien.PaymentGroup == "Pending" || Medicien.PaymentGroup == "Cash")
                {
                    RoshitaDetail roshitaDetail = List_R_Details.Where(x => x.RoshitaID == id && x.MedicienCode == Medicien.MedicienCode && x.IsDealed == false).FirstOrDefault();
                    if (roshitaDetail != null)
                    {
                        if (Medicien.PaymentGroup == "Pending")
                        {
                            Medicien.PaymentGroup = roshitaDetail.PaymentGroup;
                        }
                        //added before and insert pending or cash
                        oneNotification = true;
                        db.RoshitaDetails.Remove(roshitaDetail);
                        //db.SaveChanges();
                    }

                    if (oneNotification == false && Medicien.PaymentGroup == "Pending")
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
                            notification.Details = CardId;
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
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return Json("savd");
        }
        public ActionResult ControlPenelReport(string id)
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
            if (patient.EMP_ENAME == null)
            {
                rd.SetParameterValue("PatientName", "Unnamed");
            }
            else
            {
                rd.SetParameterValue("PatientName", patient.EMP_ENAME);
            }
            data.RoshetaType = "Lab";

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
        public ActionResult PrintClams(string From, string To, string Branch = "")
        {
            try
            {
                if (User.IsInRole("Lab"))
                {
                    Branch = User.Identity.Name;
                }
                else if (User.IsInRole("Lab_Admin") && Branch == "")
                {
                    var context = new ApplicationDbContext();
                    Branch = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Provider;
                }
                DateTime F = Convert.ToDateTime(From);
                //DateTime T = Convert.ToDateTime(To);
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);//to get all pervious day
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ClamsReportLabs.rpt"));

                var y = db.Roshitas.Where(r => r.CreatedDate >= F && r.CreatedDate <= T && !r.Manager.Contains("Stop") && r.CreatedBy == User.Identity.Name)
                   .Join(db.Comp_Employees, r => r.CardId, m => m.CARD_ID, (r, m) => new { r, m })
                   .Where(x => x.m.INS_START_DATE <= DateTime.Now && x.m.INS_END_DATE >= DateTime.Now)
                   .AsEnumerable()
                .Select(d => new RoshitaCompEmolyessReportViewModel
                {
                    //Id = d.r.Id,
                    Id = Convert.ToInt64("2" + d.r.CreatedDate.Value.ToString("ddMMyy") + d.r.Id),
                    CardId = d.r.CardId,
                    EMP_ENAME = d.m.EMP_ENAME,
                    Manager = d.r.Manager,
                    TotalValue = d.r.TotalValue.Value,
                    CompanyPayment = d.r.CompanyPayment,
                    OverInsurance = d.r.OverInsurance.Value,
                    Cash = d.r.Cash.Value,
                    PersonPayment = d.r.PersonPayment,
                }).Distinct().ToList();
                rd.SetDataSource(y);
                ApplicationDbContext users = new ApplicationDbContext();
                var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

                //string Branch = CurrentUser != null ? CurrentUser.Provider : "";
                rd.SetParameterValue("From", From);
                rd.SetParameterValue("To", To);
                rd.SetParameterValue("Provider", User.Identity.Name);
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
                throw ex;
            }
        }

        #endregion

        #region Celling

        public JsonResult CellingAmount(string id, string ServiceCode)
        {
            string Message = "";
            //ServiceCode = ServiceCode == "11604" ? "11601" : ServiceCode;
            int _IntServiceCode = Convert.ToInt32(ServiceCode);
            string _CompId = id.Split('-')[0];
            string MainService = ServiceCode.Substring(0, 3);
            var CurrentDate = DateTime.Now.Date;

            var emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= CurrentDate && c.INS_END_DATE >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            //var emp = db.fn_searchCompEmployees(id).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            //provider service permision

            var provider = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (provider != null)
            {
                ProviderServicesPermission _permision = db.ProviderServicesPermissions.Where(x => x.IsDeleted == false && x.ServiceCode == _IntServiceCode && (x.ProviderName == provider.Provider || x.ProviderName == "ALL") && (x.CardId == id || x.CompId == "ALL" || ((x.ClassCode == "" || x.ClassCode == null) ? x.CompId == _CompId : (x.CompId == _CompId && x.ClassCode == emp.CLASS_CODE)))).OrderByDescending(x => x.Id).FirstOrDefault();
                if (_permision != null && _permision.IsActive == false)
                {

                    return Json(new { Validation = false, Message = "هذا الكارت او مقدم الخدمه ليس له صلاحيه للصرف لهذه الخدمه... برجاء الرجوع للإداره الطبيه ", Limit = 0, CeilingPert = 0 });
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
                    Message = "Service is Not Coverted";
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
                    //string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    //DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                    List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    bool LimitLabPreceptionCount = false;
                    bool LimitRayPreceptionCount = false;
                    //For lab
                    if (ServiceCode == "11206")
                    {
                        List<Roshita> LabAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Lab").ToList();
                        LimitLabPreceptionCount = (CustemizedMedEmp.LAB_NO_MON == null || (CustemizedMedEmp.LAB_NO_MON - LabAcumlatorList.Count() > 0)) ?
                            true : false;//Monthly count
                        Double LimiLabPreceptionAmount = CustemizedMedEmp.LAB_NO == null ? Convert.ToDouble(CustemizedMedEmp.LAB_NO) :
                                (Convert.ToDouble(CustemizedMedEmp.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                .001 : Convert.ToDouble(CustemizedMedEmp.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                        CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.LAB_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                        if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount, CeilingPert = CeilingPert });
                            }

                        }
                        //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount });

                    }

                    // For Ray
                    else if (ServiceCode == "11201")
                    {
                        List<Roshita> RayAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Ray").ToList();
                        LimitRayPreceptionCount = (CustemizedMedEmp.LAB_NO_MON == null || (CustemizedMedEmp.LAB_NO_MON - RayAcumlatorList.Count() > 0)) ?
                           true : false;//Monthly count
                        Double LimiLabPreceptionAmount = CustemizedMedEmp.RAY_NO == null ? Convert.ToDouble(CustemizedMedEmp.RAY_NO) :
                                (Convert.ToDouble(CustemizedMedEmp.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                .001 : Convert.ToDouble(CustemizedMedEmp.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                        CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.RAY_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                        if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount, CeilingPert = CeilingPert });
                            }

                        }
                        //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount });

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
                        //string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        //DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                        bool LimitLabPreceptionCount = false;
                        bool LimitRayPreceptionCount = false;
                        //For lab
                        if (ServiceCode == "11206")
                        {
                            List<Roshita> LabAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Lab").ToList();
                            LimitLabPreceptionCount = (CustemizedMed.LAB_NO_MON == null || (CustemizedMed.LAB_NO_MON - LabAcumlatorList.Count() > 0)) ?
                                true : false;//Monthly count
                            Double LimiLabPreceptionAmount = CustemizedMed.LAB_NO == null ? Convert.ToDouble(CustemizedMed.LAB_NO) :
                                    (Convert.ToDouble(CustemizedMed.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                    .001 : Convert.ToDouble(CustemizedMed.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                            CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMed.LAB_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                            if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                            {
                                CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                    return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount, CeilingPert = CeilingPert });
                                }

                            }
                            //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                            return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount });

                        }

                        // For Ray
                        else if (ServiceCode == "11201")
                        {
                            List<Roshita> RayAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Ray").ToList();
                            LimitRayPreceptionCount = (CustemizedMed.RAY_NO_MON == null || (CustemizedMed.RAY_NO_MON - RayAcumlatorList.Count() > 0)) ?
                               true : false;//Monthly count
                            Double LimiLabPreceptionAmount = CustemizedMed.RAY_NO == null ? Convert.ToDouble(CustemizedMed.RAY_NO) :
                                    (Convert.ToDouble(CustemizedMed.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                    .001 : Convert.ToDouble(CustemizedMed.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                            CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMed.RAY_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                            if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                            {
                                CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                    return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount, CeilingPert = CeilingPert });
                                }

                            }
                            //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                            return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount });

                        }

                    }
                }

                #region Old CoInsurance
                //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                //string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                //DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                //List<Roshita> YearlyDailyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Daily").ToList();
                //List<Roshita> YearlyMonthlyAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
                //List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
                //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
                //bool LimitDailyPreceptionCount = false;
                //bool LimitMonthlyPreceptionCount = false;
                //LimitDailyPreceptionCount = (CoInsurancelimit.NO_CLEEM_YEAR == 0 || (CoInsurancelimit.NO_CLEEM_YEAR - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
                //LimitMonthlyPreceptionCount = (CoInsurancelimit.NO_CLAEM_WEEK == 0 || (CoInsurancelimit.NO_CLAEM_WEEK - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
                //LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CoInsurancelimit.NO_CLM_DAY_YYYY == 0 || (CoInsurancelimit.NO_CLM_DAY_YYYY - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
                //LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CoInsurancelimit.NO_CLM_MON_YYYY == 0 || (CoInsurancelimit.NO_CLM_MON_YYYY - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

                //Double LimitDailyMonthlyPreceptionAmount = CoInsurancelimit.COST_DALLY_YEAR == 0 ? Convert.ToDouble(CoInsurancelimit.COST_DALLY_YEAR) : (Convert.ToDouble(CoInsurancelimit.COST_DALLY_YEAR - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CoInsurancelimit.COST_DALLY_YEAR - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                //Double LimitMonthlyMonthlyPreceptionAmount = CoInsurancelimit.COST_MONTHLY_YEAR == 0 ? Convert.ToDouble(CoInsurancelimit.COST_DALLY_YEAR) : (Convert.ToDouble(CoInsurancelimit.COST_MONTHLY_YEAR - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CoInsurancelimit.COST_MONTHLY_YEAR - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Monthly Amount

                //Double LimitDailyYearlyPreceptionAmount = CoInsurancelimit.MONY_CLM_DAY_YYYY == 0 ? Convert.ToDouble(CoInsurancelimit.MONY_CLM_DAY_YYYY) : (Convert.ToDouble(CoInsurancelimit.MONY_CLM_DAY_YYYY - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CoInsurancelimit.MONY_CLM_DAY_YYYY - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Daily Amount
                //Double LimitMonthlyYearlyPreceptionAmount = CoInsurancelimit.MONY_CLM_MON_YYYY == 0 ? Convert.ToDouble(CoInsurancelimit.MONY_CLM_MON_YYYY) : (Convert.ToDouble(CoInsurancelimit.MONY_CLM_MON_YYYY - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ? .001 : Convert.ToDouble(CoInsurancelimit.MONY_CLM_MON_YYYY - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment)));//Yearly&Monthly Amount

                //CoInsurancelimit.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? CoInsurancelimit.INSURANCE_DAY : Math.Min(Convert.ToDouble(CoInsurancelimit.INSURANCE_DAY), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
                //CoInsurancelimit.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? CoInsurancelimit.INSURANCE_DAY : Math.Min(Convert.ToDouble(CoInsurancelimit.INSURANCE_DAY), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
                //CoInsurancelimit.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? CoInsurancelimit.INSURANCE_MONTH : Math.Min(Convert.ToDouble(CoInsurancelimit.INSURANCE_MONTH), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));
                //CoInsurancelimit.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? CoInsurancelimit.INSURANCE_MONTH : Math.Min(Convert.ToDouble(CoInsurancelimit.INSURANCE_MONTH), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));

                //if (CoInsurancelimit.INSURANCE_DAY < 0)
                //{
                //    CoInsurancelimit.INSURANCE_DAY = .001;
                //}
                //if (CoInsurancelimit.INSURANCE_MONTH < 0)
                //{
                //    CoInsurancelimit.INSURANCE_MONTH = .001;
                //}
                ////approval ceiling
                //if (Validation == false)
                //{
                //    var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
                //    if (accption == null)
                //    {
                //        return Json(new { Validation = false, Message = "Exceeded his annual contract limit", Limit = 0, CeilingPert = 0 });
                //    }
                //    var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
                //    if (reasons != null)
                //    {
                //        return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert });
                //    }

                //}
                ////return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                //return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount });
                #endregion

            }
            return Json(new { Validation = false, Message = "Employee contract issue ,you can call operation department", Limit = 0, CeilingPert = 0 });
        }

        public JsonResult CellingAmountEditPage(string id, string ServiceCode, Int64 RoshitaId)
        {
            string Message = "";
            ServiceCode = ServiceCode == "11604" ? "11601" : ServiceCode;
            string MainService = ServiceCode.Substring(0, 3);
            var emp = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
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
                    Message = "Service is Not Coverted";
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
                var CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
                  && c.SER_SERV == ServiceCode && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                if (CustemizedMedEmp != null)
                {
                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
                    //string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                    //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                    //DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                    //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                    List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                    bool LimitLabPreceptionCount = false;
                    bool LimitRayPreceptionCount = false;
                    //For lab
                    if (ServiceCode == "11206")
                    {
                        List<Roshita> LabAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Lab").ToList();
                        LimitLabPreceptionCount = (CustemizedMedEmp.LAB_NO_MON == null || (CustemizedMedEmp.LAB_NO_MON - LabAcumlatorList.Count() > 0)) ?
                            true : false;//Monthly count
                        Double LimiLabPreceptionAmount = CustemizedMedEmp.LAB_NO == null ? Convert.ToDouble(CustemizedMedEmp.LAB_NO) :
                                (Convert.ToDouble(CustemizedMedEmp.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                .001 : Convert.ToDouble(CustemizedMedEmp.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                        CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.LAB_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                        if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount, CeilingPert = CeilingPert });
                            }

                        }
                        //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount });

                    }

                    // For Ray
                    else if (ServiceCode == "11201")
                    {
                        List<Roshita> RayAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Ray").ToList();
                        LimitRayPreceptionCount = (CustemizedMedEmp.LAB_NO_MON == null || (CustemizedMedEmp.LAB_NO_MON - RayAcumlatorList.Count() > 0)) ?
                           true : false;//Monthly count
                        Double LimiLabPreceptionAmount = CustemizedMedEmp.RAY_NO == null ? Convert.ToDouble(CustemizedMedEmp.RAY_NO) :
                                (Convert.ToDouble(CustemizedMedEmp.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                .001 : Convert.ToDouble(CustemizedMedEmp.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                        CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMedEmp.RAY_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                        if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                        {
                            CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount, CeilingPert = CeilingPert });
                            }

                        }
                        //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount });

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
                        //string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        //string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
                        //DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        //DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
                        List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && r.Id != RoshitaId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
                        bool LimitLabPreceptionCount = false;
                        bool LimitRayPreceptionCount = false;
                        //For lab
                        if (ServiceCode == "11206")
                        {
                            List<Roshita> LabAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Lab").ToList();
                            LimitLabPreceptionCount = (CustemizedMed.LAB_NO_MON == null || (CustemizedMed.LAB_NO_MON - LabAcumlatorList.Count() > 0)) ?
                                true : false;//Monthly count
                            Double LimiLabPreceptionAmount = CustemizedMed.LAB_NO == null ? Convert.ToDouble(CustemizedMed.LAB_NO) :
                                    (Convert.ToDouble(CustemizedMed.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                    .001 : Convert.ToDouble(CustemizedMed.LAB_NO - (LabAcumlatorList.Sum(x => x.PersonPayment) + LabAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                            CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMed.LAB_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                            if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                            {
                                CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                    return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount, CeilingPert = CeilingPert });
                                }

                            }
                            //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                            return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitLabPreceptionCount });

                        }

                        // For Ray
                        else if (ServiceCode == "11201")
                        {
                            List<Roshita> RayAcumlatorList = MainAcumlatorList.Where(x => x.Manager == "Ray").ToList();
                            LimitRayPreceptionCount = (CustemizedMed.RAY_NO_MON == null || (CustemizedMed.RAY_NO_MON - RayAcumlatorList.Count() > 0)) ?
                               true : false;//Monthly count
                            Double LimiLabPreceptionAmount = CustemizedMed.RAY_NO == null ? Convert.ToDouble(CustemizedMed.RAY_NO) :
                                    (Convert.ToDouble(CustemizedMed.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)))) == 0 ?
                                    .001 : Convert.ToDouble(CustemizedMed.RAY_NO - (RayAcumlatorList.Sum(x => x.PersonPayment) + RayAcumlatorList.Sum(x => x.CompanyPayment)));//Monthly&Daily Amount
                            CoInsurancelimit2.INSURANCE_DAY_LAB = LimiLabPreceptionAmount == 0 ? 0 : Math.Min(Convert.ToDouble(CustemizedMed.RAY_NO), Convert.ToDouble(LimiLabPreceptionAmount));
                            if (CoInsurancelimit2.INSURANCE_DAY_LAB < 0)
                            {
                                CoInsurancelimit2.INSURANCE_DAY_LAB = .001;
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
                                    return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount, CeilingPert = CeilingPert });
                                }

                            }
                            //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
                            return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitRayPreceptionCount });

                        }

                    }
                }


            }
            //return Json(new { Validation = false, Limit = 0, CeilingPert = 0 });
            return Json(new { Validation = false, Message = "Employee contract issue ,you can call operation department", Limit = 0, CeilingPert = 0 });

        }

        #endregion

    }


}
