using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using Microsoft.AspNet.Identity;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers
{
    public class NewReportsController : Controller
    {
       
        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationDbContext db2 = new ApplicationDbContext();
        void getAllCompany()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                COMP_ID = c.C_COMP_ID,
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "COMP_ID", "Name");
            ViewBag.company = companylist;
        }

        #region Operation 
        public ActionResult OperationReport()
        {
            getAllCompany();

            return View();
        }
        
        public ActionResult PrintReportsPdf(string RegistrationFrom, string RegistrationTo, string ActivationFrom, string ActivationTo, string CopmanyNameReportsFrom,
                                               string CopmanyNameReportsTo, string TypeReport)
        {
            DateTime RegDateFrom, RegDateTo, ActDateFrom, ActDateTo;

            RegDateFrom = string.IsNullOrEmpty(RegistrationFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(RegistrationFrom)).Date;
            RegDateTo = string.IsNullOrEmpty(RegistrationTo) ? DateTime.Now.Date : (Convert.ToDateTime(RegistrationTo)).Date;
            ActDateFrom = string.IsNullOrEmpty(ActivationFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(ActivationFrom)).Date;
            ActDateTo = string.IsNullOrEmpty(ActivationTo) ? DateTime.Now.Date : (Convert.ToDateTime(ActivationTo)).Date;
            
            int comp1, comp2, rel1 = 0, rel2 = 0;
            string cc1, cc2;
            string crd1, crd2, cls1, cls2;
            DateTime card1, card2, dat1, dat2;


            comp1 = CopmanyNameReportsFrom == string.Empty ? 0 : Convert.ToInt32(CopmanyNameReportsFrom);
            comp2 = CopmanyNameReportsTo == string.Empty ? 99999999 : Convert.ToInt32(CopmanyNameReportsTo);
            cc1 = " ";
            cc2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            rel1 = 1;
            rel2 = 99999999;

            crd1 = " ";
            crd2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            cls1 = " ";
            cls2 = "zzzzzzz";

            card1 = (DateTime)RegDateFrom;
            card2 = (DateTime)RegDateTo;

            dat1 = (DateTime)ActDateFrom;
            dat2 = (DateTime)ActDateTo;

            ReportDocument rd = new ReportDocument();
            
            char check = TypeReport[0];
            
            if (check == '1')
            {
                if (comp1 != comp2)
                {
                    switch (TypeReport)
                    {
                        case "1-1":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationAdd.rpt"));
                            break;
                        case "1-2":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationClose.rpt"));
                            break;
                        case "1-3":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeClass.rpt"));
                            break;
                        case "1-4":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReActive.rpt"));
                            break;
                        case "1-5":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeName.rpt"));
                            break;
                        case "1-6":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationMissingCard.rpt"));
                            break;
                        case "1-7":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReplecmentCard.rpt"));
                            break;
                        case "1-8":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangePhoto.rpt"));
                            break;
                        case "1-9":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCostCsentr.rpt"));
                            break;
                        case "1-10":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCode.rpt"));
                            break;
                        case "1-11":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCodeandleval.rpt"));
                            break;
                        default:
                            return View();
                    }
                }
                else if (comp1 == comp2)
                {
                    switch (TypeReport)
                    {
                        case "1-1":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationAdd2.rpt"));
                            break;
                        case "1-2":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationClose2.rpt"));
                            break;
                        case "1-3":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeClass2.rpt"));
                            break;
                        case "1-4":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReActive2.rpt"));
                            break;
                        case "1-5":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeName2.rpt"));
                            break;
                        case "1-6":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationMissingCard2.rpt"));
                            break;
                        case "1-7":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReplecmentCard2.rpt"));
                            break;
                        case "1-8":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangePhoto2.rpt"));
                            break;
                        case "1-9":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCostCsentr2.rpt"));
                            break;
                        case "1-10":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCode2.rpt"));
                            break;
                        case "1-11":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCodeandleva2.rpt"));
                            break;
                        default:
                            return View();
                    }
                }
                rd.SetDatabaseLogon("APP", "12369");
                rd.SetParameterValue("comp1", comp1);
                rd.SetParameterValue("comp2", comp2);
                rd.SetParameterValue("crda1", card1);
                rd.SetParameterValue("crda2", card2);
                rd.SetParameterValue("dat1", dat1);
                rd.SetParameterValue("dat2", dat2);
                rd.SetParameterValue("crd1", crd1);
                rd.SetParameterValue("crd2", crd2);
                rd.SetParameterValue("cls1", cls1);
                rd.SetParameterValue("cls2", cls2);
                rd.SetParameterValue("rel1", rel1);
                rd.SetParameterValue("rel2", rel2);
                rd.SetParameterValue("cc1", cc1);
                rd.SetParameterValue("cc2", cc2);
            }
            else
            {
                switch (TypeReport)
                {
                    case "2-1":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmp.rpt"));
                        break;
                    case "2-2":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpOldCmp.rpt"));
                        break;
                    case "2-3":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpY.rpt"));
                        break;
                    case "2-4":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpYLimit.rpt"));
                        break;
                    case "2-5":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpYOldCmp.rpt"));
                        break;
                    default:
                        return View();
                }

                rd.SetDatabaseLogon("APP", "12369");
                rd.SetParameterValue("comp", comp1);
                rd.SetParameterValue("rel1", rel1);
                rd.SetParameterValue("rel2", rel2);
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
                return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "ReportMed.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult PrintReportsExcel(string RegistrationFrom, string RegistrationTo, string ActivationFrom, string ActivationTo, string CopmanyNameReportsFrom,
                                               string CopmanyNameReportsTo, string TypeReport)
        {
            DateTime RegDateFrom, RegDateTo, ActDateFrom, ActDateTo;

            RegDateFrom = string.IsNullOrEmpty(RegistrationFrom.ToString()) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(RegistrationFrom)).Date;
            RegDateTo = string.IsNullOrEmpty(RegistrationTo.ToString()) ? DateTime.Now.Date : (Convert.ToDateTime(RegistrationTo)).Date;
            ActDateFrom = string.IsNullOrEmpty(ActivationFrom.ToString()) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(ActivationFrom)).Date;
            ActDateTo = string.IsNullOrEmpty(ActivationTo.ToString()) ? DateTime.Now.Date : (Convert.ToDateTime(ActivationTo)).Date;

            int comp1, comp2, rel1 = 0, rel2 = 0;
            string cc1, cc2;
            string crd1, crd2, cls1, cls2;
            DateTime card1, card2, dat1, dat2;


            comp1 = CopmanyNameReportsFrom == string.Empty ? 0 : Convert.ToInt32(CopmanyNameReportsFrom);
            comp2 = CopmanyNameReportsTo == string.Empty ? 99999999 : Convert.ToInt32(CopmanyNameReportsTo);
            cc1 = " ";
            cc2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            rel1 = 1;
            rel2 = 99999999;

            crd1 = " ";
            crd2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            cls1 = " ";
            cls2 = "zzzzzzz";


            card1 = RegDateFrom;
            card2 = RegDateTo;

            dat1 = ActDateFrom;
            dat2 = ActDateTo;


            ReportDocument rd = new ReportDocument();


            char check = TypeReport[0];

            if (check == '1')
            {
                if (comp1 != comp2)
                {
                    switch (TypeReport)
                    {
                        case "1-1":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationAdd.rpt"));
                            break;
                        case "1-2":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationClose.rpt"));
                            break;
                        case "1-3":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeClass.rpt"));
                            break;
                        case "1-4":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReActive.rpt"));
                            break;
                        case "1-5":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeName.rpt"));
                            break;
                        case "1-6":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationMissingCard.rpt"));
                            break;
                        case "1-7":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReplecmentCard.rpt"));
                            break;
                        case "1-8":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangePhoto.rpt"));
                            break;
                        case "1-9":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCostCsentr.rpt"));
                            break;
                        case "1-10":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCode.rpt"));
                            break;
                        case "1-11":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCodeandleval.rpt"));
                            break;
                        default:
                            return View();
                    }
                }
                else if (comp1 == comp2)
                {
                    switch (TypeReport)
                    {
                        case "1-1":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationAdd2.rpt"));
                            break;
                        case "1-2":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationClose2.rpt"));
                            break;
                        case "1-3":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeClass2.rpt"));
                            break;
                        case "1-4":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReActive2.rpt"));
                            break;
                        case "1-5":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeName2.rpt"));
                            break;
                        case "1-6":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationMissingCard2.rpt"));
                            break;
                        case "1-7":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationReplecmentCard2.rpt"));
                            break;
                        case "1-8":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangePhoto2.rpt"));
                            break;
                        case "1-9":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCostCsentr2.rpt"));
                            break;
                        case "1-10":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCode2.rpt"));
                            break;
                        case "1-11":
                            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationChangeCodeandleva2.rpt"));
                            break;
                        default:
                            return View();
                    }
                }
                rd.SetDatabaseLogon("APP", "12369");
                rd.SetParameterValue("comp1", comp1);
                rd.SetParameterValue("comp2", comp2);
                rd.SetParameterValue("crda1", card1);
                rd.SetParameterValue("crda2", card2);
                rd.SetParameterValue("dat1", dat1);
                rd.SetParameterValue("dat2", dat2);
                rd.SetParameterValue("crd1", crd1);
                rd.SetParameterValue("crd2", crd2);
                rd.SetParameterValue("cls1", cls1);
                rd.SetParameterValue("cls2", cls2);
                rd.SetParameterValue("rel1", rel1);
                rd.SetParameterValue("rel2", rel2);
                rd.SetParameterValue("cc1", cc1);
                rd.SetParameterValue("cc2", cc2);
            }
            else
            {
                switch (TypeReport)
                {
                    case "2-1":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmp.rpt"));
                        break;
                    case "2-2":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpOldCmp.rpt"));
                        break;
                    case "2-3":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpY.rpt"));
                        break;
                    case "2-4":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpYLimit.rpt"));
                        break;
                    case "2-5":
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportEmpYOldCmp.rpt"));
                        break;
                    default:
                        return View();
                }
                rd.SetParameterValue("comp", comp1);
                rd.SetParameterValue("rel1", rel1);
                rd.SetParameterValue("rel2", rel2);
                rd.SetDatabaseLogon("APP", "12369");

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
                return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "ReportMed.xls");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PrintTransactionReports(string RegistrationFrom, string RegistrationTo, string CopmanyNameReportsFrom,
                                                    string CopmanyNameReportsTo, string TypeReport, string PrintAS)
        {
            int comp1, comp2;            
            string nam = "";
            DateTime dat1, dat2;

            dat1 = string.IsNullOrEmpty(RegistrationFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(RegistrationFrom)).Date;
            dat2 = string.IsNullOrEmpty(RegistrationTo) ? DateTime.Now.Date : (Convert.ToDateTime(RegistrationTo)).Date;
            comp1 = CopmanyNameReportsFrom == string.Empty ? 0 : Convert.ToInt32(CopmanyNameReportsFrom);
            comp2 = CopmanyNameReportsTo == string.Empty ? 99999999 : Convert.ToInt32(CopmanyNameReportsTo);

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/OperationReport"), "ReportOperationTransction.rpt"));
            nam = "Report Operation Transction";
      

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("comp1", comp1);
            rd.SetParameterValue("comp2", comp2);
            rd.SetParameterValue("crda1", dat1);
            rd.SetParameterValue("crda2", dat2);
                        
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();



            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Approval 
        public ActionResult ApprovalReports()
        {
            getAllCompany();

            return View();
        }
        
        public ActionResult PrintReportsExcelApprovalConsm(string DateFrom, string DateTo, string CardFrom, string CardTo, string CopmanyFrom,
                                             string CopmanyTo, string TypeReport, string ProvNo, string ClassApproval, string smal, string larg, string PrintAS)    
        {
            int comp1, comp2, prv1, prv2;
            double sml, lrg;
            string card1, card2, cls1, cls2, nam = "";
            DateTime dat1, dat2;

            //dat1 = Convert.ToDateTime(DateFrom);
            //dat2 = Convert.ToDateTime(DateTo);

            dat1 = string.IsNullOrEmpty(DateFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(DateFrom)).Date;
            dat2 = string.IsNullOrEmpty(DateTo) ? DateTime.Now.Date : (Convert.ToDateTime(DateTo)).Date;
           
            comp1 = string.IsNullOrEmpty(CopmanyFrom) ? 0 : Convert.ToInt32(CopmanyFrom);
            comp2 = string.IsNullOrEmpty(CopmanyTo) ? 999999999 : Convert.ToInt32(CopmanyTo);
            card1 = string.IsNullOrEmpty(CardFrom) ? " " : CardFrom;
            card2 = string.IsNullOrEmpty(CardTo) ? "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz" : CardTo;
            cls1 = string.IsNullOrEmpty(ClassApproval) ? " " : ClassApproval;
            cls2 = string.IsNullOrEmpty(ClassApproval) ? "zzzzz" : ClassApproval;
            prv1 = string.IsNullOrEmpty(ProvNo) ? 0 : Convert.ToInt32(ProvNo);
            prv2 = string.IsNullOrEmpty(ProvNo) ? 999999999 : Convert.ToInt32(ProvNo);


            sml = string.IsNullOrEmpty(larg) ? 0 : Convert.ToDouble(larg);
            lrg = string.IsNullOrEmpty(smal) ? 9999999999999999999 : Convert.ToDouble(smal);


            ReportDocument rd = new ReportDocument();


            switch (Convert.ToInt32(TypeReport))
            {
                case 0:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov1.rpt"));                    
                    nam = "Consumption Per Employee VS. Service Approval";
                    break;
                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov2.rpt"));                    
                    nam = "Consumption Per Super Group Service Approval";
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov6.rpt"));
                    nam = "Consumption Per Gender Approval";
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov7.rpt"));                  
                    nam = "Consumption Per Employee Approval";
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov10.rpt"));                    
                    nam = "Consumption Summary Approval";
                    break;
                case 5:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov11.rpt"));
                    nam = "Consumption Per Relation Approval";
                    break;
                case 6:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov12.rpt"));
                    nam = "Consumption Details Approval";

                    break;
                case 7:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov13.rpt"));
                    nam = "Consumption Per Provider Approval";

                    break;
                case 8:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportAprov14.rpt"));
                    nam = "Consumption Per Service Group Approval";

                    break;
                default:

                    break;
            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("crda1", dat1);
            rd.SetParameterValue("crda2", dat2);
            rd.SetParameterValue("comp1", comp1);
            rd.SetParameterValue("comp2", comp2);
            rd.SetParameterValue("crd1", card1);
            rd.SetParameterValue("crd2", card2);
            rd.SetParameterValue("cls1", cls1);
            rd.SetParameterValue("cls2", cls2);
            rd.SetParameterValue("prv1", prv1);
            rd.SetParameterValue("prv2", prv2);
            rd.SetParameterValue("lrg", lrg);
            rd.SetParameterValue("sml", sml);
            rd.SetParameterValue("UserName", User.Identity.GetUserName());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            
            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public ActionResult PrintSummaryApproval(string DateFrom, string DateTo, string CopmanyFrom, string CopmanyTo, 
                                                 string TypeReport, string val1, string val2, string PrintAS)
        {
            int comp1, comp2;
            double amt1, amt2;
            string nam = "";
            DateTime dat1, dat2;

            comp1 = string.IsNullOrEmpty(CopmanyFrom) ? 0 : Convert.ToInt32(CopmanyFrom);
            comp2 = string.IsNullOrEmpty(CopmanyTo) ? 999999999 : Convert.ToInt32(CopmanyTo);
            amt1 = string.IsNullOrEmpty(val1) ? 0 : Convert.ToDouble(val1);
            amt2 = string.IsNullOrEmpty(val2) ? 9999999999999999999 : Convert.ToDouble(val2);
            dat1 = string.IsNullOrEmpty(DateFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(DateFrom)).Date;
            dat2 = string.IsNullOrEmpty(DateTo) ? DateTime.Now.Date : (Convert.ToDateTime(DateTo)).Date;

            ReportDocument rd = new ReportDocument();
            //rd.SetDatabaseLogon("APP", "12369");

            switch (Convert.ToInt32(TypeReport))
            {
                case 0:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalServBy.rpt"));
                    nam = "Report Approval Srevice&Created By Summary";
                    break;
                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalByCard.rpt"));
                    nam = "Report Approval Card Summary";
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalDate.rpt"));
                    nam = "Report Approval Date";
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalDateServ.rpt"));
                    nam = "Report Approval Date & Serv";
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalCompany.rpt"));
                    nam = "Report Approval Company";
                    break;
                case 5:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalAmt.rpt"));
                    nam = "Report Approval Amt";
                    break;
                case 6:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalPrv.rpt"));
                    nam = "Report Approval Provider";

                    break;
                case 7:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalReply.rpt"));
                    nam = "Report Approval Reply";

                    break;
                case 8:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalEditDelete.rpt"));
                    nam = "Report Approval Edit & Delete";

                    break;
                case 9:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalRecollection.rpt"));
                    nam = "Report Approval Recollection";

                    break;
                case 10:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalService.rpt"));
                    nam = "Report Approval Service";
                    break;
                case 11:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalResdnt.rpt"));
                    nam = "Report Approval Resdnt";
                    //rd.SetDatabaseLogon("APP", "12369");
                    rd.SetParameterValue("res1", "0");
                    rd.SetParameterValue("res2", "zzzzzzzzzzzzzzzzzzzzz");

                    break;
                case 12:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalException.rpt"));
                    nam = "Report Approval Exception";

                    break;
                case 13:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalServiceComp.rpt"));
                    nam = "Report Approval Service-Type Comp";

                    break;
                default:

                    break;
            }

            //if(Convert.ToInt32(TypeReport) != 11)
                

            rd.SetParameterValue("crda1", dat1);
            rd.SetParameterValue("crda2", dat2);
            rd.SetParameterValue("comp1", comp1);            
            rd.SetParameterValue("comp2", comp2);           
            rd.SetParameterValue("amt1", amt1);
            rd.SetParameterValue("amt2", amt2);

            rd.SetDatabaseLogon("APP", "12369");
            //rd.SetParameterValue("UserName", User.Identity.GetUserName());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            
            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PrintDetailsApproval(string DateFrom, string DateTo, string CopmanyFrom, string CopmanyTo,
                                              string TypeReport, string val1, string val2, string PrintAS)
        {
            int comp1, comp2;
            double amt1, amt2;
            string nam = "";
            DateTime dat1, dat2;

            comp1 = string.IsNullOrEmpty(CopmanyFrom) ? 0 : Convert.ToInt32(CopmanyFrom);
            comp2 = string.IsNullOrEmpty(CopmanyTo) ? 999999999 : Convert.ToInt32(CopmanyTo);
            amt1 = string.IsNullOrEmpty(val1) ? 0 : Convert.ToDouble(val1);
            amt2 = string.IsNullOrEmpty(val2) ? 9999999999999999999 : Convert.ToDouble(val2);
            dat1 = string.IsNullOrEmpty(DateFrom) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(DateFrom)).Date;
            dat2 = string.IsNullOrEmpty(DateTo) ? DateTime.Now.Date : (Convert.ToDateTime(DateTo)).Date;

            ReportDocument rd = new ReportDocument();
            

            switch (Convert.ToInt32(TypeReport))
            {
                case 0:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalServByDetails.rpt"));
                    nam = "Report Approval Srevice&Created By Details";
                    break;
                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalCardDetails.rpt"));
                    nam = "Report Approval Card Details";
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalDateDetails.rpt"));
                    nam = "Report Approval Date";
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalDateServDetails.rpt"));
                    nam = "Report Approval Date & Serv";
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalCompanyDetails.rpt"));
                    nam = "Report Approval Company";
                    break;
                case 5:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalAmtDetails.rpt"));
                    nam = "Report Approval Amt";
                    break;
                case 6:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalPrvDetails.rpt"));
                    nam = "Report Approval Provider";

                    break;
                case 7:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalReplyDetails.rpt"));
                    nam = "Report Approval Reply";

                    break;
                case 8:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalEditDeleteDetails.rpt"));
                    nam = "Report Approval Edit & Delete";

                    break;
                case 9:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalRecollectionDetails.rpt"));
                    nam = "Report Approval Recollection";

                    break;
                case 10:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalServiceDetails.rpt"));
                    nam = "Report Approval Service";
                    break;
                //case 11:
                //    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalResdnt.rpt"));
                //    nam = "Report Approval Resdnt";
                //    rd.SetParameterValue("res1", "0");
                //    rd.SetParameterValue("res2", "zzzzzzzzzzzzzzzzzzzzz");

                //    break;
                case 12:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalExceptionDetails.rpt"));
                    nam = "Report Approval Exception";

                    break;
                case 13:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ApprovalReports"), "ReportApprovalServiceCompDetails.rpt"));
                    nam = "Report Approval Service-Type Comp";

                    break;
                default:

                    break;
            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("crda1", dat1);
            rd.SetParameterValue("crda2", dat2);
            rd.SetParameterValue("comp1", comp1);
            rd.SetParameterValue("comp2", comp2);
            rd.SetParameterValue("amt1", amt1);
            rd.SetParameterValue("amt2", amt2);
            //rd.SetParameterValue("UserName", User.Identity.GetUserName());
           
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();



            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Consumption
        public ActionResult ConsumptionReport()
        {
            getAllCompany();

            return View();
        }
        public ActionResult PrintConsumptionReports(string Regfrom, string Regto, string Serfrom, string Serto, 
                                                    string CompanyFrom, string CompanyTo, string ProviderName1, string ProviderName2, string ClassConsum,
                                                    string CardFrom, string CardTo, string RepotType, string PrintAS)
        {
            Int64 CompanyStart, CompanyEnd, ProviderNumber1, ProviderNumber2;
            string CardStart, CardEnd, cls1, cls2;
            DateTime RegDateFrom, RegDateTo, SerDateFrom, SerDateTo;
            
            CompanyStart = string.IsNullOrEmpty(CompanyFrom) ? 0 : Convert.ToInt64(CompanyFrom);
            CompanyEnd = string.IsNullOrEmpty(CompanyTo) ? 999999999 : Convert.ToInt64(CompanyTo);
            ProviderNumber1 = string.IsNullOrEmpty(ProviderName1) ? 0 : Convert.ToInt64(ProviderName1);
            ProviderNumber2 = string.IsNullOrEmpty(ProviderName2) ? 999999999 : Convert.ToInt64(ProviderName1);
            CardStart = string.IsNullOrEmpty(CardFrom) ? " " : CardFrom;
            CardEnd = string.IsNullOrEmpty(CardTo) ? "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz" : CardTo;
            cls1 = string.IsNullOrEmpty(ClassConsum) ? " " : ClassConsum;
            cls2 = string.IsNullOrEmpty(ClassConsum) ? "zzzzz" : ClassConsum;
            RegDateFrom = string.IsNullOrEmpty(Regfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Regfrom)).Date;
            RegDateTo = string.IsNullOrEmpty(Regto) ? DateTime.Now.Date : (Convert.ToDateTime(Regto)).Date;
            SerDateFrom = string.IsNullOrEmpty(Serfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(Serfrom)).Date;
            SerDateTo = string.IsNullOrEmpty(Serto) ? DateTime.Now.Date : (Convert.ToDateTime(Serto)).Date;

            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 901:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum901InternalCode.rpt"));
                    else if (CompanyStart.ToString().StartsWith("500") || CompanyEnd.ToString().StartsWith("500"))
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum901.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum901Prem.rpt"));
                    break;
                case 9011:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum901-1InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum901-1.rpt"));
                    break;

                case 902:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum902InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum902.rpt"));
                    break;

                case 903:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum903InternalCode.rpt"));
                    else
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
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum915InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum915.rpt"));
                    break;
                case 916:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum916InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum916.rpt"));
                    break;
                case 917:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum917InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum917.rpt"));
                    break;
                case 918:
                    if (CompanyStart == 10362 || CompanyStart == 500144 || CompanyEnd == 500144 || CompanyEnd == 500144)
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum918InternalCode.rpt"));
                    else
                        rd.Load(Path.Combine(Server.MapPath("~/Reports/ReportsConsumption"), "Consum918.rpt"));
                    break;

                case 920:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "SoficoClaims.rpt"));
                    break;

                case 921:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/HR"), "SoficoDetails.rpt"));
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
                rd.SetParameterValue("cls1", cls1);
                rd.SetParameterValue("cls2", cls2);
                rd.SetParameterValue("PRV1", ProviderNumber1);
                rd.SetParameterValue("PRV2", ProviderNumber2);
            
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", RegDateFrom.ToString("ddMMyyyy") + "Consumption.xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", RegDateFrom.ToString("ddMMyyyy") + "Consumption.pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }
        #endregion

        #region Contract
        public ActionResult ReportsContract()
        {
            //getAllCompany();

            return View();
        }
        public ActionResult PrintContractReports(string RepotType, string PrintAS)
        {
            string nam = "";
            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 0:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ContractReports"), "ReportEndComp11.rpt"));
                    nam = "End Comp Since 11 Months";
                    break;
                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/ContractReports"), "ReportEndCompBefore1.rpt"));
                    nam = "End Comp After 1 Month";
                    break;
                    
                default:
                    return View();
            }

            rd.SetDatabaseLogon("APP", "12369");

           

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CustomerServices
        public ActionResult ReportsCustomerServices()
        {
            //getAllCompany();

            return View();
        }
        public ActionResult PrintCustomerReports(string datfrom, string datto, string RepotType, string PrintAS)
        {
            string crd1, crd2, nam = "";
            DateTime dat1, dat2;
           
            dat1 = string.IsNullOrEmpty(datfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(datfrom)).Date;
            dat2 = string.IsNullOrEmpty(datto) ? DateTime.Now.Date : (Convert.ToDateTime(datto)).Date;
            crd1 = " ";
            crd2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";

            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 0:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/CustomerServicesReports"), "ReportMonitor.rpt"));
                    nam = "Monitor Report";
                    break;
                //case 1:
                   
                //    break;
                //case 2:
                  
                //    break;
                //case 3:
                   
                //    break;
                //case 4:
                  
                //    break;

                default:
                    return View();
            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("dat1", dat1);
            rd.SetParameterValue("dat2", dat2);
            rd.SetParameterValue("usr", null);
            rd.SetParameterValue("crd1", crd1);
            rd.SetParameterValue("crd2", crd2);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region LoseRatio
        public ActionResult ReportsLoseRatio()
        {
            getAllCompany();

            return View();
        }

        public ActionResult PrintLossRatioReports(string CompanyFrom, string CompanyTo,
                                              string RepotType, string PrintAS)
        {
            Int64 cmp1, cmp2;
            string nam = "";

            cmp1 = string.IsNullOrEmpty(CompanyFrom) ? 0 : Convert.ToInt64(CompanyFrom);
            cmp2 = string.IsNullOrEmpty(CompanyTo) ? 999999999 : Convert.ToInt64(CompanyTo);

            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {
                case 0:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/LoseRatioReports"), "LossRatioReport.rpt"));
                    nam = "Loss Ratio Report";
                    break;
                //case 1:
                //    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "OrderWithoutPrint.rpt"));
                //    nam = "Order Without Print";
                //    break;
                //case 2:
                //    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "PrintWithoutReview.rpt"));
                //    nam = "Print Without Review";
                //    break;
                //case 3:
                //    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "PrintWithReview.rpt"));
                //    nam = "Print With Review";
                //    break;
                //case 4:
                //    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "PrintWithDelivery.rpt"));
                //    nam = "Print With Delivery";
                //    break;

                default:
                    return View();
            }

            rd.SetDatabaseLogon("APP", "12369");
                       
            rd.SetParameterValue("COMP1", cmp1);
            rd.SetParameterValue("COMP2", cmp2);
            //rd.SetParameterValue("UserName", User.Identity.GetUserName());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Pharmacy
        public ActionResult ReportsPharmacy()
        {
            getAllCompany();

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

        #endregion

        #region Printing
        public ActionResult ReportsPrinting()
        {
            getAllCompany();

            return View();
        }
        public ActionResult PrintPrintingReports(string datfrom, string datto, string CompanyFrom, string CompanyTo, 
                                                 string RepotType, string PrintAS)
        {
            Int64 cmp1, cmp2;
            DateTime dat1, dat2;
            string nam = "";
            

            dat1 = string.IsNullOrEmpty(datfrom) ? new DateTime(2017, 1, 1) : (Convert.ToDateTime(datfrom)).Date;
            dat2 = string.IsNullOrEmpty(datto) ? DateTime.Now.Date : (Convert.ToDateTime(datto)).Date;
            cmp1 = string.IsNullOrEmpty(CompanyFrom) ? 0 : Convert.ToInt64(CompanyFrom);
            cmp2 = string.IsNullOrEmpty(CompanyTo) ? 999999999 : Convert.ToInt64(CompanyTo);

            ReportDocument rd = new ReportDocument();

            switch (Convert.ToInt32(RepotType))
            {                
                case 0:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "TransactionWithoutPrint.rpt"));
                    nam = "Transaction Without Print";
                    break;
                case 1:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "OrderWithoutPrint.rpt"));
                    nam = "Order Without Print";
                    break;
                case 2:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "PrintWithoutReview.rpt"));
                    nam = "Print Without Review";
                    break;
                case 3:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "PrintWithReview.rpt"));
                    nam = "Print With Review";
                    break;
                case 4:
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/PrintingReports"), "PrintWithDelivery.rpt"));
                    nam = "Print With Delivery";
                    break;
                
                default:
                    return View();
            }

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("dat1", dat1);
            rd.SetParameterValue("dat2", dat2);
            rd.SetParameterValue("cmp1", cmp1);
            rd.SetParameterValue("cmp2", cmp2);
            rd.SetParameterValue("UserName", User.Identity.GetUserName());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            try
            {
                if (PrintAS == "1")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/xls", nam + ".xls");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Close();
                    rd.Dispose();
                    GC.Collect();
                    return File(stream, "application/pdf", nam + ".pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}