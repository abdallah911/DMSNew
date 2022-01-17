using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class SERVICES : BaseEntityDB
    {
        public string SERV_CODE { get; set; }
        public int COMP_ID { get; set; }
        public int BRANCH_CODE { get; set; }
        public string SERV_ANAME { get; set; }
        public string SERV_ENAME { get; set; }
        public int SERV_LEVEL { get; set; }
        public string SERV_TYP { get; set; }
        public string SERV_CODE_H { get; set; }
        public int COMP_ID_H { get; set; }
        public int BRANCH_CODE_H { get; set; }
        public string REF_FLAG { get; set; }
        public int OLD_SERV_CODE { get; set; }
        public int SERV_GROUP { get; set; }
        public string ACTIVE { get; set; }
        public string NOTES { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public int NO_SERVE { get; set; }
        public string SERV_TYPE { get; set; }
        public string AUTO_REP { get; set; }
        public string REC_FLAG { get; set; }
    }
}