using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;


namespace DMS_TEST.ViewModel
{
    public class RoshitaDetailsMedicineDataReportViewModel
    {

        //Roshita Details
        public long DId { get; set; }
        public string MedicienCode { get; set; }
        public string MedicienName { get; set; }
        public int Dose { get; set; }
        public int Duration { get; set; }
        public int TotalDuration { get; set; }
        public int TotalUnits { get; set; }
        public double Amount { get; set; }
        public double RealAmount { get; set; }
        public int RoshitaID { get; set; }
        public string PaymentGroup { get; set; }
        public string MedicineNoPay { get; set; }
        //-----------------------------------------------------------------------------------------
        //Medicine Data
        public int UNIT_NO { get; set; }
        public string MED_GROUP { get; set; }
        public string TRADE_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public string LIC_TYPE { get; set; }

        //public int NoPay { get; set; }
        //public int NoOver { get; set; }
        //public double CellingPert { get; set; }



    }
}