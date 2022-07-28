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

namespace DMS_Authontication1.Controllers
{
    public class HomeController : Controller
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

        public HomeController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();

        }
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Search()
        {
            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;

            var address = db.Basic_Data.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_CODE", "BS_ANAME");
            ViewBag.address = addresslist;
            return View();
        }
        public JsonResult GetProviders(string country, int region, int providerId, string specialistid)
        {


            var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid))
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

            return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult getUser(int id)
        {
            string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
            DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);

            var OracleClames = db.Roshitas.Where(x => x.CreatedDate >= firstDayOfMonthTime && x.SyncBy == "Admin").ToList();
            int OracleUserCount = OracleClames.Select(x => x.CreatedBy).Distinct().Count();
            int OracleClamesCount = OracleClames.Count();

            var SqlClames = db.Roshitas.Where(x => x.CreatedDate >= firstDayOfMonthTime && (x.SyncBy == "SQL" || x.SyncBy == "" || x.SyncBy == null)).ToList();
            int SqlUserCount = SqlClames.Select(x => x.CreatedBy).Distinct().Count();
            int SqlClamesCount = SqlClames.Count();
            return Json(new { SqlUserCount, OracleUserCount, SqlClamesCount, OracleClamesCount }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProviderClames(int id)
        {
            string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
            DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);

            var PharmacyClames = db.Roshitas.Where(x => x.CreatedDate >= firstDayOfMonthTime && (x.Manager == "Daily" || x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic")).GroupBy(x => x.CreatedBy)
          .Select(l => new
          {
              provider = l.Key,
              clames = l.Count()
          })
                .ToList();
            return Json(new { PharmacyClames }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCurrentProviderClames(int id)
        {
            string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
            DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);

            var UserClames = db.Roshitas.Where(x => x.CreatedDate >= firstDayOfMonthTime && x.CreatedBy == User.Identity.Name).GroupBy(x => x.Manager)
          .Select(l => new
          {
              manger = l.Key,
              clames = l.Count()
          })
                .ToList();
            return Json(new { UserClames }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getMangerClames(int id)
        {
            string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
            DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);

            var MangerClames = db.Roshitas.Where(x => x.CreatedDate >= firstDayOfMonthTime && x.Manager != "Doctor_Chronic" && x.Manager != "Empty").GroupBy(x => x.Manager)
          .Select(l => new
          {
              manger = l.Key,
              clames = l.Count()
          })
                .ToList();
            return Json(new { MangerClames }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OnlineSystem()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Services()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CompanyProfile()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Clients()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Department()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(FormCollection collection)
        {
            string Name = Convert.ToString(collection["name"]);
            string email = Convert.ToString(collection["email"]);
            string telephone = Convert.ToString(collection["telephone"]);
            string comments = Convert.ToString(collection["comments"]);

            if (ModelState.IsValid)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(email);
                mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["Email"]));

                mailMessage.Subject = "Dms";
                mailMessage.SubjectEncoding = System.Text.Encoding.Default;
                mailMessage.Body = comments + "   " + email;
                mailMessage.BodyEncoding = System.Text.Encoding.Default;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"], ConfigurationManager.AppSettings["Password"]);
                client.EnableSsl = true;

                client.Send(mailMessage);



            }
            return View();
        }

        public FileResult DownloadAttachment(string FileName)
        {
            var path = System.IO.Path.Combine(Server.MapPath("/Content/EmployeesRequestsImage/"), FileName);
            if (System.IO.File.Exists(path))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            return null;
        }

    }
}
