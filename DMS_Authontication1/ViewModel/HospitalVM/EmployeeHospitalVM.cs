using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HospitalVM
{
    public class EmployeeHospitalVM
    {
        public int Id { get; set; }
        public string C_ENAME { get; set; }
        public string CARD_ID { get; set; }
        public Nullable<int> COMP_ID { get; set; }
        public Nullable<int> CONTRACT_NO { get; set; }
        public string CLASS_CODE { get; set; }
        public string EMP_ANAME { get; set; }
        public string EMP_ENAME { get; set; }
        public Nullable<System.DateTime> BIRTH_DATE { get; set; }
        
        public string TERMINATE_FLAG { get; set; }
        public Nullable<System.DateTime> TERMINATE_DATE { get; set; }
        public Nullable<int> EXP_CELLING { get; set; }
        public Nullable<int> GENDER { get; set; }
        public Nullable<System.DateTime> INS_START_DATE { get; set; }
        public Nullable<System.DateTime> INS_END_DATE { get; set; }
        
        public string ACTIVE { get; set; }
        public DateTime Now { get; set; }
        public int Provider_Level { get; set; }
        public int HOSPITAL_DEGREE { get; set; }
    }
}