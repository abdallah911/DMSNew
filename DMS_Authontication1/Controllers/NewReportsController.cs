using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers
{
    public class NewReportsController : Controller
    {
        DMS_TESTEntities db;
        public NewReportsController()
        {
            db = new DMS_TESTEntities();
        }
        public ActionResult Index()
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
        public ActionResult PrintReportsExcel(DateTime RegistrationFrom, DateTime RegistrationTo, DateTime ActivationFrom, DateTime ActivationTo, string CopmanyNameReportsFrom,
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

            crd1 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            crd2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            cls1 = "ZZZZZZZ";
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
    }
}