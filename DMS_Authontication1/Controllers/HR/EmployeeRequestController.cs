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
    public class EmployeeRequestController : Controller
    {
        #region Properties
        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationUserManager _userManager;
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
        // GET: EmployeeRequest
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult DashBoard()
        {
            // return View(db.Employee_Request.ToList());
            return View();
        }

        public JsonResult RequestList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            long lgSearch;
            long.TryParse(sSearch, out lgSearch);
            if (User.IsInRole("Admin"))
            {
                var result2 = new
                {
                    sEcho = sEcho,
                    aaData = db.Employee_Request.OrderByDescending(m => m.REQUEST_CODE).AsEnumerable()
                .Where(r => sSearch != "" ? r.CARD_ID.Contains(sSearch) || r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) || r.REQUEST_CODE == lgSearch : true).Where(x => x.CREATED_DATE.Value.AddDays(90).Date > DateTime.Now.Date)
                .Select(l => new Employee_Request
                {
                    REQUEST_CODE = l.REQUEST_CODE,
                    CARD_ID = l.CARD_ID,
                    CHANG_EMP_NAME = l.EMP_ANAME_ST + ' ' + l.EMP_ANAME_SC + ' ' + l.EMP_ANAME_TH,
                    APPROVE_FLAG = l.APPROVE_FLAG,
                    TYPE = l.TYPE,
                    CREATED_DATE = l.CREATED_DATE,
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Employee_Request.AsEnumerable()
                .Where(r => sSearch != "" ? r.CARD_ID.Contains(sSearch) || r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) || r.REQUEST_CODE == lgSearch : true).Count(),
                    iTotalDisplayRecords = db.Employee_Request.OrderBy(m => m.REQUEST_CODE).AsEnumerable()
                .Where(r => sSearch != "" ? r.CARD_ID.Contains(sSearch) || r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) || r.REQUEST_CODE == lgSearch : true).Where(x => x.CREATED_DATE.Value.AddDays(90).Date > DateTime.Now.Date).Count()
                };
                return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            var result = new
            {
                sEcho = sEcho,
                aaData = db.Employee_Request.Where(x => x.CREATED_BY == User.Identity.Name).OrderByDescending(m => m.REQUEST_CODE).AsEnumerable()
                .Where(r => sSearch != "" ? r.CARD_ID.Contains(sSearch) || r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) || r.REQUEST_CODE == lgSearch : true).Where(x => x.CREATED_DATE.Value.AddDays(90).Date > DateTime.Now.Date)
                .Select(l => new Employee_Request
                {
                    REQUEST_CODE = l.REQUEST_CODE,
                    CARD_ID = l.CARD_ID,
                    CHANG_EMP_NAME = l.EMP_ANAME_ST + ' ' + l.EMP_ANAME_SC + ' ' + l.EMP_ANAME_TH,
                    APPROVE_FLAG = l.APPROVE_FLAG,
                    TYPE = l.TYPE,
                    CREATED_DATE = l.CREATED_DATE,
                }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.Employee_Request.Where(x => x.CREATED_BY == User.Identity.Name /*&& !x.Manager.Contains("Lab") && !x.Manager.Contains("Ray") && !x.Manager.Contains("Stop")*/).AsEnumerable()
                .Where(r => sSearch != "" ? r.CARD_ID.Contains(sSearch) || r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) || r.REQUEST_CODE == lgSearch : true).Count(),
                iTotalDisplayRecords = db.Employee_Request.Where(x => x.CREATED_BY == User.Identity.Name).OrderBy(m => m.REQUEST_CODE).AsEnumerable()
                .Where(r => sSearch != "" ? r.CARD_ID.Contains(sSearch) || r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) || r.REQUEST_CODE == lgSearch : true).Where(x => x.CREATED_DATE.Value.AddDays(90).Date > DateTime.Now.Date).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }

        // GET: EmployeeRequest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee_Request employee_Request = db.Employee_Request.Find(id);
            if (employee_Request == null)
            {
                return HttpNotFound();
            }
            return View(employee_Request);
        }

        // GET: EmployeeRequest/Create
        public ActionResult Create(int? id)
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            int companyId = Convert.ToInt32(CurrentUser.Provider);

            var Branchs = db.Company_Cost_Center.Where(x => x.C_COMP_ID == companyId).ToList();
            SelectList BranchsList = new SelectList(Branchs, "ID", "A_NAME");
            ViewBag.Branchs = BranchsList;

            var Classcodes = db.CompContractClasses.Where(x => x.C_COMP_ID == companyId).Select(x => x.CLASS_CODE).Distinct().ToList();
            List<Insurance_Class> levels = new List<Insurance_Class>();
            foreach (var item in Classcodes)
            {
                Insurance_Class level = db.Insurance_Class.Where(x => x.CLASS_CODE == item).FirstOrDefault();
                levels.Add(level);
            }
            SelectList levelsList = new SelectList(levels, "CLASS_CODE", "ALIAS_CODE");
            ViewBag.levels = levelsList;
            if (id != null)
            {
                Employee_Request employee_Request = db.Employee_Request.Find(id);
                if (employee_Request == null)
                {
                    return View();
                }
                return View(employee_Request);
            }
            return View();
        }

        // POST: EmployeeRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EMP_CODE,EMP_ENAME_ST,EMP_ENAME_SC,EMP_ENAME_TH,EMP_ENAME_FR,EMP_ANAME_ST,EMP_ANAME_SC,EMP_ANAME_TH,EMP_ANAME_FR,NATIONAL_ID,BIRTHDATE,GENDER,MOBILE,EMAIL,START_DATE,END_DATE,EMP_RELATION,EMP_IMG,ADDRESS,BRANCH,RECEIVE_CARD,EMP_CLASS,EMP_CLASS_REASON,CREATED_BY,CREATED_DATE,UPDATED_BY,UPDATED_DATE,TYPE,REGISTER_TYPE,CARD_ID,TERMINATE_DATE,DELIVER_CARD_FLAG,DELIVER_CARD_DATE,APPROVE_FLAG,PRINT_REASON,PRINT_IMG,REOPEN_DATE,NEW_CARD_ID,REQUEST_CODE,GLASSES,DISEASE,TYP_EMP_UPDATE,COMP_ID,FLAG_REMOVE,CHANG_EMP_NAME,REPRINT_EMP_CARD,RESON,DATE_CHANGE_TYP,RELATION")] Employee_Request employee_Request)
        {
            if (ModelState.IsValid)
            {
                db.Employee_Request.Add(employee_Request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee_Request);
        }

        // GET: EmployeeRequest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee_Request employee_Request = db.Employee_Request.Find(id);
            if (employee_Request == null)
            {
                return HttpNotFound();
            }
            return View(employee_Request);
        }

        // POST: EmployeeRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EMP_CODE,EMP_ENAME_ST,EMP_ENAME_SC,EMP_ENAME_TH,EMP_ENAME_FR,EMP_ANAME_ST,EMP_ANAME_SC,EMP_ANAME_TH,EMP_ANAME_FR,NATIONAL_ID,BIRTHDATE,GENDER,MOBILE,EMAIL,START_DATE,END_DATE,EMP_RELATION,EMP_IMG,ADDRESS,BRANCH,RECEIVE_CARD,EMP_CLASS,EMP_CLASS_REASON,CREATED_BY,CREATED_DATE,UPDATED_BY,UPDATED_DATE,TYPE,REGISTER_TYPE,CARD_ID,TERMINATE_DATE,DELIVER_CARD_FLAG,DELIVER_CARD_DATE,APPROVE_FLAG,PRINT_REASON,PRINT_IMG,REOPEN_DATE,NEW_CARD_ID,REQUEST_CODE,GLASSES,DISEASE,TYP_EMP_UPDATE,COMP_ID,FLAG_REMOVE,CHANG_EMP_NAME,REPRINT_EMP_CARD,RESON,DATE_CHANGE_TYP,RELATION")] Employee_Request employee_Request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee_Request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee_Request);
        }

        // GET: EmployeeRequest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee_Request employee_Request = db.Employee_Request.Find(id);
            if (employee_Request == null)
            {
                return HttpNotFound();
            }
            return View(employee_Request);
        }

        // POST: EmployeeRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee_Request employee_Request = db.Employee_Request.Find(id);
            db.Employee_Request.Remove(employee_Request);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult EditRequest()
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (User.IsInRole("HR_Admin"))
            {
                ViewBag.CompName = null;
                var compines = db.HrAdminCompanies.Where(x => x.UserId == CurrentUser.Id).Select(c => c.CompId).ToList();
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

            int companyId = Convert.ToInt32(CurrentUser.Provider);
            ViewBag.CompName = companyId;
            var Branchs = db.Company_Cost_Center.Where(x => x.C_COMP_ID == companyId).ToList();
            SelectList BranchsList = new SelectList(Branchs, "ID", "A_NAME");
            ViewBag.Branchs = BranchsList;

            var Classcodes = db.CompContractClasses.Where(x => x.C_COMP_ID == companyId).Select(x => x.CLASS_CODE).Distinct().ToList();
            List<Insurance_Class> levels = new List<Insurance_Class>();
            foreach (var item in Classcodes)
            {
                Insurance_Class level = db.Insurance_Class.Where(x => x.CLASS_CODE == item).FirstOrDefault();
                levels.Add(level);
            }
            SelectList levelsList = new SelectList(levels, "CLASS_CODE", "ALIAS_CODE");
            ViewBag.levels = levelsList;
            return View();
        }
        public ActionResult Termination()
        {
            return View();
        }

        public ActionResult PrintXLC(int Type)
        {
            try
            {
                switch (Type)
                {
                    case 1:
                        var ExcelFile = Server.MapPath("~/Content/EmployeesRequestsImage/Add Employee.xlsx");

                        return File(ExcelFile, "application/xls", "Add Employees.xls");
                    case 2:
                        var ExcelFile2 = Server.MapPath("~/Content/EmployeesRequestsImage/Level.xlsx");

                        return File(ExcelFile2, "application/xls", "Level.xls");
                    case 3:
                        var ExcelFile3 = Server.MapPath("~/Content/EmployeesRequestsImage/Re Open.xlsx");

                        return File(ExcelFile3, "application/xls", "Re Open.xls");
                    case 4:
                        var ExcelFile4 = Server.MapPath("~/Content/EmployeesRequestsImage/Termination Request.xlsx");

                        return File(ExcelFile4, "application/xls", "Termination Request.xls");
                    default:
                        var ExcelFile5 = Server.MapPath("~/Content/EmployeesRequestsImage/Add Employess.xlsx");

                        return File(ExcelFile5, "application/xls", "Add Employees.xls");
                }
                //var ExcelFile2 = Server.MapPath("~/Content/EmployeesRequestsImage/Add Employess.xlsx");

                //return File(ExcelFile2, "application/xls", "Add Employees.xls");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");

                throw ex;
            }
        }

        #endregion

        #region Helper Methods
        public JsonResult GetClassList(string compId)
        {
            int companyId = Convert.ToInt32(compId);
            var model = (from CompConClass in db.CompContractClasses
                         where CompConClass.C_COMP_ID == companyId
                         join InsurClass in db.Insurance_Class
                         on CompConClass.CLASS_CODE equals InsurClass.CLASS_CODE
                         select new
                         {
                             CLASS_CODE = InsurClass.CLASS_CODE,
                             ALIAS_CODE = InsurClass.ALIAS_CODE
                         }).Distinct().ToList();

            SelectList ClassList = new SelectList(model, "CLASS_CODE", "ALIAS_CODE");

            return Json(ClassList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranchList(string compId)
        {
            int companyId = Convert.ToInt32(compId);
            var Branchs = db.Company_Cost_Center.Where(x => x.C_COMP_ID == companyId).ToList();
            SelectList BranchsList = new SelectList(Branchs, "ID", "A_NAME");
            return Json(BranchsList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CardValidation(string id)
        {
            bool CardValidation = db.Comp_Employees.Where(x => x.CARD_ID == id).Any();
            return Json(new { Validation = !CardValidation, Limit = 0, CeilingPert = 0 });
        }

        public JsonResult GetActiveEmployess(string search, int page)
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (User.IsInRole("HR_Admin"))
            {
                var companyId = db.HrAdminCompanies.Where(c => c.UserId == CurrentUser.Id).Select(c => c.CompId).ToList();
                if (companyId[0] == "All")
                {
                    int compId = int.Parse(search.Split('-')[0]);
                    int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                    var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "N", search)
                                     .Select(c => new
                                     {
                                         id = c.id,
                                         text = c.text
                                     }).ToList();
                    return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    if (companyId.Contains(search.Split('-')[0]))
                    {
                        int compId = int.Parse(search.Split('-')[0]);
                        int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                        var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "N", search)
                                         .Select(c => new
                                         {
                                             id = c.id,
                                             text = c.text
                                         }).ToList();
                        return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }

            else if (User.IsInRole("Admin"))
            {
                int compId = int.Parse(search.Split('-')[0]);
                int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "N", search)
                                 .Select(c => new
                                 {
                                     id = c.id,
                                     text = c.text
                                 }).ToList();
                return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                int Provider = Convert.ToInt32(CurrentUser.Provider);
                int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == Provider).Max(x => x.CONTRACT_NO);
                var Employees = db.fn_GetEmployessForCompany(Provider, maxcontract, "N", search)
                   .Select(c => new
                   {
                       id = c.id,
                       text = c.text
                   }).ToList();

                return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }

        }

        public JsonResult GetInActiveEmployess(string search, int page)
        {

            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (User.IsInRole("HR_Admin"))
            {
                var companyId = db.HrAdminCompanies.Where(c => c.UserId == CurrentUser.Id).Select(c => c.CompId).ToList();
                if (companyId[0] == "All")
                {
                    int compId = int.Parse(search.Split('-')[0]);
                    int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                    var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "Y", search)
                 .Select(c => new
                 {
                     id = c.id,
                     text = c.text
                 }).ToList();

                    return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
                else
                {
                    if (companyId.Contains(search.Split('-')[0]))
                    {
                        int compId = int.Parse(search.Split('-')[0]);
                        int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                        var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "Y", search)
                  .Select(c => new
                  {
                      id = c.id,
                      text = c.text
                  }).ToList();

                        return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                    }
                    return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }

            else if (User.IsInRole("Admin"))
            {
                int compId = int.Parse(search.Split('-')[0]);
                int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == compId).Max(x => x.CONTRACT_NO);
                var Employees = db.fn_GetEmployessForCompany(compId, maxcontract, "Y", search)
                 .Select(c => new
                 {
                     id = c.id,
                     text = c.text
                 }).ToList();

                return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            else
            {
                int Provider = Convert.ToInt32(CurrentUser.Provider);
                int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == Provider).Max(x => x.CONTRACT_NO);
                var Employees = db.fn_GetEmployessForCompany(Provider, maxcontract, "Y", search)
                 .Select(c => new
                 {
                     id = c.id,
                     text = c.text
                 }).ToList();

                return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            //long lgSearch;
            //long.TryParse(search, out lgSearch);
            //ApplicationDbContext users = new ApplicationDbContext();
            //var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //int Provider = Convert.ToInt32(CurrentUser.Provider);
            //int maxcontract = db.Contract_Data.Where(x => x.C_COMP_ID == Provider).Max(x => x.CONTRACT_NO);
            //var Employees = db.fn_GetEmployessForCompany(Provider, maxcontract, "Y", search)
            //     .Select(c => new
            //     {
            //         id = c.id,
            //         text = c.text
            //     }).ToList();

            //return new JsonResult { Data = Employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }

        public JsonResult SaveRequest(Employee_Request data)
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            string[] single = data.EMP_IMG.Split('.');
            data.EMP_IMG = single[0] + DateTime.Now.ToString("yyMMddHH") + '.' + single[1];

            if (data.REQUEST_CODE != 0)
            {
                Employee_Request model = db.Employee_Request.Find(data.REQUEST_CODE);

                model.UPDATED_BY = User.Identity.Name;
                model.UPDATED_DATE = DateTime.Now;
                model.APPROVE_FLAG = "Pending";
                model.TYPE = 1;
                model.EMP_IMG = data.EMP_IMG;
                model.EMP_ENAME_ST = data.EMP_ENAME_ST;
                model.EMP_ENAME_SC = data.EMP_ENAME_SC;
                model.EMP_ENAME_TH = data.EMP_ENAME_TH;
                model.EMP_ENAME_FR = data.EMP_ENAME_FR;
                model.EMP_ANAME_FR = data.EMP_ANAME_FR;
                model.EMP_ANAME_SC = data.EMP_ANAME_SC;
                model.EMP_ANAME_ST = data.EMP_ANAME_ST;
                model.EMP_ANAME_TH = data.EMP_ANAME_TH;
                model.CARD_ID = data.CARD_ID;
                model.NATIONAL_ID = data.NATIONAL_ID;
                model.BIRTHDATE = data.BIRTHDATE;
                model.GENDER = data.GENDER;
                model.MOBILE = data.MOBILE;
                model.EMAIL = data.EMAIL;
                model.START_DATE = data.START_DATE;
                model.ADDRESS = data.ADDRESS;
                model.BRANCH = data.BRANCH;
                model.GLASSES = data.GLASSES;
                model.DISEASE = data.DISEASE;
                model.EMP_RELATION = data.EMP_RELATION;
                model.EMP_CLASS = data.EMP_CLASS;
                db.Entry(model).State = EntityState.Modified;
                model.CARD_ID = data.CARD_ID;
                var x = db.SaveChanges();
                return Json(data.REQUEST_CODE);
            }

            data.CREATED_BY = User.Identity.Name;
            data.CREATED_DATE = DateTime.Now;
            data.TYPE = 1;//addition request
            data.APPROVE_FLAG = "Pending";//Pending
            data.COMP_ID = Convert.ToInt32(CurrentUser.Provider);
            data.CARD_ID = "0";
            data.REGISTER_TYPE = "P";
            db.Employee_Request.Add(data);

            int result = db.SaveChanges();

            var userid = User.Identity.GetUserId();
            var Hospitalprovider = UserManager.FindById(userid);

            string sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
            string msg = @"<h3> Employee Name  : </h3>" + data.EMP_ANAME_ST + " " + data.EMP_ANAME_SC + " " + data.EMP_ANAME_TH + " <br/>" +
                "<h3> Request Code: </h3> " + data.REQUEST_CODE + " <br/> " +
                "<h3> Request Type : </ h3 > New Employee <br/> " +
                "<h4> Employee Class: </h4>" + data.EMP_CLASS + "<br/>" +
                          "<h4> National Id :  </h4>" + data.NATIONAL_ID + "<br/>" +
                          "<h4> BirthDate  : </h4>" + data.BIRTHDATE + "<br/>" +
                          "<h4> Gender  : </h4>" + data.GENDER + "<br/>" +
                          "<h4> Start Date  : </h4>" + data.START_DATE + "<br/>" +
                          "<h4> Branch  : </h4>" + data.BRANCH + "<br/>" +
                          "<h4> Mobile : </h4>" + data.MOBILE + "<br/>";

            AlternateView altView = AlternateView.CreateAlternateViewFromString(msg, null, MediaTypeNames.Text.Html);

            string extension = Path.GetExtension(data.EMP_IMG);
            var extenti = MediaTypeNames.Application.Pdf;
            //string path= Server.MapPath()
            if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
            {
                extenti = MediaTypeNames.Image.Jpeg;
            }
            LinkedResource Img = new LinkedResource(Server.MapPath("/Content/EmployeesRequestsImage/" + data.EMP_IMG), extenti);
            Img.ContentId = "MyImage" + 0;
            altView.LinkedResources.Add(Img);
            var image = DownloadAttachment(data.EMP_IMG);
            msg = msg + image + "<br/>";
            SendMail("Operation@dms-eg.com", sub, msg, altView/*, Hospitalprovider*/);
            SendMail("Operation.aso@dms-eg.com", sub, msg, altView/*, Hospitalprovider*/);
            SendMail("marian@dms-eg.com", sub, msg, altView/*, Hospitalprovider*/);
            //SendMail("dms.medical1@gmail.com", sub, msg, altView/*, Hospitalprovider*/);
            //SendMail("dms.medical2@gmail.com", sub, msg, altView/*, Hospitalprovider*/);

            return Json(data.REQUEST_CODE);
        }

        public JsonResult SaveImage(HttpPostedFileBase File)
        {
            if (File != null)
            {
                var fileName = Path.GetFileName(File.FileName);
                var extention = Path.GetExtension(File.FileName);
                var filenamewithoutextension = Path.GetFileNameWithoutExtension(File.FileName);
                fileName = filenamewithoutextension + DateTime.Now.ToString("yyMMddHH") + extention;

                File.SaveAs(Server.MapPath("/Content/EmployeesRequestsImage/" + fileName));

                return new JsonResult { Data = fileName, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = "No", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SendExcelFile(HttpPostedFileBase File)
        {
            if (File != null)
            {
                var userid = User.Identity.GetUserId();
                var Hospitalprovider = UserManager.FindById(userid);

                string sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
                SendExcelMail("mediacl.approv@gmail.com", "Operation@dms-eg.com", sub, File);
                SendExcelMail("mediacl.approv@gmail.com", "Operation.aso@dms-eg.com", sub, File);
                SendExcelMail("mediacl.approv@gmail.com", "marian@dms-eg.com", sub, File);
                //using (MailMessage mail = new MailMessage("mediacl.approv@gmail.com", "Operation@dms-eg.com"))
                //{
                //    mail.Subject = sub;
                //    mail.Body = "";
                //    string fileName = Path.GetFileName(File.FileName);
                //    mail.Attachments.Add(new Attachment(File.InputStream, fileName));

                //    mail.IsBodyHtml = false;
                //    SmtpClient smtp = new SmtpClient();
                //    smtp.Host = "smtp.gmail.com";
                //    smtp.EnableSsl = true;
                //    NetworkCredential networkCredential = new System.Net.NetworkCredential("mediacl.approv@gmail.com", "Dms123456");
                //    smtp.UseDefaultCredentials = true;
                //    smtp.Credentials = networkCredential;
                //    smtp.Port = 587;
                //    smtp.Send(mail);
                //}
                return new JsonResult { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }

            return new JsonResult { Data = "r", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveEditRequest(Employee_Request data)
        {
            string imgPath = null;
            string sub = null;
            string msg = null;
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            data.CREATED_BY = User.Identity.Name;
            data.CREATED_DATE = DateTime.Now;
            data.APPROVE_FLAG = "Pending";//Pending
            data.REGISTER_TYPE = "P";
            data.COMP_ID = Convert.ToInt32(CurrentUser.Provider);
            if (data.PRINT_IMG != null)
            {
                imgPath = data.PRINT_IMG;
            }
            if (data.TYP_EMP_UPDATE != null)
            {
                imgPath = data.TYP_EMP_UPDATE;
            }

            db.Employee_Request.Add(data);

            int result = db.SaveChanges();

            var userid = User.Identity.GetUserId();
            var Hospitalprovider = UserManager.FindById(userid);
            if (data.TYPE == 2)
            {
                sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
                msg = @"<h3> Request Code  : </h3>" + data.REQUEST_CODE + " <br/>" +
                   "<h3> Type : </h3> Edit Level <br/> " +
                   "<h3> Card Id: </h3> " + data.CARD_ID + " <br/> " +
                   "<h4> Employee Class :  </h4>" + data.EMP_CLASS + "<br/>" +
                   "<h4> Reson :  </h4>" + data.RESON + "<br/>" +
                   "<h4> Start Date  : </h4>" + data.START_DATE + "<br/>";
            }

            if (data.TYPE == 4)
            {
                sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
                msg = @"<h3> Request Code  : </h3>" + data.REQUEST_CODE + " <br/>" +
                   "<h3>Type : </h3> Re-print <br/> " +
                   "<h3> Card Id: </h3> " + data.CARD_ID + " <br/> " +
                   "<h4> Print Reason: </h4>" + data.PRINT_REASON + "<br/>";
            }

            if (data.TYPE == 5)
            {
                sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
                msg = @"<h3> Request Code  : </h3>" + data.REQUEST_CODE + " <br/>" +
                   "<h3>Type : </h3> Re-Active <br/> " +
                   "<h3> Card Id: </h3> " + data.CARD_ID + " <br/> " +
                   "<h4> Re-Open Date  : </h4>" + data.REOPEN_DATE + "<br/>";
            }

            if (data.TYPE == 6)
            {
                sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
                msg = @"<h3> Request Code  : </h3>" + data.REQUEST_CODE + " <br/>" +
                   "<h3>Type : </h3> Change employee number <br/> " +
                   "<h3> Card Id: </h3> " + data.CARD_ID + " <br/> " +
                   "<h4> New Card Id  : </h4>" + data.NEW_CARD_ID + "<br/>";
            }

            if (data.TYPE == 7)
            {
                sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
                msg = @"<h3> Request Code  : </h3>" + data.REQUEST_CODE + " <br/>" +
                   "<h3>Type : </h3> Change employee name <br/> " +
                   "<h3> Card Id: </h3> " + data.CARD_ID + " <br/> " +
                   "<h3> New Employee Name  : </h3>" + data.EMP_ANAME_ST + " " + data.EMP_ANAME_SC + " " + data.EMP_ANAME_TH + " <br/>";
            }


            AlternateView altView = AlternateView.CreateAlternateViewFromString(msg, null, MediaTypeNames.Text.Html);

            if (imgPath != null)
            {

                string extension = Path.GetExtension(imgPath);
                var extenti = MediaTypeNames.Application.Pdf;
                //string path= Server.MapPath()
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    extenti = MediaTypeNames.Image.Jpeg;
                }
                LinkedResource Img = new LinkedResource(Server.MapPath("/Content/EmployeesRequestsImage/" + imgPath), extenti);
                Img.ContentId = "MyImage" + 0;
                altView.LinkedResources.Add(Img);
                var image = DownloadAttachment(imgPath);
                msg = msg + image + "<br/>";

            }

            SendMail("Operation@dms-eg.com", sub, msg, altView);
            SendMail("Operation.aso@dms-eg.com", sub, msg, altView);
            SendMail("marian@dms-eg.com", sub, msg, altView);

            return Json(data.REQUEST_CODE);
        }

        public JsonResult SaveTerminationRequest(Employee_Request data)
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            data.CREATED_BY = User.Identity.Name;
            data.CREATED_DATE = DateTime.Now;
            data.TYPE = 3;//Termination request
            data.APPROVE_FLAG = "Pending";//Pending
            data.COMP_ID = Convert.ToInt32(CurrentUser.Provider);

            db.Employee_Request.Add(data);

            int result = db.SaveChanges();

            var userid = User.Identity.GetUserId();
            var Hospitalprovider = UserManager.FindById(userid);

            string sub = @"Request Employee From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
            string msg = @"<h3>  Request Code: </h3> " + data.REQUEST_CODE + " <br/> " +
                "<h3>  Request Type: </h3> Termination Request <br/> " +
                "<h4> Card Id: </h4>" + data.CARD_ID + "<br/>" +
                "<h4> Terminate Date  : </h4>" + data.TERMINATE_DATE + "<br/>" +
                "<h4> Is Received Card ??!! : </h4>" + data.DELIVER_CARD_FLAG + "<br/>" +
                "<h4> Received Date : </h4>" + data.DELIVER_CARD_DATE + "<br/>";

            AlternateView altView = AlternateView.CreateAlternateViewFromString(msg, null, MediaTypeNames.Text.Html);

            SendMail("Operation@dms-eg.com", sub, msg, altView/*, Hospitalprovider*/);
            SendMail("Operation.aso@dms-eg.com", sub, msg, altView/*, Hospitalprovider*/);
            SendMail("marian@dms-eg.com", sub, msg, altView/*, Hospitalprovider*/);
            //SendMail("dms.medical1@gmail.com", sub, msg, altView/*, Hospitalprovider*/);
            //SendMail("dms.medical2@gmail.com", sub, msg, altView/*, Hospitalprovider*/);

            return Json(data.REQUEST_CODE);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void SendMail(string to, string subject, string Message, AlternateView altView)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("mediacl.approv@gmail.com", to, subject, Message);
            mail.AlternateViews.Add(altView);
            System.Net.NetworkCredential mailAuthenticaion = new System.Net.NetworkCredential("mediacl.approv@gmail.com", "mqaumlhlrnxqbjre");

            System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            mailclient.EnableSsl = true;
            mailclient.UseDefaultCredentials = false;
            mailclient.Credentials = mailAuthenticaion;
            mail.IsBodyHtml = true;
            mailclient.Send(mail);
        }

        public void SendExcelMail(string from, string to, string sub, HttpPostedFileBase File)
        {
            using (MailMessage mail = new MailMessage(from, to))
            {
                mail.Subject = sub;
                mail.Body = "";
                string fileName = Path.GetFileName(File.FileName);
                mail.Attachments.Add(new Attachment(File.InputStream, fileName));

                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new System.Net.NetworkCredential("mediacl.approv@gmail.com", "mqaumlhlrnxqbjre");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(mail);
            }
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

        public JsonResult UpdateStatus(int id, string statustext)
        {
            var model = db.Employee_Request.Where(r => r.REQUEST_CODE == id).FirstOrDefault();
            if (model != null)
            {
                model.APPROVE_FLAG = statustext;
                model.UPDATED_BY = User.Identity.Name;
                model.UPDATED_DATE = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return new JsonResult { Data = "Data Saved Susseccfuly", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                return new JsonResult { Data = "Rrequest not found", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        #endregion
    }
}
