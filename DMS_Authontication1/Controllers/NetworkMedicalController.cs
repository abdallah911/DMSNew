using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Globalization;
using System.Threading;

namespace DMS_Authontication1.Controllers
{
    public class NetworkMedicalController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();
        ApplicationDbContext myEntities = new ApplicationDbContext();
        DBApproval dbData = new DBApproval();
        DBData dbData2 = new DBData();

        [HttpGet]
        public ActionResult NetworkMedical(string cardId)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            EmployeeDataViewModel mod = new EmployeeDataViewModel();

            if (!string.IsNullOrEmpty(cardId))
            {
                DataTable dtcrd = new DataTable();
                DataTable dtNotes = new DataTable();

                //dtcrd = dbData.RunReader(@" SELECT e.EMP_ANAME_ST || ' ' || e.EMP_ANAME_SC || ' ' || e.EMP_ANAME_TH NAME, TO_CHAR(e.INS_START_DATE,'DD-MM-YYYY') INS_START_DATE, TO_CHAR(e.INS_END_DATE,'DD-MM-YYYY') INS_END_DATE, q.NOTES  
                //                            FROM    DMS_TEST.COMP_EMPLOYEES e
                //                            LEFT OUTER JOIN APP.CARD_QR q ON  e.C_COMP_ID = q.COMP_ID AND e.CARD_ID = q.CARD_ID
                //                            WHERE q.CARD_ID = '" + cardId + "'");
                dtcrd = dbData2.getData(cardId);

                if (dtcrd.Rows.Count != 0)
                {
                    if (dtcrd.Rows.Count > 0 && dtcrd.Rows[0]["TERMINATE_FLAG"].ToString() == "Y" && Convert.ToDateTime(dtcrd.Rows[0]["TERMINATE_DATE"]).Date < DateTime.Now.Date)
                    {
                        mod.Mesage = "تم إغلاق هذا الكارت بتاريخ " + dtcrd.Rows[0]["TERMINATE_DATE"].ToString() + "\n";

                        DataTable dtdelcrd = dbData.RunReader(@"SELECT CARD_ID, to_char(WITHDRAW_CARD_DATE, 'DD-MM-YYYY') FROM DMS_TEST.CLOSE_EMP_DATA WHERE WITHDRAW_CARD_DATE IS NOT NULL AND CARD_ID = '" + cardId + "'");

                        if (dtdelcrd.Rows.Count > 0)
                            mod.Mesage += " وتم استلام الكارت بتاريخ " + dtdelcrd.Rows[0][1].ToString();
                        else
                            mod.Mesage += " ولم يتم إستلام الكارت بعد ";
                    }
                    else if (dtcrd.Rows.Count > 0 && Convert.ToDateTime(dtcrd.Rows[0]["INS_END_DATE"]).Date < DateTime.Now.Date)
                    {
                        mod.Mesage = " أنتهى التعاقد مع هذا الموظف بتاريخ " + dtcrd.Rows[0]["INS_END_DATE"].ToString();
                    }
                    else
                    {
                        mod.Mesage = "";
                        string comp = dtcrd.Rows[0]["C_COMP_ID"].ToString(), contr = dtcrd.Rows[0]["CONTRACT_NO"].ToString(), cls = dtcrd.Rows[0]["CLASS_CODE"].ToString();

                        //dtNotes = dbData.RunReader(@" SELECT NOTES FROM COMP_NOTES WHERE COMP_ID = '" + comp + "' AND CONTRACT_NO = '" + contr + "' AND CLASS_CODE = '" + cls + "'AND CARD_ID = '" + cardId + "'");

                        //if (dtNotes.Rows.Count == 0)
                        //    dtNotes = dbData.RunReader(@" SELECT NOTES FROM COMP_NOTES WHERE COMP_ID = '" + comp + "' AND CONTRACT_NO = '" + contr + "' AND CLASS_CODE = '" + cls + "'");

                        //if (dtNotes.Rows.Count > 0)
                        //    notes = dtNotes.Rows[0]["NOTES"].ToString();
                      


                        
                        dtNotes = dbData.RunReader(@" SELECT * FROM COMP_INSTRUCTIONS WHERE COMP_ID = '" + comp + "' AND CONTRACT_NO = '" + contr + "' AND CLASS_CODE = '" + cls + "'AND CARD_ID = '" + cardId + "' AND ACTIVE = 'Y' ORDER BY CODE_NOTES");

                        if (dtNotes.Rows.Count == 0)
                            dtNotes = dbData.RunReader(@" SELECT * FROM COMP_INSTRUCTIONS WHERE COMP_ID = '" + comp + "' AND CONTRACT_NO = '" + contr + "' AND CLASS_CODE = '" + cls + "' AND ACTIVE = 'Y' AND CARD_ID IS NULL ORDER BY CODE_NOTES");

                        if (dtNotes.Rows.Count > 0)
                            mod.Notes = dtNotes.AsEnumerable().Select(row => row["NOTES"]?.ToString()).ToArray();

                        //mod.Notes = dtNotes.AsEnumerable()
                        //                                .SelectMany(row => row.ItemArray.Select(field => field.ToString()))
                        //                                .ToArray();


                        mod.CardId = cardId;
                        mod.EmpName = dtcrd.Rows[0]["NAME"].ToString();
                        mod.StartDate = dtcrd.Rows[0]["INS_START_DATE"].ToString();
                        mod.EndDate = dtcrd.Rows[0]["INS_END_DATE"].ToString();
                        //mod.Notes = notes;

                        //mod.Notes = mod.Notes.Replace("\n", "<br>");
                        //mod.Notes = notes.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    }                                      
                }
                else                
                    mod.Mesage += "لا توجد بيانات للكارت الرجاء التأكد من رقم الكارت المدخل وحاول ثانية";
                
                var provider = db.ProviderTypeNews.ToList();
                SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
                ViewBag.provider = Providerlist;

                var address = db.Basic_Data.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();

                SelectList addresslist = new SelectList(address, "BS_CODE", "BS_ANAME");
                ViewBag.address = addresslist;

                return View(mod);
            }
            
            return View();
        }
        public JsonResult GetRegion(string id)
        {
            myEntities.Configuration.ProxyCreationEnabled = false;

            var State = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == id).ToList();
            SelectList StateListlist = new SelectList(State, "BS_CODE", "BS_ANAME");
            return Json(StateListlist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProviders(string cardId, string compId, int country, int region, int providerId, string specialistid)
        {
            int Comp_ID = Convert.ToInt32(compId);
            var cardExist = db.Comp_Employees.AsNoTracking().Where(c => c.CARD_ID == cardId).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault();
            if (cardExist == null)
            {
                return new JsonResult { Data = new { providerslist = 0, msg = "Card Not Found ..." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                var coverdRelation = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == Comp_ID && c.CLASS_CODE ==
                                         cardExist.CLASS_CODE && c.CONTRACT_NO == cardExist.CONTRACT_NO).FirstOrDefault().COVER_RELATION;
                if (region != 0)
                {
                    if (coverdRelation == 1 || coverdRelation == 4)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                            .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();

                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else if (coverdRelation == 2)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" && (p.PROV_DEGREE == coverdRelation.ToString() || p.PROV_DEGREE == "3") && p.AREA_CODE == region && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3"*/ /*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                           .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();
                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" && p.PROV_DEGREE == coverdRelation.ToString() && p.AREA_CODE == region && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid) /*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                            .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC
                                                  }).ToList();

                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }
                else
                {
                    if (coverdRelation == 1 || coverdRelation == 4)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.GOVERNMENT_CODE == country && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                            .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();

                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else if (coverdRelation == 2)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" && (p.PROV_DEGREE == coverdRelation.ToString() || p.PROV_DEGREE == "3") && p.GOVERNMENT_CODE == country && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3"*/ /*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                           .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();
                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" && p.PROV_DEGREE == coverdRelation.ToString() && p.GOVERNMENT_CODE == country && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid) /*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                            .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC
                                                  }).ToList();

                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }
            }

        }

        public ActionResult PrintRoshita(Int32 cardId)
        {
            string eror;

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Roshita.rpt"));

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@idd", cardId);
            
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
                return File(stream, "application/pdf", cardId + ".pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}