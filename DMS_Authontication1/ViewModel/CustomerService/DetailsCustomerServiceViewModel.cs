using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.CustomerService
{
    public class DetailsCustomerServiceViewModel
    {
        public string ClaimNo { get; set; }
        public string PRV_NAME { get; set; }
        public string SERV_ANAME { get; set; }
        public string DIAGNOSIS { get; set; }
        public string INVT_NO { get; set; }
        public string INVT_NAM { get; set; }
        public string AMOUNT { get; set; }
        public string NOTES_DET { get; set; }
        public string DIA_CODE { get; set; }
        public string DIA_ENAME { get; set; }
        public string DIA_NOTES { get; set; }
        public string DED_CODE { get; set; }
        public string DED_DESC { get; set; }

        public string CardIdM { get; set; }
        public string MED_CODE { get; set; }
        public string MED_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public string UNIT_NO { get; set; }
        public string PACK_SIZE { get; set; }
        public string DOS_DUR { get; set; }
        public string TOT_DUR { get; set; }
        public string CON_MED { get; set; }
        public string ACTIVE { get; set; }
        public string INV_DATE { get; set; }
        public string STATE    { get; set; }


        public string DOSAGE { get; set; }
        public string PACK_PRICE { get; set; }
        public string SIZE_UNIT { get; set; }
        public string UNIT { get; set; }
        public string PRICE_UNIT { get; set; }
        public string DOSE { get; set; }
        public string DUR { get; set; }
        public string DURATION { get; set; }
        public string REPEAT { get; set; }
        public string COUNT { get; set; }
    }
}