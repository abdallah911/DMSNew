using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class DoctorContainerViewModel
    {
        public long Id { get; set; }
        public string MedicienCode { get; set; }
        public string MedicienName { get; set; }
        public int Dose { get; set; }
        public int Duration { get; set; }
        [DisplayName("Total Duration")]
        public int TotalDuration { get; set; }
        [DisplayName("Total Units")]
        public Nullable<int> TotalUnits { get; set; }
        public double Amount { get; set; }
        public Nullable<bool> IsDealed { get; set; }

        [DisplayName("Group")]
        public string PaymentGroup { get; set; }

        public int RoshitaID { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        //Medicien
        public int COMP_ID { get; set; }
        public int BRANCH_CODE { get; set; }
        public string M_CODE { get; set; }
        public string LIC_TYPE { get; set; }
        public string MED_GROUP { get; set; }
        public string TRADE_NAME { get; set; }
        [DisplayName("Dossage Form")]
        public string DOSAGE_FORM { get; set; }
        [DisplayName("Pack Size")]
        public Nullable<double> PACK_SIZE { get; set; }
        [DisplayName("Pack Price")]
        public Nullable<double> PACK_PRICE { get; set; }
        public string M_TYPE { get; set; }
        public string CON_MED { get; set; }
        [DisplayName("Unit No")]
        public Nullable<int> UNIT_NO { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<double> UNIT_PRICE { get; set; }
        [DisplayName("Company Pay")]
        public string MedicineNoPay { get; set; }

        // public virtual Roshita Roshita { get; set; }
        //public Roshita Roshita { get; set; }
        //public RoshitaDetail RoshitaDetail { get; set; }
        // public MEDICINE_DATA MEDICINE_DATA { get; set; }
    }
}