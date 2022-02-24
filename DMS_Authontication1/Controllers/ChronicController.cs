using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DMS_TEST.Controllers
{
    public class ChronicController : Controller
    {
        DMS_TESTEntities db = new DMS_TESTEntities();
        // GET: Chronic
        //[Authorize(Roles = "Admin,Pharmacy")]
        public ActionResult Chronic(string id, string NationalId)
        {
            List<ChronicViewModel> data = new List<ChronicViewModel>();
            //if (DateTime.Now.Day>=16&& DateTime.Now.Day <= 31)
            //{
            //    ViewBag.Message = "ValidationDate";
            //    return View(data);
            //}
            int CompId = Convert.ToInt32(id.Split('-')[0].ToString());

            Contract_Comp contractComp = db.Contract_Comp.Where(x => x.C_COMP_ID == CompId).FirstOrDefault();
            string emp = "";
            if (contractComp != null)
            {
                emp = contractComp.ACTIVE;
            }
            else
            {
                ViewBag.Message = "Company is not existed";
                return View(data);
            }
            var empCardTerminationFlag = db.Comp_Employees.Where(x => x.CARD_ID == id && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            if (empCardTerminationFlag != null)
            {
                if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "N")
                {
                    ViewBag.Message = "ok";
                }
                else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE > DateTime.Now)
                {
                    ViewBag.Message = "ok";
                }
                else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE < DateTime.Now)
                {
                    ViewBag.Message = "Expired Card";
                    return View(data);
                }
            }
            else
            {
                var CompTerminationFlag = db.Contract_Data.Where(x => x.C_COMP_ID == CompId && x.DATE_FROM <= DateTime.Now && x.DATE_TO >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                if (CompTerminationFlag != null)
                {
                    ViewBag.Message = "Card is not existed";
                    return View(data);
                }
            }
            if (ViewBag.Message != "ok")
            {
                ViewBag.Message = "Expired Company";
                return View(data);
            }
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            long userProvider = Convert.ToInt64(CurrentUser.Provider);
            var Provider = db.Serv_Providers1.Where(x => x.PR_CODE == userProvider).FirstOrDefault();

            Med_Card medCard = db.Med_Card.Where(m => m.CARD_NO == id && m.LOOK_01 == 0).FirstOrDefault();
            if (medCard != null)
            {
                var Rosita = db.Roshitas.Where(r => r.CardId == medCard.CARD_NO && r.Manager == "Doctor_Chronic").Where(x => x.RoshetaType == "11603" || x.RoshetaType == "11602").OrderByDescending(c => c.CreatedDate).FirstOrDefault();
                if (Rosita != null)
                {
                    if (medCard.PROVIDER_CODE == 1268 || medCard.PROVIDER_CODE == Provider.PR_CODE)
                    {

                        data = db.RoshitaDetails.Where(x => x.RoshitaID == Rosita.Id && x.IsDealed == false && x.TotalUnits != 0)
                        .Join(db.Med_Medicine, d => d.MedicienCode, m => m.MED_CODE, (d, m) => new { d, m })
                        //.Join(db.MedicineDatas, d => d.MedicienCode, m => m.M_CODE, (d, m) => new { d, m })
                        .Where(l => l.m.CARD_NO == id)
                        .Select(l => new ChronicViewModel
                        {
                            Id = l.d.Id,
                            MED_CODE = l.d.MedicienCode,
                            MED_NAME = l.d.MedicienName,
                            DOSE = l.d.Dose,
                            MED_DURATION = l.d.Duration,
                            NO_OF_UINT = l.d.TotalUnits,
                            TOTAL_AMT = l.d.Amount,
                            DOSAGE_FORM = l.m.DOSAGE_FORM,
                            UNIT_NO = l.m.UNIT_NO,
                            Des_PACK_PRICE = l.m.PACK_PRICE,
                            PACK_SIZE = l.m.PACK_SIZE,
                            UNIT_PRICE = l.m.UNIT_PRICE,
                            MedicineNoPay = l.m.MedicineNoPay.Trim()
                        }).Distinct().ToList();
                        if (data.Count == 0)
                            ViewBag.Message = "No Mediciens";
                        return View(data);
                    }
                    else
                    {
                        ViewBag.Message = "Can despense your medicine at your specific pharmacy";
                        return View(data);

                    }

                }
                else if (Rosita == null)
                {
                    //No Rosita
                    ViewBag.Message = "No Chronic Roshita";
                    return View(data);
                }
            }
            else
            {
                //Not Active
                ViewBag.Message = "This Card doesn't have chronic medicines";
                return View(data);
            }
            return View(data);
        }
        public JsonResult Validation(string id, string CardId)
        {
            bool Validation = false;
            Comp_Employees Card = db.Comp_Employees.Where(x => x.CARD_ID == CardId && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            if (Card.EMP_ID == id || Card.TEL1 == id || Card.TEL2 == id)
            {
                Validation = true;
            }
            return Json(new { Validation = Validation });
        }
        public JsonResult GetChronicMedData(string id)
        {
            try
            {
                Med_Card medCard = db.Med_Card.Where(m => m.CARD_NO == id && m.LOOK_01 == 0).FirstOrDefault();
                return Json(new { ok = true, medCard = medCard, message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Alternatives(string id, string Price)
        {
            int MedicinePrice = Convert.ToInt32(Price);
            var group = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Where(m => m.M_CODE == id).FirstOrDefault();
            List<ChronicViewModel> Alternativies = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Where(d => d.MED_GROUP == group.MED_GROUP && d.PACK_PRICE >= (MedicinePrice - 10) && d.PACK_PRICE <= (MedicinePrice + 10))
               .Select(d => new ChronicViewModel
               {

                   MED_CODE = d.M_CODE,
                   MED_NAME = d.TRADE_NAME,
                   DOSAGE_FORM = d.DOSAGE_FORM,
                   Des_PACK_PRICE = d.PACK_PRICE,
                   PACK_SIZE = d.PACK_SIZE,
                   UNIT_NO = d.UNIT_NO,
                   UNIT_PRICE = d.UNIT_PRICE,
               }).ToList();
            var serializer = new JavaScriptSerializer();

            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = serializer.Serialize(Alternativies),
                ContentType = "application/json"
            };
            return Json(Alternativies, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]

        public JsonResult SavePrescription(PrescriptionViewModel data)
        {
            //update in Rosita temprory untill knows where to save
            //Docotr_Pharmacy
            Roshita DoctorChronicRoshita = db.Roshitas.Where(r => r.CardId == data.CardId && r.Manager == "Doctor_Chronic").OrderByDescending(c => c.Id).First();
            data.Speciality = DoctorChronicRoshita.Speciality;
            data.RoshetaType = "11602";
            data.Manager = "Pharmacy_Chronic";


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
                Manager = data.Manager,
                IsSync = null,
                SyncDate = null,
                SyncBy = null
            };

            db.Roshitas.Add(roshita);
            db.SaveChanges();

            var med_card = db.Med_Card.Where(m => m.CARD_NO == data.CardId).FirstOrDefault();
            RoshitaNoOverNoPay roshitaNoOverNoPay = new RoshitaNoOverNoPay
            {
                RositaId = roshita.Id,
                CreatedBy = User.Identity.Name,
                CreatedDate = DateTime.Now,
                NoOver = med_card.NO_OVER,
                NoPay = med_card.NO_PAY,
            };
            db.RoshitaNoOverNoPays.Add(roshitaNoOverNoPay);
            // RoshitaDetails
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                RoshitaDetail roshitaDetail1 = new RoshitaDetail();
                roshitaDetail1.RoshitaID = roshita.Id;
                roshitaDetail1.MedicienCode = Medicien.MedicienCode;
                roshitaDetail1.MedicienName = Medicien.MedicienName;
                roshitaDetail1.TotalUnits = Medicien.TotalUnits;
                roshitaDetail1.PaymentGroup = "Yes";
                roshitaDetail1.Dose = Medicien.Dose;
                roshitaDetail1.Duration = Medicien.Duration;
                roshitaDetail1.TotalDuration = Medicien.TotalDuration;
                roshitaDetail1.Amount = Medicien.Amount;
                roshitaDetail1.MedicineNoPay = Medicien.MedicineNoPay;
                roshitaDetail1.IsDealed = true;
                db.RoshitaDetails.Add(roshitaDetail1);
            }
            //update rostita with Doctor_chronic
            //update med_medicine
            RoshitaDetail roshitaDetail = new RoshitaDetail();
            Med_Medicine medMedicine = new Med_Medicine();
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                medMedicine = db.Med_Medicine.Where(x => x.CARD_NO == data.CardId && x.MED_CODE == Medicien.MedicienCode).FirstOrDefault();
                if (medMedicine != null)//if alternative
                {
                    medMedicine.EXCESS += (medMedicine.NO_OF_UINT * (medMedicine.PACK_SIZE / medMedicine.UNIT_NO)) - (Medicien.Dose * Medicien.Duration);
                    if (medMedicine.EXCESS >= (medMedicine.PACK_SIZE / medMedicine.UNIT_NO) || medMedicine.EXCESS < 0)
                    {
                        medMedicine.EXCESS = 0;
                    }
                    medMedicine.NO_OF_UINT = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(((Medicien.Dose * Medicien.Duration) - Convert.ToDouble(medMedicine.EXCESS)) / (Convert.ToDouble(medMedicine.PACK_SIZE) / Convert.ToDouble(medMedicine.UNIT_NO)))));
                    medMedicine.TOTAL_AMT = medMedicine.NO_OF_UINT * medMedicine.UNIT_PRICE;
                    medMedicine.SyncBy = "Updated";
                    db.Entry(medMedicine).State = EntityState.Modified;

                    roshitaDetail = db.RoshitaDetails.Where(r => r.Id == Medicien.Id).FirstOrDefault();
                    roshitaDetail.TotalUnits = medMedicine.NO_OF_UINT;
                    roshitaDetail.Amount = medMedicine.TOTAL_AMT.Value;
                    roshitaDetail.IsDealed = true;
                    db.Entry(roshitaDetail).State = EntityState.Modified;
                }
                //alternative
                // var group = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Where(m => m.M_CODE == id).FirstOrDefault();
                //List<ChronicViewModel> Alternativies = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Where(d => d.MED_GROUP == group.MED_GROUP && d.PACK_PRICE >= (MedicinePrice - 10) && d.PACK_PRICE <= (MedicinePrice + 10))


            }
            //SaveDiagnoises
            //diagnoise
            PrescriptionRoshitaDignosi diagnose = new PrescriptionRoshitaDignosi();
            diagnose.RositaId = roshita.Id;
            diagnose.DiagnoiseName = DoctorChronicRoshita.Diagnose1;
            db.PrescriptionRoshitaDignosis.Add(diagnose);

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
                int result = db.SaveChanges();
                return Json("2" + roshita.CreatedDate.Value.ToString("ddMMyy") + roshita.Id);

            }
            catch (DbEntityValidationException e)
            {
                db.Roshitas.Remove(roshita);
                db.SaveChanges();
                return Json("Failed to Save Prescription");
            }

        }

        //[Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        //public JsonResult Save(string id, float Totalvalue, float OverInsurance, float Cash, float PersonPayment, float CompanyPayment, int CompanyPercent, int Limit, string NationalId)
        //{
        //    //update in Rosita temprory untill knows where to save
        //    //Docotr_Pharmacy
        //    Roshita DoctorChronicRoshita = db.Roshitas.Where(r => r.CardId == id && r.Manager == "Doctor_Chronic").OrderByDescending(c => c.Id).First();
        //    Roshita PharmacyChronicRoshita = new Roshita();
        //    PharmacyChronicRoshita.CreatedBy = User.Identity.Name;
        //    PharmacyChronicRoshita.CardId = id;
        //    PharmacyChronicRoshita.Speciality = DoctorChronicRoshita.Speciality;
        //    PharmacyChronicRoshita.CreatedDate = DateTime.Now;
        //    PharmacyChronicRoshita.TotalValue = Math.Round(Totalvalue, 2);
        //    PharmacyChronicRoshita.OverInsurance = Math.Round(OverInsurance, 2);
        //    PharmacyChronicRoshita.Cash = Math.Round(Cash, 2);
        //    PharmacyChronicRoshita.PersonPayment = Math.Round(PersonPayment, 2);
        //    PharmacyChronicRoshita.CompanyPayment = Math.Round(CompanyPayment, 2);
        //    PharmacyChronicRoshita.CompanyPercent = CompanyPercent;
        //    PharmacyChronicRoshita.Limit = Limit;
        //    PharmacyChronicRoshita.Diagnose2 = NationalId;
        //    PharmacyChronicRoshita.RoshetaType = "11602";
        //    PharmacyChronicRoshita.Manager = "Pharmacy_Chronic";
        //    PharmacyChronicRoshita.IsSync = null;
        //    PharmacyChronicRoshita.SyncDate = null;
        //    PharmacyChronicRoshita.SyncBy = null;
        //    db.Roshitas.Add(PharmacyChronicRoshita);
        //    db.SaveChanges();
        //    //diagnoise
        //    PrescriptionRoshitaDignosi diagnose = new PrescriptionRoshitaDignosi();
        //    diagnose.RositaId = PharmacyChronicRoshita.Id;
        //    diagnose.DiagnoiseName = DoctorChronicRoshita.Diagnose1;
        //    db.PrescriptionRoshitaDignosis.Add(diagnose);
        //    int result = db.SaveChanges();
        //    Session["id"] = PharmacyChronicRoshita.Id;
        //    return Json("2" + PharmacyChronicRoshita.CreatedDate.Value.ToString("ddMMyy") + PharmacyChronicRoshita.Id, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize(Roles = "Admin,Pharmacy,Pharmacy_Admin")]
        //public JsonResult SaveMediciens(List<RoshitaDetail> Medciens)
        //{
        //    long RoshitaId = Convert.ToInt64(Session["id"]);
        //    string CardId = db.Roshitas.Where(x => x.Id == RoshitaId).FirstOrDefault().CardId;
        //    //insert RositaDetails 
        //    foreach (RoshitaDetail Medicien in Medciens)
        //    {
        //        RoshitaDetail roshitaDetail1 = new RoshitaDetail();
        //        roshitaDetail1.RoshitaID = Convert.ToInt64(Session["id"]);
        //        roshitaDetail1.MedicienCode = Medicien.MedicienCode;
        //        roshitaDetail1.MedicienName = Medicien.MedicienName;
        //        roshitaDetail1.TotalUnits = Medicien.TotalUnits;
        //        roshitaDetail1.PaymentGroup = "Yes";
        //        roshitaDetail1.Dose = Medicien.Dose;
        //        roshitaDetail1.Duration = Medicien.Duration;
        //        roshitaDetail1.TotalDuration = Medicien.TotalDuration;
        //        roshitaDetail1.Amount = Medicien.Amount;
        //        roshitaDetail1.IsDealed = true;
        //        db.RoshitaDetails.Add(roshitaDetail1);
        //    }
        //    //db.SaveChanges();
        //    //update rostita with Doctor_chronic
        //    //update med_medicine
        //    RoshitaDetail roshitaDetail = new RoshitaDetail();
        //    Med_Medicine medMedicine = new Med_Medicine();
        //    foreach (RoshitaDetail Medicien in Medciens)
        //    {
        //        medMedicine = db.Med_Medicine.Where(x => x.CARD_NO == CardId && x.MED_CODE == Medicien.MedicienCode).FirstOrDefault();
        //        if (medMedicine != null)
        //        {
        //            medMedicine.EXCESS += (medMedicine.NO_OF_UINT * (medMedicine.PACK_SIZE / medMedicine.UNIT_NO)) - (Medicien.Dose * Medicien.Duration);
        //            if (medMedicine.EXCESS >= (medMedicine.PACK_SIZE / medMedicine.UNIT_NO) || medMedicine.EXCESS < 0)
        //            {
        //                medMedicine.EXCESS = 0;
        //            }
        //            medMedicine.NO_OF_UINT = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(((Medicien.Dose * Medicien.Duration) - Convert.ToDouble(medMedicine.EXCESS)) / (Convert.ToDouble(medMedicine.PACK_SIZE) / Convert.ToDouble(medMedicine.UNIT_NO)))));
        //            medMedicine.TOTAL_AMT = medMedicine.NO_OF_UINT * medMedicine.UNIT_PRICE;
        //            medMedicine.SyncBy = "Updated";
        //            db.Entry(medMedicine).State = EntityState.Modified;

        //            roshitaDetail = db.RoshitaDetails.Where(r => r.Id == Medicien.Id).FirstOrDefault();
        //            roshitaDetail.TotalUnits = medMedicine.NO_OF_UINT;
        //            roshitaDetail.Amount = medMedicine.TOTAL_AMT.Value;
        //            roshitaDetail.IsDealed = true;
        //            db.Entry(roshitaDetail).State = EntityState.Modified;
        //        }
        //        //alternative
        //        // var group = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Where(m => m.M_CODE == id).FirstOrDefault();
        //        //List<ChronicViewModel> Alternativies = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Where(d => d.MED_GROUP == group.MED_GROUP && d.PACK_PRICE >= (MedicinePrice - 10) && d.PACK_PRICE <= (MedicinePrice + 10))


        //    }
        //    db.SaveChanges();
        //    return Json("saved");
        //}
    }
}