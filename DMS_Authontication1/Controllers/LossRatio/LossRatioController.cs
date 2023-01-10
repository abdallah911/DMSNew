using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel.AfterSale;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.LossRatio
{
    [Authorize(Roles = "Admin,LossRatio")]
    public class LossRatioController : Controller
    {
        #region Fields

        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationDbContext db2 = new ApplicationDbContext();
        #endregion

        #region Actions

        // GET: LossRatio
        public ActionResult Index()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "Name", "Name");
            ViewBag.company = companylist;
            if (User.IsInRole("Admin"))
            {
                var usersname = db2.Users.Where(u => u.Type == "AfterSale").Select(c => new
                {
                    user = c.UserName,
                    Name = c.FName + " " + c.LName

                }).ToList();
                SelectList userslist = new SelectList(usersname, "user", "Name");
                ViewBag.users = userslist;
            }

            return View();
        }
        public ActionResult Map()
        {
            return View();
        }

        // GET: AfterSale/Create
        public ActionResult Create()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "Name", "Name");
            ViewBag.company = companylist;
            return View();
        }


        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AfterSale afterSale = db.AfterSales.Find(id);
            if (afterSale == null)
            {
                return HttpNotFound();
            }
            return View(afterSale);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AfterSale afterSale = db.AfterSales.Find(id);
            if (afterSale == null)
            {
                return HttpNotFound();
            }
            if (afterSale.IsNew == false)
            {
                var companyname = db.Contract_Comp.Select(c => new
                {
                    Name = c.C_ANAME + " || " + c.C_COMP_ID

                }).ToList();
                SelectList companylist = new SelectList(companyname, "Name", "Name");
                ViewBag.company = companylist;
                return View(afterSale);
            }
            return View(afterSale);
        }

        // GET: AfterSales1/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AfterSale afterSale = db.AfterSales.Find(id);
            if (afterSale == null)
            {
                return HttpNotFound();
            }
            return View(afterSale);
        }

        // POST: AfterSales1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            AfterSale afterSale = db.AfterSales.Find(id);
            afterSale.IsDeleted = true;
            afterSale.DeletedBy = User.Identity.GetUserName();
            afterSale.DeletedDate = DateTime.Now;
            //db.AfterSales.Remove(afterSale);
            db.Entry(afterSale).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="CompName"></param>
        /// <param name="username"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintVisits(string from, string to
            , string CompName, string username)
        {
            string CompNa,usr;
            DateTime DateFrom, DateTo;

            CompNa = CompName == string.Empty ? "" : CompName;


            DateFrom = from == string.Empty ?  new DateTime(2020,06,01) : (Convert.ToDateTime(from)).Date;
            DateTo = to == string.Empty ? DateTime.Now.AddDays(1) : (Convert.ToDateTime(to)).Date;
            if(User.IsInRole("Admin"))
            {
                if(string.IsNullOrEmpty(username))
                {
                    usr = "";
                }
                else
                {
                    usr = username;

                }
            }
            else
            {
                usr = User.Identity.GetUserName();

            }
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/AfterSale"), "AfterSales.rpt"));

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@from", DateFrom);
            rd.SetParameterValue("@to", DateTo);
            rd.SetParameterValue("@cmp", CompNa);
            rd.SetParameterValue("@usr", usr);

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
                return File(stream, "application/pdf", DateFrom.ToString("ddMMyyyy") + "AfterSale.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="CompName"></param>
        /// <param name="username"></param>
        /// <returns>Report as EXCEL</returns>
        public ActionResult PrintXlxVisits(string from, string to
            , string CompName, string username)
        {
            string CompNa, usr;
            DateTime DateFrom, DateTo;

            CompNa = CompName == string.Empty ? "" : CompName;


            DateFrom = from == string.Empty ? new DateTime(2020, 06, 01) : (Convert.ToDateTime(from)).Date;
            DateTo = to == string.Empty ? DateTime.Now.AddDays(1) : (Convert.ToDateTime(to)).Date;
            if (User.IsInRole("Admin"))
            {
                if (string.IsNullOrEmpty(username))
                {
                    usr = null;
                }
                else
                {
                    usr = username;

                }
            }
            else
            {
                usr = User.Identity.GetUserName();

            }
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/AfterSale"), "AfterSales.rpt"));

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@from", DateFrom);
            rd.SetParameterValue("@to", DateTo);
            rd.SetParameterValue("@cmp", CompNa);
            rd.SetParameterValue("@usr", usr);

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
                return File(stream, "application/xls", DateFrom.ToString("ddMMyyyy") + "AfterSale.xls");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Help Json
        public JsonResult GetBranshs(int id)
        {
            var listBranshs = db.Company_Cost_Center.Where(m => m.C_COMP_ID == id).Select(
                c => new
                {
                    COST_CODE = c.COST_CODE,
                    A_NAME = c.A_NAME
                }).ToList();
            //SelectList Branshslist = new SelectList(listBranshs, "COST_CODE", "A_NAME");


            return new JsonResult { Data = new { providerslist = listBranshs }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult SaveVisit(string CompName, string BranchName, string PersonName, string VisitReasonList, bool IsNew
                          , DateTime VisitDate, string Region, string PhoneNumber, string FeedBackText, string Note,
                            string DateOfStartMeeting, string DateOfEndtMeeting, string Duration)
        {
            bool feed = false;
            DateTime? feedDate = null;
            if (!string.IsNullOrEmpty(FeedBackText))
            {
                feed = true;
                feedDate = DateTime.Now;
            }
            var afterSale = new AfterSale
            {
                CompanyName = CompName,
                BranchName = BranchName,
                PersonName = PersonName,
                VisitDate = VisitDate,
                VisitReason = VisitReasonList,
                Region = Region,
                Phone = PhoneNumber,
                FeedBackDate = feedDate,
                FeedBackText = FeedBackText,
                CreatedBy = User.Identity.GetUserName(),
                CreatedDate = DateTime.Now,
                IsDeleted = false,
                HasFeedBack = feed,
                Note = Note,
                IsNew = IsNew,
                MeetingTime = Duration,
                StartMeeting = DateOfStartMeeting,
                EndMeeting = DateOfEndtMeeting

            };
            db.AfterSales.Add(afterSale);
            var x = db.SaveChanges();
            if (x == 1)
            {
                return new JsonResult { Data = new { result = "Visit Add Successfully ", msg = "OK" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            else
            {
                return new JsonResult { Data = new { result = "Invalid Request", msg = "NO" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public JsonResult EditVisit(long Id, string CompName, string BranchName, string PersonName, string VisitReasonList, bool IsNew
                          , DateTime VisitDate, string Region, string PhoneNumber, string FeedBackText, string Note,
                           string DateOfStartMeeting, string DateOfEndtMeeting, string Duration)
        {
            AfterSale model = db.AfterSales.Find(Id);
            if (model == null)
            {
                return new JsonResult { Data = new { result = "Invalid Request", msg = "NO" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            bool feed = false;
            DateTime? feedDate = null;
            if (!string.IsNullOrEmpty(FeedBackText) && string.IsNullOrEmpty(model.FeedBackText))
            {
                feed = true;
                feedDate = DateTime.Now;
            }
            else if (!string.IsNullOrEmpty(FeedBackText) && !string.IsNullOrEmpty(model.FeedBackText))
            {
                feed = true;
                feedDate = model.FeedBackDate;
            }
            model.CompanyName = CompName;
            model.BranchName = BranchName;
            model.PersonName = PersonName;
            model.VisitDate = VisitDate;
            model.VisitReason = VisitReasonList;
            model.Region = Region;
            model.Phone = PhoneNumber;
            model.FeedBackDate = feedDate;
            model.FeedBackText = FeedBackText;
            model.IsDeleted = false;
            model.HasFeedBack = feed;
            model.Note = Note;
            model.IsNew = model.IsNew;
            model.UpdatedBy = User.Identity.GetUserName();
            model.UpdatedDate = DateTime.Now;
            model.MeetingTime = Duration;
            model.StartMeeting = DateOfStartMeeting;
            model.EndMeeting = DateOfEndtMeeting;
            db.Entry(model).State = EntityState.Modified;
            var x = db.SaveChanges();

            return new JsonResult { Data = new { result = "Visit Update Successfully ", msg = "OK" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult AfterSaleList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch, string From = "", string To = "", string CompName = "", string userName = "")
        {
            //var VisitList = db.AfterSales.Where(a => a.CreatedBy == user && a.IsDeleted == false).ToList();

            if (User.IsInRole("Admin"))
            {

                // if admin search with( date from and to only) or with (company name or username)
                if (From != "" & To != "")
                {
                    DateTime dateFrom = Convert.ToDateTime(From);
                    DateTime dateTo = Convert.ToDateTime(To);
                    var result1 = new
                    {
                        sEcho = sEcho,
                        aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains( CompName ): true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).AsEnumerable()
                       .Select(l =>
                       new
                       {
                           Id = l.Id,
                           CompanyName = l.CompanyName,
                           BranchName = l.BranchName,
                           VisitReason = l.VisitReason,
                           VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                           PersonName = l.PersonName
                       })
                       .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.AfterSales.Where(r => r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) >= dateTo)).Count(),
                        iTotalDisplayRecords = db.AfterSales.Where(r => r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) >= dateTo)).Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }


                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)
                       ).AsEnumerable()
                       .Select(l =>
                    new
                    {
                        Id = l.Id,
                        CompanyName = l.CompanyName,
                        BranchName = l.BranchName,
                        VisitReason = l.VisitReason,
                        VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                        PersonName = l.PersonName
                    })
                    .Skip(iDisplayStart).Take(iDisplayLength).ToList(),


                    iTotalRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).Count(),
                    iTotalDisplayRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)
                       ).Count()

                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                var user = User.Identity.GetUserName();


                // if user search with( date from and to only) or with company name 
                if (From != "" & To != "")
                {
                    DateTime dateFrom = Convert.ToDateTime(From);
                    DateTime dateTo = Convert.ToDateTime(To);
                    var result1 = new
                    {
                        sEcho = sEcho,
                        aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).AsEnumerable()
                       .Select(l =>
                       new
                       {
                           Id = l.Id,
                           CompanyName = l.CompanyName,
                           BranchName = l.BranchName,
                           VisitReason = l.VisitReason,
                           VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                           PersonName = l.PersonName
                       })
                       .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).Count(),
                        iTotalDisplayRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }



                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).AsEnumerable()
                    .Select(l =>
                    new
                    {
                        Id = l.Id,
                        CompanyName = l.CompanyName,
                        BranchName = l.BranchName,
                        VisitReason = l.VisitReason,
                        VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                        PersonName = l.PersonName
                    })
                    .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).Count(),
                    iTotalDisplayRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).Count()
                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };




                //var result = new
                //{
                //    sEcho = sEcho,
                //    aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                //    .Where(r => r.CreatedBy == user && r.IsDeleted == false && sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                //   || r.PersonName.Contains(sSearch)) : true).AsEnumerable().Select(l =>
                //     new
                //     {
                //         Id = l.Id,
                //         CompanyName = l.CompanyName,
                //         BranchName = l.BranchName,
                //         VisitReason = l.VisitReason,
                //         VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                //         PersonName = l.PersonName
                //     }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                //    iTotalRecords = db.AfterSales.Count(),
                //    iTotalDisplayRecords = db.AfterSales.Count()
                //};
                ////return Ok(result);
                //return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }


        }

        #endregion

    }
}