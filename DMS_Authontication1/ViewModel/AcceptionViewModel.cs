using DMS_Authontication1.Models;
using DMS_TEST;
using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class AcceptionViewModel
    {
        public int Provider { get; set; }
        public IEnumerable<Provider> GetProvidersList { get; set; }

        public int Reasons { get; set; }
        public IEnumerable<AcceptionReason> GetReasonsList { get; set; }

        public IEnumerable<Comp_Employees> GetEmployeeList { get; set; }

        public CompEmployees compEmp { get; set; }
        public ContractComp contractComp { get; set; }
        public Specialities speciality { get; set; }
        public Diagnose diagnose { get; set; }
        public Insurance insurance { get; set; }
        public D_D_Emp ddEmp { get; set; }
        public Medicin medicin { get; set; }
        public Co_Insurance Co_insurance { get; set; }


        public int Id { get; set; }
        public int ProvidersId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string CardId { get; set; }
        public string ApprovalType { get; set; }
        public Nullable<int> COMP_EMPLOYEESID { get; set; }

        public string[] Array { get; set; }
        [Display(Name = "Reasons")]
        public string[] ReasonsList { get; set; }

        public string[] ProvidersList { get; set; }
        public double? PatientPercent { get; set; }
        public double? PatientAmount { get; set; }

    }
}