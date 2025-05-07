using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.Chart
{
    public class CardDataViewModel
    {
        public string Consum { get; set; }
        public string ConsumRatio { get; set; }
        public string EmpCount { get; set; }
        public string EmpRatio { get; set; }
        public string EmpOtherCount { get; set; }
        public string EmpOtherRatio { get; set; }
        public string MaleCount { get; set; }
        public string MaleRatio { get; set; }
        public string FeMaleCount { get; set; }
        public string FeMaleRatio { get; set; }
        public string ProviderCount { get; set; }
        public string ProviderRatio { get; set; }
        //public string GroupName { get; set; }  
    }
}