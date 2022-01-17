namespace DMS_Synchronization.Models
{
    using DMS_Synchronization.BaseEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Co_Insurance_01: BaseEntityDB
    {
        // public int id { get; set; }

        public long CO_ID { get; set; }

        //[StringLength(5)]
        public string LIVEL { get; set; }

        public decimal INSURANCE_DAY { get; set; }

        public decimal INSURANCE_MONTH { get; set; }

        public string LAB_DAY { get; set; }

        public string RAY_DAY { get; set; }

        public decimal? INSURANCE_DAY_LAB { get; set; }

        public decimal? INSURANCE_MONTH_LAB { get; set; }

        public decimal? COST_MONTHLY_YEAR { get; set; }

        public decimal? COST_DALLY_YEAR { get; set; }

        public decimal? NO_CLAEM_WEEK { get; set; }

        public decimal? NO_CLEEM_YEAR { get; set; }

        public string FREE { get; set; }

        public decimal? NO_CLM_DAY_YYYY { get; set; }

        public decimal? MONY_CLM_DAY_YYYY { get; set; }

        public decimal? NO_CLM_MON_YYYY { get; set; }

        public decimal? MONY_CLM_MON_YYYY { get; set; }

        public decimal? MED_DAY { get; set; }

        public decimal? MED_MONTH { get; set; }

        //public DateTime? REG_DATE { get; set; }

    }
}
