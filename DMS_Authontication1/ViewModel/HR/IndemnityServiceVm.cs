using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class IndemnityServiceVm
    {
        public string ServiceCardId { get; set; }
        public int ServiceId { get; set; }
        public int SpecialistId { get; set; }
        //public string ServiceName { get; set; }
        //public string SpecialistName { get; set; }
        public DateTime? ServiceDate { get; set; }
        public double Value { get; set; }
        public HttpPostedFileBase[] AttachPDF { get; set; }
    }
}