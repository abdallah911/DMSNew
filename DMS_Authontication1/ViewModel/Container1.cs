using DMS_Authontication1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DMS_TEST.ViewModel
{
    public class Container1
    {
        public CompEmployees compEmp { get; set; }
        public ContractComp contractComp { get; set; }
        public Specialities speciality { get; set; }
        public Diagnose diagnose { get; set; }
        public Insurance insurance { get; set; }
        public D_D_Emp ddEmp { get; set; }
        public Medicin medicin { get; set; }
        public Co_Insurance Co_insurance { get; set; }
        public Approval approvals { get; set; }
    }
}