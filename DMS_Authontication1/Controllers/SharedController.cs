using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers
{
    public class SharedController : Controller
    {
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        public SharedController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();
        }
        // GET: Shared
        public JsonResult VerificationCode(string CardId, string VerificationCode)
        {

            var emp = db.Comp_Employees.Where(c => c.CARD_ID == CardId && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            EmployeesSMSCode _permision = db.EmployeesSMSCodes.Where(x => x.EmpId == emp.Id && x.IsActive == true).OrderByDescending(x => x.Id).FirstOrDefault();
            if (_permision != null && _permision.SMSCode == VerificationCode)
            {

                return Json(new { Validation = true, Message = "Verified" });
            }
            return Json(new { Validation = false, Message = "invalid Code", Limit = 0, CeilingPert = 0 });
        }
        public JsonResult VerificationCardForCode(string CardId)
        {

            var emp = db.Comp_Employees.Where(c => c.CARD_ID == CardId && c.INS_START_DATE <= DateTime.Now && c.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            EmployeesSMSCode _permision = db.EmployeesSMSCodes.Where(x => x.EmpId == emp.Id && x.IsActive == true).OrderByDescending(x => x.Id).FirstOrDefault();
            if (_permision != null)
            {

                return Json(new { Validation = true});
            }
            return Json(new { Validation = false });
        }
    }
}