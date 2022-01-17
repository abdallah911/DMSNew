using DMS_Authontication1.Models;
using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.PharmacyAdmin
{
    public class PharmacyAdminVm
    {
       
        public List<DoctorContainerViewModel> RoshDetsils { get; set; }
        public List<PrescriptionRoshitaDignosi> RDignosis { get; set; }
        public List<RoshitaDiagnosisAdmin> OldRDignosis { get; set; }
        public string spec { get; set; }

    }
}