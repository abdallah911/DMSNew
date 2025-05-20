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
using DMS_Authontication1.ViewModel.HR;

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
                    else if (dtcrd.Rows.Count > 0 && dtcrd.Rows[0]["TERMINATE_FLAG"].ToString() == "H")
                    {
                        mod.Mesage = "تم إغلاق الكارت بسبب تعديه الحد الأقصى";
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
                        if(cardId.StartsWith("500172") || cardId.StartsWith("10000-VIP-100159-1"))
                        {
                            var medicin = (from md in db.Med_Card
                                           join m in db.Med_Medicine
                                           on md.CARD_NO equals m.CARD_NO
                                           where m.ACTIVE == "Y" &&
                                                 md.LOOK_01 == 0 &&
                                                    m.CARD_NO == cardId
                                           orderby m.CREATED_DATE ?? m.UPDATE_DATE descending
                                           select m).ToList();

                            if (medicin != null && medicin.Count > 0)
                                mod.ChronicCount = medicin.Count.ToString();

                            DateTime dat1, dat2;

                            dat1 = string.IsNullOrEmpty(mod.StartDate) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(mod.StartDate)).Date;
                            dat2 = string.IsNullOrEmpty(mod.EndDate) ? DateTime.Now.Date : (Convert.ToDateTime(mod.EndDate)).Date;

                            var dt = dbData2.getApproval(cardId, dat1, dat2, cardId);

                            if (dt != null && dt.Rows.Count > 0)
                                mod.ApprovalCount = dt.Rows.Count.ToString();
                        }


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
            var cardExist = db.Comp_Employees.AsNoTracking().Where(c => c.CARD_ID == cardId && DateTime.Now >= c.INS_START_DATE && DateTime.Now <= c.INS_END_DATE).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault();
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
                        //var allowedProvCodes = new List<int?> { 14, 662, 665 };

                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y"
                                                                    && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)
                                                                    && !db.CompContractClassProviders.Any(b => b.COMP_ID == Comp_ID && b.CONTRACT_NO == cardExist.CONTRACT_NO
                                                                    && b.CLASS_CODE == cardExist.CLASS_CODE && b.PRV_TYP == providerId && b.TYPE == "Black" && b.ACTIVE == "Y" && b.PR_CODE == p.PR_CODE))
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

                        var whiteList = db.CompContractClassProviders
                            .Join(db.SERV_PROVIDERS_NEW,
                             c => new { PR_CODE = c.PR_CODE, PRV_TYP = c.PRV_TYP },
                             p => new { PR_CODE = p.PR_CODE, PRV_TYP = p.PRV_TYPE },
                            (c, p) => new { c, p }) 
                            .Where(x => x.p.PRV_TYPE == providerId
                            && x.p.AREA_CODE == region
                            && x.p.TERMINATE_FLAG != "Y"
                            && x.c.COMP_ID == Comp_ID
                            && x.c.CONTRACT_NO == cardExist.CONTRACT_NO
                            && x.c.CLASS_CODE == cardExist.CLASS_CODE                            
                            && x.c.TYPE == "White"
                            && x.c.ACTIVE == "Y"
                            && (string.IsNullOrEmpty(specialistid) || x.p.DOC_SPEC == specialistid))
                            .Select(x => new
                            {
                                x.p.PR_ANAME,
                                x.p.ADDRESS1,
                                x.p.ADDRESS2,
                                x.p.TEL1,
                                x.p.TEL2,
                                x.p.PR_DESC
                            });


                        providerList = providerList.Union(whiteList).ToList();

                        //var providerList = (compId == "500172") ?
                        //    db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && !allowedProvCodes.Contains(p.PR_CODE) && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                        //                    .Select(
                        //                          s => new
                        //                          {
                        //                              s.PR_ANAME,
                        //                              s.ADDRESS1,
                        //                              s.ADDRESS2,
                        //                              s.TEL1,
                        //                              s.TEL2,
                        //                              s.PR_DESC

                        //                          }).ToList()
                        //:
                        //db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                        //                    .Select(
                        //                          s => new
                        //                          {
                        //                              s.PR_ANAME,
                        //                              s.ADDRESS1,
                        //                              s.ADDRESS2,
                        //                              s.TEL1,
                        //                              s.TEL2,
                        //                              s.PR_DESC

                        //                          }).ToList();

                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else if (coverdRelation == 2)
                    {
                        var providerList = 
                            db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" 
                                                    && (p.PROV_DEGREE == coverdRelation.ToString() || p.PROV_DEGREE == "3") 
                                                    && p.AREA_CODE == region && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)
                                                    && !db.CompContractClassProviders.Any(b => b.COMP_ID == Comp_ID && b.CONTRACT_NO == cardExist.CONTRACT_NO
                                                                      && b.CLASS_CODE == cardExist.CLASS_CODE && b.PRV_TYP == providerId && b.TYPE == "Black" && b.ACTIVE == "Y" && b.PR_CODE == p.PR_CODE))
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

                        var whiteList = db.CompContractClassProviders
                          .Join(db.SERV_PROVIDERS_NEW,
                           c => new { PR_CODE = c.PR_CODE, PRV_TYP = c.PRV_TYP },
                           p => new { PR_CODE = p.PR_CODE, PRV_TYP = p.PRV_TYPE },
                          (c, p) => new { c, p })
                          .Where(x => x.p.PRV_TYPE == providerId
                          && x.p.AREA_CODE == region
                          && x.p.TERMINATE_FLAG != "Y"
                          && x.c.COMP_ID == Comp_ID
                          && x.c.CONTRACT_NO == cardExist.CONTRACT_NO
                          && x.c.CLASS_CODE == cardExist.CLASS_CODE
                          && x.c.TYPE == "White"
                          && x.c.ACTIVE == "Y"
                          && (string.IsNullOrEmpty(specialistid) || x.p.DOC_SPEC == specialistid))
                          .Select(x => new
                          {
                              x.p.PR_ANAME,
                              x.p.ADDRESS1,
                              x.p.ADDRESS2,
                              x.p.TEL1,
                              x.p.TEL2,
                              x.p.PR_DESC
                          });


                        providerList = providerList.Union(whiteList).ToList();
                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else
                    {
                        var providerList = 
                            db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" 
                                                     && p.PROV_DEGREE == coverdRelation.ToString() && p.AREA_CODE == region 
                                                     && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)
                                                     && !db.CompContractClassProviders.Any(b => b.COMP_ID == Comp_ID && b.CONTRACT_NO == cardExist.CONTRACT_NO
                                                                      && b.CLASS_CODE == cardExist.CLASS_CODE && b.PRV_TYP == providerId && b.TYPE == "Black" && b.ACTIVE == "Y" && b.PR_CODE == p.PR_CODE))
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

                        var whiteList = db.CompContractClassProviders
                          .Join(db.SERV_PROVIDERS_NEW,
                           c => new { PR_CODE = c.PR_CODE, PRV_TYP = c.PRV_TYP },
                           p => new { PR_CODE = p.PR_CODE, PRV_TYP = p.PRV_TYPE },
                          (c, p) => new { c, p })
                          .Where(x => x.p.PRV_TYPE == providerId
                          && x.p.AREA_CODE == region
                          && x.p.TERMINATE_FLAG != "Y"
                          && x.c.COMP_ID == Comp_ID
                          && x.c.CONTRACT_NO == cardExist.CONTRACT_NO
                          && x.c.CLASS_CODE == cardExist.CLASS_CODE
                          && x.c.TYPE == "White"
                          && x.c.ACTIVE == "Y"
                          && (string.IsNullOrEmpty(specialistid) || x.p.DOC_SPEC == specialistid))
                          .Select(x => new
                          {
                              x.p.PR_ANAME,
                              x.p.ADDRESS1,
                              x.p.ADDRESS2,
                              x.p.TEL1,
                              x.p.TEL2,
                              x.p.PR_DESC
                          });


                        providerList = providerList.Union(whiteList).ToList();
                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }
                else
                {
                    if (coverdRelation == 1 || coverdRelation == 4)
                    {
                        //var allowedProvCodes = new List<int?> { 14, 662, 665 };


                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.GOVERNMENT_CODE == country && p.TERMINATE_FLAG != "Y" 
                                                                   && (string.IsNullOrEmpty(specialistid) || p.DOC_SPEC == specialistid)
                                                                   && !db.CompContractClassProviders.Any(b => b.COMP_ID == Comp_ID && b.CONTRACT_NO == cardExist.CONTRACT_NO
                                                                      && b.CLASS_CODE == cardExist.CLASS_CODE && b.PRV_TYP == providerId && b.TYPE == "Black" && b.ACTIVE == "Y" && b.PR_CODE == p.PR_CODE))
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

                        var whiteList = db.CompContractClassProviders
                           .Join(db.SERV_PROVIDERS_NEW,
                            c => new { PR_CODE = c.PR_CODE, PRV_TYP = c.PRV_TYP },
                            p => new { PR_CODE = p.PR_CODE, PRV_TYP = p.PRV_TYPE },
                           (c, p) => new { c, p })
                           .Where(x => x.p.PRV_TYPE == providerId
                           && x.p.GOVERNMENT_CODE == country
                           && x.p.TERMINATE_FLAG != "Y"
                           && x.c.COMP_ID == Comp_ID
                           && x.c.CONTRACT_NO == cardExist.CONTRACT_NO
                           && x.c.CLASS_CODE == cardExist.CLASS_CODE
                           && x.c.TYPE == "White"
                           && x.c.ACTIVE == "Y"
                           && (string.IsNullOrEmpty(specialistid) || x.p.DOC_SPEC == specialistid))
                           .Select(x => new
                           {
                               x.p.PR_ANAME,
                               x.p.ADDRESS1,
                               x.p.ADDRESS2,
                               x.p.TEL1,
                               x.p.TEL2,
                               x.p.PR_DESC
                           });


                        providerList = providerList.Union(whiteList).ToList();
                               
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else if (coverdRelation == 2)
                    {
                        var providerList = 
                            db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" 
                                                     && (p.PROV_DEGREE == coverdRelation.ToString() || p.PROV_DEGREE == "3") 
                                                     && p.GOVERNMENT_CODE == country && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)
                                                     && !db.CompContractClassProviders.Any(b => b.COMP_ID == Comp_ID && b.CONTRACT_NO == cardExist.CONTRACT_NO
                                                                      && b.CLASS_CODE == cardExist.CLASS_CODE && b.PRV_TYP == providerId && b.TYPE == "Black" && b.ACTIVE == "Y" && b.PR_CODE == p.PR_CODE))
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

                        var whiteList = db.CompContractClassProviders
                          .Join(db.SERV_PROVIDERS_NEW,
                           c => new { PR_CODE = c.PR_CODE, PRV_TYP = c.PRV_TYP },
                           p => new { PR_CODE = p.PR_CODE, PRV_TYP = p.PRV_TYPE },
                          (c, p) => new { c, p })
                          .Where(x => x.p.PRV_TYPE == providerId
                          && x.p.GOVERNMENT_CODE == country
                          && x.p.TERMINATE_FLAG != "Y"
                          && x.c.COMP_ID == Comp_ID
                          && x.c.CONTRACT_NO == cardExist.CONTRACT_NO
                          && x.c.CLASS_CODE == cardExist.CLASS_CODE
                          && x.c.TYPE == "White"
                          && x.c.ACTIVE == "Y"
                          && (string.IsNullOrEmpty(specialistid) || x.p.DOC_SPEC == specialistid))
                          .Select(x => new
                          {
                              x.p.PR_ANAME,
                              x.p.ADDRESS1,
                              x.p.ADDRESS2,
                              x.p.TEL1,
                              x.p.TEL2,
                              x.p.PR_DESC
                          });


                        providerList = providerList.Union(whiteList).ToList();
                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else
                    {
                        var providerList = 
                            db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" 
                                                     && p.PROV_DEGREE == coverdRelation.ToString() && p.GOVERNMENT_CODE == country 
                                                     && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)
                                                     && !db.CompContractClassProviders.Any(b => b.COMP_ID == Comp_ID && b.CONTRACT_NO == cardExist.CONTRACT_NO
                                                                      && b.CLASS_CODE == cardExist.CLASS_CODE && b.PRV_TYP == providerId && b.TYPE == "Black" && b.ACTIVE == "Y" && b.PR_CODE == p.PR_CODE)
                                                     )
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

                        var whiteList = db.CompContractClassProviders
                        .Join(db.SERV_PROVIDERS_NEW,
                         c => new { PR_CODE = c.PR_CODE, PRV_TYP = c.PRV_TYP },
                         p => new { PR_CODE = p.PR_CODE, PRV_TYP = p.PRV_TYPE },
                        (c, p) => new { c, p })
                        .Where(x => x.p.PRV_TYPE == providerId
                        && x.p.GOVERNMENT_CODE == country
                        && x.p.TERMINATE_FLAG != "Y"
                        && x.c.COMP_ID == Comp_ID
                        && x.c.CONTRACT_NO == cardExist.CONTRACT_NO
                        && x.c.CLASS_CODE == cardExist.CLASS_CODE
                        && x.c.TYPE == "White"
                        && x.c.ACTIVE == "Y"
                        && (string.IsNullOrEmpty(specialistid) || x.p.DOC_SPEC == specialistid))
                        .Select(x => new
                        {
                            x.p.PR_ANAME,
                            x.p.ADDRESS1,
                            x.p.ADDRESS2,
                            x.p.TEL1,
                            x.p.TEL2,
                            x.p.PR_DESC
                        });


                        providerList = providerList.Union(whiteList).ToList();


                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }
            }

        }
        string getncardapproval(string crd)
        {
            string ncrd = "";

            System.Data.DataTable dtoldcrdaprov = new System.Data.DataTable();
            dtoldcrdaprov = dbData.RunReader(@"SELECT CLOSE_EMP_DATA.CARD_ID FROM DMS_TEST.CLOSE_EMP_DATA WHERE (CLOSE_EMP_DATA.TRANS_TYP = 'D' OR CLOSE_EMP_DATA.TRANS_TYP = 'L') AND CLOSE_EMP_DATA.N_CARD = '" + crd + "'");

            if (dtoldcrdaprov.Rows.Count > 0 && dtoldcrdaprov.Rows[0][0].ToString() != string.Empty)
                ncrd = dtoldcrdaprov.Rows[0][0].ToString();
            else
                ncrd = "";

            return ncrd;
        }
        public JsonResult getAprovalData(string CardId, string startDate, string endDate)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            DataTable dt = new DataTable();

            DateTime dat1, dat2;

            string oldcrd = getncardapproval(CardId);

            oldcrd = oldcrd != "" ? oldcrd : CardId;

            dat1 = string.IsNullOrEmpty(startDate) ? new DateTime(2020, 1, 1) : (Convert.ToDateTime(startDate)).Date;
            dat2 = string.IsNullOrEmpty(endDate) ? DateTime.Now.Date : (Convert.ToDateTime(endDate)).Date;

            dt = dbData2.getApproval(CardId, dat1, dat2, oldcrd);


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


        public ActionResult CreateRequestMedical(string cardId)
        {
            Enum_RequestsViewModel ENUM_REQUESTSViewModel = new Enum_RequestsViewModel();
          
            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;
            //var datenow = DateTime.Now.Date;

            ENUM_REQUESTSViewModel.CompName = cardId.Substring(0, cardId.IndexOf('-'));
            ENUM_REQUESTSViewModel.CARD_ID = cardId;

            return View(ENUM_REQUESTSViewModel);

          //  return View();
        }

        public ActionResult CreateRequestChronic(string cardId)
        {
            Enum_RequestsViewModel ENUM_REQUESTSViewModel = new Enum_RequestsViewModel();

            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;
            //var datenow = DateTime.Now.Date;
            
            ENUM_REQUESTSViewModel.CompName = cardId.Substring(0, cardId.IndexOf('-'));
            ENUM_REQUESTSViewModel.CARD_ID = cardId;

            return View(ENUM_REQUESTSViewModel);
            //Enum_RequestsViewModel ENUM_REQUESTSViewModel = new Enum_RequestsViewModel();

            //if (User.IsInRole("HR_Admin"))
            //{
            //    ViewBag.compnum = null;
            //    var userid = User.Identity.GetUserId();
            //    var compines = db.HrAdminCompanies.Where(x => x.UserId == userid).Select(c => c.CompId).ToList();
            //    if (compines[0] == "All")
            //    {
            //        var companyname = db.Contract_Comp
            //            .Select(l => new
            //            {
            //                Code = l.C_COMP_ID,
            //                Name = l.C_ENAME + " || " + l.C_COMP_ID

            //            }).ToList();
            //        SelectList companylist = new SelectList(companyname, "Code", "Name");
            //        ViewBag.company = companylist;
            //    }
            //    else
            //    {
            //        var companyname = (from comp in compines
            //                           join contCo in db.Contract_Comp
            //                           on int.Parse(comp) equals contCo.C_COMP_ID
            //                           select new
            //                           {
            //                               Code = contCo.C_COMP_ID,
            //                               Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
            //                           }).ToList();
            //        SelectList companylist = new SelectList(companyname, "Code", "Name");
            //        ViewBag.company = companylist;
            //    }
            //}
            //else
            //{
            //    var HrUserNamre = User.Identity.GetUserName();
            //    var comp_id = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            //    ENUM_REQUESTSViewModel.CompName = comp_id;


            //    var comp = Convert.ToInt32(comp_id);
            //    var datenow = DateTime.Now.Date;
            //    var employees = db.Comp_Employees.Where(m => m.C_COMP_ID == comp && m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
            //          .Select(l => new
            //          {
            //              CARD_ID = l.CARD_ID,
            //              EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
            //          }).ToList();
            //    //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //    SelectList addresslist = new SelectList(employees, "CARD_ID", "EMP_ANAME");
            //    ViewBag.address = addresslist;
            //    ViewBag.compnum = comp_id;
            //    if (User.IsInRole("User"))
            //    {
            //        var usr = User.Identity.GetUserId();
            //        var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
            //        ENUM_REQUESTSViewModel.CARD_ID = cardId;
            //        ENUM_REQUESTSViewModel.CompName = cardId.Split('-')[0];
            //        var comp2 = int.Parse(ENUM_REQUESTSViewModel.CompName);
            //        //var employees2 = db.Comp_Employees.Where(m => m.C_COMP_ID == comp2 && m.CARD_ID == cardId &&
            //        //m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
            //        //  .Select(l => new
            //        //  {
            //        //      CARD_ID = l.CARD_ID,
            //        //      EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
            //        //  }).ToList();
            //        ////var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //        //SelectList addresslist2 = new SelectList(employees2, "CARD_ID", "EMP_ANAME");
            //        //ViewBag.address = addresslist2;
            //        var subCards = CardList(cardId);

            //        var cards = subCards.Select(c => new
            //        {
            //            CardIDValue = c.CARD_ID,
            //            CardIdString = c.CARD_ID
            //        }).ToList();
            //        SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
            //        ViewBag.address = Cardlist;

            //        ViewBag.compnum = ENUM_REQUESTSViewModel.CompName;
            //    }
            //}
            //if (id != null)
            //{
            //    var model = db.Enum_Requests.Find(id);
            //    if (model == null)
            //    {
            //        return View(ENUM_REQUESTSViewModel);
            //    }
            //    Enum_RequestsViewModel ViewModel = new Enum_RequestsViewModel();
            //    ViewModel.ID = model.ID;
            //    ViewModel.CompName = model.CARD_ID.Split('-')[0];
            //    ViewModel.CARD_ID = model.CARD_ID;
            //    ViewModel.TYPE = model.TYPE;
            //    ViewModel.TYP_ANAME = model.TYP_ANAME;
            //    ViewModel.PR_ENAME = model.PR_ENAME;
            //    ViewModel.NOTES = model.NOTES;
            //    ViewModel.MAIL_SEND = model.MAIL_SEND;
            //    ViewModel.APPROVAL_IMAGE = model.APPROVAL_IMAGE;
            //    ViewBag.compnum = ViewModel.CompName;
            //    return View(ViewModel);
            //}
            //return View(ENUM_REQUESTSViewModel);
        }
    }
}