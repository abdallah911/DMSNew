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
using DMS_Authontication1.Data_Function;

namespace DMS_Authontication1.Controllers
{
    public class DashboardController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        DBData dbApproval;

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
            dbApproval = new DBData();
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
        public JsonResult GetTypeProviderData()
        {
            var data = new[]
            {
        new { company = "أطباء", gross = 1790324.00m, net = 1790324.00m, percent = 1.40 },
        new { company = "خدمات خارج الهيئة الطبية", gross = 1261949.53m, net = 791818.15m, percent = 0.62 },
        new { company = "خدمات علاج طبيعى", gross = 575770.00m, net = 556822.00m, percent = 0.44 },
        new { company = "صيدليات", gross = 59261164.36m, net = 48305251.82m, percent = 37.91 },
        new { company = "مراكز أشعة", gross = 5679658.45m, net = 5673220.37m, percent = 4.45 },
        new { company = "مستشفيات", gross = 78767467.85m, net = 65281897.50m, percent = 51.23 },
        new { company = "معامل تحاليل", gross = 5037100.53m, net = 5037010.34m, percent = 3.95 }
    };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceData()
        {
            var data = new[]
            {
        new { company = "أدويه", gross = 59330518.40m, net = 48362434.90m, percent = 37.95 },
        new { company = "اسنان", gross = 662699.50m, net = 419370.50m, percent = 0.33 },
        new { company = "أشعات", gross = 7798013.90m, net = 7424979.61m, percent = 5.83 },
        new { company = "بصريات", gross = 195044.00m, net = 93587.00m, percent = 0.07 },
        new { company = "تحاليل طبية", gross = 6398044.34m, net = 6152742.20m, percent = 4.83 },
        new { company = "خدمات داخل المستشفيات", gross = 174310.00m, net = 173444.00m, percent = 0.14 },
        new { company = "خدمات عيادة خارجية", gross = 2672600.46m, net = 1531471.36m, percent = 1.20 },
        new { company = "علاج طبيعي", gross = 897078.06m, net = 834315.06m, percent = 0.65 },
        new { company = "عمليات جراحية", gross = 66241152.59m, net = 56334589.72m, percent = 44.21 },
        new { company = "كشف طبيب", gross = 7877715.50m, net = 6068378.32m, percent = 4.76 },
        new { company = "مناظير", gross = 7893.75m, net = 6698.75m, percent = 0.01 },
        new { company = "ولادة", gross = 118364.22m, net = 34332.76m, percent = 0.03 }
    };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceDetailsData()
        {
            var data = new[]
            {
        new { company = "ادوية مزمن - خارج الهيئة الطبية", gross = 8127.00m, net = 5512.70m, percent = 0.004 },
        new { company = "ادوية مزمن - داخل الهيئة الطبية", gross = 50023043.87m, net = 40827419.29m, percent = 32.04 },
        new { company = "ادوية يومي - خارج الهيئة الطبية", gross = 60304.04m, net = 50896.08m, percent = 0.04 },
        new { company = "ادوية يومي - داخل الهيئة الطبية", gross = 9238120.49m, net = 7477832.53m, percent = 5.87 },
        new { company = "اشعات - خارج الهيئة الطبية", gross = 9836.25m, net = 7560.00m, percent = 0.01 },
        new { company = "اشعات - داخل المستشفيات", gross = 2111052.80m, net = 1746732.84m, percent = 1.37 },
        new { company = "اشعات - داخل الهيئة الطبية", gross = 5677829.85m, net = 5671391.77m, percent = 4.45 },
        new { company = "باكدج عمليات - داخل الهيئة الطبية", gross = 66242075.59m, net = 56335364.02m, percent = 44.21 },
        new { company = "بصريات - خارج الهيئة الطبية", gross = 195044.00m, net = 93587.00m, percent = 0.07 },
        new { company = "تحاليل طبية - خارج الهيئة الطبية", gross = 32505.00m, net = 28736.00m, percent = 0.02 },
        new { company = "تحاليل طبية - داخل المستشفيات", gross = 1323613.81m, net = 1082170.86m, percent = 0.85 },
        new { company = "تحاليل طبية - داخل الهيئة الطبية", gross = 5037100.53m, net = 5037010.34m, percent = 3.95 },
        new { company = "خدمات اسنان - خارج الهيئة الطبية", gross = 660241.50m, net = 417263.50m, percent = 0.33 },
        new { company = "خدمات اسنان - داخل المستشفيات", gross = 2458.00m, net = 2107.00m, percent = 0.002 },
        new { company = "خدمات عيادة خارجية - خارج الهيئة الطبية", gross = 144312.52m, net = 134878.11m, percent = 0.11 },
        new { company = "خدمات عيادة خارجية - داخل الهيئة الطبية", gross = 2694065.69m, net = 1561604.00m, percent = 1.23 },
        new { company = "علاج طبيعي - خارج الهيئة الطبية", gross = 480.00m, net = 384.00m, percent = 0.000 },
        new { company = "علاج طبيعي - داخل المستشفيات", gross = 322378.06m, net = 278659.06m, percent = 0.22 },
        new { company = "علاج طبيعي - داخل الهيئة الطبية", gross = 575770.00m, net = 556822.00m, percent = 0.44 },
        new { company = "كشف طبيب - خارج الهيئة الطبية", gross = 4085.00m, net = 2668.00m, percent = 0.002 },
        new { company = "كشف طبيب - داخل المستشفيات", gross = 7828190.50m, net = 6020270.32m, percent = 4.72 },
        new { company = "كشف طبيب - داخل الهية الطبية", gross = 50610.00m, net = 50610.00m, percent = 0.04 },
        new { company = "مناظير", gross = 13826.00m, net = 12532.00m, percent = 0.01 },
        new { company = "ولادة - خارج الهيئة الطبية", gross = 118364.22m, net = 34332.76m, percent = 0.03 }
    };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetConsumType()
        {
            var data = new[]
     {
        new { company = "موظف", gross = 95676812m, net = 77712318m, percent = 49.62 },
        new { company = "معاش", gross = 90552903m, net = 78902995m, percent = 50.38 }
    };


            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetConsumGender()
        {
            var data = new[]
   {
        new { company = "أنثى", gross = 39521526m, net = 30678886m, percent = 19.59 },
        new { company = "ذكر",  gross = 146708189m, net = 125936427m, percent = 80.41 }
    };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTypeProvider()
        {
            var data = new[]
            {
        new { typeprovider = "عيادات أطباء",             net = 361m,     percent = 0.43 },
        new { typeprovider = "خدمات خارج الهيئة الطبية", net = 739m,     percent = 0.88 },
        new { typeprovider = "مراكز علاج طبيعي",         net = 801m,     percent = 0.95 },
        new { typeprovider = "صيدليات",                  net = 49666m,   percent = 58.98 },
        new { typeprovider = "مراكز اشعة",               net = 3197m,    percent = 3.80 },
        new { typeprovider = "مستشفيات",                 net = 23625m,   percent = 28.05 },
        new { typeprovider = "معامل تحاليل",             net = 5822m,    percent = 6.91 }
    };

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAllConsumGroup()
        {
            var data = new[]
{
    new { company = "Pharmacy",   gross = 7783030m,   net = 6809483.489m, percent = 4.35 },
    new { company = "Lab",        gross = 48331006m,  net = 40914950.34m, percent = 26.12 },
    new { company = "Ray",        gross = 42652394m,  net = 33662545.39m, percent = 21.49 },
    new { company = "OutPatient", gross = 85807314m,  net = 73853841.5m,  percent = 47.16 },
    new { company = "InPatient",  gross = 1655971m,   net = 1374491.839m, percent = 0.88 }
};

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllConsumMed()
        {
            var data = new[]
{
    new { company = "Daily",   gross = 7783030m,   net = 6809483.489m, percent = 4.35 },
    new { company = "Chronic",        gross = 48331006m,  net = 40914950.34m, percent = 26.12 }
   
};

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChartData()
        {
            var data = new[] {
        new { category = "", value = 32000 },
        new { category = "", value = 19000 },
      
    };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTypeProviderLive()
        {
            var data = new[]
            {
        new { typeprovider = "Pharmacy",             net = 361 },
        new { typeprovider = "Lab", net = 739 },
        new { typeprovider = "Ray",         net = 801 },
        new { typeprovider = "OutPatient",                  net = 49666 },
        new { typeprovider = "InPatient",                 net = 23625 }
    };           
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoadPartial(string viewName)
        {
            // Secure only known partials
            switch (viewName)
            {
                case "_Live":
                case "_Screen2":
                case "_Screen3":
                    return PartialView($"~/Views/Dashboard/{viewName}.cshtml");
                default:
                    return HttpNotFound();
            }
        }
    }
}
