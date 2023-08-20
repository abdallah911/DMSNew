using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel.CustomerService;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Globalization;
using System.Threading;

namespace DMS_Authontication1.Controllers.ReportMain
{

    public class ReportMainController : Controller
    {
        #region Fields

        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationDbContext db2 = new ApplicationDbContext();
        #endregion

        #region Reports1               
        public ActionResult Index()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                COMP_ID = c.C_COMP_ID,
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "COMP_ID", "Name");
            ViewBag.company = companylist;


            var usersname = db2.Users.Where(u => u.UserName != "").Select(c => new
            {
                user = c.UserName,
                Name = c.FName + " " + c.LName

            }).Distinct().ToList();
            SelectList userslist = new SelectList(usersname, "user", "Name");
            ViewBag.users = userslist;


            var medcodall = db.RoshitaDetails.Select(c => new
            {
                Code = c.MedicienCode,
                Name = c.MedicienCode + " || " + c.MedicienName

            }).Distinct().ToList();
            SelectList medcodlist = new SelectList(medcodall, "Code", "Name");
            ViewBag.medall = medcodlist;

            return View();
        }
        public ActionResult PrintReportsMedPdf(string Regfrom, string Regto, string CopmanyNumber,
                                            string usernam, string medcod, string type, string RepotType,
                                            string crd, string typmngr)
        {
            DateTime RegDateFrom, RegDateTo;

            RegDateFrom = string.IsNullOrEmpty(Regfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = string.IsNullOrEmpty(Regto) ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;



            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {

                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ChronicDataDetails.rpt"));
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "EditMedicine2.rpt"));
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineSummary.rpt"));
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RepeatDispensingMedicine.rpt"));
                    break;
                default:
                    return View();
            }

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@from", RegDateFrom);
            rd.SetParameterValue("@to", RegDateTo);
            rd.SetParameterValue("@comp", CopmanyNumber);
            rd.SetParameterValue("@medcod", medcod);
            rd.SetParameterValue("@usernam", usernam);
            rd.SetParameterValue("@crd", crd);
            rd.SetParameterValue("@typ", type);

            if (RepotType != "1")
                rd.SetParameterValue("@typmngr", typmngr);


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
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "ReportMed.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PrintReportsMedExcel(string Regfrom, string Regto, string CopmanyNumber,
                                           string usernam, string medcod, string type, string RepotType,
                                           string crd, string typmngr)
        {
            DateTime RegDateFrom, RegDateTo;

            RegDateFrom = string.IsNullOrEmpty(Regfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = string.IsNullOrEmpty(Regto) ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;



            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {

                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ChronicDataDetails.rpt"));
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "EditMedicine2.rpt"));
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineSummary.rpt"));
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RepeatDispensingMedicine.rpt"));
                    break;
                default:
                    return View();
            }


            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");


            rd.SetParameterValue("@from", RegDateFrom);
            rd.SetParameterValue("@to", RegDateTo);
            rd.SetParameterValue("@comp", CopmanyNumber);
            rd.SetParameterValue("@medcod", medcod);
            rd.SetParameterValue("@usernam", usernam);
            rd.SetParameterValue("@crd", crd);
            rd.SetParameterValue("@typ", type);

            if (RepotType != "1")
                rd.SetParameterValue("@typmngr", typmngr);


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
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "ReportMed.xls");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public JsonResult GetEmployessReport(string search, int page, Int32 cmp)
        {
            long lgSearch;
            long.TryParse(search, out lgSearch);
            //db.fn_GetEmployessForCompany(Provider, maxcontract, "N", search)
            var Employees = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now && x.TERMINATE_FLAG == "N")
                /*.AsEnumerable()*/.Where(x => x.C_COMP_ID == cmp && x.CARD_ID.Contains(search) /*|| x.EMP_ENAME.Contains(search)*/)
                .Select(c => new
                {
                    id = c.CARD_ID,
                    text = c.CARD_ID + " || " + c.EMP_ENAME
                }).ToList();

            return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #endregion

        #region Reports2
        public ActionResult ReportMed2()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                COMP_ID = c.C_COMP_ID,
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "COMP_ID", "Name");
            ViewBag.company = companylist;
            
            return View();
        }

        public ActionResult PrintReports2MedPdf(string Regfrom, string Regto, string CopmanyNumber,
                                                string crd, string RepotType)
        {
            DateTime RegDateFrom, RegDateTo;

            RegDateFrom = string.IsNullOrEmpty(Regfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = string.IsNullOrEmpty(Regto) ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;



            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {

                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineChronicDetails.rpt"));
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineChronicSummary.rpt"));
                    break;
                    
                default:
                    return View();
            }

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@from", RegDateFrom);
            rd.SetParameterValue("@to", RegDateTo);
            rd.SetParameterValue("@comp", CopmanyNumber);          
            rd.SetParameterValue("@crd", crd);            


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
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "ReportMed.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PrintReports2MedExcel(string Regfrom, string Regto, string CopmanyNumber,
                                                  string crd, string RepotType)
        {
            DateTime RegDateFrom, RegDateTo;

            RegDateFrom = string.IsNullOrEmpty(Regfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = string.IsNullOrEmpty(Regto) ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;



            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {

                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineChronicDetails.rpt"));
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineChronicSummary.rpt"));
                    break;

                default:
                    return View();
            }

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@from", RegDateFrom);
            rd.SetParameterValue("@to", RegDateTo);
            rd.SetParameterValue("@comp", CopmanyNumber);
            rd.SetParameterValue("@crd", crd);


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
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "ReportMed.xls");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region Report3
        public ActionResult CompanyChronicReport(string Date = "", int CompId = 0, int GroupId = 0, string CardId = "")
        {
            
            DateTime DispenseDate = Convert.ToDateTime(Date);

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CompanyChronicReport.rpt"));

            //ApplicationDbContext users = new ApplicationDbContext();
            //var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //int ProviderId = Convert.ToInt32(CurrentUser.Provider);

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@CompId", CompId);
            rd.SetParameterValue("@CardId", CardId);
            rd.SetParameterValue("@DispenseDate", DispenseDate.ToShortDateString());
            rd.SetParameterValue("@GroupId", GroupId);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            rd.Close();
            rd.Dispose();
            GC.Collect();
            return File(stream, "application/pdf", Date + "CompanyChronicReport.pdf");



            //DateTime DispenseDate = Convert.ToDateTime(Date).Date;

            //ReportDocument rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CompanyChronicReport.rpt"));

            //rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            //rd.SetParameterValue("@CompId", CompId);
            //rd.SetParameterValue("@CardId", CardId);
            //rd.SetParameterValue("@DispenseDate", DispenseDate);
            //rd.SetParameterValue("@GroupId", GroupId);

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //rd.Close();
            //rd.Dispose();
            //GC.Collect();
            //return File(stream, "application/pdf", Date + "CompanyChronicDelivery.pdf");


        }



        #endregion

        #region Report Print
        public ActionResult ReportPrint()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                COMP_ID = c.C_COMP_ID,
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "COMP_ID", "Name");
            ViewBag.company = companylist;

            return View();
        }

        public JsonResult GetContractNo(string CompId)
        {
            int comp = int.Parse(CompId);
           
            var contractNo = db.Contract_Data.Where(u => u.C_COMP_ID == comp).Select(c => new
            {
                ContractNo = c.CONTRACT_NO,               

            }).OrderBy(u => u.ContractNo).ToList();
            SelectList contractNumberList = new SelectList(contractNo, "ContractNo", "ContractNo");


            return Json(contractNumberList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintReport(Int32 CompId, int contract, string typ)
        {            
            ReportDocument rd = new ReportDocument();


            if(typ == "Large")
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ReportPrintHorizontal4.rpt"));
            else if (typ == "Medium")
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ReportPrintHorizontal5.rpt"));
            else if (typ == "Small")
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ReportPrintHorizontal6.rpt"));
            else if (typ == "Mini")
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ReportPrintHorizontal88.rpt"));


            rd.SetDatabaseLogon("APP", "12369");
            
            rd.SetParameterValue("cmp", CompId);
            rd.SetParameterValue("contr", contract);
          

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            rd.Close();
            rd.Dispose();
            GC.Collect();
            return File(stream, "application/pdf", "PrintPreview-" + CompId.ToString() + ".pdf");
        }

        #endregion

        #region Reports
        public ActionResult Reports()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                COMP_ID = c.C_COMP_ID,
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "COMP_ID", "Name");
            ViewBag.company = companylist;

            return View();
        }
     
        public ActionResult PrintReportsPdf(string ServiceFrom, string ServiceTo, string CopmanyNumber, string RepotType)
        {
            DateTime ServiceDateFrom, ServiceDateTo;

            ServiceDateFrom = string.IsNullOrEmpty(ServiceFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(ServiceFrom)).Date;
            ServiceDateTo = string.IsNullOrEmpty(ServiceTo) ? DateTime.Now.Date : (Convert.ToDateTime(ServiceTo)).Date;



            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {

                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "ActivationReport.rpt"));
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "SartEmpReport.rpt"));
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "CloseEmpReport.rpt"));
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "ActiveMedicineChronicReport.rpt"));
                    break;
                case 5:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "AllClaimsReport.rpt"));
                    break;
                //case 2:
                //    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineChronicSummary.rpt"));
                //    break;

                default:
                    return View();
            }

            //rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");



            if (Convert.ToInt32(RepotType) > 4)
            {
                rd.SetDatabaseLogon("APP", "12369");

                rd.SetParameterValue("comp", CopmanyNumber);
                rd.SetParameterValue("dat1", ServiceDateFrom);
                rd.SetParameterValue("dat2", ServiceDateTo);
            }
            else if (Convert.ToInt32(RepotType) == 4)
            {
                rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

                rd.SetParameterValue("@comp", CopmanyNumber);
            }
            else
            {
                rd.SetDatabaseLogon("APP", "12369");

                rd.SetParameterValue("comp", CopmanyNumber);
            }
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
                return File(stream, "application/pdf", DateTime.Now.ToString("ddMMyyyy") + "Reports.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PrintReportsExcel(string ServiceFrom, string ServiceTo, string CopmanyNumber, string RepotType)
        {
            DateTime ServiceDateFrom, ServiceDateTo;

            ServiceDateFrom = string.IsNullOrEmpty(ServiceFrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(ServiceFrom)).Date;
            ServiceDateTo = string.IsNullOrEmpty(ServiceTo) ? DateTime.Now.Date : (Convert.ToDateTime(ServiceTo)).Date;


            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {

                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "ActivationReport.rpt"));
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "SartEmpReport.rpt"));
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "CloseEmpReport.rpt"));
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "ActiveMedicineChronicReport.rpt"));
                    break;
                case 5:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/SoficoReports"), "AllClaimsReport.rpt"));
                    break;
                //case 2:
                //    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AuditMedicineChronicSummary.rpt"));
                //    break;

                default:
                    return View();
            }

            if (Convert.ToInt32(RepotType) > 4)
            {
                rd.SetDatabaseLogon("APP", "12369");

                rd.SetParameterValue("comp", CopmanyNumber);
                rd.SetParameterValue("dat1", ServiceDateFrom);
                rd.SetParameterValue("dat2", ServiceDateTo);
            }
            else if (Convert.ToInt32(RepotType) == 4)
            {
                rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

                rd.SetParameterValue("@comp", CopmanyNumber);
            }
            else
            {
                rd.SetDatabaseLogon("APP", "12369");

                rd.SetParameterValue("comp", CopmanyNumber);
            }


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
                return File(stream, "application/xls", DateTime.Now.ToString("ddMMyyyy") + "Reports.xls");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}