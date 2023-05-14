//using DMS_Authontication1.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace DMS_Authontication1.Models
//{
//    public class Basic
//    {
//        DMS_TESTEntities db;
//        ApplicationDbContext UserDB;
//        public Basic()
//        {
//            db = new DMS_TESTEntities();
//            UserDB = new ApplicationDbContext();
//        }

//        public bool IsValidForAdd(PrescriptionViewModel data)
//        {
//            double PersonNoPay = 0;
//            string Message = "";
//            data.RoshetaType = data.RoshetaType == "11604" ? "11601" : data.RoshetaType;
//            int _IntServiceCode = Convert.ToInt32(data.RoshetaType);
//            string _CompId = data.CardId.Split('-')[0];
//            string MainService = data.RoshetaType.Substring(0, 3);
//            var CurrentDate = DateTime.Now.Date;
//            Comp_Employees emp = new Comp_Employees();
//            emp = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId && c.INS_START_DATE <= CurrentDate && c.INS_END_DATE >= CurrentDate).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();

//            if (data.RoshetaType == "11602")
//            {

//                string compardatestr = "05/" + DateTime.Now.ToString("MM/yyyy").ToString();
//                DateTime compardate = DateTime.ParseExact(compardatestr, "dd/MM/yyyy", null);

//                if ((emp.INS_END_DATE < compardate) && !(emp.CARD_ID.Split('-')[0].Contains("500")))
//                {
//                    var nextEmployeecontract = db.Comp_Employees.Where(c => c.CARD_ID == data.CardId).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
//                    if (nextEmployeecontract == null)
//                    {
//                        return false;

//                    }
//                    if (nextEmployeecontract.CONTRACT_NO <= emp.CONTRACT_NO)
//                    {
//                        return false;

//                    }
//                    emp = nextEmployeecontract;
//                }
//            }

//            if (emp != null)
//            {
//                double CompContractClassMAX_AMOUNT = 0;
//                double MaxServiceAmount = 0;
//                double CeilingPert;
//                double MaxSubServiceAmount;
//                bool type = false;
//                var remainingconsumption = db.RemainConsumptions.Where(x => x.CARD_ID == data.CardId && x.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
//                if (remainingconsumption != null)
//                {
//                    if (remainingconsumption.REMAINING >= 0)
//                    {
//                        CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value;
//                        type = true;
//                    }
//                    else
//                    {
//                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
//                        if (accption == null)
//                        {
//                            return false;
//                        }
//                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
//                        if (reasons != null)
//                        {
//                            CompContractClassMAX_AMOUNT = remainingconsumption.REMAINING.Value;
//                            type = true;
//                        }
//                        else
//                        {
//                            return false;
//                        }
//                    }
//                }
//                var CompContractClassEmp = db.CompContractClassEmps.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.CARD_ID == data.CardId).FirstOrDefault();
//                if (CompContractClassEmp == null)
//                {
//                    var classLimit = db.CompContractClasses.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO).FirstOrDefault();
//                    CompContractClassMAX_AMOUNT = Convert.ToDouble(classLimit.MAX_AMOUNT * 0.85);
//                }
//                else
//                {
//                    CompContractClassMAX_AMOUNT = Convert.ToDouble(CompContractClassEmp.MAX_AMOUNT * 0.85);
//                }
//                type = false;

//                var DataService1 = new Comp_Customized_D_D();
//                var DataService = db.Comp_Customized_D_D_Emp.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == data.RoshetaType && c.CARD_ID == data.CardId).FirstOrDefault();
//                if (DataService != null)
//                {
//                    var max_serv = db.COMP_CUSTOMIZED_D_EMP.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.D_SERV_CODE == MainService && c.CARD_ID == data.CardId).FirstOrDefault();
//                    MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);

//                }
//                else if (DataService == null)
//                {
//                    DataService1 = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE && c.CONTRACT_NO == emp.CONTRACT_NO && c.SER_SERV == data.RoshetaType).FirstOrDefault();
//                    if (DataService1 != null)
//                    {
//                        var max_serv = db.COMP_CUSTOMIZED_D.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CONTRACT_NO == emp.CONTRACT_NO && c.CLASS_CODE == emp.CLASS_CODE && c.D_SERV_CODE == MainService).FirstOrDefault();
//                        MaxServiceAmount = (max_serv == null || max_serv.CEILING_AMT == null) ? Convert.ToDouble(CompContractClassMAX_AMOUNT) : Convert.ToDouble(max_serv.CEILING_AMT);

//                    }

//                }
//                //Ceiling pert
//                if (DataService != null)
//                {
//                    CeilingPert = DataService.CEILING_PERT != null ? Convert.ToDouble(DataService.CEILING_PERT) : 100;
//                    MaxSubServiceAmount = (DataService.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService.CEILING_AMT);

//                }
//                else if (DataService1 != null)
//                {
//                    CeilingPert = DataService1.CEILING_PERT != null ? Convert.ToDouble(DataService1.CEILING_PERT) : 100;
//                    MaxSubServiceAmount = (DataService1.CEILING_AMT == null) ? MaxServiceAmount : Convert.ToDouble(DataService1.CEILING_AMT);
//                }
//                else
//                {
//                    Message = "هذه الخدمه غير مغطاه برجاء الرجوع للاداره الطبيه";
//                    CeilingPert = 100;
//                    MaxSubServiceAmount = 0;
//                    return false;

//                }
//                double Available = 0;
//                double Limit = 0;
//                List<Roshita> AcumlatorList = db.Roshitas.Where(r => r.CardId == data.CardId && !r.Manager.Contains("Stop") && r.Manager != "Doctor_Chronic"
//                     && r.Manager != "Doctor_Daily" && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE).ToList();
//                if (data.RoshetaType == "11602")
//                {
//                    PersonNoPay = (from roshita in db.Roshitas
//                                   join details in db.RoshitaDetails
//                                         on roshita.Id equals details.RoshitaID
//                                   where roshita.CardId == data.CardId && roshita.Manager == "Pharmacy_Chronic"
//                                   && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= emp.INS_START_DATE && roshita.CreatedDate < emp.INS_END_DATE
//                                   select new
//                                   {
//                                       Amount = details.Amount,
//                                   }).ToList().Sum(r => r.Amount);
//                }
//                //Main consumption
//                double AcumlatorAmount = 0;
//                foreach (var item in AcumlatorList)
//                {
//                    AcumlatorAmount += item.CompanyPayment;
//                }
//                AcumlatorAmount -= PersonNoPay;
//                Available = Convert.ToDouble(CompContractClassMAX_AMOUNT) - AcumlatorAmount;
//                //Service consumption
//                List<Roshita> AcumlatorServiceList = AcumlatorList.Where(r => r.RoshetaType.Contains(MainService)).ToList();
//                double AcumlatorServiceAmount = 0;
//                foreach (var item in AcumlatorServiceList)
//                {
//                    AcumlatorServiceAmount += item.CompanyPayment;
//                }
//                AcumlatorServiceAmount -= PersonNoPay;
//                double ServiceAvailable = (MaxServiceAmount - AcumlatorServiceAmount) < 0 ? 0 : MaxServiceAmount - AcumlatorServiceAmount;
//                //SubService consumption
//                List<Roshita> AcumlatorSubServiceList = AcumlatorServiceList.Where(r => r.RoshetaType == data.RoshetaType).ToList();
//                double AcumlatorSubServiceAmount = 0;
//                foreach (var item in AcumlatorSubServiceList)
//                {
//                    AcumlatorSubServiceAmount += item.CompanyPayment;
//                }
//                AcumlatorSubServiceAmount -= PersonNoPay;
//                double SubServiceAvailable = (MaxSubServiceAmount - AcumlatorSubServiceAmount) < 0 ? 0 : MaxSubServiceAmount - AcumlatorSubServiceAmount;
//                //limit
//                Limit = (Available >= ServiceAvailable) ? ServiceAvailable : Available;
//                Limit = (Limit >= SubServiceAvailable) ? SubServiceAvailable : Limit;
//                //}
//                //polling
//                if (remainingconsumption != null)
//                {
//                    Limit = (double)(remainingconsumption.REMAINING.Value < Limit ? remainingconsumption.REMAINING : Limit);
//                }
//                bool Validation = Limit > 0 ? true : false;
//                Message = Validation ? "Ok" : "لقد استهلك العميل الحد الاقصي للتغطيه خلال العقد";
//                //Message = Validation ? "Ok" : "Exceeded his annual contract limit";
//                //Co-insurance
//                double nopaylast21day = 0;
//                Co_Insurance_01 CoInsurancelimit2 = new Co_Insurance_01();
//                var CustemizedMedEmp = db.COMP_CUSTOMIZED_D_D_MED_EMP.Where(c => c.CARD_ID == emp.CARD_ID && c.C_COMP_ID == emp.C_COMP_ID
//                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService
//                  && c.SER_SERV == data.RoshetaType && c.CLASS_CODE == emp.CLASS_CODE).FirstOrDefault();
//                if (CustemizedMedEmp != null)
//                {
//                    //var CoInsurancelimit = db.Co_Insurance_01.Where(x => x.CO_ID == emp.C_COMP_ID && x.LIVEL == emp.CLASS_CODE).FirstOrDefault();
//                    string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
//                    string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
//                    DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
//                    DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);

//                    nopaylast21day = (from roshita in db.Roshitas
//                                      join details in db.RoshitaDetails
//                                            on roshita.Id equals details.RoshitaID
//                                      where roshita.CardId == data.CardId && roshita.Manager == "Pharmacy_Chronic"
//                                      && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
//                                      select new
//                                      {
//                                          Amount = details.Amount,
//                                      }).ToList().Sum(r => r.Amount);
//                    //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
//                    List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
//                    List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
//                    List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic")).ToList();
//                    List<Roshita> MonthlyMonthlyAcumlatorList = db.Roshitas.Where(x => x.CreatedDate >= Last21Time).ToList();
//                    //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
//                    bool LimitDailyPreceptionCount = false;
//                    bool LimitMonthlyPreceptionCount = false;
//                    LimitDailyPreceptionCount = (CustemizedMedEmp.DAY_NO_ROSHTA_MON == null || (CustemizedMedEmp.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
//                    LimitMonthlyPreceptionCount = (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
//                    LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
//                    LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMedEmp.MON_NO_ROSHTA_YEAR == null || (CustemizedMedEmp.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

//                    Double LimitDailyMonthlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
//                    Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

//                    Double LimitDailyYearlyPreceptionAmount = CustemizedMedEmp.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
//                    Double LimitMonthlyYearlyPreceptionAmount = CustemizedMedEmp.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMedEmp.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
//                    if (data.RoshetaType == "11601" || data.RoshetaType == "11604")
//                    {
//                        if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
//                            Limit = LimitDailyMonthlyPreceptionAmount;
//                        if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
//                            Limit = LimitDailyYearlyPreceptionAmount;
//                    }
//                    if (data.RoshetaType == "11602" || data.RoshetaType == "11603")
//                    {
//                        if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount)
//                            Limit = LimitMonthlyMonthlyPreceptionAmount;
//                        if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount)
//                            Limit = LimitMonthlyYearlyPreceptionAmount;
//                    }
//                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
//                    CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
//                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
//                    CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMedEmp.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMedEmp.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

//                    if (CoInsurancelimit2.INSURANCE_DAY < 0)
//                    {
//                        CoInsurancelimit2.INSURANCE_DAY = .001;
//                    }
//                    if (CoInsurancelimit2.INSURANCE_MONTH < 0)
//                    {
//                        CoInsurancelimit2.INSURANCE_MONTH = .001;
//                    }
//                    if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
//                        CoInsurancelimit2.INSURANCE_MONTH = .001;
//                    //approval ceiling
//                    if (Validation == false)
//                    {
//                        var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
//                        if (accption == null)
//                        {
//                            return false;
//                        }
//                        var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
//                        if (reasons != null)
//                        {
//                            return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert });
//                        }

//                    }
//                    //return Json(new { ok = true, limit = limit, message = "ok", LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount }, JsonRequestBehavior.AllowGet);
//                    return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount });

//                }
//                else
//                {
//                    var CustemizedMed = db.COMP_CUSTOMIZED_D_D_MED.Where(c => c.C_COMP_ID == emp.C_COMP_ID && c.CLASS_CODE == emp.CLASS_CODE
//                  && c.CONTRACT_NO == emp.CONTRACT_NO && c.SERV_CODE == "11" && c.D_SERV_CODE == MainService && c.SER_SERV == data.RoshetaType).FirstOrDefault();
//                    if (CustemizedMed == null)
//                    {
//                        return false;
//                    }
//                    else
//                    {

//                        string Last21 = "21/" + ((DateTime.Now.Day >= 21) ? DateTime.Now.ToString("MM/yyyy") : DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToString();
//                        string firstDayOfMonth = ("01/" + DateTime.Now.ToString("MM/yyyy")).ToString();
//                        DateTime Last21Time = DateTime.ParseExact(Last21, "dd/MM/yyyy", null);
//                        DateTime firstDayOfMonthTime = DateTime.ParseExact(firstDayOfMonth, "dd/MM/yyyy", null);
//                        nopaylast21day = (from roshita in db.Roshitas
//                                          join details in db.RoshitaDetails
//                                                on roshita.Id equals details.RoshitaID
//                                          where roshita.CardId == data.CardId && roshita.Manager == "Pharmacy_Chronic"
//                                          && details.MedicineNoPay == "Yes" && roshita.CreatedDate >= Last21Time
//                                          select new
//                                          {
//                                              Amount = details.Amount,
//                                          }).ToList().Sum(r => r.Amount);
//                        //List<Roshita> MainAcumlatorList = db.Roshitas.Where(r => r.CardId == id && !r.Manager.Contains("Stop") && r.CreatedDate >= emp.INS_START_DATE && r.CreatedDate < emp.INS_END_DATE && r.Manager != "Doctor_Chronic").ToList();
//                        List<Roshita> YearlyDailyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Daily" || x.Manager == "Pharmacy_Doctor").ToList();
//                        List<Roshita> YearlyMonthlyAcumlatorList = AcumlatorList.Where(x => x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic").ToList();
//                        List<Roshita> MonthlyDailyAcumlatorList = YearlyDailyAcumlatorList.Where(x => x.CreatedDate >= firstDayOfMonthTime).ToList();
//                        List<Roshita> MonthlyMonthlyAcumlatorList = db.Roshitas.Where(x => x.CardId == data.CardId && x.CreatedDate >= Last21Time && (x.Manager == "Monthly" || x.Manager == "Pharmacy_Chronic")).ToList();
//                        //List<Roshita> MonthlyMonthlyAcumlatorList = YearlyMonthlyAcumlatorList.Where(x => x.CreatedDate >= Last21Time).ToList();
//                        bool LimitDailyPreceptionCount = false;
//                        bool LimitMonthlyPreceptionCount = false;
//                        LimitDailyPreceptionCount = (CustemizedMed.DAY_NO_ROSHTA_MON == null || (CustemizedMed.DAY_NO_ROSHTA_MON - MonthlyDailyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Daily count
//                        LimitMonthlyPreceptionCount = (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - MonthlyMonthlyAcumlatorList.Count() > 0)) ? true : false;//Monthly&Monthly count
//                        LimitDailyPreceptionCount = (LimitDailyPreceptionCount && (CustemizedMed.DAY_NO_ROSHTA_YEAR == null || (CustemizedMed.DAY_NO_ROSHTA_YEAR - YearlyDailyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Daily count
//                        LimitMonthlyPreceptionCount = (LimitMonthlyPreceptionCount && (CustemizedMed.MON_NO_ROSHTA_YEAR == null || (CustemizedMed.MON_NO_ROSHTA_YEAR - YearlyMonthlyAcumlatorList.Count() > 0))) ? true : false;//Yearly&Monthly count

//                        Double LimitDailyMonthlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_MON - (MonthlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Daily Amount
//                        Double LimitMonthlyMonthlyPreceptionAmount = CustemizedMed.MON_MED_AMT_MON == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_MON - (MonthlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (MonthlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - nopaylast21day)));//Monthly&Monthly Amount

//                        Double LimitDailyYearlyPreceptionAmount = CustemizedMed.DAY_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.DAY_MED_AMT_YEAR - (YearlyDailyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyDailyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Daily Amount
//                        Double LimitMonthlyYearlyPreceptionAmount = CustemizedMed.MON_MED_AMT_YEAR == null ? Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR) : (Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)))) == 0 ? .001 : Convert.ToDouble(CustemizedMed.MON_MED_AMT_YEAR - (YearlyMonthlyAcumlatorList.Sum(x => x.PersonPayment) + (YearlyMonthlyAcumlatorList.Sum(x => x.CompanyPayment) - PersonNoPay)));//Yearly&Monthly Amount
//                        if (data.RoshetaType == "11601" || data.RoshetaType == "11604")
//                        {
//                            if (LimitDailyMonthlyPreceptionAmount != 0 && Limit > LimitDailyMonthlyPreceptionAmount)
//                                Limit = LimitDailyMonthlyPreceptionAmount;
//                            if (LimitDailyYearlyPreceptionAmount != 0 && Limit > LimitDailyYearlyPreceptionAmount)
//                                Limit = LimitDailyYearlyPreceptionAmount;
//                        }
//                        if (data.RoshetaType == "11602" || data.RoshetaType == "11603")
//                        {
//                            if (LimitMonthlyMonthlyPreceptionAmount != 0 && Limit > LimitMonthlyMonthlyPreceptionAmount)
//                                Limit = LimitMonthlyMonthlyPreceptionAmount;
//                            if (LimitMonthlyYearlyPreceptionAmount != 0 && Limit > LimitMonthlyYearlyPreceptionAmount)
//                                Limit = LimitMonthlyYearlyPreceptionAmount;
//                        }
//                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitDailyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitDailyMonthlyPreceptionAmount), Convert.ToDouble(LimitDailyYearlyPreceptionAmount));
//                        CoInsurancelimit2.INSURANCE_DAY = LimitDailyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.DAY_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.DAY_AMT), Convert.ToDouble(LimitDailyMonthlyPreceptionAmount));
//                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyYearlyPreceptionAmount == 0 ? Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount) : Math.Min(Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount), Convert.ToDouble(LimitMonthlyYearlyPreceptionAmount));
//                        CoInsurancelimit2.INSURANCE_MONTH = LimitMonthlyMonthlyPreceptionAmount == 0 ? Convert.ToDouble(CustemizedMed.MON_AMT) : Math.Min(Convert.ToDouble(CustemizedMed.MON_AMT), Convert.ToDouble(LimitMonthlyMonthlyPreceptionAmount));

//                        if (CoInsurancelimit2.INSURANCE_DAY < 0)
//                        {
//                            CoInsurancelimit2.INSURANCE_DAY = .001;
//                        }
//                        if (CoInsurancelimit2.INSURANCE_MONTH < 0)
//                        {
//                            CoInsurancelimit2.INSURANCE_MONTH = .001;
//                        }

//                        if (CoInsurancelimit2.INSURANCE_DAY == null)
//                        {
//                            CoInsurancelimit2.INSURANCE_DAY = 0;
//                        }
//                        if (CoInsurancelimit2.INSURANCE_MONTH == null)
//                        {
//                            CoInsurancelimit2.INSURANCE_MONTH = 0;
//                        }
//                        if (LimitMonthlyYearlyPreceptionAmount < 0 && CoInsurancelimit2.INSURANCE_MONTH == 0)
//                            CoInsurancelimit2.INSURANCE_MONTH = .001;
//                        //approval ceiling
//                        if (Validation == false)
//                        {
//                            var accption = db.Acceptions.Where(x => x.CompEmployeesId == emp.Id && x.AcceptionFlag == true).OrderByDescending(d => d.Id).FirstOrDefault();
//                            if (accption == null)
//                            {
//                                return false;
//                            }
//                            var reasons = db.CardAcceptionReasons.Where(x => x.AcceptionId == accption.Id && x.AcceptionReason.Name == "Disregard Ceiling").FirstOrDefault();
//                            if (reasons != null)
//                            {
//                                return Json(new { Validation = true, Message = "Has Approval", Limit = ".001", CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount, CeilingPert = CeilingPert });
//                            }

//                        }
//                        return Json(new { Validation = Validation, Message = Message, Limit = Limit, CeilingPert = CeilingPert, CoInsurancelimit = CoInsurancelimit2, LimitDailyPreceptionCount = LimitDailyPreceptionCount, LimitMonthlyPreceptionCount = LimitMonthlyPreceptionCount });

//                    }
//                }

//            }
//            else
//            {
//                return false;

//            }
//        }
//    }
//}