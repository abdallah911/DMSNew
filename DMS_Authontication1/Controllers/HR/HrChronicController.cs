using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR,User")]
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class HrChronicController : Controller
    {
        DMS_TESTEntities db = new DMS_TESTEntities();
        ApplicationDbContext myEntities = new ApplicationDbContext();
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
        public ActionResult InquiryAboutCard()
        {
            if (User.IsInRole("HR"))
            {
                var HrUserNamre = User.Identity.GetUserName();
                ViewBag.compnum = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            }
            else if (User.IsInRole("User"))
            {
                var usr = User.Identity.GetUserId();
                var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                ViewBag.cardID = cardId;
                ViewBag.compnum = cardId.Split('-')[0];


                var subCards = CardList(usr);

                var cards = subCards.Select(c => new
                {
                    CardIDValue = c.CARD_ID,
                    CardIdString = c.CARD_ID
                }).ToList();
                SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
                ViewBag.Cardslist = Cardlist;
            }


            return View();
        }

        [HttpGet]
        public ActionResult CreateRequest(int? id)
        {
            Enum_RequestsViewModel ENUM_REQUESTSViewModel = new Enum_RequestsViewModel();
            var HrUserNamre = User.Identity.GetUserName();
            var comp_id = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            ENUM_REQUESTSViewModel.CompName = comp_id;


            var comp = Convert.ToInt32(comp_id);
            var datenow = DateTime.Now.Date;
            var employees = db.Comp_Employees.Where(m => m.C_COMP_ID == comp && m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
                  .Select(l => new
                  {
                      CARD_ID = l.CARD_ID,
                      EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
                  }).ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(employees, "CARD_ID", "EMP_ANAME");
            ViewBag.address = addresslist;

            if (User.IsInRole("User"))
            {
                var usr = User.Identity.GetUserId();
                var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                ENUM_REQUESTSViewModel.CARD_ID = cardId;
                ENUM_REQUESTSViewModel.CompName = cardId.Split('-')[0];
                var comp2 = int.Parse(ENUM_REQUESTSViewModel.CompName);
                var employees2 = db.Comp_Employees.Where(m => m.C_COMP_ID == comp2 && m.CARD_ID == cardId &&
                m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
                  .Select(l => new
                  {
                      CARD_ID = l.CARD_ID,
                      EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
                  }).ToList();
                //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
                SelectList addresslist2 = new SelectList(employees2, "CARD_ID", "EMP_ANAME");
                ViewBag.address = addresslist2;
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

        [HttpPost]

        public ActionResult CreateRequest(Enum_RequestsViewModel addApproval)
        {
            var userid = User.Identity.GetUserId();
            var CompProvider = UserManager.FindById(userid);
            if (!ModelState.IsValid)
            {
                #region Error
                Enum_RequestsViewModel ENUM_REQUESTSViewModel = new Enum_RequestsViewModel();
                //var HrUserNamre = User.Identity.GetUserName();
                //var comp_id = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ENUM_REQUESTSViewModel.CompName = CompProvider.Provider;
                ENUM_REQUESTSViewModel.TYPE = addApproval.TYPE; ;
                ENUM_REQUESTSViewModel.PR_ENAME = addApproval.PR_ENAME;
                ENUM_REQUESTSViewModel.TYP_ANAME = addApproval.TYP_ANAME;
                ENUM_REQUESTSViewModel.NOTES = addApproval.NOTES;
                ENUM_REQUESTSViewModel.CARD_ID = addApproval.CARD_ID;
                ENUM_REQUESTSViewModel.MAIL_SEND = addApproval.MAIL_SEND;

                var comp = Convert.ToInt32(CompProvider.Provider);
                var datenow = DateTime.Now.Date;
                var employees = db.Comp_Employees.Where(m => m.C_COMP_ID == comp && m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
                      .Select(l => new
                      {
                          CARD_ID = l.CARD_ID,
                          EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
                      }).ToList();
                //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
                SelectList addresslist = new SelectList(employees, "CARD_ID", "EMP_ANAME");
                ViewBag.address = addresslist;
                if (User.IsInRole("User"))
                {
                    var usr = User.Identity.GetUserId();
                    var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                    ENUM_REQUESTSViewModel.CARD_ID = cardId;
                    ENUM_REQUESTSViewModel.CompName = cardId.Split('-')[0];
                    var comp2 = int.Parse(ENUM_REQUESTSViewModel.CompName);
                    var employees2 = db.Comp_Employees.Where(m => m.C_COMP_ID == comp2 &&m.CARD_ID== cardId&&
                    m.INS_START_DATE <= datenow && m.INS_END_DATE >= datenow && ((m.TERMINATE_FLAG == "N" || m.TERMINATE_FLAG == null) || (m.TERMINATE_FLAG == "Y" && m.TERMINATE_DATE >= datenow)))
                      .Select(l => new
                      {
                          CARD_ID = l.CARD_ID,
                          EMP_ANAME = l.CARD_ID + " | " + l.EMP_ANAME
                      }).ToList();
                    //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
                    SelectList addresslist2 = new SelectList(employees2, "CARD_ID", "EMP_ANAME");
                    ViewBag.address = addresslist2;
                }
                #endregion
                return View(ENUM_REQUESTSViewModel);

            }
            var model = new Enum_Requests();
            List<string> paths = new List<string>();
            List<string> exten = new List<string>();

            model.NOTES = addApproval.NOTES;
            model.REQ_DATE = DateTime.Now;
            model.EMP_ENAME = User.Identity.GetUserName();
            model.REQ_TYPE = "M";

            model.REQUEST_TYP = "Web";
            model.CREATED_BY = User.Identity.GetUserName();
            model.CREATED_DATE = DateTime.Now;
            model.STATE = 2;
            model.TYPE = addApproval.TYPE; ;
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
            int com = int.Parse(model.CARD_ID.Trim().Split('-')[0]);
            var compID = Convert.ToInt32(CompProvider.Provider);
            var companyName = db.Contract_Comp.Where(c => c.C_COMP_ID == com).FirstOrDefault().C_ANAME;

            string sub = "Request Approval From " + companyName;

            string msg = @"<h3> Send By: </h3>" + CompProvider.FName +
                          "<h3> Replay To Email :  </h3>" + addApproval.MAIL_SEND + "<br/>" +

                          "<h4> Card Id: </h4>" + addApproval.CARD_ID + "<br/>" +
                          "<h4> Approval Type  :  </h4>" + addApproval.TYPE + "<br/>" +
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

            SendMail("dms.pharmacy1@gmail.com", sub, msg, altView, CompProvider);
            SendMail("dr.mona.abdallah@dms-eg.com", sub, msg, altView, CompProvider);

            //SendMail("dms.medical1@gmail.com", sub, msg, altView, CompProvider);
            //SendMail("dms.medical2@gmail.com", sub, msg, altView, CompProvider);


            return RedirectToAction("InquiryAboutCard");
        }

        #region help Methods

        public JsonResult GetEmployeeChronicData(string cardId, string compId)
        {
            int Comp_ID = Convert.ToInt32(compId);
            var cardExist = db.Comp_Employees.AsNoTracking().Where(c => c.CARD_ID == cardId).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault();
            if (cardExist == null)
            {
                return new JsonResult { Data = new { employee = 0, roshDetails = 0, msg = "Card Not Found ..." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                //return new JsonResult { Data = new { providerslist = 0, msg = "Card Not Found ..." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                //var coverdRelation = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == Comp_ID && c.CLASS_CODE ==
                //                         cardExist.CLASS_CODE && c.CONTRACT_NO == cardExist.CONTRACT_NO).FirstOrDefault().COVER_RELATION;

                var status = ChicActiveCard(compId, cardId);
                if (status.data == "Y" && status.message == "ok")
                {
                    var medCard = db.Med_Card.AsNoTracking().Where(m => m.CARD_NO == cardId).OrderByDescending(x => x.CREATED_DATE).FirstOrDefault();

                    if (medCard == null)
                    {
                        return new JsonResult { Data = new { employee = 0, roshDetails = 0, msg = "No Chronic For This Card Number" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                    }

                    var provider = db.Serv_Providers1.Where(p => p.PR_CODE == medCard.PROVIDER_CODE).FirstOrDefault();


                    var Employee = new HrChronicEmployee
                    {
                        CardId = medCard.CARD_NO,
                        EmployeeFN = cardExist.EMP_ANAME_ST,
                        EmployeeSN = cardExist.EMP_ANAME_SC,
                        EmployeeTN = cardExist.EMP_ANAME_TH,
                        ProviderCode = medCard.PROVIDER_CODE,
                        ProviderName = provider.PR_ANAME,
                        PharmacyName = "---",
                        UserName = "",
                        CreatedDate = "",
                        GroupName = medCard.GROUP_NAME
                        //RoshitaId = roshita.Id
                    };


                    var roshita = db.Roshitas.AsNoTracking().Where(r => r.CardId == cardId && r.Manager == "Pharmacy_Chronic")
                        .OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (roshita == null)
                    {
                        return new JsonResult { Data = new { employee = Employee, roshDetails = 0, msg = "No Roshita Details For This Card Number" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                    }

                    var user = int.Parse(myEntities.Users.Where(u => u.UserName == roshita.CreatedBy).FirstOrDefault().Provider);

                    var providerRoshita = db.SERV_PROVIDERS_NEW.Where(p => p.PR_CODE == user).FirstOrDefault();

                    Employee.RoshitaId = roshita.Id;
                    Employee.CreatedDate = string.Format("{0:g}", roshita.CreatedDate);
                    Employee.UserName = roshita.CreatedBy;
                    Employee.PharmacyName = providerRoshita.PR_ANAME;

                    DateTime datenow = DateTime.Now;
                    var compare = new DateTime(datenow.Year, datenow.Month, datenow.Day);
                    var roshitaDetails = db.Med_Medicine.Where(item => item.CARD_NO == cardId && item.ACTIVE == "Y" && (item.MONTH_DATE_STOP >=
                                 compare || item.MONTH_DATE_STOP == null))
                         .AsEnumerable().Select(x => new RoshitaChronicDetails
                         {
                             MedicienCode = x.MED_CODE,
                             MedicienName = x.MED_NAME,
                             TotalDuration = x.DOS_DUR ?? 0,
                             //IsDealed = x.IsDealed,
                             Duration = x.NO_OF_UINT ?? 0,
                             ACTIVE = x.ACTIVE,
                             DOSAGE_FORM = x.DOSAGE_FORM,
                             PACK_SIZE = x.PACK_SIZE,
                             UNIT_NO = x.UNIT_NO,
                             MedicineCreatedDate = string.Format("{0:g}", x.CREATED_DATE)
                         }).ToList();


                    //var roshitaDetails = (from roshDet in db.RoshitaDetails
                    //                      where roshDet.RoshitaID == Employee.RoshitaId
                    //                      join MedDat in db.MedicineDatas
                    //                      on roshDet.MedicienCode equals MedDat.M_CODE
                    //                      select new RoshitaChronicDetails
                    //                      {
                    //                          MedicienCode = roshDet.MedicienCode,
                    //                          MedicienName = roshDet.MedicienName,
                    //                          TotalDuration = roshDet.TotalDuration,
                    //                          IsDealed = roshDet.IsDealed,
                    //                          Duration = roshDet.Duration,
                    //                          ACTIVE = MedDat.ACTIVE,
                    //                          DOSAGE_FORM = MedDat.DOSAGE_FORM,
                    //                          PACK_SIZE = MedDat.PACK_SIZE,
                    //                          UNIT_NO = MedDat.UNIT_NO
                    //                      }).ToList();
                    //foreach (var item in roshitaDetails)
                    //{
                    //    var credate = db.Med_Medicine.AsNoTracking().Where(m => m.MED_CODE == item.MedicienCode && m.CARD_NO == roshita.CardId)
                    //        .OrderByDescending(x => x.CREATED_DATE).FirstOrDefault();
                    //    if (credate == null)
                    //    {
                    //        item.MedicineCreatedDate = "----";
                    //    }
                    //    else
                    //        item.MedicineCreatedDate = string.Format("{0:g}", credate.CREATED_DATE);
                    //}
                    return new JsonResult { Data = new { employee = Employee, roshDetails = roshitaDetails, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


                }
                else
                {
                    return new JsonResult { Data = new { employee = 0, roshDetails = 0, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    //return new JsonResult { Data = new { providerslist = 0, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }


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

        public void SendMail(string to, string subject, string Message, AlternateView altView, ApplicationUser applicationUser)
        {

            //StringBuilder strBody = new StringBuilder();
            //strBody.Append("One Try To login To Ypur Accoun");
            //var EmailAndPassword = db.ProviderEmails.Where(p => p.PrvoderCode == applicationUser.Provider).FirstOrDefault();
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("dmsdms032@gmail.com", to, subject, Message);
            //pasing the Gmail credentials to send the email
            mail.AlternateViews.Add(altView);
            System.Net.NetworkCredential mailAuthenticaion = new System.Net.NetworkCredential("dmsdms032@gmail.com", "Dms123456");

            System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            mailclient.EnableSsl = true;
            mailclient.UseDefaultCredentials = false;
            mailclient.Credentials = mailAuthenticaion;
            mail.IsBodyHtml = true;
            //mailclient.UseDefaultCredentials = false;
            mailclient.Send(mail);
        }

        public List<Comp_Employees> CardList(string userId)
        {
            var CardId = db.EmployeePersonalDatas.Where(e => e.UserId == userId).FirstOrDefault().CardId;
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
                return subCards;
            }
            else
            {
                return new List<Comp_Employees>();
            }
        }

        public class ReturnResult
        {
            public string data { get; set; }
            public string message { get; set; }
        }

        public class RoshitaChronicDetails
        {
            public string MedicienCode { get; set; }
            public string MedicienName { get; set; }
            public int Duration { get; set; }
            public int TotalDuration { get; set; }
            public Nullable<bool> IsDealed { get; set; }
            public string DOSAGE_FORM { get; set; }
            public double? PACK_SIZE { get; set; }
            public int? UNIT_NO { get; set; }
            public string ACTIVE { get; set; }
            public string MedicineCreatedDate { get; set; }
        }


        #endregion

    }
}