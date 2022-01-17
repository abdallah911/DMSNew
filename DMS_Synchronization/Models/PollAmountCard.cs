using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class PollAmountCard : BaseEntityDB
    {
        public decimal POLL_CODE { get; set; }
        public string CARD_ID { get; set; }
        public decimal TYPE_AMOUNT { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal REMAINING { get; set; }
        public decimal SUB_CODE { get; set; }
        public string LAST_UPDATED_BY { get; set; }
        public DateTime? LAST_UPDATED_DATE { get; set; }
    }
}