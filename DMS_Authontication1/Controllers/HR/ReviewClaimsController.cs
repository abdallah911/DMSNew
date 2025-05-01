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
        //public ActionResult History()
        //{
        //    if (User.IsInRole("HR_Admin"))
        //    {
        //        var userid = User.Identity.GetUserId();
        //        var compines = db.HrAdminCompanies.Where(x => x.UserId == userid).Select(c => c.CompId).ToList();
        //        if (compines[0] == "All")
        //        {
        //            var companyname = db.Contract_Comp
        //                .Select(l => new
        //                {
        //                    Code = l.C_COMP_ID,
        //                    Name = l.C_ENAME + " || " + l.C_COMP_ID

        //                }).ToList();
        //            SelectList companylist = new SelectList(companyname, "Code", "Name");
        //            ViewBag.company = companylist;
        //        }
        //        else
        //        {
        //            var companyname = (from comp in compines
        //                               join contCo in db.Contract_Comp
        //                               on int.Parse(comp) equals contCo.C_COMP_ID
        //                               select new
        //                               {
        //                                   Code = contCo.C_COMP_ID,
        //                                   Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
        //                               }).ToList();
        //            SelectList companylist = new SelectList(companyname, "Code", "Name");
        //            ViewBag.company = companylist;
        //        }

        //        return View();
        //    }
        //    else
        //    {
        //        var HrUserNamre = User.Identity.GetUserName();
        //        string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
        //        ViewBag.compnum = compa;
        //        return View();
        //    }
        //}

        #region CompanyInvoice
        public ActionResult CompanyInvoice()
        {
            //DataTable companyAll = dbOra.RunReader(@"SELECT DISTINCT R.COMP_ID, C.C_ANAME
            //                                         FROM   APP.REVIEW_CLAIMS R, DMS_TEST.CONTRACT_COMP C
            //                                         WHERE  R.COMP_ID = C.C_COMP_ID");

            //var companyList = companyAll.AsEnumerable()
            //    .Select(row => new
            //    {
            //        Code = row["COMP_ID"].ToString(),
            //        Name = row["C_ANAME"].ToString() + " || " + row["COMP_ID"].ToString()
            //    }).ToList();

            //SelectList companylist = new SelectList(companyList, "Code", "Name");
            //ViewBag.company = companylist;

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
                int compa = int.Parse(myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider);

                var Companies = db.Contract_Comp.Where(x => x.C_COMP_ID == compa)
                    .Select(c => new
                    {
                        Code = c.C_COMP_ID,
                        Name = c.C_ENAME + " || " + c.C_COMP_ID
                    }).ToList(); ;

                SelectList companylist = new SelectList(Companies, "Code", "Name");
                ViewBag.company = companylist;
                
                return View();
            }
            //return View();
        }

        public JsonResult GetCompanyInvoice(string compId, string servFrom, string servTo, string regFrom, string regTo,
                                            string invocNo, string batchNo)
        {
            //string compId, string servFrom, string servTo, string regFrom, string regTo,
            //                        string aprovNo, string cardId, string invocNo, string batchNo

            Int64 invocNoFrom, invocNoTo, batchNoFrom, batchNoTo, comp1, comp2;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
            regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
            servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
            servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;

            comp1 = string.IsNullOrEmpty(compId) ? 0 : Convert.ToInt64(compId);
            comp2 = string.IsNullOrEmpty(compId) ? 999999999999999999 : Convert.ToInt64(compId);
           
            invocNoFrom = string.IsNullOrEmpty(invocNo) ? 0 : Convert.ToInt64(invocNo);
            invocNoTo = string.IsNullOrEmpty(invocNo) ? 999999999999999999 : Convert.ToInt64(invocNo);
            batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
            batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);

            DataTable dt = new DataTable();

            dt = dbData.getInvoices(comp1, comp2, servDateFrom, servDateTo, regDateFrom, regDateTo,
                                    invocNoFrom, invocNoTo, batchNoFrom, batchNoTo);

            List<InvoiceViewModel> invo = new List<InvoiceViewModel>();
      
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    invo.Add(new InvoiceViewModel
                    {
                        CompId = row["COMP_ID"].ToString(),
                        CompName = row["C_ANAME"].ToString(),
                        StartDate = row["START_DATE"].ToString(),
                        EndDate = row["END_DATE"].ToString(),
                        CountOfBatch = row["COUNT_BATCH"].ToString(),
                        CountOfClaim = row["COUNT_CLAIM"].ToString(),
                        Gross = row["GROSS"].ToString(),
                        Net = row["NET"].ToString(),
                        InvoiceNo = row["INVOICE_NO"].ToString()
                    });
                }
                return new JsonResult { Data = new { invoicelist = invo, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            else
                return new JsonResult { Data = new { invoicelist = invo, msg = "empty" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult GetBatchDetails(string compNo, string invocNo, string servFrom, string servTo, string regFrom, string regTo, string batchNo)
        {
            DataTable dt = new DataTable();

            Int64 batchNoFrom, batchNoTo;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
            regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
            servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
            servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;

            batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
            batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);


            dt = dbData.getBatch(Int64.Parse(compNo), Int64.Parse(invocNo), servDateFrom, servDateTo, regDateFrom, regDateTo, batchNoFrom, batchNoTo);

            List<BatchViewModel> clms = new List<BatchViewModel>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    clms.Add(new BatchViewModel
                    {
                        BatchNumber = row["BATCH_NO"].ToString(),
                        ProviderID = row["PRV_NO"].ToString(),
                        ProviderName = row["PRV_NAME"].ToString(),
                        ProviderType = row["PROVIDER_TYPE"].ToString(),
                        CountOfClaim = row["COUNT_CLAIM"].ToString(),
                        Gross = row["GROSS"].ToString(),
                        Net = row["NET"].ToString()
                    });
                }
                return new JsonResult { Data = new { batchlist = clms, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            else
                return new JsonResult { Data = new { batchlist = clms, msg = "empty" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult PrintCompanyInvoiceReport(string compId, string servFrom, string servTo, string regFrom, string regTo,
                                                      string invocNo, string batchNo, int typ)
        {
            Int64 invocNoFrom, invocNoTo, batchNoFrom, batchNoTo, comp1, comp2;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
            regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
            servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
            servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;

            comp1 = string.IsNullOrEmpty(compId) ? 0 : Convert.ToInt64(compId);
            comp2 = string.IsNullOrEmpty(compId) ? 999999999999999999 : Convert.ToInt64(compId);

            invocNoFrom = string.IsNullOrEmpty(invocNo) ? 0 : Convert.ToInt64(invocNo);
            invocNoTo = string.IsNullOrEmpty(invocNo) ? 999999999999999999 : Convert.ToInt64(invocNo);
            batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
            batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);

            DataTable dt = new DataTable();

            dt = dbData.getInvoices(comp1, comp2, servDateFrom, servDateTo, regDateFrom, regDateTo,
                                    invocNoFrom, invocNoTo, batchNoFrom, batchNoTo);

            List<InvoiceViewModel> invo = new List<InvoiceViewModel>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    invo.Add(new InvoiceViewModel
                    {
                        CompId = row["COMP_ID"].ToString(),
                        CompName = row["C_ANAME"].ToString(),
                        StartDate = row["START_DATE"].ToString(),
                        EndDate = row["END_DATE"].ToString(),
                        CountOfBatch = row["COUNT_BATCH"].ToString(),
                        CountOfClaim = row["COUNT_CLAIM"].ToString(),
                        Gross = row["GROSS"].ToString(),
                        Net = row["NET"].ToString(),
                        InvoiceNo = row["INVOICE_NO"].ToString()
                    });
                }              
            }

            ReportDocument rd = new ReportDocument();


            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "CompanyInvoiceReport.rpt"));
            rd.SetDataSource(invo);
            rd.SetParameterValue("comp", compId);
            
            
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
             
            try
            {
                if (typ == 1)
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", "CompanyInvoiceReport-" + DateTime.Now.ToString("ddMMyyyy") + ".pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", "CompanyInvoiceReport-" + DateTime.Now.ToString("ddMMyyyy") + ".xls");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult PrintBatchReviewReport(string compNo, string invocNo, string servFrom, string servTo, string regFrom, string regTo, string batchNo, int typ)
        {
            DataTable dt = new DataTable();

            Int64 batchNoFrom, batchNoTo;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
            regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
            servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
            servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;

            batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
            batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);

            dt = dbData.getBatch(Int64.Parse(compNo), Int64.Parse(invocNo), servDateFrom, servDateTo, regDateFrom, regDateTo, batchNoFrom, batchNoTo);

            List<BatchViewModel> clms = new List<BatchViewModel>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    clms.Add(new BatchViewModel
                    {
                        BatchNumber = row["BATCH_NO"].ToString(),
                        ProviderID = row["PRV_NO"].ToString(),
                        ProviderName = row["PRV_NAME"].ToString(),
                        ProviderType = row["PROVIDER_TYPE"].ToString(),
                        CountOfClaim = row["COUNT_CLAIM"].ToString(),
                        Gross = row["GROSS"].ToString(),
                        Net = row["NET"].ToString()
                    });
                }
            }

            ReportDocument rd = new ReportDocument();


            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "CompanyBatchReport.rpt"));
            rd.SetDataSource(clms);
            rd.SetParameterValue("comp", compNo);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                if (typ == 1)
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", "BatchReviewReport-" + DateTime.Now.ToString("ddMMyyyy") + ".pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", "BatchReviewReport-" + DateTime.Now.ToString("ddMMyyyy") + ".xls");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BatchReview       
        [HttpPost]
        public ActionResult BatchReview(string RegFrom, string RegTo, string ServFrom, string ServTo,
                                        string batchNumber, string providerID, string providerName,
                                        string invoiceNumber, string compNumber, string compName)
        {
            ViewBag.RegFrom = RegFrom;
            ViewBag.RegTo = RegTo;
            ViewBag.ServFrom = ServFrom;
            ViewBag.ServTo = ServTo;
            ViewBag.BatchNumber = batchNumber;
            ViewBag.ProviderID = providerID;
            ViewBag.ProviderName = providerName;
            ViewBag.InvoiceNumber = invoiceNumber;
            ViewBag.CompNumber = compNumber;
            ViewBag.CompName = compName;

            return View();
        }              
        public JsonResult GetClaims(string RegFrom, string RegTo, string ServFrom, string ServTo,
                                    string compId, string aprovNo, string cardId, string invocNo, string batchNo)
        {
            //string compId, string servFrom, string servTo, string regFrom, string regTo,
            //                        string aprovNo, string cardId, string invocNo, string batchNo
            Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;
            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            regDateFrom = string.IsNullOrEmpty(RegFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(RegFrom)).Date;
            regDateTo = string.IsNullOrEmpty(RegTo) ? DateTime.Now.Date : (Convert.ToDateTime(RegTo)).Date;
            servDateFrom = string.IsNullOrEmpty(ServFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(ServFrom)).Date;
            servDateTo = string.IsNullOrEmpty(ServTo) ? DateTime.Now.Date : (Convert.ToDateTime(ServTo)).Date;
           

            aprovNoFrom = string.IsNullOrEmpty(aprovNo) ? 0 : Convert.ToInt64(aprovNo);
            aprovNoTo = string.IsNullOrEmpty(aprovNo) ? 999999999999999999 : Convert.ToInt64(aprovNo);
            invocNoFrom = string.IsNullOrEmpty(invocNo) ? 0 : Convert.ToInt64(invocNo);
            invocNoTo = string.IsNullOrEmpty(invocNo) ? 999999999999999999 : Convert.ToInt64(invocNo);
            batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
            batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);

            int comp = Convert.ToInt32(compId);

            DataTable dt = new DataTable();

            dt = dbData.getClaims(regDateFrom, regDateTo, servDateFrom, servDateTo, comp, aprovNoFrom, aprovNoTo, cardId, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo);

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
                        Diagnosis = row["DIAGNOSIS"].ToString(),
                        ServType = row["SERV_TYPE"].ToString(),
                        Gross = row["GROSS"].ToString(),
                        Net = row["NET"].ToString()                       
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
        public JsonResult GetActiveEmployess(string search, int page)
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (User.IsInRole("HR_Admin"))
            {
                var companyId = db.HrAdminCompanies.Where(c => c.UserId == CurrentUser.Id).Select(c => c.CompId).ToList();
                if (companyId[0] == "All")
                {
                    int compId = int.Parse(search.Split('-')[0]);
                    int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                    var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "N", search)
                                     .Select(c => new
                                     {
                                         id = c.id,
                                         text = c.text
                                     }).ToList();
                    return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    if (companyId.Contains(search.Split('-')[0]))
                    {
                        int compId = int.Parse(search.Split('-')[0]);
                        int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                        var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "N", search)
                                         .Select(c => new
                                         {
                                             id = c.id,
                                             text = c.text
                                         }).ToList();
                        return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }

            else if (User.IsInRole("Admin"))
            {
                int compId = int.Parse(search.Split('-')[0]);
                int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "N", search)
                                 .Select(c => new
                                 {
                                     id = c.id,
                                     text = c.text
                                 }).ToList();


                return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                int Provider = Convert.ToInt32(CurrentUser.Provider);
                int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == Provider).Max(x => x.CONTRACT_NO);
                var Employees = db.fn_GetEmployessForCompany(Provider, maxcontract, "N", search)
                   .Select(c => new
                   {
                       id = c.id,
                       text = c.text
                   }).ToList();

                return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }

        }
        //public ActionResult PrintAllClaims(string compId, string servFrom, string servTo, string regFrom, string regTo,
        //                                    string aprovNo, string cardId, string invocNo, string batchNo)
        //{
        //    Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;

        //    DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

        //    string cardStart, cardEnd;

        //    regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
        //    regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
        //    servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
        //    servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;


        //    aprovNoFrom = string.IsNullOrEmpty(aprovNo) ? 0 : Convert.ToInt64(aprovNo);
        //    aprovNoTo = string.IsNullOrEmpty(aprovNo) ? 999999999999999999 : Convert.ToInt64(aprovNo);
        //    invocNoFrom = string.IsNullOrEmpty(invocNo) ? 0 : Convert.ToInt64(invocNo);
        //    invocNoTo = string.IsNullOrEmpty(invocNo) ? 999999999999999999 : Convert.ToInt64(invocNo);
        //    batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
        //    batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);

        //    cardStart = string.IsNullOrEmpty(cardId) || cardId == "null" ? " " : cardId;
        //    cardEnd = string.IsNullOrEmpty(cardId) || cardId == "null" ? "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz" : cardId;

        //    int comp = Convert.ToInt32(compId);

        //    ReportDocument rd = new ReportDocument();

        //    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReviewClaimsReport.rpt"));

        //    rd.SetDatabaseLogon("APP", "15+08+2017");


        //    rd.SetParameterValue("reg1", regDateFrom);
        //    rd.SetParameterValue("reg2", regDateTo);
        //    rd.SetParameterValue("serv1", servDateFrom);
        //    rd.SetParameterValue("serv2", servDateTo);


        //    rd.SetParameterValue("cmp", comp);
        //    rd.SetParameterValue("crd1", cardStart);
        //    rd.SetParameterValue("crd2", cardEnd);

        //    rd.SetParameterValue("aprov1", aprovNoFrom);
        //    rd.SetParameterValue("aprov2", aprovNoTo);
        //    rd.SetParameterValue("invoc1", invocNoFrom);
        //    rd.SetParameterValue("invoc2", invocNoTo);

        //    rd.SetParameterValue("batch1", batchNoFrom);
        //    rd.SetParameterValue("batch2", batchNoTo);

        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    try
        //    {
        //        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
        //        stream.Seek(0, SeekOrigin.Begin);
        //        rd.Close();
        //        rd.Dispose();
        //        GC.Collect();
        //        return File(stream, "application/xls", compId + "AllClaims" + DateTime.Now.ToString("ddMMyyyy") + ".xls");

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReviewCliamsNew.rpt"));

            rd.SetDatabaseLogon("APP", "15+08+2017");


            //rd.SetParameterValue("reg1", regDateFrom);
            //rd.SetParameterValue("reg2", regDateTo);
            //rd.SetParameterValue("serv1", servDateFrom);
            //rd.SetParameterValue("serv2", servDateTo);


            //rd.SetParameterValue("cmp", comp);
            rd.SetParameterValue("crd1", cardStart);
            rd.SetParameterValue("crd2", cardEnd);

            rd.SetParameterValue("aprov1", aprovNoFrom);
            rd.SetParameterValue("aprov2", aprovNoTo);
            //rd.SetParameterValue("invoc1", invocNoFrom);
            //rd.SetParameterValue("invoc2", invocNoTo);

            //rd.SetParameterValue("batch1", batchNoFrom);
            //rd.SetParameterValue("batch2", batchNoTo);

            rd.SetParameterValue("btch", batchNoFrom);

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
        public JsonResult getClaimsCardData(string CardId)
        {
            DataTable dt = new DataTable();

            List<ClaimsCardDetails> clmD = new List<ClaimsCardDetails>();

            dt = dbOra.RunReader(@"SELECT CLAIM_NO,  to_char(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE, BATCH_NO, PRV_NAME, SERV_NAME, GROSS, NET FROM APP.REVIEW_CLAIMS_NEW WHERE CARD_NO = '" + CardId + "'");

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    clmD.Add(new ClaimsCardDetails
                    {
                        Claim_No = row["CLAIM_NO"].ToString(),
                        Claim_Date = row["CLAIM_DATE"].ToString(),
                        Batch_No = row["BATCH_NO"].ToString(),
                        Provider_Name = row["PRV_NAME"].ToString(),
                        Serv_Name = row["SERV_NAME"].ToString(),
                        Gross = row["GROSS"].ToString(),
                        Net = row["NET"].ToString()
                    });
                }
                return new JsonResult { Data = new { clmD }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
                return new JsonResult { Data = new { clmD }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public ActionResult PrintReviewClaimsNew(string compId, string aprovNo, string cardId, string invocNo, string batchNo, int typ)
        {
            Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            string cardStart, cardEnd;

            regDateFrom = new DateTime(2020, 1, 1);
            regDateTo = DateTime.Now.Date;
            servDateFrom = new DateTime(2017, 1, 1);
            servDateTo = DateTime.Now.Date;


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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReviewCliamsNew.rpt"));

            rd.SetDatabaseLogon("APP", "15+08+2017");


            rd.SetParameterValue("crd1", cardStart);
            rd.SetParameterValue("crd2", cardEnd);

            rd.SetParameterValue("aprov1", aprovNoFrom);
            rd.SetParameterValue("aprov2", aprovNoTo);
            rd.SetParameterValue("cmp", comp);
            rd.SetParameterValue("btch", batchNoFrom);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //return File(stream, "application/pdf", compId + "AllClaimsSummary" + DateTime.Now.ToString("ddMMyyyy") + ".pdf");

            try
            {
                if (typ == 1)
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", "ClaimsReviewReport-" + DateTime.Now.ToString("ddMMyyyy") + ".pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", "ClaimsReviewReport-" + DateTime.Now.ToString("ddMMyyyy") + ".xls");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult PrintPdf(string compId, string aprovNo, string cardId, string invocNo, string batchNo)
        {
            Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            string cardStart, cardEnd;

            regDateFrom = new DateTime(2020, 1, 1);
            regDateTo = DateTime.Now.Date;
            servDateFrom = new DateTime(2017, 1, 1) ;
            servDateTo = DateTime.Now.Date;


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

            rd.SetDatabaseLogon("APP", "15+08+2017");


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
        public ActionResult PrintExcel(string compId, string aprovNo, string cardId, string invocNo, string batchNo)
        {
            Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;

            DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

            string cardStart, cardEnd;

            regDateFrom = new DateTime(2020, 1, 1);
            regDateTo = DateTime.Now.Date;
            servDateFrom = new DateTime(2017, 1, 1);
            servDateTo = DateTime.Now.Date;

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

            rd.SetDatabaseLogon("APP", "15+08+2017");


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
        public ActionResult PrintRoshita(string claimNo)
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "OneClaimReport.rpt"));

            rd.SetDatabaseLogon("APP", "15+08+2017");

            rd.SetParameterValue("clm", claimNo);
            
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
                return File(stream, "application/pdf", DateTime.Now.ToString("ddMMyyyy") + "-Roshita.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DownloadClaim(string claimNo)
        {
            string fileName = claimNo + ".pdf";
            string filePath = Server.MapPath("~/Reports/HR/File/" + fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return Json(new { success = false, message = "File not found" }, JsonRequestBehavior.AllowGet);
            }

            return File(filePath, "application/pdf", fileName);

            //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            //string contentType = MimeMapping.GetMimeMapping(fileName); 

            //return File(fileBytes, contentType, fileName);
        }
        public ActionResult DownloadBatch(string batchNo)
        {
            string fileName = batchNo + ".pdf";
            string filePath = Server.MapPath("~/Reports/HR/File/Batch/" + fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return Json(new { success = false, message = "File not found" }, JsonRequestBehavior.AllowGet);
            }

            return File(filePath, "application/pdf", fileName);

            //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            //string contentType = MimeMapping.GetMimeMapping(fileName); 

            //return File(fileBytes, contentType, fileName);
        }
        //[HttpPost]
        //public ActionResult RedirectToBatchReview(string batchNumber, string providerId, string providerName, string invoiceNumber, string compNumber, string compName)
        //{
        //    // Construct the URL dynamically based on parameters
        //    var redirectUrl = Url.Action("BatchReview", "ReviewClaims", new
        //    {
        //        batchNumber = batchNumber,
        //        providerId = providerId,
        //        providerName = providerName,
        //        invoiceNumber = invoiceNumber,
        //        compNumber = compNumber,
        //        compName = compName
        //    });

        //    // Return the redirect URL as part of the response
        //    return Json(new { redirectUrl = redirectUrl });
        //}

        //public JsonResult GetClaims(string compId, string servFrom, string servTo, string regFrom, string regTo,
        //                            string aprovNo, string cardId, string invocNo, string batchNo)
        //{
        //    //string compId, string servFrom, string servTo, string regFrom, string regTo,
        //    //                        string aprovNo, string cardId, string invocNo, string batchNo

        //    Int64 aprovNoFrom, aprovNoTo, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo;

        //    DateTime regDateFrom, regDateTo, servDateFrom, servDateTo;

        //    regDateFrom = string.IsNullOrEmpty(regFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(regFrom)).Date;
        //    regDateTo = string.IsNullOrEmpty(regTo) ? DateTime.Now.Date : (Convert.ToDateTime(regTo)).Date;
        //    servDateFrom = string.IsNullOrEmpty(servFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(servFrom)).Date;
        //    servDateTo = string.IsNullOrEmpty(servTo) ? DateTime.Now.Date : (Convert.ToDateTime(servTo)).Date;


        //    aprovNoFrom = string.IsNullOrEmpty(aprovNo) ? 0 : Convert.ToInt64(aprovNo);
        //    aprovNoTo = string.IsNullOrEmpty(aprovNo) ? 999999999999999999 : Convert.ToInt64(aprovNo);
        //    invocNoFrom = string.IsNullOrEmpty(invocNo) ? 0 : Convert.ToInt64(invocNo);
        //    invocNoTo = string.IsNullOrEmpty(invocNo) ? 999999999999999999 : Convert.ToInt64(invocNo);
        //    batchNoFrom = string.IsNullOrEmpty(batchNo) ? 0 : Convert.ToInt64(batchNo);
        //    batchNoTo = string.IsNullOrEmpty(batchNo) ? 999999999999999999 : Convert.ToInt64(batchNo);

        //    int comp = Convert.ToInt32(compId);

        //    DataTable dt = new DataTable();

        //        dt = dbData.getClaims(comp, servDateFrom, servDateTo, regDateFrom, regDateTo, aprovNoFrom, aprovNoTo, 
        //                              cardId, invocNoFrom, invocNoTo, batchNoFrom, batchNoTo);

        //    List<ClaimsViewModel> clms = new List<ClaimsViewModel>();

        //    if (dt.Rows.Count != 0)
        //    {
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            clms.Add(new ClaimsViewModel
        //            {
        //                ClaimNo = row["CLAIM_NO"].ToString(),
        //                CreatedDate = row["CREATED_DATE"].ToString(),
        //                ClaimDate = row["CLAIM_DATE"].ToString(),
        //                CardNo = row["CARD_NO"].ToString(),
        //                EmpName = row["EMP_NAME"].ToString(),
        //                ProvName = row["PRV_NAME"].ToString(),
        //                ProvType = row["PROVIDER_TYPE"].ToString(),
        //                Diagnosis = row["DIAGNOSIS"].ToString(),
        //                ServType = row["SERV_TYPE"].ToString()                        
        //            });
        //        }
        //        return new JsonResult { Data = new { claimslist = clms, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //    }
        //    else
        //        return new JsonResult { Data = new { claimslist = clms, msg = "empty" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //}

        #endregion


    }
}