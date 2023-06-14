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
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "EditMedicine.rpt"));
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
            rd.SetParameterValue("@typmngr", typmngr);
            rd.SetParameterValue("@crd", crd);
            rd.SetParameterValue("@typ", type);


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
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "EditMedicine.rpt"));
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
            rd.SetParameterValue("@typmngr", typmngr);
            rd.SetParameterValue("@crd", crd);
            rd.SetParameterValue("@typ", type);


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
    }
}