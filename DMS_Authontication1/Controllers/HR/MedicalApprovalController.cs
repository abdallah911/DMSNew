using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.HR
{
    public class MedicalApprovalController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();
        ApplicationDbContext myEntities = new ApplicationDbContext();
        DBApproval dbAproval = new DBApproval();
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

        public ActionResult Index()
        {
            var HrUserNamre = User.Identity.GetUserName();
            var compcode = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            ViewBag.CompName = compcode;
            int companyCode = Convert.ToInt32(compcode);

            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;

            var maxcontract = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == companyCode).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault().CONTRACT_NO;

            var compclasses = (from comcont in db.CompContractClasses
                               where comcont.CONTRACT_NO == maxcontract && comcont.C_COMP_ID == companyCode
                               join insclass in db.Insurance_Class
                               on comcont.CLASS_CODE equals insclass.CLASS_CODE
                               select new
                               {
                                   ClassCode = comcont.CLASS_CODE,
                                   ClassString = comcont.CLASS_CODE + " | " + insclass.CLASS_ANAME
                               }).ToList();

            //var compclasses = db.CompContractClasses.AsNoTracking().Where(cl=>cl.CONTRACT_NO==maxcontract&& cl.C_COMP_ID== companyCode).ToList();
            SelectList compclasseslist = new SelectList(compclasses, "ClassCode", "ClassString");
            ViewBag.compclasseslist = compclasseslist;


            var address = db.Basic_Data.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_ENAME", "BS_ANAME");
            ViewBag.address = addresslist;
            return View();
        }

        [Authorize(Roles = "HR,HR_Admin")]
        [HttpGet]
        public ActionResult Search2(string CompId = null)
        {
            var HrUserNamre = User.Identity.GetUserName();
            var compcode = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            var cComp = int.Parse(compcode);
            var CurrentDate = DateTime.Now.Date;
            ViewBag.CompId = CompId;
            List<ApprovalComp> approval = new List<ApprovalComp>();
            try
            {
                if (CompId != null)
                {
                    var userid2 = User.Identity.GetUserId();
                    var compines = db.HrAdminCompanies.Where(x => x.UserId == userid2).Select(c => c.CompId).ToList();
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

                    var cComp2 = int.Parse(CompId);
                    var contract = db.Contract_Data.Where(c => c.C_COMP_ID == cComp2 && c.DATE_FROM <= CurrentDate && c.DATE_TO >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault().CONTRACT_NO;

                    DataTable dt = dbAproval.RunReader("SELECT CODE,APROVAL_TYP,MEDICAL_REPLAY,VALUE_AFTER,RECIV_DATE,CREATED_DATE,END_DATE,EXPAIRE_DATE,CREATED_BY,CARD_NO FROM MEDICAL_APPROVALS WHERE COMPANY_ID = "
                                                + cComp2 +
                                               " AND COMP_CONTRACT_NO='" + contract + "' AND ACTIVE ='Y' AND  EXPAIRE_DATE >= sysdate-14 ORDER BY CREATED_DATE DESC");

                    if (dt.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            approval.Add(new ApprovalComp
                            {
                                Code = row["CODE"].ToString(),
                                Approval_Type = row["APROVAL_TYP"].ToString(),
                                Medical_Replay = row["MEDICAL_REPLAY"].ToString(),
                                Value_After = float.Parse(row["VALUE_AFTER"].ToString()),
                                Recieve_Date = row["RECIV_DATE"].ToString(),
                                Created_Date = row["CREATED_DATE"].ToString(),
                                End_Date = row["END_DATE"].ToString(),
                                Expaire_Date = row["EXPAIRE_DATE"].ToString(),
                                CreatedBy = row["CREATED_BY"].ToString(),
                                CardId = row["CARD_NO"].ToString(),

                            });
                        }
                        return View(approval);
                    }
                    else
                        return View(approval);
                }
                if (User.IsInRole("HR_Admin"))
                {
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
                        DataTable dt2 = dbAproval.RunReader("SELECT CODE,APROVAL_TYP,MEDICAL_REPLAY,VALUE_AFTER,RECIV_DATE,CREATED_DATE,END_DATE,EXPAIRE_DATE,CREATED_BY,CARD_NO FROM MEDICAL_APPROVALS WHERE"

                                                   + " ACTIVE ='Y' AND  EXPAIRE_DATE >= sysdate-14 ORDER BY CREATED_DATE DESC");

                        if (dt2.Rows.Count != 0)
                        {
                            foreach (DataRow row in dt2.Rows)
                            {
                                approval.Add(new ApprovalComp
                                {
                                    Code = row["CODE"].ToString(),
                                    Approval_Type = row["APROVAL_TYP"].ToString(),
                                    Medical_Replay = row["MEDICAL_REPLAY"].ToString(),
                                    Value_After = float.Parse(row["VALUE_AFTER"].ToString()),
                                    Recieve_Date = row["RECIV_DATE"].ToString(),
                                    Created_Date = row["CREATED_DATE"].ToString(),
                                    End_Date = row["END_DATE"].ToString(),
                                    Expaire_Date = row["EXPAIRE_DATE"].ToString(),
                                    CreatedBy = row["CREATED_BY"].ToString(),
                                    CardId = row["CARD_NO"].ToString(),

                                });
                            }
                        }
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

                        foreach (var item in companyname)
                        {
                            var contract3 = db.Contract_Data.Where(c => c.C_COMP_ID == item.Code && c.DATE_FROM <= CurrentDate && c.DATE_TO >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault().CONTRACT_NO;

                            DataTable dt3 = dbAproval.RunReader("SELECT CODE,APROVAL_TYP,MEDICAL_REPLAY,VALUE_AFTER,RECIV_DATE,CREATED_DATE,END_DATE,EXPAIRE_DATE,CREATED_BY,CARD_NO FROM MEDICAL_APPROVALS WHERE COMPANY_ID = "
                                                        + item.Code +
                                                       " AND COMP_CONTRACT_NO='" + contract3 + "' AND ACTIVE ='Y' AND  EXPAIRE_DATE >= sysdate-14 ORDER BY CREATED_DATE DESC");

                            if (dt3.Rows.Count != 0)
                            {
                                foreach (DataRow row in dt3.Rows)
                                {
                                    approval.Add(new ApprovalComp
                                    {
                                        Code = row["CODE"].ToString(),
                                        Approval_Type = row["APROVAL_TYP"].ToString(),
                                        Medical_Replay = row["MEDICAL_REPLAY"].ToString(),
                                        Value_After = float.Parse(row["VALUE_AFTER"].ToString()),
                                        Recieve_Date = row["RECIV_DATE"].ToString(),
                                        Created_Date = row["CREATED_DATE"].ToString(),
                                        End_Date = row["END_DATE"].ToString(),
                                        Expaire_Date = row["EXPAIRE_DATE"].ToString(),
                                        CreatedBy = row["CREATED_BY"].ToString(),
                                        CardId = row["CARD_NO"].ToString(),

                                    });
                                }
                            }
                        }

                    }
                    return View(approval);
                }
                else
                {
                    var contract = db.Contract_Data.Where(c => c.C_COMP_ID == cComp && c.DATE_FROM <= CurrentDate && c.DATE_TO >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault().CONTRACT_NO;

                    DataTable dt = dbAproval.RunReader("SELECT CODE,APROVAL_TYP,MEDICAL_REPLAY,VALUE_AFTER,RECIV_DATE,CREATED_DATE,END_DATE,EXPAIRE_DATE,CREATED_BY,CARD_NO FROM MEDICAL_APPROVALS WHERE COMPANY_ID = "
                                                + compcode +
                                               " AND COMP_CONTRACT_NO='" + contract + "' AND ACTIVE ='Y' AND  EXPAIRE_DATE >= sysdate-14 ORDER BY CREATED_DATE DESC");

                    if (dt.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            approval.Add(new ApprovalComp
                            {
                                Code = row["CODE"].ToString(),
                                Approval_Type = row["APROVAL_TYP"].ToString(),
                                Medical_Replay = row["MEDICAL_REPLAY"].ToString(),
                                Value_After = float.Parse(row["VALUE_AFTER"].ToString()),
                                Recieve_Date = row["RECIV_DATE"].ToString(),
                                Created_Date = row["CREATED_DATE"].ToString(),
                                End_Date = row["END_DATE"].ToString(),
                                Expaire_Date = row["EXPAIRE_DATE"].ToString(),
                                CreatedBy = row["CREATED_BY"].ToString(),
                                CardId = row["CARD_NO"].ToString(),

                            });
                        }
                        return View(approval);
                    }
                    else
                        return View(approval);
                }
            }
            catch (Exception)
            {
                List<ApprovalComp> approval2 = new List<ApprovalComp>();
                return View(approval2);
            }

        }
        [Authorize(Roles = "HR,HR_Admin")]
        [HttpGet]
        public ActionResult Search()
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
                int companyCode = Convert.ToInt32(compcode);
                ViewBag.company = null;
            }

            return View();
        }

        [Authorize(Roles = "HR,User,HR_Admin")]
        [HttpGet]
        public ActionResult CreateRequest(int? id)
        {
            Enum_RequestsViewModel ENUM_REQUESTSViewModel = new Enum_RequestsViewModel();
            if (User.IsInRole("HR_Admin"))
            {
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

                    var companyname = (from compn in compines
                                       join contCo in db.Contract_Comp
                                       on int.Parse(compn) equals contCo.C_COMP_ID
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
                var comp_id = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ENUM_REQUESTSViewModel.CompName = comp_id;

            }
            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;
            var datenow = DateTime.Now.Date;
            if (User.IsInRole("User"))
            {
                var usr = User.Identity.GetUserId();
                var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                ENUM_REQUESTSViewModel.CARD_ID = cardId;
                ENUM_REQUESTSViewModel.CompName = cardId.Split('-')[0];
                var comp2 = int.Parse(ENUM_REQUESTSViewModel.CompName);
                var subCards = CardList(cardId);

                var cards = subCards.Select(c => new
                {
                    CardIDValue = c.CARD_ID,
                    CardIdString = c.CARD_ID
                }).ToList();
                SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
                ViewBag.Cardslist = Cardlist;
                //var employees2 = db.Comp_Employees.Where(m => m.C_COMP_ID == comp2 && m.CARD_ID == cardId &&
                //m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
                //  .Select(l => new
                //  {
                //      CARD_ID = l.CARD_ID,
                //      EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
                //  }).ToList();
                //SelectList addresslist2 = new SelectList(employees2, "CARD_ID", "EMP_ANAME");
                //ViewBag.address = addresslist2;
            }

            if (id != null)
            {
                var model = db.Enum_Requests.Find(id);
                if (model == null)
                {
                    return View(ENUM_REQUESTSViewModel);
                }
                Enum_RequestsViewModel ViewModel = new Enum_RequestsViewModel();
                ViewModel.ID = model.ID;
                var comp_id = model.CARD_ID.Split('-')[0];
                ViewModel.CompName = comp_id;
                ViewModel.CARD_ID = model.CARD_ID;
                ViewModel.TYPE = model.TYPE;
                ViewModel.TYP_ANAME = model.TYP_ANAME;
                ViewModel.PR_ENAME = model.PR_ENAME;
                ViewModel.NOTES = model.NOTES;
                ViewModel.MAIL_SEND = model.MAIL_SEND;
                ViewModel.APPROVAL_IMAGE = model.APPROVAL_IMAGE;
                return View(ViewModel);
            }
            return View(ENUM_REQUESTSViewModel);
        }

        [Authorize(Roles = "HR,User,HR_Admin")]
        [HttpPost]

        public ActionResult CreateRequest(Enum_RequestsViewModel addApproval)
        {
            var userid = User.Identity.GetUserId();
            var CompProvider = UserManager.FindById(userid);
            if (!ModelState.IsValid)
            {
                #region Error
                var usr = User.Identity.GetUserId();
                var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                Enum_RequestsViewModel ENUM_REQUESTSViewModel = new Enum_RequestsViewModel();
                //var HrUserNamre = User.Identity.GetUserName();
                //var comp_id = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ENUM_REQUESTSViewModel.CompName = addApproval.CARD_ID.Split('-')[0];
                ENUM_REQUESTSViewModel.TYPE = addApproval.TYPE; ;
                ENUM_REQUESTSViewModel.PR_ENAME = addApproval.PR_ENAME;
                ENUM_REQUESTSViewModel.TYP_ANAME = addApproval.TYP_ANAME;
                ENUM_REQUESTSViewModel.NOTES = addApproval.NOTES;
                ENUM_REQUESTSViewModel.CARD_ID = addApproval.CARD_ID;
                ENUM_REQUESTSViewModel.MAIL_SEND = addApproval.MAIL_SEND;

                var provider = db.ProviderTypeNews.ToList();
                SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
                ViewBag.provider = Providerlist;
                var subCards = CardList(cardId);

                var cards = subCards.Select(c => new
                {
                    CardIDValue = c.CARD_ID,
                    CardIdString = c.CARD_ID
                }).ToList();
                SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
                ViewBag.Cardslist = Cardlist;
                //var comp = Convert.ToInt32(addApproval.CARD_ID.Split('-')[0]);
                //var datenow = DateTime.Now.Date;
                //var employees = db.Comp_Employees.Where(m => m.C_COMP_ID == comp && m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
                //      .Select(l => new
                //      {
                //          CARD_ID = l.CARD_ID,
                //          EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
                //      }).ToList();
                ////var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
                //SelectList addresslist = new SelectList(employees, "CARD_ID", "EMP_ANAME");
                //ViewBag.address = addresslist;
                #endregion
                return View(ENUM_REQUESTSViewModel);

            }
            var model = new Enum_Requests();
            List<string> paths = new List<string>();
            List<string> exten = new List<string>();

            model.NOTES = addApproval.NOTES;
            model.REQ_DATE = DateTime.Now;
            model.EMP_ENAME = db.Comp_Employees.Where(e => e.CARD_ID == addApproval.CARD_ID).FirstOrDefault().EMP_ENAME;
            model.REQ_TYPE = "M";

            model.REQUEST_TYP = "Web";
            model.CREATED_BY = User.Identity.GetUserName();
            model.CREATED_DATE = DateTime.Now;
            model.STATE = 2;
            model.TYPE = addApproval.TYPE;
            model.PR_ENAME = addApproval.PR_ENAME;
            model.TYP_ANAME = addApproval.TYP_ANAME;
            model.NOTES = addApproval.NOTES;
            model.CARD_ID = addApproval.CARD_ID;
            model.MAIL_SEND = addApproval.MAIL_SEND;
            for (int i = 0; i < addApproval.ImageFile.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(addApproval.ImageFile[i].FileName);
                string extension = Path.GetExtension(addApproval.ImageFile[i].FileName);
                exten.Add(extension);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                model.APPROVAL_IMAGE = "~/Content/EmployeesRequestsImage/" + fileName;
                paths.Add("~/Content/EmployeesRequestsImage/" + fileName);

                addApproval.ImageFile[i].SaveAs(Server.MapPath("/Content/EmployeesRequestsImage/" + fileName /*ImageFile.FileName*/));
            }
            db.Enum_Requests.Add(model);
            db.SaveChanges();
            var compID = Convert.ToInt32(addApproval.CARD_ID.Split('-')[0]);
            var companyName = db.Contract_Comp.Where(c => c.C_COMP_ID == compID).FirstOrDefault().C_ANAME;

            string sub = "Request Approval From " + companyName;

            string msg = @"<h3> Send By: </h3>" + CompProvider.FName +
                          "<h3> Replay To Email :  </h3>" + addApproval.MAIL_SEND + "<br/>" +

                          "<h4> Card Id: </h4>" + addApproval.CARD_ID + "<br/>" +
                          "<h4> Approval Type  :  </h4>" + addApproval.TYPE + "<br/>" +
                          "<h4> Provider Type  : </h4>" + addApproval.TYP_ANAME + "<br/>" +
                          "<h4> Provider Id  : </h4>" + addApproval.PR_ENAME + "<br/>" +
                          "<h4> Approval Notes : </h4>" + addApproval.NOTES + "<br/>";



            AlternateView altView = AlternateView.CreateAlternateViewFromString(msg, null, MediaTypeNames.Text.Html);

            for (int i = 0; i < paths.Count; i++)
            {
                var extenti = MediaTypeNames.Application.Pdf;
                //string path= Server.MapPath()
                if (exten[i].ToLower() == ".jpg" || exten[i].ToLower() == ".jpeg" || exten[i].ToLower() == ".png")
                {
                    extenti = MediaTypeNames.Image.Jpeg;
                }
                LinkedResource Img = new LinkedResource(Server.MapPath(paths[i]), extenti);
                Img.ContentId = "MyImage" + i;
                altView.LinkedResources.Add(Img);
                msg = msg + addApproval.ImageFile[i] + "<br/>";
            }

            if (model.TYPE == "Medications outpatient")
            {
                SendMail("dms.pharmacy@gmail.com", sub, msg, altView, CompProvider);

            }
            else if (model.TYPE == "Dental services outpatient")
            {
                SendMail("dms.dental1@gmail.com", sub, msg, altView, CompProvider);
            }
            else
            {
                SendMail("dms.medical1@gmail.com", sub, msg, altView, CompProvider);
                SendMail("dms.medical2@gmail.com", sub, msg, altView, CompProvider);
            }
            if (User.IsInRole("User"))
            {
                return Redirect("/Employee/Approvales");
            }
            return RedirectToAction("Search");
        }

        public ActionResult Download(long id)
        {
            string files = ""; /*db.Enum_Requests.Where(x => x.ID == id).FirstOrDefault().IMAGE;*/
            string[] filesArray = files.Split(',');
            List<string> filesList = new List<string>();
            for (int i = 0; i <= filesArray.Length - 1; i++)
            {
                filesArray[i] = /*"~/Content/IndemnitiesAttachments/" +*/ filesArray[i];
                filesList.Add(Server.MapPath(filesArray[i]));
            }
            // Server.MapPath("/Content/IndemnitiesAttachments/"
            //string file = @"c:\someFolder\foo.xlsx";
            //string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //return File(file, contentType, Path.GetFileName(file));
            //fileName should be like "photo.jpg"
            string fullPath = Path.Combine(Server.MapPath("~/Content/EmployeesRequestsImage"), files);
            return File(fullPath, "application/jpg", files);

#pragma warning disable CS0162 // Unreachable code detected
            var archive = Server.MapPath("~/Content/IndemnitiesAttachments/archive.zip");
#pragma warning restore CS0162 // Unreachable code detected
            var temp = Server.MapPath("~/Content/EmployeesRequestsImage/");

            // clear any existing archive
            if (System.IO.File.Exists(archive))
            {
                System.IO.File.Delete(archive);
            }
            // empty the temp folder
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

            // copy the selected files to the temp folder
            filesList.ForEach(f => System.IO.File.Copy(f, Path.Combine(temp, Path.GetFileName(f))));

            // create a new archive
            ZipFile.CreateFromDirectory(temp, archive);

            return File(archive, "application/zip", "archive.zip");
        }
        #region Helper Method

        public JsonResult GetAprovel(string CardID, string compId, string code, string datefrom, string dateto)
        {

            DateTime date_from = Convert.ToDateTime(datefrom == string.Empty ? "10-11-2000" : datefrom);
            DateTime date_to = Convert.ToDateTime(dateto == string.Empty ? DateTime.Now.ToShortDateString() : dateto).AddDays(1);
            int Comp_ID = Convert.ToInt32(compId);

            // classLevel).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault().COVER_RELATION;
            var AproveList = db.fn_MedicalApprovalSearsh(CardID, datefrom, dateto, code).Where(b => b.CARD_ID.Contains(compId) && b.REQ_DATE >= date_from.Date && b.REQ_DATE <= date_to.Date).OrderBy(b => b.REQ_DATE).ToList();
            if (AproveList.Count > 0)
            {
                return new JsonResult { Data = new { AproveList = AproveList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { AproveList = 0, msg = "لا يوجد بيانات" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }
        public JsonResult GetAproveder(int id)
        {
            var listproveders = db.SERV_PROVIDERS_NEW.Where(m => m.PRV_TYPE == id).Select(l => new { PR_CODE = l.PR_CODE, PR_ANAME = l.PR_ANAME }).ToList().Distinct();
            SelectList Providerlist = new SelectList(listproveders, "PR_CODE", "PR_ANAME");
            return new JsonResult { Data = new { providerslist = Providerlist }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }


        public JsonResult GetProviders(string cardId, string compId, string country, string region, int providerId)
        {
            int Comp_ID = Convert.ToInt32(compId);
            var cardExist = db.Comp_Employees.AsNoTracking().Where(c => c.CARD_ID == cardId).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault();
            if (cardExist == null)
            {
                return new JsonResult { Data = new { providerslist = 0, msg = "Card Not Found ..." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                var coverdRelation = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == Comp_ID && c.CLASS_CODE ==
                                         cardExist.CLASS_CODE && c.CONTRACT_NO == cardExist.CONTRACT_NO).FirstOrDefault().COVER_RELATION;

                var status = ChicActiveCard(compId, cardId);
                if (status.data == "Y" && status.message == "ok")
                {
                    var bsCode = db.Basic_Data.Where(b => b.BS_ENAME == country && b.SOURCE_MOD == "M").FirstOrDefault().BS_CODE;
                    var arbicReagonName = db.Basic_Data.Where(r => r.SOURCE_MOD == "M" && r.BS_ENAME == region).FirstOrDefault().BS_CODE;

                    if (coverdRelation == 1 || coverdRelation == 4)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == arbicReagonName /*&& p.ADDRESS1.Contains(arbicReagonName)*/).ToList();

                        return new JsonResult { Data = new { providerslist = providerList, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }

                    else if (coverdRelation == 2)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.PROV_DEGREE == coverdRelation.ToString() && p.AREA_CODE == arbicReagonName && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3" /*&& p.ADDRESS1.Contains(arbicReagonName)*/).ToList();

                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }

                    else
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.PROV_DEGREE == coverdRelation.ToString() && p.AREA_CODE == arbicReagonName && p.PROV_DEGREE == "3" /*&& p.ADDRESS1.Contains(arbicReagonName)*/).ToList();

                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                else
                {
                    return new JsonResult { Data = new { providerslist = 0, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }


            }

        }

        [HttpPost]
        public JsonResult SaveImage(HttpPostedFileBase File)
        {
            //HttpPostedFileBase File = Files[0];
            if (File != null)
            {
                //foreach (HttpPostedFileBase File in Files)
                //{}
                var fileName = Path.GetFileName(File.FileName);
                var extention = Path.GetExtension(File.FileName);
                var filenamewithoutextension = Path.GetFileNameWithoutExtension(File.FileName);
                fileName = filenamewithoutextension + DateTime.Now.ToString("yyMMddHH") + extention;
                //var filenamewithoutextension = Path.GetFileNameWithoutExtension(ImageFile.FileName);

                File.SaveAs(Server.MapPath("/Content/EmployeesRequestsImage/" + fileName /*ImageFile.FileName*/));

                //  Session["FileName"] = "/Content/IndemnitiesAttaches/" + fileName;
            }

            return new JsonResult { Data = File.FileName, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SaveRequest(Enum_Requests data)
        {
            if (data.ID != 0)
            {
                Enum_Requests model = db.Enum_Requests.Find(data.ID);
                model.REQ_DATE = DateTime.Now;
                model.CARD_ID = data.CARD_ID;
                model.TYPE = data.TYPE;
                model.NOTES = data.NOTES;
                model.TYP_ANAME = data.TYP_ANAME;
                model.PR_ENAME = data.PR_ENAME;
                model.APPROVAL_IMAGE = data.APPROVAL_IMAGE;
                db.Entry(model).State = EntityState.Modified;
                var x = db.SaveChanges();
                return new JsonResult { Data = new { respcode = model.ID, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            data.REQ_DATE = DateTime.Now;
            data.EMP_ENAME = User.Identity.GetUserName();
            data.REQ_TYPE = "M";

            data.REQUEST_TYP = "Web";
            data.CREATED_BY = User.Identity.GetUserName();
            data.CREATED_DATE = DateTime.Now;

            data.STATE = 2;
            //data.IMAGE = ima;

            db.Enum_Requests.Add(data);

            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = data.ID;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SearchRequest(long searchId)
        {
            var model = db.Enum_Requests.Where(r => r.ID == searchId).FirstOrDefault();
            if (model == null)
            {
                return new JsonResult { Data = new { modelreturn = 0, msg = "There's no data for that Request Code : " + searchId }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            var statss = "";
            if (model.STATE == 2)
            {
                statss = "pending";

            }
            else if (model.STATE == 1)
            {
                statss = "accept";
            }
            else if (model.STATE == 0)
            {
                statss = "refuse";
            }
            Enum_RequestsViewModel ENUM_REQUESTS = new Enum_RequestsViewModel
            {
                CompName = model.CARD_ID.Split('-')[0].ToString(),
                CARD_ID = model.CARD_ID,
                TYPE = model.TYPE,
                NOTES = model.NOTES,
                TYP_ANAME = model.TYP_ANAME,
                PR_ENAME = model.PR_ENAME,
                //IMAGE = model.IMAGE,
                ///مرجعه في دا عشان string
                REPLAYED_BY = statss,
                ID = model.ID
            };

            return new JsonResult { Data = new { modelreturn = ENUM_REQUESTS, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]

        public JsonResult EditRequest(Enum_Requests data)
        {
            var files = db.Enum_Requests.Where(x => x.ID == data.ID).FirstOrDefault();

            if (files == null)
            {
                return new JsonResult { Data = new { respcode = 0, msg = "There's no data for that Request Code : " + data.ID }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }

#pragma warning disable CS0219 // The variable 'ima' is assigned but its value is never used
            var ima = "";
#pragma warning restore CS0219 // The variable 'ima' is assigned but its value is never used
            //if (data.IMAGE != string.Empty && data.IMAGE != null && data.IMAGE != files.IMAGE)
            //{
            //    var nameimg = data.IMAGE;
            //    string[] single = nameimg.Split('.');
            //    ima = single[0] + DateTime.Now.ToString("yyMMddHH") + '.' + single[1];
            //}
            //else
            //{
            //    ima = files.IMAGE;
            //}
            //data.IMAGE = ima;


            files.CARD_ID = data.CARD_ID;
            files.TYPE = data.TYPE;
            files.TYP_ANAME = data.TYP_ANAME;
            files.PR_ENAME = data.PR_ENAME;
            files.NOTES = data.NOTES;
            //files.IMAGE = data.IMAGE;

            db.Entry(files).State = EntityState.Modified;
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = data.ID;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }



        public ReturnResult ChicActiveCard(string compid, string CardId)
        {
            int CompId = Convert.ToInt32(compid);
            try
            {
                var emp = db.Contract_Comp.Where(x => x.C_COMP_ID == CompId).FirstOrDefault().ACTIVE;

                var empCardTerminationFlag = db.Comp_Employees.Where(x => x.CARD_ID == CardId && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                if (empCardTerminationFlag != null)
                {
                    if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "N")
                    {
                        return (new ReturnResult { data = "Y", message = "ok" });
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE > DateTime.Now)
                    {
                        return (new ReturnResult { data = "Y", message = "ok" });
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE < DateTime.Now)
                    {
                        return (new ReturnResult { data = "N", message = "Expired Card" });
                    }
                }
                else
                {
                    var CompTerminationFlag = db.Contract_Data.Where(x => x.C_COMP_ID == CompId && x.DATE_FROM <= DateTime.Now && x.DATE_TO >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (CompTerminationFlag != null)
                    {
                        return (new ReturnResult { data = "N", message = "Card is not existed" });

                    }
                }
                return (new ReturnResult { data = "N", message = "Expired Company" });
            }
            catch (Exception ex)
            {
                return (new ReturnResult { data = "EX", message = ex.Message });
            }
        }



        public class ReturnResult
        {
            public string data { get; set; }
            public string message { get; set; }
        }

        public class ClassCodeSt
        {
            public string ClassCode { get; set; }
            public string ClassString { get; set; }
        }

        public void SendMail(string to, string subject, string Message, AlternateView altView, ApplicationUser applicationUser)
        {

            //StringBuilder strBody = new StringBuilder();
            //strBody.Append("One Try To login To Ypur Accoun");
            //var EmailAndPassword = db.ProviderEmails.Where(p => p.PrvoderCode == applicationUser.Provider).FirstOrDefault();
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("dmsdms032@gmail.com", to, subject, Message);
            //pasing the Gmail credentials to send the email
            mail.AlternateViews.Add(altView);
            System.Net.NetworkCredential mailAuthenticaion = new System.Net.NetworkCredential("dmsdms032@gmail.com", "ngjmgonhutkyfftv");

            System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            mailclient.EnableSsl = true;
            mailclient.UseDefaultCredentials = false;
            mailclient.Credentials = mailAuthenticaion;
            mail.IsBodyHtml = true;
            //mailclient.UseDefaultCredentials = false;
            mailclient.Send(mail);
        }


        public List<Comp_Employees> CardList(string CardId)
        {
            var EmpCode = long.Parse(CardId.Split('-')[2]);
            var CompId = int.Parse(CardId.Split('-')[0]);
            var Employee = db.Comp_Employees.Where(e => e.CARD_ID == CardId &&
                             DateTime.Now >= e.INS_START_DATE && DateTime.Now <= e.INS_END_DATE)
                            .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();
            if (Employee != null)
            {
                var maxContract = Employee.CONTRACT_NO;
                var subCards = db.Comp_Employees.Where(e => e.EMP_CODE == EmpCode &&
                               (e.TERMINATE_FLAG == "N" || e.TERMINATE_FLAG == null) && e.C_COMP_ID == CompId
                               && e.CONTRACT_NO == maxContract).ToList();
                var compStatement = db.CompStatements.Where(c => c.ContractNo == maxContract && c.MainCompCode == CompId).FirstOrDefault();
                if (compStatement != null)
                {
                    var cards = db.Comp_Employees.Where(e => e.C_COMP_ID == compStatement.CompID
                      && e.CONTRACT_NO == maxContract && e.EMP_CODE == EmpCode).ToList();
                    subCards.AddRange(cards);
                }
                return subCards;
            }
            else
            {
                return new List<Comp_Employees>();
            }
        }
        #endregion

    }
}