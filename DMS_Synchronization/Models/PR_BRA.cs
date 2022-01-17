using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class Pr_Bra:BaseEntityDB
    {
        public Decimal PR_CODE { get; set; }
        public Decimal COMP_ID { get; set; }
        public Decimal BRANCH_CODE { get; set; }
        public Decimal PRV_TYPE { get; set; }
        public string PR_ANAME { get; set; }
        public string PR_ENAME { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string TEL1 { get; set; }
        public string TEL2 { get; set; }
        public string FAX { get; set; }
        public string EMAIL { get; set; }
        public Decimal AREA_CODE { get; set; }
        public string PERSON1 { get; set; }
        public string PERSON2 { get; set; }
        public string PERSON1_TEL { get; set; }
        public string PERSON2_TEL { get; set; }
        public Decimal BRA_CODE { get; set; }

    }
}