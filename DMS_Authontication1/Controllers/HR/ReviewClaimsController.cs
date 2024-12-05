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
using DMS_Authontication1.ViewModel.HR;
using DMS_Authontication1.Data_Function;
using static System.Net.WebRequestMethods;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR,HR_Admin")]
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class ReviewClaimsController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext myEntities;
        DB106 dbData;

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

        public ReviewClaimsController()
        {
            db = new DMS_TESTEntities();
            myEntities = new ApplicationDbContext();
            dbData = new DB106();
        }
        public ActionResult History()
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

        public JsonResult GetClaims(string compId, string servFrom, string servTo, string regFrom, string regTo,
                                    string aprovNo, string cardId, string invocNo, string batchNo)
        {
            //string compId, string servFrom, string servTo, string regFrom, string regTo,
            //                        string aprovNo, string cardId, string invocNo, string batchNo

            Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;
           
            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
            regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
            servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
            servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;


            aprovNoFrom = string.IsNullOrEmpty(aprovNo) ? 0 : Convert.ToInt64(aprovNo);
            aprovNoTo = string.IsNullOrEmpty(aprovNo) ? 999999999999999999 : Convert.ToInt64(aprovNo);
            invocNoFrom = string.IsNullOrEmpty(invocNo) ? 0 : Convert.ToInt64(invocNo);
            invocNoTo = string.IsNullOrEmpty(invocNo) ? 999999999999999999 : Convert.ToInt64(invocNo);
            batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
            batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);
            
            int comp = Convert.ToInt32(compId);

            DataTable dt = new DataTable();

                dt = dbData.getClaims(comp, servDateFrom, servDateTo, regDateFrom, regDateTo, aprovNoFrom, aprovNoTo, 
                                      cardId, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo);
           
            List<ClaimsViewModel> clms = new List<ClaimsViewModel>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    clms.Add(new ClaimsViewModel
                    {
                        ClaimNo = row["CLAIM_NO"].ToString(),
                        CreatedDate = row["CREATED_DATE"].ToString(),
                        ClaimDate = row["CLAIM_DATE"].ToString(),
                        CardNo = row["CARD_NO"].ToString(),
                        EmpName = row["EMP_NAME"].ToString(),
                        ProvName = row["PRV_NAME"].ToString(),
                        ProvType = row["PROVIDER_TYPE"].ToString(),
                        Diagnosis = row["DIAGNOSIS"].ToString(),
                        ServType = row["SERV_TYPE"].ToString()                        
                    });
                }
                return new JsonResult { Data = new { claimslist = clms, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            else
                return new JsonResult { Data = new { claimslist = clms, msg = "empty" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult GetClaimDetails(string claimNo)
        {           
            DataTable dt = new DataTable();

            dt = dbData.getClaimDetails(Int64.Parse(claimNo));

            List<ClaimDetailsViewModel> clms = new List<ClaimDetailsViewModel>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    clms.Add(new ClaimDetailsViewModel
                    {
                        Services = row["SERVICES"].ToString(),
                        SercvName = row["SERCV_NAME"].ToString(),
                        ClaimSubmitted = row["CLAIM_SUBMITTED"].ToString(),
                        OverInsurance = row["OVER_INSURANCE"].ToString(),
                        Discount = row["DISCOUNT"].ToString(),
                        ApprovAmount = row["APPROV_AMOUNT"].ToString(),
                        CopayAmt = row["COPAY_AMT"].ToString(),
                        AfterCopay = row["AFTER_COPAY"].ToString(),
                        LocalAmount = row["LOCAL_AMOUNT"].ToString(),
                        ImportAmount = row["IMPORT_AMOUNT"].ToString(),
                        LocalDisc = row["LOCAL_DISC"].ToString(),
                        ImportDisc = row["IMPORT_DISC"].ToString(),
                        TotalDiscount = row["TOTAL_DISCOUNT"].ToString(),
                        Net = row["NET"].ToString()
                    });
                }
                return new JsonResult { Data = new { claimslist = clms, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            else
                return new JsonResult { Data = new { claimslist = clms, msg = "empty" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult PrintAllClaims(string compId, string servFrom, string servTo, string regFrom, string regTo,
                                            string aprovNo, string cardId, string invocNo, string batchNo)
        {
            Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            string cardStart, cardEnd;

            regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
            regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
            servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
            servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;


            aprovNoFrom = string.IsNullOrEmpty(aprovNo) ? 0 : Convert.ToInt64(aprovNo);
            aprovNoTo = string.IsNullOrEmpty(aprovNo) ? 999999999999999999 : Convert.ToInt64(aprovNo);
            invocNoFrom = string.IsNullOrEmpty(invocNo) ? 0 : Convert.ToInt64(invocNo);
            invocNoTo = string.IsNullOrEmpty(invocNo) ? 999999999999999999 : Convert.ToInt64(invocNo);
            batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
            batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);
            
            cardStart = string.IsNullOrEmpty(cardId) || cardId == "null" ? " " : cardId;
            cardEnd = string.IsNullOrEmpty(cardId) || cardId == "null" ? "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz" : cardId;

            int comp = Convert.ToInt32(compId);

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReviewClaimsReport.rpt"));

            rd.SetDatabaseLogon("APP", "12369");


            rd.SetParameterValue("reg1", regDateFrom);
            rd.SetParameterValue("reg2", regDateTo);
            rd.SetParameterValue("serv1", servDateFrom);
            rd.SetParameterValue("serv2", servDateTo);


            rd.SetParameterValue("cmp", comp);
            rd.SetParameterValue("crd1", cardStart);
            rd.SetParameterValue("crd2", cardEnd);

            rd.SetParameterValue("aprov1", aprovNoFrom);
            rd.SetParameterValue("aprov2", aprovNoTo);
            rd.SetParameterValue("invoc1", invocNoFrom);
            rd.SetParameterValue("invoc2", invocNoTo);

            rd.SetParameterValue("batch1", batchNoFrom);
            rd.SetParameterValue("batch2", batchNoTo);

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
                return File(stream, "application/xls", compId + "AllClaims" + DateTime.Now.ToString("ddMMyyyy") + ".xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}