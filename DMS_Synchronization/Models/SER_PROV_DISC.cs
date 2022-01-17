using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class SER_PROV_DISC : BaseEntityDB
    {
        public int PROV_ID { get; set; }
        public string PROV_NAME { get; set; }
        public double LOC_DIS { get; set; }
        public double DEV_LOC_DIS { get; set; }
        public double IMP_DIS { get; set; }
        public double DEV_IMP_DIS { get; set; }
        public double PRV_TYPE { get; set; }
        public string TERMINATE_FLAG { get; set; }
        public DateTime? TERMINATE_DATE { get; set; }
        public string USER_CR { get; set; }
        public DateTime? USER_DT { get; set; }
        public string USER_UP { get; set; }
        public DateTime? USER_DT_UP { get; set; }
        public string MOBIL { get; set; }
        public string MOBIL2 { get; set; }
        public string PHON { get; set; }
        public string PHON2 { get; set; }
        public string MAIL { get; set; }
        public string MAIL2 { get; set; }
        public string AD_STREET { get; set; }
        public string AD_ARYA { get; set; }
        public double FREE_LIM { get; set; }
    
    }              
}                  