using DMS_Authontication1.Models;
using System;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace DMS_TEST.Controllers
{

    public class ConsumptionAdminController : Controller
    {
        #region Properties
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        #endregion

        #region CTO
        public ConsumptionAdminController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();
        }
        #endregion

        #region Actions
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintConsumption(string From, string To, string CardId)
        {
            try
            {

                DateTime F = Convert.ToDateTime(From);
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);//to get all pervious day
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Consumption"), "ConsumptionofCard.rpt"));
                rd.SetDatabaseLogon("APP", "12369");
                rd.SetParameterValue("SRDA1", F);
                rd.SetParameterValue("SRDA2", T);
                rd.SetParameterValue("CRD", CardId);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", F.ToString("ddMMyyyy") + "Clams.pdf");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");
                // throw ex.Message("There are no calmes");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult HighAdminConsumptionReports()
        {
            var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
            SelectList companylist = new SelectList(companyname, "Code", "Name");
            ViewBag.company = companylist;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult NormalAdminConsumptionReports()
        {
            var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
            SelectList companylist = new SelectList(companyname, "Code", "Name");
            ViewBag.company = companylist;
            return View();
        }


        [Authorize(Roles = "HR,HR_Admin")]
        public ActionResult HrConsumptionReports()
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
                string compa = UserDB.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                return View();
            }
        }

        public ActionResult PrintConsumptionReports(string Regfrom, string Regto, string Serfrom, string Serto
            , string CompanyFrom, string CompanyTo, string ProviderName, string CardFrom
            , string CardTo, string RepotType)
        {
            Int64 CompanyStart, CompanyEnd,ProviderNumber1, ProviderNumber2;
            string CardStart, CardEnd;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;
            CompanyStart = string.IsNullOrEmpty(CompanyFrom)  ? 0 : Convert.ToInt64(CompanyFrom);
            CompanyEnd =  string.IsNullOrEmpty(CompanyTo) ? 999999999 : Convert.ToInt64(CompanyTo);
            ProviderNumber1 = ProviderName=="null" ? 0 : Convert.ToInt64(ProviderName);
            ProviderNumber2 = ProviderName == "null" ? 999999999 : Convert.ToInt64(ProviderName);
            CardStart = CardFrom=="null" ? "0" : CardFrom;
            CardEnd = CardTo=="null" ? "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz" : CardTo;



            RegDateFrom = string.IsNullOrEmpty(Regfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = string.IsNullOrEmpty(Regto) ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = string.IsNullOrEmpty(Serfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = string.IsNullOrEmpty(Serto) ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;

            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 901:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum901.rpt"));
                    break;
                case 9011:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum901-1.rpt"));
                    break;

                case 902:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum902.rpt"));
                    break;

                case 903:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum903.rpt"));
                    break;
                case 904:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum904.rpt"));
                    break;
                case 905:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum905.rpt"));
                    break;
                case 906:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum906.rpt"));
                    break;
                case 907:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum907.rpt"));
                    break;
                case 908:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum908.rpt"));
                    break;
                case 909:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum909.rpt"));
                    break;
                case 910:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum910.rpt"));
                    break;
                case 911:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum911.rpt"));
                    break;
                case 912:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum912.rpt"));
                    break;
                case 913:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum913.rpt"));
                    break;
                case 914:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum914.rpt"));
                    break;
                case 915:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum915.rpt"));
                    break;
                case 916:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum916.rpt"));
                    break;
                case 917:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum917.rpt"));
                    break;
                case 918:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum918.rpt"));
                    break;
                default:
                    return View();
            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("crda1", RegDateFrom);
            rd.SetParameterValue("crda2", RegDateTo);
            rd.SetParameterValue("srda1", SerDateFrom);
            rd.SetParameterValue("srda2", SerDateTo);
            rd.SetParameterValue("comp1", CompanyStart);
            rd.SetParameterValue("comp2", CompanyEnd);
            rd.SetParameterValue("crd1", CardStart);
            rd.SetParameterValue("crd2", CardEnd);
            rd.SetParameterValue("cls1", "0");
            rd.SetParameterValue("cls2", "zzzzzzzzz");
            rd.SetParameterValue("PRV1", ProviderNumber1);
            rd.SetParameterValue("PRV2", ProviderNumber2);
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
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "Consumption.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Helper Methods

        public JsonResult GetProviderUsers(string search, int page)
        {
            int intSearch;
            int.TryParse(search, out intSearch);

            var ProviderUsers = db.Serv_Providers1.Where(x => x.PR_CODE == intSearch || x.PR_ENAME.Contains(search) || x.PR_ANAME.Contains(search))
                .Select(c => new
                {
                    id = c.PR_CODE,
                    text = c.PR_CODE + " || " + c.PR_ENAME
                }).ToList();
            return new JsonResult { Data = ProviderUsers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion
    }
}