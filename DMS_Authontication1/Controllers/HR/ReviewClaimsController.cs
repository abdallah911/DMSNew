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
        DBApproval106 dbOra;
        DBData dbApproval;
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
            dbOra = new DBApproval106();
            dbApproval = new DBData();
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
        string getncardapproval(string crd)
        {
            string ncrd = "";

            System.Data.DataTable dtoldcrdaprov = new System.Data.DataTable();
            dtoldcrdaprov = dbOra.RunReader(@"SELECT CLOSE_EMP_DATA.CARD_ID FROM DMS_TEST.CLOSE_EMP_DATA WHERE (CLOSE_EMP_DATA.TRANS_TYP = 'D' OR CLOSE_EMP_DATA.TRANS_TYP = 'L') AND CLOSE_EMP_DATA.N_CARD = '" + crd + "'");

            if (dtoldcrdaprov.Rows.Count > 0 && dtoldcrdaprov.Rows[0][0].ToString() != string.Empty)
                ncrd = dtoldcrdaprov.Rows[0][0].ToString();
            else
                ncrd = "";

            return ncrd;
        }
        string getMaxAmountForCard(string cmp, string contr, string cls, string crd)
        {
            string maxAmount = "";
            DataTable dtmxamt = new DataTable();

            dtmxamt = dbOra.RunReader("SELECT MAX_AMOUNT FROM DMS_TEST.COMP_CONTRACT_CLASS_EMP WHERE C_COMP_ID = '" + cmp + "' and CONTRACT_NO = '" + contr + "' and CLASS_CODE = '" + cls + "' AND CARD_ID = '" + crd + "'");

            if (dtmxamt.Rows.Count > 0 && dtmxamt.Rows[0][0].ToString() != string.Empty)
                maxAmount = dtmxamt.Rows[0][0].ToString();
            else
            {
                dtmxamt = dbOra.RunReader("SELECT MAX_AMOUNT FROM DMS_TEST.COMP_CONTRACT_CLASS WHERE C_COMP_ID = '" + cmp + "' and CONTRACT_NO = '" + contr + "' and CLASS_CODE = '" + cls + "'");

                maxAmount = dtmxamt.Rows[0][0].ToString();
            }

            return maxAmount;
        }
        string getColorCardApproval(string crd, string crdold)
        {
            string crdcolr = "";

            System.Data.DataTable dtColorCard = new System.Data.DataTable();
            dtColorCard = dbOra.RunReader(@"SELECT CARD_ID, COLOR_NAME, PRINT_DATE FROM CARD_PRINT_HISTORY 
                                         WHERE (CARD_ID = '" + crd + "' OR CARD_ID = '" + crdold + "') ORDER BY PRINT_DATE desc");

            if (dtColorCard.Rows.Count > 0)
                crdcolr = dtColorCard.Rows[0][1].ToString();

            return crdcolr;
        }
        public JsonResult getData(string CardId)
        {          
            DataTable dtcrd = new DataTable();

            dtcrd = dbOra.RunReader("select  to_char(BIRTH_DATE,'DD-MM-YYYY'),C_COMP_ID,CLASS_CODE,NVL(to_char(SPECIFIC_DATE,'DD-MM-YYYY'),to_char(INS_START_DATE,'DD-MM-YYYY')),to_char(INS_END_DATE,'DD-MM-YYYY'),TERMINATE_FLAG ,EMP_ANAME_ST ,EMP_ANAME_SC,EMP_ANAME_TH ,EMP_ENAME_ST ,EMP_ENAME_SC,EMP_ENAME_TH, to_char(INS_START_DATE,'DD-MM-YYYY'), to_char(TERMINATE_DATE,'DD-MM-YYYY') ,CONTRACT_NO, TEL1, TEL2, EMP_ID,  DECODE (GENDER, 1, 'Male', 2, 'Female')  Gender, extract(year from numtoyminterval(months_between(trunc(sysdate),TRUNC(NVL(BIRTH_DATE, sysdate))),'month')) AS AGE  from dms_test.COMP_EMPLOYEES where CARD_ID='" + CardId + "' order by ins_start_date DESC");

            List<CardInformationViewModel> dtDetails = new List<CardInformationViewModel>();

            if (dtcrd.Rows.Count != 0)
            {
                string nopay = "", nover = "", oldcrd = "";

                    DataTable dtpo = dbOra.RunReader(@"SELECT decode(NVL(NO_PAY,0), 1, 'Yes', 'No') no_pay, decode(NO_OVER, 1, 'Yes', 'No') no_over FROM MED_CARD_NEW WHERE CARD_NO = '" + CardId + "'");

                    if (dtpo.Rows.Count > 0)
                    {
                        nopay = dtpo.Rows[0][0].ToString();
                        nover = dtpo.Rows[0][1].ToString();
                    }

                    oldcrd = getncardapproval(CardId);

                    string oldcrd2 = oldcrd != "" ? oldcrd : CardId;

                    dtDetails.Add(new CardInformationViewModel
                    {                       
                        EmployeeName = dtcrd.Rows[0][6].ToString() + " " + dtcrd.Rows[0][7].ToString() + " " + dtcrd.Rows[0][8].ToString(),
                        BirthDate = dtcrd.Rows[0][0].ToString(),
                        Age = dtcrd.Rows[0]["AGE"].ToString(),
                        SpecificDate = dtcrd.Rows[0][3].ToString(),
                        StartDate = dtcrd.Rows[0][12].ToString(),
                        EndDate = dtcrd.Rows[0][4].ToString(),
                        MaxAmount = getMaxAmountForCard(dtcrd.Rows[0][1].ToString(), dtcrd.Rows[0][14].ToString(), dtcrd.Rows[0][2].ToString(), CardId),
                        ClassName = dbOra.RunReader("select CLASS_ENAME from V_CLASS_NAME where CLASS_CODE ='" + dtcrd.Rows[0][2].ToString() + "'").Rows[0][0].ToString(),
                        HospitalDegree = dbOra.RunReader(@"SELECT HOSPITAL_DEGREE, BS_ANAME  FROM dms_test.COMP_CONTRACT_CLASS, dms_test.BASIC_DATA WHERE SOURCE_MOD = 'ACCDEG' AND BS_CODE = HOSPITAL_DEGREE AND C_COMP_ID= '" + dtcrd.Rows[0][1].ToString() + "' AND CONTRACT_NO = '" + dtcrd.Rows[0][14].ToString() + "' AND CLASS_CODE = '" + dtcrd.Rows[0][2].ToString() + "'").Rows[0][1].ToString(),
                        MedicalNetwork = dbOra.RunReader(@"SELECT COVER_RELATION, BS_ANAME  FROM dms_test.COMP_CONTRACT_CLASS, dms_test.BASIC_DATA WHERE SOURCE_MOD = 'PRDEG' AND BS_CODE = COVER_RELATION AND C_COMP_ID= '" + dtcrd.Rows[0][1].ToString() + "' AND CONTRACT_NO = '" + dtcrd.Rows[0][14].ToString() + "' AND CLASS_CODE = '" + dtcrd.Rows[0][2].ToString() + "'").Rows[0][1].ToString(),
                        ExceptionPayment = nopay,
                        ExceptionOver = nover,
                        NationalId = dtcrd.Rows[0]["EMP_ID"].ToString(),
                        Mobile1 = dtcrd.Rows[0]["TEL1"].ToString(),
                        Mobile2 = dtcrd.Rows[0]["TEL2"].ToString(),
                        Gender = dtcrd.Rows[0]["Gender"].ToString(),
                        CardColor = getColorCardApproval(CardId, oldcrd),
                        OldCard = oldcrd                       
                    });
                }
                return new JsonResult { Data = new { dtDetails }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            
        }
        public JsonResult getAprovalData(string CardId)
        {            
            DataTable dt = new DataTable();

            string oldcrd = getncardapproval(CardId);

            oldcrd = oldcrd != "" ? oldcrd : CardId;


            dt = dbApproval.getApproval(CardId, oldcrd);

            
            List<MedicalApprovalViewModel> approval = new List<MedicalApprovalViewModel>();

            if (dt.Rows.Count != 0)
            {              
                foreach (DataRow row in dt.Rows)
                {
                    approval.Add(new MedicalApprovalViewModel
                    {
                        ApprovalNo = row["APPROV_NO"].ToString(),
                        ApprovalType = row["SERVECE_TYP"].ToString(),
                        Reply = row["REPLY"].ToString(),
                        ApprovalAmount = row["APPROV_AMOUNT"].ToString(),
                        MedicalReply = row["MEDICAL_REPLAY"].ToString(),
                        CreatedBy = row["CREATED_BY"].ToString(),
                        CreatedDate = row["CREATED_DATE"].ToString(), 
                        CreatedDate1 = row["CREATED_DATE1"].ToString()
                    });
                }
                return new JsonResult { Data = new { approval }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
                return new JsonResult { Data = new { approval }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult getChornicData(string CardId)
        {
            DataTable dt = new DataTable();
            
            List<ChronicDetailsViewModel> clmD = new List<ChronicDetailsViewModel>();

            var medicin = (from md in db.Med_Card
                           join m in db.Med_Medicine
                           on md.CARD_NO equals m.CARD_NO
                           where m.ACTIVE == "Y" &&
                                 md.LOOK_01 == 0 &&
                                    m.CARD_NO == CardId
                           orderby m.CREATED_DATE ?? m.UPDATE_DATE descending
                           select m).ToList();

            if (medicin != null && medicin.Count > 0)
            {
                foreach (var med in medicin)
                {
                    clmD.Add(new ChronicDetailsViewModel
                    {
                        MED_CODE = med.MED_CODE,
                        MED_NAME = med.MED_NAME,
                        DOSE = med.DOSE.ToString(),
                        MED_DURATION = med.MED_DURATION.ToString(),
                        UNIT_NO = med.UNIT_NO.ToString(),
                        DOSAGE_FORM = med.DOSAGE_FORM.ToString(),
                        MONTH_DATE_STOP = med.MONTH_DATE_STOP.ToString()
                    });
                }
                return new JsonResult { Data = new { clmD }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
                return new JsonResult { Data = new { clmD }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public ActionResult PrintPdf(string compId, string servFrom, string servTo, string regFrom, string regTo,
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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReviewCliamsReportSummary.rpt"));

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
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", compId + "AllClaimsSummary" + DateTime.Now.ToString("ddMMyyyy") + ".pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult PrintExcel(string compId, string servFrom, string servTo, string regFrom, string regTo,
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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReviewCliamsReportSummary.rpt"));

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
                return File(stream, "application/xls", compId + "AllClaimsSummary" + DateTime.Now.ToString("ddMMyyyy") + ".xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}