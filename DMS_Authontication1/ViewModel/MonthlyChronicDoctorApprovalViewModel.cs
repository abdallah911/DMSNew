using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class MonthlyChronicDoctorApprovalViewModel
    {
        //med_Card
        public string CARD_NO { get; set; }
        public string PR_ENAME { get; set; }
        public string PR_ANAME { get; set; }
        public Nullable<int> C_COMP_ID { get; set; }
        public string NOTES { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public Nullable<System.DateTime> MONTH_START_DATE { get; set; }
        public Nullable<System.DateTime> MONTH_END_DATE { get; set; }
        public Nullable<int> GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public Nullable<int> LOOK_01 { get; set; }
        public Nullable<int> SEQ { get; set; }
        public Nullable<int> PROVIDER_CODE_OLD { get; set; }
        public string TASHKHES_01 { get; set; }
        public Nullable<int> NO_PAY { get; set; }
        public Nullable<int> NO_OVER { get; set; }
        public Nullable<int> ST_DAY { get; set; }

        public string PhoneNumber { get; set; }
        public string NationalId { get; set; }
        //comp_Employees
        public string EMP_ANAME { get; set; }
        public Nullable<int> CONTRACT_NO { get; set; }
        
    }
}