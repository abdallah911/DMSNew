using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_Authontication1.ViewModel.HospitalVM;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.HospitalSystem
{
    [Authorize(Roles = "Admin,Hospital,HR,User,HR_Admin")]
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class HospitalController : Controller
    {

        #region Fields

        DMS_TESTEntities db;
        ApplicationDbContext myEntities;
        DBApproval dbAproval;
        private ApplicationUserManager _userManager;

        #endregion


        #region Ctor

        public HospitalController()
        {
            db = new DMS_TESTEntities();
            myEntities = new ApplicationDbContext();
            dbAproval = new DBApproval();
        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns> hospital view </returns>
        public ActionResult Index()
        {
            // System.Data.SqlClient.SqlParameter[] @params =
            //{
            //   new System.Data.SqlClient.SqlParameter("@return_value", 0) {Direction = System.Data.ParameterDirection.Output}
            // };
            // var a = db.Database.ExecuteSqlCommand("exec @return_value = [dbo].[DB_A45413_DMSERP].[spGetNextClaimNO]", @params);

            // var result = @params[0].Value;

            var HrUserNamre = User.Identity.GetUserName();
            ViewBag.SpecialitySelect = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
            ViewBag.Provider = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> List of Hospital Claims for last 7 days only </returns>
        public ActionResult Claims()
        {
            var HrUserName = User.Identity.GetUserName();
            var Provider = Convert.ToInt32(myEntities.Users.Where(u => u.UserName == HrUserName).FirstOrDefault().Provider);
            DateTime dat = DateTime.Now.AddDays(-7);
            var claimList = db.HospitalClaims.Where(c => c.CreatedDate >= dat && c.PROVIDER_CODE == Provider && c.IsDeleted != true)
                .ToList().OrderBy(m => m.ID);
            return View(claimList);
        }


        public ActionResult AddApproval()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddApproval(AddApproval addApproval)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var model = new ApprovalMaster();
            List<string> paths = new List<string>();
            List<string> exten = new List<string>();

            model.Note = addApproval.Note;
            for (int i = 0; i < addApproval.ImageFile.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(addApproval.ImageFile[i].FileName);
                string extension = Path.GetExtension(addApproval.ImageFile[i].FileName);
                exten.Add(extension);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                model.ApprovalDetails.Add(new ApprovalDetail { FilePath = "~/Content/AddApprovalFiles/" + fileName });
                paths.Add("~/Content/AddApprovalFiles/" + fileName);

                addApproval.ImageFile[i].SaveAs(Server.MapPath("/Content/AddApprovalFiles/" + fileName /*ImageFile.FileName*/));
            }
            db.ApprovalMasters.Add(model);
            db.SaveChanges();
            var userid = User.Identity.GetUserId();
            var Hospitalprovider = UserManager.FindById(userid);
            string sub = @"Request Approval From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
            string msg = @"<h3> Approval Notes  : </h3>" + addApproval.Note + "<br/>";
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
            //+ "Attachments : <h3> <br/>" + br_ad + "<br/>"
            //+ "With Doctor  : <h3>" + doct_name + "<br/>"
            //+ "Cost of reservation : <h3>" + cost + "<br/>"
            //+ "On Day : <h3>" + day_re + "<br/>"
            //+ "Time : <h3>" + time_re;
            //SendMail("dms.medical1@gmail.com", sub, msg);
            //SendMail("dms.medical2@gmail.com", sub, msg);
            SendMail("dms.medical1@gmail.com", sub, msg, altView/*, Hospitalprovider*/);
            SendMail("dms.medical2@gmail.com", sub, msg, altView/*, Hospitalprovider*/);
            //SendMail("obad1452@gmail.com", sub, msg, altView, Hospitalprovider);

            return View();
        }

        public ActionResult Edit(long? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = db.HospitalClaims.Where(c => c.ID == Id && c.IsDeleted != true).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Hospital/Delete/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">Claim Id</param>
        /// <returns></returns>
        public ActionResult Delete(long? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = db.HospitalClaims.Find(Id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Hospital/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long Id)
        {
            var model = db.HospitalClaims.Find(Id);
            model.IsDeleted = true;
            model.DeletedBy = User.Identity.GetUserName();
            model.DeletedDate = DateTime.Now;
            //db.AfterSales.Remove(afterSale);
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Claims");
        }

        public void SendMail(string to, string subject, string Message, AlternateView altView/*, ApplicationUser applicationUser*/)
        {

            //StringBuilder strBody = new StringBuilder();
            //strBody.Append("One Try To login To Ypur Accoun");
            //var EmailAndPassword = db.ProviderEmails.Where(p => p.PrvoderCode == applicationUser.Provider).FirstOrDefault();
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("mediacl.approv@gmail.com" /*EmailAndPassword.Email*/, to, subject, Message);
            //pasing the Gmail credentials to send the email
            mail.AlternateViews.Add(altView);
            System.Net.NetworkCredential mailAuthenticaion = new System.Net.NetworkCredential("mediacl.approv@gmail.com", "Dms123456"/*EmailAndPassword.Email, EmailAndPassword.Password*/);

            System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            mailclient.EnableSsl = true;
            mailclient.UseDefaultCredentials = false;
            mailclient.Credentials = mailAuthenticaion;
            mail.IsBodyHtml = true;
            mailclient.Send(mail);
        }
        #endregion


        #region Helper Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> Card Id </param>
        /// <param name="provider"> provider code for the hospital </param>
        /// <returns> Employee data if founded and have contract </returns>
        public JsonResult AddCard(string id, int provider)
        {
            DateTime datenow = DateTime.Now.Date;
            var da = new DateTime(datenow.Year, datenow.Month, datenow.Day);
            var employe = db.Comp_Employees.Where(e => e.CARD_ID == id &&
                            da >= e.INS_START_DATE && da <= e.INS_END_DATE)
                           .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();
            if (employe == null)
            {
                var result = new { Success = "Enter Correct Card ID " };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                int compID = Convert.ToInt32(id.Split('-')[0].ToString());
                int Provider_Level = Convert.ToInt32(db.SERV_PROVIDERS_NEW.Where(s => s.PR_CODE == provider).FirstOrDefault().PROV_DEGREE);
                int HOSPITAL_DEGREE = Convert.ToInt32(db.CompContractClasses.Where(c => c.C_COMP_ID == compID
                                   && c.CONTRACT_NO == employe.CONTRACT_NO && c.CLASS_CODE == employe.CLASS_CODE)
                                   .FirstOrDefault().COVER_RELATION);
                var C_ENAME2 = db.Contract_Comp.Where(c => c.C_COMP_ID == compID).FirstOrDefault().C_ENAME;
                List<EmployeeHospitalVM> EmployeeVM = new List<EmployeeHospitalVM>();
                EmployeeVM.Add(new EmployeeHospitalVM
                {

                    EMP_ANAME = employe.EMP_ANAME_ST + " " + employe.EMP_ANAME_SC + " " + employe.EMP_ANAME_TH,
                    EMP_ENAME = employe.EMP_ENAME_ST + " " + employe.EMP_ENAME_SC + " " + employe.EMP_ENAME_TH,
                    INS_START_DATE = employe.INS_START_DATE,
                    INS_END_DATE = employe.INS_END_DATE,
                    TERMINATE_DATE = DateTime.Now,
                    TERMINATE_FLAG = employe.TERMINATE_FLAG,
                    CLASS_CODE = employe.CLASS_CODE,
                    COMP_ID = compID,
                    CONTRACT_NO = employe.CONTRACT_NO,
                    C_ENAME = C_ENAME2,
                    CARD_ID = id,
                    Provider_Level = Provider_Level,
                    Now = DateTime.Now,
                    HOSPITAL_DEGREE = HOSPITAL_DEGREE
                });
                var exception = (from a in db.Acceptions
                                 join c in db.CardAcceptionReasons on a.Id equals c.AcceptionId
                                 where (a.ProvidersId == 2 && a.AcceptionFlag == true
                                 && a.CompEmployeesId == employe.Id && c.AcceptionReasonsId == 8)
                                 select new
                                 {
                                     ExceptionId = a.Id
                                 }).Select(x => x.ExceptionId).FirstOrDefault();
                if (exception != 0)
                {
                    string message = "ok";
                    var result1 = new { Success = "True", Data = EmployeeVM, Exceptions = exception, messages = message };
                    return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
                var result = new { Success = "True", Data = EmployeeVM, Exceptions = exception, messages = "no" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
        }

        public JsonResult GetExceptions(string cardID, int exceptionReasonId)
        {
            var employe = db.Comp_Employees.Where(e => e.CARD_ID == cardID &&
                            DateTime.Now >= e.INS_START_DATE && DateTime.Now <= e.INS_END_DATE)
                           .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();
            if (employe == null)
            {
                var result = new { Success = "Enter Correct Card ID " };
                return new JsonResult { Data = new { msg = "No" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var exception = (from a in db.Acceptions
                                 join c in db.CardAcceptionReasons on a.Id equals c.AcceptionId
                                 where (a.ProvidersId == 2 && a.AcceptionFlag == true
                                 && a.CompEmployeesId == employe.Id && c.AcceptionReasonsId == exceptionReasonId)
                                 select new
                                 {
                                     ExceptionId = a.Id
                                 }).Select(x => x.ExceptionId).FirstOrDefault();
                if (exception != 0)
                {
                    return new JsonResult { Data = new { ExceptionResult = exception, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
                return new JsonResult { Data = new { ExceptionResult = exception, msg = "No" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="CompNumber"></param>
        /// <param name="ContractNumber"></param>
        /// <param name="ClassCode"></param>
        /// <returns> Result of Emergancy Covid19 Coverd or not </returns>
        public JsonResult ChickCrona(string CompNumber, int ContractNumber, string ClassCode)
        {
            var employe = db.CronaEmergancies.Where(e => e.CompId == CompNumber &&
                            DateTime.Now >= e.StartCover && DateTime.Now <= e.EndCover &&
                            e.ContractNo == ContractNumber && e.ClassCode == ClassCode)
                           .OrderByDescending(e => e.ContractNo).FirstOrDefault();
            if (employe == null)
            {
                var result = new { Success = "No" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = new { Success = "Yes" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }


        public JsonResult ChickNationalId(string NationalId, string cardId)
        {
            if (NationalId == "" || NationalId.Length != 14)
            {
                var result = new { Success = "No" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var employe = db.Comp_Employees.Where(e => e.CARD_ID == cardId &&
                            DateTime.Now >= e.INS_START_DATE && DateTime.Now <= e.INS_END_DATE)
                           .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();
            if (employe == null)
            {
                var result = new { Success = "No" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var birthDate = employe.BIRTH_DATE;
                string NationalBirthDate = NationalId.Substring(1, 6);
                string day = birthDate.Value.Day.ToString().Length == 1 ? "0" + birthDate.Value.Day.ToString() : birthDate.Value.Day.ToString();
                string month = birthDate.Value.Month.ToString().Length == 1 ? "0" + birthDate.Value.Month.ToString() : birthDate.Value.Month.ToString();
                string comparestring = birthDate.Value.Year.ToString().Substring(2, 2) +
                    month + day;

                if (NationalBirthDate.CompareTo(comparestring) == 0)
                {

                    var result = new { Success = "Yes" };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var result = new { Success = "No" };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CardId"></param>
        /// <param name="ContractNum"></param>
        /// <param name="ClassCode"></param>
        /// <param name="ProvideCode"></param>
        ///  For Get All Approvals belong to specifec Employe of that Provider(Hospital)
        /// <returns> All Approvals belong to specifec Employe of that Provider(Hospital) </returns>

        public JsonResult Get_Approvels(string CardId, string ContractNum, string ClassCode, string ProvideCode)

        {

            DataTable dt = dbAproval.RunReader("SELECT CODE,APROVAL_TYP,MEDICAL_REPLAY,VALUE_AFTER,RECIV_DATE,CREATED_DATE,END_DATE,EXPAIRE_DATE,CREATED_BY FROM MEDICAL_APPROVALS WHERE CARD_NO = '"
                                        + CardId + "' AND CLASS_CODE = '" + ClassCode + "'" +
                                        " AND COMP_CONTRACT_NO = '" + ContractNum + "' AND PROVIDER_ID IN(" + ProvideCode + ", 16159) AND EXPAIRE_DATE >= sysdate-14 ORDER BY CREATED_DATE DESC");
            List<Approval> approval = new List<Approval>();
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    approval.Add(new Approval
                    {
                        Code = row["CODE"].ToString(),
                        Approval_Type = row["APROVAL_TYP"].ToString(),
                        Medical_Replay = row["MEDICAL_REPLAY"].ToString(),
                        Value_After = int.Parse(row["VALUE_AFTER"].ToString()),
                        Recieve_Date = row["RECIV_DATE"].ToString(),
                        Created_Date = row["CREATED_DATE"].ToString(),
                        End_Date = row["END_DATE"].ToString(),
                        Expaire_Date = row["EXPAIRE_DATE"].ToString(),
                        CreatedBy = row["CREATED_BY"].ToString()

                    });
                }
                return new JsonResult { Data = approval, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
                return new JsonResult { Data = approval, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApprovalCode"></param>
        /// <returns> PDF file of the approval </returns>

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Print(string ApprovalCode)
        {

            DataTable dts = dbAproval.RunReader("SELECT CARD_NO,DIAG_NAME,SERVECE_TYP,CREATED_BY FROM MEDICAL_APPROVALS WHERE CODE='" + ApprovalCode + "'");
            string cardNum = dts.Rows[0][0].ToString();
            DataTable dtsrev = dbAproval.RunReader(@"select APPROVAL_SUB_SERV.S_SERV_NAME, CASE WHEN (S_SERV_NAME = 'الاقامة'OR S_SERV_CODE LIKE '114%') 
                                                                        THEN CONCAT(APPROVAL_SUB_SERV.DETAILS, APPROVAL_SUB_SERV.DISCRIPTION) ELSE APPROVAL_SUB_SERV.DETAILS END
                                                             FROM  APPROVAL_SUB_SERV WHERE APPROVAL_SUB_SERV.CODE = '" + ApprovalCode + "'");
            DataTable dtdiag = dbAproval.RunReader(@"select MEDICAL_APPROVALS.APROVAL_IMAG, APPROVAL_DIAG.DIAG_NAME, MEDICAL_APPROVALS.PATH FROM      MEDICAL_APPROVALS, APPROVAL_DIAG
                                                              WHERE     MEDICAL_APPROVALS.CODE = APPROVAL_DIAG.CODE  AND MEDICAL_APPROVALS.CODE = '" + ApprovalCode + "' ");
            string DServ_Details = "";
            string DDiag_Details = "";

            if (dtsrev.Rows.Count != 0)
            {
                for (int i = 0; i < dtsrev.Rows.Count; i++)
                    DServ_Details = DServ_Details + dtsrev.Rows[i][0].ToString() + " : " + dtsrev.Rows[i][1].ToString() + "\n";
            }
            try
            {
                if (dtdiag.Rows.Count != 0)
                {
                    for (int i = 0; i < dtdiag.Rows.Count; i++)
                    {
                        DDiag_Details = DDiag_Details + dtdiag.Rows[i][1].ToString() + "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Hospital"), "ReportApproval.rpt"));

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("crd", cardNum);
            rd.SetParameterValue("cod", ApprovalCode);
            rd.SetParameterValue("diag", DDiag_Details);
            rd.SetParameterValue("service", DServ_Details);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();

                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", DateTime.Now.ToString("ddMMyyyy") + "MedicalApproval.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #region Old Report
            //DataTable dtApproval = dbAproval.RunReader("select MEDICAL_APPROVALS.* , SEALS.image_seal from MEDICAL_APPROVALS left outer join SEALS on (MEDICAL_APPROVALS.servece_typ = SEALS.provider AND MEDICAL_APPROVALS.replay = SEALS.answer AND MEDICAL_APPROVALS.created_by = SEALS.doctor_name) WHERE       MEDICAL_APPROVALS.CODE= '" + ApprovalCode + "' AND  MEDICAL_APPROVALS.CARD_NO = '" + dts.Rows[0][0].ToString() + "' ORDER BY MEDICAL_APPROVALS.CREATED_DATE DESC");
            //string diaggg = dts.Rows[0][1].ToString();
            //string services = dts.Rows[0][2].ToString();


            //byte[] imageByteData;
            //ViewBag.createBy = dts.Rows[0][3].ToString();
            //List<ApprovalReport> approvalReports = new List<ApprovalReport>();
            //if (dtApproval.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dtApproval.Rows)
            //    {
            //        if (row["BIRTHDAY"].ToString() != "")
            //            birthDate = Convert.ToDateTime(row["BIRTHDAY"].ToString());
            //        if (row["START_DATE"].ToString() != "")
            //            startDaaate = Convert.ToDateTime(row["START_DATE"].ToString());
            //        if (row["END_DATE"].ToString() != "")
            //            endDaate = Convert.ToDateTime(row["END_DATE"].ToString());
            //        if (row["CREATED_DATE"].ToString() != "")
            //            approvalDate = Convert.ToDateTime(row["CREATED_DATE"].ToString());
            //        approvalReports.Add(new ApprovalReport
            //        {
            //            ApprovalCode = row["CODE"].ToString(),
            //            PatientName = row["EMP_ANAME"].ToString(),
            //            ApprovalType = row["SERVECE_TYP"].ToString(),
            //            ApprovalDate = approvalDate.ToShortDateString(),
            //            CardNo = row["CARD_NO"].ToString(),
            //            CompanyName = row["COMP_NAME"].ToString(),
            //            DateOfBirth = birthDate.ToShortDateString(),
            //            StartDate = startDaaate.ToShortDateString(),
            //            EndDate = endDaate.ToShortDateString(),
            //            Email = row["EMAIL"].ToString(),
            //            Diagnos = diaggg,
            //            Service = services,
            //            MedicalReplay = row["MEDICAL_REPLAY"].ToString(),
            //            ReplayNotes = row["NOTS"].ToString()
            //        });
            //        ViewBag.img = null;
            //        if (!DBNull.Value.Equals(row["APROVAL_IMAG"]))
            //        {
            //            imageByteData = (byte[])row["APROVAL_IMAG"];
            //            ViewBag.img = imageByteData;
            //        }
            //        ViewBag.approvalImage = null;
            //        ViewBag.logo = "/Content/images/logo.png";
            //        if (!DBNull.Value.Equals(row["image_seal"]))
            //        {
            //            imageByteData = (byte[])row["image_seal"];
            //            ViewBag.approvalImage = imageByteData;
            //        }



            //    }

            //}
            //return View("~/Views/User/ApprovalReport.cshtml", approvalReports);

            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Services_id"></param>
        /// <param name="cardID"></param>
        /// <param name="ContractNum"></param>
        /// <param name="ClassCode"></param>
        /// <returns> list of specialists that belongs to the main service
        /// and check if it coverd or not </returns>
        public JsonResult Get_Specialist(string Services_id, string cardID, int ContractNum, string ClassCode)
        {
            int compID = Convert.ToInt32(cardID.Split('-')[0].ToString());
            var EmployeeService = db.Comp_Customized_D_D.Where(e => e.SER_SERV == Services_id && e.C_COMP_ID == compID &&
            e.CLASS_CODE == ClassCode && e.CONTRACT_NO == ContractNum).FirstOrDefault();
            string msg = "";
            List<SERV_PROVIDERS> Specialists = new List<SERV_PROVIDERS>();
            if (EmployeeService != null)
            {
                if (Services_id == "11301" || Services_id == "11414")
                {
                    if (EmployeeService.REFUND_FLAG != "Y")
                    {
                        msg = "ok";
                    }
                    else
                    {
                        msg = " لا يمكن تقديم الخدمة لهذا الموظف حيث ان الخدمة غير مغطاة وسوف يتحمل المريض اجمالى قيمة الخدمة نقدا  ";
                    }
                }
                else if (Services_id == "11201" || Services_id == "11206")
                {
                    //var classCod = int.Parse(ClassCode);
                    var chickemployee = db.CronaEmergancies.Where(c => c.CompId == compID.ToString() &&
                      c.ContractNo == ContractNum && c.ClassCode == ClassCode).FirstOrDefault();
                    if (chickemployee != null)
                    {
                        if (Services_id == "11201" && chickemployee.HasHospitalLabAndRay == true)
                        {
                            msg = "ok";
                        }
                        else if (Services_id == "11206" && chickemployee.HasHospitalLabAndRay == true)
                        {
                            msg = "ok";
                        }
                        else
                        {
                            msg = " لا يمكن تقديم الخدمة لهذا الموظف حيث ان الخدمة غير مغطاة وسوف يتحمل المريض اجمالى قيمة الخدمة نقدا  ";
                        }
                    }
                    else
                    {
                        msg = "ok";
                        //msg = " لا يمكن تقديم الخدمة لهذا الموظف حيث ان الخدمة غير مغطاة وسوف يتحمل المريض اجمالى قيمة الخدمة نقدا  ";
                    }
                }
                else
                {
                    msg = "ok";
                }
            }
            else
            {
                msg = " لا يمكن تقديم الخدمة لهذا الموظف حيث ان الخدمة غير مغطاة وسوف يتحمل المريض اجمالى قيمة الخدمة نقدا  ";
            }
            if (Services_id == "11204")
            {
                Specialists = db.SERVICES1.Where(x => x.SERV_CODE.Contains("11204")).Select(x => new SERV_PROVIDERS
                {
                    PR_CODE = x.SERV_CODE,
                    PR_ANAME = x.SERV_ANAME
                }).ToList();
            }
            #region old services
            //switch (Services_id)
            //{
            //    // To list Radio 
            //    case "11201":
            //        Specialists = db.SERVICES1.Where(s => s.SERV_CODE.StartsWith("11201")).Select(se =>
            //               new SERV_PROVIDERS
            //               {
            //                   PR_CODE = se.SERV_CODE,
            //                   PR_ANAME = se.SERV_ANAME
            //               }).ToList();
            //        break;

            //    // To list Laboratory
            //    case "11206":
            //        Specialists = db.SERVICES1.Where(s => s.SERV_CODE.StartsWith("11206")).Select(se =>
            //                new SERV_PROVIDERS
            //                {
            //                    PR_CODE = se.SERV_CODE,
            //                    PR_ANAME = se.SERV_ANAME
            //                }).ToList();
            //        break;

            //    // To list Optical 
            //    case "11301":
            //        Specialists = db.SERVICES1.Where(s => s.SERV_CODE.StartsWith("11301")).Select(se =>
            //                new SERV_PROVIDERS
            //                {
            //                    PR_CODE = se.SERV_CODE,
            //                    PR_ANAME = se.SERV_ANAME
            //                }).ToList();
            //        break;

            //    // To list Dential 
            //    case "11414":
            //        Specialists = db.SERVICES1.Where(s => s.SERV_CODE.StartsWith("11414")).Select(se =>
            //                new SERV_PROVIDERS
            //                {
            //                    PR_CODE = se.SERV_CODE,
            //                    PR_ANAME = se.SERV_ANAME
            //                }).ToList();
            //        break;

            //    // To list Out Patient Clinic 
            //    case "11203":
            //        Specialists = db.SERVICES1.Where(s => s.SERV_CODE.StartsWith("11203")).Select(se =>
            //                new SERV_PROVIDERS
            //                {
            //                    PR_CODE = se.SERV_CODE,
            //                    PR_ANAME = se.SERV_ANAME
            //                }).ToList();
            //        break;

            //    // To list Doctor Visit 
            //    case "11205":
            //        Specialists = db.DoctorSpecialists.Select(se =>
            //                new SERV_PROVIDERS
            //                {
            //                    PR_CODE = se.Id.ToString(),
            //                    PR_ANAME = se.SPECIALIST
            //                }).ToList();
            //        break;

            //    default:
            //        break;
            //}
            #endregion

            return new JsonResult { Data = new { Specialists = Specialists, msg = msg }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }


        //public JsonResult GetSpecialityList(  int sEcho = 1, int iDisplayStart = 0, int iDisplayLength = 24, string sSearch = "")
        //{
        //        if (sSearch != null)
        //        {
        //            sSearch = sSearch.ToLower();
        //            var result = new
        //            {
        //                sEcho = sEcho,
        //                aaData = db.Specialities1.Where(s => s.MainServiceCode.StartsWith(Services_id) &&
        //                (s.HospitalCode == Provider || s.HospitalCode == "0") && (s.MainServiceCode.StartsWith(sSearch) ||
        //                s.ServiceArName.Contains(sSearch) || s.ServiceEnName.Contains(sSearch))).OrderBy(m => m.MainServiceCode)
        //                .Select(se =>
        //                       new HospitalServices
        //                       {
        //                           Price = se.Price == null ? "0 | " + se.Id : se.Price.ToString() + " | " + se.Id,
        //                           ServiceName = se.ServiceArName
        //                       }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

        //                iTotalRecords = db.Specialities1.Count(),
        //                iTotalDisplayRecords = db.Specialities1.Count()
        //            };
        //            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        }
        //        else
        //        {
        //            var result = new
        //            {
        //                sEcho = sEcho,
        //                aaData = db.Specialities1.AsEnumerable().Select(se =>
        //                       new Specialities1
        //                       {
        //                           SPEC_ID = se.SPEC_ID,
        //                           SPEC_ANAME = se.SPEC_ANAME
        //                       }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),
        //                iTotalRecords = db.Specialities1.Count(),
        //                iTotalDisplayRecords = db.Specialities1.Count()
        //            };
        //            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        }


        //    }




        public JsonResult Get_Specialist2(string Provider, string Services_id, string cardID, int ContractNum, string ClassCode, int sEcho = 1, int iDisplayStart = 0, int iDisplayLength = 24, string sSearch = "")
        {
            if (Services_id != "" && Services_id != "0")
            {
                //var user = User.Identity.GetUserId();
                //var userModel = myEntities.Users.Where(u => u.Id == user).FirstOrDefault();

                if (sSearch != null)
                {
                    sSearch = sSearch.ToLower();
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.HospitalServices.Where(s => s.MainServiceCode.StartsWith(Services_id) &&
                        (s.HospitalCode == Provider || s.HospitalCode == "0") && (s.MainServiceCode.StartsWith(sSearch) ||
                        s.ServiceArName.Contains(sSearch) || s.ServiceEnName.Contains(sSearch))).OrderBy(m => m.MainServiceCode)
                        .Select(se =>
                               new HospitalServices
                               {
                                   Price = se.Price == null ? "0 | " + se.Id : se.Price.ToString() + " | " + se.Id,
                                   ServiceName = se.ServiceArName
                               }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.HospitalServices.Where(s => s.MainServiceCode.StartsWith(Services_id) && (s.HospitalCode == Provider || s.HospitalCode == "0")).Count(),
                        iTotalDisplayRecords = db.HospitalServices.Where(s => s.MainServiceCode.StartsWith(Services_id) && (s.HospitalCode == Provider || s.HospitalCode == "0")).Count()
                    };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var result = new
                    {
                        sEcho = sEcho,
                        aaData = db.HospitalServices.Where(s => s.MainServiceCode.StartsWith(Services_id) && (s.HospitalCode == Provider || s.HospitalCode == "0"))
                        .OrderBy(m => m.MainServiceCode).AsEnumerable().Select(se =>
                               new HospitalServices
                               {
                                   Price = se.Price == null ? "0" : se.Price.ToString(),
                                   ServiceName = se.ServiceArName
                               }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),
                        iTotalRecords = db.HospitalServices.Where(s => s.MainServiceCode.StartsWith(Services_id) && (s.HospitalCode == Provider || s.HospitalCode == "0")).Count(),
                        iTotalDisplayRecords = db.HospitalServices.Where(s => s.MainServiceCode.StartsWith(Services_id) && (s.HospitalCode == Provider || s.HospitalCode == "0")).Count()
                    };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = 0,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0
                };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }


        }


        public JsonResult GetDoctors(string Provider, int sEcho = 1, int iDisplayStart = 0, int iDisplayLength = 24, string sSearch = "")
        {

            if (sSearch != "")
            {
                sSearch = sSearch.ToLower();
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.HospitalDoctors.Where(s => s.ProviderId == Provider &&
                     (s.DoctorName.Contains(sSearch) ||
                    s.Degree.Contains(sSearch) || s.Speciality.Contains(sSearch))).OrderBy(m => m.Id)
                    .Select(se =>
                           new HospitalDovtors
                           {
                               Doctorid = se.Id.ToString(),
                               doctorName = se.Degree + " || " + se.DoctorName
                           }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.HospitalDoctors.Where(s => s.ProviderId == Provider).Count(),
                    iTotalDisplayRecords = db.HospitalDoctors.Where(s => s.ProviderId == Provider).Count()
                };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.HospitalDoctors.Where(s => s.ProviderId == Provider)
                    .OrderBy(m => m.Id).AsEnumerable().Select(se =>
                           new HospitalDovtors
                           {
                               Doctorid = se.Id.ToString(),
                               doctorName = se.Degree + " || " + se.DoctorName
                           }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),
                    iTotalRecords = db.HospitalDoctors.Where(s => s.ProviderId == Provider).Count(),
                    iTotalDisplayRecords = db.HospitalDoctors.Where(s => s.ProviderId == Provider).Count()
                };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }


        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="C_Com_ID"></param>
        /// <param name="Provider_Code"></param>
        /// <param name="Card_ID"></param>
        /// <param name="Services_ID"></param>
        /// <param name="CLAIM_NO"></param>
        /// <param name="Total_Cash"></param>
        /// <param name="Person_Payment"></param>
        /// <param name="Services"></param>
        /// <param name="Contract_Number"></param>
        /// <param name="Class_Code"></param>
        /// <param name="Comp_Payment"></param>
        /// <param name="TotalValue"></param>
        /// <param name="OverInsurance"></param>
        /// <param name="Cash"></param>
        /// <param name="ServType"></param>
        /// <param name="NATIONAL_ID"></param>
        /// <param name="Phone"></param>
        /// <param name="COMP_PERC"></param>
        /// <returns> for save hospital claims </returns>

        [HttpPost]
        public JsonResult Save([Bind] int C_Com_ID, int Provider_Code, string Card_ID, int Services_ID, string CLAIM_NO,
            string Total_Cash, string Person_Payment, string Services, string Contract_Number, string Class_Code
            , string Comp_Payment, string TotalValue, string OverInsurance, string Cash, string ServType, string NATIONAL_ID,
            string Phone, string COMP_PERC, string Notes, int? HospitalException, int? ExceptionLabRayDoctor
            , int? SpecalistID, string DoctorName)
        {
            string codeRequestDate;
            long len = db.HospitalClaims.DefaultIfEmpty().Max(r => r == null ? 0 : r.ID) + 1;
            string tim = DateTime.Now.Date.ToString("ddMMyyyy");
            codeRequestDate = tim + "-" + len.ToString();

            // Services = "كشف دكتور "
            HospitalClaim hospitalClaim = new HospitalClaim
            {
                ID = len,
                REQUEST_NUM = codeRequestDate,
                C_COMP_ID = C_Com_ID,
                PROVIDER_CODE = Provider_Code,
                CARD_ID = Card_ID,
                SERVICE_CODE = Services_ID.ToString(),
                SERVICES = Services,
                CLASS_CODE = Class_Code,
                CONTRACT_NO = Convert.ToInt32(Contract_Number),
                SERV_TYP = Convert.ToInt32(ServType),
                CLAIM_NO = CLAIM_NO,
                NATIONAL_ID = NATIONAL_ID,
                PHONE = Phone,
                COMP_PERC = Math.Round(Convert.ToDouble(COMP_PERC), 3),
                CASH = Math.Round(Convert.ToDouble(Cash), 3),
                TOTAL_VALUE = Math.Round(Convert.ToDouble(TotalValue), 3),
                OVER_INSURANCE = Math.Round(Convert.ToDouble(OverInsurance), 3),
                TOTAL_CASH = Math.Round(Convert.ToDouble(Total_Cash), 3),
                PERSON_PAYMENT = Math.Round(Convert.ToDouble(Person_Payment), 3),
                COMPANY_PAYMENT = Math.Round(Convert.ToDouble(Comp_Payment), 3),
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.Name,
                Note = Notes,
                IsDeleted = false,
                DoctorName = DoctorName,
                SpecialistId = SpecalistID
            };
            if (Services_ID == 11205)
            {
                string checkdoctorvisit = checkDoctorVisit(C_Com_ID, Provider_Code, Card_ID, Services_ID.ToString(), Services);
                if (checkdoctorvisit == "sorry")
                {

                    return new JsonResult { Data = new { result = "لا يمكن حفظ الكشف وذلك لعدم مرور 7 ايام من تاريخ الكشف لنفس التخصص ", ok = "NO" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    // Services = "كشف دكتور ";

                    var x = db.HospitalClaims.Add(hospitalClaim);
                    var xx = db.SaveChanges();

                    //x = db.RunNonQuery
                    //   ("insert into PATIENT_REQUIST(REQUEST_NUM,C_COMP_ID,PROVIDER_CODE,CARD_ID,SERVICE_CODE,TOTAL_CASH," +
                    //   "PERSON_PAYMENT,SERVICES,CREATED_DATE,COMPANY_PAYMENT,ID,CLASS_CODE,CONTRACT_NO" +
                    //   ",TOTAL_VALUE,OVER_INSURANCE,CASH,COMP_PERC,PHONE,NATIONAL_ID,CLAIM_NO,SERV_TYP)" +
                    //   "values('" + codeRequestDate + "','" + C_Com_ID + "','" + Provider_Code + "','" + Card_ID + "','" + Services_ID + "','"
                    //       + Total_Cash + "','" + Person_Payment + "','" + Services + "',sysdate , '" + Comp_Payment + "' , '" + Convert.ToInt32(len) + "' , '" + Class_Code + "' ," + Contract_Number +
                    //      "," + TotalValue + "," + OverInsurance + "," + Cash + "," + COMP_PERC + "," + Phone + "," + NATIONAL_ID +
                    //      "," + CLAIM_NO + "," + ServType + ")");


                    if (xx > 0)
                        return new JsonResult { Data = new { result = "تم حفظ العملية بنجاح كود الموافقة  :" + codeRequestDate, ID = hospitalClaim.IdPrimary, msg = "OK" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    else
                        return new JsonResult { Data = new { result = "Invalid Request", msg = "NO" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }
            else
            {
                if (Services_ID == 111 && Services == "")
                    Services = "InPatient";

                if (Services_ID == 11104)
                    Services = "Emergency_Service";
                hospitalClaim.SERVICES = Services;
                var x = db.HospitalClaims.Add(hospitalClaim);
                var xx = db.SaveChanges();
                if (xx > 0)
                {
                    if (HospitalException != 0 && HospitalException != null)
                    {
                        var exc = db.Acceptions.Where(a => a.Id == HospitalException).FirstOrDefault();
                        exc.AcceptionFlag = false;
                        db.Entry(exc).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (ExceptionLabRayDoctor != null)
                    {
                        var exc = db.Acceptions.Where(a => a.Id == ExceptionLabRayDoctor).FirstOrDefault();
                        exc.AcceptionFlag = false;
                        db.Entry(exc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return new JsonResult { Data = new { result = "تم حفظ العملية بنجاح كود الموافقة  :" + codeRequestDate, ID = hospitalClaim.IdPrimary, msg = "OK" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                    return new JsonResult { Data = new { result = "Invalid Request", msg = "NO" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C_Com_ID"></param>
        /// <param name="Provider_Code"></param>
        /// <param name="Card_ID"></param>
        /// <param name="Services_ID"></param>
        /// <param name="Services"></param>
        /// <returns> if employee take the same doctor specialist at 7 days befor or not </returns>
        public string checkDoctorVisit(int? C_Com_ID, int? Provider_Code, string Card_ID, string Services_ID, string Services)
        {
            string result = "";
            //string resCheck = "";
            string[] checks;
            string[] words = Services.Split('-');
            words = words.Take(words.Count() - 1).ToArray();
            DateTime dateCreat = DateTime.Now.AddDays(-7);
            //List<string> Serveces = new List<string>();
            var Serveces = db.HospitalClaims.Where(h => h.C_COMP_ID == C_Com_ID && h.PROVIDER_CODE == Provider_Code && h.IsDeleted == false
            && h.CARD_ID == Card_ID && h.SERVICE_CODE == Services_ID && h.CreatedDate >= dateCreat).Select(
                s => s.SERVICES).ToList();
            //&& h.PROVIDER_CODE == Provider_Code && h.CARD_ID == Card_ID && h.SERVICE_CODE = Services_ID && h.CreatedDate >= dateCreat
            //DataTable dtServeces = db.RunReader("select SERVICES from patient_requist where C_COMP_ID='" + C_Com_ID + "' and PROVIDER_CODE='" + Provider_Code + "' and CARD_ID='" + Card_ID + "' and SERVICE_CODE='" + Services_ID + "' and CREATED_DATE between sysdate-7 and sysdate ");
            for (int i = 0; i < Serveces.Count; i++)
            {
                checks = Serveces[i].ToString().Split('-');
                checks = checks.Take(checks.Count() - 1).ToArray();
                string[] commonElements = checks.Intersect(words).ToArray();
                result = commonElements.Count().ToString();
                if (commonElements.Count() > 0)
                {
                    result = "sorry";
                    break;
                }
                else
                {
                    result = "ok";
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns> PDF file for hospital claim </returns>
        public ActionResult PrintRequest(string ID)
        {
            Int64 Claim;

            Claim = ID == string.Empty ? 0 : Convert.ToInt64(ID);
            ReportDocument rd = new ReportDocument();

            var model = db.HospitalClaims.Where(x => x.IdPrimary == Claim).FirstOrDefault();
            // for print report for doctor chick
            if (model.SpecialistId != null)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Hospital"), "HospitalClaimReport.rpt"));
                rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");
                rd.SetParameterValue("@idd", model.IdPrimary);
            }

            // fro print report that belongs to rays and labs claim
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Hospital"), "PatientRequest.rpt"));
                rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");
                rd.SetParameterValue("@COD", model.ID);
            }
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();

                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", DateTime.Now.ToString("ddMMyyyy") + "PatientServices.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }




        }



        public JsonResult GetSer_Services(int Services_id, string SubServiceCode, string CardId)
        {
            double Celling_Pert = 0;
            double TotalCompanyPaidCash = 0;
            double remain1 = 0, remain2 = 0, remain3 = 0;
            var CurrentDate = DateTime.Now.Date;
            var emp = db.Comp_Employees.Where(c => c.CARD_ID == CardId && c.INS_START_DATE <= CurrentDate && c.INS_END_DATE >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
            var claim = db.HospitalClaims.Where(h => h.CARD_ID == CardId && h.CONTRACT_NO == emp.CONTRACT_NO && h.IsDeleted == false).ToList();
            double ConsServe = 0;
            double SumAllOfServ = 0;
            string MainService = "";
            if (Services_id == 11204)
                MainService = Services_id.ToString().Substring(0, 3);
            else
                MainService = SubServiceCode.Substring(0, 3);
            Services_id = int.Parse(MainService);
            if (claim.Count > 0)
            {
                ConsServe = (double)claim.Sum(c => c.COMPANY_PAYMENT);
                SumAllOfServ = (double)claim.Where(c => c.SERV_TYP == Services_id).ToList().Sum(c => c.COMPANY_PAYMENT);
                TotalCompanyPaidCash += (double)claim.Where(c => c.SERV_TYP == Services_id && c.SERVICE_CODE == SubServiceCode)
                    .ToList().Sum(c => c.COMPANY_PAYMENT);

            }
            List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == CardId && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();

            double ConsOther = MainAcumlatorList.Sum(c => c.CompanyPayment);
            double SumAll = ConsServe + ConsOther;

            double conSubSer = TotalCompanyPaidCash;
            //double conOtherSubSer = Convert.ToDouble(db.RunReader(" SELECT NVL(SUM(CLAIM_AMOUNT),0) FROM ONLINE_CONS_01 WHERE card_no ='" + CardId + "' AND SERV_CODE=" + Services_id + " and claim_date BETWEEN '" + con + "' AND sysdate").Rows[0][0]);

            double SumAllSubSer = conSubSer + ConsOther;
            //double SumAllSubSer = conSubSer + conOtherSubSer;

            double PertSub = 0, AmtSub = 0, AmtServ = 0, AmtCon = 0;

            var COMP_CUSTOMIZED_D_D_EMP = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO &&
              c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService && c.SERV_CODE == SubServiceCode && c.CARD_ID == CardId).FirstOrDefault();

            if (COMP_CUSTOMIZED_D_D_EMP != null)
            {
                PertSub = COMP_CUSTOMIZED_D_D_EMP.CEILING_PERT != null ? (double)COMP_CUSTOMIZED_D_D_EMP.CEILING_PERT : 100;
                AmtSub = COMP_CUSTOMIZED_D_D_EMP.CEILING_AMT != null ? (double)COMP_CUSTOMIZED_D_D_EMP.CEILING_AMT : -1;

                var COMP_CUSTOMIZED_D_EMP = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService
                 && c.CARD_ID == CardId).FirstOrDefault();
                if (COMP_CUSTOMIZED_D_EMP != null)
                {
                    AmtServ = COMP_CUSTOMIZED_D_EMP.CEILING_AMT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D_EMP.CEILING_AMT) : -1;

                }
                else
                {
                    var COMP_CUSTOMIZED_D = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE
                     && c.D_SERV_CODE == MainService).FirstOrDefault();
                    if (COMP_CUSTOMIZED_D != null)
                    {
                        AmtServ = COMP_CUSTOMIZED_D.CEILING_AMT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D.CEILING_AMT) : -1;
                    }
                }
            }
            else
            {
                var COMP_CUSTOMIZED_D_D = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.D_SERV_CODE == MainService &&
                c.SER_SERV == SubServiceCode && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
                if (COMP_CUSTOMIZED_D_D != null)
                {
                    PertSub = COMP_CUSTOMIZED_D_D.CEILING_PERT != null ? (double)COMP_CUSTOMIZED_D_D.CEILING_PERT : 100;
                    AmtSub = COMP_CUSTOMIZED_D_D.CEILING_AMT != null ? (double)COMP_CUSTOMIZED_D_D.CEILING_AMT : -1;

                    var COMP_CUSTOMIZED_D_EMP = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService
                     && c.CARD_ID == CardId).FirstOrDefault();
                    if (COMP_CUSTOMIZED_D_EMP != null)
                    {
                        AmtServ = COMP_CUSTOMIZED_D_EMP.CEILING_AMT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D_EMP.CEILING_AMT) : -1;

                    }
                    else
                    {
                        var COMP_CUSTOMIZED_D = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE
                         && c.D_SERV_CODE == MainService).FirstOrDefault();
                        if (COMP_CUSTOMIZED_D != null)
                        {
                            AmtServ = COMP_CUSTOMIZED_D.CEILING_AMT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D.CEILING_AMT) : -1;
                        }
                    }
                }
                else
                {
                    var COMP_CUSTOMIZED_D_EMP = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService
                     && c.CARD_ID == CardId).FirstOrDefault();
                    if (COMP_CUSTOMIZED_D_EMP != null)
                    {
                        PertSub = COMP_CUSTOMIZED_D_EMP.CEILING_PERT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D_EMP.CEILING_PERT) : 100;
                        AmtServ = AmtSub = COMP_CUSTOMIZED_D_EMP.CEILING_AMT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D_EMP.CEILING_AMT) : -1;
                    }
                    else
                    {
                        var COMP_CUSTOMIZED_D = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE
                           && c.D_SERV_CODE == MainService).FirstOrDefault();
                        if (COMP_CUSTOMIZED_D != null)
                        {
                            PertSub = COMP_CUSTOMIZED_D.CEILING_PERT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D.CEILING_PERT) : 100;
                            AmtServ = AmtSub = COMP_CUSTOMIZED_D.CEILING_AMT != null ? Convert.ToDouble(COMP_CUSTOMIZED_D.CEILING_AMT) : -1;
                        }
                    }
                }

            }
            AmtCon = (double)db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO
             && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault().MAX_AMOUNT;

            Celling_Pert = PertSub;

            if (AmtCon > SumAll)
            {
                remain1 = AmtCon - SumAll;
                if ((AmtServ != -1 || AmtServ != 0) && AmtServ > SumAllOfServ)
                {
                    remain2 = AmtServ - SumAllOfServ;
                    if ((AmtSub != -1 || AmtSub != 0) && AmtSub > SumAllSubSer)
                    {
                        remain3 = AmtSub - SumAllSubSer;
                        if (remain1 > remain2 && remain1 > remain3)
                        {
                            if (remain2 > remain3)
                            {
                                return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain3 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                            }
                            else
                            {
                                return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain2 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                            }
                        }
                        else
                        {
                            return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain1 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                        }

                    }
                    else
                    {
                        if (remain1 > remain2)
                        {
                            return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain2 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                        else
                        {
                            return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain1 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                        }
                    }

                }
                else
                {
                    if ((AmtSub != -1 || AmtSub != 0) && AmtSub > SumAllSubSer)
                    {
                        remain3 = AmtSub - SumAllSubSer;
                        if (remain1 > remain3)
                        {
                            return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain3 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                        }
                        else
                        {
                            return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain1 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                        }
                    }
                    else
                    {
                        return new JsonResult { Data = new { celing = Celling_Pert, max_am = remain1 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
            }
            else
            {
                return new JsonResult { Data = new { celing = Celling_Pert, max_am = 0 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }


        }


        #endregion

    }
}