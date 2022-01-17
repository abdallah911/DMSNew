using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class Diagnosis:BaseEntityDB
    {
        public int Id { get; set; }
        public string DIAG_CODE { get; set; }
        public Decimal SPEC_ID { get; set; }
        public Decimal COMP_ID { get; set; }
        public Decimal BRANCH_CODE { get; set; }
        public string DIAG_ANAME { get; set; }
        public string DIAG_ENAME { get; set; }
        public string DIAG_CODE_H { get; set; }
        public Decimal COMP_ID_H { get; set; }
        public Decimal BRANCH_CODE_H { get; set; }
        public Decimal SERV_LEVEL { get; set; }
        public string SERV_TYP { get; set; }
        public string ACTIVE { get; set; }
        public string NOTES { get; set; }
        public string REF_FLAG { get; set; }
        public Decimal OLD_DIAG_CODE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
    }
}