using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;


namespace DMS_TEST.ViewModel
{
    public class RoshitaCompEmolyessReportViewModel
    {
        //Roshita
        public long Id { get; set; }
        public string CardId { get; set; }
        //public string RoshetaType { get; set; }
        public int Limit { get; set; }
        public int CompanyPercent { get; set; }
        public double OverInsurance { get; set; }
        public double TotalValue { get; set; }
        public double TotalLocal { get; set; }
        public double TotalLocalDiscount { get; set; }
        public double TotalLocalDevlopment { get; set; }
        public double TotalImport { get; set; }
        public double TotalImportDiscount { get; set; }
        public double TotalImportDevelopment { get; set; }
        public double CompanyPayment { get; set; }
        public double PersonPayment { get; set; }
        public double Cash { get; set; }
        public string Manager { get; set; }
        //public string CreatedBy { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
  
        //compEmp
      
        public string EMP_ENAME { get; set; }
       

    }
}