using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR,HR_Admin")]
    public class ContractorsController : Controller
    {
        #region Properties
        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationUserManager _userManager;
        ApplicationDbContext myEntities = new ApplicationDbContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        #region Actions
        // GET: Company Index
        public ActionResult Index()
        {
            if (User.IsInRole("HR_Admin"))
            {
                ViewBag.CompName = null;
                var userid = User.Identity.GetUserId();
                var compines = db.HrAdminCompanies.Where(x => x.UserId == userid).Select(c => c.CompId).ToList();
                if (compines[0] == "All")
                {
                    var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
                else
                {
                    var companyname = (from comp in compines
                                       join contCo in db.Contract_Comp
                                       on int.Parse(comp) equals contCo.C_COMP_ID
                                       select new
                                       {
                                           Code = contCo.C_COMP_ID,
                                           Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
                                       }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
            }
            else
            {

                var HrUserNamre = User.Identity.GetUserName();
                var compcode = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.CompName = compcode;
            }
            return View();
        }

        // GET: Providers Index
        public ActionResult ProvidersIndex()
        {
            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;
            return View();
        }

        #endregion

        #region Helper Methods

        public JsonResult CompContractList(int compId, string Type, int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            string typesearch;
            if (Type == "Contract")
                typesearch = "عقد";
            else
                typesearch = "تظهير";
            var contract = db.CompanyContractPhotoes.Where(c => c.CompId == compId && c.DeleteFlag == "N").Max(c => c.ContractNo);
            var model = db.CompanyContractPhotoes.Where(c => c.Contractype == typesearch && c.CompId == compId && c.ContractNo == contract && c.DeleteFlag == "N")
                .FirstOrDefault();
            List<photopathe> paths = new List<photopathe>();
            if (model != null)
                paths = AddList(model);
            var result = new
            {
                sEcho = sEcho,
                aaData = paths.AsEnumerable().Select(l => new
                {
                    path = l.path,
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = paths.AsEnumerable()
                 .Count(),
                iTotalDisplayRecords = paths.AsEnumerable()
                .Count()

            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }

        public JsonResult ProviderContractList(int ProviderCode, int ProviderType, string Type, int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            string typesearch;
            if (Type == "Contract")
                typesearch = "1";
            else
                typesearch = "2";
            //var model = db.ProviderContractPhotoes.Where(c => c.TypeContract == typesearch && c.ProviderCode == ProviderCode && c.TypeImage == 1 && c.Active == "Y")
            //    .FirstOrDefault();
            //List<photopathe> paths = new List<photopathe>();
            //if (model != null)
            //    paths = AddProviderList(model);
            var result = new
            {
                sEcho = sEcho,
                aaData = db.ProviderContractPhotoes.AsEnumerable()
                .Where(c => c.TypeContract == typesearch && c.ProviderCode == ProviderCode && c.TypeImage == 1 && c.Active == "Y")
                .Select(l => new
                {
                    path = l.Path,
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.ProviderContractPhotoes.AsEnumerable()
                .Where(c => c.TypeContract == typesearch && c.ProviderCode == ProviderCode && c.TypeImage == 1 && c.Active == "Y")
                 .Count(),
                iTotalDisplayRecords = db.ProviderContractPhotoes.AsEnumerable()
                .Where(c => c.TypeContract == typesearch && c.ProviderCode == ProviderCode && c.TypeImage == 1 && c.Active == "Y")
                .Count()

            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<photopathe> AddList(CompanyContractPhoto model)
        {
            List<photopathe> paths = new List<photopathe>();
            if (model.Path1 != null)
                paths.Add(new photopathe { path = model.Path1 });
            if (model.Path2 != null)
                paths.Add(new photopathe { path = model.Path2 });
            if (model.Path3 != null)
                paths.Add(new photopathe { path = model.Path3 });
            if (model.Path4 != null)
                paths.Add(new photopathe { path = model.Path4 });
            if (model.Path5 != null)
                paths.Add(new photopathe { path = model.Path5 });
            if (model.Path6 != null)
                paths.Add(new photopathe { path = model.Path6 });
            if (model.Path7 != null)
                paths.Add(new photopathe { path = model.Path7 });
            if (model.Path8 != null)
                paths.Add(new photopathe { path = model.Path8 });
            if (model.Path9 != null)
                paths.Add(new photopathe { path = model.Path9 });
            if (model.Path10 != null)
                paths.Add(new photopathe { path = model.Path10 });
            if (model.Path11 != null)
                paths.Add(new photopathe { path = model.Path11 });
            if (model.Path12 != null)
                paths.Add(new photopathe { path = model.Path12 });
            if (model.Path13 != null)
                paths.Add(new photopathe { path = model.Path13 });
            if (model.Path14 != null)
                paths.Add(new photopathe { path = model.Path14 });
            if (model.Path15 != null)
                paths.Add(new photopathe { path = model.Path15 });
            if (model.Path16 != null)
                paths.Add(new photopathe { path = model.Path16 });
            if (model.Path17 != null)
                paths.Add(new photopathe { path = model.Path17 });
            if (model.Path18 != null)
                paths.Add(new photopathe { path = model.Path18 });
            if (model.Path19 != null)
                paths.Add(new photopathe { path = model.Path19 });
            if (model.Path20 != null)
                paths.Add(new photopathe { path = model.Path20 });
            return paths;
        }

        //public List<photopathe> AddProviderList(ProviderContractPhoto model)
        //{
        //    List<photopathe> paths = new List<photopathe>();
        //    if (model.Path1 != null)
        //        paths.Add(new photopathe { path = model.Path1 });
        //    if (model.Path2 != null)
        //        paths.Add(new photopathe { path = model.Path2 });
        //    if (model.Path3 != null)
        //        paths.Add(new photopathe { path = model.Path3 });
        //    if (model.Path4 != null)
        //        paths.Add(new photopathe { path = model.Path4 });
        //    if (model.Path5 != null)
        //        paths.Add(new photopathe { path = model.Path5 });
        //    if (model.Path6 != null)
        //        paths.Add(new photopathe { path = model.Path6 });
        //    if (model.Path7 != null)
        //        paths.Add(new photopathe { path = model.Path7 });
        //    if (model.Path8 != null)
        //        paths.Add(new photopathe { path = model.Path8 });
        //    if (model.Path9 != null)
        //        paths.Add(new photopathe { path = model.Path9 });
        //    if (model.Path10 != null)
        //        paths.Add(new photopathe { path = model.Path10 });
        //    if (model.Path11 != null)
        //        paths.Add(new photopathe { path = model.Path11 });
        //    if (model.Path12 != null)
        //        paths.Add(new photopathe { path = model.Path12 });
        //    if (model.Path13 != null)
        //        paths.Add(new photopathe { path = model.Path13 });
        //    if (model.Path14 != null)
        //        paths.Add(new photopathe { path = model.Path14 });
        //    if (model.Path15 != null)
        //        paths.Add(new photopathe { path = model.Path15 });
        //    if (model.Path16 != null)
        //        paths.Add(new photopathe { path = model.Path16 });
        //    if (model.Path17 != null)
        //        paths.Add(new photopathe { path = model.Path17 });
        //    if (model.Path18 != null)
        //        paths.Add(new photopathe { path = model.Path18 });
        //    if (model.Path19 != null)
        //        paths.Add(new photopathe { path = model.Path19 });
        //    if (model.Path20 != null)
        //        paths.Add(new photopathe { path = model.Path20 });
        //    return paths;
        //}

        public class photopathe
        {
            public string path { get; set; }
        }
        #endregion
    }
}
