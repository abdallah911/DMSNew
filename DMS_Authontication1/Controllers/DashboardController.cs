using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using DMS_Authontication1.ViewModel;
using DMS_Authontication1.Models;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Globalization;
using System.Threading;
using Microsoft.AspNet.Identity;


namespace DMS_Authontication1.Controllers
{
    public class DashboardController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public JsonResult GetNotifications()
        {
            //search 
            //  MED_CARD mED_CARD=db.MED_CARD.Where()
            //add
            List<Notification> notifications = new List<Notification>();
            string UserName = User.Identity.Name;
            if (User.IsInRole("Admin"))
            {
                notifications = db.Notifications.Where(x => x.IsRead == false && x.SentTo == "Admin")
                     .AsEnumerable().Where(x => x.CreatedDate.AddDays(7).Date > DateTime.Now.Date)
               .OrderByDescending(x => x.Id).Select(x => new Notification
               {
                   DetailsURL = x.DetailsURL,
                   Id = x.Id,
                   TypeNmae = x.TypeNmae,
                   Details = x.Details
               })
               .ToList();
            }
            else if (User.IsInRole("AdminHelth"))
            {
                notifications = db.Notifications.Where(x => x.IsRead == false && x.SentTo == "AdminHelth")
                     .AsEnumerable().Where(x => x.CreatedDate.AddDays(7).Date > DateTime.Now.Date)
               .OrderByDescending(x => x.Id).Select(x => new Notification
               {
                   DetailsURL = x.DetailsURL,
                   Id = x.Id,
                   TypeNmae = x.TypeNmae,
                   Details = x.Details
               })
               .ToList();
            }
            else
            {
                notifications = db.Notifications.Where(x => x.IsRead == false && x.SentTo == UserName)
                    .AsEnumerable().Where(x => x.CreatedDate.AddDays(7).Date > DateTime.Now.Date)
               .OrderByDescending(x => x.Id).Select(x => new Notification
               {
                   DetailsURL = x.DetailsURL,
                   Id = x.Id,
                   TypeNmae = x.TypeNmae,
                   Details = x.Details
               })
               .ToList();
            }


            return new JsonResult { Data = notifications, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public DashboardController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();

        }
        public ActionResult Index()
        {

            return View();

        }
        public List<int> GetAllCompanyCodes()
        {
            List<int> company = new List<int>();

            if (User.IsInRole("HR_Admin"))
            {
                var userId = User.Identity.GetUserId();
                var compines = db.HrAdminCompanies
                                 .Where(x => x.UserId == userId)
                                 .Select(c => c.CompId)
                                 .ToList();

                if (compines.Count > 0 && compines[0] == "All")
                {
                    company = db.Contract_Comp
                                     .Select(c => c.C_COMP_ID)
                                     .ToList();
                }
                else
                {
                    company = compines
                        .Select(int.Parse)
                        .ToList();
                }
            }
            else
            {
                var userName = User.Identity.GetUserName();
                int compa = int.Parse(UserDB.Users
                                    .Where(u => u.UserName == userName)
                                    .FirstOrDefault()
                                    .Provider);

                company = new List<int> { compa };
            }

            return company;
        }

        public JsonResult GetCardData()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;



            //var infocomp = ;




            return new JsonResult { };
            //return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetCompData()
        {
            var data = new[]
            {
                new { company = "500118", gross = 7783030m, net = 6809483.489m, percent = 4.35 },
                new { company = "500119", gross = 48331006m, net = 40914950.34m, percent = 26.12 },
                new { company = "500120", gross = 42652394m, net = 33662545.39m, percent = 21.49 },
                new { company = "500121", gross = 85807314m, net = 73853841.5m, percent = 47.16 },
                new { company = "500122", gross = 1655971m, net = 1374491.839m, percent = 0.88 }
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
