using DMS_Authontication1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.ControlPanal
{
    public class ControlPanelController : Controller
    {
        public ApplicationDbContext db;
        public ControlPanelController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            db = new ApplicationDbContext();
        }
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }
        [Authorize]
        // GET: ControlPanel
        public ActionResult Main()
        {

            ViewBag.UserName =  User.Identity.Name;
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.PhoneNumber=user.PhoneNumber;
            ViewBag.Address = user.Address;
            ViewBag.Email = user.Email;
            return View(user);
        }
    }
}