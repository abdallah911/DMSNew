using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class COMP_CUSTOMIZED_D_D_EMP: BaseEntityDB
    {
        public int id { get; set; }
        public Decimal COMP_ID { get; set; }
        public Decimal BRANCH_CODE { get; set; }
        public Decimal C_COMP_ID { get; set; }
        public Decimal CONTRACT_NO { get; set; }
        public string SERV_CODE { get; set; }
        public string D_SERV_CODE { get; set; }
        public string CLASS_CODE { get; set; }
        public string SER_SERV { get; set; }
        public string CARD_ID { get; set; }
        public Decimal EMP_CODE { get; set; }
        public Decimal CEILING_AMT { get; set; }
        public Decimal CEILING_PERT { get; set; }
        public Decimal NO_DAYS { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string SER_SERV_H { get; set; }
        public string POLL_FLAG { get; set; }
        public string SERV_DET_LEV { get; set; }
        public string REQ_APPRPV { get; set; }
        public string REQ_LETTER { get; set; }
        public string IND_LIST_PRICE { get; set; }
        public Decimal REPEAT_NO { get; set; }
        public string REPEAT_TYP { get; set; }
        public Decimal REPEAT_PERIOD { get; set; }
        public Decimal CARR_AMT { get; set; }
        public string MAT_COV_TYP { get; set; }
        public string DOC_EXP { get; set; }
        public string DOC_EXP_VAL_TYP { get; set; }
        public Decimal DOC_EXP_VALUE { get; set; }
        public string REFUND_FLAG { get; set; }
        public string POLL_CONSUMPTION { get; set; }

    }
}