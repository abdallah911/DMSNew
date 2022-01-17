using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using System.Collections;

namespace DMS_Authontication1.Controllers
{
    public class Doctors_PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult CreatePermission(string id)
        {

          ApplicationUser aspNetUser = db.Users.Find(id);
           ViewBag.CurrentUserName = aspNetUser.UserName;
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", aspNetUser.Id);
            ViewBag.PermissionId = new SelectList(db.DailyPermission, "Id", "Name");
          //  ViewBag.MPermissionId = new SelectList(db.MonthlyPermissions, "Id", "Name");
            ViewBag.CurrentUser = id;
            //  ViewBag.PermissionType = new SelectList(db.Doctors_Permissions, "Id", "");
            var doctorPermission = db.Doctors_Permissions.Where(d => d.UserId == id&&d.permissionType=="Daily").Include(m => m.DailyPermission).ToList();

            List<DoctorPermissionViewModel> doctorPermissions = new List <DoctorPermissionViewModel>();
           
            
            foreach(var item in doctorPermission)
            {
                var dp = new DoctorPermissionViewModel();                   
                dp.Id = item.Id;
                dp.permissionType = item.permissionType;
                dp.UserId = item.UserId;
                dp.Status = item.Status;
               // dp.DailyPermission.Id = item.DailyPermission.Id;
                dp.PermissionName = item.DailyPermission.Name;
                doctorPermissions.Add(dp);
            }
            return View(doctorPermissions);
        }
        public ActionResult CreateMonthlyPermission(string id)
        {
            ApplicationUser aspNetUser = db.Users.Find(id);
            ViewBag.CurrentUserName = aspNetUser.UserName;
            ViewBag.UserId = new SelectList(db.Users,"Id", "UserName", aspNetUser.Id);
            ViewBag.PermissionId = new SelectList(db.MonthlyPermission, "Id", "Name");
            ViewBag.CurrentUser = id;
            //  ViewBag.PermissionType = new SelectList(db.Doctors_Permissions, "Id", "");
            var doctorspermissions = db.Doctors_Permissions.Where(d => d.UserId == id&&d.permissionType == "Monthly").Include(m=>m.MonthlyPermission).ToList();

            List<DoctorPermissionViewModel> doctorPermissions = new List<DoctorPermissionViewModel>();
                 //fetch list ( database) to list  (ViewModel) To perview Data 

            foreach (var item in doctorspermissions)
            {
                var dp = new DoctorPermissionViewModel();
                dp.Id = item.Id;
                dp.permissionType = item.permissionType;
                dp.UserId = item.UserId;
                dp.Status = item.Status;
                // dp.DailyPermission.Id = item.DailyPermission.Id;
                dp.PermissionName = item.MonthlyPermission.Name;
                doctorPermissions.Add(dp);
            }
            return View(doctorPermissions);
        }

        public JsonResult InsertPermission(List<DoctorPermissionViewModel> doctors_Permissions)
        {
            using (ApplicationDbContext entities = new ApplicationDbContext())
            {
                //Truncate Table to delete all old records.
                entities.Database.ExecuteSqlCommand("Delete from Doctors_Permissions where UserId='" + doctors_Permissions.FirstOrDefault().UserId + "' and permissionType='"+doctors_Permissions.FirstOrDefault().permissionType+"'");
                
                //Check for NULL.
                if (doctors_Permissions == null)
                {
                    doctors_Permissions = new List<DoctorPermissionViewModel>();
                }

                //Loop and insert records.
                foreach (var doctors_Permission in doctors_Permissions)
                {
                    //var id = db.DailyPermissions.Where(d => d.Name == doctors_Permission.PermissionId.ToString()).Select(s => s.Id);
                    //doctors_Permission.PermissionId = Convert.ToInt32(id);
                    //if (doctors_Permission.permissionType == "Daily")
                    //{
                    //    doctors_Permission.DailyPermission.Name
                    //}
                    //else
                    //{

                    //}
                    

                        Doctors_Permissions model = new Doctors_Permissions()
                        {

                            permissionType = doctors_Permission.permissionType,
                            UserId = doctors_Permission.UserId,
                            Status = doctors_Permission.Status

                        };


                    if (doctors_Permission.permissionType == "Daily")
                    {
                        foreach (var item in db.DailyPermission)
                        {
                            if (doctors_Permission.PermissionName == item.Name)
                            {
                                model.DailyPermissionId = item.Id;
                                //model.MonthlyPermissionId = 1;

                                //doctors_Permission.DailyPermissionId = item.Id;

                            }
                        }
                    }
                    else
                    {
                        foreach (var item in db.MonthlyPermission)
                        {
                            if (doctors_Permission.PermissionName == item.Name)
                            {
                                model.MonthlyPermissionId = item.Id;
                                //model.DailyPermissionId = 1;


                            }
                        }

                    }
                    // model.DailyPermissionId = 1;
                    //      model.MonthlyPermission.Id = 1;

                    entities.Doctors_Permissions.Add(model);
                    }
                    int insertedRecords = entities.SaveChanges();
             
                return Json(insertedRecords);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
