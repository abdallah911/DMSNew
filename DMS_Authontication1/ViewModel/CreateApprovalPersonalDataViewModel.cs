using DMS_Authontication1.Models;
using DMS_TEST;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class CreateApprovalPersonalDataViewModel
    {
        
        public string CardId { get; set; }
     
        public int Limit { get; set; }
        public int CompanyPercent { get; set; }
        public double OverInsurance { get; set; }
        public double TotalValue { get; set; }
        public double CompanyPayment { get; set; }
        public double PersonPayment { get; set; }
        public double Cash { get; set; }
        public string Manager { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        //----------------------------------------------------------------------
        //Roshita Details
        
        public string MedicienCode { get; set; }
        public string Name { get; set; }
        
        public int Duration { get; set; }
        public int TotalDuration { get; set; }
        public int TotalUnits { get; set; }
        public int Amount { get; set; }
        public bool IsDealed { get; set; }
        public int RoshitaID { get; set; }
        public string PaymentGroup { get; set; }
        //----------------------------------------------------------------------
        //compEmp
        [DisplayName("Card ID")]
        public string CARD_ID { get; set; }
        [DisplayName("Company ID")]
        public Nullable<int> C_COMP_ID { get; set; }
        [DisplayName("Age")]
        public Nullable<int> Age { get; set; }
        
         public string CLASS_CODE { get; set; }
        [DisplayName("Name")]
        public string EMP_ANAME { get; set; }
        [DisplayName(" Name")]
        public string EMP_ENAME { get; set; }
        [DisplayName("First Name")]
        public string EMP_ANAME_ST { get; set; }
        [DisplayName("Secand Name")]
        public string EMP_ANAME_SC { get; set; }
        [DisplayName("Third Name")]
        public string EMP_ANAME_TH { get; set; }
        [DisplayName("Birth Date")]
        public Nullable<System.DateTime> BirthDate { get; set; }
        [DisplayName("Start Date")]
        public Nullable<System.DateTime> Specific_date { get; set; }
        [DisplayName("End Date")]
        public Nullable<System.DateTime> INS_END_DATE { get; set; }
        public Nullable<System.DateTime> INS_START_DATE { get; set; }
        public int ContractNo { get; set; }
        public int MaxAmount { get; set; }
        
        public int IRSConsumption { get; set; }
        
        public int OnlineConsumption { get; set; }
        public int OtherConsumption { get; set; }
        
        public string CostCenter { get; set; }
        public string CostCenterName { get; set; }
        public int OnlineLiveConsumption { get; set; }
        public int TotalApproval { get; set; }

        //-----------------------------------------------------------------------------------------
        //Medicine Data
        [DisplayName("Medicien Code")]
        public string M_CODE { get; set; }
        public string MED_GROUP { get; set; }
        public string TRADE_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public Nullable<int> PACK_SIZE { get; set; }
        public Nullable<decimal> PACK_PRICE { get; set; }
        public Nullable<int> UNIT_NO { get; set; }
        public Nullable<int> UNIT_PRICE { get; set; }
        //----------------------------------------------------------------------------------------
        //public Roshita roshita { get; set; }

        //public RoshitaDetail roshitaDetail { get; set; }
        public CompEmployees compEmp { get; set; }
        //public ContractComp contractComp { get; set; }
        //public Specialities speciality { get; set; }
        //public Diagnose diagnose { get; set; }
        //public Insurance insurance { get; set; }
        //public D_D_Emp ddEmp { get; set; }
        //public Medicin medicin { get; set; }
        //public Co_Insurance Co_insurance { get; set; }
    }
}