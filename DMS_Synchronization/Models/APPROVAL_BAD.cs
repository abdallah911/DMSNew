using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class APPROVAL_BAD : BaseEntityDB
    {
        public decimal COMP_ID { get; set; }
        public string CARD_NO { get; set; }
        public string CLASS_CODE { get; set; }
        public int? PR_CODE { get; set; }
        public string FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public decimal? CODE { get; set; }
        public string TYPE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}