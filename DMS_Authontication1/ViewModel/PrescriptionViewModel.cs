using DMS_Authontication1.Models;
using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class PrescriptionViewModel
    {


        //Roshita
        public long Id { get; set; }
        public string CardId { get; set; }
        public string Speciality { get; set; }
        public string Diagnose1 { get; set; }
        public string Diagnose2 { get; set; }
        public string diagnose3 { get; set; }
        public string RoshetaType { get; set; }
        public int Limit { get; set; }
        public int CompanyPercent { get; set; }
        public Nullable<double> OverInsurance { get; set; }
        public Nullable<double> TotalValue { get; set; }
        public double CompanyPayment { get; set; }
        public double PersonPayment { get; set; }
        public Nullable<double> Cash { get; set; }
        public string Manager { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> PatchId { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<double> ClaimNumber { get; set; }
        public bool hasApproval { get; set; }
        public string IsFamily { get; set; }
        public string IsPool { get; set; }

        public virtual ICollection<RoshitaDetail> roshitaDetail { get; set; }
        public virtual ICollection<Diagnose> diagnose { get; set; }


        //public Roshita roshita;
        //public List<RoshitaDetail> roshitaDetail;
        //public List<Diagnose> diagnose;
    }
}