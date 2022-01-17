using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class MedicineData:BaseEntityDB
    {
        public Decimal BRANCH_CODE { get; set; }
        public Decimal COMP_ID { get; set; }
        public string M_CODE { get; set; }
        public string LIC_TYPE { get; set; }
        public string MED_GROUP { get; set; }
        public string TRADE_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public Decimal PACK_SIZE { get; set; }
        public decimal PACK_PRICE { get; set; }
        public string M_TYPE { get; set; }
        public string CON_MED { get; set; }
        public Decimal UNIT_NO { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public string ACTIVE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string GRN_CODE { get; set; }
        public Decimal DiagnoiseGender { get; set; }
        public Decimal DiagnoiseAge { get; set; }
        public bool IsCovered { get; set; }
    }
}