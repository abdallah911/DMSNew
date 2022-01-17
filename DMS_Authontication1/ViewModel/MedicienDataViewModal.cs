using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class MedicienDataViewModal
    {
       
    
        public MedicienDataViewModal()
        {
            this.MedicinesDiagnosis = new HashSet<MedicinesDiagnosi>();
        }

        public int Id { get; set; }
        public int COMP_ID { get; set; }
        public int BRANCH_CODE { get; set; }
        public string M_CODE { get; set; }
        public string LIC_TYPE { get; set; }
        public string MED_GROUP { get; set; }
        public string TRADE_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public Nullable<double> PACK_SIZE { get; set; }
        public Nullable<double> PACK_PRICE { get; set; }
        public string M_TYPE { get; set; }
        public string CON_MED { get; set; }
        public Nullable<int> UNIT_NO { get; set; }
        public Nullable<double> UNIT_PRICE { get; set; }
        public string ACTIVE { get; set; }
        public string UPDATE_BY { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string GRN_CODE { get; set; }
        public Nullable<int> DiagnoiseGender { get; set; }
        public string DiagnoiseAge { get; set; }
        public Nullable<bool> IsCovered { get; set; }
        public virtual string[] Diagnose { get; set; }
        public string Group_Type { get; set; }

       public virtual ICollection<MedicinesDiagnosi> MedicinesDiagnosis { get; set; }
    }
}