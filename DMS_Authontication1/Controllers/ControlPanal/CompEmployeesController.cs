using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;

namespace DMS_Authontication1.Controllers.ControlPanal
{
    [Authorize(Roles = "Admin")]
    public class CompEmployeesController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();

        // GET: CompEmployees
        public ActionResult Index()
        {
            //return View(db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).ToList());
            return View();
        }
        // GET: CompEmployees
        public ActionResult IndexSms()
        {
            //return View(db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).ToList());
            return View();
        }
        public JsonResult EmployeesListSms(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch, string CardId = "", string Company = "")
        {
            int outresut = 0;
            int.TryParse(Company, out outresut);
            var result = new
            {
                sEcho = sEcho,
                aaData = (db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now && x.TERMINATE_FLAG != "Y")//.OrderByDescending(x => x.CONTRACT_NO)
                .Where(r => sSearch != "" ? (r.EMP_ANAME.Contains(sSearch) || r.EMP_ENAME.Contains(sSearch) || r.CARD_ID.Contains(sSearch)) : true
                && CardId != "" ? r.CARD_ID.Contains(CardId) : true && Company != "" ? r.C_COMP_ID == outresut : true)
                .Include(x => x.EmployeesSMSCodes))
                .Join(db.Med_Card, d => d.CARD_ID, m => m.CARD_NO, (d, m) => new { d, m })
                        .Where(l => l.m.CARD_NO == l.d.CARD_ID && l.m.LOOK_01 == 0)
                .Select(l => new //Comp_Employees
                {
                    Id = l.d.Id,
                    CARD_ID = l.d.CARD_ID,
                    EMP_ENAME = l.d.EMP_ENAME,
                    CONTRACT_NO = l.d.CONTRACT_NO,
                    EMP_ID = l.d.EMP_ID,
                    TEL1 = l.d.TEL1,
                    Code = l.d.EmployeesSMSCodes.Where(x => x.EmpId == l.d.Id && x.IsActive).FirstOrDefault().SMSCode ?? ""
                }).OrderBy(x => x.Id).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = (db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now &&x.TERMINATE_FLAG != "Y")//.OrderByDescending(x => x.CONTRACT_NO)
                .Where(r => sSearch != "" ? (r.EMP_ANAME.Contains(sSearch) || r.EMP_ENAME.Contains(sSearch) || r.CARD_ID.Contains(sSearch)) : true
                && CardId != "" ? r.CARD_ID.Contains(CardId) : true && Company != "" ? r.C_COMP_ID == outresut : true))
                .Join(db.Med_Card, d => d.CARD_ID, m => m.CARD_NO, (d, m) => new { d, m })
                        .Where(l => l.m.CARD_NO == l.d.CARD_ID && l.m.LOOK_01 == 0).Count(),
                iTotalDisplayRecords = (db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now &&x.TERMINATE_FLAG != "Y")//.OrderByDescending(x => x.CONTRACT_NO)
                .Where(r => sSearch != "" ? (r.EMP_ANAME.Contains(sSearch) || r.EMP_ENAME.Contains(sSearch) || r.CARD_ID.Contains(sSearch)) : true
                && CardId != "" ? r.CARD_ID.Contains(CardId) : true && Company != "" ? r.C_COMP_ID == outresut : true))
                .Join(db.Med_Card, d => d.CARD_ID, m => m.CARD_NO, (d, m) => new { d, m })
                        .Where(l => l.m.CARD_NO == l.d.CARD_ID && l.m.LOOK_01 == 0).Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }

        public JsonResult EmployeesList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            if (sSearch != "")
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now)//.OrderByDescending(x => x.CONTRACT_NO)
                    .Where(r => r.EMP_ANAME.Contains(sSearch) || r.EMP_ENAME.Contains(sSearch) || r.CARD_ID.Contains(sSearch))
                    .Select(l => new //Comp_Employees
                    {
                        Id = l.Id,
                        CARD_ID = l.CARD_ID,
                        EMP_ENAME = l.EMP_ENAME,
                        CONTRACT_NO = l.CONTRACT_NO,
                        EMP_ID = l.EMP_ID,
                        TEL1 = l.TEL1,
                        TEL2 = l.TEL2
                    }).OrderBy(x => x.Id).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now)
                    .Where(r => r.EMP_ANAME.Contains(sSearch) || r.EMP_ENAME.Contains(sSearch) || r.CARD_ID.Contains(sSearch)).Count(),
                    iTotalDisplayRecords = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now)
                    .Where(r => r.EMP_ANAME.Contains(sSearch) || r.EMP_ENAME.Contains(sSearch) || r.CARD_ID.Contains(sSearch)).Count()
                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = new
                {
                    sEcho = sEcho,
                    aaData = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now)
                    .Select(l => new //Comp_Employees
                    {
                        Id = l.Id,
                        CARD_ID = l.CARD_ID,
                        EMP_ENAME = l.EMP_ENAME,
                        CONTRACT_NO = l.CONTRACT_NO,
                        EMP_ID = l.EMP_ID,
                        TEL1 = l.TEL1,
                        TEL2 = l.TEL2
                    }).OrderBy(x => x.Id).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                    iTotalRecords = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).Count(),
                    iTotalDisplayRecords = db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).Count()
                };
                //return Ok(result);
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }


        }


        [HttpPost]
        public JsonResult SMSCode(int id)
        {


            Comp_Employees employee = db.Comp_Employees.Where(c => c.Id == id).FirstOrDefault();
            if (employee.TEL1 != "" && employee.TEL1 != null && employee.TEL1.Length == 11)
            {
                Random generator = new Random();
                String SMSCode = generator.Next(0, 1000000).ToString("D6");
                EmployeesSMSCode EmpSMSCode = db.EmployeesSMSCodes.Where(c => c.EmpId == id && c.SMSCode == SMSCode && c.IsActive).FirstOrDefault();
                if (EmpSMSCode == null)//insert
                {
                    EmployeesSMSCode NewEmpSMSCode = new EmployeesSMSCode();
                    NewEmpSMSCode.EmpId = id;
                    NewEmpSMSCode.SMSCode = SMSCode;
                    NewEmpSMSCode.IsActive = true;
                    NewEmpSMSCode.CreatedBy = User.Identity.Name;
                    NewEmpSMSCode.CreatedDate = DateTime.Now;
                    NewEmpSMSCode.LastSentDate = DateTime.Now;
                    db.EmployeesSMSCodes.Add(NewEmpSMSCode);

                }
                else //update code
                {
                    EmpSMSCode.SMSCode = SMSCode;
                    EmpSMSCode.IsActive = true;
                    EmpSMSCode.UpdatedBy = User.Identity.Name;
                    EmpSMSCode.UpdatedDate = DateTime.Now;
                    EmpSMSCode.LastSentDate = DateTime.Now;
                    db.Entry(EmpSMSCode).State = EntityState.Modified;

                }
                db.SaveChanges();
                try
                {
                    PostSMSData("your DMS verification code to dispense chronic medicines is " + SMSCode, employee.TEL1);

                }
                catch (Exception)
                {

                    throw;
                }
                return Json(new { ok = true, returndata = SMSCode, message = "ok" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { ok = false, returndata = "Invalid Telephone,Please Update Telephone 1 Number", message = "ok" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SMSCodeForAll(int CopmId)
        {
            try
            {


                var result = (db.Comp_Employees.Where(x => x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now && x.C_COMP_ID == CopmId && x.TERMINATE_FLAG == "Y")
                    .Include(x => x.EmployeesSMSCodes))
                    .Join(db.Med_Card, d => d.CARD_ID, m => m.CARD_NO, (d, m) => new { d, m })
                            .Where(l => l.m.CARD_NO == l.d.CARD_ID && l.m.LOOK_01 == 0)
                    .Select(l => new //Comp_Employees
                    {
                        Id = l.d.Id,
                        TEL1 = l.d.TEL1,
                        Code = l.d.EmployeesSMSCodes.Where(c => c.IsActive == true)
                    }).OrderBy(x => x.Id).ToList();
                foreach (var employee in result)
                {
                    if (employee.Code.Count() == 0)
                    {
                        if (employee.TEL1 != "" && employee.TEL1 != null && employee.TEL1.Length == 11)
                        {
                            Random generator = new Random();
                            String SMSCode = generator.Next(0, 1000000).ToString("D6");
                            EmployeesSMSCode NewEmpSMSCode = new EmployeesSMSCode();
                            NewEmpSMSCode.EmpId = employee.Id;
                            NewEmpSMSCode.SMSCode = SMSCode;
                            NewEmpSMSCode.IsActive = true;
                            NewEmpSMSCode.CreatedBy = User.Identity.Name;
                            NewEmpSMSCode.CreatedDate = DateTime.Now;
                            NewEmpSMSCode.LastSentDate = DateTime.Now;
                            db.EmployeesSMSCodes.Add(NewEmpSMSCode);

                            db.SaveChanges();
                            try
                            {
                                PostSMSData("your DMS verification code to dispense chronic medicines is " + SMSCode, employee.TEL1);

                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }
                    }
                }
                return Json(new { ok = true, returndata = "Send all", message = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                return Json(new { ok = false, returndata = "Invalid Telephone,Please Update Telephone 1 Number", message = "ok" }, JsonRequestBehavior.AllowGet);

            }

        }
        // GET: CompEmployees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comp_Employees comp_Employees = db.Comp_Employees.Find(id);
            if (comp_Employees == null)
            {
                return HttpNotFound();
            }
            return View(comp_Employees);
        }

        // GET: CompEmployees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompEmployees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CARD_ID,COMP_ID,BRANCH_CODE,C_COMP_ID,CONTRACT_NO,CLASS_CODE,DEPT_ID,EMP_CODE,EMP_ANAME,EMP_ENAME,ADDRESS1,ADDRESS2,TEL1,TEL2,FAX,EMAIL,BIRTH_DATE,HIRE_DATE,INS_TYP,REF_EMP,MATERIAL_STATUS,TERMINATE_FLAG,TERMINATE_DATE,EXP_CELLING,GENDER,INS_START_DATE,INS_END_DATE,W_GLASS,EXP_AGE,EMP_ID,EMP_INSURANCE_NO,EMP_IMG,OLD_CLASS_CODE,TRANS_EMO_TYPE,EFFECT_DATE_TYPE,SPECIFIC_DATE,DELV_CARD_DATE,PRINT_CARD,NOTES,ACTIVE,CREATED_DATE,CREATED_BY,UPDATE_BY,UPDATE_DATE,EMP_ANAME_ST,EMP_ANAME_SC,EMP_ANAME_TH,EMP_ANAME_FR,EMP_ANAME_FAM,EMP_ENAME_ST,EMP_ENAME_SC,EMP_ENAME_TH,EMP_ENAME_FR,EMP_ENAME_FAM,DEL_DELIVIER_CARD,BK_CODE,BK_ACC_NO,COST_CODE,PRINT_FST_NAME,PRINT_SEC_NAME,PRINT_THR_NAME,PRINT_FTH_NAME,PRINT_LST_NAME,PRINT_TYP,END_WRK_DT,USR_TYP,EMP_SEQ,IsSync,SyncDate,SyncBy")] Comp_Employees comp_Employees)
        {
            if (ModelState.IsValid)
            {
                db.Comp_Employees.Add(comp_Employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(comp_Employees);
        }

        // GET: CompEmployees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comp_Employees comp_Employees = db.Comp_Employees.Find(id);
            if (comp_Employees == null)
            {
                return HttpNotFound();
            }
            return View(comp_Employees);
        }

        // POST: CompEmployees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CARD_ID,ADDRESS1,ADDRESS2,TEL1,TEL2,EMP_ID,EMAIL,EMP_ANAME_ST,EMP_ANAME_SC,EMP_ANAME_TH,EMP_ANAME_FR,EMP_ENAME_ST,EMP_ENAME_SC,EMP_ENAME_TH,EMP_ENAME_FR")] Comp_Employees comp_Employees)
        {
            if (ModelState.IsValid)
            {
                Comp_Employees current = db.Comp_Employees.Find(comp_Employees.Id);
                current.EMP_ANAME = comp_Employees.EMP_ANAME_ST + " " + comp_Employees.EMP_ANAME_SC + " " + comp_Employees.EMP_ANAME_TH;
                current.EMP_ENAME = comp_Employees.EMP_ENAME_ST + " " + comp_Employees.EMP_ENAME_SC + " " + comp_Employees.EMP_ENAME_TH;
                current.ADDRESS1 = comp_Employees.ADDRESS1;
                current.ADDRESS2 = comp_Employees.ADDRESS2;
                current.TEL1 = comp_Employees.TEL1;
                current.TEL2 = comp_Employees.TEL2;
                current.EMP_ID = comp_Employees.EMP_ID;
                current.EMAIL = comp_Employees.EMAIL;
                current.EMP_ANAME_ST = comp_Employees.EMP_ANAME_ST;
                current.EMP_ANAME_SC = comp_Employees.EMP_ANAME_SC;
                current.EMP_ANAME_TH = comp_Employees.EMP_ANAME_TH;
                current.EMP_ANAME_FR = comp_Employees.EMP_ANAME_FR;
                current.EMP_ENAME_ST = comp_Employees.EMP_ENAME_ST;
                current.EMP_ENAME_SC = comp_Employees.EMP_ENAME_SC;
                current.EMP_ENAME_TH = comp_Employees.EMP_ENAME_TH;
                current.EMP_ENAME_FR = comp_Employees.EMP_ENAME_FR;

                db.Entry(current).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexSms");
            }
            return View(comp_Employees);
        }

        // GET: CompEmployees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comp_Employees comp_Employees = db.Comp_Employees.Find(id);
            if (comp_Employees == null)
            {
                return HttpNotFound();
            }
            return View(comp_Employees);
        }

        // POST: CompEmployees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comp_Employees comp_Employees = db.Comp_Employees.Find(id);
            db.Comp_Employees.Remove(comp_Employees);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #region Methods
        private string SecretHashMethod(string Message, string PhoneNumber)
        {
            string secret = "B88551A75DC04D78BB92ABAD298BB19F";
            StringBuilder SecretHash = new StringBuilder();

            //var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes("AccountId=200001555&Password=Vodafone.1&SenderName=DIAMOND MED&ReceiverMSISDN=" + PhoneNumber + "&SMSText=" + Message);
            //byte[] messageBytes = encoding.GetBytes("AccountId=200001555&Password=Vodafone.1&SenderName=DIAMOND MED&ReceiverMSISDN=01028599477&SMSText=Hello World");
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                foreach (Byte b in hashmessage)
                    SecretHash.Append(b.ToString("x2"));
                return SecretHash.ToString().ToUpper();
            }
        }
        public string PostSMSData(string Message, string PhoneNumber)
        {
            string requestXml =
                "<SubmitSMSRequest xmlns='http://www.edafa.com/web2sms/sms/model/'>" +
                "<AccountId>200001555</AccountId>" +
                "<Password>Vodafone.1</Password>" +
                "<SecureHash>" + SecretHashMethod(Message, PhoneNumber) + "</SecureHash>" +
                "<SMSList>" +
                "<SenderName>DIAMOND MED</SenderName>" +
                "<ReceiverMSISDN>" + PhoneNumber + "</ReceiverMSISDN>" +
                "<SMSText>" + Message + "</SMSText>" +
                "</SMSList>" +
                "</SubmitSMSRequest>";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://e3len.vodafone.com.eg/web2sms/sms/submit/");
            byte[] bytes;
            bytes = System.Text.Encoding.UTF8.GetBytes(requestXml);
            request.ContentType = "application/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }
            return null;
        }
        #endregion
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
