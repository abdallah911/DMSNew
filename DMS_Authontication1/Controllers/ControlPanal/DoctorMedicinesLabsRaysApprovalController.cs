using DMS_Authontication1.Models;
using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using DMS_TEST;
using DMS_Authontication1.ViewModel;
using System.IO;
using System.Text;

namespace DMS_Authontication1.Controllers.ControlPanal
{
    public class DoctorMedicinesLabsRaysApprovalController : Controller
    {
        private DMS_TESTEntities db;
        // private UsersEntities db1;

        public DoctorMedicinesLabsRaysApprovalController()
        {
            db = new DMS_TESTEntities();
        }
        // GET: DoctorMedicinesLabsRaysApproval

        [Authorize(Roles = "Admin,Doctor")]

        public ActionResult Index(int NotificationId)
        {
            var model = db.Notifications.Where(n => n.Id == NotificationId && n.IsDeleted == false && n.IsRead == false)
                .Include(x => x.Roshita).Include(r => r.Roshita.RoshitaDetails).Include(rd => rd.Roshita.PrescriptionRoshitaDignosis)
                .FirstOrDefault();
            //model.Roshita.RoshitaDetails = model.Roshita.RoshitaDetails.Where(x => x.PaymentGroup == "Pending").ToList();
            return View(model);
        }
        // GET: DoctorMedicinesLabsRaysApproval

        [Authorize(Roles = "Admin,Doctor")]

        public ActionResult Index2(string Id)
        {
            ViewBag.cardId = Id;
            return View();
        }
        public JsonResult Approvals(string id)
        {
            List<Roshita_RoshitaDetails> RoshitaDetailsList = new List<Roshita_RoshitaDetails>();
            //  var date = db.Roshitas.Where(x => x.CardId == id).OrderByDescending(t => t.CreatedDate).FirstOrDefault();
            var RoshitaDetails = db.Roshitas.Where(x => x.CardId == id && !x.Manager.Contains("_Stop"))
                .Join(db.RoshitaDetails, r => r.Id, d => d.RoshitaID, (r, d) => new { r, d })
                .Where(c => c.d.PaymentGroup == "Pending")
                .Select(l => new Roshita_RoshitaDetails
                {
                    Id = l.r.Id
                }).Distinct().ToList();
            foreach (var item in RoshitaDetails)
            {
                List<Roshita_RoshitaDetails> currentRoshitaDetailsList = db.Roshitas.Where(x => x.Id == item.Id)
                .Join(db.RoshitaDetails, r => r.Id, d => d.RoshitaID, (r, d) => new { r, d })
                .Select(l => new Roshita_RoshitaDetails
                {
                    Id = l.r.Id,  //approvalId 
                    DId = l.d.Id,
                    MedicienName = l.d.MedicienName,
                    CreatedBy = l.r.CreatedBy,//branchid
                    Amount = l.d.Amount,
                    Manager = l.r.Manager,
                    CreatedDate = l.r.CreatedDate,
                    PaymentGroup = l.d.PaymentGroup,
                }).OrderBy(x => x.Id).ThenBy(x => x.PaymentGroup).ToList();
                RoshitaDetailsList.AddRange(currentRoshitaDetailsList);
            }

            return new JsonResult { Data = RoshitaDetailsList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ChangeStatus(int MedicineId, int NotificationId, string status)
        {



            if (status != "N")
            {
                var emp = db.RoshitaDetails.Where(x => x.Id == MedicineId).FirstOrDefault();
                emp.PaymentGroup = status;
                db.Entry(emp).State = EntityState.Modified;
                var Roshita = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault();
                string CardId = Roshita.CardId;
                Roshita.SyncBy = "Update";
                Roshita.UpdatedBy = User.Identity.Name;
                Roshita.UpdatedDate = DateTime.Now;
                Notification notification = new Notification();
                Notification notificationchick = db.Notifications.Where(x => x.Id == NotificationId && x.Title != "Pending").OrderByDescending(x => x.Id).FirstOrDefault();
                if (notificationchick != null)
                {
                    notification = notificationchick;
                }
                else
                {
                    notification = db.Notifications.Where(x => x.Id == NotificationId).OrderByDescending(x => x.Id).FirstOrDefault();
                }
                NotificationHub objNotifHub = new NotificationHub();
                //Notification notification = db.Notifications.Where(x => x.Id == NotificationId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (notification.CreatedBy != User.Identity.Name)
                {
                    notification.SentTo = notification.CreatedBy;
                    notification.CreatedBy = User.Identity.Name;
                }
                notification.CreatedDate = DateTime.Now;
                //notification.Title = status;
                if (status == "Accepted")
                {
                    notification.Type = 2;//Accepted
                    notification.Title = status;

                    if (Roshita.Manager == "Daily" || Roshita.Manager == "Monthly")
                    {
                        notification.DetailsURL = "/Pharmacy/Pending";
                        notification.TypeNmae = "Medicine";//Accepted
                    }
                    else if (Roshita.Manager == "Lab")
                    {
                        notification.DetailsURL = "/Labs/Pending";
                        notification.TypeNmae = "Lab";//Accepted
                    }
                    else if (Roshita.Manager == "Ray")
                    {
                        notification.DetailsURL = "/Rays/Pending";
                        notification.TypeNmae = "Ray";//Accepted
                    }
                }
                else if (status == "Rejected")
                {
                    var roshitaList = db.RoshitaDetails.Where(x => x.RoshitaID == Roshita.Id && x.Id != MedicineId && x.PaymentGroup == "Accepted" && x.IsDealed == false).ToList();
                    if (roshitaList.Count == 0)
                    {
                        notification.Type = 3;//Rejected
                        notification.Title = status;
                        if (Roshita.Manager == "Daily" || Roshita.Manager == "Monthly")
                        {
                            notification.DetailsURL = "/Pharmacy/Pending";
                            notification.TypeNmae = "Medicine";//Accepted
                        }
                        else if (Roshita.Manager == "Lab")
                        {
                            notification.DetailsURL = "/Labs/Pending";
                            notification.TypeNmae = "Lab";//Accepted
                        }
                        else if (Roshita.Manager == "Ray")
                        {
                            notification.DetailsURL = "/Rays/Pending";
                            notification.TypeNmae = "Ray";//Accepted
                        }
                    }
                    else
                    {
                        notification.Type = 2;//Accepted
                        notification.Title = status;

                        if (Roshita.Manager == "Daily" || Roshita.Manager == "Monthly")
                        {
                            notification.DetailsURL = "/Pharmacy/Pending";
                            notification.TypeNmae = "Medicine";//Accepted
                        }
                        else if (Roshita.Manager == "Lab")
                        {
                            notification.DetailsURL = "/Labs/Pending";
                            notification.TypeNmae = "Lab";//Accepted
                        }
                        else if (Roshita.Manager == "Ray")
                        {
                            notification.DetailsURL = "/Rays/Pending";
                            notification.TypeNmae = "Ray";//Accepted
                        }
                    }

                }
                db.Entry(notification).State = EntityState.Modified;
                if (notificationchick == null)
                {
                    objNotifHub.SendMessages();
                }
            }
            int result = db.SaveChanges();
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ChangeStatus2(int ApprovalId, string status)
        {



            if (status != "N")
            {
                var emp = db.RoshitaDetails.Where(x => x.Id == ApprovalId).FirstOrDefault();
                emp.PaymentGroup = status;
                db.Entry(emp).State = EntityState.Modified;
                var Roshita = db.Roshitas.Where(x => x.Id == emp.RoshitaID).FirstOrDefault();
                string CardId = Roshita.CardId;
                Roshita.SyncBy = "Update";
                Roshita.UpdatedBy = User.Identity.Name;
                Roshita.UpdatedDate = DateTime.Now;
                NotificationHub objNotifHub = new NotificationHub();
                Notification notification = db.Notifications.Where(x => x.Details == CardId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (notification.CreatedBy != User.Identity.Name)
                {
                    notification.SentTo = notification.CreatedBy;
                    notification.CreatedBy = User.Identity.Name;
                }
                notification.CreatedDate = DateTime.Now;
                //notification.Title = status;
                if (status == "Accepted")
                {
                    notification.Type = 2;//Accepted
                    notification.Title = status;

                    if (Roshita.Manager == "Daily" || Roshita.Manager == "Monthly")
                    {
                        notification.DetailsURL = "/Pharmacy/Pending?id=" + CardId;
                    }
                    else if (Roshita.Manager == "Lab")
                    {
                        notification.DetailsURL = "/Labs/Pending?id=" + CardId;
                    }
                    else if (Roshita.Manager == "Ray")
                    {
                        notification.DetailsURL = "/Rays/Pending?id=" + CardId;
                    }
                }
                else if (status == "Rejected")
                {
                    var roshitaList = db.RoshitaDetails.Where(x => x.RoshitaID == Roshita.Id && x.Id != ApprovalId && x.PaymentGroup == "Accepted" && x.IsDealed == false).ToList();
                    if (roshitaList.Count == 0)
                    {
                        notification.Type = 3;//Rejected
                        notification.Title = status;
                        if (Roshita.Manager == "Daily" || Roshita.Manager == "Monthly")
                        {
                            notification.DetailsURL = "/Pharmacy/Pharmacy";
                        }
                        else if (Roshita.Manager == "Lab")
                        {
                            notification.DetailsURL = "/Labs/Lab";
                        }
                        else if (Roshita.Manager == "Ray")
                        {
                            notification.DetailsURL = "/Rays/Ray";
                        }
                    }

                }
                db.Entry(notification).State = EntityState.Modified;

                objNotifHub.SendMessages();
            }
            int result = db.SaveChanges();
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Create2()
        {
            return View();
        }
        public JsonResult getDiag()
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=196.221.203.130)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            cmd = new OracleCommand("select *  from diagnosis", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            var diagnoise = (from DataRow dr in dt.Rows
                             select new
                             {
                                 Code = Convert.ToInt32(dr["DIA_CODE"]),
                                 Name = dr["DIA_ENAME"].ToString()
                             }).ToList();
            //    data.OnlineLiveConsumption = Convert.ToInt32(dt.Rows[0][0]);
            return new JsonResult { Data = diagnoise, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult PersonalData(string id)
        {
            string[] CompId = id.Split('-');
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            //model Personal data
            CreateApprovalPersonalDataViewModel data = new CreateApprovalPersonalDataViewModel();
            //max contract
            cmd = new OracleCommand("select max(CONTRACT_NO) from comp_employeess WHERE C_COMP_ID = '" + CompId[0] + "' ", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            data.ContractNo = Convert.ToInt32(dt.Rows[0][0]);
            //comp_employees data
            cmd = new OracleCommand("select CARD_ID,Emp_Ename,BIRTH_DATE,Specific_date,Ins_End_date,C_COMP_ID,CLASS_CODE,INS_START_DATE FROM comp_employeess where card_id = '" + id + "' and contract_no=" + Convert.ToInt32(dt.Rows[0][0]), con);

            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            data.CARD_ID = dt.Rows[0][0].ToString();
            Session["CardId"] = data.CARD_ID;
            data.EMP_ENAME = dt.Rows[0][1].ToString();
            data.BirthDate = Convert.ToDateTime(dt.Rows[0][2]);
            data.Specific_date = Convert.ToDateTime(dt.Rows[0][3]);
            data.INS_END_DATE = Convert.ToDateTime(dt.Rows[0][4]);
            data.C_COMP_ID = Convert.ToInt32(dt.Rows[0][5]);
            data.CLASS_CODE = Convert.ToString(dt.Rows[0][6]);
            data.INS_START_DATE = Convert.ToDateTime(dt.Rows[0][7]);
            Session["CompId"] = data.C_COMP_ID;
            Session["ClassCode"] = data.CLASS_CODE;

            //v_P_Contract_class
            cmd = new OracleCommand("select MAX_AMOUNT from V_P_COMP_CONTRACT_CLASS where C_COMP_ID=" + data.C_COMP_ID + " and contract_no=" + data.ContractNo + " and CLASS_CODE='" + data.CLASS_CODE + "'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            data.MaxAmount = Convert.ToInt32(dt.Rows[0][0]);
            //IRS Consumption
            cmd = new OracleCommand("select TOT_NET_AMT from IRS_CONSUMPTION where CARD_NO = '" + data.CARD_ID + "' and CONTRACT_NO = (select max(CONTRACT_NO) FROM IRS_CONSUMPTION where CARD_NO = '" + data.CARD_ID + "') ", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0 || dt.Rows[0][0].ToString() == string.Empty)
            {
                data.IRSConsumption = 0;
            }
            else
            {
                data.IRSConsumption = Convert.ToInt32(dt.Rows[0][0]);
            }
            //online_cons_01 other
            //online consumption
            cmd = new OracleCommand(" SELECT SUM(CLAIM_AMOUNT) FROM ONLINE_CONS_01 WHERE group_no =116 And CARD_NO = '" + data.CARD_ID + "'and claim_date BETWEEN :dat1 AND :dat2", con);
            cmd.Parameters.Clear();
            cmd.Parameters.Add(":date1", OracleDbType.Date).Value = Convert.ToDateTime(data.Specific_date);
            cmd.Parameters.Add(":date2", OracleDbType.Date).Value = Convert.ToDateTime(data.INS_END_DATE);

            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0 || dt.Rows[0][0].ToString() == string.Empty)
            {
                data.OnlineConsumption = 0;

            }
            else
            {
                data.OnlineConsumption = Convert.ToInt32(dt.Rows[0][0]);
            }

            //others
            cmd = new OracleCommand(" SELECT SUM(CLAIM_AMOUNT) FROM ONLINE_CONS_01 WHERE group_no !=116 And CARD_NO = '" + data.CARD_ID + "'and claim_date BETWEEN :dat1 AND :dat2", con);
            cmd.Parameters.Clear();
            cmd.Parameters.Add(":date1", OracleDbType.Date).Value = Convert.ToDateTime(data.Specific_date);
            cmd.Parameters.Add(":date2", OracleDbType.Date).Value = Convert.ToDateTime(data.INS_END_DATE);

            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0 || dt.Rows[0][0].ToString() == string.Empty)
            {
                data.OtherConsumption = 0;

            }
            else
            {
                data.OtherConsumption = Convert.ToInt32(dt.Rows[0][0]);
            }

            //cost centers
            cmd = new OracleCommand(@"select IRS_EMPLOYEES.COMP_DEP_CODE, V_COMPANIES_CC.E_NAME 
                                        FROM IRS_EMPLOYEES, V_COMPANIES_CC
                                        WHERE TO_CHAR(IRS_EMPLOYEES.COMP_DEP_CODE) = V_COMPANIES_CC.COST_CODE AND IRS_EMPLOYEES.CARD_NO = '" + data.CARD_ID + "'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                data.CostCenter = dt.Rows[0][0].ToString() + "||" + dt.Rows[0][1].ToString();
            }
            else
            {
                data.CostCenter = "No Cost Center";
            }
            //online live consumption
            cmd = new OracleCommand(@"select NVL(sum(TOT_AMOUNT_ACT),0) 
                                            FROM ONLINE_CONS_ACT_02 
                                            where CARD_NO = '" + data.CARD_ID + "'" +
                                       " and COMP_ID = '" + data.C_COMP_ID + "'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            data.OnlineLiveConsumption = Convert.ToInt32(dt.Rows[0][0]);
            //Total approvals 
            //contract
            cmd = new OracleCommand("select max(contract_no) from COMP_EMPLOYEESS where C_COMP_ID=" + data.C_COMP_ID + "", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            data.ContractNo = Convert.ToInt32(dt.Rows[0][0]);
            Session["ContractNo"] = data.ContractNo;
            //new approval
            cmd = new OracleCommand(@"select sum(VALUE_AFTER) FROM MEDICAL_APPROVALS 
                                            where CARD_NO = '" + data.CARD_ID + "'" +
                                            " and COMP_CONTRACT_NO = " + data.ContractNo + "AND active = 'Y'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            data.TotalApproval = Convert.ToInt32(dt.Rows[0][0]);
            //old approval
            //cmd = new OracleCommand(@" select sum(APPROV_AMOUNT) from V_APPROVAL where CARD_NO = :crd and CREATED_DATE between (select  max(INS_START_DATE) from comp_employees where card_id = :crd) and :dat2  ", con);
            //cmd.Parameters.Clear();
            //cmd.Parameters.Add(":crd", OracleDbType.Varchar22).Value = data.CARD_ID;
            //// cmd.Parameters.Add(":dat1", OracleDbType.DateTime).Value = dat1;
            //cmd.Parameters.Add(":dat2", OracleDbType.Date).Value =Convert.ToDateTime(data.INS_START_DATE);
            //da = new OracleDataAdapter(cmd);
            //dt = new DataTable();
            //da.Fill(dt);
            //data.TotalApproval +=Convert.ToInt32(dt.Rows[0][0]);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ServiceProviderType()
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            cmd = new OracleCommand("select prv_type, typ_ename FROM provider_typ22", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            var ser = (from DataRow dr in dt.Rows
                       select new
                       {
                           ProviderType = Convert.ToInt32(dr["prv_type"]),
                           ProviderName = dr["typ_ename"].ToString()
                       }).ToList();
            //    data.OnlineLiveConsumption = Convert.ToInt32(dt.Rows[0][0]);
            return new JsonResult { Data = ser, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult Providers(int id)
        {
            var ser = db.Serv_Providers1.Where(a => a.PRV_TYPE == id).ToList();
            return new JsonResult { Data = ser, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ServiceType(string ProviderTypeId)
        {
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            cmd = new OracleCommand("select super_group_ename,super_group_Code from SERVICES_SUPER_GROUP where " + ProviderTypeId + " in (prv_type1 , prv_type2 , prv_type3 , prv_type4 , prv_type5) ", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "--Select Service type--", Value = "0" });
            foreach (DataRow row in dt.Rows)
                list.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[0]), Value = Convert.ToString(row.ItemArray[1]) });
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        public JsonResult SubService(string ServiceType)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            cmd = new OracleCommand("select group_no, group_ename from services_group where super_group_code ='" + ServiceType + "'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            var SubService = (from DataRow dr in dt.Rows
                              select new
                              {
                                  Code = dr["group_no"].ToString(),
                                  Name = dr["group_ename"].ToString()
                              }).ToList();
            //    data.OnlineLiveConsumption = Convert.ToInt32(dt.Rows[0][0]);
            return new JsonResult { Data = SubService, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Services(string id)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            cmd = new OracleCommand("SELECT * FROM (select SERV_CODE, SERV_ANAME, SERV_ENAME, SERV_CODE_H FROM v_services where serv_code like '" + id + "%' order by SERV_ENAME) ", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            var Service = (from DataRow dr in dt.Rows
                           select new
                           {
                               Code = dr["SERV_CODE"].ToString(),
                               ArName = dr["SERV_ANAME"].ToString(),
                               EnName = dr["SERV_ENAME"].ToString()
                           }).ToList();
            return new JsonResult { Data = Service, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult Pools()
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            cmd = new OracleCommand(@"SELECT V_P_COMP_CONTRACT_POLLS.M_AMOUNT_TYP, V_P_COMP_CONTRACT_POLLS.M_AMT_TYP, V_P_COMP_CONTRACT_POLLS.M_PERCENT_FROM, V_P_COMP_CONTRACT_POLLS.M_CALC_METHOD, 
                                                                V_P_COMP_CONTRACT_POLLS.POOL_NAME, V_P_COMP_CONTRACT_POLLS.AMOUNT_POLL, V_P_COMP_CONTRACT_POLLS.AMOUNT_TYP, V_P_COMP_CONTRACT_POLLS.AMT_TYP,
                                                                V_P_CONTRACT_POLLS_SERV.SERV_CODE, V_SERVICES.SERV_ENAME, V_SERVICES.SERV_ANAME 
                                                                FROM V_P_COMP_CONTRACT_POLLS 
                                                                LEFT OUTER JOIN V_P_CONTRACT_POLLS_SERV ON V_P_COMP_CONTRACT_POLLS.POLL_CODE = V_P_CONTRACT_POLLS_SERV.POLL_CODE
                                                                LEFT OUTER JOIN V_SERVICES ON V_P_CONTRACT_POLLS_SERV.SERV_CODE = V_SERVICES.SERV_CODE
                                                                WHERE  V_P_COMP_CONTRACT_POLLS.C_COMP_ID = " + Convert.ToInt32(Session["CompId"]) + " AND V_P_COMP_CONTRACT_POLLS.CONTRACT_NO = " + Convert.ToInt32(Session["ContractNo"]) + "AND V_P_COMP_CONTRACT_POLLS.ACTIVE = 'Y' AND V_P_CONTRACT_POLLS_SERV.CLASS_CODE = '" + Session["ClassCode"].ToString() + "'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            var Pools = (from DataRow dr in dt.Rows
                         select new
                         {
                             M_PERCENT_FROM = dr["V_P_COMP_CONTRACT_POLLS.M_PERCENT_FROM"].ToString(),
                             M_AMOUNT_TYP = dr["V_P_COMP_CONTRACT_POLLS.M_AMOUNT_TYP"].ToString(),
                             M_AMT_TYP = dr[" V_P_COMP_CONTRACT_POLLS.M_AMT_TYP"].ToString()
                         }).ToList();
            //    data.OnlineLiveConsumption = Convert.ToInt32(dt.Rows[0][0]);
            return new JsonResult { Data = Pools, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult CardDesign()
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;
            byte[] bimg = null;
            cmd = new OracleCommand(@"select CARD_IMAGE,VIP_IMAGE from CARDS_DESIGN WHERE code='" + Session["CardId"].ToString() + "'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                if (!(dt.Rows[0][0].Equals(DBNull.Value)))
                {
                    bimg = (byte[])dt.Rows[0][0];
                    // Imgcardesign.Source = BitmapImageFromBytes(bimg);
                }
                else if (!(dt.Rows[0][1].Equals(DBNull.Value)))
                {
                    bimg = (byte[])dt.Rows[0][1];
                    // Imgcardesign.Source = BitmapImageFromBytes(bimg);
                }
            }
            else
            {
                cmd = new OracleCommand(@"select CARD_IMAGE,VIP_IMAGE from CARDS_DESIGN WHERE C_COMP_ID='" + Convert.ToInt32(Session["CompId"]) + "' AND CONTRACT_NO='" + Convert.ToInt32(Session["ContractNo"]) + "' AND  CLASS_CODE='" + Session["ClassCode"].ToString() + "'", con);
                da = new OracleDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    if (!(dt.Rows[0][0].Equals(DBNull.Value)))
                    {
                        bimg = (byte[])dt.Rows[0][0];
                        // Imgcardesign.Source = BitmapImageFromBytes(bimg);
                    }
                    else if (!(dt.Rows[0][1].Equals(DBNull.Value)))
                    {
                        bimg = (byte[])dt.Rows[0][1];
                        // Imgcardesign.Source = BitmapImageFromBytes(bimg);
                    }
                }
            }
            String base64string;
            if (bimg == null)
            {
                base64string = "0";
            }
            else
            {
                base64string = Convert.ToBase64String(bimg);
            }
            return new JsonResult { Data = base64string, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult History()
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            // DataTable dt;
            System.Data.DataTable dataprovold = new System.Data.DataTable();
            System.Data.DataTable dataprovold2 = new System.Data.DataTable();
            System.Data.DataTable dataprovnew = new System.Data.DataTable();

            cmd = new OracleCommand(@"SELECT * FROM ( 
                                            select    TO_CHAR(APROV_NO) APPROV_NO, COMP_ID COMP_ID, CARD_NO CARD_NO, PATIENT_NAME NAME,TO_CHAR(DATE_RECIVE,'DD-MM-YYYY') RECIV_DATE,
                                                      TO_CHAR(DATE_SEND,'DD-MM-YYYY') SEND_DATE, SERV_ENAME SERVECE_TYP, APROV_REPLY  REPLY,
                                                      APPROV_AMOUNT APPROV_AMOUNT, MED_APP MEDICAL_REPLAY,CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE 
                                            FROM      V_APPROVAL 
                                      LEFT OUTER JOIN IRS_SUPER_GROUP_NEW ON V_APPROVAL.APROV_TYP = IRS_SUPER_GROUP_NEW.IRS_CODE
                                            WHERE     CARD_NO = '" + Session["CardId"].ToString() + "' order by  TO_DATE(created_date,'DD-MM-YYYY') desc) /* where rownum < 6 */", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dataprovold);
            cmd = new OracleCommand(@" SELECT * FROM (
                                            select    TO_CHAR(APROV_NO) APPROV_NO, COMP_ID COMP_ID, CARD_NO CARD_NO, PATIENT_NAME NAME,TO_CHAR(DATE_RECIVE,'DD-MM-YYYY') RECIV_DATE,
                                                      TO_CHAR(DATE_SEND,'DD-MM-YYYY') SEND_DATE, SERV_ENAME SERVECE_TYP, APROV_REPLY  REPLY,
                                                      APPROV_AMOUNT APPROV_AMOUNT, MED_APP MEDICAL_REPLAY,CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE 
                                            FROM      IRS_APPROVAL_HIST 
                                      LEFT OUTER JOIN IRS_SUPER_GROUP_NEW ON IRS_APPROVAL_HIST.APROV_TYP = IRS_SUPER_GROUP_NEW.IRS_CODE
                                            WHERE     CARD_NO = '" + Session["CardId"].ToString() + "' order by  TO_DATE(created_date,'DD-MM-YYYY') desc) /*where rownum < 6*/", con);
            da = new OracleDataAdapter(cmd);

            da.Fill(dataprovold2);
            dataprovold.Merge(dataprovold2);
            dataprovnew.Merge(dataprovold);
            cmd = new OracleCommand(@"SELECT * FROM ( 
                                            select    CODE APPROV_NO, COMPANY_ID COMP_ID, CARD_NO CARD_NO, EMP_ENAME NAME, TO_CHAR(RECIV_DATE,'DD-MM-YYYY') RECIV_DATE, TO_CHAR(SEND_DATE,'DD-MM-YYYY') SEND_DATE, 
                                                      SERVECE_TYP SERVECE_TYP, REPLAY REPLY, VALUE_AFTER APPROV_AMOUNT, MEDICAL_REPLAY MEDICAL_REPLAY, 
                                                      CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE
                                            FROM      MEDICAL_APPROVALS 
                                            WHERE     CARD_NO = '" + Session["CardId"].ToString() + "' AND active = 'Y' order by  TO_DATE(created_date,'DD-MM-YYYY') desc) /* where rownum < 6*/ ", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dataprovnew);
            var data = (from DataRow dr in dataprovnew.Rows
                        select new
                        {
                            // ProviderType = Convert.ToInt32(dr["prv_type"]),
                            APPROV_NO = dr["APPROV_NO"].ToString(),
                            COMP_ID = dr["COMP_ID"].ToString(),
                            CARD_NO = dr["CARD_NO"].ToString(),
                            NAME = dr["NAME"].ToString(),
                            RECIV_DATE = dr["RECIV_DATE"].ToString(),
                            SEND_DATE = dr["SEND_DATE"].ToString(),
                            SERVECE_TYP = dr["SERVECE_TYP"].ToString(),
                            REPLY = dr["REPLY"].ToString(),
                            APPROV_AMOUNT = dr["APPROV_AMOUNT"].ToString(),
                            MEDICAL_REPLAY = dr["MEDICAL_REPLAY"].ToString(),
                            CREATED_BY = dr["CREATED_BY"].ToString(),
                            CREATED_DATE = dr["CREATED_DATE"].ToString(),
                        }).ToList();
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //public JsonResult ApprovalDetails()
        // {
        //     System.Data.DataTable dtsrev = db.RunReader(@"select    APPROVAL_SUB_SERV.S_SERV_NAME, APPROVAL_SUB_SERV.DETAILS
        //                                                       FROM      APPROVAL_SUB_SERV WHERE    APPROVAL_SUB_SERV.CODE = '" + row[0].ToString() + "'").Result;

        //     System.Data.DataTable dtdiag = db.RunReader(@"select    MEDICAL_APPROVALS.APROVAL_IMAG, APPROVAL_DIAG.DIAG_NAME FROM      MEDICAL_APPROVALS, APPROVAL_DIAG
        //                                                       WHERE     MEDICAL_APPROVALS.CODE = APPROVAL_DIAG.CODE  AND MEDICAL_APPROVALS.CODE = '" + row[0].ToString() + "' ").Result;

        //     //connection
        //     OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
        //                                     (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
        //                                     (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
        //     OracleCommand cmd;
        //     OracleDataAdapter da;
        //     // DataTable dt;
        //     System.Data.DataTable dataprovold = new System.Data.DataTable();
        //     System.Data.DataTable dataprovold2 = new System.Data.DataTable();
        //     System.Data.DataTable dataprovnew = new System.Data.DataTable();

        //     cmd = new OracleCommand(@"SELECT * FROM ( 
        //                                     select    TO_CHAR(APROV_NO) APPROV_NO, COMP_ID COMP_ID, CARD_NO CARD_NO, PATIENT_NAME NAME,TO_CHAR(DATE_RECIVE,'DD-MM-YYYY') RECIV_DATE,
        //                                               TO_CHAR(DATE_SEND,'DD-MM-YYYY') SEND_DATE, SERV_ENAME SERVECE_TYP, APROV_REPLY  REPLY,
        //                                               APPROV_AMOUNT APPROV_AMOUNT, MED_APP MEDICAL_REPLAY,CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE 
        //                                     FROM      V_APPROVAL 
        //                               LEFT OUTER JOIN IRS_SUPER_GROUP_NEW ON V_APPROVAL.APROV_TYP = IRS_SUPER_GROUP_NEW.IRS_CODE
        //                                     WHERE     CARD_NO = '" + Session["CardId"].ToString() + "' order by  TO_DATE(created_date,'DD-MM-YYYY') desc) /* where rownum < 6 */", con);
        //     da = new OracleDataAdapter(cmd);
        //     da.Fill(dataprovold);
        //     cmd = new OracleCommand(@" SELECT * FROM (
        //                                     select    TO_CHAR(APROV_NO) APPROV_NO, COMP_ID COMP_ID, CARD_NO CARD_NO, PATIENT_NAME NAME,TO_CHAR(DATE_RECIVE,'DD-MM-YYYY') RECIV_DATE,
        //                                               TO_CHAR(DATE_SEND,'DD-MM-YYYY') SEND_DATE, SERV_ENAME SERVECE_TYP, APROV_REPLY  REPLY,
        //                                               APPROV_AMOUNT APPROV_AMOUNT, MED_APP MEDICAL_REPLAY,CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE 
        //                                     FROM      IRS_APPROVAL_HIST 
        //                               LEFT OUTER JOIN IRS_SUPER_GROUP_NEW ON IRS_APPROVAL_HIST.APROV_TYP = IRS_SUPER_GROUP_NEW.IRS_CODE
        //                                     WHERE     CARD_NO = '" + Session["CardId"].ToString() + "' order by  TO_DATE(created_date,'DD-MM-YYYY') desc) /*where rownum < 6*/", con);
        //     da = new OracleDataAdapter(cmd);

        //     da.Fill(dataprovold2);
        //     dataprovold.Merge(dataprovold2);
        //     dataprovnew.Merge(dataprovold);
        //     cmd = new OracleCommand(@"SELECT * FROM ( 
        //                                     select    CODE APPROV_NO, COMPANY_ID COMP_ID, CARD_NO CARD_NO, EMP_ENAME NAME, TO_CHAR(RECIV_DATE,'DD-MM-YYYY') RECIV_DATE, TO_CHAR(SEND_DATE,'DD-MM-YYYY') SEND_DATE, 
        //                                               SERVECE_TYP SERVECE_TYP, REPLAY REPLY, VALUE_AFTER APPROV_AMOUNT, MEDICAL_REPLAY MEDICAL_REPLAY, 
        //                                               CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE
        //                                     FROM      MEDICAL_APPROVALS 
        //                                     WHERE     CARD_NO = '" + Session["CardId"].ToString() + "' AND active = 'Y' order by  TO_DATE(created_date,'DD-MM-YYYY') desc) /* where rownum < 6*/ ", con);
        //     da = new OracleDataAdapter(cmd);
        //     da.Fill(dataprovnew);
        //     var data = (from DataRow dr in dataprovnew.Rows
        //                 select new
        //                 {
        //                     // ProviderType = Convert.ToInt32(dr["prv_type"]),
        //                     APPROV_NO = dr["APPROV_NO"].ToString(),
        //                     COMP_ID = dr["COMP_ID"].ToString(),
        //                     CARD_NO = dr["CARD_NO"].ToString(),
        //                     NAME = dr["NAME"].ToString(),
        //                     RECIV_DATE = dr["RECIV_DATE"].ToString(),
        //                     SEND_DATE = dr["SEND_DATE"].ToString(),
        //                     SERVECE_TYP = dr["SERVECE_TYP"].ToString(),
        //                     REPLY = dr["REPLY"].ToString(),
        //                     APPROV_AMOUNT = dr["APPROV_AMOUNT"].ToString(),
        //                     MEDICAL_REPLAY = dr["MEDICAL_REPLAY"].ToString(),
        //                     CREATED_BY = dr["CREATED_BY"].ToString(),
        //                     CREATED_DATE = dr["CREATED_DATE"].ToString(),
        //                 }).ToList();
        //     return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        // }
        //ServiceHistory
        public JsonResult ServiceHistory()
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt;

            string ServiceType = "112";
            cmd = new OracleCommand("select SUPER_GROUP_CODE, SUPER_GROUP_ANAME, SUPER_GROUP_ENAME from SERVICES_SUPER_GROUP where SUPER_GROUP_CODE = '" + ServiceType + "'", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            string ServiceTypeName = dt.Rows[0][2].ToString();
            cmd = new OracleCommand("select * from medical_approvals where SERVECE_TYP = '" + ServiceTypeName + "' AND active = 'Y' and rownum <=5", con);
            da = new OracleDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            var data = (from DataRow dr in dt.Rows
                        select new
                        {

                            //"APROVAL_IMAG" BLOB,
                            //"UPDATED_BY" Varchar22(70 CHAR),
                            //"UPDATED_DATE" DATE,
                            //"SUB_CODE" Int32 DEFAULT 0,
                            //"EMP_ANAME" Varchar22(100 CHAR),
                            //"" Varchar22(100 CHAR),
                            //"BIRTHDAY" DATE,
                            //"START_DATE" DATE,
                            //"END_DATE" DATE,
                            //"TOT_CONSUM" Int32,
                            //"PROVIDER_NAME" Varchar22(200 BYTE),
                            //"COMP_NAME" Varchar22(200 CHAR),
                            //"DIAG_CODE" Int32,
                            //"DIAG_NAME" Varchar22(200 BYTE),
                            //"STATUS" Varchar22(50 BYTE) DEFAULT 'YES',
                            //"STATUS2" Varchar22(50 CHAR) DEFAULT 'YES',
                            //"EXPAIRE_DATE" DATE DEFAULT sysdate,
                            //"CHANGE_PERCENT" Varchar22(100 CHAR),
                            //"ACTIVE" Varchar22(20 CHAR) DEFAULT 'Y',
                            //"RECOLLECTION_REASON" Varchar22(200 CHAR),
                            //"VISIT" Varchar22(20 CHAR)
                            // ProviderType = Convert.ToInt32(dr["prv_type"]),
                            RECIV_DATE = dr["RECIV_DATE"].ToString(),
                            SEND_DATE = dr["SEND_DATE"].ToString(),
                            APROVAL_TYP = dr["APROVAL_TYP"].ToString(),
                            PROVIDER_NUM = dr["PROVIDER_NUM"].ToString(),
                            EMAIL = dr["EMAIL"].ToString(),
                            MOBILE_NUMBER = dr["MOBILE_NUMBER"].ToString(),
                            SERVECE_TYP = dr["SERVECE_TYP"].ToString(),
                            REPLAY = dr["REPLAY"].ToString(),
                            APROVAL_VALUE = dr["APROVAL_VALUE"].ToString(),
                            MEDICAL_REPLAY = dr["MEDICAL_REPLAY"].ToString(),
                            CREATED_BY = dr["CREATED_BY"].ToString(),
                            CREATED_DATE = dr["CREATED_DATE"].ToString(),
                            CODE = dr["CODE"].ToString(),
                            COMPANY_ID = dr["COMPANY_ID"].ToString(),
                            CARD_NO = dr["CARD_NO"].ToString(),
                            EMP_ENAME = dr["EMP_ENAME"].ToString(),

                            FAX = dr["FAX"].ToString(),
                            NOTS = dr["NOTS"].ToString(),
                            VALUE_AFTER = dr["VALUE_AFTER"].ToString(),
                            PROVIDER_TYP = dr["PROVIDER_TYP"].ToString(),
                            ENDURANCE_RATIO = dr["ENDURANCE_RATIO"].ToString(),
                            MAX_AMOUNT = dr["MAX_AMOUNT"].ToString(),
                            COMP_CONTRACT_NO = dr["COMP_CONTRACT_NO"].ToString(),
                            MAX_AMOUNT_CONTRACT = dr["MAX_AMOUNT_CONTRACT"].ToString(),
                            CLASS_CODE = dr["CLASS_CODE"].ToString(),
                        }).ToList();
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ServiceMaxValueAndCeilingPert(string ServiceTypeName, string SubServiceCode, string ServiceTypeCode)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();
            //System.Data.DataTable dt = new System.Data.DataTable();

            if (ServiceTypeName != "Outpatient")
            {
                cmd = new OracleCommand("select CEILING_PERT,CEILING_AMT,SER_SERV from V_P_COMP_CUSTOMIZED_D_D where C_COMP_ID='" + Session["CompId"].ToString() + "' and CONTRACT_NO='" + Session["ContractNo"].ToString() + "' and CLASS_CODE='" + Session["ClassCode"].ToString() + "' and SER_SERV='" + SubServiceCode + "'", con);
                da = new OracleDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                // cbxindtyp1_Copy6.ItemsSource = db.RunReader("select code,reply from MEDICAL_REPLY where notes in ('" + ServiceTypeName + "','NAKDY', 'NO','Other')  order by code ").Result.DefaultView;
            }
            else if (ServiceTypeName == "Outpatient")
            {
                cmd = new OracleCommand("select CEILING_PERT,CEILING_AMT,SER_SERV from V_P_COMP_CUSTOMIZED_D_D where C_COMP_ID='" + Session["CompId"].ToString() + "' and CONTRACT_NO='" + Session["ContractNo"].ToString() + "' and CLASS_CODE='" + Session["ClassCode"].ToString() + "' and D_SERV_CODE='" + ServiceTypeCode + "'", con);
                da = new OracleDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                //  cbxindtyp1_Copy6.ItemsSource = db.RunReader("select code,reply from MEDICAL_REPLY where notes not in ('Inpatient','Optical', 'Dental')  order by code ").Result.DefaultView;
            }

            //if (ServiceTypeName == "Dental")
            //    dtooth = db.RunReader(@"select APPROVAL_SUB_SERV.S_SERV_CODE, APPROVAL_SUB_SERV.S_SERV_NAME, APPROVAL_SUB_SERV.details, MEDICAL_APPROVALS.CREATED_DATE,MEDICAL_APPROVALS.CODE from APPROVAL_SUB_SERV, MEDICAL_APPROVALS 
            //                                                where APPROVAL_SUB_SERV.code = MEDICAL_APPROVALS.code
            //                                                AND MEDICAL_APPROVALS.card_no = '" + cbxindtyp1_Copy1.Text + "' AND MEDICAL_APPROVALS.COMP_CONTRACT_NO ='" + Session["ContractNo"].ToString() + "' AND APPROVAL_SUB_SERV.S_SERV_CODE ='" + SubServiceCode + "' AND MEDICAL_APPROVALS.active = 'Y'").Result;

            if ((dt.Rows.Count == 0) || (dt.Rows.Count != 0 && dt.Rows[0][0].ToString() == string.Empty))
            {
                cmd = new OracleCommand("select CEILING_PERT,CEILING_AMT from V_P_COMP_CUSTOMIZED_D where C_COMP_ID='" + Session["CompId"].ToString() + "' and CONTRACT_NO='" + Session["ContractNo"].ToString() + "' and CLASS_CODE='" + Session["ClassCode"].ToString() + "' and D_SERV_CODE='" + ServiceTypeCode + "'", con);
                da = new OracleDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }

            //if (dt.Rows.Count != 0 && dt.Rows[0][0].ToString() != string.Empty)
            //{

            //okaprovpercnt = txtindcode1_Copy1.Text;
            //flagapproval = "YES";
            //MessageBox.Show(txtindcode1_Copy1.Text.Substring(0, txtindcode1_Copy1.Text.IndexOf(" %")));
            //ScreenApproval();
            //}
            //else
            //{
            //    Recollection.Visibility = Visibility.Visible;
            //    ApprovalGridScreen.IsEnabled = false;
            //    reqaddhrfalse_Copy3.IsEnabled = false;
            //    txtindcode1_Copy1.Text = "";
            //    txtindcode1_Copy2.Text = "";
            //}

            var service = (from DataRow dr in dt.Rows
                           select new
                           {
                               CeilingPert = dr["CEILING_PERT"].ToString(),
                               ServiceMaxValue = dr["CEILING_AMT"].ToString()
                           }).ToList();
            return new JsonResult { Data = service, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult MedicalReplay(string ServiceTypeName/*, string SubServiceCode, string ServiceTypeCode*/)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();
            //System.Data.DataTable dt = new System.Data.DataTable();

            if (ServiceTypeName != "Outpatient")
            {
                cmd = new OracleCommand("select code,reply from MEDICAL_REPLY where notes in ('" + ServiceTypeName + "','NAKDY', 'NO','Other')  order by code ", con);
                da = new OracleDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

            }
            else if (ServiceTypeName == "Outpatient")
            {
                cmd = new OracleCommand("select code,reply from MEDICAL_REPLY where notes not in ('Inpatient','Optical', 'Dental')  order by code ", con);
                da = new OracleDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

            }


            var Replay = (from DataRow dr in dt.Rows
                          select new
                          {
                              code = dr["code"].ToString(),
                              reply = dr["reply"].ToString()
                          }).ToList();
            //    data.OnlineLiveConsumption = Convert.ToInt32(dt.Rows[0][0]);
            return new JsonResult { Data = Replay, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Save(CreateApprovalViewModel Approval)
        {

            if (Approval.cod == null)
            {
                Random rnd = new Random();
                Approval.cod = DateTime.Now.ToString("ddMMyyyyhhmmss") + (rnd.Next(0, 1000000000)).ToString();
            }
            else
            {
                Approval.cod = Approval.cod + "-1";
            }
            Approval.cretby = User.Identity.Name;
            Approval.cretdat = DateTime.Now;
            Approval.clss = Session["ClassCode"].ToString();
            Approval.contr = Session["ContractNo"].ToString();

            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            if (con.State != ConnectionState.Open)
                con.Open();
            OracleCommand cmd;

            cmd = new OracleCommand(@"  INSERT INTO MEDICAL_APPROVALS (CODE, COMPANY_ID, CARD_NO, FAX, EMAIL, MOBILE_NUMBER, RECIV_DATE, SEND_DATE, APROVAL_TYP, PROVIDER_NUM, REPLAY, 
                                            MEDICAL_REPLAY, NOTS, APROVAL_VALUE, VALUE_AFTER, SERVECE_TYP, PROVIDER_TYP, ENDURANCE_RATIO, MAX_AMOUNT, APROVAL_IMAG, COMP_CONTRACT_NO, 
                                            MAX_AMOUNT_CONTRACT, CLASS_CODE, CREATED_BY, CREATED_DATE, EMP_ANAME, EMP_ENAME, BIRTHDAY, START_DATE, END_DATE, TOT_CONSUM, PROVIDER_NAME,
                                            SUB_CODE,COMP_NAME, DIAG_CODE, DIAG_NAME, STATUS, STATUS2, CHANGE_PERCENT, RECOLLECTION_REASON, VISIT)
                                            VALUES 
                                            ('" + Approval.cod + "', '" + Approval.comp.ToString() + "', '" + Approval.crd + "', '" + Approval.fx + "', '" + Approval.emil
                                        + "', '" + Approval.mob + "', :rcdat, :sedat, :apptyp, :pvdnum, :rply, :medrply, :nts, '" + Approval.appval.ToString() + "', '" + Approval.valaft.ToString() + "', :srvtyp, :pvd, :rato, :mxamun, :appimg, :contr, :mxamutcontr, :clss, :cretby, :cretdat, :anam, :enam, :birth, :strtdat, :enddat, '" + Approval.totcon.ToString() + "', :pvdnam,'" + Approval.sbcod.ToString() + "',:cmpnam,'" + Approval.digcod.ToString() + "',:dignam,:flg,:flg2, :chpcent, :rsnrecol, :vist)", con);

            //   byte[] bytes = System.Text.Encoding.UTF8.GetBytes(tim);
            cmd.Parameters.Clear();
            //    cmd.Parameters.Add(":cod", OracleDbType.Varchar2).Value = code;
            //  cmd.Parameters.Add(":comp", OracleDbType.Decimal).Value =Convert.ToInt32(Approval.comp);
            //cmd.Parameters.Add(":crd", OracleDbType.Varchar2).Value = Approval.crd;
            //cmd.Parameters.Add(":fx", OracleDbType.Varchar2).Value = Approval.fx;
            // cmd.Parameters.Add(":emil", OracleDbType.Varchar2).Value = Approval.emil;
            // cmd.Parameters.Add(":mob", OracleDbType.Decimal).Value = Convert.ToInt64(Approval.mob);
            cmd.Parameters.Add(":rcdat", OracleDbType.Date).Value = DateTime.ParseExact(Approval.rcdat, "dd-MM-yyyy", null);
            cmd.Parameters.Add(":sedat", OracleDbType.Date).Value = DateTime.ParseExact(Approval.sedat, "dd-MM-yyyy", null);
            cmd.Parameters.Add(":apptyp", OracleDbType.Varchar2).Value = Approval.apptyp;
            cmd.Parameters.Add(":pvdnum", OracleDbType.Decimal).Value = Convert.ToInt64(Approval.pvdnum);
            cmd.Parameters.Add(":rply", OracleDbType.Varchar2).Value = Approval.rply;
            cmd.Parameters.Add(":medrply", OracleDbType.Varchar2).Value = Approval.medrply;
            cmd.Parameters.Add(":nts", OracleDbType.Varchar2).Value = Approval.nts;
            // cmd.Parameters.Add(":appval", OracleDbType.Decimal).Value = Convert.ToInt32(Approval.appval);
            //  cmd.Parameters.Add(":valaft", OracleDbType.Decimal).Value = Convert.ToInt32(Approval.valaft);
            cmd.Parameters.Add(":srvtyp", OracleDbType.Varchar2).Value = Approval.srvtyp;
            cmd.Parameters.Add(":pvd", OracleDbType.Varchar2).Value = Approval.pvd;
            cmd.Parameters.Add(":rato", OracleDbType.Varchar2).Value = Approval.rato;
            cmd.Parameters.Add(":mxamun", OracleDbType.Varchar2).Value = Approval.mxamun;
            cmd.Parameters.Add(":appimg", OracleDbType.Blob).Value = System.Text.Encoding.UTF8.GetBytes(Session["FileName"].ToString());//.appimg;Approval.Image
            cmd.Parameters.Add(":contr", OracleDbType.Varchar2).Value = Approval.contr;
            cmd.Parameters.Add(":mxamutcontr", OracleDbType.Varchar2).Value = Approval.mxamutcontr;
            cmd.Parameters.Add(":clss", OracleDbType.Varchar2).Value = Approval.clss;
            cmd.Parameters.Add(":cretby", OracleDbType.Varchar2).Value = Approval.cretby;
            cmd.Parameters.Add(":cretdat", OracleDbType.Date).Value = Convert.ToDateTime(Approval.cretdat);
            cmd.Parameters.Add(":anam", OracleDbType.Varchar2).Value = Approval.anam;
            cmd.Parameters.Add(":enam", OracleDbType.Varchar2).Value = Approval.enam;
            cmd.Parameters.Add(":birth", OracleDbType.Date).Value = Convert.ToDateTime(Approval.birth);
            cmd.Parameters.Add(":strtdat", OracleDbType.Date).Value = Convert.ToDateTime(Approval.strtdat);
            cmd.Parameters.Add(":enddat", OracleDbType.Date).Value = Convert.ToDateTime(Approval.enddat);
            //cmd.Parameters.Add(":totcon", OracleDbType.Decimal).Value = Convert.ToInt32(Approval.totcon);
            //cmd.Parameters.Add(":sbcod", OracleDbType.Decimal).Value = Convert.ToInt32(Approval.sbcod);
            cmd.Parameters.Add(":pvdnam", OracleDbType.Varchar2).Value = Approval.pvdnam;
            cmd.Parameters.Add(":cmpnam", OracleDbType.Varchar2).Value = Approval.cmpnam;
            //cmd.Parameters.Add(":digcod", OracleDbType.Decimal).Value = Convert.ToInt64(Approval.digcod);
            cmd.Parameters.Add(":dignam", OracleDbType.Varchar2).Value = Approval.dignam;
            cmd.Parameters.Add(":flg", OracleDbType.Varchar2).Value = Approval.flg;
            cmd.Parameters.Add(":flg2", OracleDbType.Varchar2).Value = Approval.flg2;
            cmd.Parameters.Add(":chpcent", OracleDbType.Varchar2).Value = Approval.chpcent;
            cmd.Parameters.Add(":rsnrecol", OracleDbType.Varchar2).Value = Approval.rsnrecol;
            cmd.Parameters.Add(":vist", OracleDbType.Varchar2).Value = Approval.vist;

            int result = cmd.ExecuteNonQuery();
            con.Close();

            return new JsonResult { Data = Approval.cod, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveImage(HttpPostedFileBase ImageFile)
        {

            if (ImageFile != null)
            {

                var fileName = Path.GetFileName(ImageFile.FileName);
                var extention = Path.GetExtension(ImageFile.FileName);
                var filenamewithoutextension = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                fileName = filenamewithoutextension + DateTime.Now.ToString("yymmssfff") + extention;
                //var filenamewithoutextension = Path.GetFileNameWithoutExtension(ImageFile.FileName);

                ImageFile.SaveAs(Server.MapPath("/Content/ApprovalImages/" + fileName /*ImageFile.FileName*/));
                Session["FileName"] = "/Content/ApprovalImages/" + fileName;
            }

            return new JsonResult { Data = "r", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //Sub Approvals

        public JsonResult GetApprovals()
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();

            cmd = new OracleCommand(@"SELECT * FROM MEDICAL_APPROVALS WHERE CARD_NO = '" + Session["CardId"].ToString() + "' AND active = 'Y'", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt);

            var data = (from DataRow dr in dt.Rows
                        select new
                        {
                            // ProviderType = Convert.ToInt32(dr["prv_type"]),
                            CODE = dr["CODE"].ToString(),
                            CARD_NO = dr["CARD_NO"].ToString(),
                            //FAX = dr["FAX"].ToString(),
                            //EMAIL = dr["EMAIL"].ToString(),
                            EMP_ENAME = dr["EMP_ENAME"].ToString(),
                            //SERVECE_TYP = dr["SERVECE_TYP"].ToString(),
                            //REPLY = dr["REPLY"].ToString(),
                            APPROVAL_VALUE = dr["APROVAL_VALUE"].ToString(),
                            MEDICAL_REPLAY = dr["MEDICAL_REPLAY"].ToString(),
                            CREATED_BY = dr["CREATED_BY"].ToString(),
                            CREATED_DATE = dr["CREATED_DATE"].ToString(),
                        }).ToList();
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ApprovalData(string id)
        {

            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();
            CreateApprovalViewModel data = new CreateApprovalViewModel();
            cmd = new OracleCommand(@"select PROVIDER_TYP,PROVIDER_NAME,PROVIDER_NUM ,SERVECE_TYP,APROVAL_VALUE,
                                      ENDURANCE_RATIO,VALUE_AFTER,DIAG_NAME,FAX,EMAIL,MOBILE_NUMBER,RECIV_DATE,
                                      SEND_DATE,APROVAL_TYP,NOTS,REPLAY,MEDICAL_REPLAY,APROVAL_IMAG,CARD_NO
                                        from MEDICAL_APPROVALS
                                      WHERE CODE = '" + id + "'", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt);

            data.pvd = dt.Rows[0][0].ToString();
            data.vist = dt.Rows[0][0].ToString();//flag for anther thing
            data.pvdnam = dt.Rows[0][1].ToString();
            data.pvdnum = Convert.ToInt64(dt.Rows[0][2]);
            data.srvtyp = dt.Rows[0][3].ToString();
            data.appval = Convert.ToUInt32(dt.Rows[0][4]);
            data.rato = dt.Rows[0][5].ToString();
            data.valaft = Convert.ToUInt32(dt.Rows[0][6]);
            data.dignam = dt.Rows[0][7].ToString();
            data.fx = dt.Rows[0][8].ToString();
            data.emil = dt.Rows[0][9].ToString();
            data.mob = Convert.ToUInt32(dt.Rows[0][10]);
            data.rcdat = dt.Rows[0][11].ToString();
            data.sedat = dt.Rows[0][12].ToString();
            data.apptyp = dt.Rows[0][13].ToString();
            data.nts = dt.Rows[0][14].ToString();
            data.rply = dt.Rows[0][15].ToString();
            data.medrply = dt.Rows[0][16].ToString();
            // data.Image = dt.Rows[0][17].ToString();
            data.Image = Encoding.UTF8.GetString((byte[])dt.Rows[0][17]);
            data.crd = dt.Rows[0][18].ToString();
            DataTable dt2 = new DataTable();
            int providerType = Convert.ToInt32(data.pvd);
            cmd = new OracleCommand(@"select TYP_ENAME
                                        from PROVIDER_TYP22
                                      WHERE PRV_TYPE = " + providerType, con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt2);
            data.pvd = dt2.Rows[0][0].ToString();


            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetSubServicsApproval(string id)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();

            cmd = new OracleCommand(@"SELECT S_SERV_NAME, DETAILS, DISCRIPTION FROM APPROVAL_SUB_SERV WHERE CODE = '" + id + "' ", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt);

            var data = (from DataRow dr in dt.Rows
                        select new
                        {
                            // ProviderType = Convert.ToInt32(dr["prv_type"]),
                            SubService = dr["S_SERV_NAME"].ToString(),
                            Details = dr["DETAILS"].ToString(),
                            Discription = dr["DISCRIPTION"].ToString()

                        }).ToList();
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetSubDiagnoiseApproval(string id)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();

            cmd = new OracleCommand(@"SELECT DIAG_NAME FROM APPROVAL_DIAG WHERE CODE = '" + id + "' ", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt);

            var data = (from DataRow dr in dt.Rows
                        select new
                        {
                            // ProviderType = Convert.ToInt32(dr["prv_type"]),
                            DiagnosisName = dr["DIAG_NAME"].ToString(),

                        }).ToList();
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult DeleteApproval(string id, string reasonValue)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();


            cmd = new OracleCommand(@"SELECT * FROM Medical_Approvals WHERE CODE = '" + id + "'AND active = 'Y' order by CREATED_DATE desc ", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt);
            int result = 0;
            if (dt.Rows.Count != 0)
            {

                if (con.State != ConnectionState.Open)
                    con.Open();
                cmd = new OracleCommand(@"UPDATE MEDICAL_APPROVALS SET active = 'N', UPDATED_BY = '" + User.Identity.Name + "', UPDATED_DATE = SYSDATE WHERE CODE = '" + id + "'", con);
                cmd.ExecuteNonQuery();
                cmd = new OracleCommand(@"INSERT INTO APPROVAL_CHANGE (CODE, ACTION, REASON, UPDATED_BY, UPDATED_DATE)
                                                 VALUES('" + id + "','DELETE','" + reasonValue + "','" + User.Identity.Name + "',sysdate)", con);
                result = cmd.ExecuteNonQuery();
                con.Close();
            }
            else
            {
                result = 0;
            }

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult EditApproval(string id)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;
            OracleDataAdapter da;
            DataTable dt = new DataTable();


            cmd = new OracleCommand(@"SELECT * FROM Medical_Approvals WHERE CODE = '" + id + "'AND active = 'Y' order by CREATED_DATE desc ", con);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt);
            int result = 0;
            if (dt.Rows.Count != 0)
            {
                result = 1;

            }
            else
            {
                result = 0;
            }

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SaveEdit(CreateApprovalViewModel Approval)
        {

            Approval.cod = Approval.cod;

            Approval.cretby = User.Identity.Name;
            Approval.cretdat = DateTime.Now;
            //Approval.clss = Session["ClassCode"].ToString();
            //Approval.contr = Session["ContractNo"].ToString();

            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            if (con.State != ConnectionState.Open)
                con.Open();
            OracleCommand cmd;

            //cmd = new OracleCommand(@"  INSERT INTO MEDICAL_APPROVALS (CODE, COMPANY_ID, CARD_NO, FAX, EMAIL, MOBILE_NUMBER, RECIV_DATE, SEND_DATE, APROVAL_TYP, PROVIDER_NUM, REPLAY, 
            //                                MEDICAL_REPLAY, NOTS, APROVAL_VALUE, VALUE_AFTER, SERVECE_TYP, PROVIDER_TYP, ENDURANCE_RATIO, MAX_AMOUNT, APROVAL_IMAG, COMP_CONTRACT_NO, 
            //                                MAX_AMOUNT_CONTRACT, CLASS_CODE, CREATED_BY, CREATED_DATE, EMP_ANAME, EMP_ENAME, BIRTHDAY, START_DATE, END_DATE, TOT_CONSUM, PROVIDER_NAME,
            //                                SUB_CODE,COMP_NAME, DIAG_CODE, DIAG_NAME, STATUS, STATUS2, CHANGE_PERCENT, RECOLLECTION_REASON, VISIT)
            //                                VALUES 
            //                                ('" + Approval.cod + "', '" + Approval.comp.ToString() + "', '" + Approval.crd + "', '" + Approval.fx + "', '" + Approval.emil
            //                            + "', '" + Approval.mob + "', :rcdat, :sedat, :apptyp, :pvdnum, :rply, :medrply, :nts, '" + Approval.appval.ToString() + "', '" + Approval.valaft.ToString() + "', :srvtyp, :pvd, :rato, :mxamun, :appimg, :contr, :mxamutcontr, :clss, :cretby, :cretdat, :anam, :enam, :birth, :strtdat, :enddat, '" + Approval.totcon.ToString() + "', :pvdnam,'" + Approval.sbcod.ToString() + "',:cmpnam,'" + Approval.digcod.ToString() + "',:dignam,:flg,:flg2, :chpcent, :rsnrecol, :vist)", con);
            cmd = new OracleCommand(@"UPDATE MEDICAL_APPROVALS
                                     SET   FAX = :fx, EMAIL = :emil, MOBILE_NUMBER = :mob, 
                                            APROVAL_TYP = :apptyp, PROVIDER_NUM = :pvdnum, REPLAY = :rply,
                                            MEDICAL_REPLAY = :medrply, NOTS = :nts,
                                            APROVAL_VALUE = :appval, VALUE_AFTER = :valaft, 
                                            SERVECE_TYP = :srvtyp, PROVIDER_TYP = :pvd, ENDURANCE_RATIO = :rato,
                                            MAX_AMOUNT_CONTRACT = :mxamutcontr,  UPDATED_BY = :cretby, UPDATED_DATE = :cretdat,  
                                             PROVIDER_NAME = :pvdnam,
                                            DIAG_NAME = :dignam, STATUS = :flg, STATUS2 = :flg2,
                                            CHANGE_PERCENT = :chpcent, RECOLLECTION_REASON = :rsnrecoll, VISIT = :vist 
                                            WHERE CODE = :cod", con);
            //, APROVAL_IMAG = :appimg
            /*RECIV_DATE = :rcdat, SEND_DATE = :sedat, */
            //   byte[] bytes = System.Text.Encoding.UTF8.GetBytes(tim);
            cmd.Parameters.Clear();
            cmd.Parameters.Add(":cod", OracleDbType.Varchar2).Value = Approval.cod;
            cmd.Parameters.Add(":fx", OracleDbType.Varchar2).Value = Approval.fx;
            cmd.Parameters.Add(":emil", OracleDbType.Varchar2).Value = Approval.emil;
            cmd.Parameters.Add(":mob", OracleDbType.Decimal).Value = Convert.ToInt64(Approval.mob);
            //cmd.Parameters.Add(":rcdat", OracleDbType.Date).Value = DateTime.ParseExact(Approval.rcdat, "dd-MM-yyyy", null);
            //cmd.Parameters.Add(":sedat", OracleDbType.Date).Value = DateTime.ParseExact(Approval.sedat, "dd-MM-yyyy", null);
            cmd.Parameters.Add(":apptyp", OracleDbType.Varchar2).Value = Approval.apptyp;
            cmd.Parameters.Add(":pvdnum", OracleDbType.Decimal).Value = Convert.ToInt64(Approval.pvdnum);
            cmd.Parameters.Add(":rply", OracleDbType.Varchar2).Value = Approval.rply;
            cmd.Parameters.Add(":medrply", OracleDbType.Varchar2).Value = Approval.medrply;
            cmd.Parameters.Add(":nts", OracleDbType.Varchar2).Value = Approval.nts;
            cmd.Parameters.Add(":appval", OracleDbType.Decimal).Value = Convert.ToInt32(Approval.appval);//
            cmd.Parameters.Add(":valaft", OracleDbType.Decimal).Value = Convert.ToInt32(Approval.valaft);//
            cmd.Parameters.Add(":srvtyp", OracleDbType.Varchar2).Value = Approval.srvtyp;
            cmd.Parameters.Add(":pvd", OracleDbType.Varchar2).Value = Approval.pvd;
            cmd.Parameters.Add(":rato", OracleDbType.Varchar2).Value = Approval.rato;
            //cmd.Parameters.Add(":appimg", OracleDbType.Blob).Value = System.Text.Encoding.UTF8.GetBytes(Session["FileName"].ToString());//.appimg;Approval.Image

            cmd.Parameters.Add(":mxamutcontr", OracleDbType.Varchar2).Value = Approval.mxamutcontr;

            cmd.Parameters.Add(":cretby", OracleDbType.Varchar2).Value = Approval.cretby;
            cmd.Parameters.Add(":cretdat", OracleDbType.Date).Value = Convert.ToDateTime(Approval.cretdat);
            cmd.Parameters.Add(":pvdnam", OracleDbType.Varchar2).Value = Approval.pvdnam;
            cmd.Parameters.Add(":dignam", OracleDbType.Varchar2).Value = Approval.dignam;
            cmd.Parameters.Add(":flg", OracleDbType.Varchar2).Value = Approval.flg;
            cmd.Parameters.Add(":flg2", OracleDbType.Varchar2).Value = Approval.flg2;
            cmd.Parameters.Add(":chpcent", OracleDbType.Varchar2).Value = Approval.chpcent;
            cmd.Parameters.Add(":rsnrecol", OracleDbType.Varchar2).Value = Approval.rsnrecol;
            cmd.Parameters.Add(":vist", OracleDbType.Varchar2).Value = Approval.vist;

            int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {

                cmd = new OracleCommand(@"INSERT INTO APPROVAL_CHANGE (CODE, ACTION, REASON, UPDATED_BY, UPDATED_DATE)
                                                 VALUES('" + Approval.cod + "','Edit','" + Approval.EditReason + "','" + User.Identity.Name + "',sysdate)", con);
                result = cmd.ExecuteNonQuery();

            }
            con.Close();

            return new JsonResult { Data = Approval.cod, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveDiagnoises(List<Diagnose> Diagnoises)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;

            string ApprovalCode = Diagnoises.First().DIAG_CODE;
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd = new OracleCommand(@"DELETE FROM APPROVAL_DIAG WHERE CODE = '" + ApprovalCode + "' ", con);
            cmd.ExecuteNonQuery();
            foreach (Diagnose item in Diagnoises)
            {

                cmd = new OracleCommand(@"INSERT INTO APPROVAL_DIAG (CODE, DIAG_NAME) VALUES ('" + item.DIAG_CODE + "','" + item.DIAG_ANAME + "')", con);
                cmd.ExecuteNonQuery();
                // DIAG_CODE,  +item.code + "','"
            }
            con.Close();
            return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SaveServices(List<ApprovalServices> Services)
        {
            //connection
            OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=217.139.1.61)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369");
            OracleCommand cmd;

            string ApprovalCode = Services.First().ApprovalCode;
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd = new OracleCommand(@"DELETE FROM APPROVAL_SUB_SERV WHERE CODE = '" + ApprovalCode + "' ", con);
            cmd.ExecuteNonQuery();
            foreach (ApprovalServices item in Services)
            {
                cmd = new OracleCommand(@"INSERT INTO APPROVAL_SUB_SERV (CODE, S_SERV_NAME,S_SERV_CODE,DETAILS,DISCRIPTION) VALUES ('" + item.ApprovalCode + "','" + item.ServiceName + "','" + item.ServiceCode + "','" + item.Details + "','" + item.Description + "')", con);
                cmd.ExecuteNonQuery();
                // DIAG_CODE,  +item.code + "','"
            }
            con.Close();
            return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}