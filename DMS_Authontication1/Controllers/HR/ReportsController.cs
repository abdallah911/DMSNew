using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR,HR_Admin")]
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class ReportsController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext myEntities = new ApplicationDbContext();
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

        public ReportsController()
        {
            db = new DMS_TESTEntities();
        }

        #region Reports View
        public async Task<ActionResult> Index()
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

               // var user = await UserManager.FindByNameAsync(User.Identity.Name);
               // bool found = false;
               // var provider = int.Parse(user.Provider);
               // var CurrentDate = DateTime.Now.Date;
               // var company = db.Contract_Data.Where(c => c.C_COMP_ID == provider && c.DATE_FROM <= CurrentDate
               //&& c.DATE_TO >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
               // if (company != null)
               // {
               //     var GetServActive = db.COMP_CUSTOMIZED_D.Where(p => p.C_COMP_ID == provider && p.CONTRACT_NO == company.CONTRACT_NO
               //       && p.SERV_CODE == "12").FirstOrDefault();
               //     if (GetServActive != null)
               //         found = true;
               // }
               // ViewBag.IsIndemnity = found;

                return View();
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                string compa = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.compnum = compa;
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }
        public ActionResult Inquiries()
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
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }
        public ActionResult StaffConsumption()
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
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }

        public ActionResult Consumption()
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
                if (compa.Contains("500") || compa.Contains("800"))
                {
                    return View();
                }
                return View("~/Views/Reports/Premium.cshtml");
            }
        }


        public ActionResult ConsumptionPremium()
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




        #endregion

        #region Print Reports
        /// <summary>
        /// For print MedicalServices report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="Claimfrom"></param>
        /// <param name="Claimto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServNum"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintMedicalServices(string Regfrom, string Regto, string Serfrom, string Serto
            , string Claimfrom, string Claimto, string CompNum, string ServNum, string ButtType)
        {
            Int64 ClaimStart, ClaimEnd, CompNumber, ServNum1, ServNum2;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            ClaimStart = Claimfrom == string.Empty ? 0 : Convert.ToInt64(Claimfrom);
            ClaimEnd = Claimto == string.Empty ? 999999999999999999 : Convert.ToInt64(Claimto);
            CompNumber = Convert.ToInt64(CompNum);

            ServNum1 = ServNum == string.Empty ? 0 : Convert.ToInt64(ServNum);
            ServNum2 = ServNum == string.Empty ? 9999999999999 : Convert.ToInt64(ServNum);

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "MedicalServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEob.rpt"));
            }
            else if (ButtType == "OtherServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEobService.rpt"));
            }
            else if (ButtType == "SerOutMediAuth")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEobServiceOut.rpt"));

            }


            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("seda1", SerDateFrom);
            rd.SetParameterValue("seda2", SerDateTo);
            rd.SetParameterValue("crda1", RegDateFrom);
            rd.SetParameterValue("crda2", RegDateTo);
            rd.SetParameterValue("clno1", ClaimStart);
            rd.SetParameterValue("clno2", ClaimEnd);
            rd.SetParameterValue("comp", CompNumber);
            rd.SetParameterValue("prv1", ServNum1);
            rd.SetParameterValue("prv2", ServNum2);


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
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "MedicalServices.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// For Print MedicalServices Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="Claimfrom"></param>
        /// <param name="Claimto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServNum"></param>
        /// <returns>Report as EXCEL</returns>
        public ActionResult PrintXlxMedicalServices(string Regfrom, string Regto, string Serfrom, string Serto
            , string Claimfrom, string Claimto, string CompNum, string ServNum, string ButtType)
        {
            Int64 ClaimStart, ClaimEnd, CompNumber, ServNum1, ServNum2;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            ClaimStart = Claimfrom == string.Empty ? 0 : Convert.ToInt64(Claimfrom);
            ClaimEnd = Claimto == string.Empty ? 999999999999999999 : Convert.ToInt64(Claimto);
            CompNumber = Convert.ToInt64(CompNum);

            ServNum1 = ServNum == string.Empty ? 0 : Convert.ToInt64(ServNum);
            ServNum2 = ServNum == string.Empty ? 9999999999999 : Convert.ToInt64(ServNum);

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "MedicalServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEob.rpt"));
            }
            else if (ButtType == "OtherServices")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "PReportEobService.rpt"));
            }
            else if (ButtType == "SerOutMediAuth")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEobServiceOut.rpt"));
            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("seda1", SerDateFrom);
            rd.SetParameterValue("seda2", SerDateTo);
            rd.SetParameterValue("crda1", RegDateFrom);
            rd.SetParameterValue("crda2", RegDateTo);
            rd.SetParameterValue("clno1", ClaimStart);
            rd.SetParameterValue("clno2", ClaimEnd);
            rd.SetParameterValue("comp", CompNumber);
            rd.SetParameterValue("prv1", ServNum1);
            rd.SetParameterValue("prv2", ServNum2);

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
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "MedicalServices.xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// For print Staff Consumption report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="CardNum"></param>
        /// <param name="ButtType"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintEmployees(string Regfrom, string Regto
            , string CompNum, string CardNum, string ButtType)
        {
            Int32 CompNumber;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "Active")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmp.rpt"));
            }
            else if (ButtType == "Closed")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpY.rpt"));
            }


            rd.SetDatabaseLogon("APP", "12369");

            rd.SetDatabaseLogon("APP", "12369");
            rd.SetParameterValue("comp", CompNumber);
            rd.SetParameterValue("rel1", 0);
            rd.SetParameterValue("rel2", 99999999);

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
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "Employees.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// For Print Staff Consumption Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="CardNum"></param>
        /// <param name="ButtType"></param>
        /// <returns>Report as EXCEL</returns>
        public ActionResult PrintXlxEmployees(string Regfrom, string Regto
            , string CompNum, string CardNum, string ButtType)
        {

            Int32 CompNumber;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();
            if (ButtType == "Active")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmp.rpt"));
            }
            else if (ButtType == "Closed")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportEmpY.rpt"));
            }


            rd.SetDatabaseLogon("APP", "12369");

            rd.SetDatabaseLogon("APP", "12369");
            rd.SetParameterValue("comp", CompNumber);
            rd.SetParameterValue("rel1", 0);
            rd.SetParameterValue("rel2", 99999999);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                // 
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Employees.xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// For print Staff Consumption report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServiceType"></param>
        /// <param name="CardNum"></param>
        /// <param name="Family"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintEmployeesConsumption(string Regfrom, string Regto
            , string CompNum, string ServiceType, string CardNum, string Family)
        {
            Int32 CompNumber, grp1, grp2;
            string card;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);
            card = CardNum;

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(ServiceType))
            {
                case 1:
                    grp1 = 1016;
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHr.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
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
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 2:
                    grp1 = 1014;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
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
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 3:
                    grp1 = 1009;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
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
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 4:
                    grp1 = 116;
                    grp2 = 1013;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp2);
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
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 5:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrPrvIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
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
                        return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.pdf");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                default:
                    return View();


            }

        }


        /// <summary>
        /// For Print Staff Consumption Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="CompNum"></param>
        /// <param name="ServiceType"></param>
        /// <param name="CardNum"></param>
        /// <param name="Family"></param>
        /// <returns>Report as Excel </returns>
        public ActionResult PrintXlxEmployeesConsumption(string Regfrom, string Regto
            , string CompNum, string ServiceType, string CardNum, string Family)
        {

            Int32 CompNumber, grp1, grp2;
            string card;
            DateTime RegDateFrom, RegDateTo;

            CompNumber = CompNum == string.Empty ? 0 : Convert.ToInt32(CompNum);
            card = CardNum;

            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;

            ReportDocument rd = new ReportDocument();
            switch (Convert.ToInt32(ServiceType))
            {
                case 1:
                    grp1 = 1016;
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHr.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 2:
                    grp1 = 1014;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 3:
                    grp1 = 1009;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp1);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 4:
                    grp1 = 116;
                    grp2 = 1013;


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
                    rd.SetParameterValue("grp1", grp1);
                    rd.SetParameterValue("grp2", grp2);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 5:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "ReportHrPrvIRS.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("srda1", RegDateFrom);
                    rd.SetParameterValue("srda2", RegDateTo);
                    rd.SetParameterValue("comp", CompNumber);
                    rd.SetParameterValue("crd", card);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "EmployeesConsumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                default:
                    return View();


            }
        }





        /// <summary>
        /// For print Consumption report as PDF
        /// </summary>
        /// <param name="Regfrom"></param>
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="CompanyFrom"></param>
        /// <param name="CompanyTo"></param>
        /// <param name="ClassFrom"></param>
        /// <param name="ClassTo"></param>
        /// <param name="CardFrom"></param>
        /// <param name="CardTo"></param>
        /// <param name="ContractNumber"></param>
        /// <param name="RepotType"></param>
        /// <param name="larg"></param>
        /// <param name="smal"></param>
        /// <param name="Percent"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintConsumption(string Regfrom, string Regto, string Serfrom, string Serto
            , string CompanyFrom, string CompanyTo, string ClassFrom, string ClassTo, string CardFrom
            , string CardTo, string ContractNumber, string RepotType, string larg, string smal, string Percent)
        {
            Int64 CompanyStart, CompanyEnd,
                ContrNumber, per, lrg, sml;
            string CardStart, CardEnd, ClassStart, ClassEnd;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            CompanyStart = CompanyFrom == string.Empty ? 0 : Convert.ToInt64(CompanyFrom);
            CompanyEnd = CompanyTo == string.Empty ? 999999999 : Convert.ToInt64(CompanyTo);
            ClassStart = ClassFrom == string.Empty ? "0" : ClassFrom;
            ClassEnd = ClassTo == string.Empty ? "zzzzz" : ClassTo;
            CardStart = CardFrom == string.Empty ? "0" : CardFrom;
            CardEnd = CardTo == string.Empty ? "9999999999999999999999" : CardTo;
            ContrNumber = ContractNumber == string.Empty ? 0 : Convert.ToInt64(ContractNumber);



            per = Percent == string.Empty ? 0 : Convert.ToInt64(Percent);
            lrg = larg == string.Empty ? 0 : Convert.ToInt64(larg);
            sml = smal == string.Empty ? 0 : Convert.ToInt64(smal);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;


            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub1.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
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

                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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

                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub4.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
                    rd.SetParameterValue("cls1", ClassStart);
                    rd.SetParameterValue("cls2", ClassEnd);
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

                case 4:

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub7.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
                    rd.SetParameterValue("cls1", ClassStart);
                    rd.SetParameterValue("cls2", ClassEnd);
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

                case 5:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub10.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
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

                case 6:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub11.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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

                case 7:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub12.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
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

                case 8:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub13.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("cls1", ClassStart);
                    rd.SetParameterValue("cls2", ClassEnd);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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

                case 9:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub14New.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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

                case 10:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2New.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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

                default:
                    return View();


            }
        }


        /// <summary>
        /// For Print Consumption Report as EXCEL
        /// </summary>
        /// <param name="Regfrom"></param> 
        /// <param name="Regto"></param>
        /// <param name="Serfrom"></param>
        /// <param name="Serto"></param>
        /// <param name="CompanyFrom"></param>
        /// <param name="CompanyTo"></param>
        /// <param name="ClassFrom"></param>
        /// <param name="ClassTo"></param>
        /// <param name="CardFrom"></param>
        /// <param name="CardTo"></param>
        /// <param name="ContractNumber"></param>
        /// <param name="RepotType"></param>
        /// <param name="larg"></param>
        /// <param name="smal"></param>
        /// <param name="Percent"></param>
        /// <returns>Report as EXCEL</returns>
        public ActionResult PrintXlxConsumption(string Regfrom, string Regto, string Serfrom, string Serto
            , string CompanyFrom, string CompanyTo, string ClassFrom, string ClassTo, string CardFrom
            , string CardTo, string ContractNumber, string RepotType, string larg, string smal, string Percent)
        {
            Int64 CompanyStart, CompanyEnd,
                ContrNumber, per, lrg, sml;
            string CardStart, CardEnd, ClassStart, ClassEnd;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;

            CompanyStart = CompanyFrom == string.Empty ? 0 : Convert.ToInt64(CompanyFrom);
            CompanyEnd = CompanyTo == string.Empty ? 999999999 : Convert.ToInt64(CompanyTo);
            ClassStart = ClassFrom == string.Empty ? "0" : ClassFrom;
            ClassEnd = ClassTo == string.Empty ? "zzzzz" : ClassTo;
            CardStart = CardFrom == string.Empty ? "0" : CardFrom;
            CardEnd = CardTo == string.Empty ? "9999999999999999999999" : CardTo;
            ContrNumber = ContractNumber == string.Empty ? 0 : Convert.ToInt64(ContractNumber);



            per = Percent == string.Empty ? 0 : Convert.ToInt64(Percent);
            lrg = larg == string.Empty ? 0 : Convert.ToInt64(larg);
            sml = smal == string.Empty ? 0 : Convert.ToInt64(smal);


            RegDateFrom = Regfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = Regto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = Serfrom == string.Empty ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = Serto == string.Empty ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;


            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub1.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub4.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
                    rd.SetParameterValue("cls1", ClassStart);
                    rd.SetParameterValue("cls2", ClassEnd);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 4:

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub7.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");

                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
                    rd.SetParameterValue("cls1", ClassStart);
                    rd.SetParameterValue("cls2", ClassEnd);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 5:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub10.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 6:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub11.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 7:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub12.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 8:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub13.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("cls1", ClassStart);
                    rd.SetParameterValue("cls2", ClassEnd);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 9:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub14New.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                case 10:


                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "Reportaub2New.rpt"));

                    rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("crda1", RegDateFrom);
                    rd.SetParameterValue("crda2", RegDateTo);
                    rd.SetParameterValue("comp1", CompanyStart);
                    rd.SetParameterValue("comp2", CompanyEnd);
                    rd.SetParameterValue("crd1", CardStart);
                    rd.SetParameterValue("crd2", CardEnd);
                    rd.SetParameterValue("srda1", SerDateFrom);
                    rd.SetParameterValue("srda2", SerDateTo);
                    rd.SetParameterValue("per", per);
                    rd.SetParameterValue("larg", lrg);
                    rd.SetParameterValue("small", sml);
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
                        return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                default:
                    return View();


            }
        }
        #endregion
    }
}