namespace DMS_Synchronization.Models
{
    using DMS_Synchronization.BaseEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Contract_Comp:BaseEntityDB
    {
       
        public Decimal C_COMP_ID { get; set; }

        public Decimal COMP_ID { get; set; }

        public Decimal BRANCH_CODE { get; set; }

        
        public string C_ANAME { get; set; }

        
        public string C_ENAME { get; set; }

        
        public string ADDRESS1 { get; set; }

        
        public string ADDRESS2 { get; set; }

       
        public string TEL1 { get; set; }

        
        public string TEL2 { get; set; }

        
        public string FAX { get; set; }

       
        public string EMAIL { get; set; }

        public Decimal DOC_CODE { get; set; }

        public Decimal BROKER_CODE { get; set; }

        public Decimal BROK_PERT { get; set; }

        
        public string PAYMENT_TYP { get; set; }

        
        public string ACTIVE { get; set; }

    
        public string NOTES { get; set; }

      
        public DateTime? CREATED_DATE { get; set; }

        
        public string CREATED_BY { get; set; }

        public string UPDATE_BY { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        public string CANCEL_REASON { get; set; }

        public string SERIAL_TYP { get; set; }

        public string CHECK_TYP { get; set; }

        public string CHK_NAME { get; set; }

        public Decimal DOC_NO_DAYS { get; set; }

        public string DEPUTY_ADDRESS { get; set; }

        public string DEPUTY_JOB { get; set; }

        public DateTime? CANCEL_DATE { get; set; }

        public string BROK_PAY_TYP { get; set; }

        public string DAYS_W_M { get; set; }
    }
}
