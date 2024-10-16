using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_TEST.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DMS_TEST.Controllers
{
    public class ChronicController : Controller
    {
        DMS_TESTEntities db = new DMS_TESTEntities();
        ApplicationDbContext myEntities = new ApplicationDbContext();
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
            DateTime datenow = DateTime.Now.Date;
            //var datenowvalue = new DateTime(datenow.Year, datenow.Month, datenow.Day);
            var empCardTerminationFlag = db.Comp_Employees.Where(x => x.CARD_ID == id && x.INS_START_DATE <= datenow
            && x.INS_END_DATE >= datenow).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            int CompId = Convert.ToInt32(id.Split('-')[0].ToString());
            if (!string.IsNullOrEmpty(empCardTerminationFlag.FAX))
            {
                if (empCardTerminationFlag.FAX == "0")
                {
                    if (CompId.ToString().StartsWith("70") || CompId.ToString().StartsWith("10"))
                    {
                        if (DateTime.Now.Day > 5 && DateTime.Now.Day <= 20)
                        {
                            ViewBag.Message = "Finish Dispence Date";
                            return View(data);
                        }
                    }
                }
            }

            //chick if company is hold or not 

            var model = db.APPROVAL_BAD.Where(x => x.COMP_ID == CompId).FirstOrDefault();
            if (model != null)
            {
                if (model.FLAG == "Y")
                {
                    ViewBag.Message = "Hold Company";
                    return View(data);
                }
            }

            var HrUserNamre = User.Identity.GetUserName();
            var userid = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Id;
            var providerBlock = db.ProviderBlocks.Where(x => x.UserId == userid && x.CardId == id && x.ServiceCode == "11602").FirstOrDefault();
            if (providerBlock != null)
            {
                if (providerBlock.IsActive.Value)
                {
                    ViewBag.Message = "Block";
                    return View(data);
                }
            }
            else
            {

                var found = db.ProviderBlocks.Where(x => x.UserId == userid && x.CompId == CompId && x.ServiceCode == "11602" && x.IsActive == true).Any();
                if (found)
                {
                    ViewBag.Message = "Block";
                    return View(data);
                }
            }
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
                    var providers = medCard.PROVIDER_CODE.Split('_');
                    if (providers.Contains("1268") || providers.Contains(Provider.PR_CODE.ToString()))
                    {
                        data = db.RoshitaDetails.Where(x => x.RoshitaID == Rosita.Id && x.IsDealed == false && x.TotalUnits != 0)
                        .Join(db.Med_Medicine, d => d.MedicienCode, m => m.MED_CODE, (d, m) => new { d, m })
                        .Join(db.MedicineDatas, med => med.m.MED_CODE, md => md.M_CODE, (med, md) => new { med, md })
                        .Where(l => l.med.m.CARD_NO == id && l.med.m.ACTIVE != "N" && l.md.ACTIVE != "N" && (l.med.m.StartDate <= datenow || l.med.m.StartDate == null))
                        .Select(l => new ChronicViewModel
                        {
                            Id = l.med.d.Id,
                            MED_CODE = l.med.d.MedicienCode,
                            MED_NAME = l.med.d.MedicienName,
                            DOSE = l.med.d.Dose,
                            MED_DURATION = l.med.d.Duration,
                            NO_OF_UINT = l.med.m.NO_OF_UINT,
                            TOTAL_AMT = l.med.m.TOTAL_AMT.Value,
                            //NO_OF_UINT = l.med.d.TotalUnits,
                            //TOTAL_AMT = l.med.d.Amount,
                            DOSAGE_FORM = l.med.m.DOSAGE_FORM,
                            UNIT_NO = l.med.m.UNIT_NO,
                            Des_PACK_PRICE = l.med.m.PACK_PRICE,
                            PACK_SIZE = l.med.m.PACK_SIZE,
                            UNIT_PRICE = l.med.m.UNIT_PRICE,
                            MedicineNoPay = l.med.m.MedicineNoPay.Trim()
                        }).Distinct().ToList();
                        //var datacompare = new List<ChronicViewModel>();
                        //datacompare.AddRange(data);
                        //DateTime valuedate = new DateTime(DateTime.Now.Year, 3, 20);
                        //var roshitaold = db.Roshitas.Where(r => r.CardId == medCard.CARD_NO && r.Manager == "Pharmacy_Chronic" && DbFunctions.TruncateTime(r.CreatedDate) > valuedate).Include(x => x.RoshitaDetails).ToList();
                        //if (roshitaold.Count > 0)
                        //{
                        //    foreach (var item in roshitaold)
                        //    {
                        //        foreach (var item2 in item.RoshitaDetails)
                        //        {
                        //            foreach (var item3 in datacompare)
                        //            {
                        //                if (item3.MED_CODE == item2.MedicienCode)
                        //                    data.Remove(item3);
                        //            }
                        //        }
                        //    }
                        //}
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
                DateTime datenow = DateTime.Now.Date;
                var date = new DateTime(datenow.Year, datenow.Month, datenow.Day);
                Med_Card medCard = db.Med_Card.Where(m => m.CARD_NO == id && m.LOOK_01 == 0).FirstOrDefault();
                if (medCard.NO_PAY == 1 && (medCard.NoPayEndDate == null || medCard.NoPayEndDate > date))
                    medCard.NO_PAY = 1;
                else
                    medCard.NO_PAY = 0;
                if (medCard.NO_OVER == 1 && (medCard.NoOverEndDate == null || medCard.NoOverEndDate > date))
                    medCard.NO_OVER = 1;
                else
                    medCard.NO_OVER = 0;
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
            List<ChronicViewModel> Alternativies = db.MedicineDatas.Where(x => x.ACTIVE == "Y").Where(d => d.MED_GROUP == group.MED_GROUP && /*d.PACK_PRICE >= (MedicinePrice - 10) &&*/ d.PACK_PRICE <= (MedicinePrice + 10))
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
            var username = User.Identity.Name;
            var carduse = db.CardUseds.Where(c => c.CardId == data.CardId && c.CreatedBy == username).FirstOrDefault();
            if (carduse == null)
            {
                return Json("Failed");
            }
            //var carduse = db.CardUseds.Where(c => c.CardId == data.CardId).FirstOrDefault();
            //if (carduse == null)
            //{
            //    return Json("Failed to Save Prescription");
            //}
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
                SyncBy = null,
                IsFamily = data.IsFamily,
                IsPool = data.IsPool,
            };
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
            db.Roshitas.Add(roshita);
            db.CardUseds.Remove(carduse);
            db.SaveChanges();
            DateTime datenow = DateTime.Now.Date;
            var date = new DateTime(datenow.Year, datenow.Month, datenow.Day);
            var med_card = db.Med_Card.Where(m => m.CARD_NO == data.CardId).FirstOrDefault();
            if (med_card.NO_PAY == 1 && (med_card.NoPayEndDate == null || med_card.NoPayEndDate > date))
                med_card.NO_PAY = 1;
            else
                med_card.NO_PAY = 0;
            if (med_card.NO_OVER == 1 && (med_card.NoOverEndDate == null || med_card.NoOverEndDate > date))
                med_card.NO_OVER = 1;
            else
                med_card.NO_OVER = 0;
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
            diagnose.DiagnoiseName = med_card.TASHKHES_01;
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
                //Comp_Employees employee = db.Comp_Employees.Where(x => x.CARD_ID == data.CardId && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now)
                //    .Include(x => x.EmployeesSMSCodes)
                //    .FirstOrDefault();
                var model = db.EmployeesSMSCodes.Include(c => c.Comp_Employees).Where(x => x.Comp_Employees.CARD_ID == data.CardId).FirstOrDefault();
                if (model != null)
                {
                    model.IsActive = false;
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;
                    model.LastSentDate = DateTime.Now;
                    db.Entry(model).State = EntityState.Modified;
                    Random generator = new Random();
                    String SMSCode = generator.Next(0, 1000000).ToString("D6");
                    EmployeesSMSCode NewEmpSMSCode = new EmployeesSMSCode();
                    NewEmpSMSCode.EmpId = model.Comp_Employees.Id;
                    NewEmpSMSCode.SMSCode = SMSCode;
                    NewEmpSMSCode.IsActive = true;
                    NewEmpSMSCode.CreatedBy = User.Identity.Name;
                    NewEmpSMSCode.CreatedDate = DateTime.Now;
                    NewEmpSMSCode.LastSentDate = DateTime.Now;
                    db.EmployeesSMSCodes.Add(NewEmpSMSCode);
                    try
                    {
                        PostSMSData("your DMS verification code to dispense chronic medicines is " + SMSCode, model.Comp_Employees.TEL1);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                var modelSms = db.CardsSms.Where(c => c.CardId == roshita.CardId).FirstOrDefault();
                if (modelSms != null)
                {
                    try
                    {
                        PostSMSData("Your medication card has been dispensed . If it is not used, please call 0226390390 \n download claim :" +
                            "https://sios-eg.com/NetworkMedical/PrintRoshita/" + roshita.Id, modelSms.Phone);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

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