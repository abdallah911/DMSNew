using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class CLOSE_EMP_DATA /*: BaseEntityDB*/
    {
        public string CARD_ID { get; set; }

        public string N_CARD { get; set; }


    }
    public class COMP_EMPLOYEES_D /*: BaseEntityDB*/
    {
        public string CARD_ID { get; set; }

        public string OLD_CARD_ID { get; set; }
        public decimal? CONTRACT_NO { get; set; }


    }
}