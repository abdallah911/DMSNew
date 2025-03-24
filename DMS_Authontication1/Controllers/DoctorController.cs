using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS_TEST.ViewModel;
using DMS_Authontication1.Models;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using DMS_Authontication1.ViewModel;

namespace DMS_TEST.Controllers
{
    public class DoctorController : Controller
    {
        DMS_TESTEntities db;
        public DoctorController()
        {
            db = new DMS_TESTEntities();
        }

        //GET: Doctor
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        public ActionResult Doctor(string id, string NationalId)
        {

            List<DoctorContainerViewModel> RositaDetails = db.RoshitaDetails.Where(d => d.RoshitaID == 0)
                .Select(d => new DoctorContainerViewModel
                {
                    Id = d.Id,
                    MedicienCode = d.MedicienCode,
                    MedicienName = d.MedicienName,
                    Dose = d.Dose,
                    Duration = d.Duration,
                    TotalDuration = d.TotalDuration,
                    TotalUnits = d.TotalUnits,
                    Amount = d.Amount,
                }).ToList();
            return View(RositaDetails);
        }
        [HttpPost]
        public ActionResult Doctor(string id, int TxtSearch)
        {

            var Rosita = db.Roshitas.Where(r => r.CardId == id && r.Id == TxtSearch).FirstOrDefault();
            if (Rosita != null)
            {
                List<DoctorContainerViewModel> data = db.RoshitaDetails
                 .Join(db.MedicineDatas,
                       d => d.MedicienCode, m => m.M_CODE,
                       (d, m) => new { d, m })
                 .Where(l => l.d.RoshitaID == Rosita.Id && l.d.IsDealed == false)
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
                return View(data);
            }
            else
            {
                ViewBag.Message = "No Mediciens";
                List<DoctorContainerViewModel> RositaDetails = db.RoshitaDetails.Where(d => d.RoshitaID == 0)
                          .Select(d => new DoctorContainerViewModel
                          {
                              Id = d.Id,
                              MedicienCode = d.MedicienCode,
                              MedicienName = d.MedicienName,
                              Dose = d.Dose,
                              Duration = d.Duration,
                              TotalDuration = d.TotalDuration,
                              TotalUnits = d.TotalUnits,
                              Amount = d.Amount,
                          }).ToList();
                return View(RositaDetails);
            }
        }

        public ActionResult RoshitaReport(string id)
        {
            //long Approval = Convert.ToInt64(Session["id"]);
            //  long Approval = Convert.ToInt64(id);
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

            //var data = db.Roshitas.Where(r => r.Id == Approval).FirstOrDefault();
            var patient = db.Comp_Employees.Where(b => b.CARD_ID == data.CardId).FirstOrDefault();
            var Company = db.Contract_Comp.Where(c => c.C_COMP_ID == patient.C_COMP_ID).FirstOrDefault();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RoshitaReport.rpt"));
            //var y = db.RoshitaDetails.Where(r => r.RoshitaID == Approval).AsEnumerable()
            // .Select(d => new
            // {
            //     MedicienName = d.MedicienName,
            //     TotalDuration = d.TotalDuration,
            //     Amount = d.Amount,
            //     Dose = d.Dose,
            //     Duration = d.Duration,
            //     TotalUnits = Convert.ToInt32(d.TotalUnits)
            // }).ToList();
            var y = db.RoshitaDetails.Where(r => r.RoshitaID == Approval && r.IsDealed == true)
            .Join(db.MedicineDatas, r => r.MedicienCode, m => m.M_CODE, (r, m) => new { r, m })
            .AsEnumerable()
         .Select(d => new RoshitaDetailsMedicineDataReportViewModel
         {
             MedicienName = d.r.MedicienName,//name
             LIC_TYPE = d.m.LIC_TYPE,//form
             UNIT_NO = d.m.UNIT_NO.Value,//size
             TotalUnits = Convert.ToInt32(d.r.TotalUnits),//count
             Amount = d.r.Amount,//
             PaymentGroup = d.r.PaymentGroup//type
         }).ToList();
            rd.SetDataSource(y);
            if (patient.EMP_ANAME == null)
            {
                rd.SetParameterValue("PatientName", "Unnamed");
            }
            else
            {
                rd.SetParameterValue("PatientName", patient.EMP_ANAME);
            }
            data.RoshetaType = "Pharmacy_Doctor";
            rd.SetParameterValue("CompType", patient.COMP_ID);
            rd.SetParameterValue("Type", data.RoshetaType);
            rd.SetParameterValue("Pharmacy", User.Identity.Name);
            rd.SetParameterValue("Approval", id);
            rd.SetParameterValue("PhoneNumber", "");
            rd.SetParameterValue("CompanyName", Company.C_ENAME);
            rd.SetParameterValue("CardId", data.CardId);
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
                return File(stream, "application/pfd", ApprovalDate.ToString("ddMMyyyy") + Approval.ToString() + ".pdf");
            }
            catch
            {
                throw;
            }
        }
        public JsonResult Approvals(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Roshita> approvals = new List<Roshita>();
            var date = DateTime.Now.AddDays(-14);
            approvals = db.Roshitas.Where(r => r.CardId == id && r.Manager == "Doctor_Daily" && r.CreatedDate >= date).OrderByDescending(x => x.Id).ToList();
            var serializer = new JavaScriptSerializer();

            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = serializer.Serialize(approvals),
                ContentType = "application/json"
            };
            return new JsonResult { Data = approvals, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult ApprovedMedicine(string id, long TxtSearch)
        {

            var Rosita = db.Roshitas.Where(r => r.CardId == id && r.Id == TxtSearch).FirstOrDefault();
            if (Rosita != null)
            {
                List<DoctorContainerViewModel> data = db.RoshitaDetails
                 .Join(db.MedicineDatas,
                       d => d.MedicienCode, m => m.M_CODE,
                       (d, m) => new { d, m })
                 .Where(l => l.d.RoshitaID == Rosita.Id)//&& l.d.IsDealed == false
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
                     UNIT_PRICE = l.m.UNIT_PRICE,
                     IsDealed = l.d.IsDealed
                 })
                 .ToList();
                return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                ViewBag.Message = "No Mediciens";
                List<DoctorContainerViewModel> RositaDetails = db.RoshitaDetails.Where(d => d.RoshitaID == 0)
                          .Select(d => new DoctorContainerViewModel
                          {
                              Id = d.Id,
                              MedicienCode = d.MedicienCode,
                              MedicienName = d.MedicienName,
                              Dose = d.Dose,
                              Duration = d.Duration,
                              TotalDuration = d.TotalDuration,
                              TotalUnits = d.TotalUnits,
                              Amount = d.Amount,
                          }).ToList();
                return new JsonResult { Data = RositaDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpGet]
        public JsonResult Calc(string id, long txt)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Rosita = db.Roshitas.Where(r => r.CardId == id && r.Id == txt).FirstOrDefault();

            return Json(Rosita, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]

        public JsonResult SavePrescription(PrescriptionViewModel data)
        {

            //var carduse = db.CardUseds.Where(c => c.CardId == data.CardId).FirstOrDefault();
            //if (carduse == null)
            //{
            //    return Json("Failed to Save Prescription");
            //}
            DateTime datenow = DateTime.Now.Date;
            var EmpId = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId && c.INS_START_DATE <= datenow  && c.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            var DoctorDailyRosita = db.Roshitas.Where(r => r.CardId == data.CardId && r.Id == data.Id).Select(l => new
            {
                l.CardId,
                l.Speciality,
                l.Diagnose1,
                l.RoshetaType,
                l.Limit,
                l.CompanyPercent
            }).FirstOrDefault();


            //Roshita
            Roshita roshita = new Roshita()
            {
                CardId = DoctorDailyRosita.CardId,
                RoshetaType = DoctorDailyRosita.RoshetaType,
                CompanyPercent = DoctorDailyRosita.CompanyPercent,
                Limit = DoctorDailyRosita.Limit,
                Speciality = DoctorDailyRosita.Speciality,
                Diagnose1 = DoctorDailyRosita.Diagnose1,
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
                Manager = "Pharmacy_Doctor",
                IsSync = null,
                SyncDate = null,
                SyncBy = null,
                CompHolderCode=EmpId.COMP_ID,
            };

            db.Roshitas.Add(roshita);
            //db.CardUseds.Remove(carduse);
            db.SaveChanges();
            // RoshitaDetails
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                RoshitaDetail DoctorDailyRoshitaDetails = db.RoshitaDetails.Where(r => r.Id == Medicien.Id).FirstOrDefault();

                Medicien.RoshitaID = roshita.Id;
                Medicien.IsDealed = true;
                Medicien.PaymentGroup = DoctorDailyRoshitaDetails.PaymentGroup;
                DoctorDailyRoshitaDetails.IsDealed = true;
                db.RoshitaDetails.Add(Medicien);
                db.Entry(DoctorDailyRoshitaDetails).State = EntityState.Modified;
            }
            //SaveDiagnoises
            List<PrescriptionRoshitaDignosi> MainDiagnoises = db.PrescriptionRoshitaDignosis.Where(x => x.RositaId == data.Id).ToList();
            foreach (PrescriptionRoshitaDignosi item in MainDiagnoises)
            {
                PrescriptionRoshitaDignosi dignosi = new PrescriptionRoshitaDignosi();
                dignosi.RositaId = roshita.Id;
                dignosi.DiagnoiseName = item.DiagnoiseName;
                db.PrescriptionRoshitaDignosis.Add(dignosi);
            }
            //SaveDealApproval
            if (data.hasApproval)
            {
                var accptionlist = db.Acceptions.Where(x => x.CompEmployeesId == EmpId.Id && x.AcceptionFlag == true).ToList();
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
                    var remaining = db.RemainConsumptions.Where(r => r.CARD_ID == roshita.CardId)
                        .OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (remaining != null)
                    {
                        remaining.REMAINING = remaining.REMAINING - roshita.CompanyPayment;
                        remaining.NET = remaining.NET + roshita.CompanyPayment;
                        db.Entry(remaining).State = EntityState.Modified;
                    }
                }
                int result = db.SaveChanges();
                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);

            }
            catch (DbEntityValidationException e)
            {
                db.Roshitas.Remove(roshita);
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
        //[Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]

        //public JsonResult Save(string id, long txt, float Totalvalue, float OverInsurance, float Cash, float PersonPayment, float CompanyPayment,string NationalId)
        //{
        //    // db.Configuration.ProxyCreationEnabled = false;
        //    var DoctorDailyRosita = db.Roshitas.Where(r => r.CardId == id && r.Id == txt).Select(l => new
        //    {
        //        l.CardId,
        //        l.Speciality,
        //        l.Diagnose1,
        //        l.RoshetaType,
        //        l.Limit,
        //        l.CompanyPercent


        //    }).FirstOrDefault();
        //    //Session["id"] = Rosita.Id;
        //    //Rosita.TotalValue = Math.Round(Totalvalue, 2);
        //    //Rosita.OverInsurance = Math.Round(OverInsurance,2);
        //    //Rosita.Cash = Math.Round(Cash,2);
        //    //Rosita.PersonPayment = Math.Round(PersonPayment,2);
        //    //Rosita.CompanyPayment = Math.Round(CompanyPayment,2);
        //    //Rosita.Cash = Convert.ToInt32(Cash);
        //    //Rosita.PersonPayment = Convert.ToInt32(PersonPayment);
        //    //Rosita.CompanyPayment = Convert.ToInt32(CompanyPayment);

        //    //List<RoshitaDetail> isDealed = db.RoshitaDetails.Where(r => r.RoshitaID == Rosita.Id && r.IsDealed == false).ToList();
        //    //if (Rosita.Manager == "Doctor_Daily" && isDealed.Count == 0)
        //    //{
        //    //    //update daily
        //    //    Rosita.Manager = "Daily";


        //    //}
        //    //if (Rosita.Manager == "Doctor_Monthly" && isDealed.Count == 0)
        //    //{
        //    //    //update Monthly
        //    //    Rosita.Manager = "Monthly";

        //    //}
        //    //Oracle_Id
        //    //string NeworacleId = "";
        //    //Roshita roshita = db.Roshitas.Where(x => x.Manager != "Doctor_Chronic" && x.Oracle_Id.Value.ToString().StartsWith("2") && (x.SyncBy == null || x.SyncBy == "Sql")).OrderByDescending(x => x.Id).FirstOrDefault();
        //    //if (DateTime.Now.Day == 1 && Convert.ToInt64(roshita.Oracle_Id.ToString().Substring(9)) != 1)
        //    //{
        //    //    NeworacleId = "2" + DateTime.Now.ToString("ddMMyyyy") + "1";
        //    //}
        //    //else
        //    //{
        //    //    long OracleId = roshita.Oracle_Id == null ? 0 : Convert.ToInt64(roshita.Oracle_Id.ToString().Substring(9))+1;
        //    //    NeworacleId = "2" + DateTime.Now.ToString("ddMMyyyy") +  OracleId;

        //    //}
        //    //Rosita.Oracle_Id = Convert.ToInt64(NeworacleId);
        //    //db.Entry(Rosita).State = EntityState.Modified;
        //    //db.SaveChanges();
        //    ////insert Rosita as Pharmacy Doctor
        //    //Rosita.Id = 0;
        //    Roshita Rosita = new Roshita();
        //    Rosita.CardId = DoctorDailyRosita.CardId;
        //    Rosita.Speciality = DoctorDailyRosita.Speciality;
        //    Rosita.Diagnose1 = DoctorDailyRosita.Diagnose1;
        //    Rosita.RoshetaType = DoctorDailyRosita.RoshetaType;
        //    Rosita.Limit = DoctorDailyRosita.Limit;
        //    Rosita.CompanyPercent = DoctorDailyRosita.CompanyPercent;
        //    Rosita.CreatedBy = User.Identity.Name;
        //    Rosita.CreatedDate = DateTime.Now;
        //    Rosita.TotalValue = Totalvalue;
        //    Rosita.OverInsurance = OverInsurance;
        //    Rosita.Cash = Convert.ToInt32(Cash);
        //    Rosita.PersonPayment = Convert.ToInt32(PersonPayment);
        //    Rosita.CompanyPayment = Convert.ToInt32(CompanyPayment);
        //    Rosita.Manager = "Pharmacy_Doctor";
        //    Rosita.Diagnose2 = NationalId;
        //    db.Roshitas.Add(Rosita);
        //    int result = db.SaveChanges();
        //    Session["id"] = Rosita.Id;
        //    List<PrescriptionRoshitaDignosi> MainDiagnoises = db.PrescriptionRoshitaDignosis.Where(x => x.RositaId == txt).ToList();
        //    foreach (PrescriptionRoshitaDignosi item in MainDiagnoises)
        //    {
        //        PrescriptionRoshitaDignosi dignosi = new PrescriptionRoshitaDignosi();
        //        dignosi.RositaId = Rosita.Id;
        //        dignosi.DiagnoiseName = item.DiagnoiseName;
        //        db.PrescriptionRoshitaDignosis.Add(dignosi);
        //    }
        //    db.SaveChanges();
        //    return Json("2" + Rosita.CreatedDate.Value.ToString("ddMMyy") + Rosita.Id, JsonRequestBehavior.AllowGet);
        //}
        ////SaveMediciens
        //[Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        //public JsonResult SaveMediciens(List<RoshitaDetail> Medciens)
        //{

        //    //insert RositaDetails 
        //    foreach (RoshitaDetail Medicien in Medciens)
        //    {
        //        RoshitaDetail roshitaDetail1 = new RoshitaDetail();
        //        RoshitaDetail roshitaDetail = db.RoshitaDetails.Where(r => r.Id == Medicien.Id).FirstOrDefault();
        //        roshitaDetail1.RoshitaID = Convert.ToInt64(Session["id"]);
        //        roshitaDetail1.MedicienCode = Medicien.MedicienCode;
        //        roshitaDetail1.MedicienName = Medicien.MedicienName;
        //        roshitaDetail1.TotalUnits = Medicien.TotalUnits;
        //        roshitaDetail1.PaymentGroup = roshitaDetail.PaymentGroup;
        //        roshitaDetail1.Dose = Medicien.Dose;
        //        roshitaDetail1.Duration = Medicien.Duration;
        //        roshitaDetail1.TotalDuration = Medicien.TotalDuration;
        //        roshitaDetail1.Amount = Medicien.Amount;
        //        roshitaDetail1.IsDealed = true;
        //        db.RoshitaDetails.Add(roshitaDetail1);
        //        roshitaDetail.IsDealed = true;
        //        db.Entry(roshitaDetail).State = EntityState.Modified;

        //    }

        //    //RoshitaDetail roshitaDetail = new RoshitaDetail();
        //    //foreach (RoshitaDetail Medicien in Medciens)
        //    //{
        //    //     roshitaDetail = db.RoshitaDetails.Where(r => r.Id == Medicien.Id).FirstOrDefault();
        //    //    roshitaDetail.IsDealed = true;
        //    //    db.Entry(roshitaDetail).State = EntityState.Modified;
        //    //}

        //    //    int result = db.SaveChanges();

        //    ////insert RositaDetails 
        //    //foreach (RoshitaDetail Medicien in Medciens)
        //    //{
        //    //    roshitaDetail.RoshitaID = Convert.ToInt64(Session["id"]);
        //    //    roshitaDetail.Dose = Medicien.Dose;
        //    //    roshitaDetail.Duration = Medicien.Duration;
        //    //    roshitaDetail.TotalDuration = Medicien.TotalDuration;
        //    //    roshitaDetail.Amount = Medicien.Amount;
        //    //    roshitaDetail.IsDealed = true;
        //    //    db.RoshitaDetails.Add(roshitaDetail);
        //    //}
        //    db.SaveChanges();



        //    return Json("saved");
        //}

        public JsonResult Alternatives(string id)
        {

            var group = db.MedicineDatas.Where(m => m.M_CODE == id).FirstOrDefault();
            List<DoctorContainerViewModel> Alternativies = db.MedicineDatas.Where(d => d.MED_GROUP == group.MED_GROUP)
               .Select(d => new DoctorContainerViewModel
               {

                   MedicienCode = d.M_CODE,
                   MedicienName = d.TRADE_NAME,
                   DOSAGE_FORM = d.DOSAGE_FORM,
                   PACK_PRICE = d.PACK_PRICE,
                   PACK_SIZE = d.PACK_SIZE,
                   UNIT_NO = d.UNIT_NO,
                   UNIT_PRICE = d.UNIT_PRICE,
               }).ToList();

            return Json(Alternativies, JsonRequestBehavior.AllowGet);
        }

    }
}
