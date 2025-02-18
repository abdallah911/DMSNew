using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_TEST.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DMS_Authontication1.Controllers
{
    [Authorize(Roles = "Admin,Rays,Rays_Admin")]
    public class RaysController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public RaysController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();

        }
        #region Rays
        [Authorize(Roles = "Admin,Rays,Rays_Admin")]

        public ActionResult Ray()
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
              .Where(z => z.x.CardId == id && z.d.MedicienCode == code && z.x.Manager == "Ray" && z.d.PaymentGroup != "Cash" && z.d.IsDealed == true)
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
        [Authorize(Roles = "Admin,Rays,Rays_Admin")]
        public JsonResult GetList(int sEcho = 1, int iDisplayStart = 0, int iDisplayLength = 24, string sSearch = "")
        {
            ApplicationDbContext myEntities = new ApplicationDbContext();
            var labs = new List<Serv_Lab>();
            var user = myEntities.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            long userProvider = Convert.ToInt64(user.Provider);
            long lgSearch;
            long.TryParse(sSearch, out lgSearch);
            // bool UserRole = User.IsInRole("Rays");
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Serv_Ray.Where(x => x.LAB_CODE == userProvider && x.LOOK == 0).Where(r => sSearch != "" ? r.SERV_ANAME.Contains(sSearch) || r.SERV_CODE == lgSearch : true)
                // .Where(x => UserRole ? x.LAB_CODE == userProvider : x.LAB_CODE == 1120101/*11206009*/)
                .OrderBy(m => m.SERV_CODE)
          .Select(l => new
          {
              SERV_CODE = l.SERV_CODE,
              SERV_ANAME = l.SERV_ANAME,
          }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.Serv_Lab.Where(x => x.LAB_CODE == userProvider).Where(r => sSearch != "" ? r.SERV_ANAME.Contains(sSearch) || r.Id == lgSearch : true)
                //.Where(x => UserRole ? x.LAB_CODE == userProvider : x.LAB_CODE == 1120101)
                .Count(),
                iTotalDisplayRecords = db.Serv_Lab.Where(x => x.LAB_CODE == userProvider).Where(r => sSearch != "" ? r.SERV_ANAME.Contains(sSearch) || r.Id == lgSearch : true)
                //.Where(x => UserRole ? x.LAB_CODE == userProvider : x.LAB_CODE == 1120101)
                .Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetRayByCode(long code)
        {
            var user = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            long userProvider = Convert.ToInt64(user.Provider);
            var Data = db.Serv_Ray.Where(x => x.LAB_CODE == userProvider && x.LOOK == 0).
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

        [Authorize(Roles = "Admin,Rays,Rays_Admin")]

        public JsonResult Save(Roshita data)
        {
            var companyId = data.CardId.Split('-')[0];
            ApprovalCode modelcode = new ApprovalCode();
            DateTime datenow = DateTime.Now.Date;
            var employee = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId && c.INS_START_DATE <= datenow && c.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

            if (companyId == "888" && data.ClaimNumber != null)
            {
                var claimchick = data.ClaimNumber.ToString();
                modelcode = db.ApprovalCodes.Where(x => x.Code == claimchick && x.Card_ID == data.CardId && x.IsActive).FirstOrDefault();
                if (modelcode == null)
                {
                    return Json("Failed");
                }
                modelcode.IsActive = false;
                db.Entry(modelcode).State = EntityState.Modified;
            }
            if (data.CreatedBy == null || data.CreatedBy == "")
            {
                data.CreatedBy = User.Identity.Name;
            }
            var compholder = db.Contract_Data.Where(c => c.C_COMP_ID == employee.C_COMP_ID).OrderByDescending(c => c.CONTRACT_NO).Select(c => c.COMP_ID).First();

            data.CreatedDate = DateTime.Now;
            data.Manager = "Ray";
            data.RoshetaType = "11204";
            data.CompHolderCode =compholder;
            db.Roshitas.Add(data);
            if (data.CompanyPayment > 0)
            {
                var EmpCode = data.CardId.Split('-')[2];
                var CompCodeCard = data.CardId.Split('-')[0];
                if (data.IsFamily == "Y" && data.IsPool != "Y")
                {
                    var remaining = db.RemainConsumptions.Where(r => SqlFunctions.PatIndex(CompCodeCard + "-%-" + EmpCode + "-%", r.CARD_ID) > 0)
                    .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (remaining != null)
                    {
                        remaining.REMAINING = remaining.REMAINING - data.CompanyPayment;
                        remaining.NET = remaining.NET + data.CompanyPayment;
                        db.Entry(remaining).State = EntityState.Modified;
                    }
                }
                if (data.IsPool == "Y")
                {
                    var CompCode = int.Parse(data.CardId.Split('-')[0]);
                    var remaining = db.CONSUMPTION_POOL.Where(r => r.COMP_ID == CompCode)
                        .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (remaining != null)
                    {
                        remaining.REMAINING = remaining.REMAINING - data.CompanyPayment;
                        db.Entry(remaining).State = EntityState.Modified;
                    }
                }
                if (data.IsFamily != "Y" && data.IsPool != "Y")
                {
                    var remaining = db.RemainConsumptions.Where(r => r.CARD_ID == data.CardId)
                    .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (remaining != null)
                    {
                        remaining.REMAINING = remaining.REMAINING - data.CompanyPayment;
                        remaining.NET = remaining.NET + data.CompanyPayment;
                        db.Entry(remaining).State = EntityState.Modified;
                    }
                }

            }
            if (data.ClaimNumber != null)
            {
                long claim = long.Parse(data.ClaimNumber.Value.ToString());
                var claimphoto = db.ClaimPhotoes.Where(x => x.CardId == data.CardId && x.ClaimNumber == claim).FirstOrDefault();
                if (claimphoto != null)
                {
                    claimphoto.IsDispense = (claimphoto.IsDespenseLab.Value && claimphoto.IsDespensePharm.Value) ? true : false;
                    claimphoto.IsDespenseRay = true;
                    claimphoto.UpdatedBy = User.Identity.Name;
                    claimphoto.UpdatedDate = DateTime.Now;
                    db.Entry(claimphoto).State = EntityState.Modified;

                }
            }
            int result = db.SaveChanges();
            Session["id"] = data.Id;
            return Json("2" + data.CreatedDate.Value.ToString("ddMMyy") + data.Id);
        }
        [Authorize(Roles = "Admin,Rays,Rays_Admin")]
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
                        notification.SentTo = CardId.Split('-')[0] == "888" ? "AdminHelth" : "Admin";
                        notification.CreatedBy = User.Identity.Name;
                        notification.CreatedDate = DateTime.Now;
                        notification.Type = 1;//pending
                        notification.TypeNmae = "Ray";//pending
                        notification.Details = CardId;
                        notification.RoshitaId = Medicien.RoshitaID;
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

        [Authorize(Roles = "Admin,Rays,Rays_Admin")]
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
        public ActionResult Pending(int NotificationId)
        {
            var model = db.Notifications.Where(n => n.Id == NotificationId && n.IsDeleted == false && n.IsRead == false)
                .Include(x => x.Roshita).Include(r => r.Roshita.RoshitaDetails).Include(rd => rd.Roshita.PrescriptionRoshitaDignosis)
                .FirstOrDefault();
            model.Roshita.RoshitaDetails = model.Roshita.RoshitaDetails.Where(x => x.PaymentGroup == "Pending" || x.PaymentGroup == "Accepted"
            || x.PaymentGroup == "Rejected").ToList();
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
        public JsonResult ChangeStatus(int ApprovalId, string status)
        {

            var emp = db.RoshitaDetails.Where(x => x.Id == ApprovalId).FirstOrDefault();
            bool Flag;
            if (status == "true")
            {
                Flag = true;
                var Roshta = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault();
                double OldCompanyPayment = Roshta.CompanyPayment;
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
                            Roshta.Cash -= Roshta.PersonPayment;

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
                Notification notification = db.Notifications.Where(x => x.RoshitaId == emp.RoshitaID).OrderByDescending(x => x.Id).FirstOrDefault();
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
        //    var approvals = db.Roshitas.Where(x => x.CardId == id && x.CreatedBy == User.Identity.Name && x.Manager == "Ray")
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
        //--------------------------------------------------------------------------------------------------------
        #region  Control Panel Rays
        [Authorize(Roles = "Admin,Rays,Rays_Admin")]

        public ActionResult Index()
        {
            if (User.IsInRole("Rays"))
            {
                ViewBag.ddlUsers = new SelectList(UserDB.Users.ToList(), "UserName", "UserName", User.Identity.Name);
            }
            else if (User.IsInRole("Rays_Admin"))
            {
                var CurrentUser = UserDB.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                ViewBag.ddlUsers = new SelectList(UserDB.Users.Where(x => x.Provider == CurrentUser.Provider).ToList(), "UserName", "UserName", User.Identity.Name);
            }
            ViewBag.comphoder = new SelectList(db.CompHolders.ToList(), "CompHolderCode", "CompHolderName");
            return View();
        }
        public JsonResult PreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "", string Company = "",
            string Provider = "", string From = "", string To = "", string CardId = "", string Branch = "", string ApprovalNo = "", int CompHoder = 0)
        {
            string Type = "Ray";
            if (!User.IsInRole("Admin") && From != "" && To != "")
            {
                if (To != "")
                {
                    DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                    To = T.ToString();
                }
                if (User.IsInRole("Rays"))
                {
                    Branch = User.Identity.Name;
                }
                else if (User.IsInRole("Rays_Admin") && Branch == "")
                {
                    var context = new ApplicationDbContext();
                    Provider = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Provider;
                }
                var RaysResult = new
                {
                    sEcho = sEcho,
                    aaData = db.fn_AdminRayClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type, CompHoder).OrderByDescending(m => m.Id)
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

                    iTotalRecords = db.fn_AdminRayClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type, CompHoder).Count(),
                    iTotalDisplayRecords = db.fn_AdminRayClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type, CompHoder).Count()
                };
                return new JsonResult { Data = RaysResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


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
                    aaData = db.fn_AdminRayClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type, CompHoder).OrderByDescending(m => m.Id)
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

                    iTotalRecords = db.fn_AdminRayClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type, CompHoder).Count(),
                    iTotalDisplayRecords = db.fn_AdminRayClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type, CompHoder).Count()
                };
                return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Stop") && (CompHoder != 0 ? x.CompHolderCode == CompHoder : true)).AsEnumerable()
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

                iTotalRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Stop") && (CompHoder != 0 ? x.CompHolderCode == CompHoder : true)).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Count(),
                iTotalDisplayRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Stop") && (CompHoder != 0 ? x.CompHolderCode == CompHoder : true)).OrderBy(m => m.Id).AsEnumerable()
                .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || ((r.Oracle_Id == null || r.Oracle_Id == 0) ? Convert.ToString("2" + r.CreatedDate.Value.ToString("ddMMyy") + r.Id) : Convert.ToString(r.Oracle_Id)).Contains(sSearch) : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };





        }
        public JsonResult PreseptionAdminCount(string Company = "", string Provider = "", string From = "", string To = "", string CardId = "",
            string Branch = "", string ApprovalNo = "", int CompHoder = 0)
        {


            if (To != "")
            {
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);
                To = T.ToString();
            }
            var Adminresult = db.fn_AdminRayClamsCounts(From, To, Company, Provider, Branch, ApprovalNo, CardId, "Ray", CompHoder).FirstOrDefault();

            return new JsonResult { Data = Adminresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        //public JsonResult PreseptionList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        //{
        //    long lgSearch;
        //    long.TryParse(sSearch, out lgSearch);
        //    var result = new
        //    {
        //        sEcho = sEcho,
        //        aaData = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Pharmacy") && !x.Manager.Contains("Daily") && !x.Manager.Contains("Monthly") && !x.Manager.Contains("Rays") && !x.Manager.Contains("Stop")).OrderByDescending(m => m.Id).AsEnumerable()
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

        //        iTotalRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name && !x.Manager.Contains("Pharmacy") && !x.Manager.Contains("Daily") && !x.Manager.Contains("Monthly") && !x.Manager.Contains("Rays") && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
        //        .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || r.Id == lgSearch : true).Count(),
        //        iTotalDisplayRecords = db.Roshitas.Where(x => x.CreatedBy == User.Identity.Name  && !x.Manager.Contains("Pharmacy") && !x.Manager.Contains("Daily") && !x.Manager.Contains("Monthly") && !x.Manager.Contains("Rays") && !x.Manager.Contains("Stop")).OrderBy(m => m.Id).AsEnumerable()
        //        .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Manager.Contains(sSearch) || r.Id == lgSearch : true).Where(x => x.CreatedDate.Value.AddDays(7).Date > DateTime.Now.Date).Count()
        //    };
        //    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        //}

        [Authorize(Roles = "Admin,Rays,Rays_Admin")]

        public ActionResult Details(int id)
        {

            List<DoctorContainerViewModel> data = db.RoshitaDetails.Where(l => l.RoshitaID == id && l.IsDealed == true).AsEnumerable()
               //.Join(db.Serv_Ray, d => d.MedicienCode, m => Convert.ToString(m.SERV_CODE), (d, m) => new { d, m })

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

        [HttpPost]
        public JsonResult RayDelete(int id)
        {
            try
            {
                Roshita roshta = db.Roshitas.Where(c => c.Id == id).FirstOrDefault();
                if (roshta.Manager == "Ray")
                {
                    roshta.Manager = "Ray_Stop";
                }
                roshta.UpdatedBy = User.Identity.Name;
                roshta.UpdatedDate = DateTime.Now;
                db.Entry(roshta).State = EntityState.Modified;
                var companyId = roshta.CardId.Split('-')[0];
                ApprovalCode modelcode = new ApprovalCode();
                if (companyId == "888")
                {
                    var claimchick = roshta.ClaimNumber.ToString();
                    modelcode = db.ApprovalCodes.Where(x => x.Code == claimchick && x.Card_ID == roshta.CardId).FirstOrDefault();
                    if (modelcode != null)
                    {
                        modelcode.IsActive = true;
                        modelcode.UpdatedBy = User.Identity.Name;
                        modelcode.UpdatedDate = DateTime.Now;
                        db.Entry(modelcode).State = EntityState.Modified;
                    }

                }
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
                if (roshta.ClaimNumber != null)
                {
                    long claim = long.Parse(roshta.ClaimNumber.Value.ToString());
                    var claimphoto = db.ClaimPhotoes.Where(x => x.CardId == roshta.CardId && x.ClaimNumber == claim).FirstOrDefault();
                    if (claimphoto != null)
                    {
                        claimphoto.IsDispense = false;
                        claimphoto.IsDespenseRay = false;
                        claimphoto.UpdatedBy = User.Identity.Name;
                        claimphoto.UpdatedDate = DateTime.Now;
                        db.Entry(claimphoto).State = EntityState.Modified;

                    }
                }
                db.SaveChanges();
                NotificationHub objNotifHub = new NotificationHub();
                Notification notification = db.Notifications.AsEnumerable().Where(x => x.RoshitaId == roshta.Id && x.CreatedDate.ToShortDateString() == roshta.CreatedDate.Value.ToShortDateString()).OrderByDescending(x => x.Id).FirstOrDefault();
                if (notification != null)
                {
                    notification.IsRead = true;
                    db.Entry(notification).State = EntityState.Modified;
                    db.SaveChanges();
                    objNotifHub.SendMessages();
                }
                return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Rays,Rays_Admin")]
        public ActionResult Edit(int id)
        {
            var roshita = db.Roshitas.Where(r => r.Id == id && !r.Manager.Contains("Stop")).FirstOrDefault();
            if (roshita != null)
            {
                List<DoctorContainerViewModel> data = db.RoshitaDetails.Where(l => l.RoshitaID == id /*&& l.IsDealed == true*/).AsEnumerable()
                 //  .Join(db.Serv_Ray, d => d.MedicienCode, m =>Convert.ToString(m.SERV_CODE), (d, m) => new { d, m })

                 .Select(l => new DoctorContainerViewModel
                 {
                     Id = l.Id,
                     MedicienCode = l.MedicienCode,
                     MedicienName = l.MedicienName,
                     Amount = l.Amount,
                     IsDealed = l.IsDealed,
                     PaymentGroup = l.PaymentGroup
                 })
                   .GroupBy(x => new { x.MedicienCode })
                .Select(x => x.FirstOrDefault())
                 .ToList();
                return View(data);
            }
            else
            {
                return HttpNotFound();
            }
        }
        public JsonResult Manger(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //var roshitaDetails = db.RoshitaDetails.Where(x => x.Id == id).FirstOrDefault();
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
            roshita.UpdatedBy = User.Identity.Name;
            roshita.UpdatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(roshita).State = EntityState.Modified;
                db.SaveChanges();
            }
            Session["id"] = roshita.Id;
            //data.CreatedDate.ToString();
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
                            notification.TypeNmae = "Ray";//pending
                            notification.Details = CardId;
                            notification.RoshitaId = Medicien.RoshitaID;
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
            roshita1.CompHolderCode = roshita.CompHolderCode;

            roshita.Manager = "Stop-ED";
            roshita.SyncBy = "Update";
            roshita.UpdatedBy = User.Identity.Name;
            roshita.UpdatedDate = DateTime.Now;

            db.Entry(roshita).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                RoshitaAcception roshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == data.Id).FirstOrDefault();
                if (roshitaAcception != null)
                {
                    db.RoshitaAcceptions.Remove(roshitaAcception);

                }
            }
            roshita1.PrescriptionRoshitaDignosis = new List<PrescriptionRoshitaDignosi>();
            foreach (var item in roshita.PrescriptionRoshitaDignosis)
            {
                roshita1.PrescriptionRoshitaDignosis.Add(new PrescriptionRoshitaDignosi
                {
                    DiagnoiseName = item.DiagnoiseName
                });
            }
            // RoshitaDetails

            roshita1.RoshitaDetails = new List<RoshitaDetail>();

            List<RoshitaDetail> List_R_Details = db.RoshitaDetails.Where(x => x.RoshitaID == data.Id).ToList();
            bool oneNotification = (List_R_Details.Where(x => x.RoshitaID == data.Id && x.PaymentGroup == "Pending").ToList().Count == 0) ? false : true;

            if (oneNotification)
            {
                var noteficationdelete = db.Notifications.Where(n => n.RoshitaId == roshita.Id).FirstOrDefault();
                noteficationdelete.IsDeleted = true;
                noteficationdelete.IsRead = true;
                db.Entry(noteficationdelete).State = EntityState.Modified;
                oneNotification = false;
            }
            var oldpending = List_R_Details.Where(r => r.RoshitaID == data.Id && (r.PaymentGroup == "Pending" || r.PaymentGroup == "Accepted"
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
                if (oldMedicien.PaymentGroup == "Pending")
                {
                    if (oneNotification == false)
                    {
                        Notification notification = new Notification();
                        notification.SentTo = roshita1.CardId.Split('-')[0] == "888" ? "AdminHelth" : "Admin";
                        notification.CreatedBy = User.Identity.Name;
                        notification.CreatedDate = DateTime.Now;
                        notification.Type = 1;//pending
                        notification.TypeNmae = "Ray";//pending
                        notification.Details = roshita1.CardId;
                        notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                        notification.Title = "Pending";
                        roshita1.Notifications.Add(notification);
                        oneNotification = true;
                    }
                }

                old.PaymentGroup = old.PaymentGroup + "-Stop";
                db.Entry(old).State = EntityState.Modified;

            }
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                Medicien.RoshitaID = 0;
                Medicien.Dose = 0;
                Medicien.Duration = 0;
                Medicien.TotalDuration = 7;
                Medicien.TotalUnits = 1;

                if (Medicien.PaymentGroup == "Pending" || Medicien.PaymentGroup == "Cash")
                {
                    if (oneNotification == false && Medicien.PaymentGroup == "Pending")
                    {
                        //if new pending and didn't have notification
                        //NotificationHub objNotifHub = new NotificationHub();

                        Notification notification = new Notification();
                        notification.SentTo = roshita1.CardId.Split('-')[0] == "888" ? "AdminHelth" : "Admin";
                        notification.CreatedBy = User.Identity.Name;
                        notification.CreatedDate = DateTime.Now;
                        notification.Type = 1;//pending
                        notification.TypeNmae = "Ray";//pending
                        notification.Details = roshita1.CardId;
                        //notification.RoshitaId = roshita1.Id;
                        notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                        notification.Title = "Pending";
                        roshita1.Notifications.Add(notification);
                        //db.Notifications.Add(notification);
                        //objNotifHub.SendMessages();
                        oneNotification = true;
                    }
                    Medicien.IsDealed = (Medicien.PaymentGroup == "Cash") ? true : false;
                }
                else
                {
                    Medicien.IsDealed = true;
                }

                roshita1.RoshitaDetails.Add(Medicien);
            }


            ////
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
                NotificationHub objNotifHub = new NotificationHub();
                objNotifHub.SendMessages();
                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita1.Id);

            }
            catch (DbEntityValidationException e)
            {
                return Json("Failed to Save Prescription");
            }

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
            if (string.IsNullOrEmpty(patient.EMP_ENAME_ST) || patient.EMP_ENAME_ST == "NULL")
            {
                if (string.IsNullOrEmpty(patient.EMP_ANAME_ST) || patient.EMP_ANAME_ST == "NULL")
                {
                    rd.SetParameterValue("PatientName", "Unnamed");
                }
                else
                {
                    rd.SetParameterValue("PatientName", patient.EMP_ANAME_ST + " " + patient.EMP_ANAME_SC + " " + patient.EMP_ANAME_TH);
                }
            }
            else
            {
                rd.SetParameterValue("PatientName", patient.EMP_ENAME_ST + " " + patient.EMP_ENAME_SC + " " + patient.EMP_ENAME_TH);
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
            if (data.Cash != null)
                rd.SetParameterValue("Cash", data.Cash);
            else rd.SetParameterValue("Cash", 0); Response.Buffer = false;
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

        public ActionResult PrintClams(string From, string To, string Branch = "", int CompHoder = 0)
        {
            try
            {
                List<string> branches = new List<string>();
                if (User.IsInRole("Rays"))
                {
                    Branch = User.Identity.Name;
                }
                else if (User.IsInRole("Rays_Admin") && Branch == "")
                {
                    var context = new ApplicationDbContext();
                    Branch = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Provider;
                    branches = context.Users.Where(x => x.Provider == Branch).Select(u => u.UserName).ToList();
                }
                DateTime F = Convert.ToDateTime(From);
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);//to get all pervious day
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ClamsReportLabs.rpt"));

                var y = db.Roshitas.Where(r => r.CreatedDate >= F && r.CreatedDate <= T && !r.Manager.Contains("Stop") && (r.CreatedBy == User.Identity.Name || branches.Contains(r.CreatedBy)))
                   .Join(db.Comp_Employees, r => r.CardId, m => m.CARD_ID, (r, m) => new { r, m })
                   .Where(x => x.m.INS_START_DATE <= x.r.CreatedDate && x.m.INS_END_DATE >= x.r.CreatedDate && (CompHoder != 0 ? x.r.CompHolderCode == CompHoder : true))
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
    }
}