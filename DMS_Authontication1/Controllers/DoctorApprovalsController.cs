using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_TEST;
using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DMS_Authontication1.Controllers
{
    public class DoctorApprovalsController : Controller
    {
        private DMS_TESTEntities db;

        public JsonRequestBehavior JsonRequestBehavior { get; private set; }

        public DoctorApprovalsController()
        {
            db = new DMS_TESTEntities();
        }
        // GET: DoctorApprovals
        #region Doctor Daily
        [Authorize(Roles = "Admin,Doctor")]
        public ActionResult Index()
        {
            ViewBag.ddlSpeciality = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ENAME");
            return View();
        }
        public ActionResult RoshitaReport(string id)
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
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "DoctorApprovalReport.rpt"));
            // rd.Subreports[0].SetDataSource(db.RoshitaDetails.Where(r=>r.RoshitaID==Approval).ToList());
            var y = db.RoshitaDetails.Where(r => r.RoshitaID == Approval).AsEnumerable()
             .Select(d => new
             {
                 MedicienName = d.MedicienName,
                 TotalDuration = d.TotalDuration,
                 Amount = d.Amount,
                 Dose = d.Dose,
                 Duration = d.Duration,
                 TotalUnits = Convert.ToInt32(d.TotalUnits)
             }).ToList();
            rd.SetDataSource(y);
            rd.SetParameterValue("PatientName", patient.EMP_ANAME);
            data.RoshetaType = "Doctor_Daily";
            rd.SetParameterValue("Type", data.RoshetaType);
            rd.SetParameterValue("Pharmacy", User.Identity.Name);
            rd.SetParameterValue("Approval", Convert.ToDateTime(data.CreatedDate).ToString("ddMMyyyy") + Approval.ToString());
            //rd.SetParameterValue("PhoneNumber", data.PhoneNumber);
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
                rd.Close();
                rd.Dispose();
                GC.Collect();
                stream.Seek(0, SeekOrigin.Begin);
                DateTime ApprovalDate = Convert.ToDateTime(data.CreatedDate);
                return File(stream, "application/pfd", ApprovalDate.ToString("ddMMyyyy") + Approval.ToString() + ".pdf");
            }
            catch
            {
                throw;
            }
        }

        public JsonResult History(string id)
        {
            var emp = db.Comp_Employees.Where(c => c.CARD_ID == id).FirstOrDefault();
            List<Roshita> Rosita = db.Roshitas.Where(r => r.CardId == id).ToList();

            List<DoctorContainerViewModel> newlist = new List<DoctorContainerViewModel>();
            foreach (var item in Rosita)
            {
                List<DoctorContainerViewModel> roshitaDetails = db.RoshitaDetails
                 .Join(db.MedicineDatas, d => d.MedicienCode, m => m.M_CODE, (d, m) => new { d, m })
                 .Where(l => l.d.RoshitaID == item.Id)
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
                     CreatedBy = l.d.Roshita.CreatedBy,
                     CreatedDate = l.d.Roshita.CreatedDate,
                     DOSAGE_FORM = l.m.DOSAGE_FORM,
                     UNIT_NO = l.m.UNIT_NO,
                     PACK_PRICE = l.m.PACK_PRICE,
                     PACK_SIZE = l.m.PACK_SIZE,
                     UNIT_PRICE = l.m.UNIT_PRICE,
                     M_TYPE = l.m.M_TYPE

                 })
                 .ToList();
                newlist.AddRange(roshitaDetails);
            }
            var serializer = new JavaScriptSerializer();

            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = serializer.Serialize(newlist),
                ContentType = "application/json"
            };
            return new JsonResult { Data = newlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult FromHistory(string id, string From, string To)
        {
            DateTime F = Convert.ToDateTime(From);
            DateTime T = Convert.ToDateTime(To);
            List<Roshita> Rosita = db.Roshitas.Where(r => r.CardId == id).ToList();

            List<DoctorContainerViewModel> newlist = new List<DoctorContainerViewModel>();
            foreach (var item in Rosita)
            {
                if (item.CreatedDate >= F && item.CreatedDate <= T)
                {
                    List<DoctorContainerViewModel> roshitaDetails = db.RoshitaDetails
                     .Join(db.MedicineDatas, d => d.MedicienCode, m => m.M_CODE, (d, m) => new { d, m })
                     .Where(l => l.d.RoshitaID == item.Id)
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
                         CreatedBy = l.d.Roshita.CreatedBy,
                         CreatedDate = l.d.Roshita.CreatedDate,
                         DOSAGE_FORM = l.m.DOSAGE_FORM,
                         UNIT_NO = l.m.UNIT_NO,
                         PACK_PRICE = l.m.PACK_PRICE,
                         PACK_SIZE = l.m.PACK_SIZE,
                         UNIT_PRICE = l.m.UNIT_PRICE,
                         M_TYPE = l.m.M_TYPE

                     })
                     .ToList();
                    newlist.AddRange(roshitaDetails);
                }
            }
            var serializer = new JavaScriptSerializer();

            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = serializer.Serialize(newlist),
                ContentType = "application/json"
            };
            return new JsonResult { Data = newlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Pharmacies()
        {
            List<Pr_Bra> Pharmacies = new List<Pr_Bra>();
            Pharmacies = db.Pr_Bra.Where(x => x.PRV_TYPE == 2).GroupBy(x => x.PR_CODE).Select(x => x.FirstOrDefault()).ToList();
            return new JsonResult { Data = Pharmacies, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult Branches(string id)
        {
            try
            {
                var Branchs = db.Pr_Bra.Where(x => x.PR_ANAME == id).ToList();
                return new JsonResult { Data = Branchs, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        [Authorize(Roles = "Admin,Doctor")]

        public JsonResult SavePrescription(PrescriptionViewModel data)
        {
            //Roshita
            Roshita roshita = new Roshita()
            {
                CardId = data.CardId,
                RoshetaType = data.RoshetaType,
                CompanyPercent = data.CompanyPercent,
                Limit = data.Limit,
                Speciality = data.Speciality,
                Diagnose1 = data.Diagnose1,
                //Diagnose2 = data.Diagnose2,//Pharmacytxt
                TotalValue = Math.Round(data.TotalValue.Value, 2),
                PersonPayment = Math.Round(data.PersonPayment, 2),
                CompanyPayment = Math.Round(data.CompanyPayment, 2),
                OverInsurance = Math.Round(data.OverInsurance.Value, 2),
                Cash = Math.Round(data.Cash.Value, 2),
                //PhoneNumber = data.PhoneNumber,//branchtxt
               // ClaimNumber = data.ClaimNumber,
                CreatedBy = data.CreatedBy == null ? User.Identity.Name : data.CreatedBy,
                CreatedDate = DateTime.Now,
                Manager = "Doctor_Daily",
                IsSync = null,
                SyncDate = null,
                SyncBy = null
            };

            db.Roshitas.Add(roshita);
            db.SaveChanges();
            // RoshitaDetails
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                Medicien.RoshitaID = roshita.Id;
                Medicien.IsDealed = false;
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
            //RoshitaPharmcyApproved
            if (data.Diagnose2!=null) { 
            RoshitaPharmcyApproved roshitaPharmcyApproved = new RoshitaPharmcyApproved();
            roshitaPharmcyApproved.Pharmacy = data.Diagnose2;
            roshitaPharmcyApproved.Branch = data.PhoneNumber;
            roshitaPharmcyApproved.RoshitaId =roshita.Id;
            db.RoshitaPharmcyApproveds.Add(roshitaPharmcyApproved);
            }
            try
            {
                int result = db.SaveChanges();
                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);

            }
            catch (DbEntityValidationException e)
            {
                db.Roshitas.Remove(roshita);
                return Json("Failed to Save Prescription");
            }

        }

        //[Authorize(Roles = "Admin,Doctor")]

        //public JsonResult Save(Roshita data)
        //{
        //    //roshitaPharmcyApproved.Pharmacy
        //    data.CreatedBy = User.Identity.Name;
        //    data.CreatedDate = DateTime.Now;

        //    if (data.RoshetaType == "11601")
        //    {
        //        data.Manager = "Doctor_Daily";
        //    }
        //    else if (data.RoshetaType == "11603")
        //    {
        //        data.Manager = "Doctor_Monthly";
        //    }
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
        //    //    NeworacleId = "2" + DateTime.Now.ToString("ddMMyyyy") + OracleId;

        //    //}
        //    //data.Oracle_Id = Convert.ToInt64(NeworacleId);
        //    db.Roshitas.Add(data);
        //    int result = db.SaveChanges();
        //    Session["id"] = data.Id;
        //    return Json("2" + data.CreatedDate.Value.ToString("ddMMyy") + data.Id);
        //}
        //public JsonResult SavePharmacyApproval(string Pharmacy, string Branch)
        //{
        //    RoshitaPharmcyApproved roshitaPharmcyApproved = new RoshitaPharmcyApproved();
        //    roshitaPharmcyApproved.Pharmacy = Pharmacy;
        //    roshitaPharmcyApproved.Branch = Branch;
        //    roshitaPharmcyApproved.RoshitaId = Convert.ToInt64(Session["id"]);
        //    db.RoshitaPharmcyApproveds.Add(roshitaPharmcyApproved);
        //    int result = db.SaveChanges();
        //    return Json("Done");
        //}
        //public JsonResult SaveMediciens(List<RoshitaDetail> Medciens)
        //{
        //    foreach (RoshitaDetail Medicien in Medciens)
        //    {
        //        Medicien.RoshitaID = Convert.ToInt64(Session["id"]);
        //        Medicien.IsDealed = false;
        //        db.RoshitaDetails.Add(Medicien);
        //    }
        //    try
        //    {
        //        int result = db.SaveChanges();
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
        //                    ve.PropertyName, ve.ErrorMessage);
        //            }
        //        }
        //        throw;
        //    }
        //    return Json("savd");
        //}
        //public JsonResult SaveDiagnoises(List<Diagnose> Diagnoises)
        //{
        //    foreach (Diagnose item in Diagnoises)
        //    {
        //        PrescriptionRoshitaDignosi dignosi = new PrescriptionRoshitaDignosi();
        //        dignosi.RositaId = Convert.ToInt64(Session["id"]);
        //        dignosi.DiagnoiseName = item.DIAG_ANAME;
        //        db.PrescriptionRoshitaDignosis.Add(dignosi);
        //    }
        //    db.SaveChanges();
        //    return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        #endregion
        //Monthly and chronic Approvals
        #region Doctor Chronic
        [Authorize(Roles = "Admin,Doctor")]
        public ActionResult Monthly()
        {
            return View();
        }
        public JsonResult getCompGrop(int id)
        {
            var CompGrop = db.S_Ent_7.Where(x => x.C_COMP_ID == id)
             .Select(l => new
             {
                 Code = l.S_ID,
                 Name = l.S_NAME

             }).ToList();
            return new JsonResult { Data = CompGrop, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult getProviders()
        {
            var pro = db.Serv_Providers1.Where(x => x.PRV_TYPE == 2).Select(l => new
            {
                l.PR_CODE,
                l.PR_ANAME,
                l.PR_ENAME

            }).ToList();
            return new JsonResult { Data = pro, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #region Companies
        public JsonResult MonthlyCardsApprovals(int id)
        {
            try
            {

                var Cards = db.Med_Card.Where(x => x.C_COMP_ID == id)
                    .Join(db.Comp_Employees, m => m.CARD_NO, e => e.CARD_ID, (m, e) => new { m, e })
                    .Join(db.Serv_Providers1.Where(a => a.PRV_TYPE == 2), x => x.m.PROVIDER_CODE, z => z.PR_CODE, (x, z) => new { x, z })
                    .Select(l => new MonthlyChronicDoctorApprovalViewModel
                    {
                        CARD_NO = l.x.m.CARD_NO,
                        EMP_ANAME = l.x.e.EMP_ANAME,
                        PR_ENAME = l.z.PR_ENAME,
                        PR_ANAME = l.z.PR_ANAME,
                        MONTH_START_DATE = l.x.m.MONTH_START_DATE,
                        GROUP_NAME = l.x.m.GROUP_NAME,
                        LOOK_01 = l.x.m.LOOK_01,
                        NO_PAY = l.x.m.NO_PAY,
                        NO_OVER = l.x.m.NO_OVER,
                        TASHKHES_01 = l.x.m.TASHKHES_01,
                        ST_DAY = l.x.m.ST_DAY,
                        CONTRACT_NO = l.x.e.CONTRACT_NO

                    })
                     .GroupBy(x => new { x.CARD_NO })
                    .Select(x => x.OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault())
                    .ToList();

                return new JsonResult { Data = Cards, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult ChangeGroup(List<ChronicViewModel> Ids)
        {

            foreach (var item in Ids)
            {
                Med_Card Change = db.Med_Card.Where(x => x.CARD_NO == item.CARD_NO).FirstOrDefault();
                Change.GROUP_NAME = item.MED_NAME;//MedName contains group name
                db.Entry(Change).State = EntityState.Modified;
            }

            int result = db.SaveChanges();

            return Json(result);
        }
        public JsonResult ChangeProvider(List<ChronicViewModel> Ids)
        {

            foreach (var item in Ids)
            {
                Med_Card Change = db.Med_Card.Where(x => x.CARD_NO == item.CARD_NO).FirstOrDefault();
                Change.PROVIDER_CODE_OLD = Change.PROVIDER_CODE;
                Change.PROVIDER_CODE = item.DOSE;//Dose contains provider code
                db.Entry(Change).State = EntityState.Modified;
            }

            int result = db.SaveChanges();

            return Json(result);
        }
        public JsonResult PaymentStatus(int CompId, int state)
        {
            List<Med_Card> all = db.Med_Card.Where(x => x.C_COMP_ID == CompId).ToList();
            foreach (var item in all)
            {
                item.NO_PAY = state;
                db.Entry(item).State = EntityState.Modified;
            }

            int result = db.SaveChanges();

            return Json(result);
        }
        public JsonResult OverStatus(int CompId, int state)
        {
            List<Med_Card> all = db.Med_Card.Where(x => x.C_COMP_ID == CompId).ToList();
            foreach (var item in all)
            {
                item.NO_OVER = state;
                db.Entry(item).State = EntityState.Modified;
            }

            int result = db.SaveChanges();

            return Json(result);
        }
        public JsonResult LockStatus(int CompId, string state)
        {
            int s = 1;
            if (state == "Open")
            {
                s = 0;
            }
            else if (state == "Pause")
            {
                s = 2;
            }
            List<Med_Card> all = db.Med_Card.Where(x => x.C_COMP_ID == CompId).ToList();
            foreach (var item in all)
            {
                item.LOOK_01 = s;
                db.Entry(item).State = EntityState.Modified;
            }

            int result = db.SaveChanges();

            return Json(result);
        }
        #endregion
        #region Individuals
        public JsonResult getDiag()
        {
            var Diagnoises = db.Diagnosis.Select(l => new
            {
                Code = l.Id,
                Name = l.DIAG_ANAME

            }).ToList();
            return new JsonResult { Data = Diagnoises, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #region Cards
        public JsonResult CheckCard(string id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                //Retrive
                var mED_CARD = db.Med_Card.Where(x => x.CARD_NO == id)
                     .Join(db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now), m => m.CARD_NO, e => e.CARD_ID, (m, e) => new { m, e })
                    .Join(db.Serv_Providers1.Where(a => a.PRV_TYPE == 2), x => x.m.PROVIDER_CODE, z => z.PR_CODE, (x, z) => new { x, z })
                    .Select(l => new MonthlyChronicDoctorApprovalViewModel
                    {
                        CARD_NO = l.x.m.CARD_NO,
                        EMP_ANAME = l.x.e.EMP_ANAME,
                        PR_ANAME = l.z.PR_ANAME,
                        PR_ENAME = l.z.PR_ENAME,
                        MONTH_START_DATE = l.x.m.MONTH_START_DATE,
                        GROUP_NAME = l.x.m.GROUP_NAME,
                        LOOK_01 = l.x.m.LOOK_01,
                        NO_PAY = l.x.m.NO_PAY,
                        NO_OVER = l.x.m.NO_OVER,
                        TASHKHES_01 = l.x.m.TASHKHES_01,
                        ST_DAY = l.x.m.ST_DAY,
                        NOTES=l.x.m.NOTES,

                    })
                    .FirstOrDefault();
                var Employee = db.Comp_Employees.Where(x => x.CARD_ID == "0").FirstOrDefault();
                if (mED_CARD == null)
                {
                    //add
                    Employee = db.Comp_Employees.Where(x => x.CARD_ID == id).FirstOrDefault();
                }
                var result = new { mED_CARD = mED_CARD, Employee = Employee };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        [Authorize(Roles = "Admin,Doctor")]
        public JsonResult InsertCard(MonthlyChronicDoctorApprovalViewModel data)
        {
            Med_Card mED_CARD = new Med_Card();
            mED_CARD.CARD_NO = data.CARD_NO;
            mED_CARD.NOTES = data.NOTES;
            mED_CARD.CREATED_BY = User.Identity.Name;
            mED_CARD.CREATED_DATE = DateTime.Now;
            var emp = db.Comp_Employees.Where(x => x.CARD_ID == data.CARD_NO && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            mED_CARD.C_COMP_ID = emp.C_COMP_ID;
            mED_CARD.GROUP_ID = data.GROUP_ID;
            var Group = db.S_Ent_7.Where(x => x.C_COMP_ID == emp.C_COMP_ID && x.S_ID == data.GROUP_ID).FirstOrDefault();
            mED_CARD.GROUP_NAME = Group.S_NAME;
            mED_CARD.LOOK_01 = data.LOOK_01;
            mED_CARD.MONTH_START_DATE = data.MONTH_START_DATE;
            mED_CARD.MONTH_END_DATE = emp.INS_END_DATE;
            mED_CARD.NO_OVER = data.NO_OVER;
            mED_CARD.NO_PAY = data.NO_PAY;
            var prov = db.Serv_Providers1.Where(d => d.PR_ANAME == data.PR_ANAME).FirstOrDefault();
            mED_CARD.PROVIDER_CODE = prov.PR_CODE;
            mED_CARD.PROVIDER_CODE_OLD = 1268;
            mED_CARD.ST_DAY = data.ST_DAY;
            mED_CARD.TASHKHES_01 = data.TASHKHES_01;
            db.Med_Card.Add(mED_CARD);
            db.Roshitas.Add(new Roshita
            {
                CardId = data.CARD_NO,
                Speciality = "Empty",
                Diagnose1 = data.TASHKHES_01,
                RoshetaType = "11602",
                Limit = 0,
                CompanyPayment = 0,
                OverInsurance = 0,
                TotalValue = 0,
                CompanyPercent = 0,
                PersonPayment = 0,
                Cash = 0,
                Manager = "Doctor_Chronic",
                CreatedBy = User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsSync = false,
                //SyncBy = "Admin",
                //SyncDate = DateTime.Now,
                Diagnose2 = "Empty",
                diagnose3 = "Empty",
                PhoneNumber = "Empty",
                Oracle_Id = 0

            });
            int result = db.SaveChanges();

            return Json(result);
        }
        [Authorize(Roles = "Admin,Doctor")]
        public JsonResult UpdateCard(MonthlyChronicDoctorApprovalViewModel data)
        {
            #region Authenticate
            string car = data.CARD_NO;
            var medUpdate = db.Med_Card.Where(c => c.CARD_NO == car).FirstOrDefault();
            if (medUpdate != null)
            {
                if (medUpdate.UPDATE_BY == "GodaKotb" && User.Identity.Name != "GodaKotb")
                {
                    return Json("False");
                }
            }
            #endregion
            Med_Card mED_CARD = new Med_Card();
            mED_CARD = db.Med_Card.Where(x => x.CARD_NO == data.CARD_NO).FirstOrDefault();
            mED_CARD.UPDATE_BY = User.Identity.Name;
            mED_CARD.UPDATE_DATE = DateTime.Now;
            mED_CARD.NOTES = data.NOTES;
            mED_CARD.GROUP_ID = data.GROUP_ID;
            var Group = db.S_Ent_7.Where(x => x.S_ID == data.GROUP_ID).FirstOrDefault();
            mED_CARD.GROUP_NAME = Group.S_NAME;
            mED_CARD.LOOK_01 = data.LOOK_01;
            mED_CARD.MONTH_START_DATE = data.MONTH_START_DATE;
            // mED_CARD.MONTH_END_DATE
            mED_CARD.NO_OVER = data.NO_OVER;
            mED_CARD.NO_PAY = data.NO_PAY;
            var prov = db.Serv_Providers1.Where(d => d.PR_ANAME == data.PR_ANAME).FirstOrDefault();
            mED_CARD.PROVIDER_CODE = prov.PR_CODE;
            mED_CARD.ST_DAY = data.ST_DAY;
            mED_CARD.TASHKHES_01 = data.TASHKHES_01;
            mED_CARD.SyncBy = "Updated";
            if (ModelState.IsValid)
            {
                db.Entry(mED_CARD).State = EntityState.Modified;
            }
            int result = db.SaveChanges();

            return Json(result);
        }
        public JsonResult LastApproval(string id)
        {
            //var date = db.Roshitas.Where(x => x.CardId == id).Where(x => x.RoshetaType == "Monthly" || x.RoshetaType == "Pharmacy_Chronic").OrderByDescending(t => t.CreatedDate).FirstOrDefault();
            var emp = db.Roshitas.Where(x => x.CardId == id)
                .Where(x => x.RoshetaType == "Monthly" || x.RoshetaType == "Pharmacy_Chronic")
                .Join(db.RoshitaDetails, r => r.Id, d => d.RoshitaID, (r, d) => new { r, d })
                .Join(db.Comp_Employees, c => c.r.CardId, v => v.CARD_ID, (c, v) => new { c, v })
                .Join(db.Co_Insurance_01, q => q.v.C_COMP_ID, w => w.CO_ID, (q, w) => new { q, w })
                .Join(db.MedicineDatas, m => m.q.c.d.MedicienCode, n => n.M_CODE, (m, n) => new { m, n })
                .Where(item => item.m.w.LIVEL == item.m.q.v.CLASS_CODE)
                .Select(l => new Roshita_RoshitaDetails
                {
                    Id = l.m.q.c.r.Id,  //approvalId
                    CreatedBy = l.m.q.c.r.CreatedBy,//provider
                    CreatedDate = l.m.q.c.r.CreatedDate,
                    CompanyPercent = l.m.q.c.r.CompanyPercent,
                    //comp_emp
                    CardId = l.m.q.c.r.CardId,
                    EMP_ANAME = l.m.q.v.EMP_ANAME,
                    INS_END_DATE = l.m.q.v.INS_END_DATE,
                    C_COMP_ID = l.m.q.v.C_COMP_ID,
                    //Co_insurance
                    INSURANCE_DAY = l.m.w.INSURANCE_DAY,
                    INSURANCE_MONTH = l.m.w.INSURANCE_MONTH,
                    //medicien
                    MedicienCode = l.n.M_CODE,
                    MedicienName = l.n.TRADE_NAME,
                    PACK_SIZE = l.n.PACK_SIZE,
                    PACK_PRICE = l.n.PACK_PRICE,
                    UNIT_NO = l.n.UNIT_NO,
                    UNIT_PRICE = l.n.UNIT_PRICE,
                    DOSAGE_FORM = l.n.DOSAGE_FORM,
                    TotalUnits = l.m.q.c.d.TotalDuration,
                    Amount = l.m.q.c.d.Amount,
                    RoshetaType = l.m.q.c.r.RoshetaType,
                    Dose = l.m.q.c.d.Dose,
                    Duration = l.m.q.c.d.Duration
                }).ToList();

            return new JsonResult { Data = emp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Medicines
        public JsonResult MonthlyMedicines(string id)
        {
            var Mediciens = db.Med_Medicine.Where(x => x.CARD_NO == id).ToList();
            return new JsonResult { Data = Mediciens, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [Authorize(Roles = "Admin,Doctor")]
        public JsonResult SubmetChronic(List<Med_Medicine> data)
        {
            #region Ahtenticate
            string car = data[0].CARD_NO;
            var medUpdate = db.Med_Card.Where(c => c.CARD_NO == car).FirstOrDefault();
            if (medUpdate != null)
            {
                if(medUpdate.UPDATE_BY== "GodaKotb"&& User.Identity.Name!= "GodaKotb")
                {
                    return Json("False");
                }
            }
            #endregion

            List<Med_Medicine> current = new List<Med_Medicine>();

            Med_Medicine first = data.First();
            current = db.Med_Medicine.Where(c => c.CARD_NO == first.CARD_NO).ToList();
            //db.Med_Medicine.RemoveRange(current);
            foreach (Med_Medicine item in data)
            {
                bool flag = true;
                foreach (var Cur in current)
                {
                    if (Cur.MED_CODE == item.MED_CODE)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag == true)
                {
                    //add new item
                    item.RDATE = DateTime.Now;
                    item.CREATED_BY = User.Identity.Name;
                    item.CREATED_DATE = DateTime.Now;
                    db.Med_Medicine.Add(item);
                }
                else
                {
                    //update 
                    Med_Medicine update = current.Where(x => x.MED_CODE == item.MED_CODE).FirstOrDefault();

                    update.UPDATE_BY = User.Identity.Name;
                    update.UPDATE_DATE = DateTime.Now;
                    update.MED_TYP = item.MED_TYP;
                    update.DOSE = Convert.ToInt32(item.DOSE);
                    update.NO_OF_UINT = Convert.ToInt32(item.NO_OF_UINT);
                    update.TOTAL_AMT = item.TOTAL_AMT;
                    update.MED_DURATION = Convert.ToInt32(item.MED_DURATION);
                    update.DOS_DUR = Convert.ToInt32(item.DOS_DUR);
                    update.EXCESS = item.EXCESS;
                    update.UNIT_NO = Convert.ToInt32(item.UNIT_NO);
                    update.UNIT_PRICE = item.UNIT_PRICE;
                    update.ACTIVE = item.ACTIVE;
                    update.LFT_MONTH = item.LFT_MONTH;
                    update.MONTH_DATE_STOP = item.MONTH_DATE_STOP;
                    update.SyncBy = "Updated";
                    db.Entry(update).State = EntityState.Modified;
                }
                  
            }
            //db.SaveChanges();
            //rositaDetails
            var Rosita = db.Roshitas.Where(r => r.CardId == first.CARD_NO && r.Manager == "Doctor_Chronic").OrderByDescending(c => c.CreatedDate).FirstOrDefault();
            var rositaDetails = db.RoshitaDetails.Where(x => x.RoshitaID == Rosita.Id).ToList();
            foreach (Med_Medicine item in data)
            {
                //insert or Update
                bool flag = true;
                foreach (var Cur in rositaDetails)
                {
                    if (Cur.MedicienCode == item.MED_CODE)
                    {
                        flag = false;
                    }
                }
                DateTime MONTH_DATE_STOP21Time = new DateTime();
                if (item.MONTH_DATE_STOP != null)
                {
                    string MONTH_DATE_STOP21 = "21/" + (item.MONTH_DATE_STOP.Value.ToString("MM/yyyy")).ToString();
                    MONTH_DATE_STOP21Time = DateTime.ParseExact(MONTH_DATE_STOP21, "dd/MM/yyyy", null);
                }
                if (flag == true)
                {
                    //add to roshita active only
                    RoshitaDetail roshitaDetail = new RoshitaDetail();
                    roshitaDetail.MedicienCode = item.MED_CODE;
                    roshitaDetail.MedicienName = item.MED_NAME;
                    roshitaDetail.Dose = Convert.ToInt32(item.DOSE);
                    roshitaDetail.Duration = Convert.ToInt32(item.MED_DURATION);
                    roshitaDetail.TotalDuration = 28;
                    //roshitaDetail.Duration = Convert.ToInt32(item.MED_DURATION);
                    roshitaDetail.TotalUnits = Convert.ToInt32(item.NO_OF_UINT);
                    roshitaDetail.IsDealed = item.ACTIVE == "Y" && (item.MONTH_DATE_STOP == null || MONTH_DATE_STOP21Time > DateTime.Now) ? false : true;
                    roshitaDetail.Amount = Convert.ToInt32(item.TOTAL_AMT);
                    roshitaDetail.RoshitaID = Rosita.Id;
                    roshitaDetail.PaymentGroup = "Yes";
                    roshitaDetail.IsSync = false;
                    db.RoshitaDetails.Add(roshitaDetail);
                }
                else
                {

                    //update active only
                    RoshitaDetail update = rositaDetails.Where(x => x.MedicienCode == item.MED_CODE).FirstOrDefault();
                    if (item.ACTIVE == "Y" && (item.MONTH_DATE_STOP == null || MONTH_DATE_STOP21Time > DateTime.Now))
                    {
                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
                        if (db.Roshitas.Where(x => x.Manager == "pharmacy_chronic" && x.CardId == first.CARD_NO && x.CreatedDate >= Last21Time).Join(db.RoshitaDetails, r => r.Id, rd => rd.RoshitaID, (r, rd) => new { r, rd }).Where(x => x.rd.MedicienCode == item.MED_CODE).Count() == 0)
                            update.IsDealed = false;
                    }
                    update.Dose = Convert.ToInt32(item.DOSE);
                    update.Duration = Convert.ToInt32(item.MED_DURATION);
                    update.TotalDuration = 28;
                    update.Duration = Convert.ToInt32(item.MED_DURATION);
                    update.TotalUnits = Convert.ToInt32(item.NO_OF_UINT);
                    update.Amount = Convert.ToInt32(item.TOTAL_AMT);
                    update.IsDealed = item.ACTIVE == "N" ? true : update.IsDealed;
                    update.SyncBy = "Updated";
                    db.Entry(update).State = EntityState.Modified;
                }


            }
            db.SaveChanges();
            //remove remain
            return Json("Done");
        }

        #endregion
        #endregion

        public JsonResult UpdateProvider(int CompId, int ProviderName, int OldProviderName)
        {
            List<Med_Card> all = db.Med_Card.Where(x => x.C_COMP_ID == CompId && x.PROVIDER_CODE == OldProviderName).ToList();
            foreach (var item in all)
            {
                item.PROVIDER_CODE = ProviderName;
                db.Entry(item).State = EntityState.Modified;
            }

            int result = db.SaveChanges();

            return Json(result);
        }
        public JsonResult UpdateGroup(int CompId, string GroupName, string OldGroupName)
        {
            List<Med_Card> all = db.Med_Card.Where(x => x.C_COMP_ID == CompId && x.GROUP_NAME == OldGroupName).ToList();
            foreach (var item in all)
            {
                item.GROUP_NAME = GroupName;
                db.Entry(item).State = EntityState.Modified;
            }

            int result = db.SaveChanges();

            return Json(result);
        }
        //public JsonResult CompanyGroupList()
        //{
        //    List<CONTRACT_COMP> coms = new List<CONTRACT_COMP>();
        //    coms = db.CONTRACT_COMP.ToList();
        //    return new JsonResult { Data = coms, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        #endregion
    }
}