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
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DMS_Authontication1.Controllers
{
    public class LabApprovalsController : Controller
    {
        private DMS_TESTEntities db;

        public JsonRequestBehavior JsonRequestBehavior { get; private set; }

        public LabApprovalsController()
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
            data.RoshetaType = "Lab_Daily";
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

        public JsonResult Labs()
        {
            List<Pr_Bra> Labs = new List<Pr_Bra>();
            Labs = db.Pr_Bra.Where(x => x.PRV_TYPE == 3).GroupBy(x => x.PR_CODE).Select(x => x.FirstOrDefault()).ToList();
            return new JsonResult { Data = Labs, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
                Manager = "Lab_Daily",
                IsSync = null,
                SyncDate = null,
                SyncBy = null,
                IsFamily = data.IsFamily,
                IsPool = data.IsPool,
            };

            //db.Roshitas.Add(roshita);
            //db.SaveChanges();
            // RoshitaDetails
            foreach (RoshitaDetail Medicien in data.roshitaDetail)
            {
                Medicien.RoshitaID = roshita.Id;
                Medicien.IsDealed = false;
                roshita.RoshitaDetails.Add(Medicien);
            }
            //SaveDiagnoises
            foreach (Diagnose item in data.diagnose)
            {
                PrescriptionRoshitaDignosi dignosi = new PrescriptionRoshitaDignosi();
                dignosi.RositaId = roshita.Id;
                dignosi.DiagnoiseName = item.DIAG_ANAME;
                roshita.PrescriptionRoshitaDignosis.Add(dignosi);
            }
            //RoshitaPharmcyApproved
            if (data.Diagnose2 != null)
            {
                RoshitaPharmcyApproved roshitaPharmcyApproved = new RoshitaPharmcyApproved();
                roshitaPharmcyApproved.Pharmacy = data.Diagnose2;
                roshitaPharmcyApproved.Branch = data.PhoneNumber;
                roshitaPharmcyApproved.RoshitaId = roshita.Id;
                roshita.RoshitaPharmcyApproveds.Add(roshitaPharmcyApproved);
            }
            try
            {
                db.Roshitas.Add(roshita);
                int result = db.SaveChanges();
                var model = db.CardsSms.Where(c => c.CardId == roshita.CardId).FirstOrDefault();
                if (model != null)
                {
                    try
                    {
                        PostSMSData("New Labs has been added . If it is not used, please call 0226390390 ", model.Phone);
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
                //db.Roshitas.Remove(roshita);
                return Json("Failed to Save Prescription");
            }

        }

        [HttpPost]
        public JsonResult DoctorDelete(long id, long roshitaid)
        {
            try
            {
                Roshita roshita = db.Roshitas.Include(x => x.RoshitaDetails).Where(c => c.Id == roshitaid).FirstOrDefault();
                //foreach (var item in roshita.RoshitaDetails)
                //{
                //    if (item.Id == id)
                //        db.RoshitaDetails.Remove(item);
                //}
                var roshitadetail = roshita.RoshitaDetails.Where(x => x.Id == id).First();
                // roshita.RoshitaDetails.Remove(roshitadetail);
                db.RoshitaDetails.Remove(roshitadetail);
                if (roshita.RoshitaDetails.Count > 0)
                {
                    roshita.UpdatedBy = User.Identity.Name;
                    roshita.UpdatedDate = DateTime.Now;
                    roshita.CompanyPayment -= roshitadetail.Amount * (roshita.CompanyPercent / 100);
                    roshita.PersonPayment -= roshitadetail.Amount * ((100 - roshita.CompanyPercent) / 100);
                    roshita.TotalValue = roshita.CompanyPayment + roshita.PersonPayment;

                }
                else
                {
                    roshita.Manager = "Lab_Daily_Stop";
                }
                db.Entry(roshita).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

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

        public JsonResult EmployeeData(string id)
        {
            var EmployeeData = db.Comp_Employees.Where(c => c.CARD_ID == id && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            string gender;
            switch (EmployeeData.GENDER)
            {
                case 1:
                    gender = "Male";
                    break;
                case 2:
                    gender = "Female";
                    break;
                default:
                    gender = " Not Defined ";
                    break;
            }
            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            var Age = today.Year - EmployeeData.BIRTH_DATE.Value.Year;
            string BirthDate = String.Format("{0:MM/dd/yyyy}", EmployeeData.BIRTH_DATE.Value);
            string SpecificDate = String.Format("{0:MM/dd/yyyy}", EmployeeData.SPECIFIC_DATE.Value);
            return Json(new { Age = Age, Gender = gender, BirthDate = BirthDate, SpecificDate = SpecificDate });

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
            bytes = System.Text.Encoding.UTF8.GetBytes(requestXml);
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
        [Authorize(Roles = "Admin,Lab,Lab_Admin")]
        public JsonResult GetList(long userProvider, int sEcho = 1, int iDisplayStart = 0, int iDisplayLength = 24, string sSearch = "")
        {
            ApplicationDbContext myEntities = new ApplicationDbContext();
            var labs = new List<Serv_Lab>();
            //var user = myEntities.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //long userProvider = Convert.ToInt64(user.Provider);
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
        public JsonResult GetLabByCode(long userProvider, long code)
        {

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
        #endregion
    }
}