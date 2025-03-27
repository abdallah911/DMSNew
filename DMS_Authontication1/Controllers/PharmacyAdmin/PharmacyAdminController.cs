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
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using DMS_Authontication1.ViewModel.PharmacyAdmin;

namespace DMS_TEST.Controllers
{

    public class PharmacyAdminController : Controller
    {
        // GET: Pharmacy
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public PharmacyAdminController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();
        }

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
                    aaData = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type,1).OrderByDescending(m => m.Id)
               .Select(l => new
               {
                   Id = l.Id,
                   Oracle_Id = l.Oracle_Id,
                   CardId = l.CardId,
                   CompanyPercent = l.CompanyPercent,
                   TotalValue = l.TotalValue,
                   Manager = l.Manager,
                   CreatedDate = l.CreatedDate,
                   CreatedBy = l.CreatedBy,
               }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type,1).Count(),
                    iTotalDisplayRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type,1).Count()
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
                    aaData = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type,1).OrderByDescending(m => m.Id)
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

                    iTotalRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type,1).Count(),
                    iTotalDisplayRecords = db.fn_AdminClamsList(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type,1).Count()
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
            var Adminresult = db.fn_AdminClamsCounts(From, To, Company, Provider, Branch, ApprovalNo, CardId, Type,1).FirstOrDefault();

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
                if (roshta.Manager == "Daily")
                {
                    string CardId = roshta.CardId;
                    NotificationHub objNotifHub = new NotificationHub();
                    Notification notification = db.Notifications.AsEnumerable().Where(x => x.Details == CardId && x.CreatedDate.ToShortDateString() == roshta.CreatedDate.Value.ToShortDateString()).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (notification != null)
                    {
                        notification.IsRead = true;
                        db.Entry(notification).State = EntityState.Modified;
                        objNotifHub.SendMessages();
                    }
                    roshta.Manager = "Daily_Stop";
                }
                else if (roshta.Manager == "Monthly")
                {
                    string CardId = roshta.CardId;
                    NotificationHub objNotifHub = new NotificationHub();
                    Notification notification = db.Notifications.AsEnumerable().Where(x => x.Details == CardId && x.CreatedDate.ToShortDateString() == roshta.CreatedDate.Value.ToShortDateString()).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (notification != null)
                    {
                        notification.IsRead = true;
                        db.Entry(notification).State = EntityState.Modified;
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
                    string CardId = db.Roshitas.Where(x => x.Id == id).FirstOrDefault().CardId;
                    var roshitaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == id).ToList();
                    long DoctorRositaId = db.Roshitas.Where(x => x.CardId == CardId && (x.Manager == "Doctor_Daily" /*|| x.Manager == "Doctor_Chronic"*/)).OrderByDescending(x => x.CreatedDate).FirstOrDefault().Id;
                    var DoctrorchronicRositaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == DoctorRositaId).ToList();
                    foreach (RoshitaDetail item in roshitaDetails)
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
            List<DoctorContainerViewModel> data = db.RoshitaDetails
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
                     PaymentGroup = l.d.PaymentGroup,
                     DOSAGE_FORM = l.m.DOSAGE_FORM,
                     UNIT_NO = l.m.UNIT_NO,
                     PACK_PRICE = l.m.PACK_PRICE,
                     PACK_SIZE = l.m.PACK_SIZE,
                     UNIT_PRICE = l.m.UNIT_PRICE
                 })
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
            roshita.PatchId = data.PatchId;
            if (ModelState.IsValid)
            {
                db.Entry(roshita).State = EntityState.Modified;
                RoshitaAcception roshitaAcception = db.RoshitaAcceptions.Where(x => x.RoshitaId == data.Id).FirstOrDefault();
                if (roshitaAcception != null)
                {
                    db.RoshitaAcceptions.Remove(roshitaAcception);

                }
                db.SaveChanges();
            }
            Session["id"] = data.Id;
            return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);
        }

        public JsonResult UpdateMediciens(List<RoshitaDetail> Medciens)
        {
            long id = Convert.ToInt64(Session["id"]);
            //var mediciens = db.RoshitaDetails.Where(x => x.RoshitaID == id && x.PaymentGroup != "Pending").ToList();
            List<RoshitaDetail> List_R_Details = db.RoshitaDetails.Where(x => x.RoshitaID == id).ToList();
            var mediciens = List_R_Details.Where(x => x.RoshitaID == id && x.IsDealed == true).ToList();
            db.RoshitaDetails.RemoveRange(mediciens);
            //db.SaveChanges();
            bool oneNotification = (List_R_Details.Where(x => x.RoshitaID == id && x.PaymentGroup == "Pending").ToList().Count == 0) ? false : true; ;
            foreach (RoshitaDetail Medicien in Medciens)
            {
                Medicien.RoshitaID = Convert.ToInt64(Session["id"]);
                //Medicien.IsDealed = true;
                //Medicien.Dose = 0;
                //Medicien.Duration = 0;
                //Medicien.TotalDuration = 7;
                //Medicien.TotalUnits = 1;

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
            //foreach (RoshitaDetail Medicien in Medciens)
            //{
            //    Medicien.RoshitaID = Convert.ToInt64(Session["id"]);
            //    Medicien.IsDealed = true;
            //    db.RoshitaDetails.Add(Medicien);
            //}
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
                var patient = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId && c.INS_START_DATE <= data.CreatedDate && c.INS_END_DATE >= data.CreatedDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
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
                    PaymentGroup = d.r.PaymentGroup//type
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
                if (data.RoshetaType == "11601")
                    data.RoshetaType = "Daily";
                if (data.RoshetaType == "11603")
                    data.RoshetaType = "Monthly";
                if (data.RoshetaType == "11602")
                {
                    data.RoshetaType = "Pharmacy_Chronic";
                    med_card = db.Med_Card.Where(m => m.CARD_NO == data.CardId).FirstOrDefault();
                    NoOver = med_card.NO_OVER == null ? 0 : med_card.NO_OVER;
                    NoPay = med_card.NO_OVER;
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

                var sericeProviderDiscountsCheck = db.Ser_Prov_Disc.Where(x => x.PROV_ID == ProviderId).ToList();

                if (sericeProviderDiscountsCheck != null && sericeProviderDiscountsCheck.Count > 1)
                {
                    @ViewBag.ErrorM = "يوجد خطأ في بيانات مقدم الخدمة برجاء الرجوع إلى إدارة التعاقدات";
                    return View("~/Views/Shared/Error.cshtml");
                }

                List<RoshitaCompEmolyessReportViewModel> Data = db.fn_ClaimsReport(From, To, Branch,1)//.AsEnumerable()
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
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return View("~/Views/Shared/Error.cshtml");
                // throw ex.Message("There are no calmes");
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

                var sericeProviderDiscountsCheck = db.Ser_Prov_Disc.Where(x => x.PROV_ID == ProviderId).ToList();

                if (sericeProviderDiscountsCheck != null && sericeProviderDiscountsCheck.Count > 1)
                {
                    @ViewBag.ErrorM = "يوجد خطأ في بيانات مقدم الخدمة برجاء الرجوع إلى إدارة التعاقدات";
                    return View("~/Views/Shared/Error.cshtml");
                }

                List<RoshitaCompEmolyessReportViewModel> Data = db.fn_ClaimsReport(From, To, User.Identity.Name,1)//.AsEnumerable()
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

        public JsonResult SaveDiagnoises(string[] DiagnosisList, string Speciality, int Roshitaid)
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


    }
}