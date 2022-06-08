using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class REMAIN_CONSUMATION : BaseEntityDB
    {
        public string CARD_ID { get; set; }
        public int CONTRACT_NO { get; set; }
        public double MAX_AMOUNT { get; set; }
        public double NET { get; set; }
        public decimal REMAINING { get; set; }
        
    }
}