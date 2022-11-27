using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel.CustomerService;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Globalization;
using System.Threading;

namespace DMS_Authontication1.Controllers.CustomerService
{
    //[Authorize(Roles = "Admin,AfterSale")]
    public class CustomerServiceController : Controller
    {
        #region Fields

        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationDbContext db2 = new ApplicationDbContext();
        #endregion

        #region Actions

        // GET: CustomerService
        public ActionResult Index()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "Name", "Name");
            ViewBag.company = companylist;
            if (User.IsInRole("Admin"))
            {
                var usersname = db2.Users.Where(u => u.Type == "AfterSale").Select(c => new
                {
                    user = c.UserName,
                    Name = c.FName + " " + c.LName

                }).ToList();
                SelectList userslist = new SelectList(usersname, "user", "Name");
                ViewBag.users = userslist;
            }

            return View();
        }
        public ActionResult Map()
        {
            return View();
        }

        // GET: CustomerService/Create
        public ActionResult Create()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "Name", "Name");
            ViewBag.company = companylist;
            return View();
        }


        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AfterSale afterSale = db.AfterSales.Find(id);
            if (afterSale == null)
            {
                return HttpNotFound();
            }
            return View(afterSale);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AfterSale afterSale = db.AfterSales.Find(id);
            if (afterSale == null)
            {
                return HttpNotFound();
            }
            if (afterSale.IsNew == false)
            {
                var companyname = db.Contract_Comp.Select(c => new
                {
                    Name = c.C_ANAME + " || " + c.C_COMP_ID

                }).ToList();
                SelectList companylist = new SelectList(companyname, "Name", "Name");
                ViewBag.company = companylist;
                return View(afterSale);
            }
            return View(afterSale);
        }

        // GET: CustomerService/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AfterSale afterSale = db.AfterSales.Find(id);
            if (afterSale == null)
            {
                return HttpNotFound();
            }
            return View(afterSale);
        }




        // POST: AfterSales1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            AfterSale afterSale = db.AfterSales.Find(id);
            afterSale.IsDeleted = true;
            afterSale.DeletedBy = User.Identity.GetUserName();
            afterSale.DeletedDate = DateTime.Now;
            //db.AfterSales.Remove(afterSale);
            db.Entry(afterSale).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        DBApproval dbData = new DBApproval();
        DBData dbData2 = new DBData();
        string getncardapproval(string crd)
        {
            string ncrd = "";

            System.Data.DataTable dtoldcrdaprov = new System.Data.DataTable();
            dtoldcrdaprov = dbData.RunReader(@"SELECT CLOSE_EMP_DATA.CARD_ID FROM DMS_TEST.CLOSE_EMP_DATA WHERE (CLOSE_EMP_DATA.TRANS_TYP = 'D' OR CLOSE_EMP_DATA.TRANS_TYP = 'L') AND CLOSE_EMP_DATA.N_CARD = '" + crd + "'");

            if (dtoldcrdaprov.Rows.Count > 0 && dtoldcrdaprov.Rows[0][0].ToString() != string.Empty)
                ncrd = dtoldcrdaprov.Rows[0][0].ToString();
            else
                ncrd = "";

            return ncrd;
        }
        string getColorCardApproval(string crd, string crdold)
        {
            string crdcolr = "";

            System.Data.DataTable dtColorCard = new System.Data.DataTable();
            dtColorCard = dbData.RunReader(@"SELECT CARD_ID, COLOR_NAME, PRINT_DATE FROM CARD_PRINT_HISTORY 
                                         WHERE (CARD_ID = '" + crd + "' OR CARD_ID = '" + crdold + "') ORDER BY PRINT_DATE desc");

            if (dtColorCard.Rows.Count > 0)
                crdcolr = dtColorCard.Rows[0][1].ToString();

            return crdcolr;
        }
        string getMaxAmountForCard(string cmp, string contr, string cls, string crd)
        {
            string maxAmount = "";
            DataTable dtmxamt = new DataTable();

            dtmxamt = dbData.RunReader("SELECT MAX_AMOUNT FROM DMS_TEST.COMP_CONTRACT_CLASS_EMP WHERE C_COMP_ID = '" + cmp + "' and CONTRACT_NO = '" + contr + "' and CLASS_CODE = '" + cls + "' AND CARD_ID = '" + crd + "'");

            if (dtmxamt.Rows.Count > 0 && dtmxamt.Rows[0][0].ToString() != string.Empty)
                maxAmount = dtmxamt.Rows[0][0].ToString();
            else
            {
                dtmxamt = dbData.RunReader("SELECT MAX_AMOUNT FROM DMS_TEST.COMP_CONTRACT_CLASS WHERE C_COMP_ID = '" + cmp + "' and CONTRACT_NO = '" + contr + "' and CLASS_CODE = '" + cls + "'");

                maxAmount = dtmxamt.Rows[0][0].ToString();
            }

            return maxAmount;
        }
        List<DataRow> convertDataTableToList(DataTable dt)
        {
            List<DataRow> list = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr);

            }
            return list;
        }
        public JsonResult getData(string CardId)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            DataTable dtcrd = new DataTable();

            dtcrd = dbData.RunReader("select  to_char(BIRTH_DATE,'DD-MM-YYYY'),C_COMP_ID,CLASS_CODE,NVL(to_char(SPECIFIC_DATE,'DD-MM-YYYY'),to_char(INS_START_DATE,'DD-MM-YYYY')),to_char(INS_END_DATE,'DD-MM-YYYY'),TERMINATE_FLAG ,EMP_ANAME_ST ,EMP_ANAME_SC,EMP_ANAME_TH ,EMP_ENAME_ST ,EMP_ENAME_SC,EMP_ENAME_TH, to_char(INS_START_DATE,'DD-MM-YYYY'), to_char(TERMINATE_DATE,'DD-MM-YYYY') ,CONTRACT_NO, TEL1, TEL2, EMP_ID  from dms_test.COMP_EMPLOYEES where CARD_ID='" + CardId + "' order by ins_start_date DESC");

            string flg = "", nots = "";
            DataTable phon = new DataTable();        
            List<CustomerServiceViewModel> dtDetails = new List<CustomerServiceViewModel>();
            List<CustomerServicePhoneViewModel> phonlst = new List<CustomerServicePhoneViewModel>();
            //getMaxAmountForCard(string cmp, string contr, string cls, string crd)
            if (dtcrd.Rows.Count != 0)
            {
                string nopay = "", nover = "", oldcrd = "";

                if (dtcrd.Rows.Count > 0 && dtcrd.Rows[0][5].ToString() == "Y" && Convert.ToDateTime(dtcrd.Rows[0][13]).Date < DateTime.Now.Date)
                {
                    flg = "1";

                    nots = "تم إغلاق هذا الكارت بتاريخ " + dtcrd.Rows[0][13].ToString() + "\n";

                    DataTable dtdelcrd = dbData.RunReader(@"SELECT CARD_ID, to_char(WITHDRAW_CARD_DATE, 'DD-MM-YYYY') FROM DMS_TEST.CLOSE_EMP_DATA WHERE WITHDRAW_CARD_DATE IS NOT NULL AND CARD_ID = '" + CardId + "'");

                    if (dtdelcrd.Rows.Count > 0)
                        nots = nots + " وتم استلام الكارت بتاريخ " + dtdelcrd.Rows[0][1].ToString();
                    else
                        nots = nots + " ولم يتم إستلام الكارت بعد ";
                }
                else if (dtcrd.Rows.Count > 0 && Convert.ToDateTime(dtcrd.Rows[0][4]).Date < DateTime.Now.Date)
                {
                    flg = "2";
                    nots = " أنتهى التعاقد مع هذا الموظف بتاريخ " + dtcrd.Rows[0][4].ToString();
                }
                else
                {
                    flg = "0";
                    phon = dbData.RunReader(@"select distinct PHONE From EMPPHONE where CARD_ID ='" + CardId + "'");


                    if (phon.Rows.Count == 0)
                        nots = "لا يوجد رقم هاتف مسجل لهذا الكارت من فضلك ادخل رقم الهاتف الاساسي";
                    else
                        foreach (DataRow row in phon.Rows)
                            phonlst.Add(new CustomerServicePhoneViewModel
                            {
                            phoneNumber = row["PHONE"].ToString()
                            });


                    DataTable dtpo = dbData.RunReader(@"SELECT decode(NVL(NO_PAY,0), 1, 'Yes', 'No') no_pay, decode(NO_OVER, 1, 'Yes', 'No') no_over FROM MED_CARD_NEW WHERE CARD_NO = '" + CardId + "'");

                    if (dtpo.Rows.Count > 0)
                    {
                        nopay = dtpo.Rows[0][0].ToString();
                        nover = dtpo.Rows[0][1].ToString();
                    }

                    oldcrd = getncardapproval(CardId);

                    string oldcrd2 = oldcrd != "" ? oldcrd : CardId;

                    dtDetails.Add(new CustomerServiceViewModel
                    {
                        CardID = CardId,
                        EmployeeName = dtcrd.Rows[0][6].ToString() + " " + dtcrd.Rows[0][7].ToString() + " " + dtcrd.Rows[0][8].ToString(),
                        BirthDate = dtcrd.Rows[0][0].ToString(),
                        Age = (DateTime.Now.Year - Convert.ToDateTime(dtcrd.Rows[0][0]).Year).ToString(),
                        StartDate = dtcrd.Rows[0][12].ToString(),
                        EndDate = dtcrd.Rows[0][4].ToString(),
                        MaxAmount = getMaxAmountForCard(dtcrd.Rows[0][1].ToString(), dtcrd.Rows[0][14].ToString(), dtcrd.Rows[0][2].ToString(), CardId),
                        ClassName = dbData.RunReader("select CLASS_ENAME from V_CLASS_NAME where CLASS_CODE ='" + dtcrd.Rows[0][2].ToString() + "'").Rows[0][0].ToString(),
                        HospitalDegree = dbData.RunReader(@"SELECT HOSPITAL_DEGREE, BS_ANAME  FROM dms_test.COMP_CONTRACT_CLASS, dms_test.BASIC_DATA WHERE SOURCE_MOD = 'ACCDEG' AND BS_CODE = HOSPITAL_DEGREE AND C_COMP_ID= '" + dtcrd.Rows[0][1].ToString() + "' AND CONTRACT_NO = '" + dtcrd.Rows[0][14].ToString() + "' AND CLASS_CODE = '" + dtcrd.Rows[0][2].ToString() + "'").Rows[0][1].ToString(),
                        MedicalNetwork = dbData.RunReader(@"SELECT COVER_RELATION, BS_ANAME  FROM dms_test.COMP_CONTRACT_CLASS, dms_test.BASIC_DATA WHERE SOURCE_MOD = 'PRDEG' AND BS_CODE = COVER_RELATION AND C_COMP_ID= '" + dtcrd.Rows[0][1].ToString() + "' AND CONTRACT_NO = '" + dtcrd.Rows[0][14].ToString() + "' AND CLASS_CODE = '" + dtcrd.Rows[0][2].ToString() + "'").Rows[0][1].ToString(),
                        ExceptionPayment = nopay,
                        ExceptionOver = nover,
                        NationalId = dtcrd.Rows[0]["EMP_ID"].ToString(),
                        Mobile1 = dtcrd.Rows[0]["TEL1"].ToString(),
                        Mobile2 = dtcrd.Rows[0]["TEL2"].ToString(),
                        CardColor = getColorCardApproval(CardId, oldcrd),
                        OldCard = oldcrd,
                        // Flag = flg,
                        NotesCloseCard = nots
                    });
                }

               // var phonlst = phon.AsEnumerable().ToList();
                //var phonlst = convertDataTableToList(phon);
                return new JsonResult { Data = new { flg, dtDetails, nots, phonlst }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {                
                flg = "3";
                nots = "لا توجد بيانات للكارت الرجاء التأكد من رقم الكارت المدخل وحاول ثانية";
                return new JsonResult { Data = new { flg, nots }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }
        public JsonResult getConsumption(string CardId, string dat1, string dat2, string oldcardd, string maxamt)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            DataTable dtcrd = new DataTable();

            string oldcrd = oldcardd != string.Empty ? oldcardd : CardId;

            /*
           
            ConsumptionUseOnlineMedMainCustmer.Text = dton1.Rows[0][0].ToString();
            ConsumptionUseOnlineOtherMainCustmer.Text = dton2.Rows[0][0].ToString();


            ConsumptionUseOnlineMainCustmer.Text = (double.Parse(dton1.Rows[0][0].ToString()) + double.Parse(dton2.Rows[0][0].ToString())).ToString();



            ConsumptionUseMainCustmer.Text = (Convert.ToDouble(ConsumptionUseIRSMainCustmer.Text) + Convert.ToDouble(ConsumptionUseOnlineMainCustmer.Text)).ToString();

            ConsumptionAvailableMainCustmer.Text = (Convert.ToDouble(LimitAmountMainCustmer.Text) - Convert.ToDouble(ConsumptionUseOnlineMainCustmer.Text)).ToString();

            double tm = Convert.ToDouble(ConsumptionUseMainCustmer.Text) / Convert.ToDouble(LimitAmountMainCustmer.Text) * 100;
            ConsumptionPercentMainCustmer.Text = Math.Round(tm, 2).ToString() + " %";
*/



            //dtcrd = dbData.RunReader("select  to_char(BIRTH_DATE,'DD-MM-YYYY'),C_COMP_ID,CLASS_CODE,NVL(to_char(SPECIFIC_DATE,'DD-MM-YYYY'),to_char(INS_START_DATE,'DD-MM-YYYY')),to_char(INS_END_DATE,'DD-MM-YYYY'),TERMINATE_FLAG ,EMP_ANAME_ST ,EMP_ANAME_SC,EMP_ANAME_TH ,EMP_ENAME_ST ,EMP_ENAME_SC,EMP_ENAME_TH, to_char(INS_START_DATE,'DD-MM-YYYY'), to_char(TERMINATE_DATE,'DD-MM-YYYY') ,CONTRACT_NO, TEL1, TEL2, EMP_ID  from dms_test.COMP_EMPLOYEES where CARD_ID='" + CardId + "' order by ins_start_date DESC");

            List<CustomerServiceViewModel> dtConsumption = new List<CustomerServiceViewModel>();
            
            string consmMedClaim = dbData2.getConsumptionMedClaim(CardId, Convert.ToDateTime(dat1.ToString()), Convert.ToDateTime(dat2.ToString()), oldcrd).Rows[0][0].ToString();
            string consmMedOnline = dbData2.getConsumptionMedOnline(CardId, Convert.ToDateTime(dat1), Convert.ToDateTime(dat2), oldcrd).Rows[0][0].ToString();
            string consmOther = dbData2.getConsumptionOther(CardId, Convert.ToDateTime(dat1.ToString()), Convert.ToDateTime(dat2.ToString()), oldcrd).Rows[0][0].ToString();
            string consmAll = (double.Parse(consmMedClaim) + double.Parse(consmMedOnline) + double.Parse(consmOther)).ToString();
            string remain = (double.Parse(maxamt) - double.Parse(consmAll)).ToString();
            double tm = Convert.ToDouble(remain) / Convert.ToDouble(maxamt) * 100;
            string perct = Math.Round(tm, 2).ToString() + " %";
            string consmApproval = dbData2.getConsumptionApproval(CardId, Convert.ToDateTime(dat1.ToString()), Convert.ToDateTime(dat2.ToString()), oldcrd).Rows[0][0].ToString();

            dtConsumption.Add(new CustomerServiceViewModel
            {
                MedicationClaims = consmMedClaim,
                MedicationClaimsUnderReview = consmMedOnline,
                OtherConsumption = consmOther,                
                AllConsumption = consmAll,
                Remaining = remain,
                Percent = perct,
                ApprovalConsumption = consmApproval
                
            }); 
            return new JsonResult { Data = dtConsumption, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult getAprovalData(string CardId, string dat1, string dat2, string oldcardd, int flg)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            string oldcrd = oldcardd != string.Empty ? oldcardd : CardId;
            DataTable dt = new DataTable();
            if(flg == 1)
                dt = dbData2.getApproval(CardId, Convert.ToDateTime(dat1.ToString()), Convert.ToDateTime(dat2.ToString()), oldcrd);
            else
                dt = dbData2.getApproval(CardId, Convert.ToDateTime("01-01-2012"), Convert.ToDateTime(dat2.ToString()), oldcrd);

            string cout = "", totl = "";
                     
            List<ApprovalCustomerServiceViewModel> approval = new List<ApprovalCustomerServiceViewModel>();

            if (dt.Rows.Count != 0)
            {
                cout = dt.Rows.Count.ToString();
                totl = dt.Compute("SUM(APPROV_AMOUNT)", "").ToString();

                foreach (DataRow row in dt.Rows)
                {
                    approval.Add(new ApprovalCustomerServiceViewModel
                    {
                        ApprovalNo = row["APPROV_NO"].ToString(),
                        ApprovalType = row["SERVECE_TYP"].ToString(),
                        Reply = row["REPLY"].ToString(),
                        ApprovalAmount = row["APPROV_AMOUNT"].ToString(),
                        MedicalReply = row["MEDICAL_REPLAY"].ToString(),
                        CreatedBy = row["CREATED_BY"].ToString(),
                        CreatedDate = row["CREATED_DATE"].ToString(),
                    });
                }
                return new JsonResult { Data = new { approval, cout, totl }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
                return new JsonResult { Data = new { approval, cout, totl }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        /* public List<Comp_Employees> CardList(string CardId)
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
         }*/

        public JsonResult GetRelatedCards(string CardId)
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
                               && e.CONTRACT_NO == maxContract)
                    .Select(c => new
                    {
                        CardIDValue = c.CARD_ID,
                        CardIdString = c.CARD_ID
                    }).ToList();
                var result = new { Success = "Yes", subCards = subCards };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                var result = new { Success = "No" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        public JsonResult GetDataByPhone(string Phonee)
        {
            DataTable dtcrd = new DataTable();

            // dtcrd = dbData.RunReader("select  to_char(BIRTH_DATE,'DD-MM-YYYY'),C_COMP_ID,CLASS_CODE,NVL(to_char(SPECIFIC_DATE,'DD-MM-YYYY'),to_char(INS_START_DATE,'DD-MM-YYYY')),to_char(INS_END_DATE,'DD-MM-YYYY'),TERMINATE_FLAG ,EMP_ANAME_ST ,EMP_ANAME_SC,EMP_ANAME_TH ,EMP_ENAME_ST ,EMP_ENAME_SC,EMP_ENAME_TH, to_char(INS_START_DATE,'DD-MM-YYYY'), to_char(TERMINATE_DATE,'DD-MM-YYYY') ,CONTRACT_NO, TEL1, TEL2, EMP_ID  from dms_test.COMP_EMPLOYEES where CARD_ID='" + CardId + "' order by ins_start_date DESC");
            dtcrd = dbData.RunReader(@"select distinct CARD_ID From EMPPHONE where PHONE ='" + Phonee + "'");

            string flg = "", nots = "";

            List<string> cards = new List<string>();

            if (dtcrd.Rows.Count != 0)
            {
                foreach (DataRow row in dtcrd.Rows)
                    cards.Add(row[0].ToString());
                //var phonlst = convertDataTableToList(phon);
                return new JsonResult { Data = new { flg, cards, nots }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        else
        {
                flg = "1";
                nots = "لا توجد بيانات لهذا لرقم الرجاء التأكد من الرقم المدخل وحاول ثانية";
                return new JsonResult { Data = new { flg, nots }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
}

        public ActionResult PrintApproval(string ApprovalCode)
        {

            DataTable dts = dbData.RunReader("SELECT CARD_NO,DIAG_NAME,SERVECE_TYP,CREATED_BY FROM MEDICAL_APPROVALS WHERE CODE='" + ApprovalCode + "'");
            string cardNum = dts.Rows[0][0].ToString();
            DataTable dtsrev = dbData.RunReader(@"select APPROVAL_SUB_SERV.S_SERV_NAME, CASE WHEN (S_SERV_NAME = 'الاقامة'OR S_SERV_CODE LIKE '114%') 
                                                                        THEN CONCAT(APPROVAL_SUB_SERV.DETAILS, APPROVAL_SUB_SERV.DISCRIPTION) ELSE APPROVAL_SUB_SERV.DETAILS END
                                                             FROM  APPROVAL_SUB_SERV WHERE APPROVAL_SUB_SERV.CODE = '" + ApprovalCode + "'");
            DataTable dtdiag = dbData.RunReader(@"select MEDICAL_APPROVALS.APROVAL_IMAG, APPROVAL_DIAG.DIAG_NAME, MEDICAL_APPROVALS.PATH FROM      MEDICAL_APPROVALS, APPROVAL_DIAG
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
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="CompName"></param>
        /// <param name="username"></param>
        /// <returns>Report as PDF</returns>
        public ActionResult PrintVisits(string from, string to
            , string CompName, string username)
        {
            string CompNa,usr;
            DateTime DateFrom, DateTo;

            CompNa = CompName == string.Empty ? "" : CompName;


            DateFrom = from == string.Empty ?  new DateTime(2020,06,01) : (Convert.ToDateTime(from)).Date;
            DateTo = to == string.Empty ? DateTime.Now.AddDays(1) : (Convert.ToDateTime(to)).Date;
            if(User.IsInRole("Admin"))
            {
                if(string.IsNullOrEmpty(username))
                {
                    usr = "";
                }
                else
                {
                    usr = username;

                }
            }
            else
            {
                usr = User.Identity.GetUserName();

            }
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/AfterSale"), "AfterSales.rpt"));

            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@from", DateFrom);
            rd.SetParameterValue("@to", DateTo);
            rd.SetParameterValue("@cmp", CompNa);
            rd.SetParameterValue("@usr", usr);

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
                return File(stream, "application/pdf", DateFrom.ToString("ddMMyyyy") + "AfterSale.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="CompName"></param>
        /// <param name="username"></param>
        /// <returns>Report as EXCEL</returns>
     
        #endregion

        #region Help Json
        public JsonResult GetBranshs(int id)
        {
            var listBranshs = db.Company_Cost_Center.Where(m => m.C_COMP_ID == id).Select(
                c => new
                {
                    COST_CODE = c.COST_CODE,
                    A_NAME = c.A_NAME
                }).ToList();
            //SelectList Branshslist = new SelectList(listBranshs, "COST_CODE", "A_NAME");


            return new JsonResult { Data = new { providerslist = listBranshs }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult SaveVisit(string CompName, string BranchName, string PersonName, string VisitReasonList, bool IsNew
                          , DateTime VisitDate, string Region, string PhoneNumber, string FeedBackText, string Note,
                            string DateOfStartMeeting, string DateOfEndtMeeting, string Duration)
        {
            bool feed = false;
            DateTime? feedDate = null;
            if (!string.IsNullOrEmpty(FeedBackText))
            {
                feed = true;
                feedDate = DateTime.Now;
            }
            var afterSale = new AfterSale
            {
                CompanyName = CompName,
                BranchName = BranchName,
                PersonName = PersonName,
                VisitDate = VisitDate,
                VisitReason = VisitReasonList,
                Region = Region,
                Phone = PhoneNumber,
                FeedBackDate = feedDate,
                FeedBackText = FeedBackText,
                CreatedBy = User.Identity.GetUserName(),
                CreatedDate = DateTime.Now,
                IsDeleted = false,
                HasFeedBack = feed,
                Note = Note,
                IsNew = IsNew,
                MeetingTime = Duration,
                StartMeeting = DateOfStartMeeting,
                EndMeeting = DateOfEndtMeeting

            };
            db.AfterSales.Add(afterSale);
            var x = db.SaveChanges();
            if (x == 1)
            {
                return new JsonResult { Data = new { result = "Visit Add Successfully ", msg = "OK" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            else
            {
                return new JsonResult { Data = new { result = "Invalid Request", msg = "NO" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public JsonResult EditVisit(long Id, string CompName, string BranchName, string PersonName, string VisitReasonList, bool IsNew
                          , DateTime VisitDate, string Region, string PhoneNumber, string FeedBackText, string Note,
                           string DateOfStartMeeting, string DateOfEndtMeeting, string Duration)
        {
            AfterSale model = db.AfterSales.Find(Id);
            if (model == null)
            {
                return new JsonResult { Data = new { result = "Invalid Request", msg = "NO" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            bool feed = false;
            DateTime? feedDate = null;
            if (!string.IsNullOrEmpty(FeedBackText) && string.IsNullOrEmpty(model.FeedBackText))
            {
                feed = true;
                feedDate = DateTime.Now;
            }
            else if (!string.IsNullOrEmpty(FeedBackText) && !string.IsNullOrEmpty(model.FeedBackText))
            {
                feed = true;
                feedDate = model.FeedBackDate;
            }
            model.CompanyName = CompName;
            model.BranchName = BranchName;
            model.PersonName = PersonName;
            model.VisitDate = VisitDate;
            model.VisitReason = VisitReasonList;
            model.Region = Region;
            model.Phone = PhoneNumber;
            model.FeedBackDate = feedDate;
            model.FeedBackText = FeedBackText;
            model.IsDeleted = false;
            model.HasFeedBack = feed;
            model.Note = Note;
            model.IsNew = model.IsNew;
            model.UpdatedBy = User.Identity.GetUserName();
            model.UpdatedDate = DateTime.Now;
            model.MeetingTime = Duration;
            model.StartMeeting = DateOfStartMeeting;
            model.EndMeeting = DateOfEndtMeeting;
            db.Entry(model).State = EntityState.Modified;
            var x = db.SaveChanges();

            return new JsonResult { Data = new { result = "Visit Update Successfully ", msg = "OK" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult AfterSaleList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch, string From = "", string To = "", string CompName = "", string userName = "")
        {
            //var VisitList = db.AfterSales.Where(a => a.CreatedBy == user && a.IsDeleted == false).ToList();

            if (User.IsInRole("Admin"))
            {

                // if admin search with( date from and to only) or with (company name or username)
                if (From != "" & To != "")
                {
                    DateTime dateFrom = Convert.ToDateTime(From);
                    DateTime dateTo = Convert.ToDateTime(To);
                    var result1 = new
                    {
                        sEcho = sEcho,
                        aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains( CompName ): true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).AsEnumerable()
                       .Select(l =>
                       new
                       {
                           Id = l.Id,
                           CompanyName = l.CompanyName,
                           BranchName = l.BranchName,
                           VisitReason = l.VisitReason,
                           VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                           PersonName = l.PersonName
                       })
                       .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.AfterSales.Where(r => r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) >= dateTo)).Count(),
                        iTotalDisplayRecords = db.AfterSales.Where(r => r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) >= dateTo)).Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }


                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)
                       ).AsEnumerable()
                       .Select(l =>
                    new
                    {
                        Id = l.Id,
                        CompanyName = l.CompanyName,
                        BranchName = l.BranchName,
                        VisitReason = l.VisitReason,
                        VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                        PersonName = l.PersonName
                    })
                    .Skip(iDisplayStart).Take(iDisplayLength).ToList(),


                    iTotalRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).Count(),
                    iTotalDisplayRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && (userName != "" ? r.CreatedBy == userName : true)
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)
                       ).Count()

                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                var user = User.Identity.GetUserName();


                // if user search with( date from and to only) or with company name 
                if (From != "" & To != "")
                {
                    DateTime dateFrom = Convert.ToDateTime(From);
                    DateTime dateTo = Convert.ToDateTime(To);
                    var result1 = new
                    {
                        sEcho = sEcho,
                        aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).AsEnumerable()
                       .Select(l =>
                       new
                       {
                           Id = l.Id,
                           CompanyName = l.CompanyName,
                           BranchName = l.BranchName,
                           VisitReason = l.VisitReason,
                           VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                           PersonName = l.PersonName
                       })
                       .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                        iTotalRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).Count(),
                        iTotalDisplayRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true) &&
                       (DbFunctions.TruncateTime(r.VisitDate) >= dateFrom && DbFunctions.TruncateTime(r.VisitDate) <= dateTo)).Count()
                    };
                    //return Ok(result);
                    return new JsonResult { Data = result1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }



                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                       .Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).AsEnumerable()
                    .Select(l =>
                    new
                    {
                        Id = l.Id,
                        CompanyName = l.CompanyName,
                        BranchName = l.BranchName,
                        VisitReason = l.VisitReason,
                        VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                        PersonName = l.PersonName
                    })
                    .Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).Count(),
                    iTotalDisplayRecords = db.AfterSales.Where(r => (sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                       || r.PersonName.Contains(sSearch)) : true) && r.IsDeleted == false && r.CreatedBy == user
                       && (CompName != "" ? r.CompanyName.Contains(CompName) : true)).Count()
                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };




                //var result = new
                //{
                //    sEcho = sEcho,
                //    aaData = db.AfterSales.OrderByDescending(m => m.CreatedDate)
                //    .Where(r => r.CreatedBy == user && r.IsDeleted == false && sSearch != "" ? (r.CompanyName.Contains(sSearch) || r.BranchName.Contains(sSearch)
                //   || r.PersonName.Contains(sSearch)) : true).AsEnumerable().Select(l =>
                //     new
                //     {
                //         Id = l.Id,
                //         CompanyName = l.CompanyName,
                //         BranchName = l.BranchName,
                //         VisitReason = l.VisitReason,
                //         VisitDate = string.Format("{0:ddd, MMM d, yyyy tt}", l.VisitDate),
                //         PersonName = l.PersonName
                //     }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                //    iTotalRecords = db.AfterSales.Count(),
                //    iTotalDisplayRecords = db.AfterSales.Count()
                //};
                ////return Ok(result);
                //return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }


        }

        #endregion

    }
}